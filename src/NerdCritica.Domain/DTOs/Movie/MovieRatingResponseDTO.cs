using NerdCritica.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdCritica.Domain.DTOs.Movie;

public record MovieRatingResponseDTO(
    Guid RatingId,
    Guid MoviePostId,
    string IdentityUserId,
    decimal Rating,
    IEnumerable<Comment> Comments,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
