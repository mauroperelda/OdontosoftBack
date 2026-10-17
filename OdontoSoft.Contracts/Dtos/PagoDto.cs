using OdontoSoft.Domain.Models;

namespace OdontoSoft.Contracts.Dtos
{
  public class PagoDto
  {
    public PagoDto() { }
    public PagoDto(Pago p)
    {
      Id = p.Id;
      PresupuestoId = p.PresupuestoId;
      Fecha = p.Fecha;
      Monto = p.Monto;
      MetodoPago = p.MetodoPago;
      Observaciones = p.Observaciones;
    }
    public int Id { get; set; }
    public int PresupuestoId { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Monto { get; set; }
    public string MetodoPago { get; set; } = "Efectivo";
    public string? Observaciones { get; set; }

    public void MapDtoToPago(Pago p)
    {
      p.PresupuestoId = PresupuestoId;
      p.Fecha = Fecha;
      p.Monto = Monto;
      p.MetodoPago = MetodoPago;
      p.Observaciones = Observaciones;
    }
  }
}