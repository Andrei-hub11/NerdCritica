using NerdCritica.Domain.DTOs.MappingsDapper;
using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.Entities.Aggregates;

namespace NerdCritica.Domain.Repositories.Movies;

public interface IMoviePostRepository
{
    Task<IEnumerable<MoviePostMapping>> GetMoviePostsAsync(CancellationToken cancellationToken);
    Task<Guid> CreateMoviePostAsync(MoviePost moviePost, CancellationToken cancellationToken);
    Task<bool> CreateCastMovieAsync(List<CastMember> castMovie, Guid moviePostId);
    Task<Guid> CreateRatingAsync(MovieRating rating, CancellationToken cancellationToken);
    Task<bool> CreateCommentAsync(Comment comment);
    Task<bool> UpdateMoviePostAsync(MoviePost moviePost, Guid moviePostId, CancellationToken cancellationToken);
    Task<bool> UpdateCastMovieAsync(List<CastMember> cast, Guid moviePostId);
    Task<bool> DeleteMoviePostAsync(string moviePostId);
}
