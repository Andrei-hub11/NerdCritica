namespace NerdCritica.Domain.Common;

public class ProfileImage
{
    public byte[] ProfileImageBytes { get; set; } = Array.Empty<byte>();
    public string ProfileImagePath { get; set; } = string.Empty;
}
