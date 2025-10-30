using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Thaliak.Common.Database.Models;

/// <summary>
/// Represents a single XIV version.
/// Versions normally have one patch file, but can have multiple (i.e. in the case of hist-patch versions, such as
/// version 2017.06.06.0000.0001). Each individual patch file is represented by a XivPatch.
/// </summary>
[Index(nameof(VersionString))]
[Index(nameof(RepositoryId))]
public class XivRepoVersion
{
    public static Regex VersionRegex = new(@".*[DH]?(\d{4}\.\d{2}\.\d{2}\.\d{4}\.\d{4})([a-z])?(?:\.patch)?",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string VersionString { get; set; }

    public int RepositoryId { get; set; }
    public XivRepository Repository { get; set; }

    public List<XivGameVersion> GameVersions { get; set; } = new();
    
    public List<XivPatch> Patches { get; set; } = new();

    public List<XivFile> Files { get; set; } = new();
    
    public List<XivUpgradePath> PrerequisiteVersions { get; set; }

    public List<XivUpgradePath> DependentVersions { get; set; }

    public static string UrlToString(string url)
    {
        var match = VersionRegex.Match(url);
        if (!match.Success) {
            throw new ArgumentException($"Invalid patch URL: {url}");
        }

        return match.Groups[1].ToString();
    }
}
