
namespace NerdCritica.Domain.Entities
{
    public class Wishlist
    {
        public Guid WishlistId { get; private set; }
        public Guid MoviePostId { get; private set; }
        public string UserId { get; private set; } = string.Empty;
        public string WishlistName { get; private set; } = string.Empty;
        public DateTime AddedAt { get; private set; }

        private Wishlist(Guid moviePostId, string userId, string wishListName) { 
            MoviePostId = moviePostId;
            UserId = userId;
            WishlistName = wishListName;
            AddedAt = DateTime.Now;
        }

        public static Wishlist Create(Guid moviePostId, string userId, 
            string wishListName)
        {
            return new Wishlist(moviePostId, userId, wishListName);
        }
    }
}
