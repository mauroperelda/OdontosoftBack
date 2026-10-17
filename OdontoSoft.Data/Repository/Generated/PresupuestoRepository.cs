using OdontoSoft.Data.DAL;
using OdontoSoft.Data.Repository.Interfaces;
using OdontoSoft.Data.Authentication;
using OdontoSoft.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OdontoSoft.Data.Repository
{
  public partial class PresupuestoRepository : GenericRepository<Presupuesto>, IPresupuestoRepository
  {
    public PresupuestoRepository(OdontosoftContext context, ILogger logger, IAuthenticatedUser authenticatedUser) : base(context, logger, authenticatedUser)
    {
    }
  }
}

