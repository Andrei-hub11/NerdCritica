using Microsoft.AspNetCore.Http;
using NerdCritica.Domain.Contracts;
using NerdCritica.Domain.Utils.Exceptions;
using NerdCritica.Infrastructure.Extensions;

namespace NerdCritica.Infrastructure.UserContext;

internal sealed class UserContexts : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContexts(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId =>
        _httpContextAccessor.HttpContext?.User?.GetUserId() ??
        throw new UnauthorizeUserAccessException("O contexto do usuário não está disponível");

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ??
        throw new UnauthorizeUserAccessException("O contexto do usuário não está disponível\"");
}