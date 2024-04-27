CREATE TRIGGER UpdateUpdatedAt
ON FavoriteMovie
AFTER INSERT
AS
BEGIN
    -- Atualizar registros existentes com o mesmo IdentityUserId
    UPDATE f
    SET UpdatedAt = GETDATE()
    FROM FavoriteMovie f
    JOIN inserted i ON f.IdentityUserId = i.IdentityUserId;

    -- Atualizar registros recém-inseridos
    UPDATE f
    SET UpdatedAt = GETDATE()
    FROM FavoriteMovie f
    JOIN inserted i ON f.FavoriteMovieId = i.FavoriteMovieId;
END;

