using NerdCritica.Domain.DTOs.User;
using NerdCritica.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace NerdCritica.Domain.Entities;

public class MoviePost
{
  public Guid MoviePostId { get; private set; }
  public string CreatorUserId { get; private set; }
  public string MoviePostImagePath { get; private set; } = string.Empty;
  public byte[] MoviePostImage { get; private set; } = new byte[0];
  public string MoviePostTitle { get; private set; } = string.Empty;
  public string MoviePostDescription { get; private set; } = string.Empty;
  public string Category { get; private set; } = string.Empty;


    private MoviePost(Guid moviePostId, string creatorUserId, string profileImagePath, byte[] profileImage,
        string moviePostTitle, string moviePostDescription, string category)
    {
        MoviePostId = moviePostId;
        CreatorUserId = creatorUserId;
        MoviePostImagePath = profileImagePath;
        MoviePostImage = profileImage;
        MoviePostTitle = moviePostTitle;
        MoviePostDescription = moviePostDescription;
        Category = category;
    }

    public static Result<MoviePost> Create(Guid moviePostId, string creatorUserId, string moviePostImagePath, byte[] moviePostImage,
        string moviePostTitle, string moviePostDescription, string category)
    {
        var isCreate = true;
        var result = MoviePostValidation(moviePostImage, moviePostTitle, moviePostDescription, 
            category, isCreate, creatorUserId);

        if (result.Count > 0)
        {
            // Retorna um Result<MoviePost> com os erros, sem a necessidade de cast
            var emptyMoviePost = new MoviePost(Guid.Empty, string.Empty, string.Empty, 
                new byte[0], string.Empty, string.Empty, string.Empty);
            return Result.AddErrors(result, emptyMoviePost);
        }

        var moviePost = new MoviePost(moviePostId, creatorUserId, moviePostImagePath, moviePostImage,
            moviePostTitle, moviePostDescription, category);

        return Result.Ok(moviePost);
    }

    public static Result<MoviePost> Update(MoviePost moviePost, byte[] moviePostImage,
        string moviePostTitle, string moviePostDescription, string category)
    {
        var isCreate = false;
        var result = MoviePostValidation(moviePostImage, moviePostTitle, moviePostDescription,
            category, isCreate);

        if (result.Count > 0)
        {
            var emptyMoviePost = new MoviePost(Guid.Empty, string.Empty, string.Empty,
                new byte[0], string.Empty, string.Empty, string.Empty);
            return Result.AddErrors(result, emptyMoviePost);
        }

        moviePost.MoviePostTitle = moviePostTitle;
        moviePost.MoviePostImage = moviePostImage;
        moviePost.MoviePostDescription = moviePostDescription;
        moviePost.Category = category;

        return Result.Ok(moviePost);
    }

    private static List<Error> MoviePostValidation(byte[] moviePostImage,
        string moviePostTitle, string moviePostDescription, string category, 
     bool isCreate, string creatorUserId = "")
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
