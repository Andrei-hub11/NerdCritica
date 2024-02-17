using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.Entities;

namespace NerdCritica.Domain.Repositories.Movies;

public interface IMoviePostRepository
{
    Task<IEnumerable<MoviePostDTO>> GetMoviePostsAsync(CancellationToken token);
    Task<bool> UpdateMoviePostAsync(MoviePost moviePost);
    Task<bool> DeleteMoviePostAsync(string moviePostId);
}
