using OdontoSoft.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Entities.Models
{
  public class Profesional : AuditedEntity
  {
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? Dni { get; set; }
    public string? Celular { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public string? Domicilio { get; set; }
    public int? ProvinciaId { get; set; }
    public Provincia? Provincia { get; set; }
    public int? LocalidadId { get; set; }
    public Localidad? Localidad { get; set; }
    public string? Email { get; set; }
    public string? Cuit { get; set; }

    public override string ToString()
    {
      return $"{Apellido}, {Nombre}";
    }
  }
}
