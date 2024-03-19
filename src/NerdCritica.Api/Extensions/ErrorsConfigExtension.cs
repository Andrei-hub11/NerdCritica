using NerdCritica.Api.Utils.ExceptionService;
using NerdCritica.Api.Utils.Helper;

namespace NerdCritica.Api.Extensions;

public static class ErrorsConfigExtension
{

    public static void AddErrorsConfig(this IServiceCollection services)
    {
        services.AddSingleton<ErrorHandlerOptions>();
        services.AddSingleton<ExceptionHandler>();
        services.AddSingleton<ExceptionDetailsHelper>();
    }
 }
