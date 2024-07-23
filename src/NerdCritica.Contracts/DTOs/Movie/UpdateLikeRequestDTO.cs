namespace NerdCritica.Contracts.DTOs.Movie;

public record UpdateLikeRequestDTO(Guid RatingId, string IdentityUserId);
