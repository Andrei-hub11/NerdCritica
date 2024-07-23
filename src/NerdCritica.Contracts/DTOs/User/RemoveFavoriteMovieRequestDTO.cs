namespace NerdCritica.Contracts.DTOs.User;

public record RemoveFavoriteMovieRequestDTO(Guid FavoriteMovieId, string IdentityUserId);