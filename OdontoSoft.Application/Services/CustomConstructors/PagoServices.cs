using Microsoft.Extensions.Logging;
using OdontoSoft.Application.Abstractions;
using OdontoSoft.Application.Abstractions.Persistence;

using OdontoSoft.Application.Services.Base;

namespace OdontoSoft.Application.Services {
  public partial class PagoServices : BaseServices, IPagoServices
  {
    public PagoServices(ILogger<PagoServices> logger, IUnitOfWork unitOfWork, ICurrentUser authenticatedUser) : base(logger, unitOfWork, authenticatedUser)
    {
    }
  }
}