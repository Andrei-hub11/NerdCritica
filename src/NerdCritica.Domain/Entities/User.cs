

namespace NerdCritica.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string IdentityUserId { get; private set; } = string.Empty;
    public string ProfileImagePath { get; private set; } = string.Empty;
    public byte[] ProfileImage { get; private set; } = new byte[0];
    public DateTime LastAccessDate { get; private set; } = DateTime.UtcNow;
}
