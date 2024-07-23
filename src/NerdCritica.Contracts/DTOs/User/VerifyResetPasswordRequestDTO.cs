namespace NerdCritica.Contracts.DTOs.User;

public record VerifyResetPasswordRequestDTO(string Token, string Email);
