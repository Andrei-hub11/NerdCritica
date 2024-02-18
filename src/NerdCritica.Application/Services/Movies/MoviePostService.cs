using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.Entities;

namespace NerdCritica.Application.Services.Movies
{
    public class MoviePostService : IMoviePostService
    {
        public Task<bool> DeleteMoviePostAsync(string moviePostId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MoviePostResponseDTO>> GetMoviePostsAsync(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateMoviePostAsync(MoviePost moviePost)
        {
            throw new NotImplementedException();
        }
    }
}
