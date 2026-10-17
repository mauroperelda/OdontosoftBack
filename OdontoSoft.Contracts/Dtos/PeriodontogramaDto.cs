using OdontoSoft.Domain.Models;

namespace OdontoSoft.Contracts.Dtos
{
  public class PeriodontogramaDto
  {
    public PeriodontogramaDto() { }
    public PeriodontogramaDto(Periodontograma p)
    {
      Id = p.Id;
      HistoriaClinicaId = p.HistoriaClinicaId;
      FechaRegistro = p.FechaRegistro;
      Observaciones = p.Observaciones;
      Piezas = (p.Piezas ?? new List<PeriodontogramaPieza>())
        .Select(x => new PeriodontogramaPiezaDto(x))
        .ToList();
    }

    public int Id { get; set; }
    public int HistoriaClinicaId { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string? Observaciones { get; set; }
    public List<PeriodontogramaPiezaDto> Piezas { get; set; } = new();
    /// <summary>Métricas calculadas en el controller (no se persisten).</summary>
    public PeriodontogramaMetricasDto? Metricas { get; set; }

    public void MapDtoToPeriodontograma(Periodontograma p)
    {
      p.HistoriaClinicaId = HistoriaClinicaId;
      p.FechaRegistro = FechaRegistro;
      p.Observaciones = Observaciones;
    }
  }

  public class PeriodontogramaMetricasDto
  {
    public decimal BopPorcentaje { get; set; }
    public decimal PlacaPorcentaje { get; set; }
    public int DientesPresentes { get; set; }
    public int DientesAusentes { get; set; }
  }
}