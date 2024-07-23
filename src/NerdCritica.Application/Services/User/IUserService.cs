using NerdCritica.Contracts.DTOs.User;

namespace NerdCritica.Application.Services.User;

public interface IUserService
{
    Task<ProfileUserResponseDTO> GetUserByIdAsync(string userId, CancellationToken cancellationToken);
    Task<ICollection<FavoriteMovieResponseDTO>> GetFavoriteMovies(string identityUserId, CancellationToken cancellationToken);
    Task<bool> ForgotPasswordAsync(string email, CancellationToken cancellationToken);
    Task<bool> VerifyResetPasswordAsync(VerifyResetPasswordRequestDTO request, CancellationToken cancellationToken);
    Task<AuthOperationResponseDTO> CreateUserAsync(CreateUserRequestDTO createUserRequestDTO, CancellationToken cancellationToken);
    Task<AuthOperationResponseDTO> LoginUserAsync(UserLoginRequestDTO user, CancellationToken cancellationToken);
    Task<bool> AddFavoriteMovieAsync(AddFavoriteMovieRequestDTO addFavoriteMovieRequestDTO, 
        CancellationToken cancellationToken);
    Task<ProfileUserResponseDTO> UpdateUserAsync(UpdateUserRequestDTO userDTO, string userId, CancellationToken cancellationToken);
    Task<bool> UpdateUserPasswordAsync(UpdatePasswordRequestDTO request);
    Task<bool> RemoveFavoriteMovie(Guid favoriteMovieId, string identityUserId, CancellationToken cancellationToken);
}
