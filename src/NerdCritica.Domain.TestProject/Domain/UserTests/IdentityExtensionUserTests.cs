using NerdCritica.Domain.Entities;

namespace NerdCritica.TestProject.Domain.UserTests;

public class IdentityExtensionUserTests
{
    public static readonly List<string> ValidRoles = new List<string> { "ah" };


    [Theory(DisplayName = "Create should return success result with valid data")]
    [InlineData("Andy", "example@example.com", "Strong^Password1^", "profile.jpg", new string[] { "Moderator" })]
    [InlineData("Andy", "example@example.com", "Strong^Password1*", "profile.jpg", new string[] { "User" })]
    [InlineData("Andy", "example@example.com", "Strong!Password1^", "profile.jpg", new string[] { "Admin" })]
    [InlineData("Andy", "example@example.com", "StrongPassword1##", "profile.jpg", new string[] { "Admin" })]
    public void Create_WithValidData_ShouldReturnSuccessResult(string userName, string email, string password,
        string profileImagePath, string[] roles)
    {
        var profileImage = new byte[1];

        var result = ExtensionUserIdentity.Create(userName, email, password, profileImage,
            profileImagePath, roles.ToList());

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(email, result.Value.Email);
        Assert.Equal(profileImagePath, result.Value.ProfileImagePath);
        Assert.Equal(profileImage, result.Value.ProfileImage);
        Assert.NotEqual(DateTime.MinValue, result.Value.LastAccessDate);
    }

    [Theory(DisplayName = "Create should return failure result with invalid data")]
    [MemberData(nameof(GetInvalidUserTestData))]
    public void Create_WithInvalidData_ShouldReturnFailureResult(string userName, string email, string password,
        byte[] profileImage, string profileImagePath, string expectedErrorMessage, string[] roles)
    {
        var result = ExtensionUserIdentity.Create(userName, email, password, profileImage, profileImagePath,
            roles.ToList());

        Assert.False(result.Success);
        Assert.Empty(result.Value.Email);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(expectedErrorMessage, result.Errors.Select(e => e.Description));
    }

    public static IEnumerable<object[]> GetInvalidUserTestData()
    {
        yield return new object[] { "Andy", "", "StrongPassword1", new byte[1], "profile.jpg", "O email é obrigatório.", new string[] { "Moderator" } };
        yield return new object[] { "Andy", "invalidemail", "StrongPassword1", new byte[1], "profile.jpg", "Email inválido.", new string[] { "Moderator" } };
        yield return new object[] { "Andy", "example@example.com", "", new byte[1], "profile.jpg", "A senha é obrigatória.", new string[] { "Moderator" } };
        yield return new object[] {"Andy", "example@example.com", "weak", new byte[1], "profile.jpg", "A senha deve ter pelo menos oito caracteres.",
        new string[] {"Moderator" } };
        yield return new object[] {"Andy", "example@example.com", "WeakPassword", new byte[1], "profile.jpg", "Senha inválida. A senha deve ter pelo menos dois caracteres especiais.",
        new string[] {"Moderator" } };
        yield return new object[] {"Andy", "example@example.com", "StrongPassword1", new byte[2 * 1024 * 1024 + 1], "profile.jpg", "A imagem não pode ter mais que dois 2 megabytes de tamanho.",
        new string[] {"Moderator" } };
        yield return new object[] {"Andy", "example@example.com", "StrongPassword1", new byte[1], "profile.jpg", "O role fornecido não é válido.",
        new string[] {"InvalidRole" } };
    }
}
