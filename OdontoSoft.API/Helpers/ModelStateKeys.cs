namespace OdontoSoft.API.Helpers
{
  /// <summary>
  /// Helpers para limpiar las keys que ASP.NET Core pone en
  /// <c>ModelState</c> cuando hace binding del body de una request.
  /// Esas keys vienen prefijadas con el JSON path (ej. "$.foo")
  /// o con el nombre del parámetro del action (ej. "vm.foo"),
  /// y queremos la última componente para que matchee con el nombre
  /// de la propiedad del view model.
  /// </summary>
  internal static class ModelStateKeys
  {
    /// <summary>
    /// Limpia el prefijo de binding y devuelve la última componente.
    /// Ej:
    ///   "$.DomicilioNumero" → "DomicilioNumero"
    ///   "vm.Nombre"         → "Nombre"
    ///   "$"                 → "_general"
    ///   ""                  → "_general"
    /// </summary>
    public static string Strip(string? key)
    {
      if (string.IsNullOrWhiteSpace(key)) return "_general";
      var trimmed = key.TrimStart('$', '.').Trim();
      if (string.IsNullOrEmpty(trimmed)) return "_general";

      var parts = trimmed.Split('.');
      return parts[^1];
    }
  }
}
