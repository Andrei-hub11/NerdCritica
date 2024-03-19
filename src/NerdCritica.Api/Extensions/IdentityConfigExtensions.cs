using Microsoft.AspNetCore.Identity;
using NerdCritica.Infrastructure.Context;

namespace NerdCritica.Api.Extensions;

public static class IdentityConfigExtensions
{
    public static void AddCustomIdentity(this IServiceCollection services)
    {
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }

    public static void AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            options.AddPolicy("User", policy => policy.RequireRole("User"));
            options.AddPolicy("ModeratorOrAdmin", policy => policy.RequireRole("Moderator", "Admin"));
        });
    }
}