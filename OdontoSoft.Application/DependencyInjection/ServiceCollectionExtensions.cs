using Microsoft.Extensions.DependencyInjection;
using OdontoSoft.Application.Services;

namespace OdontoSoft.Application.DependencyInjection;

/// <summary>
/// Helper de extensión que registra todos los Services de aplicación
/// (los "use cases") de OdontoSoft en el contenedor de DI.
///
/// El mapping de interfaces a implementaciones está centralizado acá
/// para que <c>Program.cs</c> no acumule <c>AddScoped</c> por cada
/// Service nuevo que se sume.
/// </summary>
public static class ServiceCollectionExtensions
{
  /// <summary>
  /// Registra los 14 Services de aplicación:
  /// - HttpClient-bound: <see cref="ILocalidadServices"/> (consulta georef API).
  /// - Scoped: el resto (dependen de IUnitOfWork + ICurrentUser de Application).
  /// </summary>
  public static IServiceCollection AddOdontoSoftApplication(this IServiceCollection services)
  {
    // HttpClient tipado: el factory de Microsoft se encarga del ciclo de
    // vida del HttpClient, evitando el problema de sockets agotados en
    // servicios de larga duración.
    services.AddHttpClient<ILocalidadServices, LocalidadServices>();

    // Scoped: cada request HTTP obtiene su propia instancia. Comparten
    // IUnitOfWork (un DbContext por request) y un ICurrentUser (también
    // scoped, lee claims del HttpContext).
    services.AddScoped<IEstadoTurnoServices, EstadoTurnoServices>();
    services.AddScoped<IFichaTratamientoServices, FichaTratamientoServices>();
    services.AddScoped<IHistoriaClinicaServices, HistoriaClinicaServices>();
    services.AddScoped<IOdontogramaPiezaServices, OdontogramaPiezaServices>();
    services.AddScoped<IPacienteServices, PacienteServices>();
    services.AddScoped<IPagoServices, PagoServices>();
    services.AddScoped<IPeriodontogramaServices, PeriodontogramaServices>();
    services.AddScoped<IPresupuestoDetalleServices, PresupuestoDetalleServices>();
    services.AddScoped<IPresupuestoServices, PresupuestoServices>();
    services.AddScoped<IProfesionalServices, ProfesionalServices>();
    services.AddScoped<IProvinciaServices, ProvinciaServices>();
    services.AddScoped<ITratamientoServices, TratamientoServices>();
    services.AddScoped<ITurnoServices, TurnoServices>();

    return services;
  }
}
