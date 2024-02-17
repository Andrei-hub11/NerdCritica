
namespace NerdCritica.Domain.DTOs.User;

public record CreateUserDTO(string Username, string Email, string Password, 
    byte[] ProfileImage);
