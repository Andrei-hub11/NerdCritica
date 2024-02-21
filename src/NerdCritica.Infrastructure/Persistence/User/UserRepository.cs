using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using NerdCritica.Domain.DTOs.MappingsDapper;
using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.DTOs.User;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.ObjectValues;
using NerdCritica.Domain.Repositories.User;
using NerdCritica.Domain.Utils;
using NerdCritica.Domain.Utils.Exceptions;
using NerdCritica.Infrastructure.Context;
using NerdCritica.Infrastructure.Extensions;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
                    throw new CreateUserException("O nome de usuário já está em uso. Escolha outro nome de usuário.");
                }
                else if (error.Code == "DuplicateEmail")
                {
                    throw new CreateUserException("O e-mail já está em uso. Utilize outro endereço de e-mail.");
                }
                else
                {
                    throw new CreateUserException("Algo deu errado ao criar o usuário.");
                }
            }
        }

        await _userManager.AddToRoleAsync(newIdentityUser, createUser.Roles[0]);
            var token = new CreateToken(_configuration).GenerateJwtToken(newIdentityUser, createUser.Roles);

            string query = "INSERT INTO ApplicationUsers (IdentityUserId, ProfileImage, ProfileImagePath) " +
                "OUTPUT INSERTED.IdentityUserId " + // Adicionando OUTPUT para retornar o IdentityUserId inserido
                "VALUES (@IdentityUserId, @ProfileImage, @ProfileImagePath)";

            using (var connection = _dapperContext.CreateConnection())
            {
                var newUserId = await connection.ExecuteScalarAsync<string>(query, new
                {
                    IdentityUserId = newIdentityUser.Id,
                    ProfileImage = createUser.ProfileImage,
                    ProfileImagePath = createUser.ProfileImagePath,
                });
                return Result.Ok(new UserCreationTokenAndId(newUserId ?? string.Empty, token) ?? 
                    new UserCreationTokenAndId(string.Empty, string.Empty));
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
