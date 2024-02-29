using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NerdCritica.Api.Utils.Helper;
using NerdCritica.Application.Services.User;
using NerdCritica.Domain.DTOs.User;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.Utils;
using System.Security.Claims;

namespace NerdCritica.Api.Controllers;

[Route("api/v1/account")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [Authorize]
    [HttpGet("get-me")]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserRequestDTO user,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
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
    [HttpPut("update/{userId}")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequestDTO user, string userId,
      CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return StatusCode(499);
        }

        byte[] profileImageBytes = Base64Helper.ConvertFromBase64String(user.ProfileImage);
        var pathProfileImage = await ImageHelper.GetPathProfileImageAsync(profileImageBytes);
        var resultDTO = await _userService.UpdateUserAsync(user, userId, pathProfileImage, profileImageBytes,
            cancellationToken);

        return Ok(new
        {
            Message = "A atualização foi bem-sucedida.",
            User = resultDTO
        });
    }

}
