using System.Text.RegularExpressions;

namespace Thaliak.Database.Models;

public class XivExpansionRepositoryMapping
{
    public static Regex ExpansionRegex = new(@"(?:https?:\/\/.*\/)?(game|boot)\/(?:ex(\d)|\w+)\/(.*)");

    public int GameRepositoryId { get; set; }
    public XivRepository GameRepository { get; set; }
    
    public int ExpansionId { get; set; }
    
    public int ExpansionRepositoryId { get; set; }
    public XivRepository ExpansionRepository { get; set; }
    
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
