using NerdCritica.Application.Services.ImageServiceConfiguration;
using NerdCritica.Domain.Common;
using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.Utils;
using System.IO;

namespace NerdCritica.Application.Services.Images;

public class ImageService : IImagesService
{
    private static string API_ROOT_DIRECTORY = string.Empty;

    public ImageService(IImageServiceConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.ApiRootDirectory))
        {
            throw new InvalidOperationException("API_ROOT_DIRECTORY não foi inicializado corretamente.");
        }

        API_ROOT_DIRECTORY = configuration.ApiRootDirectory;
    }

    public async Task<ProfileImage> GetProfileImageAsync(string profileImage)
    {
        var result = new ProfileImage();
        if (string.IsNullOrWhiteSpace(profileImage)) return result;

        var fileName = GenerateFileName();
        var profileImageBytes = ConvertFromBase64String(profileImage);
        var filePath = GetProfileImagePath(fileName);

        await SaveImageAsync(filePath, profileImageBytes);
        var profileImagePath = GetRelativeProfileImagePath(fileName);

        result.ProfileImageBytes = profileImageBytes;
        result.ProfileImagePath = profileImagePath;
        return result;
    }

    public async Task<MovieImages> GetMoviePostImagesAsync(string movieImage, string movieBackdrop)
    {
        var result = new MovieImages();

        if (!string.IsNullOrEmpty(movieImage))
        {
            var movieImageFileName = GenerateFileName();
            var movieImageBytes = ConvertFromBase64String(movieImage);
            var movieImageFilePath = GetMovieImagePath(movieImageFileName);

            await SaveImageAsync(movieImageFilePath, movieImageBytes);
            result.MovieImagePath = GetRelativeMovieImagePath(movieImageFileName);
            result.MovieImageBytes = movieImageBytes;
        }

        if (!string.IsNullOrEmpty(movieBackdrop))
        {
            var movieBackdropFileName = GenerateFileName();
            var movieBackdropBytes = ConvertFromBase64String(movieBackdrop);
            var movieBackdropFilePath = GetMovieBackdropPath(movieBackdropFileName);

            await SaveImageAsync(movieBackdropFilePath, movieBackdropBytes);
            result.MovieBackdropPath = GetRelativeMovieBackdropPath(movieBackdropFileName);
            result.MovieBackdropBytes = movieBackdropBytes;
        }

        return result;
    }

    public async Task<Dictionary<string, CastImages>> GetCastImagesAsync(List<CastMemberRequestDTO> cast)
    {
        var profileImagePaths = new Dictionary<string, CastImages>();

        if (cast == null || cast.Count == 0) return profileImagePaths;

        foreach (var item in cast)
        {
            var fileName = GenerateFileName();
            var memberImageBytes = ConvertFromBase64String(item.MemberImage);
            var filePath = GetCastImagePath(fileName);

            await SaveImageAsync(filePath, memberImageBytes);
            var profileImagePath = GetRelativeCastImagePath(fileName);

            profileImagePaths[item.MemberName] = CastImages.Create(profileImagePath, memberImageBytes);
        }

        return profileImagePaths;
    }

    public async Task<Dictionary<string, CastImages>> GetCastImagesAsync(List<UpdateCastMemberRequestDTO> cast)
    {
        var imagePaths = new Dictionary<string, CastImages>();

        if (cast == null || cast.Count == 0) return imagePaths;

        foreach (var item in cast)
        {
            var fileName = GenerateFileName();
            var memberImageBytes = ConvertFromBase64String(item.MemberImage);
            var filePath = GetCastImagePath(fileName);

            await SaveImageAsync(filePath, memberImageBytes);
            var memberImagePath = GetRelativeCastImagePath(fileName);

            imagePaths[item.MemberName] = CastImages.Create(memberImagePath, memberImageBytes);
        }

        return imagePaths;
    }

    private static async Task SaveImageAsync(string filePath, byte[] imageBytes)
    {
        try
        {
            await File.WriteAllBytesAsync(filePath, imageBytes);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao salvar a imagem: {ex.Message}");
        }
    }

    private static string GenerateFileName()
    {
        return Guid.NewGuid().ToString() + ".jpg";
    }

    private static byte[] ConvertFromBase64String(string base64String)
    {
        return Base64Helper.ConvertFromBase64String(base64String);
    }

    private static string GetProfileImagePath(string fileName)
    {
        return Path.Combine(API_ROOT_DIRECTORY, "wwwroot", "users", "images", fileName);
    }

    private static string GetRelativeProfileImagePath(string fileName)
    {
        return NormalizePath(Path.Combine("users", "images", fileName));
    }

    private static string GetMovieImagePath(string fileName)
    {
        string path = Path.Combine(API_ROOT_DIRECTORY, "wwwroot", "movies", "images", fileName);
        return path.Replace("\\", "/");
    }

    private static string GetRelativeMovieImagePath(string fileName)
    {
        return NormalizePath(Path.Combine("movies", "images", fileName));
    }

    private static string GetMovieBackdropPath(string fileName)
    {
        return Path.Combine(API_ROOT_DIRECTORY, "wwwroot", "movies", "backdrops", fileName);
    }

    private static string GetRelativeMovieBackdropPath(string fileName)
    {
        return NormalizePath(Path.Combine("movies", "backdrops", fileName));
    }

    private static string GetCastImagePath(string fileName)
    {
        return Path.Combine(API_ROOT_DIRECTORY, "wwwroot", "movies", "cast", fileName);
    }

    private static string GetRelativeCastImagePath(string fileName)
    {
        return NormalizePath(Path.Combine("movies", "cast", fileName));
    }

    private static string NormalizePath(string path)
    {
        return path.Replace("\\", "/");
    }
}

