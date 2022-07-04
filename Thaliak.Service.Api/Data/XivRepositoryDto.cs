using Thaliak.Common.Database.Models;

namespace Thaliak.Service.Api.Data;

public class XivRepositoryDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Slug { get; set; }

    public static XivRepositoryDto? MapFrom(XivRepository? repository)
    {
        if (repository == null)
        {
            return null;
        }

        return new XivRepositoryDto
        {
            Name = repository.Name,
            Description = repository.Description,
            Slug = repository.Slug
        };
    }

    public static List<XivRepositoryDto?> MapFrom(IEnumerable<XivRepository?> repositories)
    {
        return repositories.Select(MapFrom).ToList();
    }
}
