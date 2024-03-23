using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NerdCritica.Domain.DTOs.MappingsDapper;
using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.DTOs.User;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.ObjectValues;
using NerdCritica.Domain.Repositories.User;
using NerdCritica.Domain.Utils;
using NerdCritica.Infrastructure.Context;
using NerdCritica.Infrastructure.Extensions;

namespace NerdCritica.Infrastructure.Persistence.User;

public class UserRepository : IUserRepository
{
    private readonly DapperContext _dapperContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    public UserRepository(DapperContext dapperContext, UserManager<IdentityUser> userManager,
        IConfiguration configuration)
    {
        _dapperContext = dapperContext;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<Result<UserMapping>> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string selectQuery = "SELECT au.Id, au.IdentityUserId, an.UserName, an.Email, au.ProfileImagePath " +
                      "FROM ApplicationUsers au " +
                      "INNER JOIN AspNetUsers an ON au.IdentityUserId = an.Id " +
                      "WHERE au.IdentityUserId = @UserId";

        using (var connection = _dapperContext.CreateConnection())
        {
            var result = await connection.QueryFirstOrDefaultAsync<UserMapping>(new CommandDefinition(selectQuery,
                new { UserId = userId }, cancellationToken: cancellationToken));

            if (result == null)
            {
                return Result.Fail($"O usúario com id {userId} não foi encontrado",
                  new UserMapping());
            }

            return Result.Ok(result);
        }
    }

    public async Task<Result<UserMapping>> GetUserByEmailAsync(string userEmail, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string selectQuery = "SELECT au.Id, au.IdentityUserId, an.UserName, an.Email, au.ProfileImagePath " +
                      "FROM ApplicationUsers au " +
                      "INNER JOIN AspNetUsers an ON au.IdentityUserId = an.Id " +
                      "WHERE an.Email = @UserEmail";

        using (var connection = _dapperContext.CreateConnection())
        {
            var result = await connection.QueryFirstOrDefaultAsync<UserMapping>(new CommandDefinition(selectQuery,
                new { UserEmail = userEmail }, cancellationToken: cancellationToken));

            if (result == null)
            {
                return Result.Fail($"O usúario com email {userEmail} não foi encontrado",
                  new UserMapping());
            }

            return Result.Ok(result);
        }
    }
    
    public async Task<Result<string>> GetUserRoleAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string query = @"SELECT r.Name AS Role
        FROM AspNetUsers u
        JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        JOIN AspNetRoles r ON ur.RoleId = r.Id
        WHERE u.Id = @UserId";

