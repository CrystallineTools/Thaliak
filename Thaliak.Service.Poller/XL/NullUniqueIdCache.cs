using XIVLauncher.Common.PlatformAbstractions;

namespace Thaliak.Service.Poller.XL;

internal class NullUniqueIdCache : IUniqueIdCache
{
    public bool HasValidCache(string name) => false;

    public void Add(string name, string uid, int region, int maxExpansion) { }

    public bool TryGet(string userName, out IUniqueIdCache.CachedUid cached)
    {
        cached = default;
        return false;
    }

    public void Reset() { }
}
