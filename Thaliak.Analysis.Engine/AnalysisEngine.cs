using Thaliak.Common.Database;
using Thaliak.Common.Database.Models;

namespace Thaliak.Analysis.Engine;

public class AnalysisEngine
{
    private readonly ThaliakContext _context;
    private readonly XivPatch _patch;

    public AnalysisEngine(ThaliakContext context, XivPatch patch)
    {
        _context = context;
        _patch = patch;
    }

    public void Analyse()
    {
        /*
         * Analysis process can be broken down into a few steps, with sub-steps.
         *
         * Step 1: Patch Processing
         *  1. Ensure the patch file exists.
         *  2. Check patch chain for prerequisite patches. If no prerequisite patches, then skip, unless
         *     overridden with an "allow empty patch chain" argument. (This argument is used for base patches.)
         *  3. Check the version of the output directory. Ensure the patch chain is valid.
         *  4. Apply the patch to the output directory.
         *
         * Step 2: Hashing. For each file in the output directory:
         *  1. Calculate SHA1 hash, and insert into VersionFiles with the version ID, filename, and hash.
         *  2. If it doesn't already exist, copy the file to the storage directory, using the SHA1 hash as the name.
         *  3. Create symlink in the version directory.
         *
         * The initial analysis process will analyse:
         * - Root patches (where PreviousPatch is null) and all descendants
         */
    }
}
