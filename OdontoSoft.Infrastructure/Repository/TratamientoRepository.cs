using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Infrastructure.Repository
{
  public partial class TratamientoRepository : GenericRepository<Tratamiento>, ITratamientoRepository
  {
    /*Ejemplo de reemplazo de mÃ©todos existentes*/

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
    //        .OrderBy(x=>x.Descripcion);
    //}


    /*Ejemplo de nuevos mÃ©todos - Recuerde ademas modificar la interfaz ILocalidadRepository*/

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
  }
}
