using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Thaliak.Common.Database;

public class ThaliakContextFactory : IDesignTimeDbContextFactory<ThaliakContext>
{
    public ThaliakContext CreateDbContext(string[] args)
    {
        var ob = new DbContextOptionsBuilder<ThaliakContext>();
        ob.UseNpgsql("Host=localhost;Database=thaliak");

        return new ThaliakContext(ob.Options);
    }
}
