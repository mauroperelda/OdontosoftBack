using OdontoSoft.Entities.Models;

namespace OdontoSoft.Entities.ViewModels
{
	public class PresupuestoViewModel
	{
		public PresupuestoViewModel() 
		{
			Detalles = new List<PresupuestoDetalleViewModel>();
			Pagos = new List<PagoViewModel>();
		}
		public PresupuestoViewModel(Presupuesto p)
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
			Detalles = p.Detalles?.Select(p => new PresupuestoDetalleViewModel(p)).ToList() ?? new List<PresupuestoDetalleViewModel>();
			Pagos = p.Pagos?.Select(p => new PagoViewModel(p)).ToList() ?? new List<PagoViewModel>();
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
		public List<PresupuestoDetalleViewModel>? Detalles { get; set; }
		public List<PagoViewModel>? Pagos { get; set; }

		public void MapViewModelToPresupuesto(Presupuesto p)
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