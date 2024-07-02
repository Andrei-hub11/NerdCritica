namespace NerdCritica.Domain.DTOs.User;

public record VerifyResetPasswordRequestDTO(string Token, string Email);
