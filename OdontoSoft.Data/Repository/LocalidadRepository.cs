using OdontoSoft.Data.DAL;
using OdontoSoft.Data.Repository.Interfaces;
using OdontoSoft.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OdontoSoft.Data.Repository
{
  public partial class LocalidadRepository : GenericRepository<Localidad>, ILocalidadRepository
  {
    /*Ejemplo de reemplazo de métodos existentes*/

    //public override Localidad GetById(int id)
    //{
    //    return All()
    //        .Include(x => x.ProvinciaId)
    //        .FirstOrDefault(x=>x.Id == id);
    //}

    //Ejemplo para modificar la lista que llena el browser
    //public override IQueryable<Localidad> AllForBrowser()
    //{
    //    return base.AllForBrowser()
    //        .Include(x => x.Provincia)
    //        .OrderBy(x => x.Descripcion);
    //}


    /*Ejemplo de nuevos métodos - Recuerde ademas modificar la interfaz ILocalidadRepository*/

    //Ejemplo custom de getBy con un solo resultado
    //public Localidad GetByMiCampo(int miCampoId)
    //{
    //    return All().FirstOrDefault(x => x.MicampoId == miCampoId);
    //}

    //Ejemplo custom de getBy con una lista de resultado
    //public IQueryable<Localidad> GetByMiOtroCampo(int miOtroCampoId)
    //{
    //    return All().Where(x => x.MiOtroCampoId == miOtroCampoId);
    //}

    public Localidad GetById(int id)
    {
      return All().FirstOrDefault(x => x.Id == id);
    }

    public override IQueryable<Localidad> AllForBrowser()
    {
      return base.AllForBrowser()
          .Include(x => x.Provincia)
          .OrderBy(x => x.Descripcion);
    }

    public IQueryable<Localidad> GetByProvinciaId(int provinciaId)
    {
      return All().Include(x => x.Provincia).Where(x => x.ProvinciaId == provinciaId).OrderBy(x => x.Descripcion);
    }
  }
}

