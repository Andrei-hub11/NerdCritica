using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using NerdCritica.Application.Services.User;
using NerdCritica.Domain.DTOs.MappingsDapper;
using NerdCritica.Domain.DTOs.User;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.ObjectValues;
using NerdCritica.Domain.Repositories.Movies;
using NerdCritica.Domain.Repositories.User;
using NerdCritica.Domain.Utils;
using NerdCritica.Domain.Utils.Exceptions;

namespace NerdCritica.TestProject;

public class UserAccountTests
{

    [Fact]
    public async Task GetUserByIdAsync_UserFound_ReturnsUserProfile()
    {
        var identityUserId = "user123";
        var userId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        var userRepositoryMock = new Mock<IUserRepository>();
        var movieRepositoryMock = new Mock<IMoviePostRepository>();
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserMapping, ProfileUserResponseDTO>();
        });

        var mapper = new Mapper(configuration);

        var expectedUser = new UserMapping { Id = userId, IdentityUserId = identityUserId, 
            UserName = "testUser", Email = "test@example.com", ProfileImagePath = "profile.jpg" };

        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(identityUserId, cancellationToken))
                          .ReturnsAsync(expectedUser);

        var expectedProfileDTO = new ProfileUserResponseDTO(Id: userId, IdentityUserId: identityUserId,
            UserName: "testUser", Email: "test@example.com", ProfileImagePath: "profile.jpg");
      
        var userService = new UserService(userRepositoryMock.Object, movieRepositoryMock.Object, mapper);

        var result = await userService.GetUserByIdAsync(identityUserId, cancellationToken);

        Assert.NotNull(result);
        Assert.Equal(identityUserId, result.IdentityUserId);
        Assert.Equal("testUser", result.UserName);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("profile.jpg", result.ProfileImagePath);
    }

    [Fact]
    public void GetUserByIdAsync_UserNotFound_ThrowsNotFoundException()
    {
        var userId = "nonExistentUser";
        var cancellationToken = CancellationToken.None;

        var userRepositoryMock = new Mock<IUserRepository>();
        var movieRepositoryMock = new Mock<IMoviePostRepository>();
        var mapperMock = new Mock<IMapper>();

        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId, cancellationToken))
                          .ReturnsAsync(() => null);
        var userService = new UserService(userRepositoryMock.Object, movieRepositoryMock.Object, mapperMock.Object);

        Assert.ThrowsAsync<NotFoundException>(() => userService.GetUserByIdAsync(userId, cancellationToken));
    }

    [Fact]
    public async Task GetFavoriteMovies_Success_ReturnsFavoriteMovieResponseDTOList()
    {
        var identityUserId = "userId123";
        var cancellationToken = CancellationToken.None;

        var favoriteMovies = new List<FavoriteMovieMapping>
        {
            new FavoriteMovieMapping { FavoriteMovieId = Guid.NewGuid(), MovieImagePath = "image1.jpg", CreatedAt = DateTime.Now },
            new FavoriteMovieMapping { FavoriteMovieId = Guid.NewGuid(), MovieImagePath = "image2.jpg", CreatedAt = DateTime.Now }
        };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.GetFavoriteMovies(identityUserId, cancellationToken))
                          .ReturnsAsync(favoriteMovies);

        var movieRepositoryMock = new Mock<IMoviePostRepository>();

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<FavoriteMovieMapping, FavoriteMovieResponseDTO>();
        });

        var mapper = new Mapper(configuration);

        var userService = new UserService(userRepositoryMock.Object, movieRepositoryMock.Object, mapper);

        var result = await userService.GetFavoriteMovies(identityUserId, cancellationToken);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<FavoriteMovieResponseDTO>>(result);
        Assert.Equal(favoriteMovies.Count, result.Count);

        foreach (var movieResponse in result)
        {
            Assert.NotNull(movieResponse);
            Assert.Contains(favoriteMovies, m =>
                m.MoviePostId == movieResponse.MoviePostId &&
                m.FavoriteMovieId == movieResponse.FavoriteMovieId &&
                m.MovieImagePath == movieResponse.MovieImagePath &&
                m.CreatedAt == movieResponse.CreatedAt);
        }
    }

    [Fact]
    public async Task GetFavoriteMovies_NullFavoriteMovies_ReturnsEmptyList()
    {
        var identityUserId = "userId123";
        var cancellationToken = CancellationToken.None;

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.GetFavoriteMovies(identityUserId, cancellationToken))
                          .ReturnsAsync(new List<FavoriteMovieMapping>());

        var movieRepositoryMock = new Mock<IMoviePostRepository>();

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<FavoriteMovieMapping, FavoriteMovieResponseDTO>();
        });

        var mapper = new Mapper(configuration);

        var userService = new UserService(userRepositoryMock.Object, movieRepositoryMock.Object, mapper);

        var result = await userService.GetFavoriteMovies(identityUserId, cancellationToken);

        Assert.Empty(result);
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
        var movieRepositoryMock = new Mock<IMoviePostRepository>();
        userRepositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<ExtensionUserIdentity>()))
                          .ReturnsAsync(Result.Ok(new UserCreationTokenAndId(expectedUserId, expectedToken)));

        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(expectedUserId, cancellationToken))
                         .ReturnsAsync(new UserMapping
                         {
                             Id = idGuid,
                             IdentityUserId = expectedUserId, // Assuming IdentityUserId is same as expectedUserId
                             UserName = expectedUserName,
                             Email = expectedEmail
                         });

        userRepositoryMock.Setup(repo => repo.GetUserRoleAsync(expectedUserId, cancellationToken))
            .ReturnsAsync(expectedRole);

        var imagePath = "profile.jpg";
        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(mapper => mapper.Map<ProfileUserResponseDTO>(It.IsAny<UserMapping>()))
                  .Returns(new ProfileUserResponseDTO (Id: idGuid, IdentityUserId: expectedUserId,
                  UserName: expectedUserName,
                  Email: expectedEmail, ProfileImagePath: imagePath));

        var userService = new UserService(userRepositoryMock.Object, movieRepositoryMock.Object, mapperMock.Object);

        var result = await userService.CreateUserAsync(createUserRequestDTO, pathImage, profileImageBytes,
            cancellationToken);

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
        var movieRepositoryMock = new Mock<IMoviePostRepository>();

        userRepositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<ExtensionUserIdentity>()))
                          .ReturnsAsync(Result.Fail(errorCode, new UserCreationTokenAndId(string.Empty, string.Empty)));

        var userService = new UserService(userRepositoryMock.Object, movieRepositoryMock.Object, Mock.Of<IMapper>());

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
        var userId = Guid.NewGuid();
        var identityUserId = "user123";
        var imagePath = "profile.jpg";

        var userRepositoryMock = new Mock<IUserRepository>();
        var mapperMock = new Mock<IMapper>();
        var userManagerMock = new Mock<UserManager<IdentityUser>>();
        var movieRepositoryMock = new Mock<IMoviePostRepository>();

        var configurationMock = new Mock<IConfiguration>();
        var expectedUser = new IdentityUser { Id = identityUserId, UserName = "testUser", 
            Email = "test@example.com" };
        var expectedUserMapping = new UserMapping { Id = userId, IdentityUserId = identityUserId, UserName = "testUser", 
            Email = "test@example.com" };
        var expectedToken = "token123";
        var expectedRole = "UserRole";
        userRepositoryMock.Setup(repo => repo.LoginUserAsync(userLoginRequestDTO, cancellationToken))
                         .ReturnsAsync(Result.Ok(new UserLogin(expectedToken, expectedUserMapping)));
        mapperMock.Setup(mapper => mapper.Map<ProfileUserResponseDTO>(expectedUserMapping))
                  .Returns(new ProfileUserResponseDTO(Id: userId, IdentityUserId: identityUserId, UserName: "testUser",
                      Email: "test@example.com", ProfileImagePath: imagePath));
        userRepositoryMock.Setup(repo => repo.GetUserRoleAsync(expectedUser.Id, cancellationToken))
                          .ReturnsAsync(expectedRole);

        var userService = new UserService(userRepositoryMock.Object, movieRepositoryMock.Object, mapperMock.Object);
        
        var result = await userService.LoginUserAsync(userLoginRequestDTO, cancellationToken);

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
        var movieRepositoryMock = new Mock<IMoviePostRepository>();
        var mapperMock = new Mock<IMapper>();

        var userService = new UserService(userRepositoryMock.Object, movieRepositoryMock.Object, mapperMock.Object);

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

    [Fact]
    public async Task AddFavoriteMovieAsync_Success_ReturnsTrue()
    {
        var addFavoriteMovieRequestDTO = new AddFavoriteMovieRequestDTO
         (
             IdentityUserId: "userId123",
             MoviePostId: Guid.NewGuid()
         );

        var cancellationToken = CancellationToken.None;

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync("userId123", cancellationToken))
                          .ReturnsAsync(new UserMapping { });

        var moviePostRepositoryMock = new Mock<IMoviePostRepository>();
        moviePostRepositoryMock.Setup(repo => repo.GetMoviePostByIdAsync(addFavoriteMovieRequestDTO.MoviePostId, cancellationToken))
                               .ReturnsAsync(new MoviePostMapping());

        userRepositoryMock.Setup(repo => repo.AddFavoriteMovieAsync(It.IsAny<FavoriteMovie>(), cancellationToken))
                      .ReturnsAsync(true);

        var mapperMock = new Mock<IMapper>();

        var userService = new UserService(userRepositoryMock.Object, moviePostRepositoryMock.Object, mapperMock.Object);

        var result = await userService.AddFavoriteMovieAsync(addFavoriteMovieRequestDTO, cancellationToken);

        Assert.True(result);
    }

    [Fact]
    public async Task AddFavoriteMovieAsync_UserNotFound_ThrowsNotFoundException()
    {
        var addFavoriteMovieRequestDTO = new AddFavoriteMovieRequestDTO
        (
            IdentityUserId: "userId123",
            MoviePostId: Guid.NewGuid()
        );

        var cancellationToken = CancellationToken.None;

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync("userId123", cancellationToken))
                          .ReturnsAsync(() => null);

        var moviePostRepositoryMock = new Mock<IMoviePostRepository>();
        moviePostRepositoryMock.Setup(repo => repo.GetMoviePostByIdAsync(Guid.NewGuid(), cancellationToken))
                               .ReturnsAsync(new MoviePostMapping
                               {
                                   MoviePostId = Guid.Parse("6e35e9e3-8eb7-4510-87a4-7cc2cb5d2b79"),
                                   MovieImagePath = "movies/images/8de4fc2e-f4c3-4edf-a24e-6568967c9050.jpg",
                                   MovieBackdropImagePath = "movies/backdrops/1a7cdcf2-49ca-482a-96f2-4d6f0d0e164c.jpg",
                                   MovieTitle = "Star Wars: A Ascensão Skywalker",
                                   Rating = 0,
                                   Comments = new List<CommentsMapping>(),
                                   MovieDescription = "Em 'Ascensão Skywalker', a saga épica de Star Wars culmina em um confronto final entre a Resistência e a Primeira Ordem. Rey embarca em uma jornada para descobrir seu verdadeiro destino, enquanto segredos do passado são revelados. Com ação intensa, reviravoltas emocionantes e um desfecho que redefine o equilíbrio da Força, o filme cativa os fãs com uma conclusão épica e emocionante.",
                                   MovieCategory = "Ação",
                                   Director = "J.J Abrams",
                                   ReleaseDate = new DateTime(2019, 12, 25),
                                   Runtime = 9495,
                                                               Cast = new List<CastMemberMapping>
                                {
                                    new CastMemberMapping
                                    {
                                        CastMemberId = Guid.Parse("69f36d6a-2425-472f-8edc-8ff15d641bd1"),
                                        MemberName = "Mark Hamill",
                                        CharacterName = "Luke",
                                        MemberImagePath = "movies/cast/e3b234a1-009a-4f12-81a4-410a40312214.jpg",
                                        RoleInMovie = "Protagonista",
                                        RoleType = 1
                                    }
                                }
                               });

        var mapperMock = new Mock<IMapper>();

        var userService = new UserService(userRepositoryMock.Object, moviePostRepositoryMock.Object, mapperMock.Object);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            userService.AddFavoriteMovieAsync(addFavoriteMovieRequestDTO, cancellationToken));
    }

    [Fact]
    public async Task AddFavoriteMovieAsync_MoviePostNotFound_ThrowsNotFoundException()
    {
        var addFavoriteMovieRequestDTO = new AddFavoriteMovieRequestDTO
        (
            IdentityUserId: "userId123",
            MoviePostId: Guid.NewGuid()
        );
        var cancellationToken = CancellationToken.None;

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync("userId123", cancellationToken))
                          .ReturnsAsync(() => null);

        var moviePostRepositoryMock = new Mock<IMoviePostRepository>();
        moviePostRepositoryMock.Setup(repo => repo.GetMoviePostByIdAsync(Guid.NewGuid(), cancellationToken))
                               .ReturnsAsync(()=>null);

        var mapperMock = new Mock<IMapper>();

        var userService = new UserService(userRepositoryMock.Object, moviePostRepositoryMock.Object, mapperMock.Object);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            userService.AddFavoriteMovieAsync(addFavoriteMovieRequestDTO, cancellationToken));
    }

    [Fact]
    public async Task AddFavoriteMovieAsync_MovieAlreadyFavorited_ThrowsNotFoundException()
    {
        var addFavoriteMovieRequestDTO = new AddFavoriteMovieRequestDTO
        (
            IdentityUserId: "userId123",
            MoviePostId: Guid.NewGuid()
        );
        var cancellationToken = CancellationToken.None;

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync("userId123", cancellationToken))
                          .ReturnsAsync(() => null);

        userRepositoryMock.Setup(repo => repo.GetFavoriteMovies(addFavoriteMovieRequestDTO.IdentityUserId, cancellationToken))
                         .ReturnsAsync(new List<FavoriteMovieMapping>
                         {
                              new FavoriteMovieMapping { MoviePostId = addFavoriteMovieRequestDTO.MoviePostId }
                         });

        var moviePostRepositoryMock = new Mock<IMoviePostRepository>();
        moviePostRepositoryMock.Setup(repo => repo.GetMoviePostByIdAsync(Guid.NewGuid(), cancellationToken))
                               .ReturnsAsync(() => null);

        var mapperMock = new Mock<IMapper>();

        var userService = new UserService(userRepositoryMock.Object, moviePostRepositoryMock.Object, mapperMock.Object);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            userService.AddFavoriteMovieAsync(addFavoriteMovieRequestDTO, cancellationToken));
    }

    [Fact]
    public async Task UpdateUserAsync_Success_CallsRepositoryMethodsCorrectly()
    {
        var updateUserRequestDTO = new UpdateUserRequestDTO(
            Username: "exampleUsername",
            Email: "example@example.com",
            ProfileImage: "path/to/profile/image.jpg"
        );

        var cancellationToken = CancellationToken.None;
        var userId = Guid.NewGuid().ToString();
        byte[] profileImageBytes = new byte[0];
        var imagePath = "profile.jpg";

        var existingUser = new UserMapping
        {
            Id = Guid.NewGuid(),
            IdentityUserId = userId,
            UserName = "exampleUsername",
            Email = "example@example.com",
            ProfileImagePath = "path/to/profile/image.jpg"
        };


        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>(), cancellationToken))
                          .ReturnsAsync(existingUser);
        userRepositoryMock.Setup(repo => repo.UpdateUserAsync(It.IsAny<ExtensionUserIdentity>(), userId, cancellationToken))
                          .ReturnsAsync(Result.Ok(true)); // Indique que a atualização foi bem-sucedida
        userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId, cancellationToken))
                          .ReturnsAsync(existingUser); // Retorne o usuário existente após a atualização

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserMapping, ProfileUserResponseDTO>();
        });

        var mapper = new Mapper(configuration);

        var moviePostRepositoryMock = new Mock<IMoviePostRepository>();

        var userService = new UserService(userRepositoryMock.Object, moviePostRepositoryMock.Object, mapper);

        var result = await userService.UpdateUserAsync(updateUserRequestDTO, userId, imagePath, 
            profileImageBytes, cancellationToken);

        // Assert
        userRepositoryMock.Verify(repo => repo.GetUserByEmailAsync(updateUserRequestDTO.Email, cancellationToken), Times.Once);
        userRepositoryMock.Verify(repo => repo.UpdateUserAsync(It.IsAny<ExtensionUserIdentity>(), userId, cancellationToken), Times.Once);
        userRepositoryMock.Verify(repo => repo.GetUserByIdAsync(userId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_InvalidUserData_ThrowsValidationException()
    {
        var updateUserRequestDTO = new UpdateUserRequestDTO(
            Username: "",
            Email: "example@example.com",
            ProfileImage: "path/to/profile/image.jpg"
        );

        var cancellationToken = CancellationToken.None;
        var userId = Guid.NewGuid().ToString();
        byte[] profileImageBytes = new byte[0];
        var imagePath = "profile.jpg";

        var userRepositoryMock = new Mock<IUserRepository>();

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserMapping, ProfileUserResponseDTO>();
        });

        var mapper = new Mapper(configuration);

        var moviePostRepositoryMock = new Mock<IMoviePostRepository>();

        var userService = new UserService(userRepositoryMock.Object, moviePostRepositoryMock.Object, mapper);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            userService.UpdateUserAsync(updateUserRequestDTO, userId, imagePath, profileImageBytes, cancellationToken));
    }

    [Fact]
    public async Task UpdateUserAsync_UserNotFoundByEmail_ThrowsNotFoundException()
    {
        var updateUserRequestDTO = new UpdateUserRequestDTO(
            Username: "exampleUsername",
            Email: "example@example.com",
            ProfileImage: "path/to/profile/image.jpg"
        );

        var cancellationToken = CancellationToken.None;
        var userId = Guid.NewGuid().ToString();
        byte[] profileImageBytes = new byte[0];
        var imagePath = "profile.jpg";

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>(), cancellationToken))
                          .ReturnsAsync(() => null);

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserMapping, ProfileUserResponseDTO>();
        });

        var mapper = new Mapper(configuration);

        var moviePostRepositoryMock = new Mock<IMoviePostRepository>();

        var userService = new UserService(userRepositoryMock.Object, moviePostRepositoryMock.Object, mapper);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            userService.UpdateUserAsync(updateUserRequestDTO, userId, imagePath, profileImageBytes, cancellationToken));
    }

    [Fact]
    public async Task UpdateUserAsync_EmailAlreadyAssociatedWithAnotherUser_ThrowsValidationException()
    {
        var updateUserRequestDTO = new UpdateUserRequestDTO(
            Username: "exampleUsername",
            Email: "example@example.com",
            ProfileImage: "path/to/profile/image.jpg"
        );

        var cancellationToken = CancellationToken.None;
        var userId = Guid.NewGuid().ToString();
        byte[] profileImageBytes = new byte[0];
        var imagePath = "profile.jpg";

        var existingUser = new UserMapping
        {
            Id = Guid.NewGuid(),
            IdentityUserId = "anotherUserId",
            UserName = "anotherUsername",
            Email = "example@example.com", // E-mail já associado a outro usuário
            ProfileImagePath = "path/to/another/profile/image.jpg"
        };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>(), cancellationToken))
                          .ReturnsAsync(existingUser);

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserMapping, ProfileUserResponseDTO>();
        });

        var mapper = new Mapper(configuration);

        var moviePostRepositoryMock = new Mock<IMoviePostRepository>();

        var userService = new UserService(userRepositoryMock.Object, moviePostRepositoryMock.Object, mapper);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            userService.UpdateUserAsync(updateUserRequestDTO, userId, imagePath, profileImageBytes, cancellationToken));
    }


    [Fact]
    public async Task RemoveFavoriteMovie_FavoriteMovieExists_ReturnsTrue()
    {
        var favoriteMovieId = Guid.NewGuid();
        var identityUserId = "userId123";
        var cancellationToken = CancellationToken.None;

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.GetFavoriteMovies(identityUserId, cancellationToken))
                          .ReturnsAsync(new List<FavoriteMovieMapping>
                          {
                              new FavoriteMovieMapping { FavoriteMovieId = favoriteMovieId }
                          });

        userRepositoryMock.Setup(repo => repo.RemoveFavoriteMovie(favoriteMovieId, identityUserId, cancellationToken))
                          .ReturnsAsync(true);

        var movieRepositoryMock = new Mock<IMoviePostRepository>();

        var mapperMock = new Mock<IMapper>();

        var userService = new UserService(userRepositoryMock.Object, movieRepositoryMock.Object, mapperMock.Object);

        var result = await userService.RemoveFavoriteMovie(favoriteMovieId, identityUserId, cancellationToken);

        Assert.True(result);
    }

    [Fact]
    public async Task RemoveFavoriteMovie_FavoriteMovieDoesNotExist_ThrowsNotFoundException()
    {
        var favoriteMovieId = Guid.NewGuid();
        var identityUserId = "userId123";
        var cancellationToken = CancellationToken.None;

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.GetFavoriteMovies(identityUserId, cancellationToken))
                          .ReturnsAsync(new List<FavoriteMovieMapping>());

        var movieRepositoryMock = new Mock<IMoviePostRepository>();

        var mapperMock = new Mock<IMapper>();

        var userService = new UserService(userRepositoryMock.Object, movieRepositoryMock.Object, mapperMock.Object);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            userService.RemoveFavoriteMovie(favoriteMovieId, identityUserId, cancellationToken));
    }

}
