namespace NerdCritica.Contracts.DTOs.MappingsDapper;

public class UserMapping
{
    public Guid Id { get; set;}
    public string IdentityUserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ProfileImagePath { get; set; } = string.Empty;
}
