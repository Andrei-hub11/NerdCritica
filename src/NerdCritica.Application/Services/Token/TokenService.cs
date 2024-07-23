using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using NerdCritica.Contracts.DTOs.MappingsDapper;

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

    public string GenerateJwtToken(UserMapping user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>();

        if (!string.IsNullOrWhiteSpace(user.IdentityUserId) && !string.IsNullOrWhiteSpace(user.UserName))
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.IdentityUserId));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
        }
        else
        {
            new ArgumentNullException();
        }

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] 
            ?? throw new NullReferenceException("A Key dos Jwt não está presente")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
