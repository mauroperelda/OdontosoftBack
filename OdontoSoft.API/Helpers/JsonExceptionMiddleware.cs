using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace OdontoSoft.API.Helpers
{
  /// <summary>
  /// Captura errores de deserialización JSON y errores genéricos de modelo
  /// que escapan del pipeline de MVC, y los reescribe como
  /// <c>400 Bad Request</c> con el formato que el front entiende:
  /// <code>
  /// {
  ///   "error": "Mensaje general legible para el usuario",
  ///   "errors": { "Campo": ["Mensaje específico del campo"] }
  /// }
  /// </code>
  ///
  /// El front ya tiene un <c>formatApiError(err)</c> que extrae
  /// <c>errors</c> y <c>error</c> de la respuesta, así que este formato
  /// es el contrato que ambos lados comparten.
  /// </summary>
  public class JsonExceptionMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<JsonExceptionMiddleware> _logger;

    public JsonExceptionMiddleware(RequestDelegate next, ILogger<JsonExceptionMiddleware> logger)
    {
      _next = next;
      _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        await _next(context);
      }
      catch (BadHttpRequestException ex) when (ex.InnerException is JsonException jsonEx)
      {
        // El framework envuelve el JsonException en BadHttpRequestException
        // cuando falla el binding de un body.
        await HandleJsonErrorAsync(context, jsonEx);
      }
      catch (JsonException ex)
      {
        // Si por alguna razón llega desnuda, también la capturamos.
        await HandleJsonErrorAsync(context, ex);
      }
    }

    private async Task HandleJsonErrorAsync(HttpContext context, JsonException ex)
    {
      _logger.LogWarning(ex, "Error de deserialización JSON en {Path}", context.Request.Path);

      var field = ExtractFieldName(ex.Path);
      var message = TranslateMessage(ex);

      var errors = new Dictionary<string, string[]>();
      if (!string.IsNullOrEmpty(field))
        errors[field] = new[] { message };
      else
        errors["_general"] = new[] { message };

      var payload = new
      {
        error = "No se pudieron interpretar los datos enviados.",
        errors
      };

      // Si el response ya empezó a escribirse, no podemos cambiar el status,
      // pero normalmente no llegamos aquí en ese caso.
      if (context.Response.HasStarted) return;

      context.Response.Clear();
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
      context.Response.ContentType = "application/json; charset=utf-8";
      await context.Response.WriteAsJsonAsync(payload);
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    /// <summary>
    /// Convierte el JSON Path (ej. "$.DomicilioNumero") en el nombre
    /// de campo que el front conoce.
    /// </summary>
    internal static string ExtractFieldName(string? path)
    {
      if (string.IsNullOrEmpty(path)) return "";
      // $.foo.bar → foo.bar (el front hace match con la key del form, que
      // ya viene en PascalCase desde la API)
      var trimmed = path.TrimStart('$', '.');
      return trimmed;
    }

    /// <summary>
    /// Traduce el mensaje críptico de System.Text.Json a algo legible.
    /// Ej: "The JSON value could not be converted to System.Nullable`1[System.Int32]. Path: $.DomicilioNumero"
    ///   → "El valor no es un número entero válido."
    /// </summary>
    internal static string TranslateMessage(JsonException ex)
    {
      var raw = ex.Message ?? "";

      // Detectar tipo nullable destino
      var typeMatch = Regex.Match(
          raw,
          @"Nullable`1\[\s*System\.(\w+)\s*\]|System\.(\w+)");

      if (raw.IndexOf("could not be converted", StringComparison.OrdinalIgnoreCase) >= 0)
      {
        if (typeMatch.Success)
        {
          var type = typeMatch.Groups[1].Success ? typeMatch.Groups[1].Value : typeMatch.Groups[2].Value;
          return TypeToFriendlyMessage(type);
        }
        return "El valor enviado no tiene el formato correcto.";
      }

      // El body directamente no es JSON
      if (raw.IndexOf("'", StringComparison.Ordinal) >= 0 &&
          raw.IndexOf("is not a valid", StringComparison.OrdinalIgnoreCase) >= 0)
      {
        return "Uno de los campos enviados no tiene un valor válido.";
      }

      // Fallback: dar un mensaje genérico pero no exponer stack trace.
      return "Los datos enviados no se pudieron interpretar.";
    }

    private static string TypeToFriendlyMessage(string type) => type switch
    {
      "Int32" or "Int64" or "Int16" or "Byte" => "número entero",
      "Decimal" or "Double" or "Single" => "número decimal",
      "DateTime" or "DateTimeOffset" => "fecha",
      "Guid" => "identificador",
      "Boolean" => "valor verdadero/falso",
      _ => $"valor de tipo {type}"
    };
  }
}
