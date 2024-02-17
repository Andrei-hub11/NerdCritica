-- Verificar se o role 'Admin' j� existe
IF NOT EXISTS (SELECT * FROM dbo.AspNetRoles WHERE Name = 'User')
BEGIN
    INSERT INTO dbo.AspNetRoles (Id, Name, NormalizedName)
    VALUES (NEWID(), 'User', 'USER')
    
    PRINT 'Role ''User'' criado com sucesso!'
END
ELSE
BEGIN
    PRINT 'Role ''User'' j� existe.'
END
