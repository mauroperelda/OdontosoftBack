using OdontoSoft.Entities.Base;

namespace OdontoSoft.Entities.Models
{
  public class EstadoTurno : Entity
  {
    public string Nombre { get; set; }       // Pendiente, Confirmado, Cancelado, Ausente, Realizado
    public string ColorHex { get; set; }     // #FFC107, #28A745, etc.
  }
}
