namespace NerdCritica.Domain.DTOs.User;

public record AuthOperationResponseDTO(string Token, ProfileUserResponseDTO User, string Role);
