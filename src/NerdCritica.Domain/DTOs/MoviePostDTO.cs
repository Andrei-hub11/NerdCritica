using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdCritica.Domain.DTOs;

public record MoviePostDTO(
    Guid MoviePostId,
    string MoviePostImagePath,
    string MoviePostTitle,
    string MoviePostDescription,
    decimal Rating,
    string Commentary,
    string Category,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

