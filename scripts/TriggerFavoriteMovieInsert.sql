CREATE TRIGGER ManageFavoriteMovieInsert
ON FavoriteMovie
AFTER INSERT
AS
BEGIN
    -- Atualizar registros existentes com o mesmo IdentityUserId
    UPDATE f
    SET UpdatedAt = GETDATE()
    FROM FavoriteMovie f
    JOIN inserted i ON f.IdentityUserId = i.IdentityUserId;

    -- Atualizar registros rec�m-inseridos
    UPDATE f
    SET UpdatedAt = GETDATE()
    FROM FavoriteMovie f
    JOIN inserted i ON f.FavoriteMovieId = i.FavoriteMovieId;

    -- Impedir a inser��o de FavoriteMovie com o mesmo MoviePostId e IdentityUserId
    IF EXISTS (
        SELECT 1
        FROM FavoriteMovie f
        JOIN inserted i ON f.MoviePostId = i.MoviePostId
                         AND f.IdentityUserId = i.IdentityUserId
        GROUP BY f.MoviePostId, f.IdentityUserId
        HAVING COUNT(*) > 1
    )
    BEGIN
        RAISERROR ('N�o � permitido inserir um FavoriteMovie com o mesmo MoviePostId e IdentityUserId.', 16, 1)
        ROLLBACK TRANSACTION;
        RETURN;
    END;
END;
