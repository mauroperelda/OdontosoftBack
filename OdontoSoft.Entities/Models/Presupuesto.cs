using OdontoSoft.Entities.Base;

namespace OdontoSoft.Entities.Models
{
  public class Presupuesto : AuditedEntity
  {
    public int PacienteId { get; set; }
    public Paciente Paciente { get; set; }
    // Opcional: la pantalla de Facturación no pide profesional.
    public int? ProfesionalId { get; set; }
    public Profesional? Profesional { get; set; }
    public DateTime Fecha { get; set; }
    public string? Observaciones { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = "Pendiente"; // Pendiente, Aprobado, Rechazado
    public ICollection<PresupuestoDetalle> Detalles { get; set; }
    public ICollection<Pago> Pagos { get; set; }
    public string PacienteNombre => Paciente != null
    ? $"{Paciente.Apellido}, {Paciente.Nombre}"
    : string.Empty;
    public string ProfesionalNombre => Profesional != null
    ? $"{Profesional.Apellido}, {Profesional.Nombre}"
    : string.Empty;
  }
}
