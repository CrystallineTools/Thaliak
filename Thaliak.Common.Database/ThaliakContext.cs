using Microsoft.EntityFrameworkCore;
using Thaliak.Common.Database.Models;

namespace Thaliak.Common.Database;

public class ThaliakContext : DbContext
{
    public DbSet<XivAccount> Accounts { get; set; }
    public DbSet<XivPatch> Patches { get; set; }
    public DbSet<XivPatchChain> PatchChains { get; set; }
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

        builder.Entity<XivPatch>()
            .Property(r => r.LocalStoragePath)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Entity<XivPatchChain>()
            .HasOne(c => c.PreviousPatch)
            .WithMany(p => p.DependentPatches)
            .HasForeignKey(c => c.PreviousPatchId)
            .HasPrincipalKey(p => p.Id);

        builder.Entity<XivPatchChain>()
            .HasOne(c => c.Patch)
            .WithMany(p => p.PrerequisitePatches)
            .HasForeignKey(c => c.PatchId)
            .HasPrincipalKey(p => p.Id);

        builder.Entity<XivFile>()
            .HasMany(f => f.Versions)
            .WithMany(v => v.Files)
            .UsingEntity(j => j.ToTable("VersionFiles"));

        builder.Entity<XivFile>()
            .HasKey(
                nameof(XivFile.Name),
                nameof(XivFile.SHA1)
            );

        builder.Entity<XivFile>()
            .HasIndex(f => f.LastUsed);

        // SHA1 hashes are stored as 40 character long hex strings
        builder.Entity<XivFile>()
            .Property(f => f.SHA1)
            .HasMaxLength(40)
            .IsFixedLength();

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

        builder.Entity<XivPatchChain>()
            .HasOne(c => c.Repository)
            .WithMany()
            .HasForeignKey(c => c.RepositoryId)
            .HasPrincipalKey(r => r.Id);

        // https://stackoverflow.com/a/8289253
        builder.Entity<XivPatchChain>()
            .HasIndex(nameof(XivPatchChain.PatchId), nameof(XivPatchChain.PreviousPatchId))
            .IsUnique()
            .HasFilter(@"""PreviousPatchId"" IS NOT NULL");

        builder.Entity<XivPatchChain>()
            .HasIndex(nameof(XivPatchChain.PatchId))
            .IsUnique()
            .HasFilter(@"""PreviousPatchId"" IS NULL");

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
                    },
                    new()
                    {
                        Id = 2,
                        Name = "ffxivneo/win32/release/game",
                        Description = "FFXIV Global/JP - Retail - Base Game - Win32"
                    },
                    new()
                    {
                        Id = 3,
                        Name = "ffxivneo/win32/release/ex1",
                        Description = "FFXIV Global/JP - Retail - ex1 (Heavensward) - Win32"
                    },
                    new()
                    {
                        Id = 4,
                        Name = "ffxivneo/win32/release/ex2",
                        Description = "FFXIV Global/JP - Retail - ex2 (Stormblood) - Win32"
                    },
                    new()
                    {
                        Id = 5,
                        Name = "ffxivneo/win32/release/ex3",
                        Description = "FFXIV Global/JP - Retail - ex3 (Shadowbringers) - Win32"
                    },
                    new()
                    {
                        Id = 6,
                        Name = "ffxivneo/win32/release/ex4",
                        Description = "FFXIV Global/JP - Retail - ex4 (Endwalker) - Win32"
                    },
                    // Korea
                    new()
                    {
                        Id = 7,
                        Name = "actoz/win32/release_ko/game",
                        Description = "FFXIV Korea - Retail - Base Game - Win32"
                    },
                    new()
                    {
                        Id = 8,
                        Name = "actoz/win32/release_ko/ex1",
                        Description = "FFXIV Korea - Retail - ex1 (Heavensward) - Win32"
                    },
                    new()
                    {
                        Id = 9,
                        Name = "actoz/win32/release_ko/ex2",
                        Description = "FFXIV Korea - Retail - ex2 (Stormblood) - Win32"
                    },
                    new()
                    {
                        Id = 10,
                        Name = "actoz/win32/release_ko/ex3",
                        Description = "FFXIV Korea - Retail - ex3 (Shadowbringers) - Win32"
                    },
                    new()
                    {
                        Id = 11,
                        Name = "actoz/win32/release_ko/ex4",
                        Description = "FFXIV Korea - Retail - ex4 (Endwalker) - Win32"
                    },
                    // China
                    new()
                    {
                        Id = 12,
                        Name = "shanda/win32/release_chs/game",
                        Description = "FFXIV China - Retail - Base Game - Win32"
                    },
                    new()
                    {
                        Id = 13,
                        Name = "shanda/win32/release_chs/ex1",
                        Description = "FFXIV China - Retail - ex1 (Heavensward) - Win32"
                    },
                    new()
                    {
                        Id = 14,
                        Name = "shanda/win32/release_chs/ex2",
                        Description = "FFXIV China - Retail - ex2 (Stormblood) - Win32"
                    },
                    new()
                    {
                        Id = 15,
                        Name = "shanda/win32/release_chs/ex3",
                        Description = "FFXIV China - Retail - ex3 (Shadowbringers) - Win32"
                    },
                    new()
                    {
                        Id = 16,
                        Name = "shanda/win32/release_chs/ex4",
                        Description = "FFXIV China - Retail - ex4 (Endwalker) - Win32"
                    },
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
