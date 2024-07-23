using NerdCritica.Contracts.DTOs.MappingsDapper;
using NerdCritica.Contracts.DTOs.Movie;

namespace NerdCritica.Application.DTOMappers;

public static class MappingExtensions
{
    public static MoviePostResponseDTO ToDTO(this MoviePostMapping movie)
    {
        return new MoviePostResponseDTO(
             MoviePostId: movie.MoviePostId,
             MovieImagePath: movie.MovieImagePath,
             MovieBackdropImagePath: movie.MovieBackdropImagePath,
             MovieTitle: movie.MovieTitle,
             MovieDescription: movie.MovieDescription,
             Rating: movie.Rating,
             Comments: movie.Comments.Select(comment => new CommentsResponseDTO(
             CommentId: comment.CommentId,
             RatingId: comment.RatingId,
             IdentityUserId: comment.IdentityUserId,
             Content: comment.Content,
            commentsLike: comment.CommentsLike.Select(commentLike => new CommentLikeResponseDTO(
                CommentLikeId: commentLike.CommentLikeId,
                CommentId: commentLike.CommentId,
                IdentityUserId: commentLike.IdentityUserId
                )).ToList()
              )).ToList(),
              MovieCategory: movie.MovieCategory,
              Director: movie.Director,
              Runtime: TimeSpan.FromSeconds(movie.Runtime),
              ReleaseDate: movie.ReleaseDate,
              Cast: movie.Cast.Select(cast => new CastMemberResponseDTO(
              CastMemberId: cast.CastMemberId,
              MemberName: cast.MemberName,
              CharacterName: cast.CharacterName,
              MemberImagePath: cast.MemberImagePath,
              RoleInMovie: cast.RoleInMovie,
              RoleType: cast.RoleType)).ToList()
             );
    }

    public static IReadOnlyCollection<MoviePostResponseDTO> ToDTO(this IReadOnlyCollection<MoviePostMapping> movies)
    {
        return movies.Select(movie => movie.ToDTO()).ToList();
    }
}
