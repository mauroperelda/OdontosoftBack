using Microsoft.Extensions.Logging;

using OdontoSoft.Application.Abstractions;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Application.Services.Base;

namespace OdontoSoft.Application.Services {
  public partial class LocalidadServices : BaseServices, ILocalidadServices
  {
    private readonly HttpClient _httpClient;
    public LocalidadServices(ILogger<LocalidadServices> logger, IUnitOfWork unitOfWork, ICurrentUser authenticatedUser, HttpClient httpClient) : base(logger, unitOfWork, authenticatedUser)
    {
      _httpClient = httpClient;
    }
  }
}