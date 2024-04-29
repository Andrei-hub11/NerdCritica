namespace NerdCritica.Domain.DTOs.Movie;

public record CommentLikeResponseDTO(Guid CommentLikeId, Guid CommentId, string IdentityUserId,
    bool LikedByCurrentUser);

