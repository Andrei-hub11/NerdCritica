CREATE TABLE PasswordResetTokens (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    IdentityUserId NVARCHAR(450) NOT NULL,
    Token NVARCHAR(500) NOT NULL,
    ExpirationDate DATETIME NOT NULL,
    CONSTRAINT FK_Token_IdentityUser FOREIGN KEY ( IdentityUserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);