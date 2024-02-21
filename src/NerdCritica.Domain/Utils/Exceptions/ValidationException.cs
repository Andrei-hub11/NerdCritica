namespace NerdCritica.Domain.Utils.Exceptions;

public class ValidationException: Exception
{
    public object Erros { get; }

    public ValidationException(string message, object erros) : base(message)
    {
        Erros = erros;
    }

}
