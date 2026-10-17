//using OdontoSoft.Infrastructure.Data.Authentication;
//using ExtraCore.Data.Repository.UOW;
//using ExtraCore.Web.Helpers.Constants;
using OdontoSoft.Web.Helpers.Menu;
//using ExtraCore.Web.Medve.Browser;
using Microsoft.Extensions.Caching.Memory;
using OdontoSoft.Infrastructure.Data.Repository.UOW;
using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Web.Browser;
using OdontoSoft.Web.Helpers.Constants;

namespace OdontoSoft.Web.Helpers.Application
{
  public class ApplicationMenu
  {
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _accessor;
    private readonly LinkGenerator _generator;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IMemoryCache _memoryCache;
    private readonly IAuthenticatedUser _authenticatedUser;
    private readonly BrowserCacheHelper _browserCacheHelper;

    public ApplicationMenu(ILogger<ApplicationMenu> logger, IUnitOfWork unitOfWork,
        IHttpContextAccessor accessor, LinkGenerator generator,
        IHostEnvironment hostEnvironment, IMemoryCache memoryCache,
        IAuthenticatedUser authenticatedUser)
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

    private HttpContext GetContext()
    {
      return _accessor.HttpContext;
    }
    private ISession GetSession()
    {
      return _accessor.HttpContext.Session;
    }

    private const string SESSION_MODULO_SELECTED = "CodModulo";

    private string GetSessionValue(string key)
    {
      return GetSession().GetString(key);
    }
    private string GetClaimValue(string key)
    {
      return GetContext().User.Claims.Any(r => r.Type == key) ? GetContext().User.Claims.First(r => r.Type == key).Value : null;
    }
    private void SetSessionValue(string key, string value)
    {
      GetSession().SetString(key, value);
    }

    private void RemoveSession(string key)
    {
      GetSession().Remove(key);
    }

    public int? CodigoModulo()
    {
      string Value = GetSessionValue(SESSION_MODULO_SELECTED);
      return string.IsNullOrWhiteSpace(Value) ? new int?() : Convert.ToInt32(Value);
    }
    public void CodigoModulo(int idModulo)
    {
      SetSessionValue(SESSION_MODULO_SELECTED, idModulo.ToString());
    }




    public List<Modulo> GetModulos()
    {
      List<Modulo> listModulos = new List<Modulo>();

      //var permisos = Permisos();

      Modulo mPlanificacion = new Modulo() { Id = Modulos.INICIAL, Name = "Menu" };

      #region Planificacion
      var menuPlanificacion = GetMenuInicial(/*permisos*/);
      if (menuPlanificacion.Count > 0)
      {
        mPlanificacion.Menu = menuPlanificacion;
        listModulos.Add(mPlanificacion);
      }
      #endregion


      if (listModulos.Any())
      {
        int? moduloSelected = CodigoModulo();

        if (!moduloSelected.HasValue || !listModulos.Any(m => m.Id == moduloSelected))
          CodigoModulo(listModulos.First().Id);

      }

      return listModulos;
    }

    private void AddBrowserMenuItem(MenuCategory addTo, string browserCode, int menuId)
    {
      var b = _browserCacheHelper.GetBrowser(browserCode);
      if (!b.IsAllowed()) return;

      addTo.Items.Add(new MenuItem(menuId, b.GetBrowserModel().Settings.Title, "Browser", browserCode, b.GetBrowserModel().Settings.Icon));
    }


    private MenuCategory GetMenuPacientes(int id)
    {
      int idInternal = id * 100000;
      string controller = "Pacientes";
      MenuCategory menu = new MenuCategory(controller, "", "Paciente" + idInternal.ToString(), true);

      #region Prestadores
      MenuCategory menuPacientes = new MenuCategory("Pacientes", "uil-bookmark-full", "Paciente" + idInternal.ToString(), true);
      menuPacientes.Items = new List<MenuItem>();

      AddBrowserMenuItem(menuPacientes, "Paciente", ++idInternal);
      //menuPrestadores.Items.Add(new MenuItem(++idInternal, "Pacientes", "Paciente", "Browser", "uil-cloud-check"));

      menu.Submenus = new List<MenuCategory>();
      menu.Submenus.Add(menuPacientes);

      //menuServicios.Items.Add(new MenuItem(++idInternal, "ABM Prestadores", "Prestadores", "Busqueda", "uil-bookmark-full"));
      //menuServicios.Items.Add(new MenuItem(++idInternal, "Próximos servicios", "Servicio", "Proximos", "uil-bookmark-full"));
      //menuServicios.Items.Add(new MenuItem(++idInternal, "Nuevo servicio", "Servicio", "PreCreate", "uil-bookmark-full"));
      //AddBrowserMenuItem(menuServicios, "ServicioExterno", ++idInternal);
      //menuServicios.Items.Add(new MenuItem(++idInternal, "Historia clínica", "Vehiculo", "History", "uil-bookmark-full"));
      #endregion


      //#region Tablas
      //MenuCategory menuTablas = new MenuCategory("Tablas", "uil-bookmark-full", "Tablas" + idInternal.ToString(), true);
      //menuTablas.Items = new List<MenuItem>();
      //AddBrowserMenuItem(menuTablas, "Provincia", ++idInternal);
      //AddBrowserMenuItem(menuTablas, "Localidad", ++idInternal);
      //AddBrowserMenuItem(menuTablas, "PrestadorTipo", ++idInternal);
      //AddBrowserMenuItem(menuTablas, "Servicio", ++idInternal);
      //menuTablas.Items.Add(new MenuItem(++idInternal, "Sincronizar", "Prestador", "SincronizacionTablas", "uil-bookmark-full"));
      //#endregion



      //#region Novedades
      //MenuCategory menuNovedades = new MenuCategory("Novedades", "uil-bookmark-full", "Novedades" + idInternal.ToString(), true);
      //menuNovedades.Items = new List<MenuItem>();
      //AddBrowserMenuItem(menuNovedades, "Novedad", ++idInternal);
      //#endregion

      //menu.Submenus = new List<MenuCategory>();
      //menu.Submenus.Add(menuPrestadores);
      //menu.Submenus.Add(menuTablas);
      //menu.Submenus.Add(menuClientes);

      //if (_authenticatedUser.GetAdministrador())
      //    menu.Submenus.Add(menuNovedades);

      return menu;
    }


