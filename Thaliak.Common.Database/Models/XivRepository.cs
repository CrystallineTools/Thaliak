using System.ComponentModel.DataAnnotations;
using System.IO.Hashing;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Thaliak.Common.Database.Models;

[Index(nameof(Slug))]
[Index(nameof(ServiceRegion))]
public class XivRepository
{
    [Key]
    public int Id { get; set; }
    
    public XivServiceRegion ServiceRegion { get; set; }
    public int ServiceRegionId { get; set; }

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

    public List<XivVersion> Versions { get; set; }

    public List<XivPatch> Patches { get; set; }

    private string? _slug;
}
