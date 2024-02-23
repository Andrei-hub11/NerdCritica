﻿using NerdCritica.Domain.DTOs.MappingsDapper;
using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.DTOs.User;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.ObjectValues;
using NerdCritica.Domain.Utils;

namespace NerdCritica.Domain.Repositories.User;

public interface IUserRepository
{
    Task<Result<UserMapping>> GetUserByIdAsync(string userId, CancellationToken cancellationToken);
    Task<Result<UserMapping>> GetUserByEmailAsync(string userEmail, CancellationToken cancellationToken);
    Task<Result<string>> GetUserRoleAsync(string userId, CancellationToken cancellationToken);
    Task<MovieRatingResponseDTO> GetUserRatingAync(string userId, CancellationToken cancellationToken);
    Task<Result<UserCreationTokenAndId>> CreateUserAsync(ExtensionUserIdentity createUserRequestDTO);
    Task<Result<UserLogin>> LoginUserAsync(UserLoginRequestDTO userLogin, CancellationToken cancellationToken);
    Task<Result<bool>> UpdateUserAsync(ExtensionUserIdentity userDTO, string userId, CancellationToken cancellationToken);
}
