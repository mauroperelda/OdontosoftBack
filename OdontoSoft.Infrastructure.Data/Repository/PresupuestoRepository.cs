using OdontoSoft.Infrastructure.Data.DAL;
using OdontoSoft.Infrastructure.Data.Repository.Interfaces;
using OdontoSoft.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OdontoSoft.Infrastructure.Data.Repository
{
  public partial class PresupuestoRepository : GenericRepository<Presupuesto>, IPresupuestoRepository
  {
    /*Ejemplo de reemplazo de métodos existentes*/
    //public override Presupuesto GetById(int id)
    //{{
    //    return All()
    //        .Include(x => x.ProvinciaId)
    //        .FirstOrDefault(x=>x.Id == id);
    //}}
    public override IQueryable<Presupuesto> AllForBrowser()
    {
      {
        return base.AllForBrowser()
            .Include(x => x.Paciente)
            .Include(x => x.Profesional);
      }
    }
    //public Presupuesto GetByMiCampo(int miCampoId)
    //{{
    //    return All().FirstOrDefault(x => x.MicampoId == miCampoId);
    //}}
    //public IQueryable<Presupuesto> GetByMiOtroCampo(int miOtroCampoId)
    //{{
    //    return All().Where(x => x.MiOtroCampoId == miOtroCampoId);
    //}}
    public IQueryable<Presupuesto> GetByPacienteId(int pacienteId)
    {
      return All()
          .Include(x => x.Paciente)
          .Include(x => x.Profesional)
          .Where(x => x.PacienteId == pacienteId);
    }

    public Presupuesto GetPresupuestoById(int id)
    {
      return All()
          .Include(x => x.Paciente)
          .Include(x => x.Profesional)
          .Include(x => x.Detalles).ThenInclude(d => d.Tratamiento)
          .Include(x => x.Pagos)
          .FirstOrDefault(x => x.Id == id);
    }
  }
}

