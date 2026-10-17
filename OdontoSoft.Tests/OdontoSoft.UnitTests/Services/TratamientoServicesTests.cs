using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OdontoSoft.Data.Authentication;
using OdontoSoft.Data.Repository.Interfaces;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Services;
using Xunit;

namespace OdontoSoft.UnitTests.Services;

/// <summary>
/// Tests de TratamientoServices. Cubre la validación de UpsertPrecio
/// (regla que antes vivía en TratamientoController).
/// </summary>
public class TratamientoServicesTests
{
  private static TratamientoServices CreateService(
      Mock<ITratamientoRepository> tratamientoRepo,
      Mock<IUbicacionRepository> ubicacionRepo,
      Mock<ITratamientoPrecioRepository> precioRepo)
  {
    var uow = new Mock<IUnitOfWork>();
    uow.Setup(u => u.TratamientoRepository).Returns(tratamientoRepo.Object);
    uow.Setup(u => u.UbicacionRepository).Returns(ubicacionRepo.Object);
    uow.Setup(u => u.TratamientoPrecioRepository).Returns(precioRepo.Object);
    return new TratamientoServices(
        NullLogger<TratamientoServices>.Instance,
        uow.Object,
        new Mock<IAuthenticatedUser>().Object);
  }

  [Fact]
  public void UpsertPrecio_UbicacionIdCero_RetornaFail()
  {
    var t = new Mock<ITratamientoRepository>();
    var u = new Mock<IUbicacionRepository>();
    var p = new Mock<ITratamientoPrecioRepository>();
    var service = CreateService(t, u, p);

    var result = service.UpsertPrecio(1, 0, 100);

    result.IsSuccess.Should().BeFalse();
    result.Error.Should().Contain("UbicacionId");
    result.FieldErrors.Should().ContainKey("UbicacionId");
  }

  [Fact]
  public void UpsertPrecio_PrecioNegativo_RetornaFail()
  {
    var t = new Mock<ITratamientoRepository>();
    var u = new Mock<IUbicacionRepository>();
    var p = new Mock<ITratamientoPrecioRepository>();
    var service = CreateService(t, u, p);

    var result = service.UpsertPrecio(1, 1, -10);

    result.IsSuccess.Should().BeFalse();
    result.Error.Should().Contain("negativo");
    result.FieldErrors.Should().ContainKey("Precio");
  }

  [Fact]
  public void UpsertPrecio_TratamientoNoExiste_RetornaFail()
  {
    var t = new Mock<ITratamientoRepository>();
    t.Setup(r => r.GetById(99)).Returns((OdontoSoft.Domain.Models.Tratamiento?)null);
    var u = new Mock<IUbicacionRepository>();
    var p = new Mock<ITratamientoPrecioRepository>();
    var service = CreateService(t, u, p);

    var result = service.UpsertPrecio(99, 1, 100);

    result.IsSuccess.Should().BeFalse();
    result.Error.Should().Contain("Tratamiento no encontrado");
  }

  [Fact]
  public void UpsertPrecio_UbicacionNoExiste_RetornaFail()
  {
    var t = new Mock<ITratamientoRepository>();
    t.Setup(r => r.GetById(1)).Returns(new OdontoSoft.Domain.Models.Tratamiento { Id = 1 });
    var u = new Mock<IUbicacionRepository>();
    u.Setup(r => r.GetById(99)).Returns((OdontoSoft.Domain.Models.Ubicacion?)null);
    var p = new Mock<ITratamientoPrecioRepository>();
    var service = CreateService(t, u, p);

    var result = service.UpsertPrecio(1, 99, 100);

    result.IsSuccess.Should().BeFalse();
    result.Error.Should().Contain("Ubicación no encontrada");
  }
}
