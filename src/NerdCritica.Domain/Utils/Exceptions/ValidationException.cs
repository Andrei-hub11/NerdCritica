namespace NerdCritica.Domain.Utils.Exceptions;

public class ValidationException: Exception
{
    public List<string> Errors { get; } = new List<string>();

    public ValidationException(string message) : base(message)
    {
    }

    public ValidationException(string message, List<string> erros) : base(message)
    {
        Errors = erros;
    }
}
