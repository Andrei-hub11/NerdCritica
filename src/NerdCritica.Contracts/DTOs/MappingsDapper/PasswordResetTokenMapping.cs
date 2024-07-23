namespace NerdCritica.Contracts.DTOs.MappingsDapper;

public class PasswordResetTokenMapping
{
    public Guid Id { get; set; }
    public string IdentityUserId { get; set; } = string.Empty;
    public string Token { get; set;} = string.Empty;
    public DateTime ExpirationDate { get; set; }
}
