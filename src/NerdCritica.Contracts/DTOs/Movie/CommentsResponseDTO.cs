namespace NerdCritica.Contracts.DTOs.Movie;

public record CommentsResponseDTO(
    Guid CommentId,
    Guid RatingId,
    string IdentityUserId,
    string Content,
    ICollection<CommentLikeResponseDTO> commentsLike
    );