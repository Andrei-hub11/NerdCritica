
namespace NerdCritica.Domain.Entities;

public class Comment
{
    public Guid CommentId { get; private set; }
    public Guid RatingId { get; private set; }
    public string IdentityUserId { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;

    private Comment(Guid commentId, Guid ratingId, string identityUserId, string content)
    {
        CommentId = commentId;
        RatingId = ratingId;
        IdentityUserId = identityUserId;
        Content = content;
    }

    public static Comment Create(Guid commentId, Guid ratingId, string userId, string content)
    {
        return new Comment(commentId, ratingId, userId, content);
    }

    public static void Update(Comment comment, string newContent)
    {
        comment.Content = newContent;
    }
}
