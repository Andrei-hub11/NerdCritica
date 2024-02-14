using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NerdCritica.Domain.Entities
{
    public class MovieRating
    {
        public Guid RatingId { get; private set; }
        public Guid MoviePostId { get; private set; }
        public Guid UserId { get; private set; }
        public decimal Rating { get; private set; }
        public string Commentary { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private MovieRating(Guid moviePostId, Guid userId, decimal rating, string commentary)
        {
            MoviePostId = moviePostId;
            UserId = userId;
            Rating = rating;
            Commentary = commentary;
        }

        public static MovieRating Create(Guid moviePostId, Guid userId, 
            decimal rating, string commentary)
        {
            return new MovieRating(moviePostId, userId, rating, commentary);
        }

        public static void Update(MovieRating movieRating, decimal rating, string commentary)
        {
            movieRating.Rating = rating;
            movieRating.Commentary = commentary;
        }
    }
}
