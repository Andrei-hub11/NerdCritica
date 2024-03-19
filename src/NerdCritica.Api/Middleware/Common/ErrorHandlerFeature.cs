namespace NerdCritica.Api.Extensions;

public class ErrorHandlerFeature
{
    public Exception Error { get; set; }
    public ErrorHandlerFeature()
    {
        Error = new Exception();
    }

}