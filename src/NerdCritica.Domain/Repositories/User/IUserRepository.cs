using NerdCritica.Domain.DTOs.MappingsDapper;
using NerdCritica.Domain.DTOs.User;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.ObjectValues;
using NerdCritica.Domain.Utils;

namespace NerdCritica.Domain.Repositories.User;

public interface IUserRepository
{
    Task<UserMapping?> GetUserByIdAsync(string userId, CancellationToken cancellationToken);
    Task<UserMapping?> GetUserByEmailAsync(string userEmail, CancellationToken cancellationToken);
    Task<string?> GetUserRoleAsync(string userId, CancellationToken cancellationToken);
    Task<PasswordResetTokenMapping?> GetPasswordResetTokenAsync(string identityUserId, CancellationToken cancellationToken);
    Task<IEnumerable<FavoriteMovieMapping>> GetFavoriteMovies(string identityUserId, CancellationToken cancellationToken);
    Task<Result<string>> CreateUserAsync(IdentityUserExtension createUserRequestDTO);
    Task CreatePasswordResetTokenAsync(string identityUserId, string token);
    Task<Result<bool>> CheckUserPasswordAsync(UserLoginRequestDTO userLogin, CancellationToken cancellationToken);
    Task<bool> AddFavoriteMovieAsync(FavoriteMovie addFavoriteMovieRequestDTO, CancellationToken cancellationToken);
    Task<Result<bool>> UpdateUserAsync(IdentityUserExtension userDTO, string userId, CancellationToken cancellationToken);
    Task<bool> UpdateUserPassword(string userEmail, string newPassword);
    Task<bool> RemoveFavoriteMovie(Guid moviePostId, string identityUserId, CancellationToken cancellationToken);
}
