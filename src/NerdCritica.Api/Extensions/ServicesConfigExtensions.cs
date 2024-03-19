using NerdCritica.Application.Services.EmailService;
using NerdCritica.Application.Services.Movies;
using NerdCritica.Application.Services.User;

namespace NerdCritica.Api.Extensions;

public static class ServicesConfigExtensions
{

    public static void AddGeneralServices(this IServiceCollection services)
    {
        services
             .AddControllers()
                         .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
        services.AddTransient<MoviePostService>();
        services.AddTransient<UserService>();
        //services.AddTransient<EmailService>();
    }
}
