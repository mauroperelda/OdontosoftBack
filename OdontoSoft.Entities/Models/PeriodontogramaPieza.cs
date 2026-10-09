using OdontoSoft.Entities.Base;

namespace OdontoSoft.Entities.Models
{
	/// <summary>
	/// Detalle por pieza de una evaluación periodontal.
	/// Numeración FDI: 11..48.
	/// </summary>
	public class PeriodontogramaPieza : AuditedEntity
	{
		public int PeriodontogramaId { get; set; }
		public Periodontograma Periodontograma { get; set; }
		public int NumeroPieza { get; set; }
		/// <summary>Estado periodontal general: Sano | Leve | Moderado | Severo</summary>
		public string? Estado { get; set; }
		public bool Sangrado { get; set; }
		public bool Supuracion { get; set; }
		public bool Placa { get; set; }
		/// <summary>Profundidad de bolsa al sondaje (mm). Promedio o máximo según criterio clínico.</summary>
		public decimal? BolsaPeriodontal { get; set; }
		/// <summary>Recesión gingival en mm.</summary>
		public decimal? Recesion { get; set; }
		public string? Observacion { get; set; }
	}
}
