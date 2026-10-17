//using MedveCore.Entities.Base;
using Mauro.Entities.Base;
using Mauro.RepositoryPattern.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OdontoSoft.Domain.Base
{
  public abstract class AuditedEntity : AuditedBaseEntity, IObjectState
  {
    [NotMapped]
    public ObjectState ObjectState { get; set; }

    [NotMapped]
    public string EntityName
    {
      get
      {
        if (this.GetType().Module != null && this.GetType().Module.ScopeName == "EntityProxyModule")
        {
          return this.GetType().BaseType.Name;
        }
        else
        {
          return this.GetType().Name;
        }


      }
    }

    [NotMapped]
    public String GetString
    {
      get
      {
        return this.ToString();
      }
    }

    public void ToUpper()
    {
      foreach (var property in this.GetType().GetProperties())
      {
        var value = property.GetValue(this, null);
        if (value is string)
        {
          property.SetValue(this, value.ToString().ToUpper());
        }
      }
    }


  }

}
