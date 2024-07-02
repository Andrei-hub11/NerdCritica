using NerdCritica.Api.Extensions;
using NerdCritica.Api.Utils.Helper;
using NerdCritica.Domain.Utils.Exceptions;
using Newtonsoft.Json;

namespace NerdCritica.Api.Utils.ExceptionService;

public class ExceptionHandler : IExceptionHandler
{
    private readonly LoggerHelper _logger;
    private readonly ErrorHandlerOptions _options;

    public ExceptionHandler(LoggerHelper logger, ErrorHandlerOptions options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task HandleGeneralException(HttpContext context, Exception ex)
    {

        PathString originalPath = context.Request.Path;
        if (_options.ErrorHandlingPath.HasValue)
        {
            context.Request.Path = _options.ErrorHandlingPath;
        }
        try
        {
            var errorHandlerFeature = new ErrorHandlerFeature()
            {
                Error = ex,
            };

            var responseMessage = JsonConvert.SerializeObject(new
            {
                Message = "Desculpe. Algo deu errado por aqui.",
            });

            context.Features.Set((ErrorHandlerFeature?)errorHandlerFeature);
            context.Response.StatusCode = 500;
            context.Response.Headers.Clear();

            await context.Response.WriteAsync(responseMessage);
            await _options.ErrorHandler(context);
            return;
        }
        catch (Exception ex2)
        {
            _logger.LogError(ExceptionDetailsHelper.GetExceptionDetails(ex2, context, 500));
        }
        finally
        {
            context.Request.Path = originalPath;
        }
    }

    public async Task HandleValidationException(HttpContext context, ValidationException ex)
    {

        PathString originalPath = GetOriginalPath(context);

        try
        {
            string responseMessage;

            if (ex.Errors != null && ex.Errors.Any())
            {
                var responseObject = new
                {
                    ex.Message,
                    ex.Errors
                };

                responseMessage = JsonConvert.SerializeObject(responseObject);
            }
            else if (ex.DetailedErrors.Count > 1)
            {
                var responseObject = new
                {
                    ex.Message,
                    Errors = ex.DetailedErrors
                };

                responseMessage = JsonConvert.SerializeObject(responseObject);
            }
            else
            {
                var responseObject = new
                {
                    ex.Message
                };

                responseMessage = JsonConvert.SerializeObject(responseObject);
            }

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(responseMessage);
            await _options.ErrorHandler(context);
            return;
        }
        catch (Exception ex2)
        {
            _logger.LogError(ExceptionDetailsHelper.GetExceptionDetails(ex2, context, 400));
        }
        finally
        {
            context.Request.Path = originalPath;
        }
    }

    public async Task HandleNotFoundException(HttpContext context, NotFoundException ex)
    {
        PathString originalPath = GetOriginalPath(context);

        try
        {
            var responseMessage = JsonConvert.SerializeObject(new
            {
                ex.Message,
            });


            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(responseMessage);
            await _options.ErrorHandler(context);
            return;
        }
        catch (Exception ex2)
        {
            _logger.LogError(ExceptionDetailsHelper.GetExceptionDetails(ex2, context, 400));
        }
        finally
        {
            context.Request.Path = originalPath;
        }
    }

    public async Task HandleCreateUserException(HttpContext context, CreateUserException ex)
    {
        PathString originalPath = GetOriginalPath(context);

        try
        {
            var responseMessage = JsonConvert.SerializeObject(new
            {
                ex.Message,
            });

            context.Response.StatusCode = StatusCodes.Status409Conflict;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(responseMessage);
            await _options.ErrorHandler(context);
            return;
        }
        catch (Exception ex2)
        {
            _logger.LogError(ExceptionDetailsHelper.GetExceptionDetails(ex2, context, 400));
        }
        finally
        {
            context.Request.Path = originalPath;
        }
    }

    public async Task HandleUnauthorizedUserAccessException(HttpContext context, UnauthorizeUserAccessException ex)
    {
        PathString originalPath = GetOriginalPath(context);

        try
        {
            var responseMessage = JsonConvert.SerializeObject(new
            {
                StatusCode = 401,
                Message = "Você não tem acesso acesso a esse recurso, ou ainda não realizou login"
            });


            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(responseMessage);
            await _options.ErrorHandler(context);
            return;
        }
        catch (Exception ex2)
        {
            _logger.LogError(ExceptionDetailsHelper.GetExceptionDetails(ex2, context, 400));
        }
        finally
        {
            context.Request.Path = originalPath;
        }
    }

    public async Task HandleBadRequestException(HttpContext context, BadRequestException ex)
    {
        PathString originalPath = GetOriginalPath(context);

        try
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            var responseMessage = JsonConvert.SerializeObject(new
            {
                ex.Message,
            });

            await context.Response.WriteAsync(responseMessage);
            await _options.ErrorHandler(context);
            return;
        }
        catch (Exception ex2)
        {
            _logger.LogError(ExceptionDetailsHelper.GetExceptionDetails(ex2, context, 400));
        }
        finally
        {
            context.Request.Path = originalPath;
        }
    }

    private PathString GetOriginalPath(HttpContext context)
    {
        PathString originalPath = context.Request.Path;
        if (_options.ErrorHandlingPath.HasValue)
        {
            context.Request.Path = _options.ErrorHandlingPath;
        }

        return originalPath;
    }
}
