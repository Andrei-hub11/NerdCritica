
using NerdCritica.Domain.Utils;

namespace NerdCritica.Domain.Entities;

public class FavoriteMovie
{
    public Guid MoviePostId { get; private set; }
    public string IdentityUserId { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private FavoriteMovie(Guid moviePostId, string userId) { 
        MoviePostId = moviePostId;
        IdentityUserId = userId;
        CreatedAt = DateTime.Now;
    }

    public static Result<FavoriteMovie> Create(Guid moviePostId, string identityUserId)
    {
        var result = FavoriteMovieValidation(moviePostId, 
            identityUserId);

        if (result.Count > 0)
        {
            var emptyFavoriteMovie = new FavoriteMovie(Guid.Empty, string.Empty);
            return Result.AddErrors(result, emptyFavoriteMovie);
        }

        var favoriteMovie = new FavoriteMovie(moviePostId, identityUserId);

        return Result.Ok(favoriteMovie);
    }

    private static List<Error> FavoriteMovieValidation(
        Guid moviePostId, string identityUserId)
    {
        var errors = new List<Error>();

        if (moviePostId == Guid.Empty)
        {
            errors.Add(new Error("O id do post não pode estar vazio"));
        }

        if (string.IsNullOrWhiteSpace(identityUserId))
        {
            errors.Add(new Error("O id do usuário não pode estar vazio"));
        }

        if (!string.IsNullOrEmpty(identityUserId) &&
           !Guid.TryParse(identityUserId, out Guid result))
        {
            errors.Add(new Error($"{identityUserId} não é um id válido."));
        }

        return errors;
    }
}
