namespace Thaliak.Poller.Exceptions;

public class NoValidAccountException : Exception
{
    public NoValidAccountException() : base("No valid account found!") { }
}
