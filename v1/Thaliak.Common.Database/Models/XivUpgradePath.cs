using System.ComponentModel.DataAnnotations.Schema;

namespace Thaliak.Common.Database.Models;

public class XivUpgradePath
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int RepositoryId { get; set; }
    public XivRepository Repository { get; set; }

    public int RepoVersionId { get; set; }
    public XivRepoVersion RepoVersion { get; set; }

    public int? PreviousRepoVersionId { get; set; }
    public XivRepoVersion? PreviousRepoVersion { get; set; }

    public DateTime? FirstOffered { get; set; }
    public DateTime? LastOffered { get; set; }
    
    public bool IsActive { get; set; }
}
