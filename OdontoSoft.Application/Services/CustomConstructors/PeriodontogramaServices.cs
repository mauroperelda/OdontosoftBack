using Microsoft.Extensions.Logging;
using OdontoSoft.Application.Abstractions;
using OdontoSoft.Application.Abstractions.Persistence;

using OdontoSoft.Application.Services.Base;

namespace OdontoSoft.Application.Services {
  public partial class PeriodontogramaServices : BaseServices, IPeriodontogramaServices
  {
    public PeriodontogramaServices(ILogger<PeriodontogramaServices> logger, IUnitOfWork unitOfWork, ICurrentUser authenticatedUser) : base(logger, unitOfWork, authenticatedUser)
    {
    }
  }
}