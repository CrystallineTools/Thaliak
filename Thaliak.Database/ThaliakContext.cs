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
            .HasOne(v => v.Repository)
            .WithMany(r => r.Versions)
            .HasForeignKey(v => v.RepositoryId)
            .HasPrincipalKey(r => r.Id);

        builder.Entity<XivPatch>()
            .HasOne(p => p.Version)
            .WithMany(v => v.Patches)
            .HasForeignKey(p => p.VersionId)
            .HasPrincipalKey(v => v.Id);

        builder.Entity<XivPatch>()
            .HasOne(p => p.Repository)
            .WithMany(r => r.Patches)
            .HasForeignKey(p => p.RepositoryId)
            .HasPrincipalKey(r => r.Id);

        builder.Entity<XivFile>()
            .HasOne(f => f.Version)
            .WithMany(v => v.Files)
            .HasForeignKey(f => f.VersionId)
            .HasPrincipalKey(v => v.Id);

        builder.Entity<XivRepository>()
            .HasMany(r => r.ApplicableAccounts)
            .WithMany(a => a.ApplicableRepositories)
            .UsingEntity(j => j.ToTable("AccountRepositories"));

        // seed base repository data
        builder.Entity<XivRepository>()
            .HasData(
                new XivRepository
                {
                    Id = 1,
                    Name = "FFXIV Global - Boot - Win32",
                    RemoteOrigin =
                        "http://patch-bootver.ffxiv.com/http/win32/ffxivneo_release_boot/{version}/?time={time}"
                },
                new XivRepository
                {
                    Id = 2,
                    Name = "FFXIV Global - Game - Win32",
                    RemoteOrigin =
                        "https://patch-gamever.ffxiv.com/http/win32/ffxivneo_release_game/{version}/{session}"
                }
            );
    }
}
