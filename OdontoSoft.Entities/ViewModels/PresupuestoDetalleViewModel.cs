using OdontoSoft.Entities.Models;

namespace OdontoSoft.Entities.ViewModels
{
	public class PresupuestoDetalleViewModel
	{
		public PresupuestoDetalleViewModel() { }
		public PresupuestoDetalleViewModel(PresupuestoDetalle d)
		{
			Id = d.Id;
			PresupuestoId = d.PresupuestoId;
			TratamientoId = d.TratamientoId;
			Tratamiento = d.Tratamiento?.Nombre;
			NumeroPieza = d.NumeroPieza;
			Cantidad = d.Cantidad;
			PrecioUnitario = d.PrecioUnitario;
			Subtotal = d.Subtotal;
		}
		public int Id { get; set; }
		public int PresupuestoId { get; set; }
		public int TratamientoId { get; set; }
		public string? Tratamiento { get; set; }
		public int? NumeroPieza { get; set; }
		public int Cantidad { get; set; }
		public decimal PrecioUnitario { get; set; }
		public decimal Subtotal { get; set; }

		public void MapViewModelToDetalle(PresupuestoDetalle d)
		{
			d.PresupuestoId = PresupuestoId;
			d.TratamientoId = TratamientoId;
			d.NumeroPieza = NumeroPieza;
			d.Cantidad = Cantidad;
			d.PrecioUnitario = PrecioUnitario;
			d.Subtotal = Cantidad * PrecioUnitario;
		}
	}
}