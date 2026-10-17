using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Domain.Models;

namespace OdontoSoft.Application.Abstractions.Persistence;

// Repos especÃ­ficos. Firmas alineadas con GenericRepository<T> de
// Mauro.RepositoryPattern.Infrastructure (Add/Upsert devuelven bool,
// Delete devuelve Task<bool>, "GetAll" se llama "All", no hay Update).
// Los Services usan estas interfaces sin conocer EF. La implementaciÃ³n
// en OdontoSoft.Infrastructure hereda de GenericRepository<T> y
// solo agrega los mÃ©todos custom abajo.

public interface IPacienteRepository
{
  OdontoSoft.Domain.Models.Paciente? GetById(int id);
  IQueryable<OdontoSoft.Domain.Models.Paciente> All();
  IQueryable<OdontoSoft.Domain.Models.Paciente> GetAllEnabled();
  IQueryable<OdontoSoft.Domain.Models.Paciente> GetAllEnabled(int currentId);
  bool Add(OdontoSoft.Domain.Models.Paciente entity);
  bool Upsert(OdontoSoft.Domain.Models.Paciente entity);
  Task<bool> Delete(int id);
}

public interface IProfesionalRepository
{
  OdontoSoft.Domain.Models.Profesional? GetById(int id);
  IQueryable<OdontoSoft.Domain.Models.Profesional> All();
  IQueryable<OdontoSoft.Domain.Models.Profesional> GetAllEnabled();
  IQueryable<OdontoSoft.Domain.Models.Profesional> GetAllEnabled(int currentId);
  bool Add(OdontoSoft.Domain.Models.Profesional entity);
  bool Upsert(OdontoSoft.Domain.Models.Profesional entity);
  Task<bool> Delete(int id);
}

public interface ITurnoRepository
{
  Turno? GetById(int id);
  IQueryable<Turno> All();
  IQueryable<Turno> GetAll();
  IQueryable<Turno> GetAllEnabled();
  IQueryable<Turno> GetAllEnabled(int currentId);
  bool Add(Turno entity);
  bool Upsert(Turno entity);
  Task<bool> Delete(int id);
  Turno GetByProfesionalByHoraInicio(int profesionalId, DateTime? horaInicio);
  Turno GetByTurnoId(int id);
}

public interface IPresupuestoRepository
{
  Presupuesto? GetById(int id);
  IQueryable<Presupuesto> All();
  IQueryable<Presupuesto> GetAllEnabled();
  IQueryable<Presupuesto> GetAllEnabled(int currentId);
  bool Add(Presupuesto entity);
  bool Upsert(Presupuesto entity);
  Task<bool> Delete(int id);
  IQueryable<Presupuesto> GetByPacienteId(int pacienteId);
  Presupuesto GetPresupuestoById(int id);
}

public interface IPresupuestoDetalleRepository
{
  PresupuestoDetalle? GetById(int id);
  IQueryable<PresupuestoDetalle> All();
  IQueryable<PresupuestoDetalle> GetAllEnabled();
  IQueryable<PresupuestoDetalle> GetAllEnabled(int currentId);
  bool Add(PresupuestoDetalle entity);
  bool Upsert(PresupuestoDetalle entity);
  Task<bool> Delete(int id);
}

public interface IPagoRepository
{
  Pago? GetById(int id);
  IQueryable<Pago> All();
  IQueryable<Pago> GetAllEnabled();
  IQueryable<Pago> GetAllEnabled(int currentId);
  bool Add(Pago entity);
  bool Upsert(Pago entity);
  Task<bool> Delete(int id);
}

public interface IHistoriaClinicaRepository
{
  HistoriaClinica? GetById(int id);
  IQueryable<HistoriaClinica> All();
  IQueryable<HistoriaClinica> GetAllEnabled();
  IQueryable<HistoriaClinica> GetAllEnabled(int currentId);
  bool Add(HistoriaClinica entity);
  bool Upsert(HistoriaClinica entity);
  Task<bool> Delete(int id);
  HistoriaClinica GetByPacienteId(int pacienteId);
  HistoriaClinica GetFullByPacienteId(int pacienteId);
  HistoriaClinica GetHistoriaClinicaById(int id);
}

public interface IOdontogramaPiezaRepository
{
  OdontogramaPieza? GetById(int id);
  IQueryable<OdontogramaPieza> All();
  IQueryable<OdontogramaPieza> GetAllEnabled();
  IQueryable<OdontogramaPieza> GetAllEnabled(int currentId);
  bool Add(OdontogramaPieza entity);
  bool Upsert(OdontogramaPieza entity);
  Task<bool> Delete(int id);
  IQueryable<OdontogramaPieza> GetByHistoriaClinicaId(int historiaClinicaId);
  OdontogramaPieza GetByHistoriaClinicaIdAndNumeroPieza(int historiaClinicaId, int numeroPieza);
}

