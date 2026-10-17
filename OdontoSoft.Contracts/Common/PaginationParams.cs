namespace OdontoSoft.Contracts.Common
{
  /// <summary>
  /// Parámetros de paginación recibidos en la query.
  /// </summary>
  public class PaginationParams
  {
    private const int MaxPageSize = 200;
    private int _pageSize = 50;

    public int Page { get; set; } = 1;
    public int PageSize
    {
      get => _pageSize;
      set => _pageSize = value <= 0 ? 50 : (value > MaxPageSize ? MaxPageSize : value);
    }
    public string? SearchText { get; set; }
  }

  /// <summary>
  /// Respuesta paginada estándar.
  /// </summary>
  public class PagedResult<T>
  {
    public IEnumerable<T> Items { get; set; } = Array.Empty<T>();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(Total / (double)PageSize);
  }
}