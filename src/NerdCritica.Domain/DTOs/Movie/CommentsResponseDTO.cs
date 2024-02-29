namespace NerdCritica.Domain.DTOs.Movie;

public record CommentsResponseDTO (Guid CommentId, Guid RatingId, string IdentityUserId, string Content);