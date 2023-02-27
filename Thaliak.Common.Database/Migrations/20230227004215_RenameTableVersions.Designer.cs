﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Thaliak.Common.Database;

#nullable disable

namespace Thaliak.Common.Database.Migrations
{
    [DbContext(typeof(ThaliakContext))]
    [Migration("20230227004215_RenameTableVersions")]
    partial class RenameTableVersions
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Thaliak.Common.Database.Models.DiscordHookEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DiscordHooks");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivExpansionRepositoryMapping", b =>
                {
                    b.Property<int>("GameRepositoryId")
                        .HasColumnType("integer");

                    b.Property<int>("ExpansionId")
                        .HasColumnType("integer");

                    b.Property<int>("ExpansionRepositoryId")
                        .HasColumnType("integer");

                    b.HasKey("GameRepositoryId", "ExpansionId", "ExpansionRepositoryId");

                    b.HasIndex("ExpansionRepositoryId");

                    b.ToTable("ExpansionRepositoryMappings");

                    b.HasData(
                        new
                        {
                            GameRepositoryId = 2,
                            ExpansionId = 0,
                            ExpansionRepositoryId = 2
                        },
                        new
                        {
                            GameRepositoryId = 2,
                            ExpansionId = 1,
                            ExpansionRepositoryId = 3
                        },
                        new
                        {
                            GameRepositoryId = 2,
                            ExpansionId = 2,
                            ExpansionRepositoryId = 4
                        },
                        new
                        {
                            GameRepositoryId = 2,
                            ExpansionId = 3,
                            ExpansionRepositoryId = 5
                        },
                        new
                        {
                            GameRepositoryId = 2,
                            ExpansionId = 4,
                            ExpansionRepositoryId = 6
                        },
                        new
                        {
                            GameRepositoryId = 7,
                            ExpansionId = 0,
                            ExpansionRepositoryId = 7
                        },
                        new
                        {
                            GameRepositoryId = 7,
                            ExpansionId = 1,
                            ExpansionRepositoryId = 8
                        },
                        new
                        {
                            GameRepositoryId = 7,
                            ExpansionId = 2,
                            ExpansionRepositoryId = 9
                        },
                        new
                        {
                            GameRepositoryId = 7,
                            ExpansionId = 3,
                            ExpansionRepositoryId = 10
                        },
                        new
                        {
                            GameRepositoryId = 7,
                            ExpansionId = 4,
                            ExpansionRepositoryId = 11
                        },
                        new
                        {
                            GameRepositoryId = 12,
                            ExpansionId = 0,
                            ExpansionRepositoryId = 12
                        },
                        new
                        {
                            GameRepositoryId = 12,
                            ExpansionId = 1,
                            ExpansionRepositoryId = 13
                        },
                        new
                        {
                            GameRepositoryId = 12,
                            ExpansionId = 2,
                            ExpansionRepositoryId = 14
                        },
                        new
                        {
                            GameRepositoryId = 12,
                            ExpansionId = 3,
                            ExpansionRepositoryId = 15
                        },
                        new
                        {
                            GameRepositoryId = 12,
                            ExpansionId = 4,
                            ExpansionRepositoryId = 16
                        });
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivFile", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("SHA1")
                        .HasMaxLength(40)
                        .HasColumnType("character(40)")
                        .IsFixedLength();

                    b.Property<DateTime>("LastUsed")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("Size")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("Name", "SHA1");

                    b.HasIndex("LastUsed");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivPatch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("FirstOffered")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("FirstSeen")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long?>("HashBlockSize")
                        .HasColumnType("bigint");

                    b.Property<string>("HashType")
                        .HasColumnType("text");

                    b.Property<string>("Hashes")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastOffered")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastSeen")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LocalStoragePath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RemoteOriginPath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("RepoVersionId")
                        .HasColumnType("integer");

                    b.Property<int>("RepositoryId")
                        .HasColumnType("integer");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("RepoVersionId");

                    b.HasIndex("RepositoryId");

