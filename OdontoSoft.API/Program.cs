using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ExtraCore.Web.Helpers.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OdontoSoft.API.Helpers;
using OdontoSoft.Data.Authentication;
using OdontoSoft.Data.DAL;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Services;

// Restauramos el comportamiento legacy de Npgsql pre-6.0: permite escribir DateTime
// con Kind=Utc/Unspecified/Local en columnas "timestamp without time zone" sin
// lanzar excepción. Sin esto, los SaveChanges que usan DateTime.UtcNow (auditoría)
// fallan al hacer INSERT contra columnas de tipo timestamp. Debe setearse ANTES
// de cualquier llamada a Npgsql.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// ── Base de Datos ─────────────────────────────────────────────────────────────
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<OdontosoftContext>(options =>
{
    // Convención de naming: "Paciente" → tabla "paciente", "CreatedUserId" → "created_user_id".
    // PostgreSQL es case-sensitive con identifiers quoted; snake_case lowercase es idiomático
    // y nos ahorra quoted identifiers en queries raw.
    options.UseSnakeCaseNamingConvention();
    // Las migrations viven en OdontoSoft.Data (no en OdontoSoft.Web) para que
    // el data project sea self-contained y no dependa del host legacy.
    options.UseNpgsql(connString, b => b.MigrationsAssembly("OdontoSoft.Data"));
});

// ── Repositorio / UnitOfWork ───────────────────────────────────────────────────
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ── Servicios de dominio ───────────────────────────────────────────────────────
builder.Services.AddHttpClient<ILocalidadServices, LocalidadServices>();

// ── Auth JWT (comparte Key/Issuer/Audience con ApiGetToken) ──────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
             ?? throw new InvalidOperationException("Falta Jwt:Key en appsettings.json");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "https://localhost:7229";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "https://localhost:7229";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero,
        };

        // Diagnóstico: imprime el motivo real del 401 en la consola del API
        options.IncludeErrorDetails = true;
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var hdr = ctx.Request.Headers.Authorization.ToString();
                Console.WriteLine($"[JWT] Header recibido: '{hdr}' (len={hdr.Length})");
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = ctx =>
            {
                Console.Error.WriteLine($"[JWT] Auth failed: {ctx.Exception?.GetType().Name}: {ctx.Exception?.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                Console.WriteLine($"[JWT] Token OK para {ctx.Principal?.Identity?.Name ?? "?"}");
                return Task.CompletedTask;
            },
            OnChallenge = ctx =>
            {
                Console.Error.WriteLine($"[JWT] Challenge: error='{ctx.Error}' desc='{ctx.ErrorDescription}'");
                return Task.CompletedTask;
            },
        };
    });
builder.Services.AddAuthorization();

// ── Controllers + JSON (camelCase, sin ciclos, ignora nulls al escribir) ─────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        // Acepta "" como null en int?, decimal?, DateTime?, etc. y devuelve
        // mensajes en español cuando el valor es inválido.
        options.JsonSerializerOptions.Converters.Add(new LenientNullableJsonConverter());
    });

// ── Formato uniforme de errores de validación ─────────────────────────────────
// Cuando [ApiController] detecta ModelState inválido, en vez del ProblemDetails
// por defecto, devolvemos { error, errors } que es lo que el front espera.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = new Dictionary<string, string[]>();
        foreach (var (key, entry) in context.ModelState)
        {
            if (entry is null || entry.Errors.Count == 0) continue;
            // Sacamos el prefijo "$.data." o el nombre del parámetro que
            // MVC agrega a las keys de ModelState para binding de body.
            var field = ModelStateKeys.Strip(key);
            var messages = entry.Errors
                .Select(e => e.Exception?.Message ?? e.ErrorMessage)
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .ToArray();
            if (messages.Length > 0) errors[field] = messages;
        }

        var summary = errors.Count == 0
            ? "Los datos enviados no son válidos."
            : "Revisá los campos marcados.";

        return new BadRequestObjectResult(new { error = summary, errors });
    };
});

// ── CORS (permite peticiones desde el front React) ────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ── Form options (multipart upload de imágenes de pacientes) ───────────────────
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
    options.ValueLengthLimit = 10 * 1024 * 1024;
});

// ── Swagger / OpenAPI ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthenticatedUser, AuthenticatedUser>();

var app = builder.Build();

// ── EF Migrations automáticas al iniciar ─────────────────────────────────────
// Aplica las migrations pendientes en cada startup. Idempotente: si no hay
// nada nuevo, no hace nada. Si falla (DB no accesible, migration corrupta),
// la API no arranca — Render detecta el crash y deja el deploy viejo
// sirviendo en lugar del nuevo.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OdontosoftContext>();
    var pending = db.Database.GetPendingMigrations().ToList();
    if (pending.Count > 0)
    {
        app.Logger.LogInformation(
            "Aplicando {Count} migrations pendientes: {Names}",
            pending.Count, string.Join(", ", pending));
        db.Database.Migrate();
    }
}

// ── Pipeline HTTP ─────────────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();
app.UseCors("ReactFrontend");

// Diagnóstico: loggea TODOS los headers raw que llegan al API, ANTES de JWT
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/api"))
    {
        Console.WriteLine($"[RAW] {ctx.Request.Method} {ctx.Request.Path}{ctx.Request.QueryString}");
        foreach (var h in ctx.Request.Headers)
        {
            var value = h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase)
                ? $"<{h.Value.ToString().Length} chars>"
                : h.Value.ToString();
            Console.WriteLine($"[RAW]   {h.Key}: {value}");
        }
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

// Captura errores de deserialización JSON y los reescribe como
// { error, errors: { Field: [...] } } que el front sabe interpretar.
app.UseMiddleware<JsonExceptionMiddleware>();

app.MapControllers();

app.Run();
