using NerdCritica.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdCritica.TestProject.Domain.UserTests;

public class WishlistTests
{
    [Fact(DisplayName = "Create should return success result with valid data")]
    public void Create_WithValidData_ShouldReturnSuccessResult()
    {
        var moviePostId = Guid.NewGuid();
        var identityUserId = Guid.NewGuid().ToString(); // Simulando um ID de usuário válido
        var wishListName = "My Wishlist";

        var result = Wishlist.Create(moviePostId, identityUserId, wishListName);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(moviePostId, result.Value.MoviePostId);
        Assert.Equal(identityUserId, result.Value.IdentityUserId);
        Assert.Equal(wishListName, result.Value.WishlistName);
    }

    [Theory(DisplayName = "Create should return failure result with invalid data")]
    [InlineData("", "O título da lista de favoritos não pode estar vazio")]
    [InlineData(null, "O título da lista de favoritos não pode estar vazio")]
    [InlineData(" ", "O título da lista de favoritos não pode estar vazio")]
    public void Create_WithInvalidData_ShouldReturnFailureResult(string invalidWishListName, string expectedErrorMessage)
    {
        var moviePostId = Guid.NewGuid();
        var identityUserId = Guid.NewGuid().ToString();

        var result = Wishlist.Create(moviePostId, identityUserId, invalidWishListName);

        Assert.False(result.Success);
        Assert.Empty(result.Value.WishlistName);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(expectedErrorMessage, result.Errors.Select(e => e.Description));
    }

    [Fact(DisplayName = "Update should return success result with valid data")]
    public void Update_WithValidData_ShouldReturnSuccessResult()
    {
        var moviePostId = Guid.NewGuid();
        var identityUserId = Guid.NewGuid().ToString();
        var wishListName = "My Wishlist";
        var wishlist = Wishlist.Create(moviePostId, identityUserId, wishListName);

        var updatedWishListName = "Updated Wishlist Name";
        var result = Wishlist.Update(wishlist.Value, updatedWishListName);

        Assert.True(result.Success);
        Assert.Equal(updatedWishListName, result.Value.WishlistName);
    }

    [Theory(DisplayName = "Update should return failure result with invalid data")]
    [InlineData("", "O título da lista de favoritos não pode estar vazio")]
    [InlineData(null, "O título da lista de favoritos não pode estar vazio")]
    [InlineData(" ", "O título da lista de favoritos não pode estar vazio")]
    public void Update_WithInvalidData_ShouldReturnFailureResult(string invalidWishListName, string expectedErrorMessage)
    {
        var moviePostId = Guid.NewGuid();
        var identityUserId = Guid.NewGuid().ToString();
        var wishListName = "My Wishlist";
        var wishlist = Wishlist.Create(moviePostId, identityUserId, wishListName);

        var result = Wishlist.Update(wishlist.Value, invalidWishListName);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(expectedErrorMessage, result.Errors.Select(e => e.Description));
    }
}
