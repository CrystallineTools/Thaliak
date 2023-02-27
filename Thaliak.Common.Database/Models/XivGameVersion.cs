using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Thaliak.Common.Database.Models;

/// <summary>
/// Represents a single logical version of the game.
/// A game version is a combination of a service (e.g. FFXIV Global) and a patch (e.g. 5.5).
/// Each game version has one or more versions (i.e. game: 2021.08.17.0000.0000, ex1: 2021.08.17.0000.0000, etc.)
/// </summary>
[Index(nameof(ServiceId))]
[Index(nameof(VersionName))]
[Index(nameof(HotfixLevel))]
public class XivGameVersion
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// The service that this game version belongs to.
    /// </summary>
    public int ServiceId { get; set; }
    public XivService Service { get; set; }

    /// <summary>
    /// The public version name, i.e. "6.25". This should match SE's version number.
    /// </summary>
    public string VersionName { get; set; }

    /// <summary>
    /// The hotfix level. Unlike the version name, this is not officially tracked by SE.
    /// This starts at 0 for every new version name, and is incremented for every hotfix.
    /// Hotfixes are generally when "FINAL FANTASY XIV Updated" is posted on Lodestone (or similar).
    /// All hotfixes (both server-only and server + client) are tracked with a hotfix level increment.
    /// </summary>
    public int HotfixLevel { get; set; }

    /// <summary>
    /// The version's public marketing name, i.e. "Buried Memory" (global patch 6.2), or
    /// the expansion name (i.e. "Endwalker" for patch 6.0).
    /// The marketing name is in English for the global service, and in the local language for the other services.
    ///
    /// Can be empty if the version has no marketing name.
    /// </summary>
    public string? MarketingName { get; set; }

    /// <summary>
    /// The URL to the patch's official announcement.
    /// For most patches, this will probably be the URL to the patch notes on the Lodestone (or similar).
    /// For hotfix patches, this may be the URL to the "FINAL FANTASY XIV Updated" post, similar, or empty.
    /// </summary>
    public string? PatchInfoUrl { get; set; }

    /// <summary>
    /// A collection of repo versions that are part of this game version.
    /// </summary>
    public List<XivRepoVersion> RepoVersions { get; set; } = new();
}
