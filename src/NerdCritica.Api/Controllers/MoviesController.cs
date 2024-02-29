using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NerdCritica.Api.Utils.Helper;
using NerdCritica.Application.Services.Movies;
using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.Entities;

namespace NerdCritica.Api.Controllers;

[Route("api/v1/movies")]
[ApiController]
public class MoviesController : ControllerBase
{

    private MoviePostService _postService;

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

        return Ok(new {Movies = movies});
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

    [Authorize(Policy = "Admin")]
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
}
