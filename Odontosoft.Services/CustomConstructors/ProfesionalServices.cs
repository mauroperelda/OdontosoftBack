using Microsoft.Extensions.Logging;
using Odontosoft.Services;
using OdontoSoft.Data.Authentication;
using OdontoSoft.Data.Repository.UOW;

namespace OdontoSoft.Services
{
    public partial class ProfesionalServices : BaseServices, IProfesionalServices
    {
        public ProfesionalServices(ILogger<ProfesionalServices> logger, IUnitOfWork unitOfWork, IAuthenticatedUser authenticatedUser) : base (logger, unitOfWork, authenticatedUser)
        {
        }
    }
}

