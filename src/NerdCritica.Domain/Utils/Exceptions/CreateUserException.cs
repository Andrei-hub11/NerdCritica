

namespace NerdCritica.Domain.Utils.Exceptions;

public class CreateUserException : Exception
{
    public CreateUserException()
    {
    }

    public CreateUserException(string message)
        : base(message)
    {
    }

    public CreateUserException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}