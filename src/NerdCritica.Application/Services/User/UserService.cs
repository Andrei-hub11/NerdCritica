
using AutoMapper;
using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.DTOs.User;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.Repositories.Movies;
using NerdCritica.Domain.Repositories.User;
using NerdCritica.Domain.Utils;
using NerdCritica.Domain.Utils.Exceptions;


namespace NerdCritica.Application.Services.User;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMoviePostRepository _moviePostRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMoviePostRepository moviePostRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _moviePostRepository = moviePostRepository;
        _mapper = mapper;
    }

    public async Task<AuthOperationResponseDTO> CreateUserAsync(CreateUserRequestDTO createUserRequestDTO,
        string pathImage, byte[] profileImageBytes, CancellationToken cancellationToken)
    {
        try
        {
            var newUser = ExtensionUserIdentity.Create(createUserRequestDTO.UserName, createUserRequestDTO.Email,
                createUserRequestDTO.Password, profileImageBytes, pathImage, createUserRequestDTO.Roles);

            if (newUser.IsFailure)
            {
                var errorMessages = newUser.Errors.Select(error => error.Description).ToList();
                throw new ValidationException("Os campos de criação de usúario não foram preenchidos corretamente.",
                    errorMessages);
            }

            var userContextCreation = await _userRepository.CreateUserAsync(newUser.Value);

            if (userContextCreation.IsFailure || userContextCreation.Value.UserId == string.Empty)
            {
                var exception = ExceptionMapper.GetExceptionFromResult(userContextCreation);
                if (exception != null)
                {
                    throw exception;
                }
            }

            var userCreated = await _userRepository.GetUserByIdAsync(userContextCreation.Value.UserId, cancellationToken);

            if (userCreated.IsFailure)
            {
                var errorMessages = userCreated.Errors.Select(error => error.Description).ToList();
                throw new BadRequestException(string.Join(", ", errorMessages));
            }

            var userDTO = _mapper.Map<ProfileUserResponseDTO>(userCreated.Value);
            var role = await _userRepository.GetUserRoleAsync(userCreated.Value.IdentityUserId, cancellationToken);

            if (role.Value == string.Empty)
            {
                var errorMessages = role.Errors.Select(error => error.Description).ToList();
                throw new BadRequestException(string.Join(", ", errorMessages));
            }

            return new AuthOperationResponseDTO(userContextCreation.Value.Token, userDTO, role.Value);

        }
        catch (Exception)
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
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ICollection<FavoriteMovieResponseDTO>> GetFavoriteMovies(string identityUserId, CancellationToken cancellationToken)
    {
       try
        {
            var favoriteMovies = await _userRepository.GetFavoriteMovies(identityUserId, cancellationToken);

            if (!favoriteMovies.Any())
            {
                return Enumerable.Empty<FavoriteMovieResponseDTO>().ToList();
            }

            var favoriteMovieResponses = _mapper.Map<IEnumerable<FavoriteMovieResponseDTO>>(favoriteMovies)
                .ToList();

            return favoriteMovieResponses;
        } catch (Exception)
        {
            throw;
        }
    }

    public async Task<AuthOperationResponseDTO> LoginUserAsync(UserLoginRequestDTO user,
        CancellationToken cancellationToken)
    {
        try
        {

            var userContextLogin = await _userRepository.LoginUserAsync(user, cancellationToken);

            if (userContextLogin.IsFailure)
            {
                if (userContextLogin.Errors[0].Description == "Senha incorreta. Por favor, verifique suas credenciais e tente novamente.")
                {
                    var errorMessages = userContextLogin.Errors.Select(error => error.Description).ToList();
                    throw new BadRequestException(string.Join(", ", errorMessages));
                }

                var notFoundErrors = userContextLogin.Errors.Select(error => error.Description).ToList();
                throw new NotFoundException(string.Join(", ", notFoundErrors));
            }

            var userDTO = _mapper.Map<ProfileUserResponseDTO>(userContextLogin.Value.User);

            var role = await _userRepository.GetUserRoleAsync(userContextLogin.Value.User.IdentityUserId, cancellationToken);

            if (role.Value == string.Empty)
            {
                var errorMessages = role.Errors.Select(error => error.Description).ToList();
                throw new BadRequestException(string.Join(", ", errorMessages));
            }

            return new AuthOperationResponseDTO(userContextLogin.Value.Token, userDTO, role.Value);

        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> AddFavoriteMovieAsync(AddFavoriteMovieRequestDTO addFavoriteMovieRequestDTO, 
        CancellationToken cancellationToken)
    {
       try
        {
            var user = await _userRepository.GetUserByIdAsync(addFavoriteMovieRequestDTO.IdentityUserId,
                cancellationToken);

            if (user.IsFailure)
            {
                var errorMessages = user.Errors.Select(error => error.Description).ToList();
                throw new NotFoundException(string.Join(", ", errorMessages));
            }

            var favoriteMovie = await _userRepository.GetFavoriteMovies(addFavoriteMovieRequestDTO.IdentityUserId,
                cancellationToken);

            bool favoriteMovieDuplicated = favoriteMovie.Any(fm => fm.MoviePostId ==
            addFavoriteMovieRequestDTO.MoviePostId);

            if (favoriteMovieDuplicated)
            {
                throw new BadRequestException($"Não foi adicionar o filme com id " +
                    $"{addFavoriteMovieRequestDTO.MoviePostId} como favorito, pois já está favoritado.");
            }

            var movie = await _moviePostRepository.GetMoviePostByIdAsync(addFavoriteMovieRequestDTO.MoviePostId,
                cancellationToken);

            if (movie == null)
            {
                throw new NotFoundException($"O post de filme com o id {addFavoriteMovieRequestDTO.MoviePostId} não existe.");
            }

            var favoriteMovieValited = FavoriteMovie.Create(addFavoriteMovieRequestDTO.MoviePostId, 
                addFavoriteMovieRequestDTO.IdentityUserId);

            bool isCreate = await _userRepository.AddFavoriteMovieAsync(favoriteMovieValited.Value, cancellationToken);

            return isCreate;
        } catch (Exception)
        {
            throw;
        }
    }

    public Task<MovieRatingResponseDTO> GetUserRatingAync(string userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<ProfileUserResponseDTO> UpdateUserAsync(UpdateUserRequestDTO userDTO, string userId, string pathProfileImage,
        byte[] profileImageBytes, CancellationToken cancellationToken)
    {
        try
        {
          var validatedUser =  ExtensionUserIdentity.From(userDTO.Username, userDTO.Email, 
                profileImageBytes, pathProfileImage);

            if (validatedUser.IsFailure)
            {
                var errorMessages = validatedUser.Errors.Select(error => error.Description).ToList();
                throw new ValidationException("Os campos de criação de usúario não preenchidos corretamente.",
                    errorMessages);
            }

            var userByEmail = await _userRepository.GetUserByEmailAsync(userDTO.Email, cancellationToken);

            if (userByEmail.Success && userByEmail.Value.IdentityUserId != userId)
            {
                throw new ValidationException("Não é possivel atualizar para o email fornecido, " +
                    "pois já existe para outro usuário.");
            }

            if (userByEmail.IsFailure)
            {
                var errorMessages = userByEmail.Errors.Select(error => error.Description).ToList();
                throw new BadRequestException(string.Join(", ", errorMessages));
            }

            var isUpdated = await _userRepository.UpdateUserAsync(validatedUser.Value, userId, cancellationToken);

            if (!isUpdated.Value)
            {
              throw new BadRequestException("A atualização do usúario falhou.");
            }

            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);

            if (user.IsFailure)
            {
                var errorMessages = user.Errors.Select(error => error.Description).ToList();
                throw new BadRequestException(string.Join(", ", errorMessages));
            }

            var userUpdated = _mapper.Map<ProfileUserResponseDTO>(user.Value);
            return userUpdated;

        } catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> RemoveFavoriteMovie(Guid favoriteMovieId, string identityUserId, CancellationToken cancellationToken)
    {
       try
        {
            var favoriteMovies = await _userRepository.GetFavoriteMovies(identityUserId, cancellationToken);

            bool favoriteMovieExist = favoriteMovies.Any(fm =>
            fm.FavoriteMovieId == favoriteMovieId);

            if (!favoriteMovieExist)
            {
                throw new NotFoundException($"O filme favoritado com o id {favoriteMovieId} não existe.");
            }

            bool isRemoved = await _userRepository.RemoveFavoriteMovie(favoriteMovieId, identityUserId, cancellationToken);

            return isRemoved;
        } catch (Exception)
        {
            throw;
        }
    }
}
