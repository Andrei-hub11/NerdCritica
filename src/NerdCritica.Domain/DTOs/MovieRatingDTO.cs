using NerdCritica.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdCritica.Domain.DTOs;

public record MovieRatingDTO(
    Guid RatingId,
    Guid MoviePostId,
    string UserId,
    decimal Rating,
    IEnumerable<Comment> Comments,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
