using Microsoft.Extensions.Logging;
using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Infrastructure.Data.Repository.UOW;
using Odontosoft.Services;

namespace OdontoSoft.Services
{
  public partial class HistoriaClinicaServices : BaseServices, IHistoriaClinicaServices
  {
    public HistoriaClinicaServices(ILogger<HistoriaClinicaServices> logger, IUnitOfWork unitOfWork, IAuthenticatedUser authenticatedUser) : base(logger, unitOfWork, authenticatedUser)
    {
    }
  }
}

