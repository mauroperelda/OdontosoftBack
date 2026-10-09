//using MedveCore.Entities.Base;
using Mauro.Entities.Base;
using Mauro.RepositoryPattern.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;

namespace OdontoSoft.Entities.Base
{
    public abstract class Entity : BaseEntity, IObjectState
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
                if (value is string && property.SetMethod != null)
                {
                    property.SetValue(this, value.ToString().ToUpper());
                }
            }
        }
    }

}
