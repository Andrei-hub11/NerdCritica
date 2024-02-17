
namespace NerdCritica.Domain.DTOs.User;

public record WishlistDTO(Guid WishlistId, string IdentityUserId, string WishlistName,
    DateTime AddedAt);
