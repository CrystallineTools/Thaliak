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
    [Migration("20230227011028_TidyRelationships")]
    partial class TidyRelationships
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("game_version_repo_versions", b =>
                {
                    b.Property<int>("GameVersionsId")
                        .HasColumnType("integer")
                        .HasColumnName("game_versions_id");

                    b.Property<int>("RepoVersionsId")
                        .HasColumnType("integer")
                        .HasColumnName("repo_versions_id");

                    b.HasKey("GameVersionsId", "RepoVersionsId")
                        .HasName("pk_game_version_repo_versions");

                    b.HasIndex("RepoVersionsId")
                        .HasDatabaseName("ix_game_version_repo_versions_repo_versions_id");

                    b.ToTable("game_version_repo_versions", (string)null);
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.DiscordHookEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("url");

                    b.HasKey("Id")
                        .HasName("pk_discord_hooks");

                    b.ToTable("discord_hooks", (string)null);
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("pk_accounts");

                    b.ToTable("accounts", (string)null);
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivExpansionRepositoryMapping", b =>
                {
                    b.Property<int>("GameRepositoryId")
                        .HasColumnType("integer")
                        .HasColumnName("game_repository_id");

                    b.Property<int>("ExpansionId")
                        .HasColumnType("integer")
                        .HasColumnName("expansion_id");

                    b.Property<int>("ExpansionRepositoryId")
                        .HasColumnType("integer")
                        .HasColumnName("expansion_repository_id");

                    b.HasKey("GameRepositoryId", "ExpansionId", "ExpansionRepositoryId")
                        .HasName("pk_expansion_repository_mappings");

                    b.HasIndex("ExpansionRepositoryId")
                        .HasDatabaseName("ix_expansion_repository_mappings_expansion_repository_id");

                    b.ToTable("expansion_repository_mappings", (string)null);

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
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("SHA1")
                        .HasMaxLength(40)
                        .HasColumnType("character(40)")
                        .HasColumnName("sha1")
                        .IsFixedLength();

                    b.Property<DateTime>("LastUsed")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_used");

                    b.Property<decimal>("Size")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("size");

                    b.HasKey("Name", "SHA1")
                        .HasName("pk_files");

                    b.HasIndex("LastUsed")
                        .HasDatabaseName("ix_files_last_used");

                    b.ToTable("files", (string)null);
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivGameVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("HotfixLevel")
                        .HasColumnType("integer")
                        .HasColumnName("hotfix_level");

                    b.Property<string>("MarketingName")
                        .HasColumnType("text")
                        .HasColumnName("marketing_name");

                    b.Property<string>("PatchInfoUrl")
                        .HasColumnType("text")
                        .HasColumnName("patch_info_url");

                    b.Property<int>("ServiceId")
                        .HasColumnType("integer")
                        .HasColumnName("service_id");

                    b.Property<string>("VersionName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("version_name");

                    b.HasKey("Id")
                        .HasName("pk_game_versions");

                    b.HasIndex("HotfixLevel")
                        .HasDatabaseName("ix_game_versions_hotfix_level");

                    b.HasIndex("ServiceId")
                        .HasDatabaseName("ix_game_versions_service_id");

                    b.HasIndex("VersionName")
                        .HasDatabaseName("ix_game_versions_version_name");

                    b.ToTable("game_versions", (string)null);
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivPatch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("FirstOffered")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("first_offered");

                    b.Property<DateTime?>("FirstSeen")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("first_seen");

                    b.Property<long?>("HashBlockSize")
                        .HasColumnType("bigint")
                        .HasColumnName("hash_block_size");

                    b.Property<string>("HashType")
                        .HasColumnType("text")
                        .HasColumnName("hash_type");

                    b.Property<string>("Hashes")
                        .HasColumnType("text")
                        .HasColumnName("hashes");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<DateTime?>("LastOffered")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_offered");

                    b.Property<DateTime?>("LastSeen")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_seen");

                    b.Property<string>("LocalStoragePath")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("local_storage_path");

                    b.Property<string>("RemoteOriginPath")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("remote_origin_path");

                    b.Property<int>("RepoVersionId")
                        .HasColumnType("integer")
                        .HasColumnName("repo_version_id");

                    b.Property<int>("RepositoryId")
                        .HasColumnType("integer")
                        .HasColumnName("repository_id");

                    b.Property<long>("Size")
                        .HasColumnType("bigint")
                        .HasColumnName("size");

                    b.HasKey("Id")
                        .HasName("pk_patches");

                    b.HasIndex("RepoVersionId")
                        .HasDatabaseName("ix_patches_repo_version_id");

                    b.HasIndex("RepositoryId")
                        .HasDatabaseName("ix_patches_repository_id");

                    b.ToTable("patches", (string)null);
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivRepository", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int>("ServiceId")
                        .HasColumnType("integer")
                        .HasColumnName("service_id");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("slug");

                    b.HasKey("Id")
                        .HasName("pk_repositories");

                    b.HasIndex("ServiceId")
                        .HasDatabaseName("ix_repositories_service_id");

                    b.HasIndex("Slug")
                        .HasDatabaseName("ix_repositories_slug");

                    b.ToTable("repositories", (string)null);

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
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("RepositoryId")
                        .HasColumnType("integer")
                        .HasColumnName("repository_id");

                    b.Property<string>("VersionString")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("version_string");

                    b.HasKey("Id")
                        .HasName("pk_repo_versions");

                    b.HasIndex("RepositoryId")
                        .HasDatabaseName("ix_repo_versions_repository_id");

                    b.HasIndex("VersionString")
                        .HasDatabaseName("ix_repo_versions_version_string");

                    b.ToTable("repo_versions", (string)null);
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivService", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Icon")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("icon");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_services");

                    b.ToTable("services", (string)null);

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

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivUpgradePath", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("FirstOffered")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("first_offered");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<DateTime?>("LastOffered")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_offered");

                    b.Property<int>("PatchId")
                        .HasColumnType("integer")
                        .HasColumnName("patch_id");

                    b.Property<int?>("PreviousPatchId")
                        .HasColumnType("integer")
                        .HasColumnName("previous_patch_id");

                    b.Property<int>("RepositoryId")
                        .HasColumnType("integer")
                        .HasColumnName("repository_id");

                    b.HasKey("Id")
                        .HasName("pk_upgrade_paths");

                    b.HasIndex("PatchId")
                        .IsUnique()
                        .HasDatabaseName("ix_upgrade_paths_patch_id")
                        .HasFilter("\"previous_patch_id\" IS NULL");

                    b.HasIndex("PreviousPatchId")
                        .HasDatabaseName("ix_upgrade_paths_previous_patch_id");

                    b.HasIndex("RepositoryId")
                        .HasDatabaseName("ix_upgrade_paths_repository_id");

                    b.HasIndex("PatchId", "PreviousPatchId")
                        .IsUnique()
                        .HasDatabaseName("ix_upgrade_paths_patch_id_previous_patch_id")
                        .HasFilter("\"previous_patch_id\" IS NOT NULL");

                    b.ToTable("upgrade_paths", (string)null);
                });

            modelBuilder.Entity("XivAccountXivRepository", b =>
                {
                    b.Property<int>("ApplicableAccountsId")
                        .HasColumnType("integer")
                        .HasColumnName("applicable_accounts_id");

                    b.Property<int>("ApplicableRepositoriesId")
                        .HasColumnType("integer")
                        .HasColumnName("applicable_repositories_id");

                    b.HasKey("ApplicableAccountsId", "ApplicableRepositoriesId")
                        .HasName("pk_account_repositories");

                    b.HasIndex("ApplicableRepositoriesId")
                        .HasDatabaseName("ix_account_repositories_applicable_repositories_id");

                    b.ToTable("account_repositories", (string)null);
                });

            modelBuilder.Entity("XivFileXivRepoVersion", b =>
                {
                    b.Property<int>("VersionsId")
                        .HasColumnType("integer")
                        .HasColumnName("versions_id");

                    b.Property<string>("FilesName")
                        .HasColumnType("text")
                        .HasColumnName("files_name");

                    b.Property<string>("FilesSHA1")
                        .HasColumnType("character(40)")
                        .HasColumnName("files_sha1");

                    b.HasKey("VersionsId", "FilesName", "FilesSHA1")
                        .HasName("pk_version_files");

                    b.HasIndex("FilesName", "FilesSHA1")
                        .HasDatabaseName("ix_version_files_files_name_files_sha1");

                    b.ToTable("version_files", (string)null);
                });

            modelBuilder.Entity("game_version_repo_versions", b =>
                {
                    b.HasOne("Thaliak.Common.Database.Models.XivGameVersion", null)
                        .WithMany()
                        .HasForeignKey("GameVersionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_game_version_repo_versions_game_versions_game_versions_id");

                    b.HasOne("Thaliak.Common.Database.Models.XivRepoVersion", null)
                        .WithMany()
                        .HasForeignKey("RepoVersionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_game_version_repo_versions_repo_versions_repo_versions_id");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivExpansionRepositoryMapping", b =>
                {
                    b.HasOne("Thaliak.Common.Database.Models.XivRepository", "ExpansionRepository")
                        .WithMany()
                        .HasForeignKey("ExpansionRepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_expansion_repository_mappings_repositories_expansion_reposi");

                    b.HasOne("Thaliak.Common.Database.Models.XivRepository", "GameRepository")
                        .WithMany()
                        .HasForeignKey("GameRepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_expansion_repository_mappings_repositories_game_repository_");

                    b.Navigation("ExpansionRepository");

                    b.Navigation("GameRepository");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivGameVersion", b =>
                {
                    b.HasOne("Thaliak.Common.Database.Models.XivService", "Service")
                        .WithMany("GameVersions")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_game_versions_services_service_id");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivPatch", b =>
                {
                    b.HasOne("Thaliak.Common.Database.Models.XivRepoVersion", "RepoVersion")
                        .WithMany("Patches")
                        .HasForeignKey("RepoVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_patches_repo_versions_repo_version_id");

                    b.HasOne("Thaliak.Common.Database.Models.XivRepository", "Repository")
                        .WithMany("Patches")
                        .HasForeignKey("RepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_patches_repositories_repository_id");

                    b.Navigation("RepoVersion");

                    b.Navigation("Repository");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivRepository", b =>
                {
                    b.HasOne("Thaliak.Common.Database.Models.XivService", "Service")
                        .WithMany("Repositories")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_repositories_services_service_id");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivRepoVersion", b =>
                {
                    b.HasOne("Thaliak.Common.Database.Models.XivRepository", "Repository")
                        .WithMany("RepoVersions")
                        .HasForeignKey("RepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_repo_versions_repositories_repository_id");

                    b.Navigation("Repository");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivUpgradePath", b =>
                {
                    b.HasOne("Thaliak.Common.Database.Models.XivPatch", "Patch")
                        .WithMany("PrerequisitePatches")
                        .HasForeignKey("PatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_upgrade_paths_patches_patch_id");

                    b.HasOne("Thaliak.Common.Database.Models.XivPatch", "PreviousPatch")
                        .WithMany("DependentPatches")
                        .HasForeignKey("PreviousPatchId")
                        .HasConstraintName("fk_upgrade_paths_patches_previous_patch_id");

                    b.HasOne("Thaliak.Common.Database.Models.XivRepository", "Repository")
                        .WithMany()
                        .HasForeignKey("RepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_upgrade_paths_repositories_repository_id");

                    b.Navigation("Patch");

                    b.Navigation("PreviousPatch");

                    b.Navigation("Repository");
                });

            modelBuilder.Entity("XivAccountXivRepository", b =>
                {
                    b.HasOne("Thaliak.Common.Database.Models.XivAccount", null)
                        .WithMany()
                        .HasForeignKey("ApplicableAccountsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_account_repositories_accounts_applicable_accounts_id");

                    b.HasOne("Thaliak.Common.Database.Models.XivRepository", null)
                        .WithMany()
                        .HasForeignKey("ApplicableRepositoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_account_repositories_repositories_applicable_repositories_id");
                });

            modelBuilder.Entity("XivFileXivRepoVersion", b =>
                {
                    b.HasOne("Thaliak.Common.Database.Models.XivRepoVersion", null)
                        .WithMany()
                        .HasForeignKey("VersionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_version_files_repo_versions_versions_id");

                    b.HasOne("Thaliak.Common.Database.Models.XivFile", null)
                        .WithMany()
                        .HasForeignKey("FilesName", "FilesSHA1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_version_files_files_files_temp_id");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivPatch", b =>
                {
                    b.Navigation("DependentPatches");

                    b.Navigation("PrerequisitePatches");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivRepository", b =>
                {
                    b.Navigation("Patches");

                    b.Navigation("RepoVersions");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivRepoVersion", b =>
                {
                    b.Navigation("Patches");
                });

            modelBuilder.Entity("Thaliak.Common.Database.Models.XivService", b =>
                {
                    b.Navigation("GameVersions");

                    b.Navigation("Repositories");
                });
#pragma warning restore 612, 618
        }
    }
}
