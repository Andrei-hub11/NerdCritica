using Microsoft.Extensions.Configuration;
using Moq;
using NerdCritica.Application.Services.Token;
using System.Text;
using NerdCritica.Contracts.DTOs.MappingsDapper;


namespace NerdCritica.TestProject.Services.TokenServices;

public class TokenTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly TokenService _tokenService;
    private readonly string _testTokenSecret = "SuperSecretKeyForTesting";

    public TokenTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(config => config["TokenSecret"]).Returns(_testTokenSecret);

        _tokenService = new TokenService(_mockConfiguration.Object);
    }

    [Fact]
    public void GeneratePasswordResetToken_ValidUser_ReturnsToken()
    {
        // Arrange
        var user = new UserMapping { IdentityUserId = Guid.NewGuid().ToString() };

        var token = _tokenService.GeneratePasswordResetToken(user);

        Assert.NotNull(token);
        var parts = token.Split('.');
        Assert.Equal(2, parts.Length);
    }

    [Fact]
    public void ValidatePasswordResetToken_ValidToken_ReturnsTrue()
    {
        // Arrange
        var user = new UserMapping { IdentityUserId = Guid.NewGuid().ToString() };
        var token = _tokenService.GeneratePasswordResetToken(user);

        var isValid = _tokenService.ValidatePasswordResetToken(token);

        Assert.True(isValid);
    }

    [Fact]
    public void ValidatePasswordResetToken_InvalidToken_ReturnsFalse()
    {
        // Arrange
        string invalid = "invalid.token";
        byte[] bytes = Encoding.UTF8.GetBytes(invalid);
        string invalidToken = Convert.ToBase64String(bytes);

        var isValid = _tokenService.ValidatePasswordResetToken(invalidToken);

        Assert.False(isValid);
    }

    [Fact]
    public void ValidatePasswordResetToken_NullTokenSecret_ThrowsNullReferenceException()
    {
        // Arrange
        var user = new UserMapping { IdentityUserId = Guid.NewGuid().ToString() };
        var token = _tokenService.GeneratePasswordResetToken(user);

        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(config => config["TokenSecret"]).Returns(() => null); 

        var tokenService = new TokenService(mockConfiguration.Object);

        Assert.Throws<NullReferenceException>(() => tokenService.ValidatePasswordResetToken(token));
    }


    [Fact]
    public void GeneratePasswordResetToken_NullTokenSecret_ThrowsNullReferenceException()
    {
        // Arrange
        var user = new UserMapping { IdentityUserId = Guid.NewGuid().ToString() };

        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(config => config["TokenSecret"]).Returns(() => null);

        var tokenService = new TokenService(mockConfiguration.Object);

        Assert.Throws<NullReferenceException>(() => tokenService.GeneratePasswordResetToken(user));
    }
}
