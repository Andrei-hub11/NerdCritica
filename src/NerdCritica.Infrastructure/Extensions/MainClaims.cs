using NerdCritica.Domain.Utils.Exceptions;
using System.Security.Claims;

namespace NerdCritica.Infrastructure.Extensions;

internal static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal? principal)
    {
        Claim? userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            throw new UnauthorizeUserAccessException("O contexto do usuário não está disponível");
        }

        return userIdClaim.Value;
    }
}

