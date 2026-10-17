using OdontoSoft.Infrastructure.DAL;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Application.Abstractions;
using OdontoSoft.Domain.Models;
using Microsoft.Extensions.Logging;

namespace OdontoSoft.Infrastructure.Repository
{
  public partial class UbicacionRepository : GenericRepository<Ubicacion>, IUbicacionRepository
  {
    public UbicacionRepository(OdontosoftContext context, ILogger logger, ICurrentUser authenticatedUser) : base(context, logger, authenticatedUser)
    {
    }
  }
}
