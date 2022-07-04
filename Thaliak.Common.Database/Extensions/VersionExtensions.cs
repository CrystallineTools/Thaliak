using Microsoft.EntityFrameworkCore;
using Thaliak.Common.Database.Models;

namespace Thaliak.Common.Database.Extensions;

public static class VersionExtensions
{
    public static IQueryable<XivVersion> WithPatchChains(this IQueryable<XivVersion> q)
    {
        return q.Include(v => v.Patches)
            .ThenInclude(p => p.PrerequisitePatches)
            .ThenInclude(c => c.PreviousPatch)
            .ThenInclude(p => p.Version)
            .Include(v => v.Patches)
            .ThenInclude(p => p.DependentPatches)
            .ThenInclude(c => c.Patch)
            .ThenInclude(p => p.Version);
    }
}
