CREATE TABLE MoviePost (
    MoviePostId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CreatorUserId NVARCHAR(450) NOT NULL,
	MoviePostTitle NVARCHAR(150) NOT NULL,
    MoviePostImage VARBINARY(MAX) NOT NULL,
	MoviePostImagePath NVARCHAR(MAX) NOT NULL,
	MoviePostDescription NVARCHAR(MAX) NOT NULL,
	MovieCategory NVARCHAR(MAX) NOT NULL,
	CONSTRAINT FK_MoviePost_CreatorUser FOREIGN KEY (CreatorUserId) REFERENCES dbo.AspNetUsers(Id),
	CONSTRAINT Check_MinimumLength CHECK (LEN(MoviePostDescription) >= 150)
);
