using NerdCritica.Domain.Entities;

namespace NerdCritica.TestProject.Domain.UserTests;

public class FavoriteMovieTests
{
    [Fact(DisplayName = "Create should return success result with valid data")]
    public void Create_WithValidData_ShouldReturnSuccessResult()
    {
        var moviePostId = Guid.NewGuid();
        var identityUserId = Guid.NewGuid().ToString(); // Simulando um ID de usuário válido

        var result = FavoriteMovie.Create(moviePostId, identityUserId);

        Assert.False(result.IsFailure);
        Assert.NotNull(result.Value);
        Assert.Equal(moviePostId, result.Value.MoviePostId);
        Assert.Equal(identityUserId, result.Value.IdentityUserId);
    }
}
