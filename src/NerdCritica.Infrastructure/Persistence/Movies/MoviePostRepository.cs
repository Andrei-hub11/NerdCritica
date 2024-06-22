using Dapper;
using Microsoft.AspNetCore.Identity;
using NerdCritica.Domain.DTOs.MappingsDapper;
using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.Entities.Aggregates;
using NerdCritica.Domain.Repositories.Movies;
using NerdCritica.Infrastructure.Context;
using System.Threading;


namespace NerdCritica.Infrastructure.Persistence.Movies;

public class MoviePostRepository : IMoviePostRepository
{
    private readonly DapperContext _dapperContext;
    public MoviePostRepository(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task<MoviePostMapping?> GetMoviePostByIdAsync(Guid moviePostId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string query = @"
            SELECT mp.MoviePostId, mp.MovieImagePath, mp.MovieBackdropImagePath, mp.MovieTitle, mp.MovieDescription, 
              mp.MovieCategory, mp.Director, mp.ReleaseDate, mp.Runtime, mr.Rating, c.CommentId, c.RatingId,
                c.IdentityUserId, c.Content, cm.CastMemberId, cm.MemberName, cm.CharacterName,
                cm.MemberImagePath, cm.RoleInMovie, cm.RoleType, l.LikeId AS CommentLikeId,
              l.IdentityUserId, l.CommentId
            FROM 
                MoviePost mp
            LEFT JOIN 
                MovieRating mr ON mp.MoviePostId = mr.MoviePostId
            LEFT JOIN 
                Comment c ON mr.RatingId = c.RatingId
            LEFT JOIN 
                CastMember cm ON mp.MoviePostId = cm.MoviePostId
            LEFT JOIN 
                CommentLike l ON c.CommentId = l.CommentId
            ORDER BY 
                cm.RoleType ASC
           ";

        using (var connection = _dapperContext.CreateConnection())
        {
            var moviePostDictionary = new Dictionary<Guid, MoviePostMapping>();

            var moviePosts = await connection.QueryAsync<MoviePostMapping, CommentsMapping, CastMemberMapping, CommentLikeMapping, MoviePostMapping
                >(new CommandDefinition(query,
                cancellationToken: cancellationToken), 
                (moviePostMapping, commentsMapping, castMemberMapping, commentLikeMapping) =>
                {
                    if (!moviePostDictionary.TryGetValue(moviePostMapping.MoviePostId, out var currentMoviePost))
                    {
                        currentMoviePost = moviePostMapping;
                        currentMoviePost.Comments = new List<CommentsMapping>();
                        moviePostDictionary.Add(currentMoviePost.MoviePostId, currentMoviePost);
                    }

                    if (commentsMapping != null && !currentMoviePost.Comments.Any(c =>
                    c.CommentId == commentsMapping.CommentId))
                    {
                        currentMoviePost.Comments.Add(commentsMapping);
                    }

                    if (commentsMapping != null && commentsMapping.CommentsLike != null)
                    {
                        var existingComment = currentMoviePost.Comments.FirstOrDefault(c => c.CommentId == commentsMapping.CommentId);
                        if (existingComment != null)
                        {
                            existingComment.CommentsLike.Add(commentLikeMapping);
                            // Remover duplicatas de CommentLike dentro do CommentsMapping, se necessário
                            existingComment.CommentsLike = existingComment.CommentsLike
                                .GroupBy(like => like.CommentLikeId)
                                .Select(group => group.First())
                                .ToList();
                        }
                    }

                    if (castMemberMapping != null && !currentMoviePost.Cast.Any(cast =>
                    cast.CastMemberId == castMemberMapping.CastMemberId))
                    {
                        currentMoviePost.Cast.Add(castMemberMapping);
                    }

                    return currentMoviePost;
                },
                splitOn: "CommentId, CastMemberId, CommentLikeId"
                );

            return moviePosts.FirstOrDefault(moviePost => moviePost.MoviePostId == moviePostId);

        }
    }

    public async Task<IReadOnlyCollection<MoviePostMapping>> GetMoviePostsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string query = @"
            SELECT mp.MoviePostId, mp.MovieImagePath, mp.MovieBackdropImagePath, mp.MovieTitle, mp.MovieDescription, 
              mp.MovieCategory, mp.Director, mp.ReleaseDate, mp.Runtime, mr.Rating, 
              c.*, cm.CastMemberId, cm.MemberName, cm.CharacterName,
              cm.MemberImagePath, cm.RoleInMovie, cm.RoleType, l.LikeId AS CommentLikeId,
              l.IdentityUserId, l.CommentId
            FROM 
                MoviePost mp
            LEFT JOIN 
                MovieRating mr ON mp.MoviePostId = mr.MoviePostId
            LEFT JOIN 
                Comment c ON mr.RatingId = c.RatingId
            LEFT JOIN 
                CastMember cm ON mp.MoviePostId = cm.MoviePostId
            LEFT JOIN 
                CommentLike l ON c.CommentId = l.CommentId
            ORDER BY cm.RoleType ASC";

        using (var connection = _dapperContext.CreateConnection())
        {
            var moviePostDictionary = new Dictionary<Guid, MoviePostMapping>();

            await connection.QueryAsync<MoviePostMapping, CommentsMapping, CastMemberMapping, CommentLikeMapping, MoviePostMapping
                >(new CommandDefinition(query,
                cancellationToken: cancellationToken),
                (moviePostMapping, commentsMapping, castMemberMapping, commentLikeMapping) =>
                {
                    if (!moviePostDictionary.TryGetValue(moviePostMapping.MoviePostId, out var currentMoviePost))
                    {
                        currentMoviePost = moviePostMapping;
                        currentMoviePost.Comments = new List<CommentsMapping>();
                        moviePostDictionary.Add(currentMoviePost.MoviePostId, currentMoviePost);
                    }

                    if (commentsMapping != null && !currentMoviePost.Comments.Any(c =>
                    c.CommentId == commentsMapping.CommentId))
                    {
                        currentMoviePost.Comments.Add(commentsMapping);
                    }

                    if (commentsMapping != null && commentsMapping.CommentsLike != null)
                    {
                        var existingComment = currentMoviePost.Comments.FirstOrDefault(c => c.CommentId == commentsMapping.CommentId);
                        if (existingComment != null)
                        {
                            existingComment.CommentsLike.Add(commentLikeMapping);
                            // Remover duplicatas de CommentLike dentro do CommentsMapping, se necessário
                            existingComment.CommentsLike = existingComment.CommentsLike
                                .GroupBy(like => like.CommentLikeId)
                                .Select(group => group.First())
                                .ToList();
                        }
                    }

                    if (castMemberMapping != null && !currentMoviePost.Cast.Any(cast =>
                    cast.CastMemberId == castMemberMapping.CastMemberId))
                    {
                        currentMoviePost.Cast.Add(castMemberMapping);
                    }

                    return currentMoviePost;
                },
                splitOn: "CommentId, CastMemberId, CommentLikeId"
                );

            return moviePostDictionary.Values;
        }
    }

