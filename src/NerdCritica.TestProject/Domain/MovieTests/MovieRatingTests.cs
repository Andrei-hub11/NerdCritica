using NerdCritica.Domain.Entities;

namespace NerdCritica.TestProject.Domain.MovieTests;

public class MovieRatingTests
{
    [Fact(DisplayName = "Create should return success result with valid data")]
    public void Create_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        var moviePostId = Guid.NewGuid();
        var identityUserId = Guid.NewGuid().ToString();
        var rating = 4.5m;

        // Act
        var result = MovieRating.Create(moviePostId, identityUserId, rating);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(moviePostId, result.Value.MoviePostId);
        Assert.Equal(identityUserId, result.Value.IdentityUserId);
        Assert.Equal(rating, result.Value.Rating);
    }

    [Fact(DisplayName = "Create should return failure result with invalid rating")]
    public void Create_WithInvalidRating_ShouldReturnFailureResult()
    {
        var moviePostId = Guid.NewGuid();
        var identityUserId = Guid.NewGuid().ToString();
        var invalidRating = 6m;

        var result = MovieRating.Create(moviePostId, identityUserId, invalidRating);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("A avaliação não pode ser maior que 5.",
            result.Errors.Select(e => e.Description));
    }

    [Fact(DisplayName = "Update should return success result with valid data")]
    public void Update_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        var moviePostId = Guid.NewGuid();
        var identityUserId = Guid.NewGuid().ToString();
        var rating = 4.5m;
        var movieRating = MovieRating.Create(moviePostId, identityUserId, rating);

        var updatedRating = 3.5m;
        var result = MovieRating.Update(movieRating.Value, updatedRating);

        Assert.True(result.Success);
        Assert.Equal(moviePostId, result.Value.MoviePostId);
        Assert.Equal(identityUserId, result.Value.IdentityUserId);
        Assert.Equal(updatedRating, result.Value.Rating);
    }

    [Theory(DisplayName = "Update should return failure result with invalid rating")]
    [InlineData(6.0, "A avaliação não pode ser maior que 5.")]
    [InlineData(10.0, "A avaliação não pode ser maior que 5.")]
    [InlineData(-1.0, "A avaliação não pode ser menor que 0.")]
    [InlineData(-10.0, "A avaliação não pode ser menor que 0.")]
    public void Update_WithInvalidRating_ShouldReturnFailureResult(decimal invalidRating, string expectedErrorMessage)
    {
        var movieRatingId = Guid.NewGuid();
        var moviePostId = Guid.NewGuid();
        var identityUserId = Guid.NewGuid().ToString();
        var rating = 4.5m;
        var movieRating = MovieRating.Create(moviePostId, identityUserId, rating);

        var result = MovieRating.Update(movieRating.Value, invalidRating);

        Assert.False(result.Success);
        Assert.NotEqual(invalidRating, result.Value.Rating);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(expectedErrorMessage, result.Errors.Select(e => e.Description));
    }
}
