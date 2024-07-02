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
        Assert.False(result.IsFailure);
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

        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("A avaliação não pode ser maior que 5.",
            result.Errors.Select(e => e.Description));
    }

    [Fact(DisplayName = "From should return success result with valid data")]
    public void From_WithValidData_ShouldReturnSuccessResult()
    {
        var identityUserId = Guid.NewGuid().ToString();
        var rating = 4.5m;

        var result = MovieRating.From(identityUserId, rating);

        Assert.False(result.IsFailure);
        Assert.Equal(identityUserId, result.Value.IdentityUserId);
        Assert.Equal(rating, result.Value.Rating);
    }

    [Theory(DisplayName = "Update should return failure result with invalid rating")]
    [InlineData(6.0, "A avaliação não pode ser maior que 5.")]
    [InlineData(10.0, "A avaliação não pode ser maior que 5.")]
    [InlineData(-1.0, "A avaliação não pode ser menor que 0.")]
    [InlineData(-10.0, "A avaliação não pode ser menor que 0.")]
    public void From_WithInvalidRating_ShouldReturnFailureResult(decimal invalidRating, string expectedErrorMessage)
    {
        var identityUserId = Guid.NewGuid().ToString();

        var result = MovieRating.From(identityUserId, invalidRating);

        Assert.True(result.IsFailure);
        Assert.Null(result.Value);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(expectedErrorMessage, result.Errors.Select(e => e.Description));
    }
}
