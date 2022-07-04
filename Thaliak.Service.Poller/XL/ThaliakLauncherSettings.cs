using XIVLauncher.Common;
using XIVLauncher.Common.Game.Patch.Acquisition;
using XIVLauncher.Common.PlatformAbstractions;

namespace Thaliak.Service.Poller.XL;

public class ThaliakLauncherSettings : ISettings
{
    public string AcceptLanguage => "ja";
    public ClientLanguage? ClientLanguage => XIVLauncher.Common.ClientLanguage.Japanese;
    public bool? KeepPatches => true;
    public DirectoryInfo PatchPath { get; }
    public DirectoryInfo GamePath { get; }
    public AcquisitionMethod? PatchAcquisitionMethod => AcquisitionMethod.NetDownloader;
    public long SpeedLimitBytes => 0;
    public int DalamudInjectionDelayMs => 0;

    public ThaliakLauncherSettings(DirectoryInfo patchPath, DirectoryInfo gamePath)
    {
        PatchPath = patchPath;
        GamePath = gamePath;
    }
}
