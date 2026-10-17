using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace OdontoSoft.Infrastructure.DAL.Generic
{
  public static class ModelBuilderExtensions
  {
    public static void RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder)
    {
      foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
      {
        if (entity.BaseType == null)
        {
          entity.SetTableName(entity.DisplayName());
        }
      }
    }

    public static void Seed(this ModelBuilder modelBuilder)
    {
      DbDataInitializer.Seed(modelBuilder);
    }
  }
}
