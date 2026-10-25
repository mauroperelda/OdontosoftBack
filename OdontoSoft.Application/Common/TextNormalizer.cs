using System.Globalization;
using System.Text;

namespace OdontoSoft.Application.Common;

/// <summary>
/// Normalización de texto para comparar nombres que vienen de fuentes
/// distintas: localities/provinces de la API georef de datos.gob.ar contra
/// los de la tabla <c>provincia</c>, por ejemplo.
///
/// Quita acentos, recorta espacios sobrantes y unifica mayúsculas/minúsculas
/// para que "Córdoba", "cordoba" y " CÓRDOBA " sean la misma clave.
/// </summary>
public static class TextNormalizer
{
  /// <summary>
  /// Devuelve el texto normalizado: sin diacríticos, sin espacios de los
  /// bordes y en mayúsculas. Si el texto es null o vacío devuelve "".
  /// </summary>
  public static string Normalize(string? text)
  {
    if (string.IsNullOrWhiteSpace(text)) return string.Empty;

    string recortado = text.Trim();
    string descompuesto = recortado.Normalize(NormalizationForm.FormD);

    var sb = new StringBuilder(descompuesto.Length);
    foreach (char c in descompuesto)
    {
      // Los caracteres combinantes (NonSpacingMark) son los que aportan
      // el acento: descartarlos es lo que lo quita.
      if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
        sb.Append(c);
    }

    return sb.ToString().Normalize(NormalizationForm.FormC).ToUpperInvariant();
  }
}
