namespace NerdCritica.Api.Extensions;

public interface IErrorHandlerFeature
{
    Exception Error { get; set; }
}