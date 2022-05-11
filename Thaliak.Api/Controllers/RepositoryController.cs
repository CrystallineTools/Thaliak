using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Thaliak.Api.Data;
using Thaliak.Database;

namespace Thaliak.Api.Controllers;

[ApiController]
[Route("repositories")]
public class RepositoryController : ControllerBase
{
    private readonly ThaliakContext _db;
    private readonly IMapper _map;

    public RepositoryController(ThaliakContext db, IMapper map)
    {
        _db = db;
        _map = map;
    }

    [HttpGet]
    public IActionResult GetRepositories()
    {
        var repos = _db.Repositories.OrderBy(r => r.Id).ToList();
        return Ok(_map.Map<List<XivRepositoryDto>>(repos));
    }
}
