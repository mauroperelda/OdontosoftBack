using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Infrastructure.Data.Repository.UOW;
using OdontoSoft.Services;
using Xunit;

namespace OdontoSoft.UnitTests.Services;

/// <summary>
/// Tests de PresupuestoDetalleServices. Cubre las funciones puras
/// (CalcularSubtotal, ValidarCantidad). Las reglas se replican en
/// ApplicationProfile de AutoMapper — los tests verifican la versión
/// del Service para mantener coherencia entre ambas implementaciones.
/// </summary>
public class PresupuestoDetalleServicesTests
{
  private static PresupuestoDetalleServices CreateService() =>
      new(NullLogger<PresupuestoDetalleServices>.Instance,
          new Mock<IUnitOfWork>().Object,
          new Mock<IAuthenticatedUser>().Object);

  [Theory]
  [InlineData(0.0, 100.0, 0.0)]
  [InlineData(1.0, 100.0, 100.0)]
  [InlineData(3.0, 100.50, 301.50)]
  [InlineData(2.5, 10.0, 25.0)]
  [InlineData(10.0, 0.0, 0.0)]
  public void CalcularSubtotal_MultipliesCantidadByPrecioUnitario(double cantidad, double precio, double esperado)
  {
    var service = CreateService();

    // xUnit InlineData no soporta decimal; casteamos a decimal en el test.
    var subtotal = service.CalcularSubtotal((decimal)cantidad, (decimal)precio);

    subtotal.Should().Be((decimal)esperado);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  [InlineData(-0.01)]
  public void ValidarCantidad_RechazaCeroONegativos(decimal cantidad)
  {
    var service = CreateService();

    var result = service.ValidarCantidad(cantidad);

    result.IsSuccess.Should().BeFalse();
    result.Error.Should().Contain("mayor a 0");
    result.FieldErrors.Should().ContainKey("Cantidad");
  }

  [Theory]
  [InlineData(0.01)]
  [InlineData(1)]
  [InlineData(100)]
  public void ValidarCantidad_AceptaPositivos(decimal cantidad)
  {
    var service = CreateService();

    var result = service.ValidarCantidad(cantidad);

    result.IsSuccess.Should().BeTrue();
  }
}
