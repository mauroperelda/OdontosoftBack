using OdontoSoft.Data.Authentication;
using Mauro.Entities.Base;
using Mauro.RepositoryPattern.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace OdontoSoft.Web.Browser
{
  public class Browser<TEntity> : IBrowser where TEntity : BaseEntity, IObjectState
  {
    //private bool _isAllowed;



    private readonly IHttpContextAccessor _accessor;
    private readonly LinkGenerator _generator;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IAuthenticatedUser _authenticatedUser;


    private string GetConfigFileName()
    {
      string fileConfig = Path.Combine(_hostEnvironment.ContentRootPath, "Config", "Browser", "Index", typeof(TEntity).Name + ".xml");
      if (!System.IO.File.Exists(fileConfig)) throw new Exception(String.Format("Archivo {0} no encontrado", fileConfig));
      return fileConfig;
    }

    public Browser(IHttpContextAccessor accessor, LinkGenerator generator, IHostEnvironment hostEnvironment, IAuthenticatedUser authenticatedUser)
    {
      _accessor = accessor;
      _generator = generator;
      _hostEnvironment = hostEnvironment;
      _authenticatedUser = authenticatedUser;

      this.BrowserModel = new BrowserModel();
      this.BrowserModel.EntityType = typeof(TEntity);

      //this._isAllowed = GetIsBrowserAllowed<TEntity>();
      string configFileName = GetConfigFileName();
      this.BrowserColumnsGenerator = new BrowserColumnsGenerator<TEntity>(configFileName);
      this.BrowserActionsGenerator = new BrowserActionsGenerator<TEntity>(configFileName);
      this.BrowserSettingsLoader = new BrowserSettingsLoader<TEntity>(_accessor, _generator, configFileName);
      this.BrowserLoad = new BrowserLoader<TEntity>();
      this.ViewTitle = this.BrowserModel.EntityType.Name;

      if (this.BrowserModel.Columns == null)
        this.BrowserModel.Columns = this.BrowserColumnsGenerator.GetColumns();

      if (this.BrowserModel.Actions == null)
        this.BrowserModel.Actions = this.BrowserActionsGenerator.GetActions();

      if (this.BrowserModel.Settings == null)
        this.BrowserModel.Settings = this.BrowserSettingsLoader.GetSettings();
      this.BrowserModel.Code = this.BrowserModel.EntityType.Name.ToUpper();
    }

    public Browser(string viewTitle, IHttpContextAccessor accessor, LinkGenerator generator, IHostEnvironment hostEnvironment)
    {
      _accessor = accessor;
      _generator = generator;
      _hostEnvironment = hostEnvironment;

      this.BrowserModel = new BrowserModel();
      this.BrowserModel.EntityType = typeof(TEntity);
      string configFileName = GetConfigFileName();
      //this._isAllowed = GetIsBrowserAllowed<TEntity>();
      this.BrowserColumnsGenerator = new BrowserColumnsGenerator<TEntity>(configFileName);
      this.BrowserActionsGenerator = new BrowserActionsGenerator<TEntity>(configFileName);
      this.BrowserSettingsLoader = new BrowserSettingsLoader<TEntity>(_accessor, _generator, configFileName);
      this.BrowserLoad = new BrowserLoader<TEntity>();
      this.ViewTitle = viewTitle == null ? this.BrowserModel.EntityType.Name : viewTitle;
    }

    private BrowserSettingsLoader<TEntity> GetSettingsLoader()
    {
      if (this.BrowserSettingsLoader == null)
      {
        this.BrowserSettingsLoader = new BrowserSettingsLoader<TEntity>(_accessor, _generator, GetConfigFileName());
      }

      return this.BrowserSettingsLoader;
    }

    private bool GetIsBrowserAllowed<TEntity>()
    {

      if (this.BrowserModel.Settings == null)
        this.BrowserModel.Settings = GetSettingsLoader().GetSettings();

      if (!String.IsNullOrEmpty(this.BrowserModel.Settings.PermissionCode))
      {
        if (_authenticatedUser == null)
          return false;
        else
        {
          return _authenticatedUser.HasPermission(this.BrowserModel.Settings.PermissionCode);
        }
      }
      else
      {
        return true;
      }
      //var attrs = System.Attribute.GetCustomAttributes(typeof(TEntity));

      //foreach (System.Attribute attr in attrs)
      //{
      //    //if (attr is AuthAdminAttribute)
      //    //{
      //    //    IMembership membership = DependencyResolver.Current.GetService<IMembership>();
      //    //    return (membership.IsAdministrator());
      //    //}
      //}

      //return false;
    }

    public BrowserModel GetBrowserModel()
    {
      return this.BrowserModel;
    }

    public BrowserModel GetBrowserModel(BrowserDatasourceParameters parameters)
    {
      this.BrowserLoad.LoadBrowserDatasource(this.BrowserModel, parameters, _accessor);
      this.BrowserModel.DatasourceTotalCount = this.BrowserLoad.GetBrowserDatasourceTotalCount(parameters, _accessor);

      return this.BrowserModel;
    }

    private BrowserModel BrowserModel { get; set; }

    private BrowserLoader<TEntity> BrowserLoad { get; set; }

    private BrowserColumnsGenerator<TEntity> BrowserColumnsGenerator { get; set; }
    private BrowserActionsGenerator<TEntity> BrowserActionsGenerator { get; set; }
    private BrowserSettingsLoader<TEntity> BrowserSettingsLoader { get; set; }

    public Type EntityType
    {
      get
      {
        return this.BrowserModel.EntityType;
      }
    }


    public bool IsAllowed()
    {
      return GetIsBrowserAllowed<TEntity>();
      //return _isAllowed;
    }
    public bool IsAllowedCreateNew()
    {
      if (this.BrowserModel.Settings == null)
        this.BrowserModel.Settings = GetSettingsLoader().GetSettings();

      if (!String.IsNullOrEmpty(this.BrowserModel.Settings.CreateNew.PermissionCode))
      {
        if (_authenticatedUser == null)
          return false;
        else
        {
          return _authenticatedUser.HasPermission(this.BrowserModel.Settings.CreateNew.PermissionCode);
        }
      }
      else
      {
        return true;
      }
    }

    public bool IsAllowedEdit()
    {
      if (this.BrowserModel.Settings == null)
        this.BrowserModel.Settings = GetSettingsLoader().GetSettings();

      if (!String.IsNullOrEmpty(this.BrowserModel.Settings.EditForm.PermissionCode))
      {
        if (_authenticatedUser == null)
          return false;
        else
        {
          return _authenticatedUser.HasPermission(this.BrowserModel.Settings.EditForm.PermissionCode);
        }
      }
      else
      {
        return true;
      }
    }


    public string ViewTitle { get; set; }
  }
}
