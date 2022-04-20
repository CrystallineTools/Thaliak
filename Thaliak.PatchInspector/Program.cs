using ZiPatchLib;
using ZiPatchLib.Util;

namespace Thaliak.PatchInspector;

internal class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Input ZiPatch .patch file must be provided");
            Environment.Exit(1);
        }

        var zi = ZiPatchFile.FromFileName(args[0]);
        var sex = new SqexFileStreamStore();
        var config = new ZiPatchConfig(@"D:\ffxiv-test")
        {
            IgnoreMissing = false,
            IgnoreOldMismatch = false,
            Platform = ZiPatchConfig.PlatformId.Win32,
            Store = sex
        };
        
        foreach (var chunk in zi.GetChunks())
        {
            Console.WriteLine(chunk);
            chunk.ApplyChunk(config);
        }
        
        sex.Dispose();
        zi.Dispose();
    }
}
