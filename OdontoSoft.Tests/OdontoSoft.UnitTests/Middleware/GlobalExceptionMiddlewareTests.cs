using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using OdontoSoft.API.Helpers;
using Xunit;

namespace OdontoSoft.UnitTests.Middleware;

/// <summary>
/// Tests del GlobalExceptionMiddleware — verifica que cualquier excepción
/// no controlada en los controllers (sin try/catch) se traduce a la shape
/// uniforme { error, errors } que el front espera.
/// </summary>
public class GlobalExceptionMiddlewareTests
{
  [Fact]
  public async Task InvalidOperationException_TranslatesToBadRequest_WithOriginalMessage()
  {
    var middleware = new GlobalExceptionMiddleware(
      _ => throw new InvalidOperationException("No se puede eliminar porque tiene turnos asociados."),
      NullLogger<GlobalExceptionMiddleware>.Instance);

    var ctx = new DefaultHttpContext();
    ctx.Response.Body = new MemoryStream();

    await middleware.InvokeAsync(ctx);

    ctx.Response.StatusCode.Should().Be(400);
    ctx.Response.ContentType.Should().StartWith("application/json");

    ctx.Response.Body.Position = 0;
    using var doc = await JsonDocument.ParseAsync(ctx.Response.Body);
    var root = doc.RootElement;

    root.GetProperty("error").GetString()
      .Should().Be("No se puede eliminar porque tiene turnos asociados.");
  }

  [Fact]
  public async Task GenericException_TranslatesToBadRequest_WithGenericMessage()
  {
    // No exponer mensaje crudo al cliente.
    var middleware = new GlobalExceptionMiddleware(
      _ => throw new Exception("Detalles internos sensibles."),
      NullLogger<GlobalExceptionMiddleware>.Instance);

    var ctx = new DefaultHttpContext();
    ctx.Response.Body = new MemoryStream();

    await middleware.InvokeAsync(ctx);

    ctx.Response.StatusCode.Should().Be(400);

    ctx.Response.Body.Position = 0;
    using var doc = await JsonDocument.ParseAsync(ctx.Response.Body);
    var root = doc.RootElement;

    var error = root.GetProperty("error").GetString();
    error.Should().Be("Ocurrió un error al procesar la solicitud.");
    error.Should().NotContain("sensibles", "el middleware no debe filtrar mensajes crudos.");
  }

  [Fact]
  public async Task NormalRequest_PassesThroughUnchanged()
  {
    var called = false;
    var middleware = new GlobalExceptionMiddleware(
      async _ =>
      {
        called = true;
        await Task.CompletedTask;
      },
      NullLogger<GlobalExceptionMiddleware>.Instance);

    var ctx = new DefaultHttpContext();
    ctx.Response.Body = new MemoryStream();

    await middleware.InvokeAsync(ctx);

    called.Should().BeTrue();
    ctx.Response.StatusCode.Should().Be(200, "sin excepción, la respuesta queda como está.");
  }
}
