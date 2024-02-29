

namespace NerdCritica.Domain.DTOs.MappingsDapper;

public class CommentsMapping
{
    public Guid CommentId { get; private set; }
    public Guid RatingId { get; private set; }
    public string IdentityUserId { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
}
