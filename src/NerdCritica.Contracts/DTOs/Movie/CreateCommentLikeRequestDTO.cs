namespace NerdCritica.Contracts.DTOs.Movie;

public record CreateCommentLikeRequestDTO(Guid RatingId, Guid CommentId, string IdentityUserId,
     string CommentAuthorId);
