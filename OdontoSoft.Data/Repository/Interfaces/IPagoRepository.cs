using OdontoSoft.Domain.Models;

namespace OdontoSoft.Data.Repository.Interfaces
{
  public interface IPagoRepository : IGenericRepository<Pago>
  {
    /*Ejemplo de nuevos métodos - Estos métodos se implementan en PagoRepository*/
    //Pago GetByMiCampo(int miCampoId);
    //IQueryable<Pago> GetByMiOtroCampo(int miOtroCampoId);
    IQueryable<Pago> GetByPresupuestoId(int presupuestoId);
    decimal GetTotalPagadoByPresupuestoId(int presupuestoId);
  }
}

