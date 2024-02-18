
namespace NerdCritica.Domain.DTOs.User;

public record WishlistRequestDTO(Guid WishlistId, string IdentityUserId, string WishlistName,
    DateTime AddedAt);
