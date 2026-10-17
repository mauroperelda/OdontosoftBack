using System.Text.Json;
using FluentAssertions;
using OdontoSoft.Infrastructure.Http;
using Xunit;

namespace OdontoSoft.UnitTests.Helpers;

/// <summary>
/// Tests del LenientNullableJsonConverter â€” la pieza que evita que
/// `$.DomicilioNumero: The JSON value could not be converted...` explote
/// cuando el front manda un campo vacÃ­o.
/// </summary>
public class LenientNullableJsonConverterTests
{
  private static readonly JsonSerializerOptions Options = BuildOptions();

  private static JsonSerializerOptions BuildOptions()
  {
    var o = new JsonSerializerOptions();
    o.Converters.Add(new LenientNullableJsonConverter());
    return o;
  }

  private class DtoInt
  {
    public int? Value { get; set; }
  }

  private class DtoDecimal
  {
    public decimal? Value { get; set; }
  }

  private class DtoDateTime
  {
    public DateTime? Value { get; set; }
  }

  [Fact]
  public void EmptyString_ForNullableInt_ReturnsNull()
  {
    var json = "{\"Value\":\"\"}";

    var dto = JsonSerializer.Deserialize<DtoInt>(json, Options);

    dto!.Value.Should().BeNull();
  }

  [Fact]
  public void WhitespaceString_ForNullableInt_ReturnsNull()
  {
    var json = "{\"Value\":\"   \"}";

    var dto = JsonSerializer.Deserialize<DtoInt>(json, Options);

    dto!.Value.Should().BeNull();
  }

  [Fact]
  public void ValidIntegerString_ForNullableInt_Parses()
  {
    var json = "{\"Value\":\"42\"}";

    var dto = JsonSerializer.Deserialize<DtoInt>(json, Options);

    dto!.Value.Should().Be(42);
  }

  [Fact]
  public void InvalidIntegerString_ForNullableInt_ThrowsJsonException()
  {
    var json = "{\"Value\":\"no-es-numero\"}";

    var act = () => JsonSerializer.Deserialize<DtoInt>(json, Options);

    act.Should().Throw<JsonException>()
      .WithMessage($"*no es un nÃºmero entero vÃ¡lido*");
  }

  [Fact]
  public void EmptyString_ForNullableDecimal_ReturnsNull()
  {
    var json = "{\"Value\":\"\"}";

    var dto = JsonSerializer.Deserialize<DtoDecimal>(json, Options);

    dto!.Value.Should().BeNull();
  }

  [Fact]
  public void ValidDecimalString_ForNullableDecimal_ParsesWithInvariantCulture()
  {
    // El "." debe parsearse como decimal aunque el servidor estÃ© en es-AR.
    var json = "{\"Value\":\"1234.56\"}";

    var dto = JsonSerializer.Deserialize<DtoDecimal>(json, Options);

    dto!.Value.Should().Be(1234.56m);
  }

  [Fact]
  public void EmptyString_ForNullableDateTime_ReturnsNull()
  {
    var json = "{\"Value\":\"\"}";

    var dto = JsonSerializer.Deserialize<DtoDateTime>(json, Options);

    dto!.Value.Should().BeNull();
  }

  [Fact]
  public void ValidIsoDate_ForNullableDateTime_Parses()
  {
    var json = "{\"Value\":\"2026-10-16T00:00:00Z\"}";

    var dto = JsonSerializer.Deserialize<DtoDateTime>(json, Options);

    dto!.Value.Should().NotBeNull();
    dto!.Value!.Value.Year.Should().Be(2026);
  }

  [Fact]
  public void NullJsonToken_ForNullableInt_ReturnsNull()
  {
    var json = "{\"Value\":null}";

    var dto = JsonSerializer.Deserialize<DtoInt>(json, Options);

    dto!.Value.Should().BeNull();
  }

  [Fact]
  public void CanConvert_ForNonNullableType_ReturnsFalse()
  {
    var converter = new LenientNullableJsonConverter();

    converter.CanConvert(typeof(int)).Should().BeFalse();
    converter.CanConvert(typeof(string)).Should().BeFalse();
  }

  [Fact]
  public void CanConvert_ForNullableStruct_ReturnsTrue()
  {
    var converter = new LenientNullableJsonConverter();

    converter.CanConvert(typeof(int?)).Should().BeTrue();
    converter.CanConvert(typeof(decimal?)).Should().BeTrue();
    converter.CanConvert(typeof(DateTime?)).Should().BeTrue();
  }

  [Fact]
  public void CanConvert_ForUnsupportedNullableType_ReturnsFalse()
  {
    var converter = new LenientNullableJsonConverter();

    // Enum no estÃ¡ en la lista de soportados.
    converter.CanConvert(typeof(DayOfWeek?)).Should().BeFalse();
  }
}
