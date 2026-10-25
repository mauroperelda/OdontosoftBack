using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Domain.Models;

namespace OdontoSoft.Application.Common;

/// <summary>
/// Resultado del mapeo: por un lado los contadores que se devuelven al
/// front y por otro las entidades listas para insertar.
/// </summary>
public sealed class LocalidadSyncPlan
{
  public LocalidadSyncResult Resultado { get; init; } = new();

  public IReadOnlyList<Localidad> Localidades { get; init; } = Array.Empty<Localidad>();
}

/// <summary>
/// Mapeo puro (sin base de datos ni HTTP) de la respuesta de la API georef a
/// las filas de la tabla <c>localidad</c>.
///
/// Se separa del Service a propósito: la parte que decide qué localidades se
/// insertan (resolución de provincia + deduplicación) es la que más puede
/// fallar, y así se puede testear sin base de datos ni llamadas HTTP.
/// </summary>
public static class LocalidadSyncMapper
{
  /// <summary>
  /// Clave de identidad de una localidad: nombre normalizado + id de
  /// provincia. Dos localidades con la misma clave son la misma, y es lo que
  /// permite que la sincronización sea idempotente.
  /// </summary>
  public static string BuildKey(string? nombre, int provinciaId)
    => TextNormalizer.Normalize(nombre) + "|" + provinciaId;

  /// <summary>
  /// Traduce el catálogo de la fuente a las localidades a insertar.
  /// </summary>
  /// <param name="fuente">Localidades devueltas por la API georef.</param>
  /// <param name="provincias">Provincias existentes en la base.</param>
  /// <param name="clavesExistentes">
  /// Claves (<see cref="BuildKey"/>) de las localidades que ya están en la
  /// base. Se pasan ya calculadas para que esta función no toque la base.
  /// </param>
  public static LocalidadSyncPlan Map(
      IEnumerable<LocalidadGeoref>? fuente,
      IEnumerable<Provincia>? provincias,
      IEnumerable<string>? clavesExistentes)
  {
    var resultado = new LocalidadSyncResult();

    // Provincia normalizada → id. El GroupBy evita que el diccionario
    // reviente si el catálogo tuviera dos provincias con el mismo nombre
    // normalizado: en ese caso gana la primera.
    var provinciasPorNombre = (provincias ?? Enumerable.Empty<Provincia>())
        .Where(p => !string.IsNullOrWhiteSpace(p.Descripcion))
        .GroupBy(p => TextNormalizer.Normalize(p.Descripcion))
        .ToDictionary(g => g.Key, g => g.First().Id);

    // Claves ya presentes en la base. A propósito se usan TODAS las
    // localidades, no sólo las habilitadas: así una resincronización nunca
    // duplica una fila que estaba dada de baja.
    var existentes = new HashSet<string>(clavesExistentes ?? Enumerable.Empty<string>());

    var aInsertar = new List<Localidad>();
    var provinciasNoEncontradas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    var lista = (fuente ?? Enumerable.Empty<LocalidadGeoref>()).ToList();
    resultado.TotalFuente = lista.Count;

    foreach (var loc in lista)
    {
      string? nombre = loc?.nombre;
      if (string.IsNullOrWhiteSpace(nombre)) continue;

      string nombreProvincia = loc.provincia?.nombre ?? string.Empty;
      if (!provinciasPorNombre.TryGetValue(TextNormalizer.Normalize(nombreProvincia), out int provinciaId))
      {
        resultado.SinProvincia++;
        provinciasNoEncontradas.Add(nombreProvincia);
        continue;
      }

      // Add devuelve false si la clave ya estaba: deduplica contra la base
      // y también contra las localidades que ya vinieron en el mismo lote.
      if (!existentes.Add(BuildKey(nombre, provinciaId)))
      {
        resultado.YaExistentes++;
        continue;
      }

      aInsertar.Add(new Localidad
      {
        Descripcion = nombre.Trim(),
        ProvinciaId = provinciaId,
        Disabled = false
      });
    }

    resultado.AInsertar = aInsertar.Count;
    resultado.ProvinciasNoEncontradas = provinciasNoEncontradas.ToList();

    return new LocalidadSyncPlan
    {
      Resultado = resultado,
      Localidades = aInsertar
    };
  }
}
