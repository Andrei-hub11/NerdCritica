namespace NerdCritica.Domain.Utils.Exceptions;

public class UnauthorizeUserAccessException : Exception
{
    public UnauthorizeUserAccessException(string message) : base(message)
    { }
}
