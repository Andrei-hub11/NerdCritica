CREATE OR ALTER TRIGGER TriggerPreventDuplicateCommentLike
ON CommentLike
INSTEAD OF INSERT
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN CommentLike cl ON i.IdentityUserId = cl.IdentityUserId AND i.CommentId = cl.CommentId
    )
    BEGIN
        RAISERROR ('J� existe um CommentLike para este IdentityUserId e CommentId.', 16, 1);
        RETURN; -- Retorna sem fazer a inser��o
    END;

    -- Se n�o houver duplicatas, faz a inser��o normalmente
    INSERT INTO CommentLike (LikeId, IdentityUserId, CommentId)
    SELECT LikeId, IdentityUserId, CommentId
    FROM inserted;
END;
