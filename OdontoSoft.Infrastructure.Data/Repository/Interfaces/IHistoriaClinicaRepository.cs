using OdontoSoft.Domain.Models;

namespace OdontoSoft.Infrastructure.Data.Repository.Interfaces
{
  public interface IHistoriaClinicaRepository : IGenericRepository<HistoriaClinica>
  {
    /*Ejemplo de nuevos métodos - Estos métodos se implementan en HistoriaClinicaRepository*/
    //HistoriaClinica GetByMiCampo(int miCampoId);
    //IQueryable<HistoriaClinica> GetByMiOtroCampo(int miOtroCampoId);
    HistoriaClinica GetByPacienteId(int pacienteId);
    HistoriaClinica GetFullByPacienteId(int pacienteId);
    HistoriaClinica GetHistoriaClinicaById(int id);
  }
}

