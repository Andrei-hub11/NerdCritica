using NerdCritica.Infrastructure.UserContext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using NerdCritica.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using NerdCritica.Domain.Repositories.User;
using NerdCritica.Infrastructure.Persistence.User;
using NerdCritica.Domain.Repositories.Movies;
using NerdCritica.Infrastructure.Persistence.Movies;
using NerdCritica.Domain.Contracts;

namespace NerdCritica.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserContext(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddScoped<IUserContext, UserContexts>();
        services.AddScoped<DapperContext>(); 
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IMoviePostRepository, MoviePostRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}
