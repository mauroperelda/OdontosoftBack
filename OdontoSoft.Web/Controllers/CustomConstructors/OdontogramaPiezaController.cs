using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.Rendering;
using OdontoSoft.Data.Authentication;
using OdontoSoft.Web.Browser;

namespace OdontoSoft.Web.Controllers
{
  public partial class OdontogramaPiezaController : AppBaseBrowserController
  {
    private readonly ILogger<OdontogramaPiezaController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _accessor;
    private readonly LinkGenerator _generator;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IMemoryCache _memoryCache;
    private readonly IAuthenticatedUser _authenticatedUser;

    public OdontogramaPiezaController(ILogger<OdontogramaPiezaController> logger, IUnitOfWork unitOfWork,
        IHttpContextAccessor accessor, LinkGenerator generator,
        IHostEnvironment hostEnvironment, IMemoryCache memoryCache,
        IAuthenticatedUser authenticatedUser) : base("OdontogramaPieza")
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

