using Microsoft.AspNetCore.Identity;

namespace NerdCritica.Infrastructure.Persistence.Application;

public interface IToken
{
    string GenerateJwtToken(IdentityUser user, IEnumerable<string> roles);
}