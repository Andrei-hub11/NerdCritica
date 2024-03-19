namespace NerdCritica.Api.Extensions;

public class ErrorHandlerOptions
{
    public PathString ErrorHandlingPath { get;  set; }
    public Func<HttpContext, Task> ErrorHandler { get; set; }


}