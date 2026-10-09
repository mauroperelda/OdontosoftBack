using OdontoSoft.Entities.Models;

namespace OdontoSoft.Data.Repository.Interfaces
{
	public interface IFichaTratamientoRepository : IGenericRepository<FichaTratamiento>
	{
		/*Ejemplo de nuevos métodos - Estos métodos se implementan en FichaTratamientoRepository*/
		//FichaTratamiento GetByMiCampo(int miCampoId);
		//IQueryable<FichaTratamiento> GetByMiOtroCampo(int miOtroCampoId);
		IQueryable<FichaTratamiento> GetByHistoriaClinicaId(int historiaClinicaId);
		IQueryable<FichaTratamiento> GetByTurnoId(int turnoId);
		FichaTratamiento GetFichaTratamientoById(int id);
	}
}

