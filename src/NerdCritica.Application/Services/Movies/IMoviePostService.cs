﻿using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.Entities;

namespace NerdCritica.Application.Services.Movies
{
    public interface IMoviePostService
    {
        Task<IEnumerable<MoviePostResponseDTO>> GetMoviePostsAsync(CancellationToken token);
        Task<bool> UpdateMoviePostAsync(MoviePost moviePost);
        Task<bool> DeleteMoviePostAsync(string moviePostId);
    }
}
