namespace NerdCritica.Contracts.DTOs.MappingsDapper;

public class CommentLikeMapping
{
    public Guid CommentLikeId { get; set; }
    public Guid CommentId { get; set; }
    public string IdentityUserId { get; set;} = string.Empty;
}
