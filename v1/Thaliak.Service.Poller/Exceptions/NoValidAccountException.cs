namespace Thaliak.Service.Poller.Exceptions;

public class NoValidAccountException : Exception
{
    public NoValidAccountException() : base("No valid account found!") { }
}
