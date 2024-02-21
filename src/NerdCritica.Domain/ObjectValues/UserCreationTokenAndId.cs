

namespace NerdCritica.Domain.ObjectValues;

public class UserCreationTokenAndId
{
   public string UserId { get; set; } = string.Empty;
   public string Token { get; set; } = string.Empty;

    public UserCreationTokenAndId(string newUserId, string token)
    {
        UserId = newUserId;
        Token = token;
    }
}
