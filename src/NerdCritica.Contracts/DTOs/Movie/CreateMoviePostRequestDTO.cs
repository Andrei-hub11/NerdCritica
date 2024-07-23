namespace NerdCritica.Contracts.DTOs.Movie;

public record CreateMoviePostRequestDTO(string CreatorUserId, string MovieImage, string MovieBackdropImage,
    string MovieTitle, string MovieDescription, string MovieCategory, string Director,
    DateTime ReleaseDate, TimeSpan Runtime, List<CastMemberRequestDTO> Cast);
