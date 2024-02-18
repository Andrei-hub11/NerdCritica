
using NerdCritica.Domain.Utils;

namespace NerdCritica.Domain.Entities;

public class Wishlist
{
    public Guid WishlistId { get; private set; }
    public Guid MoviePostId { get; private set; }
    public string IdentityUserId { get; private set; } = string.Empty;
    public string WishlistName { get; private set; } = string.Empty;
    public DateTime AddedAt { get; private set; }

    private Wishlist(Guid moviePostId, string userId, string wishListName) { 
        MoviePostId = moviePostId;
        IdentityUserId = userId;
        WishlistName = wishListName;
        AddedAt = DateTimeHelper.NowInBrasilia();
    }

    public static Result<Wishlist> Create(Guid moviePostId, string identityUserId, 
        string wishListName)
    {
        var isCreate = true;
        var result = WishlistValidation(wishListName, isCreate, moviePostId, 
            identityUserId);

        if (result.Count > 0)
        {
            var emptyWishlist = new Wishlist(Guid.Empty, string.Empty, 
                string.Empty);
            return Result.AddErrors(result, emptyWishlist);
        }

        var wishlist = new Wishlist(moviePostId, identityUserId, wishListName);

        return Result.Ok(wishlist);
    }

    public static Result<Wishlist> Update(Wishlist wishlist,string wishListName)
    {
        var isCreate = false;
        var result = WishlistValidation(wishListName, isCreate);

        if (result.Count > 0)
        {
            var emptyWishlist = new Wishlist(Guid.Empty, string.Empty,
                string.Empty);
            return Result.AddErrors(result, emptyWishlist);
        }

        wishlist.WishlistName = wishListName;

        return Result.Ok(wishlist);
    }

    private static List<Error> WishlistValidation(string wishListName, bool isCreate,
        Guid? moviePostId = null, string identityUserId = "")
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(wishListName))
        {
            errors.Add(new Error("O título da lista de favoritos não pode estar vazio"));
        }

        if (isCreate && moviePostId == null || isCreate && moviePostId == Guid.Empty)
        {
            errors.Add(new Error("O id do post não pode estar vazio"));
        }

        if (isCreate && string.IsNullOrWhiteSpace(identityUserId))
        {
            errors.Add(new Error("O id do usuário não pode estar vazio"));
        }

        if (isCreate && !string.IsNullOrEmpty(identityUserId) &&
           !Guid.TryParse(identityUserId, out Guid result))
        {
            errors.Add(new Error($"{identityUserId} não é um id válido."));
        }

        return errors;
    }
}
