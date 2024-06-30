using NerdCritica.Domain.Common;
using NerdCritica.Domain.DTOs.Movie;

namespace NerdCritica.Application.Services.Images;

public interface IImagesService
{
    Task<ProfileImage> GetProfileImageAsync(string profileImage);
    Task<MovieImages> GetMoviePostImagesAsync(string movieImage, string movieBackdrop);
    Task<Dictionary<string, CastImages>> GetCastImagesAsync(List<CastMemberRequestDTO> cast);
    Task<Dictionary<string, CastImages>> GetCastImagesAsync(List<UpdateCastMemberRequestDTO> cast);
}
