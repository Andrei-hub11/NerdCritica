

using NerdCritica.Domain.DTOs;

namespace NerdCritica.Application.Services.User;

public interface IUserService
{
    Task<UserDTO> GetUserByIdAsync(string userId, CancellationToken cancellationToken);
    Task<MovieRatingDTO> GetUserRatingAync(string userId, CancellationToken cancellationToken);
    Task<UserDTO> UpdateUserAsync(UserDTO userDTO, CancellationToken cancellationToken);
}
