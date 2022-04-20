using Microsoft.EntityFrameworkCore;
using Thaliak.Database.Models;

namespace Thaliak.Database;

public class ThaliakContext : DbContext
{
    public DbSet<XivAccount> Accounts { get; set; }
    public DbSet<XivPatch> Patches { get; set; }
    public DbSet<XivRepository> Repositories { get; set; }
    public DbSet<XivVersion> Versions { get; set; }
    public DbSet<XivFile> Files { get; set; }

    public ThaliakContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<XivVersion>()
            .HasMany(v => v.Files)
            .WithOne(f => f.Version);

        builder.Entity<XivVersion>()
            .HasMany(v => v.Patches)
            .WithOne(p => p.Version);

        builder.Entity<XivVersion>()
            .HasOne(v => v.Repository)
            .WithMany(r => r.Versions);

        builder.Entity<XivRepository>()
            .HasMany(r => r.ApplicableAccounts)
            .WithMany(a => a.ApplicableRepositories)
            .UsingEntity(j => j.ToTable("AccountRepositories"));
    }
}
