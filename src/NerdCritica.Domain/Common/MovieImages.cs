namespace NerdCritica.Domain.Common;

public class MovieImages
{
    public string MovieImagePath { get; set; } = string.Empty;
    public string MovieBackdropPath { get; set; } = string.Empty;
    public byte[] MovieImageBytes { get; set; } = new byte[0];
    public byte[] MovieBackdropBytes { get; set; } = new byte[0];
}