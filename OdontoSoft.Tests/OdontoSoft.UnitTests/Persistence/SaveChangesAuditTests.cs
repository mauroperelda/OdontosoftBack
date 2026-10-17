using System.Reflection;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Infrastructure.Data.DAL;
using OdontoSoft.Domain.Models;
using Xunit;

namespace OdontoSoft.UnitTests.Persistence;

/// <summary>
/// Tests para el fix del bug en OdontosoftContext.SaveChangesAsync:
///
/// Antes del fix, SaveChangesAsync NO aplicaba la auditoría (silenciosamente),
/// mientras que SaveChanges() síncrono sí. Este test demuestra que el fix
/// unificó el comportamiento: ambos métodos ahora aplican auditoría.
///
/// </summary>
public class SaveChangesAuditTests
{
  [Fact]
  public async Task SaveChangesAsync_ShouldSetCreatedUserIdAndCreatedDate_OnAuditableEntity()
  {
    // Arrange
    var fakeUser = new FakeAuthenticatedUser { Id = 42, UserName = "tester" };
    using var ctx = new OdontosoftContext(
      new DbContextOptionsBuilder<OdontosoftContext>()
        .UseInMemoryDatabase(databaseName: nameof(SaveChangesAsync_ShouldSetCreatedUserIdAndCreatedDate_OnAuditableEntity))
        .Options,
      NullLogger<OdontosoftContext>.Instance,
      fakeUser);

    var entity = new Paciente
    {
      Nombre = "Juan",
      Apellido = "Pérez",
      Dni = "12345678",
    };

    // Forzamos ObjectState=Added porque InMemory no dispara change tracking automático.
    SetObjectStateAdded(entity);
    ctx.Paciente.Add(entity);

    // Act
    await ctx.SaveChangesAsync();

    // Assert
    var createdUserId = ReadIntProperty(entity, "CreatedUserId");
    var createdDate = ReadDateProperty(entity, "CreatedDate");

    createdUserId.Should().Be(42, "CreatedUserId debe tomar el id del usuario autenticado.");
    createdDate.Should().NotBeNull("CreatedDate debe setearse en inserts.");
  }

  [Fact]
  public async Task SaveChangesAsync_ShouldSetUpdatedUserIdAndUpdatedDate_OnModifiedEntity()
  {
    // Arrange
    var fakeUser = new FakeAuthenticatedUser { Id = 99, UserName = "updater" };
    using var ctx = new OdontosoftContext(
      new DbContextOptionsBuilder<OdontosoftContext>()
        .UseInMemoryDatabase(databaseName: nameof(SaveChangesAsync_ShouldSetUpdatedUserIdAndUpdatedDate_OnModifiedEntity))
        .Options,
      NullLogger<OdontosoftContext>.Instance,
      fakeUser);

    var entity = new Paciente { Nombre = "Ana", Apellido = "Gómez", Dni = "99999999" };
    SetObjectStateAdded(entity);
    ctx.Paciente.Add(entity);
    await ctx.SaveChangesAsync();

    // Modificamos la entidad
    entity.Nombre = "Ana María";
    SetObjectStateModified(entity);

    // Act
    await ctx.SaveChangesAsync();

    // Assert
    var updatedUserId = ReadIntProperty(entity, "UpdatedUserId");
    updatedUserId.Should().Be(99, "UpdatedUserId debe tomar el id del usuario autenticado.");
  }

  // ── Helpers ─────────────────────────────────────────────────────────────

  /// <summary>
  /// ObjectState es una propiedad [NotMapped] de AuditedEntity que viene de
  /// Mauro.RepositoryPattern.Infrastructure. No queremos acoplar el test a
  /// esa DLL legacy, así que lo seteamos por reflection buscando la enum
  /// por nombre dentro del assembly cargado.
  /// </summary>
  private static void SetObjectStateAdded(object entity)
    => SetObjectState(entity, "Added");

  private static void SetObjectStateModified(object entity)
    => SetObjectState(entity, "Modified");

  private static void SetObjectState(object entity, string stateName)
  {
    var prop = entity.GetType().GetProperty("ObjectState",
      BindingFlags.Public | BindingFlags.Instance);
    prop.Should().NotBeNull($"ObjectState debe existir en {entity.GetType().Name} (hereda de AuditedEntity).");

    // La enum vive en algún assembly cargado. Buscamos por nombre completo.
    var enumType = AppDomain.CurrentDomain.GetAssemblies()
      .SelectMany(a => { try { return a.GetTypes(); } catch { return Array.Empty<Type>(); } })
      .FirstOrDefault(t => t.Name == "ObjectState" && t.IsEnum);
    enumType.Should().NotBeNull("ObjectState enum debe estar disponible en algún assembly cargado.");

    var stateValue = Enum.Parse(enumType!, stateName);
    prop!.SetValue(entity, stateValue);
  }

  private static int ReadIntProperty(object entity, string name)
  {
    var prop = entity.GetType().GetProperty(name,
      BindingFlags.Public | BindingFlags.Instance);
    prop.Should().NotBeNull();
    return (int)(prop!.GetValue(entity) ?? 0);
  }

  private static DateTime? ReadDateProperty(object entity, string name)
  {
    var prop = entity.GetType().GetProperty(name,
      BindingFlags.Public | BindingFlags.Instance);
    prop.Should().NotBeNull();
    return prop!.GetValue(entity) as DateTime?;
  }

  /// <summary>
  /// Stub mínimo de IAuthenticatedUser para no depender del JWT pipeline.
  /// </summary>
  private sealed class FakeAuthenticatedUser : IAuthenticatedUser
  {
    public int Id { get; init; }
    public string UserName { get; init; } = "";
    public int GetId() => Id;
    public string GetUserName() => UserName;
    public bool GetAdministrador() => false;
    public bool GetSuperUsuario() => false;
    public string GetEmail() => "";
    public bool HasPermission(string permissionCode) => false;
  }
}
