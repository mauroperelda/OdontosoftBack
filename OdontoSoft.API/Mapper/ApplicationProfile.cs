using AutoMapper;
using OdontoSoft.Entities.Models;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.API.Mapper;

/// <summary>
/// Perfiles de AutoMapper. Reemplazan los métodos manuales
/// <c>MapDtoToXxx(entity)</c> que vivían en cada DTO.
///
/// Reglas generales:
/// - El mapeo es bidireccional (DTO ↔ Entity) salvo donde se aclara.
/// - Las propiedades con nombres idénticos se mapean solas.
/// - Las custom (ej. <c>dto.DomicilioNumero → entity.DomicilioNumero</c>)
///   van con <c>ForMember</c>.
/// - Los nombres calculados (ej. <c>dto.Paciente = "Apellido, Nombre"</c>)
///   se calculan con <c>MapFrom</c> apuntando a un método del perfil.
/// </summary>
public class ApplicationProfile : Profile
{
  public ApplicationProfile()
  {
    // ── Paciente ─────────────────────────────────────────────
    CreateMap<PacienteDto, Paciente>()
      .ForMember(d => d.DomicilioNumero, o => o.MapFrom(s => s.DomicilioNumero))
      .ForMember(d => d.Telefono, o => o.MapFrom(s => s.Telefono));

    // ── Profesional ──────────────────────────────────────────
    // ProfesionalDto.MapDtoToProfesional(profesional) tiene un
    // nombre confuso (en realidad mapea a Profesional). Lo preservamos
    // con la misma forma: DomicilioCalle → entity.Domicilio.
    CreateMap<ProfesionalDto, Profesional>()
      .ForMember(d => d.Domicilio, o => o.MapFrom(s => s.DomicilioCalle));

    // ── Turno ────────────────────────────────────────────────
    // TurnoDto → Turno: la VM tiene campos expandidos (PacienteObj,
    // ProfesionalObj) que no se persisten — sólo los FKs. La columna
    // UbicacionId se normaliza (0 → null) en el VM, no acá.
    CreateMap<TurnoDto, Turno>()
      .ForMember(d => d.HoraFin, o => o.MapFrom(s => s.HoraFin));

    // ── Presupuesto ──────────────────────────────────────────
    CreateMap<PresupuestoDto, Presupuesto>();

    // ── PresupuestoDetalle ───────────────────────────────────
    // Subtotal se calcula: Cantidad * PrecioUnitario. La VM lo manda
    // pre-calculado pero la entity lo recomputa en MapDtoToDetalle.
    CreateMap<PresupuestoDetalleDto, PresupuestoDetalle>()
      .ForMember(d => d.Subtotal, o => o.MapFrom(s => s.Cantidad * s.PrecioUnitario));

    // ── Pago ─────────────────────────────────────────────────
    CreateMap<PagoDto, Pago>();

    // ── FichaTratamiento ─────────────────────────────────────
    CreateMap<FichaTratamientoDto, FichaTratamiento>();

    // ── HistoriaClinica ──────────────────────────────────────
    CreateMap<HistoriaClinicaDto, HistoriaClinica>();

    // ── OdontogramaPieza ─────────────────────────────────────
    CreateMap<OdontogramaPiezaDto, OdontogramaPieza>();

    // ── Ubicacion ────────────────────────────────────────────
    CreateMap<UbicacionDto, Ubicacion>();
  }
}
