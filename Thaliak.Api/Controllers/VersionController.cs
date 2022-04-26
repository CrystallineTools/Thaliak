using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thaliak.Api.Data;
using Thaliak.Database;

namespace Thaliak.Api.Controllers;

[ApiController]
[Route("version")]
public class VersionController : ControllerBase
{
    private readonly ThaliakContext _db;

    private readonly IMapper _map;

    public VersionController(ThaliakContext db, IMapper map)
    {
        _db = db;
        _map = map;
    }

    [HttpGet("{repository}/latest")]
    public IActionResult GetLatestVersions([FromRoute] string repository)
    {
        var repo = _db.Repositories
            .FirstOrDefault(r => r.Id.ToString() == repository || r.Slug == repository);
        if (repo == null)
        {
            return NotFound("repository not found");
        }

        // todo
        var versions = _db.Versions
            .Include(v => v.Patches)
            .Include(v => v.Expansion)
            .Include(v => v.Repository)
            .OrderByDescending(v => v.VersionId)
            .Where(v => v.RepositoryId == repo.Id)
            .ToList();
        
        return Ok(_map.Map<List<XivVersionDto>>(versions));
    }
    [HttpGet("{repository}/{version}")]
    public IActionResult GetVersions([FromRoute] string repository, [FromRoute] string version)
    {
        var repo = _db.Repositories
            .FirstOrDefault(r => r.Id.ToString() == repository || r.Slug == repository);
        if (repo == null)
        {
            return NotFound("repository not found");
        }

        var versions = _db.Versions
            .Include(v => v.Patches)
            .Include(v => v.Expansion)
            .Include(v => v.Repository)
            .Where(v => v.RepositoryId == repo.Id && (v.Id.ToString() == version || v.VersionString == version))
            .ToList();

        return Ok(_map.Map<List<XivVersionDto>>(versions));
    }
}
