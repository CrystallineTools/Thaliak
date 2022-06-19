using Thaliak.Database.Models;

namespace Thaliak.Api.Data;

public class XivVersionDto
{
    public XivRepositoryDto? Repository { get; set; }

    public string Version { get; set; }

    public List<XivPatchDto?> Patches { get; set; } = new();

    public static XivVersionDto? MapFrom(XivVersion? version)
    {
        if (version == null)
        {
            return null;
        }

        return new XivVersionDto
        {
            Repository = XivRepositoryDto.MapFrom(version.Repository),
            Version = version.VersionString,
            Patches = version.Patches.Select(XivPatchDto.MapFrom).ToList()
        };
    }

    public static List<XivVersionDto?> MapFrom(IEnumerable<XivVersion?> versions)
    {
        return versions.Select(MapFrom).ToList();
    }
}
