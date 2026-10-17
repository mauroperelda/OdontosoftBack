/* ************************************************************************************************************************************* */
/* En este archivo solo se debe modificar el constructor en caso de ser necesario. No se debe agregar ninguna otra funcionalidad */
/* ************************************************************************************************************************************* */
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.Rendering;
using OdontoSoft.Application.Abstractions;
using OdontoSoft.Web.Browser;
using OdontoSoft.Web.Controllers;
using OdontoSoft.Application.Services;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Infrastructure.Data.Repository;

namespace OdontoSoft.Web.Controllers
{
  public partial class LocalidadController : AppBaseBrowserController
  {
    private readonly ILogger<LocalidadController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _accessor;
    private readonly LinkGenerator _generator;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IMemoryCache _memoryCache;
    private readonly ICurrentUser _authenticatedUser;
    private readonly ILocalidadServices _localidadServices;
    private readonly IConfiguration _configuration;

    public LocalidadController(ILogger<LocalidadController> logger, IUnitOfWork unitOfWork,
        IHttpContextAccessor accessor, LinkGenerator generator,
        IHostEnvironment hostEnvironment, IMemoryCache memoryCache,
        ICurrentUser authenticatedUser, ILocalidadServices localidadServices, IConfiguration configuration) : base("Localidad")
    {
      _logger = logger;
      _unitOfWork = unitOfWork;
      _accessor = accessor;
      _generator = generator;
      _hostEnvironment = hostEnvironment;
      _memoryCache = memoryCache;
      _authenticatedUser = authenticatedUser;
      _localidadServices = localidadServices;
      _configuration = configuration;
      _browserCacheHelper = new BrowserCacheHelper(_logger, _unitOfWork, _accessor, _generator, _hostEnvironment, _memoryCache, _authenticatedUser);
    }
  }
}

