using Dapper;
using NerdCritica.Domain.DTOs.MappingsDapper;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.Entities.Aggregates;
using NerdCritica.Domain.Repositories.Movies;
using NerdCritica.Infrastructure.Context;


namespace NerdCritica.Infrastructure.Persistence.Movies;

public class MoviePostRepository : IMoviePostRepository
{
    private readonly DapperContext _dapperContext;
    public MoviePostRepository(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task<IEnumerable<MoviePostMapping>> GetMoviePostsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string query = @"
            SELECT mp.MoviePostId, mp.MovieImagePath, mp.MovieBackdropImagePath, mp.MovieTitle, mp.MovieDescription, 
              mp.MovieCategory, mp.Director, mp.ReleaseDate, mp.Runtime, mr.Rating, c.CommentId, c.RatingId,
                c.IdentityUserId, c.Content, cm.CastMemberId, cm.MemberName, cm.CharacterName,
                cm.MemberImagePath, cm.RoleInMovie, cm.RoleType
            FROM 
                MoviePost mp
            LEFT JOIN 
                MovieRating mr ON mp.MoviePostId = mr.MoviePostId
            LEFT JOIN 
                Comment c ON mr.RatingId = c.RatingId
            LEFT JOIN 
                CastMember cm ON mp.MoviePostId = cm.MoviePostId
            ORDER BY cm.RoleType ASC";

        using (var connection = _dapperContext.CreateConnection())
        {
            var moviePostDictionary = new Dictionary<Guid, MoviePostMapping>();

            await connection.QueryAsync<MoviePostMapping, CommentsMapping, CastMemberMapping, MoviePostMapping
                >(query, (moviePostMapping, commentsMapping, castMemberMapping) 
                =>
                {
                    if (!moviePostDictionary.TryGetValue(moviePostMapping.MoviePostId, out var currentMoviePost))
                    {
                        currentMoviePost = moviePostMapping;
                        currentMoviePost.Comments = new List<CommentsMapping>();
                        currentMoviePost.Comments = new List<CommentsMapping>();
                        moviePostDictionary.Add(currentMoviePost.MoviePostId, currentMoviePost);
                    }

                    if (commentsMapping != null && !currentMoviePost.Comments.Any(c => 
                    c.CommentId == commentsMapping.CommentId))
                    {
                        currentMoviePost.Comments.Add(commentsMapping);
                    }

                    if (castMemberMapping != null && !currentMoviePost.Cast.Any(cast =>
                    cast.CastMemberId == castMemberMapping.CastMemberId))
                    {
                        currentMoviePost.Cast.Add(castMemberMapping);
                    }

                    return currentMoviePost;
                },
                splitOn: "CommentId, CastMemberId"
                );

            return moviePostDictionary.Values;
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
        @MovieBackdropImagePath, @MovieDescription, @MovieCategory, @Director, @ReleaseDate, @Runtime);";

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

    public Task<bool> DeleteMoviePostAsync(string moviePostId)
    {
        throw new NotImplementedException();
    }

}
