using OdontoSoft.Domain.Base;

namespace OdontoSoft.Domain.Models
{
  public class OdontogramaPieza : AuditedEntity
  {
    public int HistoriaClinicaId { get; set; }
    public HistoriaClinica HistoriaClinica { get; set; }
    public int NumeroPieza { get; set; }         // 11-48 (numeración FDI)
    public string? Estado { get; set; }          // Sano, Caries, Extraccion, Corona, etc.
    public string? Observacion { get; set; }
  }
}
