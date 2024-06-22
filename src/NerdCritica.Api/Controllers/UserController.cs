using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NerdCritica.Api.Utils.Helper;
using NerdCritica.Application.Services.EmailService;
using NerdCritica.Application.Services.User;
using NerdCritica.Domain.DTOs.User;
using NerdCritica.Domain.Utils;
using System.Security.Claims;

namespace NerdCritica.Api.Controllers;

[Route("api/v1/account")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    //private readonly EmailService _emailService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize]
    [HttpGet("get-me")]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Obtém o ID do usuário do token JWT

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException();
        }

        var user = await _userService.GetUserByIdAsync(userId, cancellationToken);
        return Ok(new
        {
            User = user,
        });
    }

    [Authorize]
    [HttpGet("favorite-movies/{identityUserId}")]
    public async Task<IActionResult> GetFavoriteMovies(string identityUserId, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        var favoriteMovies = await _userService.GetFavoriteMovies(identityUserId, cancellationToken);
        return Ok(new
        {
            FavoriteMovies = favoriteMovies,
        });
    }

    //[HttpPost("forgot-password")]
    //public async Task<IActionResult> ForgotPassword()
    //{
    //    EmailMetadata emailMetadata = new("seu-email@exemplo.com",
    //"FluentEmail Test email",
    //"This is a test email from FluentEmail.");
    //    await _emailService.Send(emailMetadata);
    //    return Ok();
    //}

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserRequestDTO user,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        byte[] profileImageBytes = Base64Helper.ConvertFromBase64String(user.ProfileImage);
        var pathProfileImage = await ImageHelper.GetPathProfileImageAsync(profileImageBytes);
        var resultDTO = await _userService.CreateUserAsync(user, pathProfileImage, profileImageBytes, cancellationToken);

        return Ok(new
        {
            Message = "Registro bem sucedido. Seja bem-vindo",
            resultDTO.User,
            resultDTO.Token,
            resultDTO.Role
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO user,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        var resultDTO = await _userService.LoginUserAsync(user, cancellationToken);

        return Ok(new
        {
            resultDTO.User,
            resultDTO.Token,
            resultDTO.Role
        });
    }

    [Authorize]
    [HttpPost("favorite-movie")]
    public async Task<IActionResult> AddFavoriteMovie([FromBody] AddFavoriteMovieRequestDTO addFavoriteMovieRequestDTO,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        bool isAdded = await _userService.AddFavoriteMovieAsync(addFavoriteMovieRequestDTO, cancellationToken);

        return Ok(isAdded);
    }

    [Authorize]
    [HttpPut("update/{identityUserId}")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequestDTO user, string identityUserId,
      CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        byte[] profileImageBytes = Base64Helper.ConvertFromBase64String(user.ProfileImage);
        var pathProfileImage = await ImageHelper.GetPathProfileImageAsync(profileImageBytes);
        var resultDTO = await _userService.UpdateUserAsync(user, identityUserId, pathProfileImage, profileImageBytes,
            cancellationToken);

        return Ok(new
        {
            Message = "A atualização foi bem-sucedida.",
            User = resultDTO
        });
    }

    [Authorize]
    [HttpDelete("favorite-movie")]
    public async Task<IActionResult> RemoveFavoriteMovie([FromBody] RemoveFavoriteMovieRequestDTO removeFavoriteMovie,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        bool isRemoved = await _userService.RemoveFavoriteMovie(removeFavoriteMovie.FavoriteMovieId, 
            removeFavoriteMovie.IdentityUserId, cancellationToken);

        return Ok(isRemoved);
    }
}
