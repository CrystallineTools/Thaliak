using System.ComponentModel.DataAnnotations;
using System.IO.Hashing;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Thaliak.Common.Database.Models;

[Index(nameof(Slug))]
[Index(nameof(Service))]
public class XivRepository
{
    [Key]
    public int Id { get; set; }
    
    public XivService Service { get; set; }
    public int ServiceId { get; set; }

    public string Name { get; set; }
    public string? Description { get; set; }

    public string Slug
    {
        get
        {
            if (_slug != null) {
                return _slug;
            }

            var bytes = Encoding.ASCII.GetBytes(Name);
            var output = Crc32.Hash(bytes);
            Array.Reverse(output);
            _slug = Convert.ToHexString(output).ToLowerInvariant();

            return _slug;
        }
        private set => _slug = value;
    }

    public List<XivAccount> ApplicableAccounts { get; set; }

    public List<XivRepoVersion> RepoVersions { get; set; }
    
    public List<XivUpgradePath> UpgradePaths { get; set; }

    private string? _slug;
}
