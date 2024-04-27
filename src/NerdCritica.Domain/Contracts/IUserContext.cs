

namespace NerdCritica.Domain.Contracts
{
    public interface IUserContext
    {
        bool IsAuthenticated { get; }
        Guid UserId { get; }
    }
}
