namespace NerdCritica.Contracts.DTOs.Movie;

public record CreateRatingRequestDTO(Guid MoviePostId, string IdentityUserId, decimal Rating, 
    CreateCommentDTO Comment);
