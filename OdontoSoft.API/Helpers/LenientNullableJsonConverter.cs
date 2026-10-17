using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OdontoSoft.API.Helpers
{
  /// <summary>
  /// Acepta string vacío / whitespace como <c>null</c> en propiedades
  /// <c>Nullable&lt;T&gt;</c> (int?, decimal?, DateTime?, etc.) y devuelve
  /// mensajes de error en español si el valor no se puede parsear.
  ///
  /// Esto evita errores como
  /// "$.DomicilioNumero: The JSON value could not be converted to System.Nullable`1[System.Int32]"
  /// cuando el front manda un campo de número vacío.
  /// </summary>
  public class LenientNullableJsonConverter : JsonConverterFactory
  {
    private static readonly Type[] s_supportedTypes =
    {
            typeof(int), typeof(long), typeof(short), typeof(byte),
            typeof(decimal), typeof(double), typeof(float),
            typeof(DateTime), typeof(DateTimeOffset), typeof(TimeSpan),
            typeof(Guid)
        };

    public override bool CanConvert(Type typeToConvert)
    {
      var underlying = Nullable.GetUnderlyingType(typeToConvert);
      if (underlying is null) return false;
      foreach (var t in s_supportedTypes)
        if (t == underlying) return true;
      return false;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
      var underlying = Nullable.GetUnderlyingType(typeToConvert)!;
      var converterType = typeof(InnerConverter<>).MakeGenericType(underlying);
      return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private sealed class InnerConverter<T> : JsonConverter<T?> where T : struct
    {
      public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
      {
        // Token null → null
        if (reader.TokenType == JsonTokenType.Null)
          return null;

        // String vacío / whitespace → null (la principal mejora)
        if (reader.TokenType == JsonTokenType.String)
        {
          var str = reader.GetString();
          if (string.IsNullOrWhiteSpace(str)) return null;

          try
          {
            return (T)Convert.ChangeType(str, typeof(T), CultureInfo.InvariantCulture);
          }
          catch (Exception ex) when (
              ex is FormatException ||
              ex is OverflowException ||
              ex is InvalidCastException ||
              ex is ArgumentException)
          {
            // Mensaje traducible para el usuario final.
            // Se eleva como JsonException para que el middleware
            // de error lo capture y devuelva { errors: { Field: [...] } }.
            throw new JsonException(
                $"El valor '{str}' no es un {NombreAmigable(typeof(T))} válido.",
                ex);
          }
        }

        // Para tokens numéricos, true/false, etc., usar el deserializer nativo.
        // Si no puede, lanza JsonException por sí mismo.
        return JsonSerializer.Deserialize<T>(ref reader, options);
      }

      public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
      {
        if (value.HasValue) JsonSerializer.Serialize(writer, value.Value, options);
        else writer.WriteNullValue();
      }

      private static string NombreAmigable(Type t) => t.Name switch
      {
        nameof(Int32) => "número entero",
        nameof(Int64) => "número entero",
        nameof(Int16) => "número entero",
        nameof(Byte) => "número entero",
        nameof(Decimal) => "número decimal",
        nameof(Double) => "número decimal",
        nameof(Single) => "número decimal",
        nameof(DateTime) => "fecha",
        nameof(DateTimeOffset) => "fecha",
        nameof(TimeSpan) => "intervalo de tiempo",
        nameof(Guid) => "identificador",
        _ => $"valor de tipo {t.Name}"
      };
    }
  }
}