    public async Task<MovieRatingMapping?> GetRatingByIdAsync(Guid ratingId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string query = @"SELECT mr.RatingId, mr.IdentityUserId, mr.Rating, 
                 c.*
                FROM MovieRating mr
                LEFT JOIN
                     Comment c ON mr.RatingId = c.RatingId
                WHERE mr.RatingId = @RatingId";

        using (var connection = _dapperContext.CreateConnection())
        {
            var movieRating = (await connection.QueryAsync<MovieRatingMapping, CommentsMapping
                , MovieRatingMapping>(new CommandDefinition(query, new { RatingId = ratingId },
                cancellationToken: cancellationToken), (rating, comment) =>
                {
                    if (comment != null)
                    {
                        rating.Comment = comment;
                    }
                    return rating;
                }, splitOn: "RatingId, CommentId")).FirstOrDefault();

            return movieRating;
        }
    }

    public async Task<CommentLikeMapping?> GetCommentLikeByIdAsync(DeleteLikeRequestDTO deleteLikeRequest,
    CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string query = @"SELECT * FROM CommentLike
                WHERE CommentId = @CommentId AND IdentityUserId = @IdentityUserId";

        using (var connection = _dapperContext.CreateConnection())
        {
            var commentLike = await connection.QueryFirstOrDefaultAsync<CommentLikeMapping>(query, new
            {
                deleteLikeRequest.CommentId,
                deleteLikeRequest.IdentityUserId
            });

            return commentLike;
        }
    }


