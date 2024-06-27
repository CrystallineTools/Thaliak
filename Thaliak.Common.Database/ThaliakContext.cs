using Microsoft.EntityFrameworkCore;
using Thaliak.Common.Database.Models;

namespace Thaliak.Common.Database;

public class ThaliakContext : DbContext
{
    public DbSet<XivAccount> Accounts { get; set; }
    public DbSet<XivPatch> Patches { get; set; }
    public DbSet<XivUpgradePath> UpgradePaths { get; set; }
    public DbSet<XivService> Services { get; set; }
    public DbSet<XivRepository> Repositories { get; set; }
    public DbSet<XivExpansionRepositoryMapping> ExpansionRepositoryMappings { get; set; }
    public DbSet<XivGameVersion> GameVersions { get; set; }
    public DbSet<XivRepoVersion> RepoVersions { get; set; }
    public DbSet<XivFile> Files { get; set; }
    public DbSet<DiscordHookEntry> DiscordHooks { get; set; }

    public ThaliakContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        //
        // Service
        //
        builder.Entity<XivService>()
            .HasMany(s => s.GameVersions)
            .WithOne(gv => gv.Service)
            .HasForeignKey(gv => gv.ServiceId)
            .HasPrincipalKey(s => s.Id);
        
        builder.Entity<XivService>()
            .HasMany(s => s.Repositories)
            .WithOne(r => r.Service)
            .HasForeignKey(r => r.ServiceId)
            .HasPrincipalKey(s => s.Id);
        
        //
        // GameVersion
        //
        builder.Entity<XivGameVersion>()
            .HasMany(gv => gv.RepoVersions)
            .WithMany(rv => rv.GameVersions)
            .UsingEntity("game_version_repo_versions");
        
        //
        // Repository
        //
        builder.Entity<XivRepository>()
            .HasMany(r => r.RepoVersions)
            .WithOne(rv => rv.Repository)
            .HasForeignKey(rv => rv.RepositoryId)
            .HasPrincipalKey(r => r.Id);
        
        //
        // RepoVersion
        //
        builder.Entity<XivRepoVersion>()
            .HasMany(rv => rv.Patches)
            .WithOne(p => p.RepoVersion)
            .HasForeignKey(p => p.RepoVersionId)
            .HasPrincipalKey(rv => rv.Id);

        //
        // UpgradePath
        //
        builder.Entity<XivUpgradePath>()
            .HasOne(c => c.PreviousRepoVersion)
            .WithMany(p => p.DependentVersions)
            .HasForeignKey(c => c.PreviousRepoVersionId)
            .HasPrincipalKey(p => p.Id);

        builder.Entity<XivUpgradePath>()
            .HasOne(c => c.RepoVersion)
            .WithMany(p => p.PrerequisiteVersions)
            .HasForeignKey(c => c.RepoVersionId)
            .HasPrincipalKey(p => p.Id);
        
        builder.Entity<XivUpgradePath>()
            .HasOne(c => c.Repository)
            .WithMany(r => r.UpgradePaths)
            .HasForeignKey(c => c.RepositoryId)
            .HasPrincipalKey(r => r.Id);
        
