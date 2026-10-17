//using MedveCore.Entities.Base;
using Mauro.Entities.Base;
using Mauro.RepositoryPattern.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Infrastructure.Data.Repository
{
  public interface IGenericRepository<T> where T : BaseEntity, IObjectState
  {
    IQueryable<T> All();
    IQueryable<T> GetAllEnabled();
    IQueryable<T> GetAllEnabled(int currentId);
    T GetById(int id);
    T GetByIdAsNoTracking(int id);
    bool Add(T entity);
    Task<bool> Delete(int id);
    bool Upsert(T entity);

    bool ValidateBeforeSave(T entity);
  }
}
