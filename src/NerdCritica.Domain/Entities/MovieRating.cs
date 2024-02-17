
namespace NerdCritica.Domain.Entities;

public class MovieRating
{
    public Guid RatingId { get; private set; }
    public Guid MoviePostId { get; private set; }
    public string IdentityUserId { get; private set; }
    public decimal Rating { get; private set; }

    private MovieRating(Guid moviePostId, string userId, decimal rating)
    {
        MoviePostId = moviePostId;
        IdentityUserId = userId;
        Rating = rating;
    }

    public static MovieRating Create(Guid moviePostId, string identityUserId, 
        decimal rating)
    {
        return new MovieRating(moviePostId, identityUserId, rating);
    }

    public static void Update(MovieRating movieRating, decimal rating, string commentary)
    {
        movieRating.Rating = rating;
    }
}
