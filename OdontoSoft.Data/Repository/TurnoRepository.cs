using Microsoft.EntityFrameworkCore;
using OdontoSoft.Data.Repository.Interfaces;
using OdontoSoft.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Data.Repository
{
  public partial class TurnoRepository : GenericRepository<Turno>, ITurnoRepository
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
    //        .OrderBy(x=>x.Descripcion);
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

    public Turno GetByTurnoId(int id)
    {
      return All().Include(x => x.Paciente).Include(x => x.Profesional).Include(x => x.Tratamiento).Include(x => x.EstadoTurno).FirstOrDefault(x => x.Id == id);
    }
    public IQueryable<Turno> GetAll()
    {
      return All().Include(x => x.Paciente)
                  .Include(x => x.Profesional)
                  .Include(x => x.Tratamiento)
                  .Include(x => x.EstadoTurno);
    }

    public Turno GetByProfesionalByHoraInicio(int profesionalId, DateTime? horaInicio)
    {
      return All().FirstOrDefault(x => x.ProfesionalId == profesionalId && x.HoraInicio == horaInicio);
    }
  }
}
