using OdontoSoft.Infrastructure.Data.DAL;
using OdontoSoft.Infrastructure.Data.Repository.Interfaces;
using OdontoSoft.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OdontoSoft.Infrastructure.Data.Repository
{
  public partial class PagoRepository : GenericRepository<Pago>, IPagoRepository
  {
    /*Ejemplo de reemplazo de métodos existentes*/
    //public override Pago GetById(int id)
    //{{
    //    return All()
    //        .Include(x => x.ProvinciaId)
    //        .FirstOrDefault(x=>x.Id == id);
    //}}
    //public override IQueryable<Pago> AllForBrowser()
    //{{
    //    return base.AllForBrowser()
    //        .Include(x => x.Provincia)
    //        .OrderBy(x=>x.Descripcion);
    //}}
    //public Pago GetByMiCampo(int miCampoId)
    //{{
    //    return All().FirstOrDefault(x => x.MicampoId == miCampoId);
    //}}
    //public IQueryable<Pago> GetByMiOtroCampo(int miOtroCampoId)
    //{{
    //    return All().Where(x => x.MiOtroCampoId == miOtroCampoId);
    //}}
    public IQueryable<Pago> GetByPresupuestoId(int presupuestoId)
    {
      return All().Where(x => x.PresupuestoId == presupuestoId);
    }

    public decimal GetTotalPagadoByPresupuestoId(int presupuestoId)
    {
      return All()
          .Where(x => x.PresupuestoId == presupuestoId)
          .Sum(x => x.Monto);
    }
  }
}

