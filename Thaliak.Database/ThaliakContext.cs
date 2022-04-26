using Microsoft.EntityFrameworkCore;
using Thaliak.Database.Models;
using Thaliak.Database.Util;

namespace Thaliak.Database;

public class ThaliakContext : DbContext
{
    public DbSet<XivAccount> Accounts { get; set; }
    public DbSet<XivPatch> Patches { get; set; }
    public DbSet<XivRepository> Repositories { get; set; }
    public DbSet<XivVersion> Versions { get; set; }
    public DbSet<XivFile> Files { get; set; }
    public DbSet<XivExpansion> Expansions { get; set; }
    public DbSet<DiscordHookEntry> DiscordHooks { get; set; }

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

        // store patch hashes as comma-separated strings
        builder.Entity<XivPatch>()
            .Property(p => p.Hashes)
            .HasConversion(
                v => v == null ? null : string.Join(',', v),
                v => v == null ? null : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
            );

        builder.Entity<XivFile>()
            .HasOne(f => f.Version)
            .WithMany(v => v.Files)
            .HasForeignKey(f => f.VersionId)
            .HasPrincipalKey(v => v.Id);

        builder.Entity<XivVersion>()
            .HasOne(v => v.Expansion)
            .WithMany(e => e.Versions)
            .HasForeignKey(v => v.ExpansionId)
            .HasPrincipalKey(e => e.Id);

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
                    Name = "FFXIV Global/JPN - Boot - Win32",
                    RemoteOrigin =
                        "http://patch-bootver.ffxiv.com/http/win32/ffxivneo_release_boot/{version}/?time={time}",
                    Slug = "jp-win-boot"
                },
                new XivRepository
                {
                    Id = 2,
                    Name = "FFXIV Global/JPN - Game - Win32",
                    RemoteOrigin =
                        "https://patch-gamever.ffxiv.com/http/win32/ffxivneo_release_game/{version}/{session}",
                    Slug = "jp-win-game"
                }
            );

        // seed XIV expac data
        // we use a custom seeder here to bypass EF Core's stupid fucking validation...
        // "a non-zero value is required" fuck you for thinking you know what I want, asshole
        builder.Seed(new[]
        {
            new XivExpansion {Id = 0, Name = "A Realm Reborn", Abbreviation = "ARR"},
            new XivExpansion {Id = 1, Name = "Heavensward", Abbreviation = "HW"},
            new XivExpansion {Id = 2, Name = "Stormblood", Abbreviation = "SB"},
            new XivExpansion {Id = 3, Name = "Shadowbringers", Abbreviation = "ShB"},
            new XivExpansion {Id = 4, Name = "Endwalker", Abbreviation = "EW"}
        });
    }
}
