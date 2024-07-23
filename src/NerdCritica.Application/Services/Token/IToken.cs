using NerdCritica.Contracts.DTOs.MappingsDapper;

namespace NerdCritica.Application.Services.Token;

public interface ITokenService
{
    string GeneratePasswordResetToken(UserMapping user);
    bool ValidatePasswordResetToken(string token);
    string GenerateJwtToken(UserMapping user, IEnumerable<string> roles);
}
