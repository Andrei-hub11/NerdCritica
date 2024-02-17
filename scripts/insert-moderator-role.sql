-- Verificar se o role 'Admin' j� existe
IF NOT EXISTS (SELECT * FROM dbo.AspNetRoles WHERE Name = 'Moderator')
BEGIN
    INSERT INTO dbo.AspNetRoles (Id, Name, NormalizedName)
    VALUES (NEWID(), 'Moderator', 'MODERATOR')
    
    PRINT 'Role ''Moderator'' criado com sucesso!'
END
ELSE
BEGIN
    PRINT 'Role ''Moderator'' j� existe.'
END
