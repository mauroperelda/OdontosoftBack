using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OdontoSoft.Data.Authentication;
using OdontoSoft.Data.Repository.UOW;

namespace OdontoSoft.Web.Controllers
{
  public partial class PacienteController : AppBaseBrowserController
  {
    private readonly ILogger<PacienteController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _accessor;
    private readonly LinkGenerator _generator;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IMemoryCache _memoryCache;
    private readonly IAuthenticatedUser _authenticatedUser;

    public PacienteController(ILogger<PacienteController> logger, IUnitOfWork unitOfWork,
        IHttpContextAccessor accessor, LinkGenerator generator, IHostEnvironment hostEnvironment,
        IMemoryCache memoryCache, IAuthenticatedUser authenticatedUser) : base("Paciente")
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
