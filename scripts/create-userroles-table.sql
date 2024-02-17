CREATE TABLE UserRoles (
	IdentityUserId NVARCHAR(450) NOT NULL,
    RoleName VARCHAR(100) NOT NULL,
	CONSTRAINT FK_Roles_UserRoles FOREIGN KEY (IdentityUserId) REFERENCES dbo.AspNetUsers(Id),
	CONSTRAINT Check_RoleName CHECK (RoleName IN ('Admin', 'Moderator', 'User')),
	CONSTRAINT UC_IdentityUserId UNIQUE (IdentityUserId) -- Restri��o para garantir que cada IdentityUserId seja �nico e n�o tenha mais de uma fun��o
);
