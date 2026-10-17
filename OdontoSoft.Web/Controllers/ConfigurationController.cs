using Microsoft.AspNetCore.Mvc;

namespace OdontoSoft.Web.Controllers
{
  public class ConfigurationController : Controller
  {
    public ActionResult MedveApp()
    {
      Response.ContentType = "application/javascript";
      return View();
    }
  }
}
