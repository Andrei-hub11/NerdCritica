using AutoMapper;
using NerdCritica.Application.DTOMappers;
using NerdCritica.Application.Services.Images;
using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.Entities.Aggregates;
using NerdCritica.Domain.Repositories.Movies;
using NerdCritica.Domain.Utils;
using NerdCritica.Domain.Utils.Exceptions;
using System.Text;

namespace NerdCritica.Application.Services.Movies;


public class MoviePostService : IMoviePostService
{

    private readonly IMoviePostRepository _moviePostRepository;
    private readonly IImagesService _imageService;
    private readonly IMapper _mapper;

    public MoviePostService(IMoviePostRepository moviePostRepository, IImagesService imageService,
        IMapper mapper)
    {
        _moviePostRepository = moviePostRepository;
        _imageService = imageService;
        _mapper = mapper;
    }

    public async Task<MoviePostResponseDTO> GetMoviePostAsync(Guid moviePostId, CancellationToken cancellationToken)
    {
        try
        {
            var movie = await _moviePostRepository.GetMoviePostByIdAsync(moviePostId, cancellationToken);

            ThrowHelper.ThrowNotFoundExceptionIfNull(movie, $"A postagem de filme com o id {moviePostId} não foi encontrada.");

            var movieDTO = _mapper.Map<MoviePostResponseDTO>(movie);

            return movieDTO;
        }
        catch
        {
            throw;
        }
    }

    public async Task<IReadOnlyCollection<MoviePostResponseDTO>> GetMoviePostsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var movies = await _moviePostRepository.GetMoviePostsAsync(cancellationToken);

        return movies.ToDTO();
    }

    public async Task<bool> CreateMoviePostAsync(CreateMoviePostRequestDTO moviePost, CancellationToken cancellationToken)
    {
        try
        {
            var postImages = await _imageService.GetMoviePostImagesAsync(moviePost.MovieImage,
            moviePost.MovieBackdropImage);
            var castImagePaths = await _imageService.GetCastImagesAsync(moviePost.Cast);

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
        }
        catch (Exception)
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

        }
        catch (Exception)
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
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> UpdateMoviePostAsync(UpdateMoviePostRequestDTO moviePost,
        Guid moviePostId, CancellationToken cancellationToken)
    {
        try
        {
            var postImages = await _imageService.GetMoviePostImagesAsync(moviePost.MovieImage,
           moviePost.MovieBackdropImage);
            var castImagePaths = await _imageService.GetCastImagesAsync(moviePost.Cast);

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

        }
        catch (Exception)
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
        }
        catch (Exception)
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
        }
        catch (Exception)
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

    public async Task<bool> DeleteCommentLikeAsync(DeleteLikeRequestDTO deleteLikeRequest,
        CancellationToken cancellationToken)
    {
        try
        {

            var likeExist = await _moviePostRepository.GetCommentLikeByIdAsync(deleteLikeRequest, cancellationToken);

            StringBuilder message = new StringBuilder();
            message.Append("O like não foi encontrado. ");
            message.Append($"Verifique se {deleteLikeRequest.CommentId} e {deleteLikeRequest.IdentityUserId} ");
            message.Append("estão corretos.");

            ThrowHelper.ThrowNotFoundExceptionIfNull(likeExist, message.ToString());

            bool isDeleted = await _moviePostRepository.DeleteCommentLikeAsync(deleteLikeRequest, cancellationToken);

            return isDeleted;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
