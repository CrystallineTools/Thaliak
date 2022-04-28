using Microsoft.EntityFrameworkCore;
using Thaliak.Database.Models;

namespace Thaliak.Database;

public class ThaliakContext : DbContext
{
    public DbSet<XivAccount> Accounts { get; set; }
    public DbSet<XivPatch> Patches { get; set; }
    public DbSet<XivRepository> Repositories { get; set; }
    public DbSet<XivExpansionRepositoryMapping> ExpansionRepositoryMappings { get; set; }
    public DbSet<XivVersion> Versions { get; set; }
    public DbSet<XivFile> Files { get; set; }
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

        builder.Entity<XivRepository>()
            .HasMany(r => r.ApplicableAccounts)
            .WithMany(a => a.ApplicableRepositories)
            .UsingEntity(j => j.ToTable("AccountRepositories"));

        builder.Entity<XivRepository>()
            .Property(r => r.Slug)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Entity<XivExpansionRepositoryMapping>()
            .HasKey(
                nameof(XivExpansionRepositoryMapping.GameRepositoryId),
                nameof(XivExpansionRepositoryMapping.ExpansionId),
                nameof(XivExpansionRepositoryMapping.ExpansionRepositoryId)
            );

        builder.Entity<XivExpansionRepositoryMapping>()
            .HasOne(erp => erp.GameRepository)
            .WithMany()
            .HasForeignKey(erp => erp.GameRepositoryId)
            .HasPrincipalKey(r => r.Id);

        builder.Entity<XivExpansionRepositoryMapping>()
            .HasOne(erp => erp.ExpansionRepository)
            .WithMany()
            .HasForeignKey(erp => erp.ExpansionRepositoryId)
            .HasPrincipalKey(r => r.Id);

        // seed base repository data
        builder.Entity<XivRepository>()
            .HasData(
                new XivRepository
                {
                    Id = 1,
                    Name = "ffxivneo/win32/release/boot",
                    Description = "FFXIV Global/JP - Boot - Win32",
                },
                new XivRepository
                {
                    Id = 2,
                    Name = "ffxivneo/win32/release/game",
                    Description = "FFXIV Global/JP - Retail - Base Game - Win32"
                },
                new XivRepository
                {
                    Id = 3,
                    Name = "ffxivneo/win32/release/ex1",
                    Description = "FFXIV Global/JP - Retail - ex1 (Heavensward) - Win32"
                },
                new XivRepository
                {
                    Id = 4,
                    Name = "ffxivneo/win32/release/ex2",
                    Description = "FFXIV Global/JP - Retail - ex2 (Stormblood) - Win32"
                },
                new XivRepository
                {
                    Id = 5,
                    Name = "ffxivneo/win32/release/ex3",
                    Description = "FFXIV Global/JP - Retail - ex3 (Shadowbringers) - Win32"
                },
                new XivRepository
                {
                    Id = 6,
                    Name = "ffxivneo/win32/release/ex4",
                    Description = "FFXIV Global/JP - Retail - ex4 (Endwalker) - Win32"
                }
            );

        // seed XIV expac data
        builder.Entity<XivExpansionRepositoryMapping>()
            .HasData(
                new XivExpansionRepositoryMapping {GameRepositoryId = 2, ExpansionId = 0, ExpansionRepositoryId = 2},
                new XivExpansionRepositoryMapping {GameRepositoryId = 2, ExpansionId = 1, ExpansionRepositoryId = 3},
                new XivExpansionRepositoryMapping {GameRepositoryId = 2, ExpansionId = 2, ExpansionRepositoryId = 4},
                new XivExpansionRepositoryMapping {GameRepositoryId = 2, ExpansionId = 3, ExpansionRepositoryId = 5},
                new XivExpansionRepositoryMapping {GameRepositoryId = 2, ExpansionId = 4, ExpansionRepositoryId = 6}
            );
    }
}
