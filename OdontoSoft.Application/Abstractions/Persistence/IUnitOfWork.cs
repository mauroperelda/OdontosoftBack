using OdontoSoft.Application.Abstractions.Persistence;

namespace OdontoSoft.Application.Abstractions.Persistence;

/// <summary>
/// Unit of Work con acceso a todos los repositorios. La implementación
/// concreta vive en OdontoSoft.Infrastructure.Data. Los nombres de las
/// propiedades están en español para preservar diff mínima con el código
/// existente (los Services ya hacen _unitOfWork.PacienteRepository, etc.).
/// </summary>
public interface IUnitOfWork
{
  IPacienteRepository PacienteRepository { get; }
  IProfesionalRepository ProfesionalRepository { get; }
  ITurnoRepository TurnoRepository { get; }
  IPresupuestoRepository PresupuestoRepository { get; }
  IPresupuestoDetalleRepository PresupuestoDetalleRepository { get; }
  IPagoRepository PagoRepository { get; }
  IHistoriaClinicaRepository HistoriaClinicaRepository { get; }
  IOdontogramaPiezaRepository OdontogramaPiezaRepository { get; }
  IPeriodontogramaRepository PeriodontogramaRepository { get; }
  IImagenPacienteRepository ImagenPacienteRepository { get; }
  ILocalidadRepository LocalidadRepository { get; }
  IProvinciaRepository ProvinciaRepository { get; }
  ITratamientoRepository TratamientoRepository { get; }
  ITratamientoPrecioRepository TratamientoPrecioRepository { get; }
  IFichaTratamientoRepository FichaTratamientoRepository { get; }
  IEstadoTurnoRepository EstadoTurnoRepository { get; }
  IUbicacionRepository UbicacionRepository { get; }
  IStoredProcedureRepository StoredProcedureRepository { get; }
  int Save();
  Task SaveAsync(CancellationToken cancellationToken = default);
}
