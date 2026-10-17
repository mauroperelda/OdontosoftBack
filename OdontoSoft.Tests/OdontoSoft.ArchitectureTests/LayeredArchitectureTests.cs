using System;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace OdontoSoft.ArchitectureTests;

/// <summary>
/// Reglas de arquitectura en capas — verifican que las dependencias entre
/// proyectos respeten la dirección esperada:
/// API → Services → Data → Entities (nunca al revés).
/// </summary>
public class LayeredArchitectureTests
{
  private const string ControllersNamespace = "OdontoSoft.API.Controllers";
  private const string ServicesNamespace = "Odontosoft.Services";
  private const string DataNamespace = "OdontoSoft.Data";
  private const string EntitiesNamespace = "OdontoSoft.Domain";
  private const string ApiNamespace = "OdontoSoft.API";

  [Fact]
  public void Entities_Should_Not_Depend_On_Data()
  {
    var result = Types.InNamespace(EntitiesNamespace)
      .ShouldNot()
      .HaveDependencyOn(DataNamespace)
      .GetResult();

    result.IsSuccessful.Should().BeTrue(
      $"Entities no debe depender de Data. Fallaron: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
  }

  [Fact]
  public void Entities_Should_Not_Depend_On_API()
  {
    var result = Types.InNamespace(EntitiesNamespace)
      .ShouldNot()
      .HaveDependencyOn(ApiNamespace)
      .GetResult();

    result.IsSuccessful.Should().BeTrue(
      $"Entities no debe depender de API. Fallaron: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
  }

  [Fact]
  public void Data_Should_Not_Depend_On_API()
  {
    var result = Types.InNamespace(DataNamespace)
      .ShouldNot()
      .HaveDependencyOn(ApiNamespace)
      .GetResult();

    result.IsSuccessful.Should().BeTrue(
      $"Data no debe depender de API. Fallaron: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
  }

  [Fact]
  public void Services_Should_Not_Depend_On_API()
  {
    // Services es lógica de dominio pura: no debe conocer controllers.
    var result = Types.InNamespace(ServicesNamespace)
      .ShouldNot()
      .HaveDependencyOn(ApiNamespace)
      .GetResult();

    result.IsSuccessful.Should().BeTrue(
      $"Services no debe depender de API. Fallaron: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
  }

  [Fact]
  public void Services_Should_Not_Depend_On_Controllers()
  {
    // Capa de lógica de negocio: no debe conocer la capa HTTP.
    var result = Types.InNamespace(ServicesNamespace)
      .ShouldNot()
      .HaveDependencyOn(ControllersNamespace)
      .GetResult();

    result.IsSuccessful.Should().BeTrue(
      $"Services no debe depender de Controllers. Fallaron: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
  }
}
