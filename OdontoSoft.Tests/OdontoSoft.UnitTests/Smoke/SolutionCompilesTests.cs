using FluentAssertions;
using Xunit;

namespace OdontoSoft.UnitTests.Smoke;

/// <summary>
/// Smoke tests — verifican que la infraestructura de tests funciona
/// (compilación de los proyectos referenciados, carga de tipos, etc.).
/// Las pruebas de verdad vienen en Fases 1 y 2.
/// </summary>
public class SolutionCompilesTests
{
  [Fact]
  public void SolutionReferencesCanBeLoaded()
  {
    // Si los ProjectReference están rotos, este test no compila.
    var apiAssembly = typeof(OdontoSoft.API.Controllers.PacienteController).Assembly;
    var dataAssembly = typeof(OdontoSoft.Infrastructure.Data.DAL.OdontosoftContext).Assembly;

    apiAssembly.Should().NotBeNull();
    dataAssembly.Should().NotBeNull();
    apiAssembly.GetName().Name.Should().Be("OdontoSoft.API");
    dataAssembly.GetName().Name.Should().Be("OdontoSoft.Infrastructure.Data");
  }

  [Fact]
  public void CoreTypesAreExported()
  {
    typeof(OdontoSoft.Domain.Models.Paciente).Should().NotBeNull();
    typeof(OdontoSoft.Contracts.Common.PaginationParams).Should().NotBeNull();
  }
}
