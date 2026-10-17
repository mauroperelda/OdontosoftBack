using OdontoSoft.Infrastructure.Data.DAL;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OdontoSoft.Infrastructure.Data.Repository
{
  public partial class OdontogramaPiezaRepository : GenericRepository<OdontogramaPieza>, IOdontogramaPiezaRepository
  {
    /*Ejemplo de reemplazo de métodos existentes*/
    //public override OdontogramaPieza GetById(int id)
    //{{
    //    return All()
    //        .Include(x => x.ProvinciaId)
    //        .FirstOrDefault(x=>x.Id == id);
    //}}
    //public override IQueryable<OdontogramaPieza> AllForBrowser()
    //{{
    //    return base.AllForBrowser()
    //        .Include(x => x.Provincia)
    //        .OrderBy(x=>x.Descripcion);
    //}}
    //public OdontogramaPieza GetByMiCampo(int miCampoId)
    //{{
    //    return All().FirstOrDefault(x => x.MicampoId == miCampoId);
    //}}
    //public IQueryable<OdontogramaPieza> GetByMiOtroCampo(int miOtroCampoId)
    //{{
    //    return All().Where(x => x.MiOtroCampoId == miOtroCampoId);
    //}}
    public IQueryable<OdontogramaPieza> GetByHistoriaClinicaId(int historiaClinicaId)
    {
      return All().Where(x => x.HistoriaClinicaId == historiaClinicaId);
    }

    public OdontogramaPieza GetByHistoriaClinicaIdAndNumeroPieza(int historiaClinicaId, int numeroPieza)
    {
      return All().FirstOrDefault(x => x.HistoriaClinicaId == historiaClinicaId && x.NumeroPieza == numeroPieza);
    }
  }
}

