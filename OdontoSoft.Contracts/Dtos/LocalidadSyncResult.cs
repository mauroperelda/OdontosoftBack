namespace OdontoSoft.Contracts.Dtos;

/// <summary>
/// Resultado de una sincronización de localidades contra la API georef de
/// datos.gob.ar. Es lo que devuelve el endpoint
/// <c>POST api/localidad/SincronizarLocalidades</c>.
/// </summary>
public class LocalidadSyncResult
{
  /// <summary>Cantidad de localidades que devuelve la fuente.</summary>
  public int TotalFuente { get; set; }

  /// <summary>
  /// Cantidad de localidades que se van a insertar (ya deduplicadas y con la
  /// provincia resuelta).
  /// </summary>
  public int AInsertar { get; set; }

  /// <summary>
  /// Cantidad que ya existía en la base y por lo tanto se omitió. Incluye
  /// los duplicados que vienen dentro del propio lote de la fuente.
  /// </summary>
  public int YaExistentes { get; set; }

  /// <summary>
  /// Cantidad de localidades descartadas porque su provincia no está en la
  /// tabla <c>provincia</c>.
  /// </summary>
  public int SinProvincia { get; set; }

  /// <summary>Cantidad realmente insertada. Es 0 en modo simulación.</summary>
  public int Insertadas { get; set; }

  /// <summary>
  /// Nombres de provincia que trae la fuente y que no se encontraron en la
  /// tabla <c>provincia</c>. Si esta lista no está vacía, conviene revisar
  /// el catálogo de provincias antes de sincronizar.
  /// </summary>
  public List<string> ProvinciasNoEncontradas { get; set; } = new();

  /// <summary>True si la operación fue sólo una simulación (no se escribió nada).</summary>
  public bool SoloSimular { get; set; }
}
