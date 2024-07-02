using NerdCritica.Domain.DTOs.User;
using NerdCritica.Domain.Utils;

namespace NerdCritica.Domain.Entities;

public class MoviePost
{
  public Guid MoviePostId { get; private set; }
  public string CreatorUserId { get; private set; } = string.Empty;
  public string MovieImagePath { get; private set; } = string.Empty;
  public string MovieBackdropImagePath { get; private set; } = string.Empty;
  public byte[] MovieImage { get; private set; } = new byte[0];
  public byte[] MovieBackdropImage { get; private set; } = new byte[0];
  public string MovieTitle { get; private set; } = string.Empty;
  public string MovieDescription { get; private set; } = string.Empty;
  public string MovieCategory { get; private set; } = string.Empty;
  public string Director { get; private set; } = string.Empty; 
  public DateTime ReleaseDate { get; private set; }
  public TimeSpan Runtime { get; private set; }


    private MoviePost(string creatorUserId,
                      string movieImagePath,
                      string movieBackdropImagePath,
                      byte[] movieImage,
                      byte[] movieBackdropImage,
                      string movieTitle,
                      string movieDescription,
                      string movieCategory,
                      string director,
                      DateTime releaseData,
                      TimeSpan runtime)
    {
        CreatorUserId = creatorUserId;
        MovieImagePath = movieImagePath;
        MovieBackdropImagePath = movieBackdropImagePath;
        MovieImage = movieImage;
        MovieBackdropImage = movieBackdropImage;
        MovieTitle = movieTitle;
        MovieDescription = movieDescription;
        MovieCategory = movieCategory;
        Director = director;
        ReleaseDate = releaseData;
        Runtime = runtime;
    }

    private MoviePost(
                      string movieImagePath,
                      string movieBackdropImagePath,
                      byte[] movieImage,
                      byte[] movieBackdropImage,
                      string movieTitle,
                      string movieDescription,
                      string movieCategory,
                      string director,
                      DateTime releaseData,
                      TimeSpan runtime)
    {
        MovieImagePath = movieImagePath;
        MovieBackdropImagePath = movieBackdropImagePath;
        MovieImage = movieImage;
        MovieBackdropImage = movieBackdropImage;
        MovieTitle = movieTitle;
        MovieDescription = movieDescription;
        MovieCategory = movieCategory;
        Director = director;
        ReleaseDate = releaseData;
        Runtime = runtime;
    }

    public static Result<MoviePost> Create(string creatorUserId, string movieImagePath, string movieBackdropPath,
        byte[] movieImage, byte[] movieBackdropImage,
        string movieTitle, string movieDescription, string category, string director, DateTime releaseDate,
        TimeSpan runtime)
    {
        var isCreate = true;
        var result = MoviePostValidation(movieImagePath, movieBackdropPath, movieImage, movieBackdropImage, 
            movieTitle, movieDescription, category, director, isCreate, creatorUserId);

        if (result.Count > 0)
        {
            return Result.Fail(result);
        }

        var moviePost = new MoviePost(creatorUserId, movieImagePath, movieBackdropPath, movieImage,
            movieBackdropImage, movieTitle, movieDescription, category, director, releaseDate, runtime);

        return Result.Ok(moviePost);
    }

    public static Result<MoviePost> From(string movieImagePath, string movieBackdropPath, byte[] movieImage, 
        byte[] movieBackdropImage, string movieTitle, string movieDescription, string movieCategory, 
        string director, DateTime releaseDate, TimeSpan runtime)
    {
        var isCreate = false;

        var result = MoviePostValidation(movieImagePath, movieBackdropPath, movieImage, movieBackdropImage, 
            movieTitle, movieDescription, movieCategory, director, isCreate);

        if (result.Count > 0)
        {
            return Result.Fail(result);
        }

        var moviePost = new MoviePost(movieImagePath, movieBackdropPath, movieImage,
           movieBackdropImage, movieTitle, movieDescription, movieCategory, director, releaseDate, runtime);

        return Result.Ok(moviePost);
    }

    private static List<Error> MoviePostValidation(string moviePostImagePath, string movieBackdropPath,
        byte[] moviePostImage, byte[] movieBackdropImage, string moviePostTitle, string moviePostDescription, 
        string category, string director, bool isCreate, string creatorUserId = "")
    {
        List<Error> errors = new List<Error>();

        if (moviePostImage == null || moviePostImage.Length == 0)
        {
            errors.Add(new Error("A imagem do post deve ser fornecida."));
        }

        if (moviePostImage?.Length > 2 * 1024 * 1024)
        {
            errors.Add(new Error("A imagem não pode ter mais que dois 2 megabytes de tamanho."));
        }

        if (movieBackdropImage == null || movieBackdropImage.Length == 0)
        {
            errors.Add(new Error("A imagem de fundo do post deve ser fornecida."));
        }

        if (movieBackdropImage?.Length > 2 * 1024 * 1024)
        {
            errors.Add(new Error("A imagem de fundo não pode ter mais que dois 2 megabytes de tamanho."));
        }

        if (string.IsNullOrWhiteSpace(moviePostImagePath))
        {
            errors.Add(new Error("A imagem do filme não pode estar vazia."));
        }

        if (string.IsNullOrWhiteSpace(movieBackdropPath)) {
            errors.Add(new Error("A imagem de fundo não pode estar vazia."));
        }

        if (string.IsNullOrWhiteSpace(moviePostTitle))
        {
            errors.Add(new Error("O título do post não pode estar vazio"));
        }

        if (string.IsNullOrWhiteSpace(moviePostDescription))
        {
            errors.Add(new Error("A descrição do post não pode estar vazia"));
        }

        if (moviePostDescription.Length < 100)
        {
            errors.Add(new Error("A descrição do post não pode ter menos de 100 caracteres"));
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            errors.Add(new Error("A categoria do post não pode estar vazia"));
        }

        if (string.IsNullOrWhiteSpace(director))
        {
            errors.Add(new Error("O diretor do filme não pode estar vazio"));
        }

        if (isCreate && string.IsNullOrWhiteSpace(creatorUserId))
        {
            errors.Add(new Error("O id do usuário não pode estar vazio"));
        }

        if (isCreate && !string.IsNullOrEmpty(creatorUserId) && 
            !Guid.TryParse(creatorUserId, out Guid result))
        {
            errors.Add(new Error($"{creatorUserId} não é um id válido."));
        }

        return errors;
    }

}
