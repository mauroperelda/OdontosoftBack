using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OdontoSoft.Application.Abstractions;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Application.Validation;
using OdontoSoft.Infrastructure.Authentication;
using OdontoSoft.Infrastructure.DAL;
using OdontoSoft.Infrastructure.Filters;
using OdontoSoft.Infrastructure.Http;
using OdontoSoft.Infrastructure.Mapping;
using OdontoSoft.Infrastructure.Repository.UOW;

namespace OdontoSoft.Infrastructure.DependencyInjection;

/// <summary>
/// Helper de extensión que registra toda la capa de infraestructura
/// (persistencia + ASP.NET Core) en el contenedor de DI.
///
/// Mantenemos un solo Infrastructure porque la API es única: si mañana
/// nace otra API (otro repo / otra arquitectura), va a traer su propio
/// set de capas. Mientras tanto, mantener Data y Web juntos en un solo
/// assembly reduce fricción sin sacrificar Clean Architecture — la
/// inversión de dependencia sigue siendo correcta: Application define
/// <see cref="IUnitOfWork"/> y <see cref="ICurrentUser"/>, Infrastructure
/// los implementa.
/// </summary>
public static class ServiceCollectionExtensions
{
  /// <summary>
  /// Registra la persistencia: <c>OdontosoftContext</c> (PostgreSQL + EF Core),
  /// <see cref="IUnitOfWork"/>, y <see cref="ICurrentUser"/>.
  /// </summary>
  public static IServiceCollection AddOdontoSoftData(this IServiceCollection services, IConfiguration configuration)
  {
    // ── DbContext ───────────────────────────────────────────────
    // Convención de naming: "Paciente" → tabla "paciente",
    // "CreatedUserId" → "created_user_id". PostgreSQL es
    // case-sensitive con identifiers quoted; snake_case lowercase es
    // idiomático y nos ahorra quoted identifiers en queries raw.
    //
    // Las migrations viven en este assembly (no en el host) para que
    // el proyecto de infra sea self-contained y no dependa de la API.
    var connString = configuration.GetConnectionString("DefaultConnection");
    services.AddDbContext<OdontosoftContext>(options =>
    {
      options.UseSnakeCaseNamingConvention();
      options.UseNpgsql(connString, b => b.MigrationsAssembly("OdontoSoft.Infrastructure"));
    });

    // ── UnitOfWork (inversión) ──────────────────────────────────
    // Application define IUnitOfWork; Infrastructure implementa la concreta.
    // Esto invierte la dependencia: Application ya no referencia Infrastructure.
    // FQN: el using IUnitOfWork = Application.Abstractions.Persistence
    //      entra en conflicto con IUnitOfWork = Repository.UOW
    //      (la versión legacy de Data que Browser usa). La concreción
    //      implementa ambas, así que registramos por la de Application.
    services.AddScoped<OdontoSoft.Application.Abstractions.Persistence.IUnitOfWork,
                      OdontoSoft.Infrastructure.Repository.UOW.UnitOfWork>();

    // ── Stored Procedure Queries (V4 del refactor Clean) ───────
    // V4: GetPacienteForTurno se movió a IStoredProcedureQueries en
    // Infrastructure, separado del IUnitOfWork que vive en Application.
    // La motivación: el stored procedure devuelve un shape específico
    // de la base (PacienteSummary, no PacienteDto) para que esta capa
    // no conozca los DTOs de Contracts.
    services.AddScoped<OdontoSoft.Infrastructure.Repository.StoredProcedure.IStoredProcedureQueries,
                      OdontoSoft.Infrastructure.Repository.StoredProcedure.StoredProcedureRepository>();

    // ── Usuario autenticado ─────────────────────────────────────
    // ICurrentUser vive en Application; la implementación lee los
    // claims del HttpContext (de ahí que viva en Infrastructure, que
    // tiene FrameworkReference a AspNetCore.App).
    services.AddHttpContextAccessor();
    services.AddScoped<ICurrentUser, AuthenticatedUser>();

    return services;
  }

  /// <summary>
  /// Registra lo que un host ASP.NET Core necesita para servir la
  /// API de OdontoSoft: AutoMapper, FluentValidation, JSON options,
  /// Filtros y el formateador uniforme de errores de ModelState.
  ///
  /// NO registra controllers (eso es <c>AddControllers()</c>) ni
  /// middlewares (esos van en el pipeline con
  /// <c>app.UseMiddleware&lt;...&gt;</c>).
  /// </summary>
  public static IServiceCollection AddOdontoSoftWeb(this IServiceCollection services)
  {
    // ── AutoMapper: escanea el assembly buscando Profile. ──────────
    services.AddAutoMapper(typeof(ApplicationProfile));

    // ── FluentValidation: todos los IValidator<T> del assembly de
    //    OdontoSoft.Application.Validation. ─────────────────────────
    services.AddValidatorsFromAssemblyContaining<PacienteDtoValidator>();

    // ── Filtro de validación global. ───────────────────────────────
    // Corre los IValidator<T> registrados en DI contra los argumentos
    // de cada acción y vuelca los errores al ModelState.
    services.AddScoped<FluentValidationFilter>();
    services.AddControllers(options =>
    {
      options.Filters.AddService<FluentValidationFilter>();
    })
    .AddJsonOptions(options =>
    {
      options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
      options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
      options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
      // Acepta "" como null en int?, decimal?, DateTime?, etc.
      options.JsonSerializerOptions.Converters.Add(new LenientNullableJsonConverter());
    });

    // ── Formato uniforme de errores de validación. ────────────────
    // Cuando [ApiController] detecta ModelState inválido, en vez del
    // ProblemDetails por defecto, devolvemos { error, errors } que
    // es lo que el front espera.
    services.Configure<ApiBehaviorOptions>(options =>
    {
      options.InvalidModelStateResponseFactory = context =>
      {
        var errors = new Dictionary<string, string[]>();
        foreach (var (key, entry) in context.ModelState)
        {
          if (entry is null || entry.Errors.Count == 0) continue;
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

    return services;
  }
}
