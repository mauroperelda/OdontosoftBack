namespace OdontoSoft.Infrastructure.Http
{
  /// <summary>
  /// Helpers para limpiar las keys que ASP.NET Core pone en
  /// <c>ModelState</c> cuando hace binding del body de una request.
  /// Esas keys vienen prefijadas con el JSON path (ej. "$.foo")
  /// o con el nombre del parÃ¡metro del action (ej. "vm.foo"),
  /// y queremos la Ãºltima componente para que matchee con el nombre
  /// de la propiedad del view model.
  /// </summary>
  internal static class ModelStateKeys
  {
    /// <summary>
    /// Limpia el prefijo de binding y devuelve la Ãºltima componente.
    /// Ej:
    ///   "$.DomicilioNumero" â†’ "DomicilioNumero"
    ///   "vm.Nombre"         â†’ "Nombre"
    ///   "$"                 â†’ "_general"
    ///   ""                  â†’ "_general"
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
