using System.ComponentModel.DataAnnotations;

namespace Thaliak.Database.Models;

// http://patch-bootver.ffxiv.com/http/win32/ffxivneo_release_boot/2012.01.01.0000.0000/
public class XivRepository
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }
    public string? Description { get; set; }
    public string RemoteOrigin { get; set; }
    
    public List<XivAccount> ApplicableAccounts { get; set; }
    
    public List<XivVersion> Versions { get; set; }
}