    private MenuCategory GetMenuProfesionales(int id)
    {
      int idInternal = id * 100000;
      string controller = "Profesionales";
      MenuCategory menu = new MenuCategory(controller, "", "Profesionales" + idInternal.ToString(), true);

      #region Prestadores
      MenuCategory menuProfesionales = new MenuCategory("Profesionales", "uil-bookmark-full", "Profesional" + idInternal.ToString(), true);
      menuProfesionales.Items = new List<MenuItem>();
      AddBrowserMenuItem(menuProfesionales, "Profesional", ++idInternal);
      AddBrowserMenuItem(menuProfesionales, "Tratamiento", ++idInternal);

      //menuProfesionales.Items.Add(new MenuItem(++idInternal, "Tratamientos", "Tratamiento", "Establecimientos", "uil-cloud-check"));
      #endregion

      menu.Submenus = new List<MenuCategory>();
      menu.Submenus.Add(menuProfesionales);

      return menu;
    }

    private MenuCategory GetMenuTurno(int id)
    {
      int idInternal = id * 100000;
      string controller = "Turnos";
      MenuCategory menu = new MenuCategory(controller, "", "Turnos" + idInternal.ToString(), true);

      #region Prestadores
      MenuCategory menuProfesionales = new MenuCategory("Turnos", "uil-bookmark-full", "Turno" + idInternal.ToString(), true);
      menuProfesionales.Items = new List<MenuItem>();
      //AddBrowserMenuItem(menuProfesionales, "Profesional", ++idInternal);

      menuProfesionales.Items.Add(new MenuItem(++idInternal, "Calendario", "Home", "Index", "uil-bookmark-full"));
      //menuProfesionales.Items.Add(new MenuItem(++idInternal, "Turnos", "Turno", "Edit", "uil-bookmark-full"));
      #endregion

      menu.Submenus = new List<MenuCategory>();
      menu.Submenus.Add(menuProfesionales);

      return menu;
    }

    private MenuCategory GetMenuConfiguracion(int id)
    {
      int idInternal = id * 100000;
      string controller = "Configuración";
      MenuCategory menu = new MenuCategory(controller, "", "Configuracion" + idInternal.ToString(), true);

      #region Tablas
      MenuCategory menuTablas = new MenuCategory("Tablas", "uil-setting", "Tablas" + idInternal.ToString(), true);
      menuTablas.Items = new List<MenuItem>();
      AddBrowserMenuItem(menuTablas, "Provincia", ++idInternal);
      AddBrowserMenuItem(menuTablas, "Localidad", ++idInternal);
      //AddBrowserMenuItem(menuTablas, "PrestadorTipo", ++idInternal);
      //AddBrowserMenuItem(menuTablas, "Servicio", ++idInternal);
      AddBrowserMenuItem(menuTablas, "EstadoTurno", ++idInternal);
      //menuTablas.Items.Add(new MenuItem(++idInternal, "Sincronizar", "Localidad", "SincronizarLocalidades", "uil-bookmark-full"));
      #endregion


      #region Seguridad
      //MenuCategory menuSeguridad = new MenuCategory("Seguridad", "uil-bookmark-full", "Seguridad" + idInternal.ToString(), true);
      //menuSeguridad.Items = new List<MenuItem>();
      //AddBrowserMenuItem(menuSeguridad, "Sucursal", ++idInternal);
      //AddBrowserMenuItem(menuSeguridad, "Usuario", ++idInternal);
      #endregion

      #region Avanzado
      //MenuCategory menuAvanzado = new MenuCategory("Avanzadas", "uil-bookmark-full", "Avanzadas" + idInternal.ToString(), true);
      //menuAvanzado.Items = new List<MenuItem>();
      //AddBrowserMenuItem(menuAvanzado, "DocumentoTipo", ++idInternal);
      #endregion

      #region Tipos de servicios
      //MenuCategory menuTipoServicio = new MenuCategory("Configuración servicios", "uil-bookmark-full", "ConfiguracionServicios" + idInternal.ToString(), true);
      //menuTipoServicio.Items = new List<MenuItem>();
      //AddBrowserMenuItem(menuTipoServicio, "ServicioTipo", ++idInternal);
      //AddBrowserMenuItem(menuTipoServicio, "ServicioTipoSeccion", ++idInternal);
      //AddBrowserMenuItem(menuTipoServicio, "ServicioTipoItem", ++idInternal);
      //AddBrowserMenuItem(menuTipoServicio, "ReferenciaCodigoValor", ++idInternal);
      #endregion

      menu.Submenus = new List<MenuCategory>();

      //if (_authenticatedUser.GetAdministrador())
      menu.Submenus.Add(menuTablas);

      //if (_authenticatedUser.GetAdministrador())
      //    menu.Submenus.Add(menuSeguridad);

      //if (_authenticatedUser.GetSuperUsuario())
      //    menu.Submenus.Add(menuAvanzado);

      //if (_authenticatedUser.GetAdministrador())
      //    menu.Submenus.Add(menuTipoServicio);

      return menu;
    }

