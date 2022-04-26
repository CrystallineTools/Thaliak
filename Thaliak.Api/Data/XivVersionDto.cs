namespace Thaliak.Api.Data;

public class XivVersionDto
{
    public int Id { get; set; }

    public XivRepositoryDto Repository { get; set; }

    public XivExpansionDto Expansion { get; set; }

    public string Version { get; set; }

    public List<XivPatchDto> Patches { get; set; }
}
