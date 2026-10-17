using OdontoSoft.Data.DAL;
using OdontoSoft.Data.Repository.Interfaces;
using OdontoSoft.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OdontoSoft.Data.Repository
{
  public partial class PresupuestoDetalleRepository : GenericRepository<PresupuestoDetalle>, IPresupuestoDetalleRepository
  {
    /*Ejemplo de reemplazo de métodos existentes*/
    //public override PresupuestoDetalle GetById(int id)
    //{{
    //    return All()
    //        .Include(x => x.ProvinciaId)
    //        .FirstOrDefault(x=>x.Id == id);
    //}}
    //public override IQueryable<PresupuestoDetalle> AllForBrowser()
    //{{
    //    return base.AllForBrowser()
    //        .Include(x => x.Provincia)
    //        .OrderBy(x=>x.Descripcion);
    //}}
    //public PresupuestoDetalle GetByMiCampo(int miCampoId)
    //{{
    //    return All().FirstOrDefault(x => x.MicampoId == miCampoId);
    //}}
    //public IQueryable<PresupuestoDetalle> GetByMiOtroCampo(int miOtroCampoId)
    //{{
    //    return All().Where(x => x.MiOtroCampoId == miOtroCampoId);
    //}}
    public IQueryable<PresupuestoDetalle> GetByPresupuestoId(int presupuestoId)
    {
      return All().Where(x => x.PresupuestoId == presupuestoId);
    }
  }
}

