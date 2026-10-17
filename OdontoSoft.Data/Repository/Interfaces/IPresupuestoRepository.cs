using OdontoSoft.Entities.Models;

namespace OdontoSoft.Data.Repository.Interfaces
{
  public interface IPresupuestoRepository : IGenericRepository<Presupuesto>
  {
    /*Ejemplo de nuevos métodos - Estos métodos se implementan en PresupuestoRepository*/
    //Presupuesto GetByMiCampo(int miCampoId);
    //IQueryable<Presupuesto> GetByMiOtroCampo(int miOtroCampoId);
    IQueryable<Presupuesto> GetByPacienteId(int pacienteId);
    Presupuesto GetPresupuestoById(int id);
  }
}

