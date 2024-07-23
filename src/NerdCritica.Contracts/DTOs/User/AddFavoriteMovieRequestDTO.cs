namespace NerdCritica.Contracts.DTOs.User;

public record AddFavoriteMovieRequestDTO(string IdentityUserId, Guid MoviePostId);
