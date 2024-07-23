using NerdCritica.Contracts.DTOs.MappingsDapper;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.Entities.Aggregates;

namespace NerdCritica.Domain.Repositories.Movies;

public interface IMoviePostRepository
{
    Task<MoviePostMapping?> GetMoviePostByIdAsync(Guid moviePostId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<MoviePostMapping>> GetMoviePostsAsync(CancellationToken cancellationToken);
    Task<MovieRatingMapping?> GetRatingByIdAsync(Guid ratingId,
       CancellationToken cancellationToken);
    Task<CommentLikeMapping?> GetCommentLikeByIdAsync(Guid commentId, string identityUserId,
    CancellationToken cancellationToken);
    Task<Guid> CreateMoviePostAsync(MoviePost moviePost, CancellationToken cancellationToken);
    Task<bool> CreateCastMovieAsync(List<CastMember> castMovie, Guid moviePostId);
    Task<Guid> CreateRatingAsync(MovieRating rating, CancellationToken cancellationToken);
    Task<bool> CreateCommentAsync(Comment comment);
    Task<bool> CreateCommentLikeAsync(Guid commentId, string identityUserId,
        CancellationToken cancellationToken);
    Task<bool> UpdateMoviePostAsync(MoviePost moviePost, Guid moviePostId, CancellationToken cancellationToken);
    Task<bool> UpdateCastMovieAsync(List<CastMember> cast, Guid moviePostId);
    Task<bool> UpdateMovieRatingAsync(MovieRating movieRating, Guid movieRatingId,
        CancellationToken cancellationToken);
    Task<bool> UpdateCommentAsync(Comment comment, Guid movieRatingId);
    Task<bool> DeleteMoviePostAsync(Guid moviePostId, CancellationToken cancellationToken);
    Task<bool> DeleteMovieRatingAsync(Guid movieRatingId, CancellationToken cancellationToken);
    Task<bool> DeleteCommentLikeAsync(Guid commentId, string identityUserId, CancellationToken cancellationToken);
}
