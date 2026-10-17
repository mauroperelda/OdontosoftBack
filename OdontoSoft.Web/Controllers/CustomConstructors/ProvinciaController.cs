/* ************************************************************************************************************************************* */
/* En este archivo solo se debe modificar el constructor en caso de ser necesario. No se debe agregar ninguna otra funcionalidad */
/* ************************************************************************************************************************************* */
using OdontoSoft.Infrastructure.Data.Repository.UOW;
using OdontoSoft.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.Rendering;
using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Web.Browser;
using OdontoSoft.Web.Controllers;

namespace OdontoSoft.Web.Controllers
{
  public partial class ProvinciaController : AppBaseBrowserController
  {

    private readonly ILogger<ProvinciaController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _accessor;
    private readonly LinkGenerator _generator;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IMemoryCache _memoryCache;
    private readonly IAuthenticatedUser _authenticatedUser;

    public ProvinciaController(ILogger<ProvinciaController> logger, IUnitOfWork unitOfWork,
        IHttpContextAccessor accessor, LinkGenerator generator,
        IHostEnvironment hostEnvironment, IMemoryCache memoryCache,
        IAuthenticatedUser authenticatedUser) : base("Provincia")
    {
      _logger = logger;
      _unitOfWork = unitOfWork;
      _accessor = accessor;
      _generator = generator;
      _hostEnvironment = hostEnvironment;
      _memoryCache = memoryCache;
      _authenticatedUser = authenticatedUser;
      _browserCacheHelper = new BrowserCacheHelper(_logger, _unitOfWork, _accessor, _generator, _hostEnvironment, _memoryCache, _authenticatedUser);
    }
  }
}

