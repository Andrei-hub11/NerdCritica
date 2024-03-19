

using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using NerdCritica.Application.Services.User;
using NerdCritica.Domain.DTOs.MappingsDapper;
using NerdCritica.Domain.DTOs.User;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.ObjectValues;
using NerdCritica.Domain.Repositories.User;
using NerdCritica.Domain.Utils;
using NerdCritica.Domain.Utils.Exceptions;

namespace NerdCritica.TestProject.Controllers.UserAccount;

public class UserAccountTests
{

    [Fact]
    public async Task GetUserByIdAsync_UserFound_ReturnsUserProfile()
    {
        var IdentityUserId = "user123";
        var UserId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        var userRepositoryMock = new Mock<IUserRepository>();
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserMapping, ProfileUserResponseDTO>();
        });

        var mapper = new Mapper(configuration);

        var expectedUser = new UserMapping { Id = UserId, IdentityUserId = IdentityUserId, 
            UserName = "testUser", Email = "test@example.com", ProfileImagePath = "profile.jpg" };

        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(IdentityUserId, cancellationToken))
                          .ReturnsAsync(Result.Ok(expectedUser));

        var expectedProfileDTO = new ProfileUserResponseDTO(Id: UserId, IdentityUserId: IdentityUserId,
            UserName: "testUser", Email: "test@example.com", ProfileImagePath: "profile.jpg");
      
        var userService = new UserService(userRepositoryMock.Object, mapper);

        var result = await userService.GetUserByIdAsync(IdentityUserId, cancellationToken);

        Assert.NotNull(result);
        Assert.Equal(IdentityUserId, result.IdentityUserId);
        Assert.Equal("testUser", result.UserName);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("profile.jpg", result.ProfileImagePath);
    }

    [Fact]
    public void GetUserByIdAsync_UserNotFound_ThrowsBadRequestException()
    {
        var userId = "nonExistentUser";
        var cancellationToken = CancellationToken.None;
        var userRepositoryMock = new Mock<IUserRepository>();
        var mapperMock = new Mock<IMapper>();
        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId, cancellationToken))
                          .ReturnsAsync(Result.Fail($"O usúario com id {userId} não foi encontrado", 
                          new UserMapping()));
        var userService = new UserService(userRepositoryMock.Object, mapperMock.Object);

        Assert.ThrowsAsync<BadRequestException>(() => userService.GetUserByIdAsync(userId, cancellationToken));
    }

    [Theory]
    [MemberData(nameof(SuccessLoginTestData))]
    public async Task CreateUserAsync_Success_ReturnsAuthOperationResponseDTO(
        string userName, string email, string password, string profileImage, string pathImage, string[] roles,
        string expectedUserId, string expectedToken, string expectedUserName, string expectedEmail, string expectedRole)
    {
        // Arrange
        var createUserRequestDTO = new CreateUserRequestDTO
        (
            UserName: userName,
            Email: email,
            Password: password,
            ProfileImage: profileImage,
            Roles: roles.ToList()
        );
        var cancellationToken = CancellationToken.None;
        var idGuid = Guid.NewGuid();
        byte[] profileImageBytes = new byte[0];

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<ExtensionUserIdentity>()))
                          .ReturnsAsync(Result.Ok(new UserCreationTokenAndId(expectedUserId, expectedToken)));

        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(expectedUserId, cancellationToken))
                         .ReturnsAsync(Result.Ok(new UserMapping
                         {
                             Id = idGuid,
                             IdentityUserId = expectedUserId, // Assuming IdentityUserId is same as expectedUserId
                             UserName = expectedUserName,
                             Email = expectedEmail
                         }));

        userRepositoryMock.Setup(repo => repo.GetUserRoleAsync(expectedUserId, cancellationToken))
            .ReturnsAsync(Result.Ok(expectedRole));

        var imagePath = "profile.jpg";
        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(mapper => mapper.Map<ProfileUserResponseDTO>(It.IsAny<UserMapping>()))
                  .Returns(new ProfileUserResponseDTO (Id: idGuid, IdentityUserId: expectedUserId,
                  UserName: expectedUserName,
                  Email: expectedEmail, ProfileImagePath: imagePath));

        var userService = new UserService(userRepositoryMock.Object, mapperMock.Object);

        // Act
        var result = await userService.CreateUserAsync(createUserRequestDTO, pathImage, profileImageBytes,
            cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedToken, result.Token);
        Assert.NotNull(result.User);
        bool isGuid = Guid.TryParse(result.User.Id.ToString(), out Guid convert);
        Assert.True(isGuid);
        Assert.Equal(expectedUserId, result.User.IdentityUserId);
        Assert.Equal(expectedUserName, result.User.UserName);
        Assert.Equal(expectedEmail, result.User.Email);
        Assert.Equal(expectedRole.ToList(), result.Role);
    }

    public static IEnumerable<object[]> SuccessLoginTestData =>
        new List<object[]>
        {
            new object[] { "testUser", "test@example.com", "password123^^", "base64", "path/to/image", 
                new[] { "User" }, "userId123", "token123", "testUser", "test@example.com", "User" },
            new object[] { "testUser2", "test123@example.com", "password123##", "base64", "path/to/image",
                new[] { "Moderator" }, "userId123", "token123", "testUser", "test123@example.com", "Moderator" },
            new object[] { "testUser3", "test631@example.com", "password123$*", "base64", "path/to/image",
                new[] { "Admin" }, "userId123", "token123", "testUser", "test631@example.com", "Admin" }
        };

    [Theory]
    [MemberData(nameof(FailureLoginTestData))]
    public async Task CreateUserAsync_Failure_ThrowsCreateUserException(string errorCode, string expectedErrorMessage)
    {
        // Arrange
        var createUserRequestDTO = new CreateUserRequestDTO
        (
            UserName: "testUser",
            Email: "test@example.com",
            Password: "password123^^",
            ProfileImage: "base64",
            Roles: new[] { "User" }.ToList()
        );
        var pathImage = "path/to/image";
        byte[] profileImageBytes = new byte[0];
        var cancellationToken = CancellationToken.None;

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<ExtensionUserIdentity>()))
                          .ReturnsAsync(Result.Fail(errorCode, new UserCreationTokenAndId(string.Empty, string.Empty)));

        var userService = new UserService(userRepositoryMock.Object, Mock.Of<IMapper>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CreateUserException>(() =>
            userService.CreateUserAsync(createUserRequestDTO, pathImage, profileImageBytes, cancellationToken));

        Assert.Equal(expectedErrorMessage, exception.Message);
    }

    public static IEnumerable<object[]> FailureLoginTestData =>
       new List<object[]>
       {
            new object[] { "DuplicateUserName", "O nome de usuário já está em uso. Escolha outro nome de usuário." },
            new object[] { "DuplicateEmail", "O e-mail já está em uso. Utilize outro endereço de e-mail." },
            new object[] { "Unknown", "Algo deu errado ao criar o usuário." }
       };

    [Fact]
    public async Task LoginUserAsync_UserFound_ReturnsAuthOperationResponseDTO()
    {
        // Arrange
        var userLoginRequestDTO = new UserLoginRequestDTO( Email: "test@example.com", Password: "password123");
        var cancellationToken = CancellationToken.None;
        var UserId = Guid.NewGuid();
        var IdentityUserId = "user123";
        var imagePath = "profile.jpg";
        var userRepositoryMock = new Mock<IUserRepository>();
        var mapperMock = new Mock<IMapper>();
        var userManagerMock = new Mock<UserManager<IdentityUser>>();
        var configurationMock = new Mock<IConfiguration>();
        var expectedUser = new IdentityUser { Id = IdentityUserId, UserName = "testUser", 
            Email = "test@example.com" };
        var expectedUserMapping = new UserMapping { Id = UserId, IdentityUserId = IdentityUserId, UserName = "testUser", 
            Email = "test@example.com" };
        var expectedToken = "token123";
        var expectedRole = "UserRole";
        userRepositoryMock.Setup(repo => repo.LoginUserAsync(userLoginRequestDTO, cancellationToken))
                         .ReturnsAsync(Result.Ok(new UserLogin(expectedToken, expectedUserMapping)));
        mapperMock.Setup(mapper => mapper.Map<ProfileUserResponseDTO>(expectedUserMapping))
                  .Returns(new ProfileUserResponseDTO(Id: UserId, IdentityUserId: IdentityUserId, UserName: "testUser",
                      Email: "test@example.com", ProfileImagePath: imagePath));
        userRepositoryMock.Setup(repo => repo.GetUserRoleAsync(expectedUser.Id, cancellationToken))
                          .ReturnsAsync(Result.Ok(expectedRole));

        var userService = new UserService(userRepositoryMock.Object, mapperMock.Object);
        
        var result = await userService.LoginUserAsync(userLoginRequestDTO, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedToken, result.Token);
        Assert.NotNull(result.User);
        Assert.Equal("user123", result.User.IdentityUserId);
        Assert.Equal("testUser", result.User.UserName);
        Assert.Equal("test@example.com", result.User.Email);
        Assert.Equal(expectedRole, result.Role);
    }

    [Theory]
    [MemberData(nameof(InvalidLoginData))]
    public async Task LoginUserAsync_InvalidLogin_ReturnsBadRequestException(string email, string password, 
        string expectedErrorMessage, Type expectedExceptionType)
    {
        var userLoginRequestDTO = new UserLoginRequestDTO(Email: email, Password: password);
        var cancellationToken = CancellationToken.None;
        var userRepositoryMock = new Mock<IUserRepository>();
        var mapperMock = new Mock<IMapper>();
        var userService = new UserService(userRepositoryMock.Object, mapperMock.Object);

        userRepositoryMock.Setup(repo => repo.LoginUserAsync(userLoginRequestDTO, cancellationToken))
                          .ReturnsAsync(Result.Fail(expectedErrorMessage, new UserLogin(string.Empty, 
                          new UserMapping())));

        var exception = await Assert.ThrowsAsync(expectedExceptionType, 
            () => userService.LoginUserAsync(userLoginRequestDTO, cancellationToken));

        Assert.Equal(expectedErrorMessage, exception.Message);
    }

    public static IEnumerable<object[]> InvalidLoginData =>
       new List<object[]>
       {
            new object[] { "test@example.com", "wrongpassword", 
                "Senha incorreta. Por favor, verifique suas credenciais e tente novamente.", typeof(BadRequestException) },
            new object[] { "nonexistent@example.com", "password", 
                "Usuário não encontrado. Por favor, verifique suas credenciais e tente novamente.", typeof(NotFoundException) }
       };
}
