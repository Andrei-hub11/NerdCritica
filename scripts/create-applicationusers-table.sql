CREATE TABLE ApplicationUsers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    IdentityUserId NVARCHAR(450) NOT NULL,
    ProfileImage VARBINARY(MAX),
	ProfileImagePath NVARCHAR(MAX),
    LastAccessDate DATETIME NOT NULL,
    CONSTRAINT FK_User_UserIdentity FOREIGN KEY (IdentityUserId) REFERENCES dbo.AspNetUsers(Id),
	CONSTRAINT Check_Image_Size CHECK (DATALENGTH(ProfileImage) <= 3145728)
);
