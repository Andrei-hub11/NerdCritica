using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NerdCritica.Infrastructure.Extensions;

internal static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        Claim? userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            throw new ApplicationException("User id is unavailable");
        }

        string userIdValue = userIdClaim.Value; // Acessando a propriedade Value para obter o valor do Claim
        return Guid.TryParse(userIdValue, out Guid parsedUserId) ?
            parsedUserId :
            throw new ApplicationException("User id is unavailable");
    }
}

