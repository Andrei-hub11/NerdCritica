

namespace NerdCritica.Domain.Utils;

public static class Base64Helper
{
    public static byte[] ConvertFromBase64String(string base64String)
    {
        try
        {
            return Convert.FromBase64String(base64String);
        }
        catch (FormatException ex)
        {
            throw new FormatException($"Erro ao converter a string Base64 para bytes: {ex.Message}");
        }
    }

}

