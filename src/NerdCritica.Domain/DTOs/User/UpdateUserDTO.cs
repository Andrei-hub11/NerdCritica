
namespace NerdCritica.Domain.DTOs.User;

public record UpdateUserDTO(string Username, string Email, string Password,
    byte[] ProfileImage);
