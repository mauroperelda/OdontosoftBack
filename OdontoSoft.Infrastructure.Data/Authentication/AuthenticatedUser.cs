using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using OdontoSoft.Application.Abstractions;

namespace OdontoSoft.Infrastructure.Data.Authentication;

/// <summary>
/// Implementación de <see cref="ICurrentUser"/> que extrae los claims
/// del <see cref="HttpContext"/> actual. Vive en OdontoSoft.Infrastructure.Data
/// para que tanto OdontoSoft.API (JWT) como OdontoSoft.Web (cookies) puedan
/// registrarla en su DI sin acoplarse entre sí.
/// </summary>
public class AuthenticatedUser : ICurrentUser
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

  public int Id
  {
    get
    {
      var raw = GetClaimValue("Id");
      return int.TryParse(raw, out var id) ? id : 0;
    }
  }

  public string? UserName => GetClaimValue(ClaimTypes.Name);

  public string? Email => GetClaimValue(ClaimTypes.Email);

  public bool IsAdministrator =>
      GetClaimValueAsBoolean(PermissionCodes.ADMINISTRATOR_CODE);

  public bool IsSuperUser =>
      GetClaimValueAsBoolean(PermissionCodes.SUPERUSER_CODE);

  public bool HasPermission(string permissionCode) => GetClaimRole(permissionCode);
}
