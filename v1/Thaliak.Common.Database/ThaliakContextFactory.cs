using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Thaliak.Common.Database;

public class ThaliakContextFactory : IDesignTimeDbContextFactory<ThaliakContext>
{
    public ThaliakContext CreateDbContext(string[] args)
    {
        var ob = new DbContextOptionsBuilder<ThaliakContext>();
        ob.UseNpgsql("Host=localhost;Database=thaliak")
            .ReplaceService<IHistoryRepository, CamelCaseHistoryContext>()
            .UseSnakeCaseNamingConvention();

        return new ThaliakContext(ob.Options);
    }
}
