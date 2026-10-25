using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OdontoSoft.Application.DependencyInjection;
using OdontoSoft.Infrastructure.DAL;
using OdontoSoft.Infrastructure.DependencyInjection;
using OdontoSoft.Infrastructure.DependencyInjection;
using OdontoSoft.Infrastructure.Filters;
using OdontoSoft.Infrastructure.Http;
using OdontoSoft.Infrastructure.Middleware;
using Serilog;
using Serilog.Formatting.Compact;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

// Restauramos el comportamiento legacy de Npgsql pre-6.0: permite escribir DateTime
// con Kind=Utc/Unspecified/Local en columnas "timestamp without time zone" sin
// lanzar excepciÃ³n. Sin esto, los SaveChanges que usan DateTime.UtcNow (auditorÃ­a)
// fallan al hacer INSERT contra columnas de tipo timestamp. Debe setearse ANTES
// de cualquier llamada a Npgsql.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// -- Serilog (configurado ANTES del WebApplicationBuilder para que capture
//    cualquier log emitido durante el startup) -------------------------------
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

  // -- ValidaciÃ³n temprana de configuraciÃ³n crÃ­tica -----------------------------
  // Falla ruidosamente al arranque si Jwt:Key o la cadena de conexiÃ³n estÃ¡n
  // ausentes o son invÃ¡lidas. Mejor un crash limpio que un HMAC con clave vacÃ­a
  // (que pasa validaciÃ³n pero firma tokens invÃ¡lidos).
  ValidateCriticalConfig(builder.Configuration);

  // -- DI: una lÃ­nea por capa. El detalle vive en cada helper de extensiÃ³n. ---
  builder.Services.AddOdontoSoftData(builder.Configuration);
  builder.Services.AddOdontoSoftApplication();
  builder.Services.AddOdontoSoftWeb();

  // -- Auth JWT (comparte Key/Issuer/Audience con ApiGetToken) ------------------
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
        // 30s de skew para tolerar desincronizaciÃ³n leve entre el emisor
        // (ApiGetToken) y este servicio. Antes era 0, lo que provocaba
        // 401s espurios cuando el reloj se desviaba.
        ClockSkew = TimeSpan.FromSeconds(30),
      };

      // DiagnÃ³stico: a nivel Debug para no spammear en producciÃ³n.
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

  // -- CORS (lista blanca por ambiente) -----------------------------------------
  // Origins permitidos vienen de configuraciÃ³n. En Development suele ser el front
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
        // Fallback permisivo solo si no se configurÃ³ nada (no deberÃ­a pasar).
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

  // -- Form options (multipart upload de imÃ¡genes de pacientes) -------------------
  builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
  {
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
    options.ValueLengthLimit = 10 * 1024 * 1024;
  });

  // -- OpenAPI nativo + Scalar UI ------------------------------------------------
  // -- Swagger / OpenAPI ---------------------------------------------------------
  // Genera el documento OpenAPI en /swagger/v1/swagger.json y la UI clÃ¡sica de
  // Swagger en /swagger. Habilitado siempre (Development y Production) porque
  // las rutas no son sensibles y el equipo de front lo usa como referencia.
  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen(options =>
  {
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
      Title = "OdontoSoft API",
      Version = "v1",
      Description = "API REST del consultorio odontolÃ³gico. Todos los endpoints requieren JWT Bearer excepto /api/Account/Login.",
    });

    // DefiniciÃ³n del esquema "Bearer" â€” habilita el botÃ³n "Authorize" en Swagger UI
    // para pegar el JWT y que se envÃ­e en cada request (header Authorization: Bearer â€¦).
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
      Description = "JWT emitido por ApiGetToken. PegÃ¡ SOLO el token (sin el prefijo 'Bearer ').",
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

  // -- Health checks (Fase 5.4) --------------------------------------------------
  // Liveness: Â¿el proceso estÃ¡ vivo? (sin tocar DB)
  // Readiness: Â¿estÃ¡ listo para recibir trÃ¡fico? (incluye el ping a PostgreSQL)
  var connString = builder.Configuration.GetConnectionString("DefaultConnection");
  builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: new[] { "live" })
    .AddNpgSql(connString, tags: new[] { "ready" });

  // -- Rate limiting nativo (Fase 5.5) -------------------------------------------
  // Ventana fija de 1 minuto, 100 requests por IP. Cubre el caso "alguien se
  // loguea 1000 veces por segundo tratando de adivinar credenciales" sin
  // agregar una dependencia externa. Endpoints sensibles (login, upload de
  // imÃ¡genes) lo aplican con [EnableRateLimiting("api")].
  builder.Services.AddRateLimiter(options =>
  {
    options.AddFixedWindowLimiter("api", opt =>
    {
      opt.Window = TimeSpan.FromMinutes(1);
      opt.PermitLimit = 100;
      opt.QueueLimit = 0;
    });
  });

  var app = builder.Build();

  // -- EF Migrations automÃ¡ticas al iniciar -------------------------------------
  // Aplica las migrations pendientes en cada startup. Idempotente: si no hay
  // nada nuevo, no hace nada. Si falla (DB no accesible, migration corrupta),
  // la API no arranca â€” Render detecta el crash y deja el deploy viejo
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

  // -- Pipeline HTTP -------------------------------------------------------------
  app.UseSwagger();
  app.UseSwaggerUI();

  //app.UseHttpsRedirection();
  app.UseCors("ReactFrontend");

  // DiagnÃ³stico de headers: solo en Development.
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

  // Middleware global: traduce cualquier excepciÃ³n no controlada a la shape
  // uniforme { error, errors } con el status code apropiado.
  app.UseMiddleware<GlobalExceptionMiddleware>();

  // Captura errores de deserializaciÃ³n JSON y los reescribe como
  // { error, errors: { Field: [...] } } que el front sabe interpretar.
  // Se registra DESPUÃ‰S del global para que las JsonException especÃ­ficas
  // no caigan en el fallback genÃ©rico.
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
  Log.Fatal(ex, "La aplicaciÃ³n fallÃ³ al arrancar.");
  // Sin esto el proceso terminaba con cÃ³digo 0 y Visual Studio lo reportaba
  // como "cerrado correctamente", ocultando que en realidad fue un crash
  // (tÃ­pico: "Failed to bind to address ...: address already in use").
  Environment.ExitCode = 1;
}
finally
{
  Log.CloseAndFlush();
}

// -- Helpers -------------------------------------------------------------------
static void ValidateCriticalConfig(IConfiguration config)
{
  var jwtKey = config["Jwt:Key"];
  if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("Falta Jwt:Key en la configuraciÃ³n.");
  if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException(
      $"Jwt:Key debe tener al menos 32 bytes (256 bits) para HS256. Tiene {Encoding.UTF8.GetByteCount(jwtKey)}.");

  var connString = config.GetConnectionString("DefaultConnection");
  if (string.IsNullOrWhiteSpace(connString))
    throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection en la configuraciÃ³n.");
}
