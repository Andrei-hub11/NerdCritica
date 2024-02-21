
using AutoMapper;
using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.DTOs.User;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.ObjectValues;
using NerdCritica.Domain.Repositories.User;
using NerdCritica.Domain.Utils.Exceptions;
using Newtonsoft.Json;

namespace NerdCritica.Application.Services.User;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<AuthOperationResponseDTO> CreateUserAsync(CreateUserRequestDTO createUserRequestDTO, 
        string pathImage, CancellationToken cancellationToken)
    {
        try
        {
            var newUser = ExtensionUserIdentity.Create(createUserRequestDTO.UserName, createUserRequestDTO.Email, 
                createUserRequestDTO.Password, createUserRequestDTO.ProfileImage ?? new byte[0], pathImage, createUserRequestDTO.Roles);

            if (newUser.IsFailure)
            {
                var errorMessages = newUser.Errors.Select(error => error.Description).ToList();
                throw new ValidationException("Os campos de criação de usúario não preenchidos corretamente.",
                    errorMessages);
            }

            var userContextCreation =  await _userRepository.CreateUserAsync(newUser.Value);

            if (userContextCreation.IsFailure)
            {
                var errorMessages = newUser.Errors.Select(error => error.Description).ToList();
                throw new BadRequestException(string.Join(", ", errorMessages));
            }

            var userCreated = await _userRepository.GetUserByIdAsync(userContextCreation.Value.UserId, cancellationToken);

            if (userCreated.IsFailure)
            {
                var errorMessages = userCreated.Errors.Select(error => error.Description).ToList();
                throw new BadRequestException(string.Join(", ", errorMessages));
            }

            var userDTO = _mapper.Map<ProfileUserResponseDTO>(userCreated.Value);

            return new AuthOperationResponseDTO(userContextCreation.Value.Token, userDTO);

        }
         catch (Exception ex)
        {
            throw;
        }
       
    }

    public async Task<ProfileUserResponseDTO> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);

            if (user.IsFailure)
            {
                var errorMessages = user.Errors.Select(error => error.Description).ToList();
                throw new BadRequestException(string.Join(", ", errorMessages));
            }

            var userDTO = _mapper.Map<ProfileUserResponseDTO>(user.Value);
            return userDTO;
        } catch(Exception ex)
        {
            throw;
        }
    }

    public Task<MovieRatingResponseDTO> GetUserRatingAync(string userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<ProfileUserResponseDTO> UpdateUserAsync(ProfileUserResponseDTO userDTO, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
