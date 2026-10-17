using Microsoft.Extensions.Logging;
using Odontosoft.Services;
using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Infrastructure.Data.Repository.Interfaces;
using OdontoSoft.Infrastructure.Data.Repository.UOW;

namespace OdontoSoft.Services
{
  public partial class LocalidadServices : BaseServices, ILocalidadServices
  {
    private readonly HttpClient _httpClient;
    public LocalidadServices(ILogger<LocalidadServices> logger, IUnitOfWork unitOfWork, IAuthenticatedUser authenticatedUser, HttpClient httpClient) : base(logger, unitOfWork, authenticatedUser)
    {
      _httpClient = httpClient;
    }
  }
}

