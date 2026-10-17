//using MedveCore.Entities.Base;
using Mauro.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace OdontoSoft.Entities.Base
{
  [NotMapped]
  public class EntityCodedValue : BaseEntity
  {
    public String Descripcion { get; set; }
  }

}
