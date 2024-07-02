using NerdCritica.Domain.Utils;

namespace NerdCritica.Domain.Entities;

public class MovieRating
{
    public Guid RatingId { get; private set; }
    public Guid MoviePostId { get; private set; }
    public string IdentityUserId { get; private set; } = string.Empty;
    public decimal Rating { get; private set; }

    private MovieRating(Guid moviePostId, string identityUserId, decimal rating)
    {
        MoviePostId = moviePostId;
        IdentityUserId = identityUserId;
        Rating = rating;
    }

    private MovieRating(string identityUserId, decimal rating)
    {
        IdentityUserId = identityUserId;
        Rating = rating;
    }

    public static Result<MovieRating> Create(Guid moviePostId, string identityUserId,
        decimal rating)
    {
        var isCreate = true;
        var result = MovieRatingValidation(rating, isCreate, identityUserId);

        if (result.Count > 0)
        {
            return Result.Fail(result);
        }

        var moviePost = new MovieRating(moviePostId, identityUserId, rating);

        return Result.Ok(moviePost);
    }

    public static Result<MovieRating> From(string identityUserId, decimal rating)
    {
        var isCreate = false;
        var result = MovieRatingValidation(rating, isCreate);

        if (result.Count > 0)
        {
            return Result.Fail(result);
        }

        var newRating = new MovieRating(identityUserId, rating);

        return Result.Ok(newRating);
    }

    private static List<Error> MovieRatingValidation(decimal rating, bool isCreate, 
        string identityUserId = "")
    {
        var errors = new List<Error>();
        if (rating > 5)
        {
            errors.Add(new Error("A avaliação não pode ser maior que 5."));
        }

        if (rating < 0)
        {
            errors.Add(new Error("A avaliação não pode ser menor que 0."));
        }

        if (isCreate && string.IsNullOrWhiteSpace(identityUserId))
        {
            errors.Add(new Error("O id do usuário não pode estar vazio"));
        }

        if (isCreate && !string.IsNullOrEmpty(identityUserId) &&
           !Guid.TryParse(identityUserId, out Guid result))
        {
            errors.Add(new Error($"{identityUserId} não é um id válido."));
        }

        return errors;
    }
}
