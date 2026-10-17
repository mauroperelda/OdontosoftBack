using OdontoSoft.Infrastructure.DAL;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OdontoSoft.Infrastructure.Repository
{
  public partial class PresupuestoDetalleRepository : GenericRepository<PresupuestoDetalle>, IPresupuestoDetalleRepository
  {
    /*Ejemplo de reemplazo de mÃ©todos existentes*/
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

