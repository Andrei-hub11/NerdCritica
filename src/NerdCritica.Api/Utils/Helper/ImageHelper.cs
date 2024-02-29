

using NerdCritica.Domain.Common;
using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.Utils;

namespace NerdCritica.Api.Utils.Helper;

public static class ImageHelper
{
    public static async Task<string> GetPathProfileImageAsync(byte[] image)
    {
        string profileImagePath = "";

        if (image != null && image.Length > 0)
        {
            using (var stream = new MemoryStream())
            {
                try
                {
                    await stream.WriteAsync(image);
                    var fileName = Guid.NewGuid().ToString() + ".jpg";
                    var filePath = "";
       
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "users", "images",
                       fileName);

                    File.WriteAllBytes(filePath, stream.ToArray());
                    profileImagePath = "users/images/" + fileName;
                } catch (Exception ex)
                {
                    throw new Exception( $"Erro ao salvar a imagem de perfil: {ex.Message}.");
                }
            }
        }

        return profileImagePath;
    }

    public static async Task<MovieImages> GetPathPostImagesAsync(string movieImage, string movieBackdrop)
    {
        var result = new MovieImages();

        if (movieImage != null && movieImage.Length > 0)
        {
            try
            {
                var movieImageFileName = Guid.NewGuid().ToString() + ".jpg";
                byte[] movieImageBytes = Base64Helper.ConvertFromBase64String(movieImage);
                var movieImageFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "movies", "images", movieImageFileName);
                //await File.WriteAllBytesAsync(movieImageFilePath, movieImage);
                await SaveImageAsync(movieImageFilePath, movieImageBytes);
                result.MovieImagePath = "movies/images/" + movieImageFileName;
                result.MovieImageBytes = movieImageBytes;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao salvar a imagem do filme: {ex.Message}.");
            }
        }

        if (movieBackdrop != null && movieBackdrop.Length > 0)
        {
            try
            {
                var movieBackdropFileName = Guid.NewGuid().ToString() + ".jpg";
                byte[] movieBackdropBytes = Base64Helper.ConvertFromBase64String(movieBackdrop);
                var movieBackdropFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "movies", "backdrops", movieBackdropFileName);
                //await File.WriteAllBytesAsync(movieBackdropFilePath, movieBackdrop);
                await SaveImageAsync(movieBackdropFilePath, movieBackdropBytes);
                result.MovieBackdropPath = "movies/backdrops/" + movieBackdropFileName;
                result.MovieBackdropBytes = movieBackdropBytes;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao salvar o backdrop do filme: {ex.Message}.");
            }
        }

        return result;
    }

    public static async Task<Dictionary<string, CastImages>> GetPathCastImagesAsync(List<CastMemberRequestDTO> cast)
    {
        Dictionary<string, CastImages> profileImagePaths = new();

        if (cast != null && cast.Count > 0)
        {
            foreach (var item in cast)
            {
                string profileImagePath = "";
                try
                {
                    byte[] memberImageBytes = Base64Helper.ConvertFromBase64String(item.MemberImage);
                    var fileName = Guid.NewGuid().ToString() + ".jpg";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "movies", "cast", fileName);

                   await SaveImageAsync(filePath, memberImageBytes);

                    profileImagePath = "movies/cast/" + fileName;

                    profileImagePaths.Add(item.MemberName, CastImages.Create(profileImagePath, memberImageBytes));
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao salvar a imagem para o membro do elenco " +
                        $"'{item.MemberName}': {ex.Message}");
                }
            }
        }

        return profileImagePaths;
    }

    public static async Task<Dictionary<string, CastImages>> GetPathCastImagesAsync(List<UpdateCastMemberRequestDTO> cast)
    {
        Dictionary<string, CastImages> profileImagePaths = new();

        if (cast != null && cast.Count > 0)
        {
            foreach (var item in cast)
            {
                string profileImagePath = "";
                try
                {
                    byte[] memberImageBytes = Base64Helper.ConvertFromBase64String(item.MemberImage);
                    var fileName = Guid.NewGuid().ToString() + ".jpg";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "movies", "cast", fileName);

                    await SaveImageAsync(filePath, memberImageBytes);

                    profileImagePath = "movies/cast/" + fileName;

                    profileImagePaths.Add(item.MemberName, CastImages.Create(profileImagePath, memberImageBytes));
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao salvar a imagem para o membro do elenco " +
                        $"'{item.MemberName}': {ex.Message}");
                }
            }
        }

        return profileImagePaths;
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
}