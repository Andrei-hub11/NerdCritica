namespace NerdCritica.Contracts.DTOs.Movie;
public record CommentLikeResponseDTO(
    Guid CommentLikeId, 
    Guid CommentId, 
    string IdentityUserId
    );

