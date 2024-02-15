
namespace NerdCritica.Domain.Entities;

public class Notification
{
    public Guid NotificationId { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public int Type { get; private set; }
    public string Content { get; private set; } = string.Empty;

    private Notification(string userId, int type, string content)
    {
        UserId = userId;
        Type = type;
        Content = content;
    }

    public static Notification Create(string userId, int type, string content)
    {
        return new Notification(userId, type, content);
    }
}
