namespace NerdCritica.Domain.DTOs.User;

public record UserDTO(Guid Id, string IdentityUserId, string UserName, string Email,
    string ProfileImagePath);
