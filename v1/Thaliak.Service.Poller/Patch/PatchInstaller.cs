using System.Collections.Concurrent;
using Serilog;
using ZiPatchLib;
using ZiPatchLib.Util;

namespace Thaliak.Service.Poller.Patch;

public class PatchInstaller
{
    private readonly ConcurrentQueue<PatchInstallData> queuedInstalls = new();
    private readonly DirectoryInfo gameDirectory;

    public PatchInstaller(DirectoryInfo gameDirectory)
    {
        this.gameDirectory = gameDirectory;
    }

    public void QueueInstall(PatchInstallData installData)
    {
        queuedInstalls.Enqueue(installData);
    }

    public async Task InstallAllQueuedPatchesAsync(CancellationToken cancellationToken = default)
    {
        Log.Information("[PATCHER] Starting batch patch installation");

        while (queuedInstalls.TryDequeue(out var installData))
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await InstallPatchAsync(installData, cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[PATCHER] Patch install failed for {PatchFile}", installData.PatchFile.FullName);
                throw;
            }
        }

        Log.Information("[PATCHER] Batch patch installation complete");
    }

    private async Task InstallPatchAsync(PatchInstallData installData, CancellationToken cancellationToken)
    {
        // Ensure that subdirs exist
        if (!gameDirectory.Exists) gameDirectory.Create();

        gameDirectory.CreateSubdirectory("game");
        gameDirectory.CreateSubdirectory("boot");

        // Run the synchronous patch installation on a background thread to avoid blocking
        await Task.Run(() =>
        {
            InstallPatch(installData.PatchFile.FullName,
                Path.Combine(gameDirectory.FullName,
                    installData.Repo == Repository.Boot ? "boot" : "game"));
        }, cancellationToken);

        try
        {
            installData.Repo.SetVer(gameDirectory, installData.VersionId);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Could not set ver file");
            throw;
        }
    }

    private static void InstallPatch(string patchPath, string gamePath)
    {
        Log.Information("[PATCHER] Installing {0} to {1}", patchPath, gamePath);

        using var patchFile = ZiPatchFile.FromFileName(patchPath);

        using (var store = new SqexFileStreamStore())
        {
            var config = new ZiPatchConfig(gamePath) { Store = store };

            foreach (var chunk in patchFile.GetChunks())
                chunk.ApplyChunk(config);
        }

        Log.Information("[PATCHER] Patch {0} installed", patchPath);
    }
}