    private MenuCategory GetMenuClinica(int id)
    {
      int idInternal = id * 100000;
      string controller = "Clinica";
      MenuCategory menu = new MenuCategory(controller, "", "Clinica" + idInternal.ToString(), true);

      MenuCategory menuClinica = new MenuCategory("Clínica", "uil-file-medical", "Clinica" + idInternal.ToString(), true);
      menuClinica.Items = new List<MenuItem>();

      AddBrowserMenuItem(menuClinica, "HistoriaClinica", ++idInternal);
      AddBrowserMenuItem(menuClinica, "Presupuesto", ++idInternal);

      menu.Submenus = new List<MenuCategory>();
      menu.Submenus.Add(menuClinica);

      return menu;
    }
    private List<MenuCategory> GetMenuInicial(/*List<OrgPermiso> permisos*/)
    {

      int id = 1;
      List<MenuCategory> l = new List<MenuCategory>();

      //l.Add(GetMenuAdministracion(id));
      //id++;
      l.Add(GetMenuPacientes(id));
      l.Add(GetMenuProfesionales(id));
      l.Add(GetMenuTurno(id));
      l.Add(GetMenuClinica(id));
      l.Add(GetMenuConfiguracion(id));

      //var menuConfiguracion = GetMenuConfiguracion(++id);
      //if (menuConfiguracion.Submenus != null && menuConfiguracion.Submenus.Count > 0)
      //    l.Add(menuConfiguracion);

      //l.Add(GetMenuConfiguracion(++id));





      //if (permisos.FindAll(x => x.CodCompania == codCompania).Any(x => x.Codigo == Permissions.PLANIFICACION_PROGRAMA))
      //{
      //    // menuPrograma.Items.Add(new MenuItem(id++, "Cargar Audiencia TVA", controller, "CargarAudienciaTVA", "uil-chart-line"));
      //    menuPrograma.Items.Add(new MenuItem(id++, "Cargar Programas", controller, "Index", "uil-bookmark-full"));
      //}



      //#region Tarifas
      //controller = "Tarifas";
      //MenuCategory menuTarifa = new MenuCategory(controller, "", controller, true);
      //menuTarifa.Items = new List<MenuItem>();

      //menuTarifa.Items.Add(new MenuItem(id++, "Cargar Colores", "Browser", "Color", "uil-bookmark-full"));
      //menuTarifa.Items.Add(new MenuItem(id++, "Cargar Colores", "Browser", "Color", "uil-bookmark-full"));
      //menuTarifa.Items.Add(new MenuItem(id++, "Cargar Colores", "Browser", "Color", "uil-bookmark-full"));
      //menuTarifa.Items.Add(new MenuItem(id++, "Cargar Colores", "Browser", "Color", "uil-bookmark-full"));
      //menuTarifa.Items.Add(new MenuItem(id++, "Cargar Colores", "Browser", "Color", "uil-bookmark-full"));
      //menuTarifa.Items.Add(new MenuItem(id++, "Cargar Colores", "Browser", "Color", "uil-bookmark-full"));

      ////if (permisos.FindAll(x => x.CodCompania == codCompania).Any(x => x.Codigo == Permissions.PLANIFICACION_TARIFA))
      ////{
      ////    menuTarifa.Items.Add(new MenuItem(id++, "Cargar Tarifas", controller, "Index", "uil-tag-alt"));
      ////}

      //if (menuTarifa.Items.Count > 0)
      //{
      //    l.Add(menuTarifa);
      //}
      //#endregion



      return l;
    }
  }
}
