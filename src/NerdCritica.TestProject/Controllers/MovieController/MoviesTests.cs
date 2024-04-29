using AutoMapper;
using Moq;
using NerdCritica.Application.Services.Movies;
using NerdCritica.Domain.Contracts;
using NerdCritica.Domain.DTOs.MappingsDapper;
using NerdCritica.Domain.DTOs.Movie;
using NerdCritica.Domain.Entities;
using NerdCritica.Domain.Repositories.Movies;
using NerdCritica.Domain.Utils.Exceptions;

namespace NerdCritica.TestProject.Controllers.MovieController;

public class MoviesTests
{

    [Fact]
    public async Task CreateRatingAsync_ValidRequest_ReturnsTrue()
    {
        var ratingId = Guid.NewGuid();
        var moviePostId = Guid.NewGuid();
        var identityUserId = Guid.NewGuid().ToString();
        var ratingValue = 4.5m;
        var commentContent = "Great movie!";

        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.CreateRatingAsync(It.IsAny<MovieRating>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(ratingId);
        mockRepository.Setup(repo => repo.CreateCommentAsync(It.IsAny<Comment>()))
                      .ReturnsAsync(true);

        var mockMapper = new Mock<IMapper>();
        var mockUserContext = new Mock<IUserContext>();
        mockUserContext.Setup(context => context.UserId).Returns(Guid.NewGuid());

        var service = new MoviePostService(mockRepository.Object, mockMapper.Object, mockUserContext.Object);

        var request = new CreateRatingRequestDTO
        (
            MoviePostId: moviePostId,
            IdentityUserId: identityUserId,
            Rating: ratingValue,
            Comment: new CreateCommentDTO ( IdentityUserId: identityUserId, Content: commentContent )
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

        var mockRepository = new Mock<IMoviePostRepository>();
        var mockMapper = new Mock<IMapper>();
        var mockUserContext = new Mock<IUserContext>();
        mockUserContext.Setup(context => context.UserId).Returns(Guid.NewGuid());

        var service = new MoviePostService(mockRepository.Object, mockMapper.Object, mockUserContext.Object);

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

        var mockRepository = new Mock<IMoviePostRepository>();
        var mockMapper = new Mock<IMapper>();
        var mockUserContext = new Mock<IUserContext>();
        mockUserContext.Setup(context => context.UserId).Returns(Guid.NewGuid());

        var service = new MoviePostService(mockRepository.Object, mockMapper.Object, mockUserContext.Object);

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

        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.GetRatingByIdAsync(movieRatingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MovieRatingMapping { Comment = new CommentsMapping { CommentId = commentId } });
        mockRepository.Setup(repo => repo.CreateCommentLikeAsync(commentId, userId.ToString(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var mockMapper = new Mock<IMapper>();
        var mockUserContext = new Mock<IUserContext>();
        mockUserContext.Setup(context => context.UserId).Returns(userId);

        var service = new MoviePostService(mockRepository.Object, mockMapper.Object, mockUserContext.Object);

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

        var mockRepository = new Mock<IMoviePostRepository>();

        mockRepository.Setup(repo => repo.GetRatingByIdAsync(movieRatingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        var mockMapper = new Mock<IMapper>();
        var mockUserContext = new Mock<IUserContext>();
        mockUserContext.Setup(context => context.UserId).Returns(userId);

        var service = new MoviePostService(mockRepository.Object, mockMapper.Object, mockUserContext.Object);

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

        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.GetRatingByIdAsync(movieRatingId, It.IsAny<CancellationToken>()))
          .ReturnsAsync(new MovieRatingMapping { Comment = new CommentsMapping 
          { CommentId = Guid.NewGuid()} });

        var mockMapper = new Mock<IMapper>();
        var mockUserContext = new Mock<IUserContext>();
        mockUserContext.Setup(context => context.UserId).Returns(userId);

        var service = new MoviePostService(mockRepository.Object, mockMapper.Object, mockUserContext.Object);

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

        var mockRepository = new Mock<IMoviePostRepository>();
        mockRepository.Setup(repo => repo.GetRatingByIdAsync(movieRatingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MovieRatingMapping { Comment = new CommentsMapping { CommentId = commentId } });

        var mockMapper = new Mock<IMapper>();
        var mockUserContext = new Mock<IUserContext>();
        mockUserContext.Setup(context => context.UserId).Returns(userId);

        var service = new MoviePostService(mockRepository.Object, mockMapper.Object, mockUserContext.Object);

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

}
