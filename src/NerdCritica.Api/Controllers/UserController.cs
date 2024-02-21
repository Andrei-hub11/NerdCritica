using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NerdCritica.Application.Services.User;
using NerdCritica.Domain.DTOs.User;
using NerdCritica.Domain.Utils;
using NerdCritica.Domain.Utils.Exceptions;
using System.Security.Claims;

namespace NerdCritica.Api.Controllers;

[Route("api/v1")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly UserService _userService;

    public UserController(IWebHostEnvironment hostingEnvironment, UserService userService)
    {
        _hostingEnvironment = hostingEnvironment;
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

       var pathProfileImage = await ImageHelper.GetPathProfileImageAsync(user.ProfileImage ?? new byte[0]);
       var resultDTO = await _userService.CreateUserAsync(user, pathProfileImage, cancellationToken);

       return Ok(new { Message = "Registro bem sucedido. Seja bem-vindo", 
           User = resultDTO.user, Token = resultDTO.token});
    }
}
