# HIST Handling in Thaliak v2

The release of FFXIV Dawntrail in mid-2024 illustrated several issues in Thaliak's handling of HIST patches. Some of
these issues include:

1. Deltas between HIST bases are not tracked, since Thaliak always pretends to have no game install
    - Thaliak knows about H2017 and H2024, but D2024 (in this document, specifically referring to the delta patch
      between H2017 and H2024)
    - This is bad both for many consumers (i.e. [Boilmaster](https://github.com/ackwell/boilmaster)), and also bad for
      game preservation
2. HIST patch ordering is often incorrect
    - Square Enix *has* a system of denoting multiple parts to patches, as they would for HIST patches that are going to
      exceed a size limit for a single ZiPatch file, but confusingly they actually just append an alphabetical letter to
      the patch version and call it a day
3. There's no clear delineation of what patches belongs to each HIST base
    - Technically, an interested client could work around this by obtaining all patches between two H-prefixed patches (
      i.e. H2017 inclusive to H2024 exclusive), but this may not be the most reliable, and certainly isn't as
      straightforward as it should be
4. There's no way to find the initial patch of a HIST chain
    - v1 currently handles this with the patch chain system by having the previous patch be NULL for the first HIST
      patch; however, this is fragile and obviously does not work when there's multiple HIST bases tracked

We plan to address these issues in Thaliak v2 through the inclusion of a new **patch base** primitive, stored in the
database and exposed externally through the API.

Each patch base entry will contain:

- **ID**: referenced in each patch that uses this base
- **Repository ID**: the repository that this patch base belongs to
    - It's theoretically possible that SE could rollup patches into a new HIST base in one repo without doing so in
      another, so specifying patch bases on a per-repository basis will account for this possibility
- **Base Version**: the name of the first HIST patch in the series, without the appended alphanumeric part (i.e.
  H2024.05.31.0000.0000)
- **First Patch ID**: ID of the first patch in the series (i.e. H2024.05.31.0000.0000a)
- **Previous Base ID**: ID of the previous base, if there is a known delta (i.e. H2017.06.06.0000.0001a)
- **Delta Patch ID**: ID of the delta patch to migrate from the previous base to this base (i.e. D2024.05.31.0000.0000)
- **Is Active**: Convenience boolean that indicates if this is the currently active patch base.