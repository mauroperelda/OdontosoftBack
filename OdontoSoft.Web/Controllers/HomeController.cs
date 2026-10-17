using Microsoft.AspNetCore.Mvc;
using OdontoSoft.Web.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Globalization;
using OdontoSoft.Infrastructure.DAL;
using Microsoft.EntityFrameworkCore;
using OdontoSoft.Domain.Models;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Contracts.Dtos;

namespace OdontoSoft.Web.Controllers
{
  [Authorize]
  public class HomeController : AppBaseController
  {
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
      _logger = logger;
    }
    //[ExtraCoreAuthenticated]
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Privacy()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
