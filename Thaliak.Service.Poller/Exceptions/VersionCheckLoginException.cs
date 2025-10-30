using Thaliak.Service.Poller.Patch;

namespace Thaliak.Service.Poller.Exceptions;

public class VersionCheckLoginException : Exception
{
    public LoginState State { get; }

    public VersionCheckLoginException(LoginState state)
        : base()
    {
        State = state;
    }
}
