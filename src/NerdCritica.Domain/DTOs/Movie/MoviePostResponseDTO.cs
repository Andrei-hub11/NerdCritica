using NerdCritica.Domain.Entities;

namespace NerdCritica.Domain.DTOs.Movie;

public record MoviePostResponseDTO(
    Guid MoviePostId,
    string MoviePostImagePath,
    string MoviePostTitle,
    string MoviePostDescription,
    decimal Rating,
    ICollection<Comment> Comments,
    string Category,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

