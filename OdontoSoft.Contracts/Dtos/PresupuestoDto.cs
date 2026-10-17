using OdontoSoft.Entities.Models;

namespace OdontoSoft.Contracts.Dtos
{
  public class PresupuestoDto
  {
    public PresupuestoDto()
    {
      Detalles = new List<PresupuestoDetalleDto>();
      Pagos = new List<PagoDto>();
    }
    public PresupuestoDto(Presupuesto p)
    {
      Id = p.Id;
      PacienteId = p.PacienteId;
      Paciente = p.Paciente?.Nombre + " " + p.Paciente?.Apellido;
      ProfesionalId = p.ProfesionalId;
      Profesional = p.Profesional?.Nombre + " " + p.Profesional?.Apellido;
      Fecha = p.Fecha;
      Observaciones = p.Observaciones;
      Total = p.Total;
      Estado = p.Estado;
      SaldoPagado = p.Pagos?.Sum(x => x.Monto) ?? 0;
      SaldoPendiente = Total - SaldoPagado;
      Detalles = p.Detalles?.Select(p => new PresupuestoDetalleDto(p)).ToList() ?? new List<PresupuestoDetalleDto>();
      Pagos = p.Pagos?.Select(p => new PagoDto(p)).ToList() ?? new List<PagoDto>();
    }
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public string? Paciente { get; set; }
    public int? ProfesionalId { get; set; }
    public string? Profesional { get; set; }
    public DateTime Fecha { get; set; }
    public string? Observaciones { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public decimal SaldoPagado { get; set; }
    public decimal SaldoPendiente { get; set; }
    public List<PresupuestoDetalleDto>? Detalles { get; set; }
    public List<PagoDto>? Pagos { get; set; }

    public void MapDtoToPresupuesto(Presupuesto p)
    {
      p.PacienteId = PacienteId;
      p.ProfesionalId = ProfesionalId;
      p.Fecha = Fecha;
      p.Observaciones = Observaciones;
      p.Total = Total;
      p.Estado = Estado;
    }
  }
}