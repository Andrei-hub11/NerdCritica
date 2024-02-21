using NerdCritica.Infrastructure.UserContext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using NerdCritica.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using NerdCritica.Domain.Repositories.User;
using NerdCritica.Infrastructure.Persistence.User;
using NerdCritica.Domain.DTOs.MappingsDapper;
using NerdCritica.Domain.DTOs.User;

namespace NerdCritica.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserContext(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddScoped<IUserContext, UserContexts>();
        services.AddAutoMapper(cfg =>
        {
            cfg.CreateMap<UserMapping, ProfileUserResponseDTO>();
        }, Assembly.GetExecutingAssembly());
        services.AddScoped<DapperContext>(); 
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}
