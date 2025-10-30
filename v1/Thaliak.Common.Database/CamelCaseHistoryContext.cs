using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations.Internal;

namespace Thaliak.Common.Database;

// https://github.com/efcore/EFCore.NamingConventions/issues/1#issuecomment-979473249
public class CamelCaseHistoryContext : NpgsqlHistoryRepository
{
    public CamelCaseHistoryContext(HistoryRepositoryDependencies dependencies) : base(dependencies)
    {
    }

    protected override void ConfigureTable(EntityTypeBuilder<HistoryRow> history)
    {
        base.ConfigureTable(history);

        history.Property(h => h.MigrationId).HasColumnName("MigrationId");
        history.Property(h => h.ProductVersion).HasColumnName("ProductVersion");
    }
}
