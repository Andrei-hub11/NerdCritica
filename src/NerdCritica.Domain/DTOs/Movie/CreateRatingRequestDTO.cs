namespace NerdCritica.Domain.DTOs.Movie;

public record CreateRatingRequestDTO(Guid MoviePostId, string IdentityUserId, decimal Rating, 
    CreateCommentDTO Comment);
