using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using OdontoSoft.API.Helpers;
using Xunit;

namespace OdontoSoft.UnitTests.Middleware;

public class JsonExceptionMiddlewareTests
{
  private static readonly JsonSerializerOptions JsonOpts = BuildOptions();

  private static JsonSerializerOptions BuildOptions()
  {
    var o = new JsonSerializerOptions();
    o.Converters.Add(new LenientNullableJsonConverter());
    return o;
  }

  private class DtoInt
  {
    public int? DomicilioNumero { get; set; }
  }

  [Fact]
  public async Task RealJsonException_WithPath_Returns400WithFieldKey()
  {
    // Arrange: deserializar un JSON malformado dispara una JsonException real,
    // con `Path` populado por el JsonReader.
    // El campo está como "" pero DomicilioNumero es int? — LenientNullableJsonConverter
    // lo convierte a null. Para forzar un error real, usamos un valor no-numérico.
    var json = "{\"DomicilioNumero\":\"no-es-numero\"}";

    JsonException? captured = null;
    try
    {
      JsonSerializer.Deserialize<DtoInt>(json, JsonOpts);
    }
    catch (JsonException ex)
    {
      captured = ex;
    }

    captured.Should().NotBeNull("queremos una JsonException real para que Path esté populado.");
    captured!.Path.Should().NotBeNullOrEmpty("Path lo popula el JsonReader durante el parse.");

    var badHttp = new BadHttpRequestException("Bad Request", captured);

    var middleware = new JsonExceptionMiddleware(
      _ => throw badHttp,
      NullLogger<JsonExceptionMiddleware>.Instance);

    var ctx = new DefaultHttpContext();
    ctx.Response.Body = new MemoryStream();

    // Act
    await middleware.InvokeAsync(ctx);

    // Assert
    ctx.Response.StatusCode.Should().Be(400);

    ctx.Response.Body.Position = 0;
    using var doc = await JsonDocument.ParseAsync(ctx.Response.Body);
    var root = doc.RootElement;

    root.GetProperty("error").GetString().Should().Be("No se pudieron interpretar los datos enviados.");
    // La key "errors" puede tener varios fields; al menos DomicilioNumero.
    var errors = root.GetProperty("errors").EnumerateObject().Select(p => p.Name).ToList();
    errors.Should().Contain(name => name.Contains("DomicilioNumero"));
  }

  [Fact]
  public async Task DirectJsonException_AlsoCaught_Returns400()
  {
    var ex = new JsonException("invalid");

    var middleware = new JsonExceptionMiddleware(
      _ => throw ex,
      NullLogger<JsonExceptionMiddleware>.Instance);

    var ctx = new DefaultHttpContext();
    ctx.Response.Body = new MemoryStream();

    await middleware.InvokeAsync(ctx);

    ctx.Response.StatusCode.Should().Be(400);
  }

  [Fact]
  public async Task NonJsonException_PropagatesUp()
  {
    var ex = new InvalidOperationException("algo");

    var middleware = new JsonExceptionMiddleware(
      _ => throw ex,
      NullLogger<JsonExceptionMiddleware>.Instance);

    var ctx = new DefaultHttpContext();
    ctx.Response.Body = new MemoryStream();

    Func<Task> act = () => middleware.InvokeAsync(ctx);

    await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("algo");
  }

  [Fact]
  public async Task EmptyPath_FallsBackToGeneralError()
  {
    // JsonException sin Path (e.g. mensaje manual sin pasar por el reader)
    var innerJson = new JsonException("The JSON value could not be converted to System.Int32.");

    var badHttp = new BadHttpRequestException("Bad Request", innerJson);

    var middleware = new JsonExceptionMiddleware(
      _ => throw badHttp,
      NullLogger<JsonExceptionMiddleware>.Instance);

    var ctx = new DefaultHttpContext();
    ctx.Response.Body = new MemoryStream();

    await middleware.InvokeAsync(ctx);

    ctx.Response.Body.Position = 0;
    using var doc = await JsonDocument.ParseAsync(ctx.Response.Body);
    var root = doc.RootElement;

    // Sin field en el path → la key es "_general"
    root.GetProperty("errors").GetProperty("_general").ValueKind.Should().Be(JsonValueKind.Array);
  }

  [Theory]
  [InlineData("$.DomicilioNumero", "DomicilioNumero")]
  [InlineData("$.data.Nombre", "data.Nombre")]
  [InlineData("$.", "")]
  public void ExtractFieldName_StripsPathPrefix(string input, string expected)
  {
    // ExtractFieldName es internal static, lo invocamos por reflection
    // para no tener que exponer API solo para tests.
    var method = typeof(JsonExceptionMiddleware).GetMethod(
      "ExtractFieldName",
      System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

    method.Should().NotBeNull("ExtractFieldName debe existir en JsonExceptionMiddleware.");

    var result = method!.Invoke(null, new object?[] { input }) as string;

    result.Should().Be(expected);
  }

  [Theory]
  [InlineData("Int32", "número entero")]
  [InlineData("Int64", "número entero")]
  [InlineData("Decimal", "número decimal")]
  [InlineData("DateTime", "fecha")]
  [InlineData("Guid", "identificador")]
  [InlineData("Boolean", "valor verdadero/falso")]
  [InlineData("AlgoRaro", "valor de tipo AlgoRaro")]
  public void TranslateTypeToFriendlyMessage_MapsCorrectly(string type, string expected)
  {
    // Reflection al método estático privado (no está expuesto, pero está
    // documentado en el JsonExceptionMiddleware.cs línea 135).
    var method = typeof(JsonExceptionMiddleware).GetMethod(
      "TypeToFriendlyMessage",
      System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

    method.Should().NotBeNull("TypeToFriendlyMessage debe existir en JsonExceptionMiddleware.");

    var result = method!.Invoke(null, new object[] { type }) as string;

    result.Should().Be(expected);
  }
}
