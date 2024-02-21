

using System.Text.Json;
using System.Text.Json.Serialization;

namespace NerdCritica.Application.Features;

public class ProfileImageConverter : JsonConverter<byte[]>
{
    public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException(); // Não é necessário para a leitura
    }

    public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
    {
        if (value == null || value.Length == 0)
        {
            value = Array.Empty<byte>(); // Atribui um array de bytes vazio
        }

        writer.WriteStringValue(Convert.ToBase64String(value));
    }

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(byte[]);
    }
}