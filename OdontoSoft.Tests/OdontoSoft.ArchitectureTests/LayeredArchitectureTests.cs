using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace OdontoSoft.ArchitectureTests;

/// <summary>
/// Reglas de arquitectura en capas — verifican que las dependencias entre
/// proyectos respeten la dirección esperada de Clean Architecture:
///
///   Api → Infrastructure → Application → Domain
///   (Api también referencia Contracts, que vive al lado de Domain)
///   Application NO referencia Infrastructure (inversión de dependencia)
///   Contracts es hoja: no conoce nada del lado del dominio ni infra.
/// </summary>
public class LayeredArchitectureTests
{
  // ── Namespaces raíz de cada proyecto. ──────────────────────────
  // NetArchTest matchea por namespace raíz con Types.InNamespace,
  // así que cualquiera de los sub-namespaces también matchea.
  private const string DomainNamespace        = "OdontoSoft.Domain";
  private const string ContractsNamespace     = "OdontoSoft.Contracts";
  private const string ApplicationNamespace   = "OdontoSoft.Application";
  private const string InfrastructureNamespace = "OdontoSoft.Infrastructure";
  private const string ApiNamespace           = "OdontoSoft.API";
  private const string ApiControllersNamespace = "OdontoSoft.API.Controllers";
  private const string WebLegacyNamespace     = "OdontoSoft.Web";

  // ── Domain: la raíz. No debe depender de nadie de la solución. ─
  [Fact]
  public void Domain_Should_Not_Depend_On_Any_Project()
  {
    var result = Types.InNamespace(DomainNamespace)
      .ShouldNot()
      .HaveDependencyOnAny(
        ContractsNamespace,
        ApplicationNamespace,
        InfrastructureNamespace,
        ApiNamespace,
        WebLegacyNamespace)
      .GetResult();

    result.IsSuccessful.Should().BeTrue(
      $"Domain no debe depender de ningún otro proyecto. Fallaron: {string.Join(", ", result.FailingTypeNames ?? System.Array.Empty<string>())}");
  }

  // ── Contracts: hoja. No conoce Application/Infra/Api. ──────────
  [Fact]
  public void Contracts_Should_Not_Depend_On_Application()
  {
    var result = Types.InNamespace(ContractsNamespace)
      .ShouldNot()
      .HaveDependencyOn(ApplicationNamespace)
      .GetResult();

    result.IsSuccessful.Should().BeTrue(
      $"Contracts no debe depender de Application. Fallaron: {string.Join(", ", result.FailingTypeNames ?? System.Array.Empty<string>())}");
  }

  [Fact]
  public void Contracts_Should_Not_Depend_On_Infrastructure()
  {
    var result = Types.InNamespace(ContractsNamespace)
      .ShouldNot()
      .HaveDependencyOn(InfrastructureNamespace)
      .GetResult();

    result.IsSuccessful.Should().BeTrue(
      $"Contracts no debe depender de Infrastructure. Fallaron: {string.Join(", ", result.FailingTypeNames ?? System.Array.Empty<string>())}");
  }

  [Fact]
  public void Contracts_Should_Not_Depend_On_Api()
  {
    var result = Types.InNamespace(ContractsNamespace)
      .ShouldNot()
      .HaveDependencyOn(ApiNamespace)
      .GetResult();

    result.IsSuccessful.Should().BeTrue(
      $"Contracts no debe depender de Api. Fallaron: {string.Join(", ", result.FailingTypeNames ?? System.Array.Empty<string>())}");
  }

  // ── Application: depende solo de Domain y Contracts. ────────────
  // El test clave de la inversión de dependencia: Application NO
  // debe conocer Infrastructure, porque Infrastructure implementa
  // las interfaces de Application.
  [Fact]
  public void Application_Should_Not_Depend_On_Infrastructure()
  {
    var result = Types.InNamespace(ApplicationNamespace)
      .ShouldNot()
      .HaveDependencyOn(InfrastructureNamespace)
      .GetResult();

    result.IsSuccessful.Should().BeTrue(
      $"Application NO debe depender de Infrastructure (inversión de dependencia). Fallaron: {string.Join(", ", result.FailingTypeNames ?? System.Array.Empty<string>())}");
  }

  [Fact]
  public void Application_Should_Not_Depend_On_Api()
  {
    var result = Types.InNamespace(ApplicationNamespace)
      .ShouldNot()
      .HaveDependencyOn(ApiNamespace)
      .GetResult();

    result.IsSuccessful.Should().BeTrue(
      $"Application no debe depender de Api. Fallaron: {string.Join(", ", result.FailingTypeNames ?? System.Array.Empty<string>())}");
  }

  [Fact]
  public void Application_Should_Not_Depend_On_Controllers()
  {
    var result = Types.InNamespace(ApplicationNamespace)
      .ShouldNot()
      .HaveDependencyOn(ApiControllersNamespace)
      .GetResult();

    result.IsSuccessful.Should().BeTrue(
      $"Application no debe depender de Controllers. Fallaron: {string.Join(", ", result.FailingTypeNames ?? System.Array.Empty<string>())}");
  }

  // ── Infrastructure: implementa Application, no conoce Api. ──────
  [Fact]
  public void Infrastructure_Should_Not_Depend_On_Api()
  {
    var result = Types.InNamespace(InfrastructureNamespace)
      .ShouldNot()
      .HaveDependencyOn(ApiNamespace)
      .GetResult();

    result.IsSuccessful.Should().BeTrue(
      $"Infrastructure no debe depender de Api. Fallaron: {string.Join(", ", result.FailingTypeNames ?? System.Array.Empty<string>())}");
  }

  // ── Reglas de contratos entre capas ─────────────────────────────
  // La API NO debe usar el nombre legacy IAuthenticatedUser (de Data);
  // debe usar ICurrentUser (de Application) que es la abstracción
  // invertida en C2.3.
  [Fact]
  public void Api_Should_Not_Use_Legacy_IAuthenticatedUser()
  {
    var result = Types.InNamespace(ApiNamespace)
      .ShouldNot()
      .HaveDependencyOn("OdontoSoft.Infrastructure.Data.Authentication")
      .GetResult();

    result.IsSuccessful.Should().BeTrue(
      $"Api no debe referenciar IAuthenticatedUser legacy; debe usar ICurrentUser. Fallaron: {string.Join(", ", result.FailingTypeNames ?? System.Array.Empty<string>())}");
  }

  // Stored procedures: la capa de Infrastructure NO debe retornar
  // DTOs de Contracts (porque eso acopla la persistencia a la API).
  // El refactor (V4) introduce un record interno (PacienteSummary) y
  // el controller mapea a PacienteDto con AutoMapper.
  [Fact]
  public void Infrastructure_StoredProcedures_Should_Not_Return_Dtos()
  {
    // La capa Repository.StoredProcedure (donde vive StoredProcedureRepository
    // + PacienteSummary) no debe usar DTOs de Contracts. Si vuelve a aparecer
    // una referencia a OdontoSoft.Contracts.Dtos en ese namespace, este test
    // falla y hay que refactorizar a un record interno (como PacienteSummary).
    var result = Types.InNamespace("OdontoSoft.Infrastructure.Repository.StoredProcedure")
      .ShouldNot()
      .HaveDependencyOn("OdontoSoft.Contracts.Dtos")
      .GetResult();

    result.IsSuccessful.Should().BeTrue(
      $"Infrastructure no debe retornar DTOs de Contracts desde stored procedures. Fallaron: {string.Join(", ", result.FailingTypeNames ?? System.Array.Empty<string>())}");
  }
}
