using NerdCritica.Domain.DTOs;
using NerdCritica.Domain.Entities;

namespace NerdCritica.Application.Services.Movies
{
    public interface IMoviePostService
    {
        Task<IEnumerable<MoviePostDTO>> GetMoviePostsAsync(CancellationToken token);
        Task<bool> UpdateMoviePostAsync(MoviePost moviePost);
        Task<bool> DeleteMoviePostAsync(string moviePostId);
    }
}
