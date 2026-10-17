using OdontoSoft.Application.Abstractions;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Domain.Models;
using OdontoSoft.Web.Controllers;
using Microsoft.Extensions.Caching.Memory;
using OdontoSoft.Web.Browser;

namespace OdontoSoft.Web.Browser
{
  public partial class BrowserCacheHelper
  {
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _accessor;
    private readonly LinkGenerator _generator;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IMemoryCache _memoryCache;
    private readonly ICurrentUser _authenticatedUser;

    public BrowserCacheHelper(ILogger logger, IUnitOfWork unitOfWork
        , IHttpContextAccessor accessor, LinkGenerator generator
        , IHostEnvironment hostEnvironment, IMemoryCache memoryCache
        , ICurrentUser authenticatedUser)
    {
      _logger = logger;
      _unitOfWork = unitOfWork;
      _accessor = accessor;
      _generator = generator;
      _hostEnvironment = hostEnvironment;
      _memoryCache = memoryCache;
      _authenticatedUser = authenticatedUser;
    }

    public void AddBrowsers()
    {
      _memoryCache.Set("BROWSER_EstadoTurno".ToUpper(), new Browser<EstadoTurno>(_accessor, _generator, _hostEnvironment, _authenticatedUser));
      _memoryCache.Set("BROWSER_FichaTratamiento".ToUpper(), new Browser<FichaTratamiento>(_accessor, _generator, _hostEnvironment, _authenticatedUser));
      _memoryCache.Set("BROWSER_HistoriaClinica".ToUpper(), new Browser<HistoriaClinica>(_accessor, _generator, _hostEnvironment, _authenticatedUser));
      _memoryCache.Set("BROWSER_Localidad".ToUpper(), new Browser<Localidad>(_accessor, _generator, _hostEnvironment, _authenticatedUser));
      _memoryCache.Set("BROWSER_OdontogramaPieza".ToUpper(), new Browser<OdontogramaPieza>(_accessor, _generator, _hostEnvironment, _authenticatedUser));
      _memoryCache.Set("BROWSER_Paciente".ToUpper(), new Browser<Paciente>(_accessor, _generator, _hostEnvironment, _authenticatedUser));
      _memoryCache.Set("BROWSER_Pago".ToUpper(), new Browser<Pago>(_accessor, _generator, _hostEnvironment, _authenticatedUser));
      _memoryCache.Set("BROWSER_Presupuesto".ToUpper(), new Browser<Presupuesto>(_accessor, _generator, _hostEnvironment, _authenticatedUser));
      _memoryCache.Set("BROWSER_PresupuestoDetalle".ToUpper(), new Browser<PresupuestoDetalle>(_accessor, _generator, _hostEnvironment, _authenticatedUser));
      _memoryCache.Set("BROWSER_Profesional".ToUpper(), new Browser<Profesional>(_accessor, _generator, _hostEnvironment, _authenticatedUser));
      _memoryCache.Set("BROWSER_Provincia".ToUpper(), new Browser<Provincia>(_accessor, _generator, _hostEnvironment, _authenticatedUser));
      _memoryCache.Set("BROWSER_Tratamiento".ToUpper(), new Browser<Tratamiento>(_accessor, _generator, _hostEnvironment, _authenticatedUser));
      _memoryCache.Set("BROWSER_Turno".ToUpper(), new Browser<Turno>(_accessor, _generator, _hostEnvironment, _authenticatedUser));
    }

    public IBrowser GetBrowser(string code)
    {
      IBrowser b;
      code = ("BROWSER_" + code).ToUpper();
      if (!this._memoryCache.TryGetValue(code, out b))
        AddBrowsers();
      if (!this._memoryCache.TryGetValue(code, out b))
        throw new Exception("BROWSER_" + code.ToUpper() + ". Browser is not registered. Please regenerate Browser Registration file.");
      if (b == null)
        throw new Exception("BROWSER_" + code.ToUpper() + ". Browser is not registered. Please regenerate Browser Registration file.");
      return b;
    }
  }
}

