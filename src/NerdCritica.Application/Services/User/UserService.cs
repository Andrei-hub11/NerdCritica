using AutoMapper;
using FluentEmail.Core;
using NerdCritica.Application.Services.EmailService;
using NerdCritica.Application.Services.Images;
using NerdCritica.Application.Services.Token;
using NerdCritica.Domain.DTOs.User;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.ObjectValues;
using NerdCritica.Domain.Repositories.Movies;
using NerdCritica.Domain.Repositories.User;
using NerdCritica.Domain.Utils;
using NerdCritica.Domain.Utils.Exceptions;
using System.Data;

namespace NerdCritica.Application.Services.User;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMoviePostRepository _moviePostRepository;
    private readonly IImagesService _imagesService;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    private readonly static string url = "https://localhost:7299";

    public UserService(IUserRepository userRepository, IMoviePostRepository moviePostRepository,
        IImagesService imagesService, ITokenService tokenService, IEmailService emailService, IMapper mapper)
    {
        _userRepository = userRepository;
        _moviePostRepository = moviePostRepository;
        _imagesService = imagesService;
        _tokenService = tokenService;
        _emailService = emailService;
        _mapper = mapper;
    }

    public async Task<ProfileUserResponseDTO> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);

            ThrowHelper.ThrowNotFoundExceptionIfNull(user, $"O usúario com id {userId} não foi encontrado");

            var userDTO = _mapper.Map<ProfileUserResponseDTO>(user);
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

            var favoriteMovieResponses = _mapper.Map<IEnumerable<FavoriteMovieResponseDTO>>(favoriteMovies)
                .ToList();

            return favoriteMovieResponses;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<AuthOperationResponseDTO> CreateUserAsync(CreateUserRequestDTO createUserRequestDTO,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _imagesService.GetProfileImageAsync(createUserRequestDTO.ProfileImage);

            var newUser = IdentityUserExtension.Create(createUserRequestDTO.UserName, createUserRequestDTO.Email,
                createUserRequestDTO.Password, result.ProfileImageBytes, result.ProfileImagePath, createUserRequestDTO.Roles);

            if (newUser.IsFailure)
            {
                var errorMessages = newUser.Errors.Select(error => error.Description).ToList();
                throw new ValidationException("Os campos de criação de usúario não foram preenchidos corretamente.",
                    errorMessages);
            }

            var userContextCreation = await _userRepository.CreateUserAsync(newUser.Value);

            if (userContextCreation.IsFailure)
            {
                var exception = CreateUserErrorHelper.GetExceptionFromResult(userContextCreation);
                    throw exception;
            }

            var userCreated = await _userRepository.GetUserByIdAsync(userContextCreation.Value, cancellationToken);

            ThrowHelper.ThrowNotFoundExceptionIfNull(userCreated, $"O usúario com id {userContextCreation.Value} não foi encontrado");

            var userDTO = _mapper.Map<ProfileUserResponseDTO>(userCreated);
            var role = await _userRepository.GetUserRoleAsync(userCreated.IdentityUserId, cancellationToken);

            ThrowHelper.ThrowNotFoundExceptionIfNull(role, $"O role do usúario com id {userCreated.IdentityUserId} não foi encontrado");
            
            var roleList = new List<string> { role };

            var token = _tokenService.GenerateJwtToken(userCreated, roleList);

            return new AuthOperationResponseDTO(token, userDTO, role);

        }
        catch (Exception)
        {
            throw;
        }

    }

    public async Task<bool> ForgotPasswordAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

            ThrowHelper.ThrowNotFoundExceptionIfNull(user, $"O usúario com email {email} não foi encontrado");

            var token = _tokenService.GeneratePasswordResetToken(user);
            await _userRepository.CreatePasswordResetTokenAsync(user.IdentityUserId, token);

            var resetLink = $"{url}/api/v1/account/verify-password-reset?token={token}&email={Uri.EscapeDataString(email)}";

            var emailObject = new EmailMetadata(email, "Aqui está o link para recuperar sua senha:",
                $"<a href=\"{resetLink}\">link</a>");

            await _emailService.Send(emailObject);

            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> VerifyResetPasswordAsync(VerifyResetPasswordRequestDTO request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);

        ThrowHelper.ThrowNotFoundExceptionIfNull(user, $"O usúario com email {request.Email} não foi encontrado");

        bool isTokenValid = _tokenService.ValidatePasswordResetToken(request.Token);

        var passwordResetTokenMapping = await _userRepository.GetPasswordResetTokenAsync(user.IdentityUserId, cancellationToken);

        ThrowHelper.ThrowNotFoundExceptionIfNull(passwordResetTokenMapping,
            $"O usuário com o email {request.Email} não possui um token de redefinição de senha. Por favor, solicite uma nova redefinição de senha.");

        if (!isTokenValid || passwordResetTokenMapping.ExpirationDate < DateTime.Now)
            return false;

        return true;
    }

    public async Task<AuthOperationResponseDTO> LoginUserAsync(UserLoginRequestDTO user,
        CancellationToken cancellationToken)
    {
        try
        {

            var validPassword = await _userRepository.CheckUserPasswordAsync(user, cancellationToken);

            if (validPassword.IsFailure)
            {
                if (validPassword.Errors[0].Description == "Senha incorreta. Por favor, verifique suas credenciais e tente novamente.")
                {
                    var errorMessages = validPassword.Errors.Select(error => error.Description).ToList();
                    throw new BadRequestException(string.Join(", ", errorMessages));
                }

                var notFoundErrors = validPassword.Errors.Select(error => error.Description).ToList();
                throw new NotFoundException(string.Join(", ", notFoundErrors));
            }

            var result = await _userRepository.GetUserByEmailAsync(user.Email, cancellationToken);

            ThrowHelper.ThrowNotFoundExceptionIfNull(result, $"O usúario com o email {user.Email} não foi encontrado");

            var userDTO = _mapper.Map<ProfileUserResponseDTO>(result);

            var role = await _userRepository.GetUserRoleAsync(result.IdentityUserId, cancellationToken);

            ThrowHelper.ThrowNotFoundExceptionIfNull(role, $"O role do usúario com id {result.IdentityUserId} não foi encontrado");

            var roleList = new List<string> { role };

            var token = _tokenService.GenerateJwtToken(result, roleList);

            return new AuthOperationResponseDTO(token, userDTO, role);

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

            ThrowHelper.ThrowNotFoundExceptionIfNull(user, $"O usúario com id {addFavoriteMovieRequestDTO.IdentityUserId} não foi encontrado");

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

            ThrowHelper.ThrowNotFoundExceptionIfNull(movie, $"O post de filme com o id {addFavoriteMovieRequestDTO.MoviePostId} não existe.");

            var favoriteMovieValited = FavoriteMovie.Create(addFavoriteMovieRequestDTO.MoviePostId,
                addFavoriteMovieRequestDTO.IdentityUserId);

            if (favoriteMovieValited.IsFailure)
            {
                var errorMessages = favoriteMovieValited.Errors.Select(error => error.Description).ToList();
                throw new ValidationException("Os campos de criação de usúario não foram preenchidos corretamente.",
                    errorMessages);
            }

            bool isCreate = await _userRepository.AddFavoriteMovieAsync(favoriteMovieValited.Value, cancellationToken);

            return isCreate;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ProfileUserResponseDTO> UpdateUserAsync(UpdateUserRequestDTO userDTO, string userId,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _imagesService.GetProfileImageAsync(userDTO.ProfileImage);

            var validatedUser = IdentityUserExtension.From(userDTO.Username, userDTO.Email,
                result.ProfileImageBytes, result.ProfileImagePath);

            if (validatedUser.IsFailure)
            {
                var errorMessages = validatedUser.Errors.Select(error => error.Description).ToList();
                throw new ValidationException("Os campos não preenchidos corretamente.",
                    errorMessages);
            }

            var userByEmail = await _userRepository.GetUserByEmailAsync(userDTO.Email, cancellationToken);

            ThrowHelper.ThrowNotFoundExceptionIfNull(userByEmail, $"O usúario com email {userDTO.Email} não foi encontrado");

            if (userByEmail.IdentityUserId != userId)
            {
                throw new ValidationException("Não é possivel atualizar para o email fornecido, " +
                    "pois já existe para outro usuário.");
            }

            var isUpdated = await _userRepository.UpdateUserAsync(validatedUser.Value, userId, cancellationToken);

            if (!isUpdated.Value)
            {
                throw new BadRequestException("A atualização do usúario falhou.");
            }

            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);

            ThrowHelper.ThrowNotFoundExceptionIfNull(user, $"O usúario com id {userId} não foi encontrado");

            var userUpdated = _mapper.Map<ProfileUserResponseDTO>(user);
            return userUpdated;

        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> UpdateUserPasswordAsync(UpdatePasswordRequestDTO request)
    {
        var errors = new Dictionary<string, List<string>>();

        var emailErrors = IdentityUserExtension.ValidateEmail(request.Email);
        if (emailErrors.Count > 0)
        {
            errors.Add("Email", emailErrors.Select(e => e.Description).ToList());
        }

        var passwordErrors = IdentityUserExtension.ValidatePassword(request.NewPassword);
        if (passwordErrors.Count > 0)
        {
            errors.Add("Password", passwordErrors.Select(e => e.Description).ToList());
        }

        if (errors.Count > 0)
        {
            throw new ValidationException("Um ou mais erro ocorreu", errors);
        }

        bool isReseted = await _userRepository.UpdateUserPassword(request.Email, request.NewPassword);

        return isReseted;
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
        }
        catch (Exception)
        {
            throw;
        }
    }
}
