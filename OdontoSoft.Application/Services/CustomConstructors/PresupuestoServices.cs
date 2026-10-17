using Microsoft.Extensions.Logging;
using OdontoSoft.Application.Abstractions;
using OdontoSoft.Application.Abstractions.Persistence;

using OdontoSoft.Application.Services.Base;

namespace OdontoSoft.Application.Services {
  public partial class PresupuestoServices : BaseServices, IPresupuestoServices
  {
    public PresupuestoServices(ILogger<PresupuestoServices> logger, IUnitOfWork unitOfWork, ICurrentUser authenticatedUser) : base(logger, unitOfWork, authenticatedUser)
    {
    }
  }
}