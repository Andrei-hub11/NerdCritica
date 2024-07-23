namespace NerdCritica.Contracts.DTOs.User;

public record UpdateUserRequestDTO(string Username, string Email,
    string ProfileImage);
