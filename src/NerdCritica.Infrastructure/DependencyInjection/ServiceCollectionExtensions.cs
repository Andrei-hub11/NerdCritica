using NerdCritica.Infrastructure.UserContext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using NerdCritica.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace NerdCritica.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserContext(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddScoped<IUserContext, UserContexts>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }
}
