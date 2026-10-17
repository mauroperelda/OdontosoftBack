using Microsoft.Extensions.Logging;
using Odontosoft.Services;
using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Infrastructure.Data.Repository.UOW;

namespace OdontoSoft.Services
{
  public partial class PacienteServices : BaseServices, IPacienteServices
  {
    public PacienteServices(ILogger<PacienteServices> logger, IUnitOfWork unitOfWork, IAuthenticatedUser authenticatedUser) : base(logger, unitOfWork, authenticatedUser)
    {
    }
  }
}

