using System.ComponentModel.DataAnnotations.Schema;

namespace Thaliak.Database.Models;

public class DiscordHookEntry
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Url { get; set; }

    public string? Name { get; set; }
}
