using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OdontoSoft.Infrastructure.Http;

namespace OdontoSoft.Infrastructure.Middleware;

/// <summary>
/// Middleware global de excepciones. Captura cualquier excepciÃ³n no controlada
/// que se filtre desde los controllers / servicios, la traduce a la shape
/// uniforme <c>{ error, errors }</c> usando <see cref="ApiErrorResponse"/>
/// y la serializa como respuesta JSON con el status code apropiado.
///
/// Antes de este middleware, cada controller tenÃ­a un <c>try/catch</c> que
/// repetÃ­a el mismo patrÃ³n (23+ acciones con boilerplate idÃ©ntico). Ahora esa
/// lÃ³gica vive acÃ¡ y los controllers quedan limpios.
///
/// Importante: NO captura excepciones de deserializaciÃ³n JSON (eso lo hace
/// <see cref="JsonExceptionMiddleware"/>, que estÃ¡ registrado despuÃ©s y las
/// maneja con un shape especÃ­fico de campo).
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
    // Log con stack trace para diagnÃ³stico interno.
    _logger.LogError(ex,
      "ExcepciÃ³n no controlada en {Method} {Path}",
      context.Request.Method, context.Request.Path);

    // Si la respuesta ya empezÃ³ a escribirse (e.g. un error en medio de un
    // stream), no podemos cambiar nada. Abortamos la conexiÃ³n.
    if (context.Response.HasStarted)
    {
      _logger.LogWarning("No se pudo traducir la excepciÃ³n porque la respuesta ya empezÃ³.");
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
