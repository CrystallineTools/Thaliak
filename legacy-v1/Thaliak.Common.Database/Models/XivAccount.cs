using System.ComponentModel.DataAnnotations.Schema;

namespace Thaliak.Common.Database.Models;

/// <summary>
/// Stores information about a FFXIV service account, used to poll the servers for new versions.
/// This model is never exposed by the API.
/// </summary>
public class XivAccount
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// The account username.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// The account password, stored in plaintext (so it can be used for authentication).
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// A list of repositories that this account is applicable to.
    /// </summary>
    public List<XivRepository> ApplicableRepositories { get; set; }
}
