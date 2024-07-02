using Microsoft.Extensions.Configuration;
using NerdCritica.Domain.DTOs.MappingsDapper;
using System.Security.Cryptography;
using System.Text;

namespace NerdCritica.Application.Services.Token;

public class TokenService: ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GeneratePasswordResetToken(UserMapping user)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["TokenSecret"] ??
            throw new NullReferenceException("O TokenSecret não está presente"));

        using (var hmac = new HMACSHA256(key))
        {
            var token = $"{user.IdentityUserId}:{Guid.NewGuid()}";
            var tokenBytes = Encoding.UTF8.GetBytes(token);
            var tokenHash = hmac.ComputeHash(tokenBytes);
            return Convert.ToBase64String(tokenHash) + "." + Convert.ToBase64String(tokenBytes);
        }
    }

    public bool ValidatePasswordResetToken(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 2) return false;

        var tokenHash = Convert.FromBase64String(parts[0]);
        var tokenBytes = Convert.FromBase64String(parts[1]);

        var key = Encoding.UTF8.GetBytes(_configuration["TokenSecret"] ??
            throw new NullReferenceException("O TokenSecret não está presente"));


        using (var hmac = new HMACSHA256(key))
        {
            var computedHash = hmac.ComputeHash(tokenBytes);
            return tokenHash.SequenceEqual(computedHash);
        }
    }
}
