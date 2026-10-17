//using MedveCore.Entities.Base;
using Mauro.Entities.Base;
using Mauro.RepositoryPattern.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OdontoSoft.Application.Abstractions;
using OdontoSoft.Infrastructure.DAL;
using OdontoSoft.Infrastructure.Repository.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Infrastructure.Repository
{
  public class GenericRepository<T> : IGenericRepository<T>, IBrowseable where T : BaseEntity, IObjectState
  {
    protected OdontosoftContext _context;
    protected DbSet<T> _dbSet;
    protected readonly ILogger _logger;
    protected readonly ICurrentUser _authenticatedUser;

    public GenericRepository(OdontosoftContext context, ILogger logger, ICurrentUser authenticatedUser)
    {
      _context = context;
      _logger = logger;
      _dbSet = context.Set<T>();
      _authenticatedUser = authenticatedUser;
    }

    public virtual IQueryable<T> AllForBrowser()
    {
      return _dbSet.AsQueryable();
    }
    public IQueryable<BaseEntity> GetBrowser()
    {
      return AllForBrowser();
    }

    public virtual IQueryable<T> All()
    {
      return _dbSet.AsQueryable();
    }

    public virtual IQueryable<T> GetAllEnabled()
    {
      return All().Where(x => !x.Disabled);
    }

    public virtual IQueryable<T> GetAllEnabled(int currentId)
    {
      return All().Where(x => !x.Disabled || x.Id == currentId);
    }

    public virtual T GetById(int id)
    {
      return _dbSet.Find(id);
    }

    public virtual T GetByIdAsNoTracking(int id)
    {
      return _dbSet.AsNoTracking().FirstOrDefault(x => x.Id == id);
    }

    public virtual bool Add(T entity)
    {
      if (ValidateBeforeSave(entity))
      {
        entity.ObjectState = ObjectState.Added;
        _dbSet.AddAsync(entity);
        return true;
      }
      return false;
    }

    public virtual async Task<bool> Delete(int id)
    {
      var curr = _dbSet.Where(x => x.Id == id).FirstOrDefault();
      if (curr != null)
      {
        curr.ObjectState = ObjectState.Deleted;
        _dbSet.Remove(curr);
        return true;
      }
      return false;
    }

    public virtual bool Upsert(T entity)
    {
      if (ValidateBeforeSave(entity))
      {
        entity.ObjectState = ObjectState.Modified;
        _dbSet.Update(entity);
        return true;
      }
      return false;
    }

    public virtual bool ValidateBeforeSave(T entity)
    {
      return true;
    }
  }
}
