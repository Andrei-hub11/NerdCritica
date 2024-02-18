

namespace NerdCritica.Domain.DTOs.Movie;

public record UpdateMoviePostRequestDTO(string MoviePostImagePath,
    string MoviePostTitle,
    string MoviePostDescription, string Category
    );