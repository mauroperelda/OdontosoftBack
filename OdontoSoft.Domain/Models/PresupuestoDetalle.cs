using OdontoSoft.Domain.Base;

namespace OdontoSoft.Domain.Models
{
  public class PresupuestoDetalle : AuditedEntity
  {
    public int PresupuestoId { get; set; }
    public Presupuesto Presupuesto { get; set; }
    public int TratamientoId { get; set; }
    public Tratamiento Tratamiento { get; set; }
    public int? NumeroPieza { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
  }
}
