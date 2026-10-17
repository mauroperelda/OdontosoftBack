using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OdontoSoft.API.Filters;
using OdontoSoft.API.Helpers;
using OdontoSoft.API.Mapper;
using OdontoSoft.API.Validators;
using OdontoSoft.Data.Authentication;
using Serilog;
using Serilog.Formatting.Compact;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using OdontoSoft.Data.DAL;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Services;

// Restauramos el comportamiento legacy de Npgsql pre-6.0: permite escribir DateTime
// con Kind=Utc/Unspecified/Local en columnas "timestamp without time zone" sin
// lanzar excepción. Sin esto, los SaveChanges que usan DateTime.UtcNow (auditoría)
// fallan al hacer INSERT contra columnas de tipo timestamp. Debe setearse ANTES
// de cualquier llamada a Npgsql.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// ── Serilog (configurado ANTES del WebApplicationBuilder para que capture
//    cualquier log emitido durante el startup) ───────────────────────────────
Log.Logger = new LoggerConfiguration()
  .ReadFrom.Configuration(new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables()
    .Build())
  .Enrich.FromLogContext()
  .WriteTo.Console(new CompactJsonFormatter())
  .CreateLogger();

try
{
  var builder = WebApplication.CreateBuilder(args);
  builder.Host.UseSerilog();

  // ── Validación temprana de configuración crítica ─────────────────────────────
  // Falla ruidosamente al arranque si Jwt:Key o la cadena de conexión están
  // ausentes o son inválidas. Mejor un crash limpio que un HMAC con clave vacía
  // (que pasa validación pero firma tokens inválidos).
  ValidateCriticalConfig(builder.Configuration);

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
  builder.Services.AddScoped<IPresupuestoServices, PresupuestoServices>();
  builder.Services.AddScoped<ITurnoServices, TurnoServices>();
  builder.Services.AddScoped<IPacienteServices, PacienteServices>();
  builder.Services.AddScoped<IPresupuestoDetalleServices, PresupuestoDetalleServices>();
  builder.Services.AddScoped<IHistoriaClinicaServices, HistoriaClinicaServices>();
  builder.Services.AddScoped<IOdontogramaPiezaServices, OdontogramaPiezaServices>();
  builder.Services.AddScoped<ITratamientoServices, TratamientoServices>();
  builder.Services.AddScoped<IPeriodontogramaServices, PeriodontogramaServices>();

  // ── Auth JWT (comparte Key/Issuer/Audience con ApiGetToken) ──────────────────
  var jwtKey = builder.Configuration["Jwt:Key"]!;
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
        // 30s de skew para tolerar desincronización leve entre el emisor
        // (ApiGetToken) y este servicio. Antes era 0, lo que provocaba
        // 401s espurios cuando el reloj se desviaba.
        ClockSkew = TimeSpan.FromSeconds(30),
      };

      // Diagnóstico: a nivel Debug para no spammear en producción.
      options.IncludeErrorDetails = builder.Environment.IsDevelopment();
      options.Events = new JwtBearerEvents
      {
        OnMessageReceived = ctx =>
        {
          var logger = ctx.HttpContext.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("JwtBearer");
          var hdr = ctx.Request.Headers.Authorization.ToString();
          logger.LogDebug("[JWT] Header recibido: {Length} chars", hdr.Length);
          return Task.CompletedTask;
        },
        OnAuthenticationFailed = ctx =>
        {
          var logger = ctx.HttpContext.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("JwtBearer");
          logger.LogWarning(ctx.Exception, "[JWT] Auth failed: {Type}", ctx.Exception?.GetType().Name);
          return Task.CompletedTask;
        },
        OnTokenValidated = ctx =>
        {
          var logger = ctx.HttpContext.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("JwtBearer");
          logger.LogDebug("[JWT] Token OK para {Name}", ctx.Principal?.Identity?.Name ?? "?");
          return Task.CompletedTask;
        },
        OnChallenge = ctx =>
        {
          var logger = ctx.HttpContext.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("JwtBearer");
          logger.LogWarning("[JWT] Challenge: error='{Error}' desc='{Desc}'", ctx.Error, ctx.ErrorDescription);
          return Task.CompletedTask;
        },
      };
    });
  builder.Services.AddAuthorization();

  // ── Controllers + JSON (camelCase, sin ciclos, ignora nulls al escribir) ─────
  // El FluentValidationFilter corre los IValidator<T> registrados en DI contra
  // los argumentos de cada acción y vuelca los errores al ModelState. Después,
  // el comportamiento automático de [ApiController] corta si ModelState es
  // inválido y el InvalidModelStateResponseFactory los formatea.
  builder.Services.AddScoped<FluentValidationFilter>();
  builder.Services.AddControllers(options =>
    {
      options.Filters.AddService<FluentValidationFilter>();
    })
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

  // Registra todos los IValidator<T> de este assembly (los 10 que viven en
  // OdontoSoft.API/Validators/) como transient en DI. La primera línea ya
  // escanea todo el assembly; las siguientes son explícitas para documentar
  // qué VMs tienen validator y tener un grep-friendly listado.
  builder.Services.AddValidatorsFromAssemblyContaining<PacienteDtoValidator>();
  builder.Services.AddValidatorsFromAssemblyContaining<ProfesionalDtoValidator>();
  builder.Services.AddValidatorsFromAssemblyContaining<PresupuestoDtoValidator>();
  builder.Services.AddValidatorsFromAssemblyContaining<PresupuestoDetalleDtoValidator>();
  builder.Services.AddValidatorsFromAssemblyContaining<PagoDtoValidator>();
  builder.Services.AddValidatorsFromAssemblyContaining<TurnoDtoValidator>();
  builder.Services.AddValidatorsFromAssemblyContaining<HistoriaClinicaDtoValidator>();
  builder.Services.AddValidatorsFromAssemblyContaining<OdontogramaPiezaDtoValidator>();
  builder.Services.AddValidatorsFromAssemblyContaining<FichaTratamientoDtoValidator>();
  builder.Services.AddValidatorsFromAssemblyContaining<UbicacionDtoValidator>();

  // AutoMapper: escanea el assembly buscando clases que hereden de Profile
  // y registra los mappings en un IMapper injectable.
  builder.Services.AddAutoMapper(typeof(ApplicationProfile));

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

  // ── CORS (lista blanca por ambiente) ─────────────────────────────────────────
  // Origins permitidos vienen de configuración. En Development suele ser el front
  // local (Vite/React). En Production, el dominio del front deployado.
  var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? (builder.Environment.IsDevelopment()
      ? new[] { "http://localhost:5173", "http://localhost:3000" }
      : Array.Empty<string>());

  builder.Services.AddCors(options =>
  {
    options.AddPolicy("ReactFrontend", policy =>
    {
      if (allowedOrigins.Length == 0)
      {
        // Fallback permisivo solo si no se configuró nada (no debería pasar).
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
      }
      else
      {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
      }
    });
  });

  // ── Form options (multipart upload de imágenes de pacientes) ───────────────────
  builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
  {
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
    options.ValueLengthLimit = 10 * 1024 * 1024;
  });

  // ── OpenAPI nativo + Scalar UI ────────────────────────────────────────────────
  // ── Swagger / OpenAPI ─────────────────────────────────────────────────────────
  // Genera el documento OpenAPI en /swagger/v1/swagger.json y la UI clásica de
  // Swagger en /swagger. Habilitado siempre (Development y Production) porque
  // las rutas no son sensibles y el equipo de front lo usa como referencia.
  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen(options =>
  {
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
      Title = "OdontoSoft API",
      Version = "v1",
      Description = "API REST del consultorio odontológico. Todos los endpoints requieren JWT Bearer excepto /api/Account/Login.",
    });

    // Definición del esquema "Bearer" → habilita el botón "Authorize" en Swagger UI
    // para pegar el JWT y que se envíe en cada request (header Authorization: Bearer …).
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
      Description = "JWT emitido por ApiGetToken. Pegá SOLO el token (sin el prefijo 'Bearer ').",
      Name = "Authorization",
      In = Microsoft.OpenApi.Models.ParameterLocation.Header,
      Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
      Scheme = "bearer",
      BearerFormat = "JWT",
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
      {
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
          Reference = new Microsoft.OpenApi.Models.OpenApiReference
          {
            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
            Id = "Bearer",
          },
        },
        Array.Empty<string>()
      },
    });
  });

  // ── Health checks (Fase 5.4) ──────────────────────────────────────────────────
  // Liveness: ¿el proceso está vivo? (sin tocar DB)
  // Readiness: ¿está listo para recibir tráfico? (incluye el ping a PostgreSQL)
  builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: new[] { "live" })
    .AddNpgSql(connString, tags: new[] { "ready" });

  // ── Rate limiting nativo (Fase 5.5) ───────────────────────────────────────────
  // Ventana fija de 1 minuto, 100 requests por IP. Cubre el caso "alguien se
  // loguea 1000 veces por segundo tratando de adivinar credenciales" sin
  // agregar una dependencia externa. Endpoints sensibles (login, upload de
  // imágenes) lo aplican con [EnableRateLimiting("api")].
  builder.Services.AddRateLimiter(options =>
  {
    options.AddFixedWindowLimiter("api", opt =>
    {
      opt.Window = TimeSpan.FromMinutes(1);
      opt.PermitLimit = 100;
      opt.QueueLimit = 0;
    });
  });

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

  // Diagnóstico de headers: solo en Development.
  if (app.Environment.IsDevelopment())
  {
    app.Use(async (ctx, next) =>
    {
      if (ctx.Request.Path.StartsWithSegments("/api"))
      {
        var logger = ctx.RequestServices.GetRequiredService<ILoggerFactory>()
          .CreateLogger("RequestDiagnostics");
        logger.LogDebug("{Method} {Path}{Query}", ctx.Request.Method, ctx.Request.Path, ctx.Request.QueryString);
        foreach (var h in ctx.Request.Headers)
        {
          var value = h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase)
            ? $"<{h.Value.ToString().Length} chars>"
            : h.Value.ToString();
          logger.LogDebug("  {Key}: {Value}", h.Key, value);
        }
      }
      await next();
    });
  }

  app.UseAuthentication();
  app.UseAuthorization();
  app.UseRateLimiter();

  // Middleware global: traduce cualquier excepción no controlada a la shape
  // uniforme { error, errors } con el status code apropiado.
  app.UseMiddleware<GlobalExceptionMiddleware>();

  // Captura errores de deserialización JSON y los reescribe como
  // { error, errors: { Field: [...] } } que el front sabe interpretar.
  // Se registra DESPUÉS del global para que las JsonException específicas
  // no caigan en el fallback genérico.
  app.UseMiddleware<JsonExceptionMiddleware>();

  app.MapControllers();

  app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
  {
    Predicate = check => check.Tags.Contains("live"),
  });
  app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
  {
    Predicate = check => check.Tags.Contains("ready"),
  });

  app.Run();
}
catch (Exception ex)
{
  Log.Fatal(ex, "La aplicación falló al arrancar.");
}
finally
{
  Log.CloseAndFlush();
}

// ── Helpers ───────────────────────────────────────────────────────────────────
static void ValidateCriticalConfig(IConfiguration config)
{
  var jwtKey = config["Jwt:Key"];
  if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("Falta Jwt:Key en la configuración.");
  if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException(
      $"Jwt:Key debe tener al menos 32 bytes (256 bits) para HS256. Tiene {Encoding.UTF8.GetByteCount(jwtKey)}.");

  var connString = config.GetConnectionString("DefaultConnection");
  if (string.IsNullOrWhiteSpace(connString))
    throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection en la configuración.");
}
