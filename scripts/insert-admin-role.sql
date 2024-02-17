-- Verificar se o role 'Admin' j� existe
IF NOT EXISTS (SELECT * FROM dbo.AspNetRoles WHERE Name = 'Admin')
BEGIN
    INSERT INTO dbo.AspNetRoles (Id, Name, NormalizedName)
    VALUES (NEWID(), 'Admin', 'ADMIN')
    
    PRINT 'Role ''Admin'' criado com sucesso!'
END
ELSE
BEGIN
    PRINT 'Role ''Admin'' j� existe.'
END
