using OdontoSoft.Entities.Models;

namespace OdontoSoft.Data.Repository.Interfaces
{
  public interface IPresupuestoDetalleRepository : IGenericRepository<PresupuestoDetalle>
  {
    /*Ejemplo de nuevos métodos - Estos métodos se implementan en PresupuestoDetalleRepository*/
    //PresupuestoDetalle GetByMiCampo(int miCampoId);
    //IQueryable<PresupuestoDetalle> GetByMiOtroCampo(int miOtroCampoId);
    IQueryable<PresupuestoDetalle> GetByPresupuestoId(int presupuestoId);
  }
}

