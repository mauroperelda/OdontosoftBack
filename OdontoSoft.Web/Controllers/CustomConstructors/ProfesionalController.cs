using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OdontoSoft.Application.Abstractions;
using OdontoSoft.Application.Abstractions.Persistence;

namespace OdontoSoft.Web.Controllers
{
  public partial class ProfesionalController : AppBaseBrowserController
  {
    private readonly ILogger<ProfesionalController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _accessor;
    private readonly LinkGenerator _generator;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IMemoryCache _memoryCache;
    private readonly ICurrentUser _authenticatedUser;

    public ProfesionalController(ILogger<ProfesionalController> logger, IUnitOfWork unitOfWork,
        IHttpContextAccessor accessor, LinkGenerator generator, IHostEnvironment hostEnvironment,
        IMemoryCache memoryCache, ICurrentUser authenticatedUser) : base("Profesional")
    {
      _logger = logger;
      _unitOfWork = unitOfWork;
      _accessor = accessor;
      _generator = generator;
      _hostEnvironment = hostEnvironment;
      _memoryCache = memoryCache;
      _authenticatedUser = authenticatedUser;
    }
  }
}
