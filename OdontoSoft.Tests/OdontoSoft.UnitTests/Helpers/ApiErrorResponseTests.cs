using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using OdontoSoft.Infrastructure.Http;
using Xunit;

namespace OdontoSoft.UnitTests.Helpers;

public class ApiErrorResponseTests
{
  [Fact]
  public void BadRequest_WithMessageOnly_ReturnsShapeWithoutFieldErrors()
  {
    var result = ApiErrorResponse.BadRequest("Algo saliÃ³ mal.");

    result.Should().BeOfType<BadRequestObjectResult>();
    result.StatusCode.Should().Be(400);

    var shape = ShapeOf(result.Value!);
    shape["error"].Should().Be("Algo saliÃ³ mal.");
    shape["errors"].Should().BeOfType<Dictionary<string, string[]>>()
      .Which.Should().BeEmpty();
  }

  [Fact]
  public void BadRequest_WithFieldErrors_ReturnsShapeWithFieldErrors()
  {
    var fieldErrors = new Dictionary<string, string[]>
    {
      ["Nombre"] = new[] { "El nombre es obligatorio." },
      ["Dni"] = new[] { "El DNI es obligatorio.", "Formato invÃ¡lido." },
    };

    var result = ApiErrorResponse.BadRequest("RevisÃ¡ los campos.", fieldErrors);

    result.StatusCode.Should().Be(400);

    var shape = ShapeOf(result.Value!);
    shape["error"].Should().Be("RevisÃ¡ los campos.");
    var errors = shape["errors"].Should().BeOfType<Dictionary<string, string[]>>().Subject;
    errors["Nombre"].Should().BeEquivalentTo(new[] { "El nombre es obligatorio." });
    errors["Dni"].Should().BeEquivalentTo(new[] { "El DNI es obligatorio.", "Formato invÃ¡lido." });
  }

  [Fact]
  public void NotFound_ReturnsShape()
  {
    var result = ApiErrorResponse.NotFound("Paciente no encontrado.");

    result.Should().BeOfType<NotFoundObjectResult>();
    result.StatusCode.Should().Be(404);

    var shape = ShapeOf(result.Value!);
    shape["error"].Should().Be("Paciente no encontrado.");
    shape["errors"].Should().BeOfType<Dictionary<string, string[]>>()
      .Which.Should().BeEmpty();
  }

  [Fact]
  public void Conflict_WithFieldErrors_ReturnsShape()
  {
    var fieldErrors = new Dictionary<string, string[]>
    {
      ["Dni"] = new[] { "Este DNI ya estÃ¡ registrado." },
    };

    var result = ApiErrorResponse.Conflict("Conflicto.", fieldErrors);

    result.Should().BeOfType<ConflictObjectResult>();
    result.StatusCode.Should().Be(409);

    var shape = ShapeOf(result.Value!);
    shape["error"].Should().Be("Conflicto.");
  }

  [Fact]
  public void FromException_InvalidOperationException_ReturnsBadRequestWithOriginalMessage()
  {
    var ex = new InvalidOperationException("No se puede eliminar porque tiene turnos asociados.");

    var result = ApiErrorResponse.FromException(ex);

    result.Should().BeOfType<BadRequestObjectResult>();
    var shape = ShapeOf(((ObjectResult)result).Value);
    shape["error"].Should().Be("No se puede eliminar porque tiene turnos asociados.");
  }

  [Fact]
  public void FromException_GenericException_ReturnsBadRequestWithGenericMessage()
  {
    // No exponer mensaje crudo al cliente.
    var ex = new Exception("Error interno con datos sensibles.");

    var result = ApiErrorResponse.FromException(ex);

    result.Should().BeOfType<BadRequestObjectResult>();
    var shape = ShapeOf(((ObjectResult)result).Value);
    var errorMsg = (string)shape["error"]!;
    errorMsg.Should().Be("OcurriÃ³ un error al procesar la solicitud.");
    errorMsg.Should().NotContain("datos sensibles");
  }

  // â”€â”€ Helper â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
  private static Dictionary<string, object?> ShapeOf(object? anonObj)
  {
    anonObj.Should().NotBeNull();
    return anonObj!.GetType()
      .GetProperties()
      .ToDictionary(p => p.Name, p => p.GetValue(anonObj));
  }
}
