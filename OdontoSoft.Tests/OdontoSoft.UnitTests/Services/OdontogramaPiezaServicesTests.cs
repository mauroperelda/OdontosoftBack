using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OdontoSoft.Application.Abstractions;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Application.Abstractions.Persistence;
using OdontoSoft.Domain.Models;
using OdontoSoft.Contracts.Dtos;
using OdontoSoft.Application.Services;
using Xunit;

namespace OdontoSoft.UnitTests.Services;

/// <summary>
/// Tests de OdontogramaPiezaServices. Cubre la regla de upsert por
/// (HC, NumeroPieza) — la misma que usa HistoriaClinicaController.GuardarPieza.
/// </summary>
public class OdontogramaPiezaServicesTests
{
  private static OdontogramaPiezaServices CreateService(Mock<IOdontogramaPiezaRepository> repo)
  {
    var uow = new Mock<IUnitOfWork>();
    uow.Setup(u => u.OdontogramaPiezaRepository).Returns(repo.Object);
    return new OdontogramaPiezaServices(
        NullLogger<OdontogramaPiezaServices>.Instance,
        uow.Object,
        new Mock<ICurrentUser>().Object);
  }

  [Fact]
  public void UpsertByPieza_PiezaNueva_RetornaOk()
  {
    var repo = new Mock<IOdontogramaPiezaRepository>();
    repo.Setup(r => r.GetByHistoriaClinicaIdAndNumeroPieza(1, 11))
        .Returns((OdontogramaPieza?)null);
    var service = CreateService(repo);

    var result = service.UpsertByPieza(new OdontogramaPiezaDto
    {
      HistoriaClinicaId = 1,
      NumeroPieza = 11,
    });

    result.IsSuccess.Should().BeTrue();
    repo.Verify(r => r.Add(It.IsAny<OdontogramaPieza>()), Times.Once);
  }

  [Fact]
  public void UpsertByPieza_PiezaExistente_RetornaOkYUsaUpsert()
  {
    var repo = new Mock<IOdontogramaPiezaRepository>();
    var piezaExistente = new OdontogramaPieza { Id = 7, HistoriaClinicaId = 1, NumeroPieza = 11 };
    repo.Setup(r => r.GetByHistoriaClinicaIdAndNumeroPieza(1, 11))
        .Returns(piezaExistente);
    var service = CreateService(repo);

    var result = service.UpsertByPieza(new OdontogramaPiezaDto
    {
      HistoriaClinicaId = 1,
      NumeroPieza = 11,
    });

    result.IsSuccess.Should().BeTrue();
    repo.Verify(r => r.Upsert(It.IsAny<OdontogramaPieza>()), Times.Once);
    repo.Verify(r => r.Add(It.IsAny<OdontogramaPieza>()), Times.Never);
  }
}
