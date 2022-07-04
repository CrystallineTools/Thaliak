using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thaliak.Common.Database;
using Thaliak.Common.Database.Extensions;
using Thaliak.Common.Database.Models;
using Thaliak.Service.Api.Data;
using Thaliak.Service.Api.Util;

namespace Thaliak.Service.Api.Controllers;

[ApiController]
[Route("versions")]
public class VersionController : ControllerBase
{
    private readonly ThaliakContext _db;

    public VersionController(ThaliakContext db)
    {
        _db = db;
    }

    [HttpGet("{repository}/latest")]
    public IActionResult GetLatestVersions([FromRoute] string repository)
    {
        IQueryable<XivVersion> baseQuery;
        if (repository.ToLower() == "all")
        {
            baseQuery = _db.Versions;
        }
        else
        {
            var repo = _db.Repositories
                .FirstOrDefault(r => r.Id.ToString() == repository || r.Slug == repository);
            if (repo == null)
            {
                return NotFound("repository not found");
            }

            baseQuery = _db.Versions
                .Where(v => v.RepositoryId == repo.Id);
        }

        var versions = baseQuery
            .WithPatchChains()
            .Include(v => v.Repository)
            .GroupBy(v => v.RepositoryId)
            .Select(g => g.OrderByDescending(v => v.VersionId).First())
            .ToList();

        return Ok(XivVersionDto.MapFrom(versions));
    }

    [HttpGet("{repository}")]
    public IActionResult GetVersions([FromRoute] string repository)
    {
        IQueryable<XivVersion> baseQuery;
        if (repository.ToLower() == "all")
        {
            baseQuery = _db.Versions;
        }
        else
        {
            var repo = _db.Repositories
                .FirstOrDefault(r => r.Id.ToString() == repository || r.Slug == repository);
            if (repo == null)
            {
                return NotFound(new ErrorResponse("repository not found"));
            }

            baseQuery = _db.Versions
                .Where(v => v.RepositoryId == repo.Id);
        }

        var versions = baseQuery
            .Include(v => v.Patches)
            .WithPatchChains()
            .Include(v => v.Repository)
            .OrderBy(v => v.VersionId)
            .ToList();

        return Ok(XivVersionDto.MapFrom(versions));
    }

    [HttpGet("{repository}/{version}")]
    public IActionResult GetVersion([FromRoute] string repository, [FromRoute] string version)
    {
        var repo = _db.Repositories
            .FirstOrDefault(r => r.Id.ToString() == repository || r.Slug == repository);
        if (repo == null)
        {
            return NotFound(new ErrorResponse("repository not found"));
        }

        var versions = _db.Versions
            .Include(v => v.Patches)
            .WithPatchChains()
            .Include(v => v.Repository)
            .Where(v => v.RepositoryId == repo.Id && (v.Id.ToString() == version || v.VersionString == version))
            .OrderBy(v => v.VersionId)
            .ToList();

        if (versions.Count == 1)
        {
            return Ok(XivVersionDto.MapFrom(versions.First()));
        }
        else if (versions.Count > 1)
        {
            return Conflict(
                new ErrorResponse("multiple versions in the same repository found, please report this bug"));
        }
        else
        {
            return NotFound(new ErrorResponse("version not found"));
        }
    }
}