    public async Task<Guid> CreateMoviePostAsync(MoviePost moviePost, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        int runtimeInSeconds = (int)moviePost.Runtime.TotalSeconds;

        string query = @"INSERT INTO MoviePost (CreatorUserId, MovieTitle, MovieImage, MovieBackdropImage, 
        MovieImagePath, MovieBackdropImagePath, MovieDescription, MovieCategory, Director, ReleaseDate, Runtime) 
        OUTPUT INSERTED.MoviePostId
        VALUES (@CreatorUserId, @MovieTitle, @MovieImage, @MovieBackdropImage, @MovieImagePath, 
        @MovieBackdropImagePath, @MovieDescription, @MovieCategory, @Director, @ReleaseDate, @Runtime)";

        using (var connection = _dapperContext.CreateConnection())
        {
            var moviePostId = await connection.QueryFirstOrDefaultAsync<Guid>(query, new
            {
                moviePost.CreatorUserId,
                moviePost.MovieTitle,
                moviePost.MovieImage,
                moviePost.MovieBackdropImage,
                moviePost.MovieImagePath,
                moviePost.MovieBackdropImagePath,
                moviePost.MovieDescription,
                moviePost.MovieCategory,
                moviePost.Director,
                moviePost.ReleaseDate,
                Runtime = runtimeInSeconds
            }
             );

            return moviePostId;
        }
    }

    public async Task<bool> CreateCastMovieAsync(List<CastMember> castMovie, Guid moviePostId)
    {
        string query = @"INSERT INTO CastMember (MoviePostId, MemberName, CharacterName, MemberImage, 
        MemberImagePath, RoleInMovie, RoleType) VALUES (@MoviePostId, @MemberName, @CharacterName, @MemberImage, 
        @MemberImagePath, @RoleInMovie, @RoleType)";

        using (var connection = _dapperContext.CreateConnection())
        {
            foreach (var item in castMovie)
            {
                // apesar da possibilidade de null, o trigger da tabela vai lidar com isso
                // e definir valores padrões de maneira adequada.
                await connection.QueryAsync(query, new
                {
                    MoviePostId = moviePostId,
                    item.MemberName,
                    CharacterName = item.CharacterName ?? null,
                    item.MemberImage,
                    item.MemberImagePath,
                    item.RoleInMovie,
                    item.RoleType
                });
            }

            return true;
        }

    }

    public async Task<Guid> CreateRatingAsync(MovieRating rating, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string query = @"INSERT INTO MovieRating (MoviePostId, IdentityUserId, Rating) 
        OUTPUT INSERTED.RatingId VALUES (@MoviePostId, @IdentityUserId, @Rating)";

        using (var connection = _dapperContext.CreateConnection())
        {
            var RatingId = await connection.QueryFirstOrDefaultAsync<Guid>(query, new
            {
                rating.MoviePostId,
                rating.IdentityUserId,
                rating.Rating
            });

            return RatingId;
        }
    }

    public async Task<bool> CreateCommentAsync(Comment comment)
    {
        string query = @"INSERT INTO Comment (RatingId, IdentityUserId, Content) 
        VALUES (@RatingId, @IdentityUserId, @Content)";

        using (var connection = _dapperContext.CreateConnection())
        {
            await connection.QueryAsync(query, new
            {
                comment.RatingId,
                comment.IdentityUserId,
                comment.Content
            });

            return true;
        }
    }

    public async Task<bool> CreateCommentLikeAsync(Guid commentId, string identityUserId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string query = @"INSERT INTO CommentLike (CommentId, IdentityUserId) 
        VALUES (@CommentId, @IdentityUserId)";

        using (var connection = _dapperContext.CreateConnection())
        {
            await connection.QueryAsync(query, new
            {
                CommentId = commentId,
                IdentityUserId = identityUserId,
            });

            return true;
        }
    }

    public async Task<bool> UpdateMoviePostAsync(MoviePost moviePost, Guid moviePostId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string query = @"UPDATE MoviePost SET MovieTitle = @MovieTitle, MovieImage = @MovieImage, 
        MovieBackdropImage = @MovieBackdropImage, MovieImagePath = @MovieImagePath,
        MovieBackdropImagePath = @MovieBackdropImagePath, MovieDescription = @MovieDescription,
        MovieCategory = @MovieCategory, Director = @Director, ReleaseDate = @ReleaseDate,
        Runtime = @Runtime WHERE MoviePostId = @MoviePostId";

        int runtimeInSeconds = (int)moviePost.Runtime.TotalSeconds;

        using (var connection = _dapperContext.CreateConnection())
        {
            await connection.QueryAsync(query, new
            {
                moviePost.MovieTitle,
                moviePost.MovieImage,
                moviePost.MovieBackdropImage,
                moviePost.MovieImagePath,
                moviePost.MovieBackdropImagePath,
                moviePost.MovieDescription,
                moviePost.MovieCategory,
                moviePost.Director,
                moviePost.ReleaseDate,
                Runtime = runtimeInSeconds,
                MoviePostId = moviePostId,
            });

            return true;
        }
    }

    public async Task<bool> UpdateCastMovieAsync(List<CastMember> castMovie, Guid moviePostId)
    {
        string query = @"UPDATE CastMember SET MemberName = @MemberName, CharacterName = @CharacterName,
        MemberImage = @MemberImage, MemberImagePath = @MemberImagePath, RoleInMovie = @RoleInMovie, 
        RoleType = @RoleType WHERE MoviePostId = @MoviePostId AND CastMemberId = @CastMemberId";

        using (var connection = _dapperContext.CreateConnection())
        {
            foreach (var item in castMovie)
            {
                // apesar da possibilidade de null, o trigger da tabela vai lidar com isso
                // e definir valores padrões de maneira adequada.
                await connection.QueryAsync(query, new
                {
                    item.MemberName,
                    CharacterName = item.CharacterName ?? null,
                    item.MemberImage,
                    item.MemberImagePath,
                    item.RoleInMovie,
                    item.RoleType,
                    MoviePostId = moviePostId,
                    item.CastMemberId
                });
            }

            return true;
        }
    }

    public async Task<bool> UpdateMovieRatingAsync(MovieRating movieRating, Guid movieRatingId,
        CancellationToken cancellationToken)
    {
        string query = @"UPDATE MovieRating SET Rating = @Rating WHERE RatingId = @RatingId AND 
        IdentityUserId = @IdentityUserId";

        using (var connection = _dapperContext.CreateConnection())
        {
            await connection.QueryAsync(query, new
            {
                movieRating.Rating,
                RatingId = movieRatingId,
                movieRating.IdentityUserId,
            });

            return true;
        }
    }

    public async Task<bool> UpdateCommentAsync(Comment comment, Guid movieRatingId)
    {
        string query = @"UPDATE Comment SET Content = @Content WHERE RatingId = @RatingId AND 
        IdentityUserId = @IdentityUserId";

        using (var connection = _dapperContext.CreateConnection())
        {
            await connection.QueryAsync(query, new
            {
                comment.Content,
                RatingId = movieRatingId,
                comment.IdentityUserId
            });

            return true;
        }
    }

    public async Task<bool> DeleteMoviePostAsync(Guid moviePostId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string query = @"DELETE FROM MoviePost WHERE MoviePostId = @MoviePostId";

        using (var connection = _dapperContext.CreateConnection())
        {
            await connection.QueryAsync(query, new
            {
                MoviePostId = moviePostId,
            });

            return true;
        }
    }

    public async Task<bool> DeleteMovieRatingAsync(Guid movieRatingId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string query = @"DELETE FROM MovieRating WHERE RatingId = @RatingId";

        using (var connection = _dapperContext.CreateConnection())
        {
            await connection.QueryAsync(query, new
            {
                RatingId = movieRatingId,
            });

            return true;
        }
    }

    public async Task<bool> DeleteCommentLikeAsync(DeleteLikeRequestDTO deleteLikeRequest,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string query = @"DELETE FROM CommentLike 
        WHERE CommentId = @CommentId AND IdentityUserId = @IdentityUserId";

        using (var connection = _dapperContext.CreateConnection())
        {
            await connection.QueryAsync(query, new
            {
                deleteLikeRequest.CommentId,
                deleteLikeRequest.IdentityUserId
            });

            return true;
        }
    }
}
