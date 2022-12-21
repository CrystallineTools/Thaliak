using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Thaliak.Common.Database.Models.Legacy;

/// <summary>
/// Represents a single XIV version.
/// Versions normally have one patch file, but can have multiple (i.e. in the case of hist-patch versions, such as
/// version 2017.06.06.0000.0001). Each individual patch file is represented by a XivPatch.
/// </summary>
[Index(nameof(VersionId))]
[Index(nameof(VersionString))]
[Index(nameof(Repository))]
public class LegacyXivVersion
{
    public static Regex VersionRegex = new(@".*[DH]?(\d{4}\.\d{2}\.\d{2}\.\d{4}\.\d{4})([a-z])?(?:\.patch)?",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Used for sorting.
    /// </summary>
    /// 2017.06.06.0000.0001c
    /// 2017060600000001030
    /// 2017_06_06_0000_0001_03_0
    /// year_month_day_patch_rev_part_reserved
    /// 2021.03.01.0000.0000 = 2021030100000000000
    /// 18446744073709551615
    public ulong VersionId { get; set; }

    public string VersionString { get; set; }

    public int RepositoryId { get; set; }
    public XivRepository Repository { get; set; }

    public List<LegacyXivPatch> Patches { get; set; } = new();

    public List<XivFile> Files { get; set; } = new();

    public static ulong StringToId(string verString)
    {
        var match = VersionRegex.Match(verString);
        if (!match.Success) {
            throw new ArgumentException($"Invalid version string: {verString}");
        }

        var trimmed = new StringBuilder(match.Groups[1].Value.Replace(".", ""));
        if (match.Groups[2].Success) {
            // convert from letter to position in alphabet
            var letter = match.Groups[2].Value.ToLowerInvariant();
            var pos = letter[0] - 'a';
            trimmed.Append(pos.ToString("D2"));
        } else {
            trimmed.Append("00");
        }

        // reserved, but always zero in current implementation
        trimmed.Append('0');

        return ulong.Parse(trimmed.ToString());
    }

    public static string UrlToString(string url)
    {
        var match = VersionRegex.Match(url);
        if (!match.Success) {
            throw new ArgumentException($"Invalid patch URL: {url}");
        }

        return match.Groups[1].ToString();
    }
}
