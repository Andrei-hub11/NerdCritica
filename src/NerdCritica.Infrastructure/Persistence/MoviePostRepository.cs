using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.Repositories.Movies;
using NerdCritica.Infrastructure.Context;

namespace NerdCritica.Infrastructure.Persistence;

public class MoviePostRepository : IMoviePostRepository
{
    private readonly DapperContext _dapperContext;
    public MoviePostRepository (DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public Task<IEnumerable<MoviePostDTO>> GetMoviePostsAsync(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateMoviePostAsync(MoviePost moviePost)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteMoviePostAsync(string moviePostId)
    {
        throw new NotImplementedException();
    }
}
