using NerdCritica.Domain.Entities;

namespace NerdCritica.TestProject.Domain.MovieTests;

public class MoviePostTests
{
    [Fact(DisplayName = "Create should return success result with valid data")]
    public void Create_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        string creatorUserId = "3053CDB2-B6D6-416F-A646-B18EE6042E02";
        string movieImagePath = "path/to/image.jpg";
        string movieBackdropPath = "path/to/image.jpg";
        byte[] moviePostImage = new byte[] { 0x00, 0x01, 0x02, 0x03 };
        byte[] movieBackdropImage = new byte[] { 0x00, 0x01, 0x02, 0x03 };
        string moviePostTitle = "Test Movie Post";
        string moviePostDescription = "Esta é uma descrição de postagem de filme de teste aaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.";
        string category = "Test Category";
        string director = "John Storm";
        DateTime releaseDate = DateTime.Now;
        TimeSpan runTime = TimeSpan.Zero;

        // Act
        var result = MoviePost.Create(creatorUserId, movieImagePath, movieBackdropPath, moviePostImage,
           movieBackdropImage, moviePostTitle, moviePostDescription, category, director, releaseDate, runTime);

        // Assert
        Assert.False(result.IsFailure);
        Assert.NotNull(result.Value);
        Assert.Equal(creatorUserId, result.Value.CreatorUserId);
        Assert.Equal(movieImagePath, result.Value.MovieImagePath);
        Assert.Equal(movieBackdropPath, result.Value.MovieBackdropImagePath);
        Assert.Equal(moviePostImage, result.Value.MovieImage);
        Assert.Equal(movieBackdropImage, result.Value.MovieBackdropImage);
        Assert.Equal(moviePostTitle, result.Value.MovieTitle);
        Assert.Equal(moviePostDescription, result.Value.MovieDescription);
        Assert.Equal(category, result.Value.MovieCategory);
        Assert.Equal(director, result.Value.Director);
        Assert.Equal(releaseDate, result.Value.ReleaseDate);
        Assert.Equal(runTime, result.Value.Runtime);
    }

    [Fact(DisplayName = "Create should return failure result with invalid data")]
    public void Create_WithInvalidData_ShouldReturnFailureResult()
    {
        string creatorUserId = "creator123";
        string moviePostImagePath = "path/to/image.jpg";
        string movieBackdropPath = "path/to/image.jpg";
        byte[] moviePostImage = new byte[0];
        byte[] movieBackdropImage = new byte[0];
        string moviePostTitle = "";
        string moviePostDescription = "Short description";
        string category = "";
        string director = "";
        DateTime releaseDate = DateTime.Now;
        TimeSpan runTime = TimeSpan.Zero;

        var result = MoviePost.Create(creatorUserId, moviePostImagePath, movieBackdropPath, moviePostImage,
           movieBackdropImage, moviePostTitle, moviePostDescription, category, director, releaseDate, runTime);

        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Errors);
        Assert.Contains($"{creatorUserId} não é um id válido.",
            result.Errors.Select(e => e.Description));
        Assert.Contains("A imagem do post deve ser fornecida.",
            result.Errors.Select(e => e.Description));
        Assert.Contains("A imagem de fundo do post deve ser fornecida.",
           result.Errors.Select(e => e.Description));
        Assert.Contains("O título do post não pode estar vazio", result.Errors.Select(e => e.Description));
        Assert.Contains("A descrição do post não pode ter menos de 100 caracteres", result.Errors.Select(e => e.Description));
        Assert.Contains("A categoria do post não pode estar vazia", result.Errors.Select(e => e.Description));
        Assert.Contains("O diretor do filme não pode estar vazio", result.Errors.Select(e => e.Description));
    }

    [Fact(DisplayName = "Update should return success result with valid data")]
    public void Update_WithValidData_ShouldReturnSuccessResult()
    {
        string movieImagePath = "path/to/image.jpg";
        string movieBackdropPath = "path/to/image.jpg";
        var movieImage = new byte[1024];
        var movieBackdropImage = new byte[1024];
        var movieTitle = "Updated Movie Post Title";
        var movieDescription = "Esta é uma atualização descrição de postagem de filme de teste" +
            " aaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.";
        var category = "Updated Category";
        string director = "New diretor";
        DateTime releaseDate = DateTime.Now;
        TimeSpan runTime = TimeSpan.Zero;

        var result = MoviePost.From(movieImagePath, movieBackdropPath, movieImage, 
            movieBackdropImage, movieTitle, movieDescription, category, director, releaseDate, runTime);

        Assert.False(result.IsFailure);
        Assert.Equal(movieImagePath, result.Value.MovieImagePath);
        Assert.Equal(movieBackdropPath, result.Value.MovieBackdropImagePath);
        Assert.Equal(movieImage, result.Value.MovieImage);
        Assert.Equal(movieBackdropImage, result.Value.MovieBackdropImage);
        Assert.Equal(movieTitle, result.Value.MovieTitle);
        Assert.Equal(movieDescription, result.Value.MovieDescription);
        Assert.Equal(category, result.Value.MovieCategory);
        Assert.Equal(director, result.Value.Director);
        Assert.Equal(releaseDate, result.Value.ReleaseDate);
        Assert.Equal(runTime, result.Value.Runtime);
    }

    [Fact(DisplayName = "Update should return failure result with invalid data")]
    public void Update_WithInvalidData_ShouldReturnFailureResult()
    {
        var movieImage = new byte[111124024];
        var movieBackdropImage = new byte[111124024];
        DateTime releaseDate = DateTime.Now;
        TimeSpan runTime = TimeSpan.Zero;

        var result = MoviePost.From("", "", movieImage, movieBackdropImage,
            "", "", "", "", releaseDate, runTime);

       
        Assert.True(result.IsFailure);
        Assert.Contains("A imagem não pode ter mais que dois 2 megabytes de tamanho.",
            result.Errors.Select(e => e.Description));
        Assert.Contains("A imagem de fundo não pode ter mais que dois 2 megabytes de tamanho.",
            result.Errors.Select(e => e.Description));
        Assert.Contains("O título do post não pode estar vazio",
            result.Errors.Select(e => e.Description));
        Assert.Contains("A descrição do post não pode estar vazia",
            result.Errors.Select(e => e.Description));
        Assert.Contains("A categoria do post não pode estar vazia",
            result.Errors.Select(e => e.Description));
        Assert.Contains("O diretor do filme não pode estar vazio", 
            result.Errors.Select(e => e.Description));
    }
}
