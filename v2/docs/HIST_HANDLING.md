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

We address these issues in Thaliak v2 by modeling patches as a **directed acyclic graph (DAG)**, stored in the
`patch_parent` table and exposed externally through the API.

## DAG Structure

The `patch_parent` table represents edges in the patch graph:

- **current_patch_id** (nullable): The patch you're updating from (NULL = base/root patch)
- **next_patch_id**: The patch to apply next
- **repository_id**: The repository this relationship belongs to

This makes patch chains a fully emergent property in the Thaliak v2 architecture, as opposed to being bolted on as an
afterthought.