using Microsoft.Extensions.Logging;
using OdontoSoft.Application.Abstractions;
using OdontoSoft.Application.Abstractions.Persistence;

using OdontoSoft.Application.Services.Base;

namespace OdontoSoft.Application.Services {
  public partial class PresupuestoDetalleServices : BaseServices, IPresupuestoDetalleServices
  {
    public PresupuestoDetalleServices(ILogger<PresupuestoDetalleServices> logger, IUnitOfWork unitOfWork, ICurrentUser authenticatedUser) : base(logger, unitOfWork, authenticatedUser)
    {
    }
  }
}