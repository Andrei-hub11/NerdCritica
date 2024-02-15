

namespace NerdCritica.Domain.DTOs;

public record UserDTO(Guid Id, string IdentityUserId, string UserName, string Email,
    string ProfileImagePath);
