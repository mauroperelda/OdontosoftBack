//using MedveCore.Entities.Base;
using Mauro.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Data.Repository.Generic
{
    public interface IBrowseable
    {
        IQueryable<BaseEntity> GetBrowser();
    }
}
