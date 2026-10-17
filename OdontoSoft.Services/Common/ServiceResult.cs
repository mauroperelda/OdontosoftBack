namespace OdontoSoft.Services.Common;

/// <summary>
/// Resultado estándar de las operaciones de Services. Permite que los
/// controllers traduzcan un único tipo a IActionResult (200/201/400/404/409)
/// sin acoplar la lógica de negocio a HTTP.
/// </summary>
public class ServiceResult
{
  public bool IsSuccess { get; init; } = true;
  public string? Error { get; init; }
  public Dictionary<string, string>? FieldErrors { get; init; }

  public static ServiceResult Ok() => new();

  public static ServiceResult Fail(string error, Dictionary<string, string>? fieldErrors = null)
      => new() { IsSuccess = false, Error = error, FieldErrors = fieldErrors };
}

/// <summary>
/// Resultado de un upsert (Create/Update). Incluye el Id resultante para que
/// el controller pueda responder 201 Created con Location header.
/// </summary>
public class UpsertResult : ServiceResult
{
  public int? Id { get; init; }

  public static UpsertResult Created(int id) =>
      new() { IsSuccess = true, Id = id };

  public static new UpsertResult Fail(string error, Dictionary<string, string>? fieldErrors = null) =>
      new() { IsSuccess = false, Error = error, FieldErrors = fieldErrors };
}

/// <summary>
/// Upsert que además devuelve una entidad/VM (caso Create/Update de
/// Periodontograma, donde el controller necesita el VM con piezas y métricas
/// ya armadas).
/// </summary>
public class UpsertResult<T> : ServiceResult
{
  public T? Value { get; init; }

  public static UpsertResult<T> Created(T value) =>
      new() { IsSuccess = true, Value = value };

  public static new UpsertResult<T> Fail(string error, Dictionary<string, string>? fieldErrors = null) =>
      new() { IsSuccess = false, Error = error, FieldErrors = fieldErrors };
}

/// <summary>
/// Resultado de RecalcularTotal: incluye el nuevo total y, si el estado
/// del presupuesto cambió a "Pagado", el nuevo estado.
/// </summary>
public class RecalcularTotalResult : ServiceResult
{
  public decimal NuevoTotal { get; init; }
  public string? Estado { get; init; }

  public static RecalcularTotalResult Ok(decimal nuevoTotal, string? estado) =>
      new() { IsSuccess = true, NuevoTotal = nuevoTotal, Estado = estado };
}

/// <summary>
/// Métricas calculadas de un periodontograma.
/// </summary>
public class MetricasResult
{
  public decimal Bop { get; init; }      // bleeding on probing %
  public decimal Placa { get; init; }     // placa %
  public int Presentes { get; init; }     // piezas presentes (de 32)
  public int Ausentes { get; init; }      // piezas ausentes
}
