using OdontoSoft.Entities.Models;

namespace OdontoSoft.Entities.ViewModels
{
	public class HistoriaClinicaViewModel
	{
		public HistoriaClinicaViewModel() 
		{
			Piezas = new List<OdontogramaPiezaViewModel>();
			Fichas = new List<FichaTratamientoViewModel>();
		}
		public HistoriaClinicaViewModel(HistoriaClinica h)
		{
			Id = h.Id;
			PacienteId = h.PacienteId;
			Paciente = h.Paciente?.Nombre + " " + h.Paciente?.Apellido;
			Alergias = h.Alergias;
			Medicamentos = h.Medicamentos;
			AntecedentesMedicos = h.AntecedentesMedicos;
			Observaciones = h.Observaciones;
			Piezas = h.OdontogramaPiezas?
				.Select(p => new OdontogramaPiezaViewModel(p))
				.ToList() ?? new List<OdontogramaPiezaViewModel>();

			Fichas = h.FichasTratamiento?
				.Select(f => new FichaTratamientoViewModel(f))
				.ToList() ?? new List<FichaTratamientoViewModel>();
		}
		public int Id { get; set; }
		public int PacienteId { get; set; }
		public string? Paciente { get; set; }
		public string? Alergias { get; set; }
		public string? Medicamentos { get; set; }
		public string? AntecedentesMedicos { get; set; }
		public string? Observaciones { get; set; }
		public List<OdontogramaPiezaViewModel>? Piezas { get; set; }
		public List<FichaTratamientoViewModel>? Fichas { get; set; }

		public void MapViewModelToHistoriaClinica(HistoriaClinica h)
		{
			h.PacienteId = PacienteId;
			h.Alergias = Alergias;
			h.Medicamentos = Medicamentos;
			h.AntecedentesMedicos = AntecedentesMedicos;
			h.Observaciones = Observaciones;
		}
	}
}