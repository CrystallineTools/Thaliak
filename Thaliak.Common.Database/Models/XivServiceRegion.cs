using System.ComponentModel.DataAnnotations.Schema;

namespace Thaliak.Common.Database.Models;

public class XivServiceRegion
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; }

    public string Icon { get; set; }
    
    public List<XivRepository> Repositories { get; set; }
}
