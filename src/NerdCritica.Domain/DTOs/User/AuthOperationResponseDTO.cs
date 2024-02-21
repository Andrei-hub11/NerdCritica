namespace NerdCritica.Domain.DTOs.User;

public record AuthOperationResponseDTO(string token, ProfileUserResponseDTO user);
