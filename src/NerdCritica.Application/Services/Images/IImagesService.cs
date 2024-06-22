using NerdCritica.Domain.Common;
using NerdCritica.Domain.DTOs.Movie;

namespace NerdCritica.Application.Services.Images;

public interface IImagesService
{
    Task<string> GetPathProfileImageAsync(byte[] image);
    Task<MovieImages> GetPathPostImagesAsync(string movieImage, string movieBackdrop);
    Task<Dictionary<string, CastImages>> GetPathCastImagesAsync(List<CastMemberRequestDTO> cast);
    Task<Dictionary<string, CastImages>> GetPathCastImagesAsync(List<UpdateCastMemberRequestDTO> cast);
}
