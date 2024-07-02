using NerdCritica.Domain.Utils;

namespace NerdCritica.TestProject.Domain.ResultType;

public class ResultTypeTest
{
    [Fact]
    public void Fail_WithErrorMessage_ShouldReturnFailureResult()
    {
        // Arrange
        var errorMessage = "An error occurred.";

        var result = Result.Fail(errorMessage);

        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Equal(errorMessage, result.Errors[0].Description);
    }

    [Fact]
    public void Fail_WithErrorsList_ShouldReturnFailureResult()
    {
        // Arrange
        var errors = new List<Error>
        {
            new Error("Error 1"),
            new Error("Error 2")
        };

        var result = Result.Fail(errors);

        Assert.True(result.IsFailure);
        Assert.Equal(2, result.Errors.Count);
        Assert.Equal("Error 1", result.Errors[0].Description);
        Assert.Equal("Error 2", result.Errors[1].Description);
    }

    [Fact]
    public void Ok_WithValue_ShouldReturnSuccessResult()
    {
        var value = 42;

        var result = Result.Ok(value);

        Assert.False(result.IsFailure);
        Assert.Empty(result.Errors);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void ImplicitConversion_FromResultError_ShouldReturnFailureResult()
    {
        // Arrange
        var errorResult = Result.Fail(new List<Error> { new Error("Error 1") });

        var result = errorResult;

        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Equal("Error 1", result.Errors[0].Description);
    }
}
