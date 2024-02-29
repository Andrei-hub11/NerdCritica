using NerdCritica.Domain.Entities;

namespace NerdCritica.TestProject.Domain.MovieTests;

public class CommentTests
{
    [Fact(DisplayName = "Create should return success result with valid data")]
    public void Create_WithValidData_ShouldReturnSuccessResult()
    {
        var ratingId = Guid.NewGuid();
        var identityUserId = Guid.NewGuid().ToString();
        var content = "This is a valid comment.";

        var result = Comment.Create(ratingId, identityUserId, content);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(ratingId, result.Value.RatingId);
        Assert.Equal(identityUserId, result.Value.IdentityUserId);
        Assert.Equal(content, result.Value.Content);
    }

    [Fact(DisplayName = "Create should return failure result with invalid data")]
    public void Create_WithInvalidData_ShouldReturnFailureResult()
    {
        var ratingId = Guid.NewGuid();
        var identityUserId = Guid.NewGuid().ToString();
        var invalidContent = "";

        var result = Comment.Create(ratingId, identityUserId, invalidContent);

        Assert.False(result.Success);
        Assert.Empty(result.Value.Content);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("O comentário do post não pode estar vazio",
            result.Errors.Select(e => e.Description));
    }

    [Fact(DisplayName = "Update should return success result with valid data")]
    public void Update_WithValidData_ShouldReturnSuccessResult()
    {
        var ratingId = Guid.NewGuid();
        var identityUserId = Guid.NewGuid().ToString();
        var content = "This is a valid comment.";
        var comment = Comment.Create(ratingId, identityUserId, content);

        var updatedContent = "This is the updated content.";
        var result = Comment.Update(comment.Value, updatedContent);

        Assert.True(result.Success);
        Assert.Equal(ratingId, result.Value.RatingId);
        Assert.Equal(identityUserId, result.Value.IdentityUserId);
        Assert.Equal(updatedContent, result.Value.Content);
    }

    [Theory(DisplayName = "Update should return failure result with invalid data")]
    [InlineData("", "O comentário do post não pode estar vazio")]
    [InlineData(null, "O comentário do post não pode estar vazio")]
    [InlineData(" ", "O comentário do post não pode estar vazio")]
    public void Update_WithInvalidData_ShouldReturnFailureResult(string invalidContent,
        string expectedErrorMessage)
    {
        var ratingId = Guid.NewGuid();
        var identityUserId = Guid.NewGuid().ToString();
        var content = "This is a valid comment.";
        var comment = Comment.Create(ratingId, identityUserId, content);

        var result = Comment.Update(comment.Value, invalidContent);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(expectedErrorMessage, result.Errors.Select(e => e.Description));
    }
}
