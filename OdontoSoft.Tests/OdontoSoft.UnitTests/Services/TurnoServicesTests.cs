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
/// Tests de TurnoServices. Cubre la validación de conflicto de horario
/// (regla que antes vivía en TurnoController.Create, ahora en el service).
/// </summary>
public class TurnoServicesTests
{
  private static TurnoServices CreateService(out Mock<ITurnoRepository> turnoRepoMock)
  {
    var uowMock = new Mock<IUnitOfWork>();
    turnoRepoMock = new Mock<ITurnoRepository>();
    uowMock.Setup(u => u.TurnoRepository).Returns(turnoRepoMock.Object);
    return new TurnoServices(
        NullLogger<TurnoServices>.Instance,
        uowMock.Object,
        new Mock<IAuthenticatedUser>().Object);
  }

  [Fact]
  public void ValidarConflictoHorario_SinConflicto_RetornaOk()
  {
    var service = CreateService(out var turnoRepo);
    var hora = new DateTime(2026, 10, 16, 10, 0, 0);
    turnoRepo.Setup(r => r.GetByProfesionalByHoraInicio(1, hora)).Returns((Turno?)null);

    var result = service.ValidarConflictoHorario(1, hora);

    result.IsSuccess.Should().BeTrue();
    result.Error.Should().BeNull();
  }

  [Fact]
  public void ValidarConflictoHorario_ConConflicto_RetornaFail()
  {
    var service = CreateService(out var turnoRepo);
    var hora = new DateTime(2026, 10, 16, 10, 0, 0);
    turnoRepo.Setup(r => r.GetByProfesionalByHoraInicio(1, hora))
        .Returns(new Turno { Id = 5, ProfesionalId = 1, HoraInicio = hora });

    var result = service.ValidarConflictoHorario(1, hora);

    result.IsSuccess.Should().BeFalse();
    result.Error.Should().Contain("ya tiene un turno");
    result.FieldErrors.Should().ContainKey("HoraInicio");
  }

  [Fact]
  public void ValidarConflictoHorario_ConflictoEsElMismoTurno_RetornaOk()
  {
    var service = CreateService(out var turnoRepo);
    var hora = new DateTime(2026, 10, 16, 10, 0, 0);
    var mismoTurno = new Turno { Id = 5, ProfesionalId = 1, HoraInicio = hora };
    turnoRepo.Setup(r => r.GetByProfesionalByHoraInicio(1, hora)).Returns(mismoTurno);

    // excludeTurnoId = 5 debe ignorar el conflicto consigo mismo
    var result = service.ValidarConflictoHorario(1, hora, excludeTurnoId: 5);

    result.IsSuccess.Should().BeTrue();
  }
}
