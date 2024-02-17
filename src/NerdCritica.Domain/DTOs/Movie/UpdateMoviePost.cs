

namespace NerdCritica.Domain.DTOs.Movie;

public record UpdateMoviePost(string MoviePostImagePath,
    string MoviePostTitle,
    string MoviePostDescription, string Category
    );