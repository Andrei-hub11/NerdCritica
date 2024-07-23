namespace NerdCritica.Contracts.DTOs.MappingsDapper;

public class UserLogin
{
    public string Token { get; set; } = string.Empty;
    public UserMapping User { get; set; } = new UserMapping();

    public UserLogin(string token, UserMapping user)
    {
        Token = token;
        User = user;
    }
}
