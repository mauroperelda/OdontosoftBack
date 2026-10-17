using OdontoSoft.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Entities.Models
{
  public class Localidad : Entity
  {
    public override string ToString()
    {
      return Descripcion;
    }
    public int Id { get; set; }
    public string Descripcion { get; set; }
    public int ProvinciaId { get; set; }
    public Provincia? Provincia { get; set; }
  }
}
