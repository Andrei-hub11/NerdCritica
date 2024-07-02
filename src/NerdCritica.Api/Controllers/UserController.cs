using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NerdCritica.Application.Services.User;
using NerdCritica.Domain.Contracts;
using NerdCritica.Domain.DTOs.User;
using NerdCritica.Domain.Utils.Exceptions;

namespace NerdCritica.Api.Controllers;

[Route("api/v1/account")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserContext _userContext;

    public UserController(IUserService userService, IUserContext userContext)
    {
        _userService = userService;
        _userContext = userContext;
    }

    [Authorize]
    [HttpGet("get-me")]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        var userId = _userContext.UserId; // Obtém o ID do usuário do token JWT

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

    [HttpGet("verify-password-reset")]
    public async Task<IActionResult> VerifyResetPassword([FromQuery] VerifyResetPasswordRequestDTO request,
    CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        var success = await _userService.VerifyResetPasswordAsync(request, cancellationToken);
        if (!success)
        {
            return BadRequest("Token inválido ou token expirou");
        }

        return Ok();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordRequestDTO request, CancellationToken cancellationToken)
    {
        var result = await _userService.ForgotPasswordAsync(request.Email, cancellationToken);

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserRequestDTO user,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        var resultDTO = await _userService.CreateUserAsync(user, cancellationToken);

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
    [HttpPut("{identityUserId}")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequestDTO user, string identityUserId,
      CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        var resultDTO = await _userService.UpdateUserAsync(user, identityUserId, cancellationToken);

        return Ok(new
        {
            Message = "A atualização foi bem-sucedida.",
            User = resultDTO
        });
    }

    [HttpPut("update-password")]
    public async Task<IActionResult> UpdateUserPassword([FromBody] UpdatePasswordRequestDTO request)
    {
        bool isUpdated = await _userService.UpdateUserPasswordAsync(request);

        if (isUpdated)
        {
            return Ok(isUpdated);
        }
        else
        {
            return StatusCode(500, "Ocorreu um erro ao atualizar a senha.");
        }
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
