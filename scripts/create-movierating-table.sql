CREATE TABLE MovieRating (
  RatingId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  MoviePostId UNIQUEIDENTIFIER NOT NULL,
  IdentityUserId NVARCHAR(450) NOT NULL,
  Rating DECIMAL(2, 1) NOT NULL CHECK (Rating >= 0 AND Rating <= 5),
  CreatedAt DATETIME DEFAULT GETDATE() NOT NULL,
  UpdatedAt DATETIME DEFAULT GETDATE() NOT NULL
  CONSTRAINT FK_MovieRating_UserIdentity FOREIGN KEY (IdentityUserId) REFERENCES dbo.AspNetUsers(Id),
  CONSTRAINT FK_MovieRating_MoviePost FOREIGN KEY (MoviePostId) REFERENCES dbo.MoviePost(MoviePostId)
)