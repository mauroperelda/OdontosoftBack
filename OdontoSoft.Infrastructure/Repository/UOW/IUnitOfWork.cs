using OdontoSoft.Application.Abstractions.Persistence;
//using MedveCore.Entities.Base;
using Mauro.Entities.Base;

namespace OdontoSoft.Infrastructure.Repository.UOW;

/// <summary>
/// Extiende la abstracciÃ³n de Application con mÃ©todos legacy del Browser
/// DevExtreme. Mantener acÃ¡ porque el Browser es un mÃ³dulo legacy del Web
/// host. La composiciÃ³n root debe registrar IUnitOfWork (de Application).
/// </summary>
public partial interface IUnitOfWork : OdontoSoft.Application.Abstractions.Persistence.IUnitOfWork
{
  IQueryable<BaseEntity> GetAll(Type type);
}
