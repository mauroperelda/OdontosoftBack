using Microsoft.Extensions.Logging;
using OdontoSoft.Data.Authentication;
using OdontoSoft.Data.Repository.UOW;
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

