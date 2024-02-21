using NerdCritica.Domain.Utils.Exceptions;

namespace NerdCritica.Api.Utils.ExceptionService;

public interface IExceptionHandler
{
    Task HandleGeneralException(HttpContext context, Exception ex);
    Task HandleValidationException(HttpContext context, ValidationException ex);
    Task HandleNotFoundException (HttpContext context, NotFoundException ex);
    Task HandleBadRequestException (HttpContext context, BadRequestException ex);
    Task HandleUnauthorizedAccessException (HttpContext context, UnauthorizedAccessException ex);
    Task HandleCreateUserException (HttpContext context, CreateUserException ex);
}
