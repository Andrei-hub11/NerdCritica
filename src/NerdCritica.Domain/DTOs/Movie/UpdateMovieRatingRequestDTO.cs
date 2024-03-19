

namespace NerdCritica.Domain.DTOs.Movie;

public record UpdateMovieRatingRequestDTO(Guid MoviePostId, string IdentityUserId, decimal Rating,
    UpdateCommentDTO Comment);
