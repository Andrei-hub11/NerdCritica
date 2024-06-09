using NerdCritica.Api.Utils.ExceptionService;
using NerdCritica.Api.Utils.Helper;
using NerdCritica.Domain.Utils.Exceptions;

namespace NerdCritica.Api.Extensions;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ErrorHandlerOptions _options;
    private readonly LoggerHelper _logger;
    private readonly ExceptionHandler _exceptionHandler;

    public ErrorHandlerMiddleware(RequestDelegate next,
                                  LoggerHelper logger,
                                  ErrorHandlerOptions options, ExceptionHandler exceptionHandler)
    {
        _next = next;
        _options = options;
        _logger = logger;
        _exceptionHandler = exceptionHandler;
        if (_options.ErrorHandler == null)
        {
            _options.ErrorHandler = async context =>
            {
                if (!context.Response.HasStarted)
                    await _next(context);
            };
        }
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
                
            _logger.LogError(ExceptionDetailsHelper.GetExceptionDetails(ex, context, 400));
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("A resposta já foi iniciada, " +
       "o manipulador de erros não será executado.");
                throw;
            }
            await  _exceptionHandler.HandleValidationException(context, ex);
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(ExceptionDetailsHelper.GetExceptionDetails(ex, context, 404));
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("A resposta já foi iniciada, " +
          "o manipulador de erros não será executado.");
                throw;
            }
            await _exceptionHandler.HandleNotFoundException(context, ex);
        }
        catch (BadRequestException ex)
        {

            _logger.LogError(ExceptionDetailsHelper.GetExceptionDetails(ex, context, 400));
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("A resposta já foi iniciada, " +
      "o manipulador de erros não será executado.");
                throw;
            }
            await _exceptionHandler.HandleBadRequestException(context, ex);
        } catch (CreateUserException ex)
        {
            _logger.LogError(ExceptionDetailsHelper.GetExceptionDetails(ex, context, 409));
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("A resposta já foi iniciada, " +
      "o manipulador de erros não será executado.");
                throw;
            }
            await _exceptionHandler.HandleCreateUserException(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ExceptionDetailsHelper.GetExceptionDetails(ex, context, 500));

            if (context.Response.HasStarted)
            {
                _logger.LogWarning("A resposta já foi iniciada, " +
       "o manipulador de erros não será executado.");
                throw;
            }
            await _exceptionHandler.HandleGeneralException(context, ex);
        }
    }
}