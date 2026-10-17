using Microsoft.EntityFrameworkCore;
using OdontoSoft.Contracts.Common;

namespace OdontoSoft.API.Helpers;

/// <summary>
/// Extension methods para centralizar el patrón de paginación que estaba
/// repetido en 4 controllers (Paciente, Turno, Pago, Presupuesto).
/// </summary>
public static class QueryableExtensions
{
  /// <summary>
  /// Pagina un <see cref="IQueryable{T}"/> aplicando Skip/Take sobre los
  /// <see cref="PaginationParams"/> recibidos.
  ///
  /// La query puede incluir <c>Include</c>/<c>ThenInclude</c> y/o un
  /// <c>Select</c> de proyección; el conteo se hace sobre la query completa
  /// (sin skip/take) para que el total refleje el universo total, no la página.
  /// </summary>
  public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
      this IQueryable<T> query,
      PaginationParams pagination,
      CancellationToken cancellationToken = default)
  {
    var total = await query.CountAsync(cancellationToken);
    var items = await query
        .Skip((pagination.Page - 1) * pagination.PageSize)
        .Take(pagination.PageSize)
        .ToListAsync(cancellationToken);

    return new PagedResult<T>
    {
      Items = items,
      Total = total,
      Page = pagination.Page,
      PageSize = pagination.PageSize,
    };
  }
}
