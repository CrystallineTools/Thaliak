using AutoMapper;
using Thaliak.Database.Models;

namespace Thaliak.Api.Data;

public class ThaliakMapperProfile : Profile
{
    public ThaliakMapperProfile()
    {
        CreateMap<XivExpansion, XivExpansionDto>();

        CreateMap<XivPatch, XivPatchDto>();

        CreateMap<XivRepository, XivRepositoryDto>();

        CreateMap<XivVersion, XivVersionDto>()
            .ForMember(v => v.Version, o => o.MapFrom(v => v.VersionString));
    }
}
