using OdontoSoft.Infrastructure.DAL;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Application.Abstractions;
using OdontoSoft.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OdontoSoft.Infrastructure.Repository
{
  public partial class TratamientoRepository : GenericRepository<Tratamiento>, ITratamientoRepository
  {
    public TratamientoRepository(OdontosoftContext context, ILogger logger, ICurrentUser authenticatedUser) : base(context, logger, authenticatedUser)
    {
    }
  }
}

