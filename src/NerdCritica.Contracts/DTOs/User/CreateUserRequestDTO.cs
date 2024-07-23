namespace NerdCritica.Contracts.DTOs.User;

public record CreateUserRequestDTO(string UserName, string Email, string Password, 
    List<string> Roles, string ProfileImage);
