namespace NerdCritica.Domain.DTOs.Movie;

public record UpdateLikeRequestDTO(Guid RatingId, string IdentityUserId);
