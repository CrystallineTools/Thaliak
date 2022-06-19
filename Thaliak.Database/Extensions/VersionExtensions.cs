using Microsoft.EntityFrameworkCore;
using Thaliak.Database.Models;

namespace Thaliak.Database.Extensions;

public static class VersionExtensions
{
    public static IQueryable<XivVersion> WithPatchChains(this IQueryable<XivVersion> q)
    {
        return q.Include(v => v.Patches)
            .ThenInclude(p => p.PrerequisitePatches)
            .Include(v => v.Patches)
            .ThenInclude(p => p.DependentPatches);
    }
}
