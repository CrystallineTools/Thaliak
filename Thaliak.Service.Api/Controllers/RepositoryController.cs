using Microsoft.AspNetCore.Mvc;
using Thaliak.Common.Database;
using Thaliak.Service.Api.Data;

namespace Thaliak.Service.Api.Controllers;

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
