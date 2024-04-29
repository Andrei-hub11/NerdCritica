using AutoMapper;
using NerdCritica.Domain.Common;
using NerdCritica.Domain.Contracts;
using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.Entities.Aggregates;
using NerdCritica.Domain.Repositories.Movies;
using NerdCritica.Domain.Utils;
using NerdCritica.Domain.Utils.Exceptions;
using System.Threading;

namespace NerdCritica.Application.Services.Movies;


public class MoviePostService : IMoviePostService
{

private readonly IMoviePostRepository _moviePostRepository;
private readonly IMapper _mapper;
private readonly IUserContext _userContext;

    public MoviePostService(IMoviePostRepository moviePostRepository, IMapper mapper, IUserContext userContext)
    {
        _moviePostRepository = moviePostRepository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<MoviePostResponseDTO> GetMoviePostAsync(Guid moviePostId, CancellationToken cancellationToken)
    {
        try
        {
            var movie = await _moviePostRepository.GetMoviePostByIdAsync(moviePostId, cancellationToken);

            _ = movie ?? throw new NotFoundException($"A postagem de filme com o id {moviePostId} não foi encontrada.");

            var movieMapping = new MoviePostResponseDTO(
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
             LikeCount: comment.CommentsLike.Count,
             LikedByCurrentUser: comment.CommentsLike.Any(l => l.IdentityUserId
             == _userContext.UserId.ToString())
              )).ToList(),
              MovieCategory: movie.MovieCategory,
              Director: movie.Director,
              ReleaseDate: movie.ReleaseDate,
              Runtime: FormatHelper.FormatRuntime(movie.Runtime),
              Cast: movie.Cast.Select(cast => new CastMemberResponseDTO(
              CastMemberId: cast.CastMemberId,
              MemberName: cast.MemberName,
              CharacterName: cast.CharacterName,
              MemberImagePath: cast.MemberImagePath,
              RoleInMovie: cast.RoleInMovie,
              RoleType: cast.RoleType)).ToList()
             );

            return movieMapping;
        } catch
        {
            throw;
        }
    }

    public async Task<ICollection<MoviePostResponseDTO>> GetMoviePostsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

       var movies = await _moviePostRepository.GetMoviePostsAsync(cancellationToken);

        var moviesMapping = new List<MoviePostResponseDTO>();

        foreach (var movie in movies)
        {
            var movieMapping = new MoviePostResponseDTO(
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
                LikeCount: comment.CommentsLike.Count,
                LikedByCurrentUser: comment.CommentsLike.Any(l => l.IdentityUserId
                == _userContext.UserId.ToString())
                 )).ToList(),
                 MovieCategory: movie.MovieCategory,
                 Director: movie.Director,
                 ReleaseDate: movie.ReleaseDate,
                 Runtime: FormatHelper.FormatRuntime(movie.Runtime),
                 Cast: movie.Cast.Select(cast => new CastMemberResponseDTO(
                 CastMemberId: cast.CastMemberId,
                 MemberName: cast.MemberName,
                 CharacterName: cast.CharacterName,
                 MemberImagePath: cast.MemberImagePath,
                 RoleInMovie: cast.RoleInMovie,
                 RoleType: cast.RoleType )).ToList()
                );

            moviesMapping.Add(movieMapping);
        }

        return moviesMapping;
    }

    public async Task<bool> CreateMoviePostAsync(CreateMoviePostRequestDTO moviePost, MovieImages postImages,
        Dictionary<string, CastImages> castImagePaths, CancellationToken cancellationToken)
    {
        try
        {
            var newMoviePost = MoviePost.Create(moviePost.CreatorUserId, postImages.MovieImagePath, 
                postImages.MovieBackdropPath, postImages.MovieImageBytes, postImages.MovieBackdropBytes, 
                moviePost.MovieTitle, moviePost.MovieDescription, moviePost.MovieCategory, moviePost.Director, 
                moviePost.ReleaseDate, moviePost.Runtime);

            if (newMoviePost.IsFailure)
            {
                var errorMessages = newMoviePost.Errors.Select(error => error.Description).ToList();
                throw new ValidationException("Os campos de criação do post do filme não foram preenchidos corretamente.",
                    errorMessages);
            }

            Guid postId = await _moviePostRepository.CreateMoviePostAsync(newMoviePost.Value,
                cancellationToken);

            List<CastMember> cast = CastMemberHelper.GetCast(moviePost.Cast, castImagePaths);

            var isSuccess = await _moviePostRepository.CreateCastMovieAsync(cast, postId);

            return isSuccess;
        } catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> CreateRatingAsync(CreateRatingRequestDTO rating, 
        CancellationToken cancellationToken)
    {
        try
        {
            var newRating = MovieRating.Create(rating.MoviePostId, rating.IdentityUserId, rating.Rating);

            if (newRating.IsFailure)
            {
                var errorMessages = newRating.Errors.Select(error => error.Description).ToList();
                throw new ValidationException("Os campos de criação da avaliação do filme não foram preenchidos " +
                    "corretamente.",
                    errorMessages);
            }


            var ratingId = await _moviePostRepository.CreateRatingAsync(newRating.Value, cancellationToken);

            var newComment = Comment.Create(ratingId, rating.IdentityUserId, rating.Comment.Content);

            if (newComment.IsFailure)
            {
                var errorMessages = newComment.Errors.Select(error => error.Description).ToList();
                throw new ValidationException("Os campos de criação do comentário do filme não foram preenchidos " +
                    "corretamente.",
                    errorMessages);
            }

            bool isCreate = await _moviePostRepository.CreateCommentAsync(newComment.Value);

            return isCreate;

        } catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> CreateCommentLikeAsync(CreateCommentLikeRequestDTO commentLikeRequest,
    CancellationToken cancellationToken)
    {
        try
        {
            var movieRatingExist = await _moviePostRepository.GetRatingByIdAsync(commentLikeRequest.RatingId,
                cancellationToken);

            ThrowHelper.ThrowNotFoundExceptionIfNull(movieRatingExist, $"A avalição com o id {commentLikeRequest.RatingId} não foi encontrada.");

            if (movieRatingExist.Comment.CommentId != commentLikeRequest.CommentId)
            {
                throw new BadRequestException($"O id de comentário {commentLikeRequest.CommentId} " +
                    $"não está presente na avaliação de id {commentLikeRequest.RatingId}.");
            }

            bool isUpdated = await _moviePostRepository.CreateCommentLikeAsync(commentLikeRequest.CommentId,
                commentLikeRequest.IdentityUserId, cancellationToken);

            return isUpdated;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> UpdateMoviePostAsync(UpdateMoviePostRequestDTO moviePost, MovieImages postImages, 
        Dictionary<string, CastImages> castImagePaths, Guid moviePostId, CancellationToken cancellationToken)
    {
        try
        {
            var moviePostExist = await _moviePostRepository.GetMoviePostByIdAsync(moviePostId, 
                cancellationToken);

            ThrowHelper.ThrowNotFoundExceptionIfNull(moviePostExist, $"A postagem de filme com o id {moviePostId} não foi encontrada.");

            var updatedMoviePost = MoviePost.From(postImages.MovieImagePath, postImages.MovieBackdropPath,
                postImages.MovieImageBytes, postImages.MovieBackdropBytes, moviePost.MovieTitle, 
                moviePost.MovieDescription, moviePost.MovieCategory, moviePost.Director, 
                moviePost.ReleaseDate, moviePost.Runtime);

            if (updatedMoviePost.IsFailure)
            {
                var errorMessages = updatedMoviePost.Errors.Select(error => error.Description).ToList();
                throw new ValidationException("Os campos de atualização do post do filme não foram preenchidos corretamente.",
                    errorMessages);
            }

            bool isMoviePostUpdated = await _moviePostRepository.UpdateMoviePostAsync(updatedMoviePost.Value, 
                moviePostId, cancellationToken);

            List<CastMember> updatedCast = CastMemberHelper.GetCastUpdated(moviePost.Cast, castImagePaths);

            bool isCastMovieUpdated = await _moviePostRepository.UpdateCastMovieAsync(updatedCast, 
                moviePostId);

            return isMoviePostUpdated;

        } catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> UpdateMovieRatingAsync(UpdateMovieRatingRequestDTO movieRating, Guid movieRatingId,
        CancellationToken cancellationToken)
    {
       try
        {
            var movieRatingExist = await _moviePostRepository.GetRatingByIdAsync(movieRatingId
                , cancellationToken);

            ThrowHelper.ThrowNotFoundExceptionIfNull(movieRatingExist, $"A avalição com o id {movieRatingId} não foi encontrada.");

            var updatedMovieRating = MovieRating.From(movieRating.IdentityUserId, movieRating.Rating);

            if (updatedMovieRating.IsFailure)
            {
                var errorMessages = updatedMovieRating.Errors.Select(error => error.Description).ToList();
                throw new ValidationException("Os campos de atualização da avaliação de filme não foram preenchidos corretamente.",
                    errorMessages);
            }

            await _moviePostRepository.UpdateMovieRatingAsync(updatedMovieRating.Value, movieRatingId,
                cancellationToken);

            var updateComment = Comment.From(movieRating.Comment.IdentityUserId, movieRating.Comment.Content);

            if (updateComment.IsFailure)
            {
                var errorMessages = updateComment.Errors.Select(error => error.Description).ToList();
                throw new ValidationException("Os campos de atualização do comentário não foram preenchidos corretamente.",
                    errorMessages);
            }

            await _moviePostRepository.UpdateCommentAsync(updateComment.Value, movieRatingId);

            return true;
        } catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> DeleteMoviePostAsync(Guid moviePostId, CancellationToken cancellationToken)
    {
       try
        {
            var moviePostExist = await _moviePostRepository.GetMoviePostByIdAsync(moviePostId, cancellationToken);

            ThrowHelper.ThrowNotFoundExceptionIfNull(moviePostExist, $"A postagem de filme com o id {moviePostId} não foi encontrada.");

            bool isDeleted = await _moviePostRepository.DeleteMoviePostAsync(moviePostId, cancellationToken);

            return isDeleted;
        } catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> DeleteMovieRatingAsync(Guid movieRatingId, CancellationToken cancellationToken)
    {
        try
        {
            var movieRatingExist = await _moviePostRepository.GetRatingByIdAsync(movieRatingId, cancellationToken);

            ThrowHelper.ThrowNotFoundExceptionIfNull(movieRatingExist, $"A avalição com o id {movieRatingId} não foi encontrada.");

            bool isDeleted = await _moviePostRepository.
                DeleteMovieRatingAsync(movieRatingId, cancellationToken);

            return isDeleted;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> DeleteCommentLikeAsync(Guid likeId, CancellationToken cancellationToken)
    {
        try {
       
            var likeExist = await _moviePostRepository.GetCommentLikeByIdAsync(likeId, cancellationToken);

            ThrowHelper.ThrowNotFoundExceptionIfNull(likeExist, $"O like com o id {likeId} não foi encontrado.");

            bool isDeleted = await _moviePostRepository.DeleteCommentLikeAsync(likeId, cancellationToken);

            return isDeleted;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