        using (var connection = _dapperContext.CreateConnection())
        {
            var role = await connection.QueryFirstOrDefaultAsync<string>(new CommandDefinition(query,
                new { UserId = userId }, cancellationToken: cancellationToken));

            if (role == null)
            {
                return Result.Fail($"O role do usúario com id {userId} não foi encontrado", string.Empty);
            }

            return Result.Ok(role);
        }
    }

    public async Task<IEnumerable<FavoriteMovieMapping>> GetFavoriteMovies(string identityUserId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string query = @"SELECT mp.MoviePostId, mp.MovieImagePath, fm.CreatedAt, fm.FavoriteMovieId
        FROM MoviePost mp
        JOIN FavoriteMovie fm ON mp.MoviePostId = fm.MoviePostId
        WHERE fm.IdentityUserId = @IdentityUserId 
        ORDER BY fm.CreatedAt DESC";

        using (var connection = _dapperContext.CreateConnection())
        {
            var favoriteMovies = await connection.QueryAsync<FavoriteMovieMapping>(new CommandDefinition(query,
                new { IdentityUserId = identityUserId }, cancellationToken: cancellationToken));

            return favoriteMovies;
        }
    }

    public async Task<Result<UserCreationTokenAndId>> CreateUserAsync(ExtensionUserIdentity createUser)
    {
        var newIdentityUser = new IdentityUser { UserName = createUser.UserName, Email = createUser.Email };
        var newUser = await _userManager.CreateAsync(newIdentityUser, createUser.Password);

        if (!newUser.Succeeded)
        {
            foreach (var error in newUser.Errors)
            {
                if (error.Code == "DuplicateUserName")
                {
                    return Result.Fail("DuplicateUserName", new UserCreationTokenAndId(string.Empty, string.Empty));
                }
                else if (error.Code == "DuplicateEmail")
                {
                    return Result.Fail("DuplicateEmail", new UserCreationTokenAndId(string.Empty, string.Empty));
                }
                else
                {
                    return Result.Fail("Unknown", new UserCreationTokenAndId(string.Empty, string.Empty));
                }
            }
        }

        await _userManager.AddToRoleAsync(newIdentityUser, createUser.Roles[0]);
        var token = new CreateToken(_configuration).GenerateJwtToken(newIdentityUser, createUser.Roles);

        string query = @"INSERT INTO ApplicationUsers (IdentityUserId, ProfileImage, ProfileImagePath)
            OUTPUT INSERTED.IdentityUserId
            VALUES (@IdentityUserId, @ProfileImage, @ProfileImagePath)";

        using (var connection = _dapperContext.CreateConnection())
        {
            var newUserId = await connection.ExecuteScalarAsync<string>(query, new
            {
                IdentityUserId = newIdentityUser.Id,
                createUser.ProfileImage,
                createUser.ProfileImagePath,
            });
            return Result.Ok(new UserCreationTokenAndId(newUserId ?? string.Empty, token));
        }
    }

    public async Task<Result<UserLogin>> LoginUserAsync(UserLoginRequestDTO userLogin,
         CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var identityUser = await _userManager.FindByEmailAsync(userLogin.Email);

        if (identityUser == null)
        {
            return Result.Fail("Usuário não encontrado. Por favor, verifique suas credenciais e tente novamente.", new UserLogin(string.Empty, new UserMapping()));
        }

        if (!await _userManager.CheckPasswordAsync(identityUser, userLogin.Password))
        {
            return Result.Fail("Senha incorreta. Por favor, verifique suas credenciais e tente novamente.", new UserLogin(string.Empty, new UserMapping()));
        }

        var roles = await _userManager.GetRolesAsync(identityUser);
        var token = new CreateToken(_configuration).GenerateJwtToken(identityUser, roles);
        var user = await GetUserByIdAsync(identityUser.Id, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        return Result.Ok(new UserLogin(token, user.Value));
    }

    public async Task<bool> AddFavoriteMovieAsync(FavoriteMovie favoriteMovie, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string query = @"INSERT INTO FavoriteMovie (IdentityUserId, MoviePostId, CreatedAt) VALUES 
        (@IdentityUserId, @MoviePostId, @CreatedAt)";

        using (var connection = _dapperContext.CreateConnection())
        {
            await connection.QueryAsync(query, new { favoriteMovie.IdentityUserId, favoriteMovie.MoviePostId,
            favoriteMovie.CreatedAt});
            return true;
        }
    }

    public Task<MovieRatingResponseDTO> GetUserRatingAync(string userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<bool>> UpdateUserAsync(ExtensionUserIdentity userDTO, string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string updateQueryAspNetUsers = @"UPDATE AspNetUsers SET UserName = @UserName, Email = @Email
                                      WHERE Id = @UserId";

        string updateQueryApplicationUsers = @"UPDATE ApplicationUsers SET ProfileImage = @ProfileImage,
                                           ProfileImagePath = @ProfileImagePath
                                           WHERE IdentityUserId = @UserId";

        using (var connection = _dapperContext.CreateConnection())
        {

            await connection.ExecuteAsync(updateQueryAspNetUsers, new
            {
                userDTO.UserName,
                userDTO.Email,
                UserId = userId
            });

            await connection.ExecuteAsync(updateQueryApplicationUsers, new
            {
                userDTO.ProfileImage,
                userDTO.ProfileImagePath,
                UserId = userId
            });

            return Result.Ok(true);
        }
    }

    public async Task<bool> RemoveFavoriteMovie(Guid favoriteMovieId, string identityUserId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string query = @"DELETE FROM FavoriteMovie WHERE FavoriteMovieId = @FavoriteMovieId AND IdentityUserId = @IdentityUserId";

        using (var connection = _dapperContext.CreateConnection())
        {
            await connection.QueryAsync(query, new
            {
                FavoriteMovieId = favoriteMovieId,
                IdentityUserId = identityUserId
            });

            return true;
        }
    }
}
