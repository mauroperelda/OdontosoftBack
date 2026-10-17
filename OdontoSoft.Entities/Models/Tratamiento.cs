using OdontoSoft.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Entities.Models
{
  public class Tratamiento : Entity
  {
    public int Id { get; set; }
    public string? Codigo { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }

    // Navegación: precios del tratamiento en cada ubicación.
    public ICollection<TratamientoPrecio> Precios { get; set; } = new List<TratamientoPrecio>();
  }
}
