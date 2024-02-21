
namespace NerdCritica.Domain.DTOs.User;

public record WishlistResponseDTO(Guid WishlistId, string IdentityUserId, string WishlistName,
    DateTime AddedAt);
