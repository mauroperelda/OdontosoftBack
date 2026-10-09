using OdontoSoft.Entities.Models;

namespace OdontoSoft.Entities.ViewModels
{
	public class PagoViewModel
	{
		public PagoViewModel() { }
		public PagoViewModel(Pago p)
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

		public void MapViewModelToPago(Pago p)
		{
			p.PresupuestoId = PresupuestoId;
			p.Fecha = Fecha;
			p.Monto = Monto;
			p.MetodoPago = MetodoPago;
			p.Observaciones = Observaciones;
		}
	}
}