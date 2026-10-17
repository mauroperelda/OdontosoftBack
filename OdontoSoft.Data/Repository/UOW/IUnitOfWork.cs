using OdontoSoft.Data.Repository.Interfaces;
//using MedveCore.Entities.Base;
using Mauro.Entities.Base;

namespace OdontoSoft.Data.Repository.UOW
{
  public partial interface IUnitOfWork
  {
    public IStoredProcedureRepository StoredProcedureRepository { get; }
    IQueryable<BaseEntity> GetAll(Type type);
    int Save();
    Task SaveAsync(CancellationToken cancellationToken = default);
  }
}
