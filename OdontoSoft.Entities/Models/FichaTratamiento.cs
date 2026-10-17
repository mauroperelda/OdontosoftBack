using OdontoSoft.Entities.Base;

namespace OdontoSoft.Entities.Models
{
  public class FichaTratamiento : AuditedEntity
  {
    public int HistoriaClinicaId { get; set; }
    public HistoriaClinica HistoriaClinica { get; set; }
    public int? TurnoId { get; set; }
    public Turno? Turno { get; set; }
    public int TratamientoId { get; set; }
    public Tratamiento Tratamiento { get; set; }
    public int? NumeroPieza { get; set; }
    public string? Descripcion { get; set; }
    public string? Observaciones { get; set; }
    public DateTime FechaRealizacion { get; set; }
  }
}
