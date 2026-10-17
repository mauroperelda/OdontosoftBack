using Microsoft.Extensions.Logging;
using OdontoSoft.Data.Authentication;
using OdontoSoft.Data.Repository.UOW;
using Odontosoft.Services;

namespace OdontoSoft.Services
{
  public partial class FichaTratamientoServices : BaseServices, IFichaTratamientoServices
  {
    public FichaTratamientoServices(ILogger<FichaTratamientoServices> logger, IUnitOfWork unitOfWork, IAuthenticatedUser authenticatedUser) : base(logger, unitOfWork, authenticatedUser)
    {
    }
  }
}

