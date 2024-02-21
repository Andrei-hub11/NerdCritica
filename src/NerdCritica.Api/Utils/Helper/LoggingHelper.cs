namespace NerdCritica.Api.Utils.Helper;
using Microsoft.Extensions.Logging;
using System;

public class LoggerHelper
{
    private readonly ILogger<LoggerHelper> _logger;

    public LoggerHelper(ILogger<LoggerHelper> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void LogWarning(string message)
    {
        if (_logger.IsEnabled(LogLevel.Warning))
        {
            _logger.LogInformation(message);
        }
    }

    public void LogInformation(string message)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation(message);
        }
    }

    public void LogError(string message)
    {
        if (_logger.IsEnabled(LogLevel.Error))
        {
            _logger.LogError(message);
        }
    }
}