using Microsoft.AspNetCore.Http;
using NerdCritica.Domain.Contracts;
using NerdCritica.Infrastructure.Extensions;

namespace NerdCritica.Infrastructure.UserContext;

internal sealed class UserContexts : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContexts(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId =>
        _httpContextAccessor.HttpContext?.User?.GetUserId() ??
        throw new ApplicationException("User context is unavailable");

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ??
        throw new ApplicationException("User context is unavailable");
}