        // https://stackoverflow.com/a/8289253
        builder.Entity<XivUpgradePath>()
            .HasIndex(nameof(XivUpgradePath.RepoVersionId), nameof(XivUpgradePath.PreviousRepoVersionId))
            .IsUnique()
            .HasFilter(@"""previous_repo_version_id"" IS NOT NULL");

        builder.Entity<XivUpgradePath>()
            .HasIndex(nameof(XivUpgradePath.RepoVersionId))
            .IsUnique()
            .HasFilter(@"""previous_repo_version_id"" IS NULL");
        
        //
        // Patch
        //
        
        // store patch hashes as comma-separated strings
        builder.Entity<XivPatch>()
            .Property(p => p.Hashes)
            .HasConversion(
                v => v == null ? null : string.Join(',', v),
                v => v == null ? null : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
            );
        
        // ensure the local storage path is correctly stored
        builder.Entity<XivPatch>()
            .Property(r => r.LocalStoragePath)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
        
        //
        // ExpansionRepositoryMapping
        //
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
        
        //
        // File
        //
        
        // SHA1 hashes are stored as 40 character long hex strings
        builder.Entity<XivFile>()
            .Property(f => f.SHA1)
            .HasMaxLength(40)
            .IsFixedLength();
        
        //
        // UNMIGRATED BELOW
        //
        builder.Entity<XivFile>()
            .HasMany(f => f.Versions)
            .WithMany(v => v.Files)
            .UsingEntity(j => j.ToTable("version_files"));

        builder.Entity<XivFile>()
            .HasKey(
                nameof(XivFile.Name),
                nameof(XivFile.SHA1)
            );

        builder.Entity<XivRepository>()
            .HasMany(r => r.ApplicableAccounts)
            .WithMany(a => a.ApplicableRepositories)
            .UsingEntity(j => j.ToTable("account_repositories"));

        builder.Entity<XivRepository>()
            .Property(r => r.Slug)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        //
        // Data Seeding
        //
        
        // seed service region data
        builder.Entity<XivService>()
            .HasData(
                new XivService
                {
                    Id = 1,
                    Name = "FFXIV Global",
                    Icon = "🇺🇳"
                },
                new XivService
                {
                    Id = 2,
                    Name = "FFXIV Korea",
                    Icon = "🇰🇷"
                },
                new XivService
                {
                    Id = 3,
                    Name = "FFXIV China",
                    Icon = "🇨🇳"
                }
            );

        // seed base repository data
        builder.Entity<XivRepository>()
            .HasData(
                new XivRepository[]
                {
                    // Global
                    new()
                    {
                        Id = 1,
                        Name = "ffxivneo/win32/release/boot",
                        Description = "FFXIV Global/JP - Retail - Boot - Win32",
                        ServiceId = 1
                    },
                    new()
                    {
                        Id = 2,
                        Name = "ffxivneo/win32/release/game",
                        Description = "FFXIV Global/JP - Retail - Base Game - Win32",
                        ServiceId = 1
                    },
                    new()
                    {
                        Id = 3,
                        Name = "ffxivneo/win32/release/ex1",
                        Description = "FFXIV Global/JP - Retail - ex1 (Heavensward) - Win32",
                        ServiceId = 1
                    },
                    new()
                    {
                        Id = 4,
                        Name = "ffxivneo/win32/release/ex2",
                        Description = "FFXIV Global/JP - Retail - ex2 (Stormblood) - Win32",
                        ServiceId = 1
                    },
                    new()
                    {
                        Id = 5,
                        Name = "ffxivneo/win32/release/ex3",
                        Description = "FFXIV Global/JP - Retail - ex3 (Shadowbringers) - Win32",
                        ServiceId = 1
                    },
                    new()
                    {
                        Id = 6,
                        Name = "ffxivneo/win32/release/ex4",
                        Description = "FFXIV Global/JP - Retail - ex4 (Endwalker) - Win32",
                        ServiceId = 1
                    },
                    new()
                    {
                        Id = 17,
                        Name = "ffxivneo/win32/release/ex5",
                        Description = "FFXIV Global/JP - Retail - ex5 (Dawntrail) - Win32",
                        ServiceId = 1
                    },
                    // Korea
                    new()
                    {
                        Id = 7,
                        Name = "actoz/win32/release_ko/game",
                        Description = "FFXIV Korea - Retail - Base Game - Win32",
                        ServiceId = 2
                    },
                    new()
                    {
                        Id = 8,
                        Name = "actoz/win32/release_ko/ex1",
                        Description = "FFXIV Korea - Retail - ex1 (Heavensward) - Win32",
                        ServiceId = 2
                    },
                    new()
                    {
                        Id = 9,
                        Name = "actoz/win32/release_ko/ex2",
                        Description = "FFXIV Korea - Retail - ex2 (Stormblood) - Win32",
                        ServiceId = 2
                    },
                    new()
                    {
                        Id = 10,
                        Name = "actoz/win32/release_ko/ex3",
                        Description = "FFXIV Korea - Retail - ex3 (Shadowbringers) - Win32",
                        ServiceId = 2
                    },
                    new()
                    {
                        Id = 11,
                        Name = "actoz/win32/release_ko/ex4",
                        Description = "FFXIV Korea - Retail - ex4 (Endwalker) - Win32",
                        ServiceId = 2
                    },
                    // China
                    new()
                    {
                        Id = 12,
                        Name = "shanda/win32/release_chs/game",
                        Description = "FFXIV China - Retail - Base Game - Win32",
                        ServiceId = 3
                    },
                    new()
                    {
                        Id = 13,
                        Name = "shanda/win32/release_chs/ex1",
                        Description = "FFXIV China - Retail - ex1 (Heavensward) - Win32",
                        ServiceId = 3
                    },
                    new()
                    {
                        Id = 14,
                        Name = "shanda/win32/release_chs/ex2",
                        Description = "FFXIV China - Retail - ex2 (Stormblood) - Win32",
                        ServiceId = 3
                    },
                    new()
                    {
                        Id = 15,
                        Name = "shanda/win32/release_chs/ex3",
                        Description = "FFXIV China - Retail - ex3 (Shadowbringers) - Win32",
                        ServiceId = 3
                    },
                    new()
                    {
                        Id = 16,
                        Name = "shanda/win32/release_chs/ex4",
                        Description = "FFXIV China - Retail - ex4 (Endwalker) - Win32",
                        ServiceId = 3
                    }
                }
            );

        // seed XIV expac data
        builder.Entity<XivExpansionRepositoryMapping>()
            .HasData(
                // Global
                new XivExpansionRepositoryMapping {GameRepositoryId = 2, ExpansionId = 0, ExpansionRepositoryId = 2},
                new XivExpansionRepositoryMapping {GameRepositoryId = 2, ExpansionId = 1, ExpansionRepositoryId = 3},
                new XivExpansionRepositoryMapping {GameRepositoryId = 2, ExpansionId = 2, ExpansionRepositoryId = 4},
                new XivExpansionRepositoryMapping {GameRepositoryId = 2, ExpansionId = 3, ExpansionRepositoryId = 5},
                new XivExpansionRepositoryMapping {GameRepositoryId = 2, ExpansionId = 4, ExpansionRepositoryId = 6},
                new XivExpansionRepositoryMapping {GameRepositoryId = 2, ExpansionId = 5, ExpansionRepositoryId = 17},
                // Korea
                new XivExpansionRepositoryMapping {GameRepositoryId = 7, ExpansionId = 0, ExpansionRepositoryId = 7},
                new XivExpansionRepositoryMapping {GameRepositoryId = 7, ExpansionId = 1, ExpansionRepositoryId = 8},
                new XivExpansionRepositoryMapping {GameRepositoryId = 7, ExpansionId = 2, ExpansionRepositoryId = 9},
                new XivExpansionRepositoryMapping {GameRepositoryId = 7, ExpansionId = 3, ExpansionRepositoryId = 10},
                new XivExpansionRepositoryMapping {GameRepositoryId = 7, ExpansionId = 4, ExpansionRepositoryId = 11},
                // China
                new XivExpansionRepositoryMapping {GameRepositoryId = 12, ExpansionId = 0, ExpansionRepositoryId = 12},
                new XivExpansionRepositoryMapping {GameRepositoryId = 12, ExpansionId = 1, ExpansionRepositoryId = 13},
                new XivExpansionRepositoryMapping {GameRepositoryId = 12, ExpansionId = 2, ExpansionRepositoryId = 14},
                new XivExpansionRepositoryMapping {GameRepositoryId = 12, ExpansionId = 3, ExpansionRepositoryId = 15},
                new XivExpansionRepositoryMapping {GameRepositoryId = 12, ExpansionId = 4, ExpansionRepositoryId = 16}
            );
    }
}
