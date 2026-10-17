using Microsoft.Extensions.Logging;
using Odontosoft.Services;
using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Infrastructure.Data.Repository.UOW;

namespace OdontoSoft.Services
{
  public partial class TratamientoServices : BaseServices, ITratamientoServices
  {
    public TratamientoServices(ILogger<TratamientoServices> logger, IUnitOfWork unitOfWork, IAuthenticatedUser authenticatedUser) : base(logger, unitOfWork, authenticatedUser)
    {
    }
  }
}

