using AutoMapper;
using Moq;
using NerdCritica.Application.Services.Images;
using NerdCritica.Application.Services.Movies;
using NerdCritica.Contracts.DTOs.MappingsDapper;
using NerdCritica.Contracts.DTOs.Movie;
using NerdCritica.Contracts.DTOs.User;
using NerdCritica.Domain.Common;
using NerdCritica.Domain.Contracts;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.Entities.Aggregates;
using NerdCritica.Domain.Repositories.Movies;
using NerdCritica.Domain.Utils.Exceptions;

namespace NerdCritica.TestProject;

public class MoviesTests
{

    [Fact]
    public async Task GetMoviePostsAsync_CallsRepositoryWithCorrectArguments()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var mockRepository = new Mock<IMoviePostRepository>();

        // Configurando o mock para verificar argumentos
        mockRepository.Setup(repo => repo.GetMoviePostsAsync(It.IsAny<CancellationToken>()))
                               .ReturnsAsync(() => new List<MoviePostMapping>())
                               .Callback<CancellationToken>(token =>
                               {
                                   // Verifica se o token de cancelamento é o mesmo passado
                                   Assert.Equal(cancellationToken, token);
                               });

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<FavoriteMovieMapping, FavoriteMovieResponseDTO>();
            cfg.CreateMap<UserMapping, ProfileUserResponseDTO>();
            cfg.CreateMap<CastMemberMapping, CastMemberResponseDTO>();
            cfg.CreateMap<CommentsMapping, CommentsResponseDTO>();
            cfg.CreateMap<CommentLikeMapping, CommentLikeResponseDTO>();
            cfg.CreateMap<MoviePostMapping, MoviePostResponseDTO>();
        });

        var mockImageService = new Mock<IImagesService>();

        var mapper = new Mapper(configuration);

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mapper);

        // Act
        var result = await service.GetMoviePostsAsync(cancellationToken);

        mockRepository.Verify(repo => repo.GetMoviePostsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task CreateMoviePostAsync_ValidInput_ReturnsTrue()
    {
        var creatorUserId = Guid.NewGuid().ToString();
        var movieTitle = "New Movie Title";
        var movieDescription = "New movie descriptionaaaaa aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.";
        var movieImage = "movie_image.jpg";
        var movieBackdrop = "movie_backdrop.jpg";
        var movieCategory = "Action";
        var director = "John Doe";
        var releaseDate = DateTime.Now;
        var runtime = TimeSpan.FromHours(2);
        var cast = new List<CastMemberRequestDTO>
    {
        new CastMemberRequestDTO("Actor 1", "Character 1", "image1.jpg", 1),
        new CastMemberRequestDTO("Actor 2", "Character 2", "image2.jpg", 2)
    };

        var mockImageService = new Mock<IImagesService>();
        mockImageService.Setup(service => service.GetMoviePostImagesAsync(It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync(new MovieImages
                        {
                            MovieImagePath = "path/to/movie/image",
                            MovieBackdropPath = "path/to/backdrop/image",
                            MovieImageBytes = new byte[1024],
                            MovieBackdropBytes = new byte[1024]
                        });

        mockImageService.Setup(service => service.GetCastImagesAsync(It.IsAny<List<CastMemberRequestDTO>>()))
                        .ReturnsAsync(new Dictionary<string, CastImages>
                        {
                        { "Actor 1", CastImages.Create("path/to/actor1/image", new byte[1024]) },
                        { "Actor 2", CastImages.Create("path/to/actor2/image", new byte[1024]) }
                        });

        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.CreateMoviePostAsync(It.IsAny<MoviePost>(), CancellationToken.None))
                      .ReturnsAsync(Guid.NewGuid()); // Simulando a criação de um novo ID de post

        mockRepository.Setup(repo => repo.CreateCastMovieAsync(It.IsAny<List<CastMember>>(), It.IsAny<Guid>()))
                      .ReturnsAsync(true); // Simulando sucesso na criação do elenco

        var userId = Guid.NewGuid();

        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        var request = new CreateMoviePostRequestDTO(
            CreatorUserId: creatorUserId,
            MovieImage: movieImage,
            MovieBackdropImage: movieBackdrop,
            MovieTitle: movieTitle,
            MovieDescription: movieDescription,
            MovieCategory: movieCategory,
            Director: director,
            ReleaseDate: releaseDate,
            Runtime: runtime,
            Cast: cast
        );

        var result = await service.CreateMoviePostAsync(request, CancellationToken.None);

        Assert.True(result); // Verifica se o método retorna true
        mockRepository.Verify(repo => repo.CreateMoviePostAsync(It.IsAny<MoviePost>(), CancellationToken.None), Times.Once); // Verifica se o método CreateMoviePostAsync foi chamado exatamente uma vez
        mockRepository.Verify(repo => repo.CreateCastMovieAsync(It.IsAny<List<CastMember>>(), It.IsAny<Guid>()), Times.Once); // Verifica se o método CreateCastMovieAsync foi chamado exatamente uma vez
    }

    [Fact]
    public async Task CreateMoviePostAsync_InvalidInput_ThrowsValidationException()
    {

        var userId = Guid.NewGuid();

        var mockImageService = new Mock<IImagesService>();

        var mockRepository = new Mock<IMoviePostRepository>();
        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        mockImageService.Setup(service => service.GetMoviePostImagesAsync(It.IsAny<string>(), It.IsAny<string>()))
                      .ReturnsAsync(new MovieImages
                      {
                          MovieImagePath = "path/to/movie/image",
                          MovieBackdropPath = "path/to/backdrop/image",
                          MovieImageBytes = new byte[1024],
                          MovieBackdropBytes = new byte[1024]
                      });

        mockImageService.Setup(service => service.GetCastImagesAsync(It.IsAny<List<CastMemberRequestDTO>>()))
                        .ReturnsAsync(new Dictionary<string, CastImages>
                        {
                        { "Actor 1", CastImages.Create("path/to/actor1/image", new byte[1024]) },
                        { "Actor 2", CastImages.Create("path/to/actor2/image", new byte[1024]) }
                        });

        var request = new CreateMoviePostRequestDTO
        (
            CreatorUserId: "sampleUserId",
            MovieImage: "sampleImagePath",
            MovieBackdropImage: "sampleBackdropPath",
            MovieTitle: "",
            MovieDescription: "",
            MovieCategory: "Sample Category",
            Director: "Sample Director",
            ReleaseDate: DateTime.Now,
            Runtime: TimeSpan.FromHours(2),
            Cast: new List<CastMemberRequestDTO>()
        );

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            service.CreateMoviePostAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task CreateMoviePostAsync_MissingMovieImage_ThrowsValidationException()
    {
        // Arrange
        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        mockImageService.Setup(service => service.GetMoviePostImagesAsync(It.IsAny<string>(), It.IsAny<string>()))
                   .ReturnsAsync(new MovieImages
                   {
                       MovieImagePath = "path/to/movie/image",
                       MovieBackdropPath = "path/to/backdrop/image",
                       MovieImageBytes = new byte[1024],
                       MovieBackdropBytes = new byte[1024]
                   });

        mockImageService.Setup(service => service.GetCastImagesAsync(It.IsAny<List<CastMemberRequestDTO>>()))
                        .ReturnsAsync(new Dictionary<string, CastImages>
                        {
                        { "Actor 1", CastImages.Create("path/to/actor1/image", new byte[1024]) },
                        { "Actor 2", CastImages.Create("path/to/actor2/image", new byte[1024]) }
                        });

        var request = new CreateMoviePostRequestDTO
        (
            CreatorUserId: "sampleUserId",
            MovieImage: "", // Caminho da imagem em branco
            MovieBackdropImage: "sampleBackdropPath",
            MovieTitle: "Sample Movie",
            MovieDescription: "This is a sample movie description with more than 100 characters to pass the validation",
            MovieCategory: "Sample Category",
            Director: "Sample Director",
            ReleaseDate: DateTime.Now,
            Runtime: TimeSpan.FromHours(2),
            Cast: new List<CastMemberRequestDTO>()
        );

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            service.CreateMoviePostAsync(request, CancellationToken.None));
    }


    [Fact]
    public async Task CreateRatingAsync_ValidRequest_ReturnsTrue()
    {
        var ratingId = Guid.NewGuid();
        var moviePostId = Guid.NewGuid();
        var identityUserId = Guid.NewGuid().ToString();
        var ratingValue = 4.5m;
        var commentContent = "Great movie!";

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.CreateRatingAsync(It.IsAny<MovieRating>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(ratingId);
        mockRepository.Setup(repo => repo.CreateCommentAsync(It.IsAny<Comment>()))
                      .ReturnsAsync(true);

        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        var request = new CreateRatingRequestDTO
        (
            MoviePostId: moviePostId,
            IdentityUserId: identityUserId,
            Rating: ratingValue,
            Comment: new CreateCommentDTO(IdentityUserId: identityUserId, Content: commentContent)
        );

        var result = await service.CreateRatingAsync(request, CancellationToken.None);

        Assert.True(result);

        mockRepository.Verify(repo => repo.CreateRatingAsync(It.IsAny<MovieRating>(), It.IsAny<CancellationToken>()), Times.Once);
        mockRepository.Verify(repo => repo.CreateCommentAsync(It.IsAny<Comment>()), Times.Once);
    }

    [Fact]
    public async Task CreateRatingAsync_InvalidRating_ThrowsValidationException()
    {
        var ratingId = Guid.NewGuid();
        var moviePostId = Guid.NewGuid();
        var identityUserId = Guid.NewGuid().ToString();
        var invalidRating = -1;
        var commentContent = "uau";

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        var request = new CreateRatingRequestDTO
        (
            MoviePostId: moviePostId,
            IdentityUserId: identityUserId,
            Rating: invalidRating,
            Comment: new CreateCommentDTO(IdentityUserId: identityUserId, Content: commentContent)
        );

        await Assert.ThrowsAsync<ValidationException>(() => service.
        CreateRatingAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task CreateRatingAsync_InvalidComment_ThrowsValidationException()
    {
        var moviePostId = Guid.NewGuid();
        var identityUserId = Guid.NewGuid().ToString();
        var validRating = 4.5m;
        var invalidCommentContent = "";

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        var request = new CreateRatingRequestDTO
        (
            MoviePostId: moviePostId,
            IdentityUserId: identityUserId,
            Rating: validRating,
            Comment: new CreateCommentDTO(IdentityUserId: identityUserId, Content: invalidCommentContent)
        );

        await Assert.ThrowsAsync<ValidationException>(() => service.
        CreateRatingAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task CreateCommentLikeAsync_ValidRequest_ReturnsTrue()
    {
        var movieRatingId = Guid.NewGuid();
        var commentId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.GetRatingByIdAsync(movieRatingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MovieRatingMapping { Comment = new CommentsMapping { CommentId = commentId } });
        mockRepository.Setup(repo => repo.CreateCommentLikeAsync(commentId, userId.ToString(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        var request = new CreateCommentLikeRequestDTO
        (
            RatingId: movieRatingId,
            CommentId: commentId,
            IdentityUserId: userId.ToString(),
            CommentAuthorId: Guid.NewGuid().ToString()
        );

        var result = await service.CreateCommentLikeAsync(request, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task CreateCommentLikeAsync_InvalidRatingId_ThrowsNotFoundException()
    {
        var movieRatingId = Guid.NewGuid();
        var commentId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();

        mockRepository.Setup(repo => repo.GetRatingByIdAsync(movieRatingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        var request = new CreateCommentLikeRequestDTO
       (
           RatingId: movieRatingId,
           CommentId: commentId,
           IdentityUserId: userId.ToString(),
           CommentAuthorId: Guid.NewGuid().ToString()
       );

        await Assert.ThrowsAsync<NotFoundException>(() =>
        service.CreateCommentLikeAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task CreateCommentLikeAsync_CommentIdMismatch_ThrowsBadRequestException()
    {
        var movieRatingId = Guid.NewGuid();
        var commentId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.GetRatingByIdAsync(movieRatingId, It.IsAny<CancellationToken>()))
          .ReturnsAsync(new MovieRatingMapping
          {
              Comment = new CommentsMapping
              { CommentId = Guid.NewGuid() }
          });

        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        var request = new CreateCommentLikeRequestDTO
       (
           RatingId: movieRatingId,
           CommentId: commentId,
           IdentityUserId: userId.ToString(),
           CommentAuthorId: Guid.NewGuid().ToString()
       );

        await Assert.ThrowsAsync<BadRequestException>(() => service.CreateCommentLikeAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task CreateCommentLikeAsync_UserAlreadyLikedComment_ThrowsBadRequestException()
    {
        var movieRatingId = Guid.NewGuid();
        var commentId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();

        mockRepository.Setup(repo => repo.GetRatingByIdAsync(movieRatingId, It.IsAny<CancellationToken>()))
          .ReturnsAsync(new MovieRatingMapping
          {
              Comment = new CommentsMapping
              { CommentId = Guid.NewGuid() }
          });

        mockRepository.Setup(repo => repo.GetCommentLikeByIdAsync(commentId, userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CommentLikeMapping { CommentId = commentId, IdentityUserId = userId.ToString() });

        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        var request = new CreateCommentLikeRequestDTO
        (
            RatingId: movieRatingId,
            CommentId: commentId,
            IdentityUserId: userId.ToString(),
            CommentAuthorId: Guid.NewGuid().ToString()
        );

        await Assert.ThrowsAsync<BadRequestException>(() => service.CreateCommentLikeAsync(request, CancellationToken.None));
    }


    [Fact]
    public async Task CreateCommentLikeAsync_RepositoryCalledWithExpectedArguments()
    {
        var movieRatingId = Guid.NewGuid();
        var commentId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.GetRatingByIdAsync(movieRatingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MovieRatingMapping { Comment = new CommentsMapping { CommentId = commentId } });

        mockRepository.Setup(repo => repo.GetCommentLikeByIdAsync(commentId, userId.ToString(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(() => null);

        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        var request = new CreateCommentLikeRequestDTO
        (
            RatingId: movieRatingId,
            CommentId: commentId,
            IdentityUserId: userId.ToString(),
            CommentAuthorId: Guid.NewGuid().ToString()
        );

        await service.CreateCommentLikeAsync(request, CancellationToken.None);

        mockRepository.Verify(repo => repo.GetRatingByIdAsync(movieRatingId, It.IsAny<CancellationToken>()), Times.Once);
        mockRepository.Verify(repo => repo.CreateCommentLikeAsync(commentId, userId.ToString(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateMoviePostAsync_ValidRequest_ReturnsTrue()
    {
        var moviePostId = Guid.NewGuid();
        var movieTitle = "New Movie Title";
        var movieDescription = "New movie descriptionaaaaa aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.";
        var movieImage = "image";
        var movieBackdrop = "image";
        var movieCategory = "Action";
        var director = "John Doe";
        var releaseDate = DateTime.Now;
        var runtime = TimeSpan.FromHours(2);
        var cast = new List<UpdateCastMemberRequestDTO>
        {
            new UpdateCastMemberRequestDTO
            (
                CastMemberId: Guid.NewGuid(),
                MemberName: "Actor Name",
                MemberImage: "aaa",
                CharacterName: "Character Name",
                RoleType: 1
            )
        };

        var userId = Guid.NewGuid();

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.GetMoviePostByIdAsync(moviePostId, CancellationToken.None))
                      .ReturnsAsync(new MoviePostMapping());
        mockRepository.Setup(repo => repo.UpdateMoviePostAsync(It.IsAny<MoviePost>(), moviePostId, CancellationToken.None))
                      .ReturnsAsync(true);
        mockRepository.Setup(repo => repo.UpdateCastMovieAsync(It.IsAny<List<CastMember>>(), moviePostId))
                      .ReturnsAsync(true);

        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        mockImageService.Setup(service => service.GetMoviePostImagesAsync(It.IsAny<string>(), It.IsAny<string>()))
                   .ReturnsAsync(new MovieImages
                   {
                       MovieImagePath = "path/to/movie/image",
                       MovieBackdropPath = "path/to/backdrop/image",
                       MovieImageBytes = new byte[1024],
                       MovieBackdropBytes = new byte[1024]
                   });

        mockImageService.Setup(service => service.GetCastImagesAsync(It.IsAny<List<UpdateCastMemberRequestDTO>>()))
                        .ReturnsAsync(new Dictionary<string, CastImages>
                        {
                        { "Actor 1", CastImages.Create("path/to/actor1/image", new byte[1024]) },
                        { "Actor 2", CastImages.Create("path/to/actor2/image", new byte[1024]) }
                        });

        var request = new UpdateMoviePostRequestDTO(
            MovieTitle: movieTitle,
            MovieImage: movieImage,
            MovieBackdropImage: movieBackdrop,
            MovieDescription: movieDescription,
            MovieCategory: movieCategory,
            Director: director,
            ReleaseDate: releaseDate,
            Runtime: runtime,
            Cast: cast
        );

        var result = await service.UpdateMoviePostAsync(request, moviePostId, CancellationToken.None);

        Assert.True(result);
        mockRepository.Verify(repo => repo.UpdateMoviePostAsync(It.IsAny<MoviePost>(), moviePostId, CancellationToken.None), Times.Once);
        mockRepository.Verify(repo => repo.UpdateCastMovieAsync(It.IsAny<List<CastMember>>(), moviePostId), Times.Once);
    }

    [Fact]
    public async Task UpdateMoviePostAsync_InvalidUpdateFields_ThrowsValidationException()
    {
        var moviePostId = Guid.NewGuid();
        var movieTitle = "New Movie Title";
        var movieDescription = "invalid description";
        var movieImage = "";
        var movieBackdrop = "";
        var movieCategory = "";
        var director = "";
        var releaseDate = DateTime.Now;
        var runtime = TimeSpan.FromHours(2);
        var cast = new List<UpdateCastMemberRequestDTO>
        {
            new UpdateCastMemberRequestDTO
            (
                CastMemberId: Guid.NewGuid(),
                MemberName: "Actor Name",
                MemberImage: "aaa",
                CharacterName: "Character Name",
                RoleType: 1
            )
        };

        var userId = Guid.NewGuid();

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.GetMoviePostByIdAsync(moviePostId, CancellationToken.None))
                      .ReturnsAsync(new MoviePostMapping());
        mockRepository.Setup(repo => repo.UpdateMoviePostAsync(It.IsAny<MoviePost>(), moviePostId, CancellationToken.None))
                      .ReturnsAsync(true);
        mockRepository.Setup(repo => repo.UpdateCastMovieAsync(It.IsAny<List<CastMember>>(), moviePostId))
                      .ReturnsAsync(true);

        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        mockImageService.Setup(service => service.GetMoviePostImagesAsync(It.IsAny<string>(), It.IsAny<string>()))
                   .ReturnsAsync(new MovieImages
                   {
                       MovieImagePath = "path/to/movie/image",
                       MovieBackdropPath = "path/to/backdrop/image",
                       MovieImageBytes = new byte[1024],
                       MovieBackdropBytes = new byte[1024]
                   });

        mockImageService.Setup(service => service.GetCastImagesAsync(It.IsAny<List<CastMemberRequestDTO>>()))
                        .ReturnsAsync(new Dictionary<string, CastImages>
                        {
                        { "Actor 1", CastImages.Create("path/to/actor1/image", new byte[1024]) },
                        { "Actor 2", CastImages.Create("path/to/actor2/image", new byte[1024]) }
                        });

        var request = new UpdateMoviePostRequestDTO(
            MovieTitle: movieTitle,
            MovieImage: movieImage,
            MovieBackdropImage: movieBackdrop,
            MovieDescription: movieDescription,
            MovieCategory: movieCategory,
            Director: director,
            ReleaseDate: releaseDate,
            Runtime: runtime,
            Cast: cast
        );

        var movieImages = new MovieImages
        {
            MovieImagePath = "path/to/image",
            MovieBackdropPath = "path/to/backdrop",
            MovieImageBytes = new byte[10],
            MovieBackdropBytes = new byte[10]
        };

        await Assert.ThrowsAsync<ValidationException>(() => service.UpdateMoviePostAsync(
            request, moviePostId, CancellationToken.None));
    }


    [Fact]
    public async Task UpdateMoviePostAsync_MoviePostNotFound_ThrowsNotFoundException()
    {
        var moviePostId = Guid.NewGuid();
        var movieTitle = "New Movie Title";
        var movieDescription = "New movie descriptionaaaaa aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.";
        var movieImage = "image";
        var movieBackdrop = "image";
        var movieCategory = "Action";
        var director = "John Doe";
        var releaseDate = DateTime.Now;
        var runtime = TimeSpan.FromHours(2);
        var cast = new List<UpdateCastMemberRequestDTO>
        {
            new UpdateCastMemberRequestDTO
            (
                CastMemberId: Guid.NewGuid(),
                MemberName: "Actor Name",
                MemberImage: "aaa",
                CharacterName: "Character Name",
                RoleType: 1
            )
        };

        var userId = Guid.NewGuid();

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.GetMoviePostByIdAsync(moviePostId, CancellationToken.None))
                      .ReturnsAsync(() => null);

        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        mockImageService.Setup(service => service.GetMoviePostImagesAsync(It.IsAny<string>(), It.IsAny<string>()))
                   .ReturnsAsync(new MovieImages
                   {
                       MovieImagePath = "path/to/movie/image",
                       MovieBackdropPath = "path/to/backdrop/image",
                       MovieImageBytes = new byte[1024],
                       MovieBackdropBytes = new byte[1024]
                   });

        mockImageService.Setup(service => service.GetCastImagesAsync(It.IsAny<List<CastMemberRequestDTO>>()))
                        .ReturnsAsync(new Dictionary<string, CastImages>
                        {
                        { "Actor 1", CastImages.Create("path/to/actor1/image", new byte[1024]) },
                        { "Actor 2", CastImages.Create("path/to/actor2/image", new byte[1024]) }
                        });

        var request = new UpdateMoviePostRequestDTO(
           MovieTitle: movieTitle,
           MovieImage: movieImage,
           MovieBackdropImage: movieBackdrop,
           MovieDescription: movieDescription,
           MovieCategory: movieCategory,
           Director: director,
           ReleaseDate: releaseDate,
           Runtime: runtime,
           Cast: cast
       );

        var movieImages = new MovieImages
        {
            MovieImagePath = "path/to/image",
            MovieBackdropPath = "path/to/backdrop",
            MovieImageBytes = new byte[10],
            MovieBackdropBytes = new byte[10]
        };

        await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateMoviePostAsync(
            request, moviePostId, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateMovieRatingAsync_ValidInput_ReturnsTrue()
    {
        var movieRatingId = Guid.NewGuid();
        var movieRating = new UpdateMovieRatingRequestDTO(
            MoviePostId: Guid.NewGuid(),
            IdentityUserId: Guid.NewGuid().ToString(),
            Rating: 4.5m,
            Comment: new UpdateCommentDTO(IdentityUserId: Guid.NewGuid().ToString(), Content: "Great movie!")
        );

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.GetRatingByIdAsync(movieRatingId, CancellationToken.None))
                      .ReturnsAsync(new MovieRatingMapping());

        mockRepository.Setup(repo => repo.UpdateMovieRatingAsync(It.IsAny<MovieRating>(),
            movieRatingId, CancellationToken.None))
                      .ReturnsAsync(true);

        mockRepository.Setup(repo => repo.UpdateCommentAsync(It.IsAny<Comment>(), movieRatingId))
                      .ReturnsAsync(true);

        var userId = Guid.NewGuid();

        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        var result = await service.UpdateMovieRatingAsync(movieRating, movieRatingId, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateMovieRatingAsync_RepositoryReceivesCorrectArguments_ReturnsTrue()
    {
        var movieRatingId = Guid.NewGuid();
        var movieRating = new UpdateMovieRatingRequestDTO(
            MoviePostId: Guid.NewGuid(),
            IdentityUserId: Guid.NewGuid().ToString(),
            Rating: 4.5m,
            Comment: new UpdateCommentDTO(IdentityUserId: Guid.NewGuid().ToString(), Content: "Great movie!")
        );

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        var mockMapper = new Mock<IMapper>();
        mockRepository.Setup(repo => repo.GetRatingByIdAsync(movieRatingId, CancellationToken.None))
                      .ReturnsAsync(new MovieRatingMapping());

        mockRepository.Setup(repo => repo.UpdateMovieRatingAsync(It.IsAny<MovieRating>(), movieRatingId, CancellationToken.None))
               .ReturnsAsync(true)
               .Callback<MovieRating, Guid, CancellationToken>((actualMovieRating, actualMovieRatingId, actualCancellationToken) =>
               {
                   // Verificar se os argumentos passados para o método estão corretos
                   Assert.Equal(movieRatingId, actualMovieRatingId);
                   Assert.Equal(movieRating.IdentityUserId, actualMovieRating.IdentityUserId);
                   Assert.Equal(movieRating.Rating, actualMovieRating.Rating);
               });


        mockRepository.Setup(repo => repo.UpdateCommentAsync(It.IsAny<Comment>(), movieRatingId))
                      .ReturnsAsync(true)
                      .Callback<Comment, Guid>((comment, id) =>
                      {
                          // Verificar se os argumentos passados para o método estão corretos
                          Assert.Equal(movieRatingId, id);
                          Assert.Equal(movieRating.Comment.IdentityUserId, comment.IdentityUserId);
                          Assert.Equal(movieRating.Comment.Content, comment.Content);
                      });

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        var result = await service.UpdateMovieRatingAsync(movieRating, movieRatingId, CancellationToken.None);

        Assert.True(result);
    }


    [Fact]
    public async Task UpdateMovieRatingAsync_RatingNotFound_ThrowsNotFoundException()
    {
        var movieRatingId = Guid.NewGuid();
        var movieRating = new UpdateMovieRatingRequestDTO(
            MoviePostId: Guid.NewGuid(),
            IdentityUserId: Guid.NewGuid().ToString(),
            Rating: 4.5m,
            Comment: new UpdateCommentDTO(IdentityUserId: Guid.NewGuid().ToString(), Content: "Great movie!")
        );

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.GetRatingByIdAsync(movieRatingId, CancellationToken.None))
                      .ReturnsAsync(() => null);

        var userId = Guid.NewGuid();

        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        await Assert.ThrowsAsync<NotFoundException>(() => service.
        UpdateMovieRatingAsync(movieRating, movieRatingId, CancellationToken.None));
    }

    [Theory]
    [InlineData("userid", 5.5, "comment")] // Teste com avaliação inválida
    [InlineData("userid", 3.5, "")] // Teste com comentário inválido
    public async Task UpdateMovieRatingAsync_InvalidInput_ThrowsValidationException(string identityUserId, decimal rating, string commentContent)
    {
        var movieRatingId = Guid.NewGuid();
        var movieRating = new UpdateMovieRatingRequestDTO(
            MoviePostId: Guid.NewGuid(),
            IdentityUserId: identityUserId,
            Rating: rating,
            Comment: new UpdateCommentDTO(IdentityUserId: identityUserId, Content: commentContent)
        );

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.GetRatingByIdAsync(movieRatingId, CancellationToken.None))
                      .ReturnsAsync(new MovieRatingMapping());

        var userId = Guid.NewGuid();

        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        await Assert.ThrowsAsync<ValidationException>(() => service.
        UpdateMovieRatingAsync(movieRating, movieRatingId, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteMoviePostAsync_ValidInput_DeletesMoviePost()
    {
        var moviePostId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.GetMoviePostByIdAsync(moviePostId, cancellationToken))
                      .ReturnsAsync(new MoviePostMapping()); // Simula a postagem do filme encontrada
        mockRepository.Setup(repo => repo.DeleteMoviePostAsync(moviePostId, CancellationToken.None))
                      .ReturnsAsync(true).Callback<Guid, CancellationToken>((id, token) =>
                      {
                          // Verificar se os argumentos passados para o método estão corretos
                          Assert.Equal(moviePostId, id);
                          Assert.Equal(cancellationToken, token);
                      });

        var userId = Guid.NewGuid();

        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        var isDeleted = await service.DeleteMoviePostAsync(moviePostId, CancellationToken.None);

        Assert.True(isDeleted);
        mockRepository.Verify(repo => repo.DeleteMoviePostAsync(moviePostId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteMoviePostAsync_MoviePostNotFound_ThrowsNotFoundException()
    {
        var moviePostId = Guid.NewGuid();

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.GetMoviePostByIdAsync(moviePostId, CancellationToken.None))
                      .ReturnsAsync(() => null); // Simula a postagem do filme não encontrada

        var userId = Guid.NewGuid();

        var mockMapper = new Mock<IMapper>();
        var mockUserContext = new Mock<IUserContext>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.DeleteMoviePostAsync(moviePostId, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteMovieRatingAsync_ValidInput_PassesCorrectArgumentsToRepository()
    {
        var movieRatingId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.GetRatingByIdAsync(movieRatingId, cancellationToken))
                      .ReturnsAsync(new MovieRatingMapping()); // Simula a avaliação do filme encontrada
        mockRepository.Setup(repo => repo.DeleteMovieRatingAsync(movieRatingId, cancellationToken))
                      .ReturnsAsync(true) // Simula a exclusão bem-sucedida
                      .Callback<Guid, CancellationToken>((id, token) =>
                      {
                          // Verificar se os argumentos passados para o método estão corretos
                          Assert.Equal(movieRatingId, id);
                          Assert.Equal(cancellationToken, token);
                      });

        var userId = Guid.NewGuid();

        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        var isDeleted = await service.DeleteMovieRatingAsync(movieRatingId, cancellationToken);

        Assert.True(isDeleted);
        mockRepository.Verify(repo => repo.DeleteMovieRatingAsync(movieRatingId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteMovieRatingAsync_RatingNotFound_ThrowsNotFoundException()
    {
        var movieRatingId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.GetRatingByIdAsync(movieRatingId, cancellationToken))
                      .ReturnsAsync(() => null); // Simula avaliação não encontrada

        var userId = Guid.NewGuid();

        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await service.DeleteMovieRatingAsync(movieRatingId, cancellationToken);
        });
    }

    [Fact]
    public async Task DeleteCommentLikeAsync_ValidInput_PassesCorrectArgumentsToRepository()
    {
        var deleteLikeRequest = new DeleteLikeRequestDTO
        (
            CommentId: Guid.NewGuid(),
            IdentityUserId: "user123"
        );
        var cancellationToken = CancellationToken.None;

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.GetCommentLikeByIdAsync(deleteLikeRequest.CommentId, 
            deleteLikeRequest.IdentityUserId, cancellationToken))
                      .ReturnsAsync(new CommentLikeMapping()); // Simula o like encontrado
        mockRepository.Setup(repo => repo.DeleteCommentLikeAsync(deleteLikeRequest.CommentId, 
            deleteLikeRequest.IdentityUserId, cancellationToken))
                      .ReturnsAsync(true); // Simula a exclusão bem-sucedida

        var userId = Guid.NewGuid();

        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        var result = await service.DeleteCommentLikeAsync(deleteLikeRequest, cancellationToken);

        Assert.True(result); // Verifica se a exclusão foi bem-sucedida

        // Verifica se os argumentos passados para o método do repositório estão corretos
        mockRepository.Verify(repo => repo.DeleteCommentLikeAsync(deleteLikeRequest.CommentId, 
            deleteLikeRequest.IdentityUserId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteCommentLikeAsync_LikeNotFound_ThrowsNotFoundException()
    {

        var deleteLikeRequest = new DeleteLikeRequestDTO
        (
            CommentId: Guid.NewGuid(),
            IdentityUserId: "user123"
        );

        var cancellationToken = CancellationToken.None;

        var mockImageService = new Mock<IImagesService>();
        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.GetCommentLikeByIdAsync(deleteLikeRequest.CommentId, deleteLikeRequest.IdentityUserId, cancellationToken))
                      .ReturnsAsync(() => null);
        var userId = Guid.NewGuid();

        var mockMapper = new Mock<IMapper>();

        var service = new MoviePostService(mockRepository.Object, mockImageService.Object, mockMapper.Object);

        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await service.DeleteCommentLikeAsync(deleteLikeRequest, cancellationToken);
        });

        mockRepository.Verify(repo => repo.DeleteCommentLikeAsync(deleteLikeRequest.CommentId, deleteLikeRequest.IdentityUserId, cancellationToken), Times.Never);
    }

}
