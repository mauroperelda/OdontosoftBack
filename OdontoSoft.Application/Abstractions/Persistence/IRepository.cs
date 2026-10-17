namespace OdontoSoft.Application.Abstractions.Persistence;

/// <summary>
/// Repositorio genérico (abstracción auxiliar). Las implementaciones concretas
/// viven en OdontoSoft.Infrastructure.Data. Los repos específicos del dominio
/// (IPresupuestoRepository, etc.) pueden heredar de éste para heredar el CRUD
/// genérico, pero en este codebase se optó por mantener las interfaces de
/// MedveCore/Mauro legacy y exponer los métodos específicos desde allí.
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
