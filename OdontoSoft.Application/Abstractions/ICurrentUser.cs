namespace OdontoSoft.Application.Abstractions;

/// <summary>
/// Usuario actual resuelto desde el HttpContext. La implementación vive
/// en OdontoSoft.Infrastructure.Data (AuthenticatedUser). El dominio y
/// Application solo conocen esta abstracción.
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
