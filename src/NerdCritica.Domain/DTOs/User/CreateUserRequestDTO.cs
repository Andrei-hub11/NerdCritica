
namespace NerdCritica.Domain.DTOs.User;

public record CreateUserRequestDTO(string Username, string Email, string Password, 
    byte[] ProfileImage);
