

namespace NerdCritica.Domain.Contracts
{
    public interface IUserContext
    {
        bool IsAuthenticated { get; }
        string UserId { get; }
    }
}
