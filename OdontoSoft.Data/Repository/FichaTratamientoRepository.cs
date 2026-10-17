using OdontoSoft.Data.DAL;
using OdontoSoft.Data.Repository.Interfaces;
using OdontoSoft.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OdontoSoft.Data.Repository
{
  public partial class FichaTratamientoRepository : GenericRepository<FichaTratamiento>, IFichaTratamientoRepository
  {
    /*Ejemplo de reemplazo de métodos existentes*/
    //public override FichaTratamiento GetById(int id)
    //{{
    //    return All()
    //        .Include(x => x.ProvinciaId)
    //        .FirstOrDefault(x=>x.Id == id);
    //}}
    //public override IQueryable<FichaTratamiento> AllForBrowser()
    //{{
    //    return base.AllForBrowser()
    //        .Include(x => x.Provincia)
    //        .OrderBy(x=>x.Descripcion);
    //}}
    public FichaTratamiento GetFichaTratamientoById(int id)
    {
      {
        return All().Include(x => x.Tratamiento).FirstOrDefault(x => x.Id == id);
      }
    }
    //public IQueryable<FichaTratamiento> GetByMiOtroCampo(int miOtroCampoId)
    //{{
    //    return All().Where(x => x.MiOtroCampoId == miOtroCampoId);
    //}}
    public IQueryable<FichaTratamiento> GetByHistoriaClinicaId(int historiaClinicaId)
    {
      return All()
          .Include(x => x.Tratamiento)
          .Where(x => x.HistoriaClinicaId == historiaClinicaId);
    }

    public IQueryable<FichaTratamiento> GetByTurnoId(int turnoId)
    {
      return All()
          .Include(x => x.Tratamiento)
          .Where(x => x.TurnoId == turnoId);
    }
  }
}

