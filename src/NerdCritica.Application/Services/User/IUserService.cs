using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.DTOs.User;

namespace NerdCritica.Application.Services.User;

public interface IUserService
{
    Task<ProfileUserResponseDTO> GetUserByIdAsync(string userId, CancellationToken cancellationToken);
    Task<MovieRatingResponseDTO> GetUserRatingAync(string userId, CancellationToken cancellationToken);
    Task<AuthOperationResponseDTO> CreateUserAsync(CreateUserRequestDTO createUserRequestDTO, 
        string pathImage, byte[] profileImageBytes, CancellationToken cancellationToken);
    Task<AuthOperationResponseDTO> LoginUserAsync(UserLoginRequestDTO user, CancellationToken cancellationToken);
    Task<ProfileUserResponseDTO> UpdateUserAsync(UpdateUserRequestDTO userDTO, string userId, string pathProfileImage,
        byte[] profileImageBytes, CancellationToken cancellationToken);
}
