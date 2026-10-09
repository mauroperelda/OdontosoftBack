using OdontoSoft.Entities.Models;

namespace OdontoSoft.Entities.ViewModels
{
	public class PeriodontogramaViewModel
	{
		public PeriodontogramaViewModel() { }
		public PeriodontogramaViewModel(Periodontograma p)
		{
			Id = p.Id;
			HistoriaClinicaId = p.HistoriaClinicaId;
			FechaRegistro = p.FechaRegistro;
			Observaciones = p.Observaciones;
			Piezas = (p.Piezas ?? new List<PeriodontogramaPieza>())
				.Select(x => new PeriodontogramaPiezaViewModel(x))
				.ToList();
		}

		public int Id { get; set; }
		public int HistoriaClinicaId { get; set; }
		public DateTime FechaRegistro { get; set; }
		public string? Observaciones { get; set; }
		public List<PeriodontogramaPiezaViewModel> Piezas { get; set; } = new();
		/// <summary>Métricas calculadas en el controller (no se persisten).</summary>
		public PeriodontogramaMetricasViewModel? Metricas { get; set; }

		public void MapViewModelToPeriodontograma(Periodontograma p)
		{
			p.HistoriaClinicaId = HistoriaClinicaId;
			p.FechaRegistro = FechaRegistro;
			p.Observaciones = Observaciones;
		}
	}

	public class PeriodontogramaMetricasViewModel
	{
		public decimal BopPorcentaje { get; set; }
		public decimal PlacaPorcentaje { get; set; }
		public int DientesPresentes { get; set; }
		public int DientesAusentes { get; set; }
	}
}
