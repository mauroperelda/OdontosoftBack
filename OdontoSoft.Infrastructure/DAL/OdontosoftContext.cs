using System;
using System.Collections.Generic;
//using MedveCore.Entities.Base;
using Mauro.Entities.Base;
using Mauro.RepositoryPattern.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using OdontoSoft.Application.Abstractions;
using OdontoSoft.Infrastructure.DAL.Generic;

namespace OdontoSoft.Infrastructure.DAL
{
  public partial class OdontosoftContext : DbContext
  {
    protected readonly ILogger _logger;
    private readonly ICurrentUser _authenticatedUser;

    public OdontosoftContext(DbContextOptions<OdontosoftContext> options, ILogger<OdontosoftContext> logger, ICurrentUser authenticatedUser)
        : base(options)
    {
      _logger = logger;
      _authenticatedUser = authenticatedUser;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // NOTA: UseSnakeCaseNamingConvention() se aplica en AddDbContext (sobre el
      // DbContextOptionsBuilder) en Program.cs. No se puede llamar desde OnModelCreating
      // porque opera sobre el Model ANTES de que se construya.

      foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
      {
        relationship.DeleteBehavior = DeleteBehavior.Restrict;
      }
      // ANTES habÃ­a: modelBuilder.RemovePluralizingTableNameConvention();
      // Esa extension hace SetTableName(entity.DisplayName()) que escribe el nombre en
      // PascalCase y PISA la convenciÃ³n snake_case. Con Npgsql queremos snake_case
      // (PostgreSQL es case-insensitive por default, lo que rompe la consistencia
      // con los FKs generados). Si en el futuro hace falta, escribir una versiÃ³n
      // snake_case-aware: SetTableName(SnakeCase(entity.DisplayName())).
      modelBuilder.Seed();
      base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
      try
      {
        ApplyAudit();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error en SaveChangesAsync");
        throw;
      }
    }

    public override int SaveChanges()
    {
      try
      {
        ApplyAudit();
        return base.SaveChanges();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error en SaveChanges");
        throw;
      }
    }

    /// <summary>
    /// Setea los datos de auditorÃ­a (CreatedDate/UpdatedDate/CreatedUserId/UpdatedUserId)
    /// en todas las entidades IAuditable que estÃ¡n en el ChangeTracker.
    /// Llamado por SaveChanges() y SaveChangesAsync() para garantizar consistencia.
    /// </summary>
    private void ApplyAudit()
    {
      var changeSet = ChangeTracker.Entries<IAuditable>().ToList();
      if (changeSet.Count == 0) return;

      int userId = _authenticatedUser.Id;
      string userName = _authenticatedUser.UserName ?? string.Empty;
      DateTime auditDate = DateTime.Now;

      foreach (var dbEntityEntry in changeSet)
      {
        switch (((IObjectState)dbEntityEntry.Entity).ObjectState)
        {
          case ObjectState.Added:
            dbEntityEntry.Entity.SetInsertAuditData(userId, userName, auditDate);
            break;
          case ObjectState.Modified:
            dbEntityEntry.Entity.SetUpdateAuditData(userId, userName, auditDate);
            break;
        }
      }
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
  }
}
