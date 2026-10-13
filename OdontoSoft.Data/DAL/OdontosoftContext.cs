using System;
using System.Collections.Generic;
//using MedveCore.Entities.Base;
using Mauro.Entities.Base;
using Mauro.RepositoryPattern.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using OdontoSoft.Data.Authentication;
using OdontoSoft.Data.DAL.Generic;

namespace OdontoSoft.Data.DAL
{
    public partial class OdontosoftContext : DbContext
    {
        protected readonly ILogger _logger;
        private readonly IAuthenticatedUser _authenticatedUser;

        public OdontosoftContext(DbContextOptions<OdontosoftContext> options, ILogger<OdontosoftContext> logger, IAuthenticatedUser authenticatedUser)
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
            // ANTES había: modelBuilder.RemovePluralizingTableNameConvention();
            // Esa extension hace SetTableName(entity.DisplayName()) que escribe el nombre en
            // PascalCase y PISA la convención snake_case. Con Npgsql queremos snake_case
            // (PostgreSQL es case-insensitive por default, lo que rompe la consistencia
            // con los FKs generados). Si en el futuro hace falta, escribir una versión
            // snake_case-aware: SetTableName(SnakeCase(entity.DisplayName())).
            modelBuilder.Seed();
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges()
        {
            try
            {

                var changeSet = ChangeTracker.Entries<IAuditable>();

                if (changeSet != null && changeSet.Count() > 0)
                {
                    int userId = _authenticatedUser.GetId(); // Converter.ToInt32(this.MembershipHelper.GetUser().Id);
                    string userName = _authenticatedUser.GetUserName(); // "Sebas"; //this._membership.GetUserName();

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
                int result = base.SaveChanges();

                return result;
            }
            //    catch (DbEntityValidationException ex)
            //    {
            //        Logger.Instance.Log(ex);
            //        throw ex;
            //    }
            //    catch (DbUpdateException ex)
            //    {
            //        SqlException sqlException = null;

            //        try
            //        {
            //            sqlException = (SqlException)ex.InnerException.InnerException;
            //        }
            //        catch { }

            //        if (sqlException != null)
            //        {
            //            Logger.Instance.Log(sqlException);
            //            if (sqlException.Number == 2601)
            //            {
            //                throw new Exception("El registro ya existe. La operación ha sido cancelada.");
            //            }
            //            else
            //            {
            //                throw new Exception("Se ha producido un inconveniente al intentar guardar los datos. Por favor contacte al administrador del sistema. SqlNumberException: " + sqlException.Number);
            //            }
            //        }
            //        else
            //        {
            //            Logger.Instance.Log(ex);
            //            throw ex;
            //        }
            //        //(SqlException)ex.InnerException.InnerException	The type or namespace name 'SqlException' is not valid in this scope	


            //    }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }

        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
