using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thaliak.Common.Database;

namespace Thaliak.Service.Api.Controllers;

[ApiController]
[Route("patch-trees")]
public class PatchTreeController : ControllerBase
{
    private readonly ThaliakContext _db;

    public PatchTreeController(ThaliakContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Get()
    {
        // patch id -> patch tree branch
        var branches = new Dictionary<int, PatchTreeBranch>();
        var roots = new List<PatchTreeBranch>();

        var dbChains = _db.PatchChains
            .Include(c => c.Patch)
            .ThenInclude(p => p.Version)
            .Include(c => c.PreviousPatch)
            .ThenInclude(p => p.Version)
            .ToList();

        // build the branches and the roots
        foreach (var chain in dbChains) {
            var branch = new PatchTreeBranch
            {
                Version = chain.Patch.Version.VersionString
            };

            if (chain.PreviousPatchId == null) {
                roots.Add(branch);
            }

            branches.Add(chain.PatchId, branch);
        }

        // link the branches
        foreach (var chain in dbChains) {
            if (chain.PreviousPatchId == null) {
                continue;
            }

            var branch = branches[chain.PatchId];
            branches[chain.PreviousPatchId.Value].Children.Add(branch);
        }

        // deliver the roots
        return Ok(Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(roots, new JsonSerializerOptions
        {
            MaxDepth = 696969
        })));
    }

    private record PatchTreeBranch
    {
        public string Version { get; init; }
        public List<PatchTreeBranch> Children { get; } = new();
    }
}
