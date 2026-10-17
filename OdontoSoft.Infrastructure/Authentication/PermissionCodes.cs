namespace OdontoSoft.Infrastructure.Authentication;

/// <summary>
/// CÃ³digos de permisos utilizados en los claims de autenticaciÃ³n.
/// Movido desde OdontoSoft.Web para que tanto la API (JWT) como Web (cookies)
/// compartan las mismas constantes sin acoplarse entre sÃ­.
/// </summary>
public static class PermissionCodes
{
  public const string ADMINISTRATOR_CODE = "administrator";
  public const string SUPERUSER_CODE = "superuser";
}
