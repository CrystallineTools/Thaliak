namespace Thaliak.Service.Poller.Patch;

public enum LoginState
{
    Unknown,
    Ok,
    NeedsPatchGame,
    NeedsPatchBoot,
    NoService,
    NoTerms,
    NoLogin
}
