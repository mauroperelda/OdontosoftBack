using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Infrastructure.Data.DAL;
using OdontoSoft.Infrastructure.Data.Repository.Interfaces;
using OdontoSoft.Domain.Models;
using Microsoft.Extensions.Logging;

namespace OdontoSoft.Infrastructure.Data.Repository
{
  public partial class ImagenPacienteRepository : GenericRepository<ImagenPaciente>, IImagenPacienteRepository
  {
    public ImagenPacienteRepository(
      OdontosoftContext context,
      ILogger logger,
      IAuthenticatedUser authenticatedUser)
      : base(context, logger, authenticatedUser)
    {
    }

    public List<ImagenPaciente> GetByHistoriaClinicaId(int historiaClinicaId)
    {
      return All()
        .Where(x => x.HistoriaClinicaId == historiaClinicaId)
        .OrderByDescending(x => x.FechaSubida)
        .ToList();
    }
  }
}
