using OdontoSoft.Domain.Base;

namespace OdontoSoft.Domain.Models
{
  public class Pago : AuditedEntity
  {
    public int PresupuestoId { get; set; }
    public Presupuesto Presupuesto { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Monto { get; set; }
    public string MetodoPago { get; set; } = "Efectivo"; // Efectivo, Transferencia, Tarjeta
    public string? Observaciones { get; set; }
  }
}
