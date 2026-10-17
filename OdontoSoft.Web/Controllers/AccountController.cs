//using ExtraCore.Data.Authentication;
//using ExtraCore.Data.Repository.UOW;
//using ExtraCore.Entities.Constants;
//using ExtraCore.Entities.Models;
using OdontoSoft.Web.IServices;
using OdontoSoft.Web.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OdontoSoft.Web.Controllers;
using System.Reflection.Metadata;
using System.Security.Claims;
using OdontoSoft.Web.Helpers.Authentication.DTO;
using Microsoft.AspNetCore.Authorization;
using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Infrastructure.Data.Repository.UOW;

namespace ExtraCore.Web.Controllers
{
  public class AccountController : AppBaseController
  {

    private readonly ILogger<AccountController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _accessor;
    private readonly LinkGenerator _generator;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IMemoryCache _memoryCache;
    private readonly IAuthenticatedUser _authenticatedUser;
    //private readonly IMailSender _mailSender;
    private readonly IAccountService _accountService;


    public AccountController(ILogger<AccountController> logger,
        IHttpContextAccessor accessor, LinkGenerator generator,
        IHostEnvironment hostEnvironment, IMemoryCache memoryCache,
        IAccountService accountService, IAuthenticatedUser authenticatedUser, IUnitOfWork unitOfWork)
    {
      _logger = logger;
      _unitOfWork = unitOfWork;
      _accessor = accessor;
      _generator = generator;
      _hostEnvironment = hostEnvironment;
      _memoryCache = memoryCache;
      _authenticatedUser = authenticatedUser;
      //_mailSender = mailSender;
      _accountService = accountService;
    }



    public IActionResult Login()
    {
      ClaimsPrincipal claimUser = HttpContext.User;

      if (claimUser.Identity.IsAuthenticated)
      {
        return RedirectToAction("Index", "Home");
      }

      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
      try
      {
        if (loginViewModel != null)
        {
          if (String.IsNullOrEmpty(loginViewModel.Email) || String.IsNullOrEmpty(loginViewModel.Password))
          {
            ViewBag.Errors = "Debe ingresar el email y la contraseña";
            return View();
          }

          var usuario = _accountService.Login(LoginDTO.MapFromViewModel(loginViewModel));

          if (usuario == null)
          {
            //_logger.LogInformation("Login ERROR: " + loginViewModel.Email);
            ViewBag.Errors = "Email y/o contraseña incorrectos";
            return View();
          }

          #region AUTENTICATION
          var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, $"{usuario.Name} {usuario.LastName}"),
                        new Claim("Nombre", usuario.Name),
                        new Claim("Apellido", usuario.LastName),
                        new Claim(ClaimTypes.Email, loginViewModel.Email),
                        new Claim("Id", usuario.Id.ToString()),
                        new Claim("Token", usuario.Token),
                        //new Claim("ProfileId", usuario.Profile.Id.ToString()),
                        //new Claim("ProfileCode", usuario.Profile.Code)
                    };

          //foreach (string permiso in usuario.Permissions)
          //{
          //    claims.Add(new Claim(ClaimTypes.Role, permiso));
          //}

          var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

          await HttpContext.SignInAsync(
              CookieAuthenticationDefaults.AuthenticationScheme,
              new ClaimsPrincipal(claimsIdentity),
              new AuthenticationProperties
              {
                //RedirectUri = Url.Action("Index", "Home"),
                IsPersistent = loginViewModel.RememberMe,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60),
                AllowRefresh = true
              }
          );

          ////_logger.LogInformation("Login OK: " + loginViewModel.Email + " " + loginViewModel.Password);
          #endregion

          //if (usuario.PerfilId == Constants.PERFIL_ID_USUARIO_INTERNO)

          //_logger.LogInformation("Login: " + loginViewModel.Email);

          return RedirectToAction("Index", "Home");

          //else
          //    return RedirectToAction("Index", "Public");
        }

        return View();
      }
      catch (Exception ex)
      {
        ViewBag.Errors = ex.Message;
        return View();
      }

    }

    public IActionResult ForgetPassword()
    {
      return View();
    }

    [HttpPost]
    public IActionResult ForgetPassword(ForgetPasswordViewModel forgetPasswordViewModel)
    {
      try
      {
        //var usuario = _unitOfWork.UsuarioRepository.GetByEmail(forgetPasswordViewModel.Email);
        //if (usuario == null) throw new Exception("Usuario no registrado");
        //AuthenticationMailSender authenticationMailSender = new AuthenticationMailSender(_accessor, _generator, _hostEnvironment, _mailSender);
        //authenticationMailSender.SendResetPasswordEmail(usuario, HashString(usuario.Id.ToString()), "ChangePasswordRequest", "account");
        return RedirectToAction("ForgetPasswordOk");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message + " " + forgetPasswordViewModel.Email);
        ViewBag.Errors = ex.Message;
        return View(forgetPasswordViewModel);
      }
    }


    public IActionResult ChangePasswordRequest(int id, string hash)
    {

      try
      {
        if (!ValidateHash(id, hash))
        {
          throw new Exception("Hash inválido. ID: " + id + " - hash: " + hash);
        }

        ChangePasswordViewModel vm = new ChangePasswordViewModel();

        vm.Id = id;
        vm.Hash = hash;

        return View("ChangePassword", vm);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
        return new ForbidResult();
      }

    }

    //[MedveRequirement]
    public IActionResult ChangePassword()
    {
      ChangePasswordViewModel vm = new ChangePasswordViewModel();

      //vm.Id = _authenticatedUser.GetId();
      //vm.Hash = HashString(vm.Id.ToString());

      return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
    {
      try
      {
        //if (!ValidateHash(changePasswordViewModel.Id, changePasswordViewModel.Hash))
        //{
        //    throw new Exception("Hash inválido. ID: " + changePasswordViewModel.Id + " - hash: " + changePasswordViewModel.Hash);
        //}

        //if (changePasswordViewModel != null)
        //{
        //    if (String.IsNullOrEmpty(changePasswordViewModel.NewPassword) || String.IsNullOrEmpty(changePasswordViewModel.NewPasswordConfirm))
        //    {
        //        ViewBag.Errors = "Debe indicar la nueva contraseña y su confirmación";
        //        return View(changePasswordViewModel);
        //    }

        //    if (!string.Equals(changePasswordViewModel.NewPassword, changePasswordViewModel.NewPasswordConfirm, StringComparison.OrdinalIgnoreCase))
        //    {
        //        ViewBag.Errors = "La nueva contraseña no es igual a su confirmación";
        //        return View(changePasswordViewModel);
        //    }

        //    _unitOfWork.UsuarioRepository.ChangePassword(changePasswordViewModel.Id.Value, Usuario.HashPassword(changePasswordViewModel.NewPassword));
        //    _unitOfWork.Save();

        //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        //    return RedirectToAction("ChangePasswordOk");
        //}

        return View(changePasswordViewModel);
      }
      catch (Exception ex)
      {
        ViewBag.Errors = ex.Message;
        return View(changePasswordViewModel);
      }

    }

    public IActionResult ChangePasswordOk()
    {
      return View();
    }

    public IActionResult ForgetPasswordOk()
    {
      return View();
    }

    public async Task<IActionResult> LogOut()
    {
      await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
      return RedirectToAction("Login", "Account");
    }
  }
}
