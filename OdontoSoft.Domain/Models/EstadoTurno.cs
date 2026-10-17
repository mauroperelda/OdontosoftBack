using OdontoSoft.Domain.Base;

namespace OdontoSoft.Domain.Models
{
  public class EstadoTurno : Entity
  {
    public string Nombre { get; set; }       // Pendiente, Confirmado, Cancelado, Ausente, Realizado
    public string ColorHex { get; set; }     // #FFC107, #28A745, etc.
  }
}
