
using NerdCritica.Domain.Entities;

namespace NerdCritica.Domain.TestProject.UserTests;

public class IdentityExtensionUserTests
{
    [Fact(DisplayName = "Create should return success result with valid data")]
    public void Create_WithValidData_ShouldReturnSuccessResult()
    {
        var identityUserId = Guid.NewGuid().ToString();
        var profileImage = new byte[1024]; // Imagem válida
        var profileImagePath = "path/to/profile.jpg";

        var result = User.Create(identityUserId, profileImage, profileImagePath);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(identityUserId, result.Value.IdentityUserId);
        Assert.Equal(profileImage, result.Value.ProfileImage);
        Assert.Equal(profileImagePath, result.Value.ProfileImagePath);
    }

    [Fact(DisplayName = "Create should return failure result with invalid data")]
    public void Create_WithInvalidData_ShouldReturnFailureResult()
    {
        var identityUserId = Guid.NewGuid().ToString();
        var invalidProfileImage = new byte[3 * 1024 * 1024]; // Imagem inválida (maior que 2MB)
        var profileImagePath = "path/to/profile.jpg";

        var result = User.Create(identityUserId, invalidProfileImage, profileImagePath);

        Assert.False(result.Success);
        Assert.Empty(result.Value.ProfileImage); 
        Assert.NotEmpty(result.Errors); 
        Assert.Contains("A imagem não pode ter mais que dois 2 megabytes de tamanho.", 
            result.Errors.Select(e => e.Description));
    }

    [Fact(DisplayName = "Create should return failure result with empty identityUserId")]
    public void Create_WithEmptyIdentityUserId_ShouldReturnFailureResult()
    {
        var identityUserId = ""; 
        var profileImage = new byte[1024]; 
        var profileImagePath = "path/to/profile.jpg";

        var result = User.Create(identityUserId, profileImage, profileImagePath);

        Assert.False(result.Success);
        Assert.Empty(result.Value.ProfileImage);
        Assert.NotNull(result.Errors);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("O id do usuário não pode estar vazio", 
            result.Errors.Select(e => e.Description));
    }

    [Fact(DisplayName = "Create should return failure result with invalid identityUserId")]
    public void Create_WithInvalidIdentityUserId_ShouldReturnFailureResult()
    {
        var invalidIdentityUserId = "invalidUserId"; 
        var profileImage = new byte[1024]; 
        var profileImagePath = "path/to/profile.jpg";

        var result = User.Create(invalidIdentityUserId, profileImage, profileImagePath);

        Assert.False(result.Success);
        Assert.Empty(result.Value.ProfileImage); 
        Assert.NotNull(result.Errors);
        Assert.NotEmpty(result.Errors); 
        Assert.Contains($"{invalidIdentityUserId} não é um id válido.", 
            result.Errors.Select(e => e.Description));
    }
}
