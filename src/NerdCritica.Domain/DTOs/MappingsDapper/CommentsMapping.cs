

namespace NerdCritica.Domain.DTOs.MappingsDapper;

public class CommentsMapping
{
    public Guid CommentId { get; set; }
    public Guid RatingId { get; set; }
    public string IdentityUserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public ICollection<CommentLikeMapping> CommentsLike { get; set; } = new List<CommentLikeMapping>();
}
