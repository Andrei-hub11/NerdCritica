namespace NerdCritica.Domain.DTOs.User;

public record FavoriteMovieResponseDTO(Guid MoviePostId, Guid FavoriteMovieId, string MovieImagePath, 
    DateTime CreatedAt);
