using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace OdontoSoft.Data.Authentication;

/// <summary>
/// Implementación de <see cref="IAuthenticatedUser"/> que extrae los claims
/// del <see cref="HttpContext"/> actual. Vive en OdontoSoft.Data para que
/// tanto OdontoSoft.API (JWT) como OdontoSoft.Web (cookies) puedan
/// registrarla en su DI sin acoplarse entre sí.
/// </summary>
public class AuthenticatedUser : IAuthenticatedUser
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public AuthenticatedUser(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  private HttpContext? GetContext() => _httpContextAccessor.HttpContext;

  private string? GetClaimValue(string key)
  {
    if (GetContext() == null) return null;
    return GetContext()!.User.Claims
        .FirstOrDefault(r => r.Type == key)?.Value;
  }

  private bool GetClaimRole(string permissionCode)
  {
    if (GetContext() == null) return false;
    return GetContext()!.User.Claims
        .Any(r => r.Type == ClaimTypes.Role &&
                  string.Equals(r.Value, permissionCode, StringComparison.OrdinalIgnoreCase));
  }

  private bool GetClaimValueAsBoolean(string key)
  {
    var s = GetClaimValue(key);
    if (string.IsNullOrEmpty(s)) return false;
    s = s.ToUpperInvariant();
    return s == "1" || s == "TRUE";
  }

  public int GetId()
  {
    var raw = GetClaimValue("Id");
    return int.TryParse(raw, out var id) ? id : 0;
  }

  public string? GetUserName() => GetClaimValue(ClaimTypes.Name);

  public string? GetEmail() => GetClaimValue(ClaimTypes.Email);

  public bool GetAdministrador() =>
      GetClaimValueAsBoolean(PermissionCodes.ADMINISTRATOR_CODE);

  public bool GetSuperUsuario() =>
      GetClaimValueAsBoolean(PermissionCodes.SUPERUSER_CODE);

  public bool HasPermission(string permissionCode) => GetClaimRole(permissionCode);
}
