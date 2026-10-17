using OdontoSoft.Application.Abstractions;
using OdontoSoft.Infrastructure.Data.DAL;
//using OdontoSoft.Infrastructure.Data.Helpers;
using OdontoSoft.Infrastructure.Data.Repository.Generic;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Infrastructure.Data.Repository.StoredProcedure;
using OdontoSoft.Domain.Models;
//using MedveCore.Entities.Base;
using Mauro.Entities.Base;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace OdontoSoft.Infrastructure.Data.Repository.UOW
{
  public partial class UnitOfWork : IUnitOfWork, IDisposable
  {
    private readonly OdontosoftContext _context;

    private readonly ILogger _logger;

    private readonly ICurrentUser _authenticatedUser;

    public IStoredProcedureRepository StoredProcedureRepository { get; private set; }


    public UnitOfWork(OdontosoftContext context, ILogger<UnitOfWork> logger, ICurrentUser authenticatedUser)
    {
      _context = context;
      _logger = logger;
      _authenticatedUser = authenticatedUser;

      InstanceRepos();
      StoredProcedureRepository = new StoredProcedureRepository(_context, _logger);
    }

    public int Save()
    {
      return _context.SaveChanges();
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
      await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
      _context.Dispose();
    }

    public IQueryable<BaseEntity> GetAll(Type type)
    {

      PropertyInfo[] props = this.GetType().GetProperties().ToArray();

      foreach (var item in props)
      {
        var b = item.GetValue(this, null) as IBrowseable;
        if (b != null)
        {
          foreach (var interf in ((System.Type[])((System.Reflection.TypeInfo)(b).GetType()).ImplementedInterfaces))
          {
            if (interf.Name == "IGenericRepository`1")
            {
              if (interf.GenericTypeArguments[0].Name == type.Name)
              {
                return ((IBrowseable)item.GetValue(this, null)).GetBrowser();
              }
            }
          }
        }

      }
      return null;

    }
  }
}
