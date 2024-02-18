using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdCritica.Domain.DTOs.Movie;

public record CreateMoviePost(string MoviePostImagePath,
    string MoviePostTitle,
    string MoviePostDescription, string Category);
