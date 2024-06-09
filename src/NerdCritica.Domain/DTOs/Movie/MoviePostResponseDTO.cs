namespace NerdCritica.Domain.DTOs.Movie;

public record MoviePostResponseDTO(
    Guid MoviePostId,
    string MovieImagePath,
    string MovieBackdropImagePath,
    string MovieTitle,
    string MovieDescription,
    decimal Rating,
    ICollection<CommentsResponseDTO> Comments,
    string MovieCategory,
    string Director,
    DateTime ReleaseDate, 
    TimeSpan Runtime,
    ICollection<CastMemberResponseDTO> Cast
);