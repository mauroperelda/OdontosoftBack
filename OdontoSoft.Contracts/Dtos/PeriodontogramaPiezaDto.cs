using OdontoSoft.Domain.Models;

namespace OdontoSoft.Contracts.Dtos
{
  public class PeriodontogramaPiezaDto
  {
    public PeriodontogramaPiezaDto() { }
    public PeriodontogramaPiezaDto(PeriodontogramaPieza p)
    {
      Id = p.Id;
      PeriodontogramaId = p.PeriodontogramaId;
      NumeroPieza = p.NumeroPieza;
      Estado = p.Estado;
      Sangrado = p.Sangrado;
      Supuracion = p.Supuracion;
      Placa = p.Placa;
      BolsaPeriodontal = p.BolsaPeriodontal;
      Recesion = p.Recesion;
      Observacion = p.Observacion;
    }

    public int Id { get; set; }
    public int PeriodontogramaId { get; set; }
    public int NumeroPieza { get; set; }
    public string? Estado { get; set; }
    public bool Sangrado { get; set; }
    public bool Supuracion { get; set; }
    public bool Placa { get; set; }
    public decimal? BolsaPeriodontal { get; set; }
    public decimal? Recesion { get; set; }
    public string? Observacion { get; set; }

    public void MapDtoToPeriodontogramaPieza(PeriodontogramaPieza p)
    {
      p.PeriodontogramaId = PeriodontogramaId;
      p.NumeroPieza = NumeroPieza;
      p.Estado = Estado;
      p.Sangrado = Sangrado;
      p.Supuracion = Supuracion;
      p.Placa = Placa;
      p.BolsaPeriodontal = BolsaPeriodontal;
      p.Recesion = Recesion;
      p.Observacion = Observacion;
    }
  }
}