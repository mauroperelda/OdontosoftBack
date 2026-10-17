using OdontoSoft.Application.Abstractions.Persistence;
//using MedveCore.Entities.Base;
using Mauro.Entities.Base;

namespace OdontoSoft.Infrastructure.Data.Repository.UOW;

/// <summary>
/// Extiende la abstracción de Application con métodos legacy del Browser
/// DevExtreme. Mantener acá porque el Browser es un módulo legacy del Web
/// host. La composición root debe registrar IUnitOfWork (de Application).
/// </summary>
public partial interface IUnitOfWork : OdontoSoft.Application.Abstractions.Persistence.IUnitOfWork
{
  IQueryable<BaseEntity> GetAll(Type type);
}
