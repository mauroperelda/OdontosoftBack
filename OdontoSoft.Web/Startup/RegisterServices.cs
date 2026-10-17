using OdontoSoft.Application.Services;

namespace OdontoSoft.Web.Startup
{
  public class RegisterServices
  {
    public static void Register(IServiceCollection services)
    {
      services.AddTransient<IEstadoTurnoServices, EstadoTurnoServices>();
      services.AddTransient<IFichaTratamientoServices, FichaTratamientoServices>();
      services.AddTransient<IHistoriaClinicaServices, HistoriaClinicaServices>();
      services.AddTransient<ILocalidadServices, LocalidadServices>();
      services.AddTransient<IOdontogramaPiezaServices, OdontogramaPiezaServices>();
      services.AddTransient<IPacienteServices, PacienteServices>();
      services.AddTransient<IPagoServices, PagoServices>();
      services.AddTransient<IPresupuestoServices, PresupuestoServices>();
      services.AddTransient<IPresupuestoDetalleServices, PresupuestoDetalleServices>();
      services.AddTransient<IProfesionalServices, ProfesionalServices>();
      services.AddTransient<IProvinciaServices, ProvinciaServices>();
      services.AddTransient<ITratamientoServices, TratamientoServices>();
      services.AddTransient<ITurnoServices, TurnoServices>();
    }
  }
}

