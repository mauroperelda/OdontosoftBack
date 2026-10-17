using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace OdontoSoft.API.Helpers;

/// <summary>
/// Middleware global de excepciones. Captura cualquier excepción no controlada
/// que se filtre desde los controllers / servicios, la traduce a la shape
/// uniforme <c>{ error, errors }</c> usando <see cref="ApiErrorResponse"/>
/// y la serializa como respuesta JSON con el status code apropiado.
///
/// Antes de este middleware, cada controller tenía un <c>try/catch</c> que
/// repetía el mismo patrón (23+ acciones con boilerplate idéntico). Ahora esa
/// lógica vive acá y los controllers quedan limpios.
///
/// Importante: NO captura excepciones de deserialización JSON (eso lo hace
/// <see cref="JsonExceptionMiddleware"/>, que está registrado después y las
/// maneja con un shape específico de campo).
/// </summary>
public class GlobalExceptionMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<GlobalExceptionMiddleware> _logger;

  public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
    catch (Exception ex)
    {
      await HandleAsync(context, ex);
    }
  }

  private async Task HandleAsync(HttpContext context, Exception ex)
  {
    // Log con stack trace para diagnóstico interno.
    _logger.LogError(ex,
      "Excepción no controlada en {Method} {Path}",
      context.Request.Method, context.Request.Path);

    // Si la respuesta ya empezó a escribirse (e.g. un error en medio de un
    // stream), no podemos cambiar nada. Abortamos la conexión.
    if (context.Response.HasStarted)
    {
      _logger.LogWarning("No se pudo traducir la excepción porque la respuesta ya empezó.");
      return;
    }

    var actionResult = ApiErrorResponse.FromException(ex);
    var (statusCode, body) = ExtractStatusAndBody(actionResult);

    context.Response.Clear();
    context.Response.StatusCode = statusCode;
    context.Response.ContentType = MediaTypeNames.Application.Json;
    await context.Response.WriteAsJsonAsync(body, JsonOpts);
  }

  private static readonly JsonSerializerOptions JsonOpts = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    ReferenceHandler = ReferenceHandler.IgnoreCycles,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
  };

  /// <summary>
  /// Deconstruye un IActionResult en (statusCode, body) para poder
  /// serializarlo en una respuesta HTTP sin pasar por el pipeline de MVC.
  /// </summary>
  private static (int StatusCode, object Body) ExtractStatusAndBody(IActionResult result)
  {
    return result switch
    {
      ObjectResult obj => (obj.StatusCode ?? StatusCodes.Status200OK, obj.Value ?? new { }),
      StatusCodeResult sc => (sc.StatusCode, new { }),
      _ => (StatusCodes.Status500InternalServerError, new { error = "Error desconocido." }),
    };
  }
}
