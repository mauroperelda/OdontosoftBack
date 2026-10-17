using OdontoSoft.Infrastructure.Data.DAL;
using OdontoSoft.Infrastructure.Data.Repository.Interfaces;
using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Domain.Models;
using Microsoft.Extensions.Logging;

namespace OdontoSoft.Infrastructure.Data.Repository
{
  public partial class UbicacionRepository : GenericRepository<Ubicacion>, IUbicacionRepository
  {
    public UbicacionRepository(OdontosoftContext context, ILogger logger, IAuthenticatedUser authenticatedUser) : base(context, logger, authenticatedUser)
    {
    }
  }
}
