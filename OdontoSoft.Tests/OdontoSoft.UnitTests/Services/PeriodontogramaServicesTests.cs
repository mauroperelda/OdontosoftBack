using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OdontoSoft.Data.Authentication;
using OdontoSoft.Data.Repository.Interfaces;
using OdontoSoft.Data.Repository.UOW;
using OdontoSoft.Entities.Models;
using OdontoSoft.Services;
using Xunit;

namespace OdontoSoft.UnitTests.Services;

/// <summary>
/// Tests de PeriodontogramaServices. Cubre CalcularMetricas (función pura).
/// Las operaciones Create/Update/Delete requieren un InMemory DbContext
/// porque el repo encadena Save() — fuera del scope de esta tanda.
/// </summary>
public class PeriodontogramaServicesTests
{
  private static PeriodontogramaServices CreateService()
  {
    var uow = new Mock<IUnitOfWork>();
    uow.Setup(u => u.PeriodontogramaRepository).Returns(new Mock<IPeriodontogramaRepository>().Object);
    return new PeriodontogramaServices(
        NullLogger<PeriodontogramaServices>.Instance,
        uow.Object,
        new Mock<IAuthenticatedUser>().Object);
  }

  [Fact]
  public void CalcularMetricas_PiezasVacias_DevuelveTodoEnCeroY32Ausentes()
  {
    var service = CreateService();

    var metricas = service.CalcularMetricas(new List<PeriodontogramaPieza>());

    metricas.BopPorcentaje.Should().Be(0);
    metricas.PlacaPorcentaje.Should().Be(0);
    metricas.DientesPresentes.Should().Be(0);
    metricas.DientesAusentes.Should().Be(32);
  }

  [Fact]
  public void CalcularMetricas_UnaPiezaConSangrado_DevuelveBoP100()
  {
    var service = CreateService();
    var piezas = new List<PeriodontogramaPieza>
        {
            new() { NumeroPieza = 11, Sangrado = true, Placa = false },
        };

    var metricas = service.CalcularMetricas(piezas);

    metricas.BopPorcentaje.Should().Be(100);
    metricas.PlacaPorcentaje.Should().Be(0);
    metricas.DientesPresentes.Should().Be(1);
    metricas.DientesAusentes.Should().Be(31);
  }

  [Fact]
  public void CalcularMetricas_32Piezas_DevuelveTodosPresentesYCeroAusentes()
  {
    var service = CreateService();
    var piezas = Enumerable.Range(11, 32)
        .Select(n => new PeriodontogramaPieza { NumeroPieza = n, Sangrado = false, Placa = false })
        .ToList();

    var metricas = service.CalcularMetricas(piezas);

    metricas.DientesPresentes.Should().Be(32);
    metricas.DientesAusentes.Should().Be(0);
  }

  [Fact]
  public void CalcularMetricas_CincoDeDiezConSangrado_Devuelve50Porciento()
  {
    var service = CreateService();
    var piezas = Enumerable.Range(11, 10)
        .Select(n => new PeriodontogramaPieza { NumeroPieza = n, Sangrado = n % 2 == 0, Placa = false })
        .ToList();

    var metricas = service.CalcularMetricas(piezas);

    metricas.BopPorcentaje.Should().Be(50);
    metricas.DientesPresentes.Should().Be(10);
  }
}
