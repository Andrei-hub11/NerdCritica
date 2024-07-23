using NerdCritica.Contracts.DTOs.Movie;

namespace NerdCritica.Application.Services.Movies;

public interface IMoviePostService
{
    Task<MoviePostResponseDTO> GetMoviePostAsync(Guid moviePostId, CancellationToken token);
    Task<IReadOnlyCollection<MoviePostResponseDTO>> GetMoviePostsAsync(CancellationToken token);
    Task<bool> CreateMoviePostAsync(CreateMoviePostRequestDTO moviePost, CancellationToken cancellationToken);
    Task<bool> CreateRatingAsync(CreateRatingRequestDTO rating, CancellationToken cancellationToken);
    Task<bool> CreateCommentLikeAsync(CreateCommentLikeRequestDTO commentLike, CancellationToken cancellationToken);
    Task<bool> UpdateMoviePostAsync(UpdateMoviePostRequestDTO moviePost, Guid moviePostId, CancellationToken cancellationToken);
    Task<bool> UpdateMovieRatingAsync(UpdateMovieRatingRequestDTO movieRating, Guid movieRatingId,
        CancellationToken cancellationToken);
    Task<bool> DeleteMoviePostAsync(Guid moviePostId, CancellationToken cancellationToken);
    Task<bool> DeleteMovieRatingAsync(Guid movieRatingId, CancellationToken cancellationToken);
    Task<bool> DeleteCommentLikeAsync(DeleteLikeRequestDTO deleteLikeRequest, CancellationToken cancellationToken);
}
