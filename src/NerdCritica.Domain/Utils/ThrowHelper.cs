

using NerdCritica.Domain.Utils.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace NerdCritica.Domain.Utils;

public class ThrowHelper
{
    public static void ThrowNotFoundExceptionIfNull<T>([NotNull] T? value, string message = "")
    {
        _ = value ?? throw new NotFoundException(message);
    }
 }