public interface IPeriodontogramaRepository
{
  Periodontograma? GetById(int id);
  IQueryable<Periodontograma> All();
  IQueryable<Periodontograma> GetAllEnabled();
  IQueryable<Periodontograma> GetAllEnabled(int currentId);
  bool Add(Periodontograma entity);
  bool Upsert(Periodontograma entity);
  Task<bool> Delete(int id);
  /// <summary>Devuelve el Ãºltimo periodontograma (por FechaRegistro) de una HC, con piezas cargadas.</summary>
  Periodontograma GetLastByHistoriaClinicaId(int historiaClinicaId);
  /// <summary>Devuelve todos los periodontogramas de una HC ordenados desc por fecha.</summary>
  IQueryable<Periodontograma> GetByHistoriaClinicaId(int historiaClinicaId);
  /// <summary>Devuelve un periodontograma con sus piezas cargadas explÃ­citamente.</summary>
  Periodontograma GetWithPiezasById(int id);
  /// <summary>Agrega una pieza al DbSet (no guarda cambios). Llamar Save() despuÃ©s.</summary>
  void AddPieza(PeriodontogramaPieza pieza);
  /// <summary>Borra todas las piezas de un periodontograma (no guarda cambios). Llamar Save() despuÃ©s.</summary>
  void DeletePiezas(int periodontogramaId);
}

public interface IImagenPacienteRepository
{
  ImagenPaciente? GetById(int id);
  IQueryable<ImagenPaciente> All();
  IQueryable<ImagenPaciente> GetAllEnabled();
  IQueryable<ImagenPaciente> GetAllEnabled(int currentId);
  bool Add(ImagenPaciente entity);
  bool Upsert(ImagenPaciente entity);
  Task<bool> Delete(int id);
  List<ImagenPaciente> GetByHistoriaClinicaId(int historiaClinicaId);
}

public interface ILocalidadRepository
{
  Localidad? GetById(int id);
  IQueryable<Localidad> All();
  IQueryable<Localidad> GetAllEnabled();
  IQueryable<Localidad> GetAllEnabled(int currentId);
  bool Add(Localidad entity);
  bool Upsert(Localidad entity);
  Task<bool> Delete(int id);
  IQueryable<Localidad> GetByProvinciaId(int provinciaId);
}

public interface IProvinciaRepository
{
  Provincia? GetById(int id);
  IQueryable<Provincia> All();
  IQueryable<Provincia> GetAllEnabled();
  IQueryable<Provincia> GetAllEnabled(int currentId);
  bool Add(Provincia entity);
  bool Upsert(Provincia entity);
  Task<bool> Delete(int id);
}

public interface ITratamientoRepository
{
  Tratamiento? GetById(int id);
  IQueryable<Tratamiento> All();
  IQueryable<Tratamiento> GetAllEnabled();
  IQueryable<Tratamiento> GetAllEnabled(int currentId);
  bool Add(Tratamiento entity);
  bool Upsert(Tratamiento entity);
  Task<bool> Delete(int id);
}

public interface ITratamientoPrecioRepository
{
  TratamientoPrecio? GetById(int id);
  IQueryable<TratamientoPrecio> All();
  IQueryable<TratamientoPrecio> GetAllEnabled();
  IQueryable<TratamientoPrecio> GetAllEnabled(int currentId);
  bool Add(TratamientoPrecio entity);
  bool Upsert(TratamientoPrecio entity);
  Task<bool> Delete(int id);
}

public interface IFichaTratamientoRepository
{
  FichaTratamiento? GetById(int id);
  IQueryable<FichaTratamiento> All();
  IQueryable<FichaTratamiento> GetAllEnabled();
  IQueryable<FichaTratamiento> GetAllEnabled(int currentId);
  bool Add(FichaTratamiento entity);
  bool Upsert(FichaTratamiento entity);
  Task<bool> Delete(int id);
  IQueryable<FichaTratamiento> GetByHistoriaClinicaId(int historiaClinicaId);
  IQueryable<FichaTratamiento> GetByTurnoId(int turnoId);
  FichaTratamiento GetFichaTratamientoById(int id);
}

public interface IEstadoTurnoRepository
{
  EstadoTurno? GetById(int id);
  IQueryable<EstadoTurno> All();
  IQueryable<EstadoTurno> GetAllEnabled();
  IQueryable<EstadoTurno> GetAllEnabled(int currentId);
  bool Add(EstadoTurno entity);
  bool Upsert(EstadoTurno entity);
  Task<bool> Delete(int id);
}

public interface IUbicacionRepository
{
  Ubicacion? GetById(int id);
  IQueryable<Ubicacion> All();
  IQueryable<Ubicacion> GetAllEnabled();
  IQueryable<Ubicacion> GetAllEnabled(int currentId);
  bool Add(Ubicacion entity);
  bool Upsert(Ubicacion entity);
  Task<bool> Delete(int id);
}
