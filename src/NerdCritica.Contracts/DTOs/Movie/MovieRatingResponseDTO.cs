using NerdCritica.Contracts.DTOs.MappingsDapper;

namespace NerdCritica.Contracts.DTOs.Movie;

public record MovieRatingResponseDTO(
    Guid RatingId,
    Guid MoviePostId,
    string IdentityUserId,
    decimal Rating,
    ICollection<CommentsMapping> Comments,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