                    b.ToTable("Patches");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivPatchChain", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("FirstOffered")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastOffered")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("PatchId")
                        .HasColumnType("integer");

                    b.Property<int?>("PreviousPatchId")
                        .HasColumnType("integer");

                    b.Property<int>("RepositoryId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PatchId")
                        .IsUnique()
                        .HasFilter("\"PreviousPatchId\" IS NULL");

                    b.HasIndex("PreviousPatchId");

                    b.HasIndex("RepositoryId");

                    b.HasIndex("PatchId", "PreviousPatchId")
                        .IsUnique()
                        .HasFilter("\"PreviousPatchId\" IS NOT NULL");

                    b.ToTable("PatchChains");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivRepository", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ServiceId")
                        .HasColumnType("integer");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.HasIndex("Slug");

                    b.ToTable("Repositories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "FFXIV Global/JP - Retail - Boot - Win32",
                            Name = "ffxivneo/win32/release/boot",
                            ServiceId = 1,
                            Slug = "2b5cbc63"
                        },
                        new
                        {
                            Id = 2,
                            Description = "FFXIV Global/JP - Retail - Base Game - Win32",
                            Name = "ffxivneo/win32/release/game",
                            ServiceId = 1,
                            Slug = "4e9a232b"
                        },
                        new
                        {
                            Id = 3,
                            Description = "FFXIV Global/JP - Retail - ex1 (Heavensward) - Win32",
                            Name = "ffxivneo/win32/release/ex1",
                            ServiceId = 1,
                            Slug = "6b936f08"
                        },
                        new
                        {
                            Id = 4,
                            Description = "FFXIV Global/JP - Retail - ex2 (Stormblood) - Win32",
                            Name = "ffxivneo/win32/release/ex2",
                            ServiceId = 1,
                            Slug = "f29a3eb2"
                        },
                        new
                        {
                            Id = 5,
                            Description = "FFXIV Global/JP - Retail - ex3 (Shadowbringers) - Win32",
                            Name = "ffxivneo/win32/release/ex3",
                            ServiceId = 1,
                            Slug = "859d0e24"
                        },
                        new
                        {
                            Id = 6,
                            Description = "FFXIV Global/JP - Retail - ex4 (Endwalker) - Win32",
                            Name = "ffxivneo/win32/release/ex4",
                            ServiceId = 1,
                            Slug = "1bf99b87"
                        },
                        new
                        {
                            Id = 7,
                            Description = "FFXIV Korea - Retail - Base Game - Win32",
                            Name = "actoz/win32/release_ko/game",
                            ServiceId = 2,
                            Slug = "de199059"
                        },
                        new
                        {
                            Id = 8,
                            Description = "FFXIV Korea - Retail - ex1 (Heavensward) - Win32",
                            Name = "actoz/win32/release_ko/ex1",
                            ServiceId = 2,
                            Slug = "573d8c07"
                        },
                        new
                        {
                            Id = 9,
                            Description = "FFXIV Korea - Retail - ex2 (Stormblood) - Win32",
                            Name = "actoz/win32/release_ko/ex2",
                            ServiceId = 2,
                            Slug = "ce34ddbd"
                        },
                        new
                        {
                            Id = 10,
                            Description = "FFXIV Korea - Retail - ex3 (Shadowbringers) - Win32",
                            Name = "actoz/win32/release_ko/ex3",
                            ServiceId = 2,
                            Slug = "b933ed2b"
                        },
                        new
                        {
                            Id = 11,
                            Description = "FFXIV Korea - Retail - ex4 (Endwalker) - Win32",
                            Name = "actoz/win32/release_ko/ex4",
                            ServiceId = 2,
                            Slug = "27577888"
                        },
                        new
                        {
                            Id = 12,
                            Description = "FFXIV China - Retail - Base Game - Win32",
                            Name = "shanda/win32/release_chs/game",
                            ServiceId = 3,
                            Slug = "c38effbc"
                        },
                        new
                        {
                            Id = 13,
                            Description = "FFXIV China - Retail - ex1 (Heavensward) - Win32",
                            Name = "shanda/win32/release_chs/ex1",
                            ServiceId = 3,
                            Slug = "77420d17"
                        },
                        new
                        {
                            Id = 14,
                            Description = "FFXIV China - Retail - ex2 (Stormblood) - Win32",
                            Name = "shanda/win32/release_chs/ex2",
                            ServiceId = 3,
                            Slug = "ee4b5cad"
                        },
                        new
                        {
                            Id = 15,
                            Description = "FFXIV China - Retail - ex3 (Shadowbringers) - Win32",
                            Name = "shanda/win32/release_chs/ex3",
                            ServiceId = 3,
                            Slug = "994c6c3b"
                        },
                        new
                        {
                            Id = 16,
                            Description = "FFXIV China - Retail - ex4 (Endwalker) - Win32",
                            Name = "shanda/win32/release_chs/ex4",
                            ServiceId = 3,
                            Slug = "0728f998"
                        });
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivRepoVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("RepositoryId")
                        .HasColumnType("integer");

                    b.Property<decimal>("VersionId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("VersionString")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RepositoryId");

                    b.HasIndex("VersionId");

                    b.HasIndex("VersionString");

                    b.ToTable("RepoVersions");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivService", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Icon")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Services");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Icon = "🇺🇳",
                            Name = "FFXIV Global"
                        },
                        new
                        {
                            Id = 2,
                            Icon = "🇰🇷",
                            Name = "FFXIV Korea"
                        },
                        new
                        {
                            Id = 3,
                            Icon = "🇨🇳",
                            Name = "FFXIV China"
                        });
                });

            modelBuilder.Entity("XivAccountXivRepository", b =>
                {
                    b.Property<int>("ApplicableAccountsId")
                        .HasColumnType("integer");

                    b.Property<int>("ApplicableRepositoriesId")
                        .HasColumnType("integer");

                    b.HasKey("ApplicableAccountsId", "ApplicableRepositoriesId");

                    b.HasIndex("ApplicableRepositoriesId");

                    b.ToTable("AccountRepositories", (string)null);
                });

            modelBuilder.Entity("XivFileXivRepoVersion", b =>
                {
                    b.Property<int>("VersionsId")
                        .HasColumnType("integer");

                    b.Property<string>("FilesName")
                        .HasColumnType("text");

                    b.Property<string>("FilesSHA1")
                        .HasColumnType("character(40)");

                    b.HasKey("VersionsId", "FilesName", "FilesSHA1");

                    b.HasIndex("FilesName", "FilesSHA1");

                    b.ToTable("VersionFiles", (string)null);
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivExpansionRepositoryMapping", b =>
                {
                    b.HasOne("Thaliak.Common.Database.Models.XivRepository", "ExpansionRepository")
                        .WithMany()
                        .HasForeignKey("ExpansionRepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Thaliak.Common.Database.Models.XivRepository", "GameRepository")
                        .WithMany()
                        .HasForeignKey("GameRepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ExpansionRepository");

                    b.Navigation("GameRepository");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivPatch", b =>
                {
                    b.HasOne("Thaliak.Common.Database.Models.XivRepoVersion", "RepoVersion")
                        .WithMany("Patches")
                        .HasForeignKey("RepoVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Thaliak.Common.Database.Models.XivRepository", "Repository")
                        .WithMany("Patches")
                        .HasForeignKey("RepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RepoVersion");

                    b.Navigation("Repository");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivPatchChain", b =>
                {
                    b.HasOne("Thaliak.Common.Database.Models.XivPatch", "Patch")
                        .WithMany("PrerequisitePatches")
                        .HasForeignKey("PatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Thaliak.Common.Database.Models.XivPatch", "PreviousPatch")
                        .WithMany("DependentPatches")
                        .HasForeignKey("PreviousPatchId");

                    b.HasOne("Thaliak.Common.Database.Models.XivRepository", "Repository")
                        .WithMany()
                        .HasForeignKey("RepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Patch");

                    b.Navigation("PreviousPatch");

                    b.Navigation("Repository");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivRepository", b =>
                {
                    b.HasOne("Thaliak.Common.Database.Models.XivService", "Service")
                        .WithMany("Repositories")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Service");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivRepoVersion", b =>
                {
                    b.HasOne("Thaliak.Common.Database.Models.XivRepository", "Repository")
                        .WithMany("Versions")
                        .HasForeignKey("RepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Repository");
                });

            modelBuilder.Entity("XivAccountXivRepository", b =>
                {
                    b.HasOne("Thaliak.Common.Database.Models.XivAccount", null)
                        .WithMany()
                        .HasForeignKey("ApplicableAccountsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Thaliak.Common.Database.Models.XivRepository", null)
                        .WithMany()
                        .HasForeignKey("ApplicableRepositoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("XivFileXivRepoVersion", b =>
                {
                    b.HasOne("Thaliak.Common.Database.Models.XivRepoVersion", null)
                        .WithMany()
                        .HasForeignKey("VersionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Thaliak.Common.Database.Models.XivFile", null)
                        .WithMany()
                        .HasForeignKey("FilesName", "FilesSHA1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivPatch", b =>
                {
                    b.Navigation("DependentPatches");

                    b.Navigation("PrerequisitePatches");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivRepository", b =>
                {
                    b.Navigation("Patches");

                    b.Navigation("Versions");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivRepoVersion", b =>
                {
                    b.Navigation("Patches");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivService", b =>
                {
                    b.Navigation("Repositories");
                });
#pragma warning restore 612, 618
        }
    }
}
