namespace OdontoSoft.Data.Authentication;

/// <summary>
/// Códigos de permisos utilizados en los claims de autenticación.
/// Movido desde OdontoSoft.Web para que tanto la API (JWT) como Web (cookies)
/// compartan las mismas constantes sin acoplarse entre sí.
/// </summary>
public static class PermissionCodes
{
  public const string ADMINISTRATOR_CODE = "administrator";
  public const string SUPERUSER_CODE = "superuser";
}
