using NerdCritica.Application.Services.EmailService;
using NerdCritica.Application.Services.Images;
using NerdCritica.Application.Services.ImageServiceConfiguration;
using NerdCritica.Application.Services.Movies;
using NerdCritica.Application.Services.Token;
using NerdCritica.Application.Services.User;

namespace NerdCritica.Api.Extensions;

public static class ServicesConfigExtensions
{

    public static void AddGeneralServices(this IServiceCollection services)
    {
        services
             .AddControllers()
                         .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
        services.AddSingleton<IImageServiceConfiguration>(new ImageServiceConfiguration(AppDomain.CurrentDomain.BaseDirectory));
        services.AddTransient<IImagesService, ImageService>();
        services.AddTransient<IMoviePostService, MoviePostService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IEmailService, EmailService>();
    }
}
