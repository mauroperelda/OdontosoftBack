using OdontoSoft.Web.Helpers.Application;
using OdontoSoft.Web.Helpers.Menu;
using Microsoft.AspNetCore.Mvc;

namespace OdontoSoft.Web.Controllers
{
  public class MenuController : AppBaseController
  {
    private ICurrentApplication _CurrentApplication;
    public MenuController(ICurrentApplication currentApplication)
    {
      _CurrentApplication = currentApplication;
    }

    public IActionResult Index()
    {
      return View();
    }
    [HttpGet]
    public IActionResult GetLeftSideMenu()
    {
      LeftSideMenu menu = _CurrentApplication.GetLeftSideMenu();

      return PartialView("~/Views/Shared/_LeftSideMenu.cshtml", menu);
    }
  }
}
