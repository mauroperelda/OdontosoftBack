using OdontoSoft.Entities.Models;

namespace OdontoSoft.Data.Repository.Interfaces
{
  public interface ILocalidadRepository : IGenericRepository<Localidad>
  {
    /*Ejemplo de nuevos métodos - Estos métodos se implementan en LocalidadRepository*/

    //Ejemplo custom de getBy con un solo resultado
    //Localidad GetByMiCampo(int miCampoId);

    //Ejemplo custom de getBy con una lista de resultado
    //IQueryable<Localidad> GetByMiOtroCampo(int miOtroCampoId);

    Localidad GetById(int id);
    IQueryable<Localidad> GetByProvinciaId(int provinciaId);
  }
}

