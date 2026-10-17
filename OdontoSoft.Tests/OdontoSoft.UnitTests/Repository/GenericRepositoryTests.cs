using System.Reflection;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using OdontoSoft.Infrastructure.Data.Authentication;
using OdontoSoft.Infrastructure.Data.DAL;
using OdontoSoft.Infrastructure.Data.Repository;
using OdontoSoft.Domain.Models;
using Xunit;

namespace OdontoSoft.UnitTests.Repository;

/// <summary>
/// Smoke tests del GenericRepository&lt;T&gt;. Cubre las operaciones CRUD
/// básicas que usan los controllers hoy, sin depender de Postgres real.
///
/// Importante: no reemplaza a tests de integración con Postgres. Estos tests
/// solo verifican que el GenericRepository se comporte como esperamos antes de
/// refactorizarlo en Fase 3.
/// </summary>
public class GenericRepositoryTests
{
  private static OdontosoftContext CreateContext(string dbName)
  {
    return new OdontosoftContext(
      new DbContextOptionsBuilder<OdontosoftContext>()
        .UseInMemoryDatabase(databaseName: dbName)
        .Options,
      NullLogger<OdontosoftContext>.Instance,
      new FakeUser());
  }

  [Fact]
  public async Task Add_PersistsEntity_WhenSaveChangesCalled()
  {
    using var ctx = CreateContext(nameof(Add_PersistsEntity_WhenSaveChangesCalled));
    var repo = new GenericRepository<Paciente>(ctx, NullLogger<GenericRepository<Paciente>>.Instance, new FakeUser());

    var entity = new Paciente { Nombre = "Lucía", Apellido = "Fernández", Dni = "11111111" };
    SetObjectState(entity, "Added");
    repo.Add(entity);

    await ctx.SaveChangesAsync();

    var loaded = await ctx.Paciente.FindAsync(entity.Id);
    loaded.Should().NotBeNull();
    loaded!.Nombre.Should().Be("Lucía");
  }

  [Fact]
  public async Task GetById_ReturnsInsertedEntity()
  {
    using var ctx = CreateContext(nameof(GetById_ReturnsInsertedEntity));
    var repo = new GenericRepository<Paciente>(ctx, NullLogger<GenericRepository<Paciente>>.Instance, new FakeUser());

    var entity = new Paciente { Nombre = "Mario", Apellido = "Rossi", Dni = "22222222" };
    SetObjectState(entity, "Added");
    repo.Add(entity);
    await ctx.SaveChangesAsync();

    var loaded = repo.GetById(entity.Id);

    loaded.Should().NotBeNull();
    loaded!.Dni.Should().Be("22222222");
  }

  [Fact]
  public async Task Upsert_UpdatesExistingEntity()
  {
    using var ctx = CreateContext(nameof(Upsert_UpdatesExistingEntity));
    var repo = new GenericRepository<Paciente>(ctx, NullLogger<GenericRepository<Paciente>>.Instance, new FakeUser());

    var entity = new Paciente { Nombre = "Ana", Apellido = "López", Dni = "33333333" };
    SetObjectState(entity, "Added");
    repo.Add(entity);
    await ctx.SaveChangesAsync();

    // Modificamos y hacemos Upsert
    entity.Nombre = "Ana María";
    SetObjectState(entity, "Modified");
    repo.Upsert(entity);
    await ctx.SaveChangesAsync();

    var loaded = repo.GetById(entity.Id);
    loaded!.Nombre.Should().Be("Ana María");
  }

  [Fact]
  public async Task Delete_RemovesEntity()
  {
    using var ctx = CreateContext(nameof(Delete_RemovesEntity));
    var repo = new GenericRepository<Paciente>(ctx, NullLogger<GenericRepository<Paciente>>.Instance, new FakeUser());

    var entity = new Paciente { Nombre = "Pepe", Apellido = "Gómez", Dni = "44444444" };
    SetObjectState(entity, "Added");
    repo.Add(entity);
    await ctx.SaveChangesAsync();

    var id = entity.Id;
    var result = await repo.Delete(id);
    await ctx.SaveChangesAsync();

    result.Should().BeTrue();
    var loaded = repo.GetById(id);
    loaded.Should().BeNull();
  }

  [Fact]
  public async Task Delete_NonExistent_ReturnsFalse()
  {
    using var ctx = CreateContext(nameof(Delete_NonExistent_ReturnsFalse));
    var repo = new GenericRepository<Paciente>(ctx, NullLogger<GenericRepository<Paciente>>.Instance, new FakeUser());

    var result = await repo.Delete(99999);

    result.Should().BeFalse();
  }

  [Fact]
  public async Task GetAllEnabled_FiltersDisabledEntities()
  {
    using var ctx = CreateContext(nameof(GetAllEnabled_FiltersDisabledEntities));
    var repo = new GenericRepository<Paciente>(ctx, NullLogger<GenericRepository<Paciente>>.Instance, new FakeUser());

    // Insertar dos: uno habilitado, otro deshabilitado (soft-delete)
    var enabled = new Paciente { Nombre = "Activo", Apellido = "Uno", Dni = "55555555" };
    SetObjectState(enabled, "Added");
    repo.Add(enabled);

    var disabled = new Paciente { Nombre = "Inactivo", Apellido = "Dos", Dni = "66666666", Disabled = true };
    SetObjectState(disabled, "Added");
    repo.Add(disabled);

    await ctx.SaveChangesAsync();

    var visible = repo.GetAllEnabled().ToList();

    visible.Should().HaveCount(1);
    visible.Should().ContainSingle(p => p.Nombre == "Activo");
    visible.Should().NotContain(p => p.Nombre == "Inactivo");
  }

  // ── Helpers ─────────────────────────────────────────────────────────────

  private static void SetObjectState(object entity, string stateName)
  {
    var prop = entity.GetType().GetProperty("ObjectState",
      BindingFlags.Public | BindingFlags.Instance);
    prop.Should().NotBeNull();

    var enumType = AppDomain.CurrentDomain.GetAssemblies()
      .SelectMany(a => { try { return a.GetTypes(); } catch { return Array.Empty<Type>(); } })
      .FirstOrDefault(t => t.Name == "ObjectState" && t.IsEnum);
    enumType.Should().NotBeNull();

    var stateValue = Enum.Parse(enumType!, stateName);
    prop!.SetValue(entity, stateValue);
  }

  private sealed class FakeUser : IAuthenticatedUser
  {
    public int GetId() => 1;
    public string GetUserName() => "test";
    public bool GetAdministrador() => false;
    public bool GetSuperUsuario() => false;
    public string GetEmail() => "";
    public bool HasPermission(string permissionCode) => false;
  }
}
