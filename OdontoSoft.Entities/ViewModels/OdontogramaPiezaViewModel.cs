using OdontoSoft.Entities.Models;

namespace OdontoSoft.Entities.ViewModels
{
	public class OdontogramaPiezaViewModel
	{
		public OdontogramaPiezaViewModel() { }
		public OdontogramaPiezaViewModel(OdontogramaPieza p)
		{
			Id = p.Id;
			HistoriaClinicaId = p.HistoriaClinicaId;
			NumeroPieza = p.NumeroPieza;
			Estado = p.Estado;
			Observacion = p.Observacion;
		}
		public int Id { get; set; }
		public int HistoriaClinicaId { get; set; }
		public int NumeroPieza { get; set; }
		public string? Estado { get; set; }
		public string? Observacion { get; set; }

		public void MapViewModelToOdontogramaPieza(OdontogramaPieza p)
		{
			p.HistoriaClinicaId = HistoriaClinicaId;
			p.NumeroPieza = NumeroPieza;
			p.Estado = Estado;
			p.Observacion = Observacion;
		}
	}
}