using OdontoSoft.Domain.Models;

namespace OdontoSoft.Data.Repository.Interfaces
{
  public interface IImagenPacienteRepository : IGenericRepository<ImagenPaciente>
  {
    List<ImagenPaciente> GetByHistoriaClinicaId(int historiaClinicaId);
  }
}
