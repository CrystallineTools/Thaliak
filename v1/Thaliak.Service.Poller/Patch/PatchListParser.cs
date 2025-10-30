﻿namespace Thaliak.Service.Poller.Patch;

public class PatchListParser
{
    public static PatchListEntry[] Parse(string list)
    {
        var lines = list.Split(
            new[] { "\r\n", "\r", "\n", Environment.NewLine },
            StringSplitOptions.None
        );
            
        var output = new List<PatchListEntry>();

        for (var i = 5; i < lines.Length - 2; i++)
        {
            var fields = lines[i].Split('\t');
            output.Add(new PatchListEntry()
            {
                Length = long.Parse(fields[0]),
                VersionId = fields[4],
                HashType = fields[5],

                HashBlockSize = fields.Length == 9 ? long.Parse(fields[6]) : 0,

                // bootver patchlists don't have a hash field
                Hashes = fields.Length == 9 ? (fields[7].Split(',')) : null,
                Url = fields[fields.Length == 9 ? 8 : 5]
            });
        }
            
        return output.ToArray();
    }
}
