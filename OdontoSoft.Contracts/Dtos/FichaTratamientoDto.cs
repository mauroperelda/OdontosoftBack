using OdontoSoft.Domain.Models;

namespace OdontoSoft.Contracts.Dtos
{
  public class FichaTratamientoDto
  {
    public FichaTratamientoDto() { }
    public FichaTratamientoDto(FichaTratamiento f)
    {
      Id = f.Id;
      HistoriaClinicaId = f.HistoriaClinicaId;
      TurnoId = f.TurnoId;
      TratamientoId = f.TratamientoId;
      Tratamiento = f.Tratamiento?.Nombre;
      NumeroPieza = f.NumeroPieza;
      Descripcion = f.Descripcion;
      Observaciones = f.Observaciones;
      FechaRealizacion = f.FechaRealizacion;
    }
    public int Id { get; set; }
    public int HistoriaClinicaId { get; set; }
    public int? TurnoId { get; set; }
    public int TratamientoId { get; set; }
    public string? Tratamiento { get; set; }
    public int? NumeroPieza { get; set; }
    public string? Descripcion { get; set; }
    public string? Observaciones { get; set; }
    public DateTime FechaRealizacion { get; set; }

    public void MapDtoToFichaTratamiento(FichaTratamiento f)
    {
      f.HistoriaClinicaId = HistoriaClinicaId;
      f.TurnoId = TurnoId;
      f.TratamientoId = TratamientoId;
      f.NumeroPieza = NumeroPieza;
      f.Descripcion = Descripcion;
      f.Observaciones = Observaciones;
      f.FechaRealizacion = FechaRealizacion;
    }
  }
}