using Microsoft.Extensions.Logging;
using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Infrastructure.Data.Repository.UOW;
using Odontosoft.Services;

namespace OdontoSoft.Services
{
  public partial class PresupuestoDetalleServices : BaseServices, IPresupuestoDetalleServices
  {
    public PresupuestoDetalleServices(ILogger<PresupuestoDetalleServices> logger, IUnitOfWork unitOfWork, IAuthenticatedUser authenticatedUser) : base(logger, unitOfWork, authenticatedUser)
    {
    }
  }
}

