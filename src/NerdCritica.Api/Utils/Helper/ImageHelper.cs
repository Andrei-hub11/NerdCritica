

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
                    // Salve a imagem no servidor (por exemplo, na pasta wwwroot/images)
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
                    File.WriteAllBytes(filePath, stream.ToArray());
                    profileImagePath = "/images/" + fileName;
                } catch (Exception ex)
                {
                    throw new Exception( $"Erro ao salvar a imagem: {ex.Message}.");
                }
            }
        }

        return profileImagePath;
    }
}