using OdontoSoft.Domain.Models;

namespace OdontoSoft.Infrastructure.Data.Repository.Interfaces
{
  public interface IProvinciaRepository : IGenericRepository<Provincia>
  {
    /*Ejemplo de nuevos métodos - Estos métodos se implementan en ProvinciaRepository*/

    //Ejemplo custom de getBy con un solo resultado
    //Provincia GetByMiCampo(int miCampoId);

    //Ejemplo custom de getBy con una lista de resultado
    //IQueryable<Provincia> GetByMiOtroCampo(int miOtroCampoId);

    Provincia GetById(int idSRT);
  }
}

