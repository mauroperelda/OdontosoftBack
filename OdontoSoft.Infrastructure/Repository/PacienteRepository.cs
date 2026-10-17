using Microsoft.EntityFrameworkCore;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Infrastructure.Repository
{
  public partial class PacienteRepository : GenericRepository<Paciente>, IPacienteRepository
  {
    /*Ejemplo de reemplazo de mÃ©todos existentes*/

    //public override Provincia GetById(int id)
    //{
    //    return All()
    //        .Include(x => x.ProvinciaId)
    //        .FirstOrDefault(x=>x.Id == id);
    //}

    //Ejemplo para modificar la lista que llena el browser
    public override IQueryable<Paciente> AllForBrowser()
    {
      return base.AllForBrowser()
          .Include(x => x.Provincia)
          .Include(x => x.Localidad)
          .OrderBy(x => x.Apellido);
    }


    /*Ejemplo de nuevos mÃ©todos - Recuerde ademas modificar la interfaz IProvinciaRepository*/

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

  }
}
