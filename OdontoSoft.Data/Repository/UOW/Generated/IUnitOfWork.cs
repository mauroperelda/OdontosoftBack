using OdontoSoft.Data.Repository.Interfaces;
using Mauro.Entities.Base;

namespace OdontoSoft.Data.Repository.UOW
{
  public partial interface IUnitOfWork
  {
    IEstadoTurnoRepository EstadoTurnoRepository { get; }
    IFichaTratamientoRepository FichaTratamientoRepository { get; }
    IHistoriaClinicaRepository HistoriaClinicaRepository { get; }
    IImagenPacienteRepository ImagenPacienteRepository { get; }
    ILocalidadRepository LocalidadRepository { get; }
    IOdontogramaPiezaRepository OdontogramaPiezaRepository { get; }
    IPacienteRepository PacienteRepository { get; }
    IPagoRepository PagoRepository { get; }
    IPeriodontogramaRepository PeriodontogramaRepository { get; }
    IPresupuestoRepository PresupuestoRepository { get; }
    IPresupuestoDetalleRepository PresupuestoDetalleRepository { get; }
    IProfesionalRepository ProfesionalRepository { get; }
    IProvinciaRepository ProvinciaRepository { get; }
    ITratamientoRepository TratamientoRepository { get; }
    ITratamientoPrecioRepository TratamientoPrecioRepository { get; }
    ITurnoRepository TurnoRepository { get; }
    IUbicacionRepository UbicacionRepository { get; }
  }
}

