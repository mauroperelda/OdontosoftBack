using OdontoSoft.Domain.Models;

namespace OdontoSoft.Contracts.Dtos
{
  public class EstadoTurnoDto
  {
    public EstadoTurnoDto() { }

    public EstadoTurnoDto(EstadoTurno e)
    {
      Id = e.Id;
      Nombre = e.Nombre;
      ColorHex = e.ColorHex;
    }

    public int Id { get; set; }
    public string Nombre { get; set; }
    public string ColorHex { get; set; }

    public void MapDtoToEstadoTurno(EstadoTurno e)
    {
      e.Nombre = Nombre;
      e.ColorHex = ColorHex;
    }
  }
}