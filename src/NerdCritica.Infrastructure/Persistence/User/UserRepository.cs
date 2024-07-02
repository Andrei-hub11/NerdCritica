using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NerdCritica.Domain.DTOs.MappingsDapper;
using NerdCritica.Domain.DTOs.User;
using NerdCritica.Domain.Entities;
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

    public async Task<UserMapping?> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string selectQuery = @"SELECT au.Id, au.IdentityUserId, an.UserName, an.Email, au.ProfileImagePath
                      FROM ApplicationUsers au 
                      INNER JOIN AspNetUsers an ON au.IdentityUserId = an.Id
                      WHERE au.IdentityUserId = @UserId";

        using (var connection = _dapperContext.CreateConnection())
        {
            var result = await connection.QueryFirstOrDefaultAsync<UserMapping>(new CommandDefinition(selectQuery,
                new { UserId = userId }, cancellationToken: cancellationToken));

            return result;
        }
    }

    public async Task<UserMapping?> GetUserByEmailAsync(string userEmail, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string selectQuery = @"SELECT au.Id, au.IdentityUserId, an.UserName, an.Email, au.ProfileImagePath
                      FROM ApplicationUsers au
                      INNER JOIN AspNetUsers an ON au.IdentityUserId = an.Id
                      WHERE an.Email = @UserEmail";

        using (var connection = _dapperContext.CreateConnection())
        {
            var result = await connection.QueryFirstOrDefaultAsync<UserMapping>(new CommandDefinition(selectQuery,
                new { UserEmail = userEmail }, cancellationToken: cancellationToken));

            return result;
        }
    }
    
    public async Task<string?> GetUserRoleAsync(string userId, CancellationToken cancellationToken)
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

            return role;
        }
    }

    public async Task<PasswordResetTokenMapping?> GetPasswordResetTokenAsync(string identityUserId, CancellationToken cancellationToken)
    {
        string query = @"SELECT * FROM PasswordResetTokens 
        WHERE IdentityUserId = @IdentityUserId AND ExpirationDate > @CurrentDate";

        using (var connection = _dapperContext.CreateConnection())
        {
            var passwordResetToken = await connection.QueryFirstOrDefaultAsync<PasswordResetTokenMapping>(new CommandDefinition(query, 
                new { IdentityUserId = identityUserId, CurrentDate = DateTime.UtcNow}, cancellationToken: cancellationToken));

            return passwordResetToken;
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

    private async Task<Result<IdentityUser>> CreateIdentityUserAsync(IdentityUserExtension createUser)
    {
        var newIdentityUser = new IdentityUser { UserName = createUser.UserName, Email = createUser.Email };
        var result = await _userManager.CreateAsync(newIdentityUser, createUser.Password);

        if (!result.Succeeded)
        {
            var errorCode = result.Errors.FirstOrDefault()?.Code;

            return errorCode switch
            {
                "DuplicateUserName" => Result.Fail("DuplicateUserName"),
                "DuplicateEmail" => Result.Fail("DuplicateEmail"),
                _ => Result.Fail("Unknown")
            };
        }

        return Result.Ok(newIdentityUser);
    }

    public async Task<Result<string>> CreateUserAsync(IdentityUserExtension createUser)
    {
        var newIdentityUser = await CreateIdentityUserAsync(createUser);

        if (newIdentityUser.IsFailure)
            return Result.Fail(newIdentityUser.Errors.ToList());

        await _userManager.AddToRoleAsync(newIdentityUser.Value, createUser.Roles[0]);

        string query = @"INSERT INTO ApplicationUsers (IdentityUserId, ProfileImage, ProfileImagePath)
            OUTPUT INSERTED.IdentityUserId
            VALUES (@IdentityUserId, @ProfileImage, @ProfileImagePath)";

        using (var connection = _dapperContext.CreateConnection())
        {
            var newUserId = await connection.ExecuteScalarAsync<string>(query, new
            {
                IdentityUserId = newIdentityUser.Value.Id,
                createUser.ProfileImage,
                createUser.ProfileImagePath,
            });

            if (newUserId == null)
                return Result.Fail("O id do usuário não foi retornado após criação.");

            return Result.Ok(newUserId);
        }
    }

    public async Task CreatePasswordResetTokenAsync(string identityUserId, string token)
    {
        string query = @"INSERT INTO PasswordResetTokens (IdentityUserId, Token, ExpirationDate) 
        VALUES (@IdentityUserId, @Token, @ExpirationDate)";

        using (var connection = _dapperContext.CreateConnection())
        {
            await connection.QueryAsync(query, new { IdentityUserId = identityUserId, 
                Token = token, ExpirationDate = DateTime.Now.AddHours(1) });
        }
    }

    public async Task<Result<bool>> CheckUserPasswordAsync(UserLoginRequestDTO userLogin,
         CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var identityUser = await _userManager.FindByEmailAsync(userLogin.Email);

        if (identityUser == null)
        {
            return Result.Fail("Usuário não encontrado. Por favor, verifique suas credenciais e tente novamente.");
        }

        if (!await _userManager.CheckPasswordAsync(identityUser, userLogin.Password))
        {
            return Result.Fail("Senha incorreta. Por favor, verifique suas credenciais e tente novamente.");
        }

        return Result.Ok(true);
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

    public async Task<Result<bool>> UpdateUserAsync(IdentityUserExtension userDTO, string userId, CancellationToken cancellationToken)
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

    public async Task<bool> UpdateUserPassword(string userEmail, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
            return false;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        return result.Succeeded;
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
