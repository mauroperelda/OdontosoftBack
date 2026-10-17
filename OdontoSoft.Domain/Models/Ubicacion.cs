using OdontoSoft.Domain.Base;
using System.Collections.Generic;

namespace OdontoSoft.Domain.Models
{
  // Representa un consultorio / sede de la práctica. Tiene una dirección,
  // contacto y un precio distinto para cada Tratamiento (servicio).
  public class Ubicacion : AuditedEntity
  {
    public string Nombre { get; set; }
    public string Direccion { get; set; }
    public string? CodigoPostal { get; set; }
    public string Ciudad { get; set; }
    public string? Provincia { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }

    // Navegación: precios asociados a esta ubicación.
    public ICollection<TratamientoPrecio> Precios { get; set; } = new List<TratamientoPrecio>();
  }
}
