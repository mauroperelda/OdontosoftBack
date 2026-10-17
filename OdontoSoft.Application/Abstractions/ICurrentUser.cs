namespace OdontoSoft.Application.Abstractions;

/// <summary>
/// Usuario actual resuelto desde el HttpContext. La implementaciÃ³n vive
/// en OdontoSoft.Infrastructure (AuthenticatedUser). El dominio y
/// Application solo conocen esta abstracciÃ³n.
/// </summary>
public interface ICurrentUser
{
  int Id { get; }
  string? UserName { get; }
  string? Email { get; }
  bool IsAdministrator { get; }
  bool IsSuperUser { get; }
  bool HasPermission(string permissionCode);
}
