namespace NerdCritica.Domain.Common;

public class CastImages
{
    public string CastMemberImagePath { get; set; } = string.Empty;
    public byte[] CastMemberImageBytes { get; set; } = new byte[0];

    private CastImages(string castMemberImagePath, byte[] castMemberImageBytes)
    {
        CastMemberImagePath = castMemberImagePath;
        CastMemberImageBytes = castMemberImageBytes;
    }

    public static CastImages Create(string castMemberImagePath, byte[] castMemberImageBytes)
    {
       return new CastImages(castMemberImagePath, castMemberImageBytes);
    }
}
