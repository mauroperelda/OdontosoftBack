using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Infrastructure.Data.Repository.UOW;
//using OdontoSoft.Web.Attributes;
using OdontoSoft.Web.Controllers;
using OdontoSoft.Web.Browser;
using MedveCore.Common.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;



namespace OdontoSoft.Web.Controllers
{
  //[ExtraCoreAuthenticated]
  public partial class BrowserController : AppBaseController
  {
    private readonly ILogger<BrowserController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _accessor;
    private readonly LinkGenerator _generator;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IMemoryCache _memoryCache;
    private readonly IAuthenticatedUser _authenticatedUser;
    private readonly BrowserCacheHelper _browserCacheHelper;

    public BrowserController(ILogger<BrowserController> logger, IUnitOfWork unitOfWork
        , IHttpContextAccessor accessor, LinkGenerator generator
        , IHostEnvironment hostEnvironment, IMemoryCache memoryCache
        , IAuthenticatedUser authenticatedUser)
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




    public IActionResult Index(string code)
    {
      IBrowser webBrowser = _browserCacheHelper.GetBrowser(code);

      if (webBrowser.IsAllowed())
      {
        ViewBag.Code = webBrowser.ViewTitle;
        return View(webBrowser.GetBrowserModel());
      }
      else
        return Unauthorized();
    }

    //private IBrowser GetBrowser(string code)
    //{
    //    IBrowser b;

    //    code = ("BROWSER_" + code).ToUpper();

    //    if (!this._memoryCache.TryGetValue(code, out b))
    //    {
    //        AddBrowsers();
    //    }

    //    if (!this._memoryCache.TryGetValue(code, out b))
    //    {
    //        throw new Exception("BROWSER_" + code.ToUpper() + ". Browser is not registered. Please regenerate Browser Registration file.");
    //    }

    //    if (b == null)
    //    {
    //        throw new Exception("BROWSER_" + code.ToUpper() + ". Browser is not registered. Please regenerate Browser Registration file.");
    //    }

    //    return b;
    //}

    protected JsonResult JsonOk(string dataLabel, object dataContent)
    {
      return Json(new MedveJSON(true, "", dataLabel, dataContent)); //, new System.Text.Json.JsonSerializerOptions());
    }

    public JsonResult GetBrowserDatasource(string parametersString)
    {
      BrowserDatasourceParameters parameters = null;
      if (!string.IsNullOrEmpty(parametersString))
      {
        parameters = JsonConvert.DeserializeObject<BrowserDatasourceParameters>(parametersString, new BrowserDatasourceFilterItemParameterConverter());
      }
      var webBrowser = _browserCacheHelper.GetBrowser(parameters.BrowserCode);
      BrowserModel browserInstance = webBrowser.GetBrowserModel(parameters);

      return JsonOk("Data", new { Items = browserInstance.Datasource.ToList(), TotalCount = browserInstance.DatasourceTotalCount });

    }


  }

  public class BrowserDatasourceFilterItemParameterConverter : JsonCreationConverter<BrowserDatasourceFilterItemParameter>
  {
    protected override BrowserDatasourceFilterItemParameter Create(Type objectType, JObject jObject)
    {
      if (FieldExists("Condition", jObject))
      {
        return new BrowserDatasourceFilterConditionParameter();
      }
      else if (FieldExists("Items", jObject))
      {
        return new BrowserDatasourceFilterListParameter();
      }
      else
      {
        return new BrowserDatasourceFilterFieldParameter();
      }
    }

    private bool FieldExists(string fieldName, JObject jObject)
    {
      return jObject[fieldName] != null;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }
  }

  public abstract class JsonCreationConverter<T> : JsonConverter
  {
    protected abstract T Create(Type objectType, JObject jObject);

    public override bool CanConvert(Type objectType)
    {
      return typeof(T).IsAssignableFrom(objectType);
    }

    public override bool CanWrite
    {
      get { return false; }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      // Load JObject from stream
      JObject jObject = JObject.Load(reader);

      // Create target object based on JObject
      T target = Create(objectType, jObject);

      // Populate the object properties
      serializer.Populate(jObject.CreateReader(), target);

      return target;
    }
  }
}
