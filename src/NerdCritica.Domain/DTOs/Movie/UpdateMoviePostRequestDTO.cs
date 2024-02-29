namespace NerdCritica.Domain.DTOs.Movie;

public record UpdateMoviePostRequestDTO(
    string MovieImage, 
    string MovieBackdropImage,
    string MovieTitle,
    string MovieDescription, 
    string MovieCategory, 
    string Director,
    List<UpdateCastMemberRequestDTO> Cast,
    TimeSpan Runtime,
    DateTime ReleaseDate
    );