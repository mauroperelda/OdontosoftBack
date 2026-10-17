namespace OdontoSoft.Application.Abstractions.Persistence;

/// <summary>
/// Repositorio genÃ©rico (abstracciÃ³n auxiliar). Las implementaciones concretas
/// viven en OdontoSoft.Infrastructure. Los repos especÃ­ficos del dominio
/// (IPresupuestoRepository, etc.) pueden heredar de Ã©ste para heredar el CRUD
/// genÃ©rico, pero en este codebase se optÃ³ por mantener las interfaces de
/// MedveCore/Mauro legacy y exponer los mÃ©todos especÃ­ficos desde allÃ­.
/// </summary>
public interface IRepository<TEntity> where TEntity : class
{
  TEntity? GetById(int id);
  IQueryable<TEntity> GetAll();
  IQueryable<TEntity> GetAllEnabled();
  void Add(TEntity entity);
  void Update(TEntity entity);
  void Upsert(TEntity entity);
  void Delete(int id);
}
