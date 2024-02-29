using NerdCritica.Domain.DTOs.MappingsDapper;

namespace NerdCritica.Domain.DTOs.Movie;

public record MovieRatingResponseDTO(
    Guid RatingId,
    Guid MoviePostId,
    string IdentityUserId,
    decimal Rating,
    ICollection<CommentsMapping> Comments,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
