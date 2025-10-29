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
    
    // private void RemoteCallHandler(PatcherIpcEnvelope envelope)
    // {
    //     switch (envelope.OpCode)
    //     {
    //         case PatcherIpcOpCode.Bye:
    //             Task.Run(() =>
    //             {
    //                 Thread.Sleep(3000);
    //                 IsDone = true;
    //             });
    //             break;
    //
    //         case PatcherIpcOpCode.StartInstall:
    //
    //             var installData = (PatcherIpcStartInstall)envelope.Data;
    //             
    //             break;
    //
    //         case PatcherIpcOpCode.Finish:
    //             var path = (DirectoryInfo)envelope.Data;
    //
    //             try
    //             {
    //                 VerToBck(path);
    //                 Log.Information("VerToBck done");
    //             }
    //             catch (Exception ex)
    //             {
    //                 Log.Error(ex, "VerToBck failed");
    //                 this.rpc.SendMessage(new PatcherIpcEnvelope
    //                 {
    //                     OpCode = PatcherIpcOpCode.InstallFailed
    //                 });
    //             }
    //
    //             break;
    //     }
    // }

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

    private static void VerToBck(DirectoryInfo gamePath)
    {
        Thread.Sleep(500);

        foreach (var repository in Enum.GetValues(typeof(Repository)).Cast<Repository>())
        {
            // Overwrite the old BCK with the new game version
            var ver = repository.GetVer(gamePath);

            try
            {
                repository.SetVer(gamePath, ver, true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[PATCHER] Could not copy to BCK");

                if (ver != Constants.BASE_GAME_VERSION)
                    throw;
            }
        }
    }
}