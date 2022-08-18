using System.ComponentModel.DataAnnotations.Schema;

namespace Thaliak.Common.Database.Models;

public class XivPatchChain
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int RepositoryId { get; set; }
    public XivRepository Repository { get; set; }

    public int PatchId { get; set; }
    public XivPatch Patch { get; set; }

    public int? PreviousPatchId { get; set; }
    public XivPatch? PreviousPatch { get; set; }

    public DateTime? FirstOffered { get; set; }
    public DateTime? LastOffered { get; set; }
    
    public bool IsActive { get; set; }
}
