using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Infrastructure.Data.Repository.Interfaces;
using OdontoSoft.Infrastructure.Data.Repository.UOW;
using OdontoSoft.Domain.Models;
using OdontoSoft.Services;
using Xunit;

namespace OdontoSoft.UnitTests.Services;

/// <summary>
/// Tests de PacienteServices. Cubre la validación de DNI único que se
/// llama antes de Create/Update para devolver 400 con field-keyed error
/// en vez de esperar al DbUpdateException (23505) del INSERT.
/// </summary>
public class PacienteServicesTests
{
  /// <summary>
  /// Construye un IQueryable en memoria a partir de una lista, para
  /// poder mockear el IQueryable<Paciente> que devuelve GetAllEnabled().
  /// EF Core necesitaría un provider real para ejecutar queries async;
  /// acá lo importante es la cláusula .Any() / .Where() que se evalúa
  /// en memoria vía LINQ-to-Objects.
  /// </summary>
  private static Mock<IPacienteRepository> SetupRepoWith(params Paciente[] pacientes)
  {
    var mock = new Mock<IPacienteRepository>();
    var queryable = pacientes.AsQueryable();
    mock.Setup(r => r.GetAllEnabled()).Returns(queryable);
    return mock;
  }

  private static PacienteServices CreateService(Mock<IPacienteRepository> repoMock)
  {
    var uowMock = new Mock<IUnitOfWork>();
    uowMock.Setup(u => u.PacienteRepository).Returns(repoMock.Object);
    return new PacienteServices(
        NullLogger<PacienteServices>.Instance,
        uowMock.Object,
        new Mock<IAuthenticatedUser>().Object);
  }

  [Fact]
  public async Task ValidarDniUnico_DniVacio_RetornaFail()
  {
    var service = CreateService(SetupRepoWith());

    var result = await service.ValidarDniUnicoAsync("");

    result.IsSuccess.Should().BeFalse();
    result.Error.Should().Contain("DNI es obligatorio");
  }

  [Fact]
  public async Task ValidarDniUnico_DniNuevo_RetornaOk()
  {
    var service = CreateService(SetupRepoWith(
        new Paciente { Id = 1, Dni = "11111111" },
        new Paciente { Id = 2, Dni = "22222222" }));

    var result = await service.ValidarDniUnicoAsync("99999999");

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public async Task ValidarDniUnico_DniDuplicado_RetornaFail()
  {
    var service = CreateService(SetupRepoWith(
        new Paciente { Id = 1, Dni = "11111111" }));

    var result = await service.ValidarDniUnicoAsync("11111111");

    result.IsSuccess.Should().BeFalse();
    result.Error.Should().Contain("Ya existe");
    result.FieldErrors.Should().ContainKey("Dni");
  }

  [Fact]
  public async Task ValidarDniUnico_DuplicadoPeroEsElMismoPaciente_RetornaOk()
  {
    var service = CreateService(SetupRepoWith(
        new Paciente { Id = 7, Dni = "11111111" }));

    // En Update del paciente 7 con el mismo DNI que ya tiene → OK
    var result = await service.ValidarDniUnicoAsync("11111111", excludePacienteId: 7);

    result.IsSuccess.Should().BeTrue();
  }
}
