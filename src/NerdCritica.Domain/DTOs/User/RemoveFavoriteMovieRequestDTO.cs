namespace NerdCritica.Domain.DTOs.User;

public record RemoveFavoriteMovieRequestDTO(Guid FavoriteMovieId, string IdentityUserId);