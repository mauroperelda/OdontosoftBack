using OdontoSoft.Entities.Base;

namespace OdontoSoft.Entities.Models
{
	public class HistoriaClinica : AuditedEntity
	{
		public int PacienteId { get; set; }
		public Paciente Paciente { get; set; }
		public string? Alergias { get; set; }
		public string? Medicamentos { get; set; }
		public string? AntecedentesMedicos { get; set; }
		public string? Observaciones { get; set; }
		public ICollection<FichaTratamiento> FichasTratamiento { get; set; }
		public ICollection<OdontogramaPieza> OdontogramaPiezas { get; set; }
		public ICollection<Periodontograma> Periodontogramas { get; set; }
		public ICollection<ImagenPaciente> ImagenesPaciente { get; set; }
		// ✅ Para que el browser muestre el nombre en la columna Paciente
		public string PacienteNombre => Paciente != null
				? $"{Paciente.Apellido}, {Paciente.Nombre}"
				: string.Empty;
	}
}