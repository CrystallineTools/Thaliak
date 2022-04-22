using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Thaliak.Database.Util;

// yoinked from https://stackoverflow.com/a/56657516
public static class CustomModelBuilder
{
    public static bool IsSignedInteger(this Type type)
        => type == typeof(int)
           || type == typeof(long)
           || type == typeof(short)
           || type == typeof(sbyte);

    public static void Seed<T>(this ModelBuilder modelBuilder, IEnumerable<T> data) where T : class
    {
        var entity = modelBuilder.Entity<T>();

        var pk = entity.Metadata
            .GetProperties()
            .FirstOrDefault(property =>
                property.RequiresValueGenerator()
                && property.IsPrimaryKey()
                && property.ClrType.IsSignedInteger()
                && property.ClrType.IsDefaultValue(0)
            );
        if (pk != null)
        {
            entity.Property(pk.Name).ValueGeneratedNever();
            entity.HasData(data);
            entity.Property(pk.Name).UseIdentityColumn();
        }
        else
        {
            entity.HasData(data);
        }
    }
}
