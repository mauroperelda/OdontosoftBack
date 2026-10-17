using OdontoSoft.Domain.Base;

namespace OdontoSoft.Domain.Models
{
  /// <summary>
  /// Cabecera de una evaluación periodontal de un paciente.
  /// Una HC puede tener N periodontogramas a lo largo del tiempo (uno por
  /// sesión clínica). El cálculo de métricas (BOP, índice de placa, etc.)
  /// se hace en el controller a partir de las <see cref="Piezas"/>.
  /// </summary>
  public class Periodontograma : AuditedEntity
  {
    public int HistoriaClinicaId { get; set; }
    public HistoriaClinica HistoriaClinica { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string? Observaciones { get; set; }
    public ICollection<PeriodontogramaPieza> Piezas { get; set; }
  }
}
