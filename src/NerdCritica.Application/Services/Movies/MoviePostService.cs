using AutoMapper;
using NerdCritica.Domain.Common;
using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.Entities.Aggregates;
using NerdCritica.Domain.Repositories.Movies;
using NerdCritica.Domain.Utils;
using NerdCritica.Domain.Utils.Exceptions;

namespace NerdCritica.Application.Services.Movies;


public class MoviePostService : IMoviePostService
{

private readonly IMoviePostRepository _moviePostRepository;
private readonly IMapper _mapper;

    public MoviePostService(IMoviePostRepository moviePostRepository, IMapper mapper)
    {
        _moviePostRepository = moviePostRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MoviePostResponseDTO>> GetMoviePostsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

       var movies = await _moviePostRepository.GetMoviePostsAsync(cancellationToken);

        var moviesMapping = movies.Select(_mapper.Map<MoviePostResponseDTO>).ToList();

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

    public async Task<bool> UpdateMoviePostAsync(UpdateMoviePostRequestDTO moviePost, MovieImages postImages, 
        Dictionary<string, CastImages> castImagePaths, Guid moviePostId, CancellationToken cancellationToken)
    {
        try
        {
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

    public Task<bool> DeleteMoviePostAsync(string moviePostId)
    {
        throw new NotImplementedException();
    }
}
