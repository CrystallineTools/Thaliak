using Microsoft.AspNetCore.Mvc;
using Thaliak.Api.Data;
using Thaliak.Database;

namespace Thaliak.Api.Controllers;

[ApiController]
[Route("repositories")]
public class RepositoryController : ControllerBase
{
    private readonly ThaliakContext _db;

    public RepositoryController(ThaliakContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult GetRepositories()
    {
        var repos = _db.Repositories.OrderBy(r => r.Id).ToList();
        return Ok(XivRepositoryDto.MapFrom(repos));
    }
}
