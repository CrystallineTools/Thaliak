using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Thaliak.Database.Models;

public class XivExpansion
{
    public static Regex ExpansionRegex = new Regex(@"(?:https?:\/\/.*\/)?(game|boot)\/(?:ex(\d)|\w+)\/(.*)");

    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string Abbreviation { get; set; }

    public static int GetExpansionId(string patchName)
    {
        var match = ExpansionRegex.Match(patchName);
        if (!match.Success)
        {
            return 0;
        }

        var expansionId = match.Groups[2].Value;
        if (string.IsNullOrEmpty(expansionId))
        {
            return 0;
        }

        return int.Parse(expansionId);
    }
}
