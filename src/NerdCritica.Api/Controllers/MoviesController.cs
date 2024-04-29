using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NerdCritica.Api.Utils.Helper;
using NerdCritica.Application.Services.Movies;
using NerdCritica.Domain.DTOs.Movie;

namespace NerdCritica.Api.Controllers;

[Route("api/v1/movies")]
[ApiController]
public class MoviesController : ControllerBase
{

    private readonly MoviePostService _postService;

    public MoviesController(MoviePostService postService)
    {
        _postService = postService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetMovies(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        var movies = await _postService.GetMoviePostsAsync(cancellationToken);

        return Ok(new { Movies = movies });
    }

    [Authorize]
    [HttpGet("{moviePostId}")]
    public async Task<IActionResult> GetMovie(Guid moviePostId, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        var movie = await _postService.GetMoviePostAsync(moviePostId, cancellationToken);

        return Ok(new { Movie = movie });
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateMovie(CreateMoviePostRequestDTO moviePost, 
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        var postImages = await ImageHelper.GetPathPostImagesAsync(moviePost.MovieImage, 
            moviePost.MovieBackdropImage);
        var castImages = await ImageHelper.GetPathCastImagesAsync(moviePost.Cast);

        bool isCreate = await _postService.CreateMoviePostAsync(moviePost, postImages, castImages, 
            cancellationToken);

        return Ok(new { Success = isCreate });
    }

    [Authorize]
    [HttpPost("create-rating")]
    public async Task<IActionResult> CreateRating(CreateRatingRequestDTO rating,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        bool isCreate = await _postService.CreateRatingAsync(rating, cancellationToken);

        return Ok(new { Success = isCreate });
    }

    [Authorize]
    [HttpPost("create-like")]
    public async Task<IActionResult> CreateLike(CreateCommentLikeRequestDTO like, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        var isCreated = await _postService.CreateCommentLikeAsync(like, cancellationToken);

        return Ok(new { Success = isCreated });
    }


    [Authorize(Policy = "Admin")]
    [HttpPut("update-movie/{moviePostId}")]
    public async Task<IActionResult> UpdateMoviePost(UpdateMoviePostRequestDTO moviePost, Guid moviePostId,
       CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        var postImages = await ImageHelper.GetPathPostImagesAsync(moviePost.MovieImage,
            moviePost.MovieBackdropImage);
        var castImages = await ImageHelper.GetPathCastImagesAsync(moviePost.Cast);

        bool isUpdated = await _postService.UpdateMoviePostAsync(moviePost, postImages, castImages, moviePostId,
            cancellationToken);

        return Ok(new { Success = isUpdated });
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("update-movierating/{movieRatingId}")]
    public async Task<IActionResult> UpdateMovieRating(UpdateMovieRatingRequestDTO movieRating, 
        Guid movieRatingId, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        var isUpdated = await _postService.UpdateMovieRatingAsync(movieRating, movieRatingId, 
            cancellationToken);

        return Ok(new { Success = true });
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("delete-movie/{moviePostId}")]
    public async Task<IActionResult> DeleteMoviePost(Guid moviePostId,
       CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        bool isDeleted = await _postService.DeleteMoviePostAsync(moviePostId, cancellationToken);

        return Ok(new { Success = isDeleted });
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("delete-movierating/{movieRatingId}")]
    public async Task<IActionResult> DeleteMovieRating(Guid movieRatingId, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        bool isDeleted = await _postService.DeleteMovieRatingAsync(movieRatingId, cancellationToken);

        return Ok(new { Success = isDeleted });
    }

    [Authorize]
    [HttpDelete("delete-like/{likeId}")]
    public async Task<IActionResult> DeleteLike(Guid likeId,
    CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        var isUpdated = await _postService.DeleteCommentLikeAsync(likeId, cancellationToken);

        return Ok(new { Success = isUpdated });
    }

}
