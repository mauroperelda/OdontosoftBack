using OdontoSoft.Data.DAL;
using OdontoSoft.Data.Repository.Interfaces;
using OdontoSoft.Data.Authentication;
using OdontoSoft.Domain.Models;
using Microsoft.Extensions.Logging;

namespace OdontoSoft.Data.Repository
{
  public partial class TratamientoPrecioRepository : GenericRepository<TratamientoPrecio>, ITratamientoPrecioRepository
  {
    public TratamientoPrecioRepository(OdontosoftContext context, ILogger logger, IAuthenticatedUser authenticatedUser) : base(context, logger, authenticatedUser)
    {
    }
  }
}
