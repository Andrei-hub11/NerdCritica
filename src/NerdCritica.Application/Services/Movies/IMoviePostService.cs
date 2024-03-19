using NerdCritica.Domain.Common;
using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.Entities;

namespace NerdCritica.Application.Services.Movies;

public interface IMoviePostService
{
    Task<IEnumerable<MoviePostResponseDTO>> GetMoviePostsAsync(CancellationToken token);
    Task<bool> CreateMoviePostAsync(CreateMoviePostRequestDTO moviePost, MovieImages postImages,
       Dictionary<string, CastImages> castImagePaths, CancellationToken cancellationToken);
    Task<bool> CreateRatingAsync(CreateRatingRequestDTO rating, CancellationToken cancellationToken);
    Task<bool> UpdateMoviePostAsync(UpdateMoviePostRequestDTO moviePost, MovieImages postImages, 
        Dictionary<string, CastImages> castImagePaths, Guid moviePostId, CancellationToken cancellationToken);
    Task<bool> UpdateMovieRatingAsync(UpdateMovieRatingRequestDTO movieRating, Guid movieRatingId,
        CancellationToken cancellationToken);
    Task<bool> DeleteMoviePostAsync(Guid moviePostId, CancellationToken cancellationToken);
    Task<bool> DeleteMovieRatingAsync(Guid movieRatingId, CancellationToken cancellationToken);
}
