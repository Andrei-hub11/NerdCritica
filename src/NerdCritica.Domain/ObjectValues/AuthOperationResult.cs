

using NerdCritica.Domain.DTOs.User;

namespace NerdCritica.Domain.ObjectValues;

public class AuthOperationResult
{

    public string Token { get; set; } = string.Empty;
    public ProfileUserResponseDTO User { get; set; } = new ProfileUserResponseDTO(Guid.Empty, string.Empty, 
        string.Empty, string.Empty, string.Empty);

    public AuthOperationResult(string token, ProfileUserResponseDTO user)
    {
        Token = token;
        User = user;
    }
}
