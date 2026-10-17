using OdontoSoft.Domain.Models;

namespace OdontoSoft.Data.Repository.Interfaces
{
  public interface IEstadoTurnoRepository : IGenericRepository<EstadoTurno>
  {
    /*Ejemplo de nuevos métodos - Estos métodos se implementan en EstadoTurnoRepository*/
    //EstadoTurno GetByMiCampo(int miCampoId);
    //IQueryable<EstadoTurno> GetByMiOtroCampo(int miOtroCampoId);
  }
}

