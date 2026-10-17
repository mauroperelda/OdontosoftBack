//using ExtraCore.Web.Attributes;
//using ExtraCore.Web.Medve.Browser;
//using MedveCore.MembershipProvider;
using Microsoft.AspNetCore.Mvc;
using OdontoSoft.Web.Browser;

namespace OdontoSoft.Web.Controllers
{
  //[ExtraCoreAuthenticatedAttribute]
  public class AppBaseBrowserController : AppBaseController
  {

    protected BrowserCacheHelper _browserCacheHelper;
    protected String _entityTypeName;

    public AppBaseBrowserController(string entityTypeName)
    {
      this._entityTypeName = entityTypeName;
    }

    protected virtual RedirectToActionResult RedirectToBrowser()
    {
      return RedirectToAction("Index", "Browser", new { code = this._entityTypeName });
    }

    protected virtual RedirectToActionResult RedirectToBrowser(Microsoft.AspNetCore.Http.IFormCollection formCollection)
    {
      return RedirectToBrowser();
    }


    protected void LoadViewBag(Dictionary<string, object> viewBag)
    {
      if (viewBag == null) return;

      foreach (var item in viewBag)
      {
        this.ViewData[item.Key] = item.Value;
      }
    }

    protected bool AllowAction(int entityId)
    {
      //var b = _browserCacheHelper.GetBrowser(_entityTypeName);
      //if (b == null) return false;
      //if (!b.IsAllowed()) return false;
      //if (entityId == 0)
      //{
      //    if (!b.IsAllowedCreateNew()) return false;
      //}
      //else
      //{
      //    if (!b.IsAllowedEdit()) return false;
      //}

      return true;
    }


  }
}
