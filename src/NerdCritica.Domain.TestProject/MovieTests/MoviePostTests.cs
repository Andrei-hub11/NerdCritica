using NerdCritica.Domain.Entities;

namespace NerdCritica.Domain.TestProject.MovieTests;

public class MoviePostTests
{
    [Fact(DisplayName = "Create should return success result with valid data")]
    public void Create_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        Guid moviePostId = Guid.NewGuid();
        string creatorUserId = "3053CDB2-B6D6-416F-A646-B18EE6042E02";
        string moviePostImagePath = "path/to/image.jpg";
        byte[] moviePostImage = new byte[] { 0x00, 0x01, 0x02, 0x03 };
        string moviePostTitle = "Test Movie Post";
        string moviePostDescription = "Esta é uma descrição de postagem de filme de teste aaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.";
        string category = "Test Category";

        // Act
        var result = MoviePost.Create(moviePostId, creatorUserId, moviePostImagePath, moviePostImage,
            moviePostTitle, moviePostDescription, category);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(moviePostId, result.Value.MoviePostId);
        Assert.Equal(creatorUserId, result.Value.CreatorUserId);
        Assert.Equal(moviePostImagePath, result.Value.MoviePostImagePath);
        Assert.Equal(moviePostImage, result.Value.MoviePostImage);
        Assert.Equal(moviePostTitle, result.Value.MoviePostTitle);
        Assert.Equal(moviePostDescription, result.Value.MoviePostDescription);
        Assert.Equal(category, result.Value.Category);
    }

    [Fact(DisplayName = "Create should return failure result with invalid data")]
    public void Create_WithInvalidData_ShouldReturnFailureResult()
    {
        // Arrange
        Guid moviePostId = Guid.NewGuid();
        string creatorUserId = "creator123";
        string moviePostImagePath = "path/to/image.jpg";
        byte[] moviePostImage = new byte[0];
        string moviePostTitle = "";
        string moviePostDescription = "Short description";
        string category = "";

        // Act
        var result = MoviePost.Create(moviePostId, creatorUserId, moviePostImagePath, moviePostImage,
            moviePostTitle, moviePostDescription, category);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Contains($"{creatorUserId} não é um id válido.",
            result.Errors.Select(e => e.Description));
        Assert.Contains("A imagem do post deve ser fornecida.",
            result.Errors.Select(e => e.Description));
        Assert.Contains("O título do post não pode estar vazio", result.Errors.Select(e => e.Description));
        Assert.Contains("A descrição do post não pode ter menos de 100 caracteres", result.Errors.Select(e => e.Description));
        Assert.Contains("A categoria do post não pode estar vazia", result.Errors.Select(e => e.Description));
    }

    [Fact(DisplayName = "Update should return success result with valid data")]
    public void Update_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        var moviePostId = Guid.NewGuid();
        var creatorUserId = "3053CDB2-B6D6-416F-A646-B18EE6042E02";
        var moviePostImagePath = "path/to/image.jpg";
        var moviePostImage = new byte[1024];
        var moviePostTitle = "Updated Movie Post Title";
        var moviePostDescription = "Esta é uma atualização descrição de postagem de filme de teste" +
            " aaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.";
        var category = "Updated Category";

        var moviePost = MoviePost.Create(moviePostId, creatorUserId, moviePostImagePath, moviePostImage,
            "Original Title", "Esta é uma descrição de postagem de filme de teste aaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.", "Original Category");

        // Act
        var result = MoviePost.Update(moviePost.Value, moviePostImage, moviePostTitle,
            moviePostDescription, category);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(moviePostId, result.Value.MoviePostId);
        Assert.Equal(creatorUserId, result.Value.CreatorUserId);
        Assert.Equal(moviePostImagePath, result.Value.MoviePostImagePath);
        Assert.Equal(moviePostImage, result.Value.MoviePostImage);
        Assert.Equal(moviePostTitle, result.Value.MoviePostTitle);
        Assert.Equal(moviePostDescription, result.Value.MoviePostDescription);
        Assert.Equal(category, result.Value.Category);
    }

    [Fact(DisplayName = "Update should return failure result with invalid data")]
    public void Update_WithInvalidData_ShouldReturnFailureResult()
    {
        // Arrange
        var moviePostId = Guid.NewGuid();
        var creatorUserId = "creator123";
        var moviePostImagePath = "path/to/image.jpg";
        var moviePostImage = new byte[111124024];
        var moviePostTitle = "a";
        var moviePostDescription = "Short description";
        var category = "aaaaaaaa";

        var moviePost = MoviePost.Create(moviePostId, creatorUserId, moviePostImagePath, moviePostImage,
            "Original Title", "Original Description", "Original Category");

        // Act
        var result = MoviePost.Update(moviePost.Value, moviePostImage, "", "",
            "");

        // Assert
        Assert.False(result.Success);
        Assert.NotEqual(moviePostTitle, result.Value.MoviePostTitle);
        Assert.NotEqual(moviePostDescription, result.Value.MoviePostDescription);
        Assert.NotEqual(category, result.Value.Category);
        Assert.Contains("A imagem não pode ter mais que dois 2 megabytes de tamanho.",
            result.Errors.Select(e => e.Description));
        Assert.Contains("O título do post não pode estar vazio",
            result.Errors.Select(e => e.Description));
        Assert.Contains("A descrição do post não pode estar vazia",
            result.Errors.Select(e => e.Description));
        Assert.Contains("A categoria do post não pode estar vazia",
            result.Errors.Select(e => e.Description));
    }
}
