namespace NerdCritica.Domain.DTOs.User;

public record AddFavoriteMovieRequestDTO(string IdentityUserId, Guid MoviePostId);
