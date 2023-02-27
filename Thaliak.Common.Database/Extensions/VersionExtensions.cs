using Microsoft.EntityFrameworkCore;
using Thaliak.Common.Database.Models;

namespace Thaliak.Common.Database.Extensions;

public static class VersionExtensions
{
    public static IQueryable<XivRepoVersion> WithUpgradePaths(this IQueryable<XivRepoVersion> q)
    {
        return q.Include(v => v.PrerequisiteVersions)
            .ThenInclude(c => c.PreviousRepoVersion)
            .Include(v => v.DependentVersions)
            .ThenInclude(c => c.RepoVersion);
        // return q.Include(v => v.Patches)
        //     .ThenInclude(p => p.PrerequisitePatches)
        //     .ThenInclude(c => c.PreviousRepoVersion)
        //     .ThenInclude(p => p.RepoVersion)
        //     .Include(v => v.Patches)
        //     .ThenInclude(p => p.DependentPatches)
        //     .ThenInclude(c => c.RepoVersion)
        //     .ThenInclude(p => p.RepoVersion);
    }
}
