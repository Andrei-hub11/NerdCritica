using Serilog;

namespace NerdCritica.Api.Extensions;

public static class SerilogConfigExtensions
{
    public static void AddSerilogSettings(this IServiceCollection services, ConfigurationManager configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog();
        });
    }
}

