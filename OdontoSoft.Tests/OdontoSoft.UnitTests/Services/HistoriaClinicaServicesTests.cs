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
/// Tests de HistoriaClinicaServices. Cubre la regla "una HC por paciente"
/// (hoy la uniqueness la atrapa el handler de unique violation, pero
/// agregar la validación acá evita el round-trip al motor de DB).
/// </summary>
public class HistoriaClinicaServicesTests
{
  private static HistoriaClinicaServices CreateService(Mock<IHistoriaClinicaRepository> hcRepo)
  {
    var uow = new Mock<IUnitOfWork>();
    uow.Setup(u => u.HistoriaClinicaRepository).Returns(hcRepo.Object);
    return new HistoriaClinicaServices(
        NullLogger<HistoriaClinicaServices>.Instance,
        uow.Object,
        new Mock<ICurrentUser>().Object);
  }

  [Fact]
  public void Create_PacienteSinHC_RetornaCreated()
  {
    var repo = new Mock<IHistoriaClinicaRepository>();
    repo.Setup(r => r.GetByPacienteId(1)).Returns((HistoriaClinica?)null);
    var service = CreateService(repo);

    var result = service.Create(new HistoriaClinicaDto { PacienteId = 1 });

    result.IsSuccess.Should().BeTrue();
    result.Id.Should().NotBeNull();
  }

  [Fact]
  public void Create_PacienteYaTieneHC_RetornaFailConFieldError()
  {
    var repo = new Mock<IHistoriaClinicaRepository>();
    repo.Setup(r => r.GetByPacienteId(1))
        .Returns(new HistoriaClinica { Id = 5, PacienteId = 1 });
    var service = CreateService(repo);

    var result = service.Create(new HistoriaClinicaDto { PacienteId = 1 });

    result.IsSuccess.Should().BeFalse();
    result.Error.Should().Contain("ya tiene una historia clínica");
    result.FieldErrors.Should().ContainKey("PacienteId");
  }

  [Fact]
  public void Update_HCNoExiste_RetornaFail()
  {
    var repo = new Mock<IHistoriaClinicaRepository>();
    repo.Setup(r => r.GetById(99)).Returns((HistoriaClinica?)null);
    var service = CreateService(repo);

    var result = service.Update(99, new HistoriaClinicaDto { PacienteId = 1 });

    result.IsSuccess.Should().BeFalse();
    result.Error.Should().Be("Historia clínica no encontrada.");
  }
}
