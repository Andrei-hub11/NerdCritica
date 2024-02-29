
using NerdCritica.Domain.Utils;

namespace NerdCritica.Application.Utils;

public class ResultHandlingService
{
    public static void HandleFailure<TException, T>(Result<T> result, string defaultMessage) where TException : Exception
    {
        if (result.IsFailure && typeof(TException) is not null)
        {
            var errorMessages = result.Errors.Select(error => error.Description).ToList();
            var message = defaultMessage + " " + string.Join(", ", errorMessages);
            throw (TException)Activator.CreateInstance(typeof(TException), message)!;
        }
    }
}
