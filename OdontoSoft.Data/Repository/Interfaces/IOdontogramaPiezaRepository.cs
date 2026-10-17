using OdontoSoft.Domain.Models;

namespace OdontoSoft.Data.Repository.Interfaces
{
  public interface IOdontogramaPiezaRepository : IGenericRepository<OdontogramaPieza>
  {
    /*Ejemplo de nuevos métodos - Estos métodos se implementan en OdontogramaPiezaRepository*/
    //OdontogramaPieza GetByMiCampo(int miCampoId);
    //IQueryable<OdontogramaPieza> GetByMiOtroCampo(int miOtroCampoId);
    IQueryable<OdontogramaPieza> GetByHistoriaClinicaId(int historiaClinicaId);
    OdontogramaPieza GetByHistoriaClinicaIdAndNumeroPieza(int historiaClinicaId, int numeroPieza);
  }
}

