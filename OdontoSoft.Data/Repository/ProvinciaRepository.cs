using OdontoSoft.Data.DAL;
using OdontoSoft.Data.Repository.Interfaces;
using OdontoSoft.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OdontoSoft.Data.Repository
{
  public partial class ProvinciaRepository : GenericRepository<Provincia>, IProvinciaRepository
  {
    /*Ejemplo de reemplazo de métodos existentes*/

    //public override Provincia GetById(int id)
    //{
    //    return All()
    //        .Include(x => x.ProvinciaId)
    //        .FirstOrDefault(x=>x.Id == id);
    //}

    //Ejemplo para modificar la lista que llena el browser
    //public override IQueryable<Provincia> AllForBrowser()
    //{
    //    return base.AllForBrowser()
    //        .Include(x => x.Provincia)
    //        .OrderBy(x=>x.Descripcion);
    //}


    /*Ejemplo de nuevos métodos - Recuerde ademas modificar la interfaz IProvinciaRepository*/

    //Ejemplo custom de getBy con un solo resultado
    //public Provincia GetByMiCampo(int miCampoId)
    //{
    //    return All().FirstOrDefault(x => x.MicampoId == miCampoId);
    //}

    //Ejemplo custom de getBy con una lista de resultado
    //public IQueryable<Provincia> GetByMiOtroCampo(int miOtroCampoId)
    //{
    //    return All().Where(x => x.MiOtroCampoId == miOtroCampoId);
    //}

    public Provincia GetById(int id)
    {
      return All().FirstOrDefault(x => x.Id == id);
    }
  }
}

