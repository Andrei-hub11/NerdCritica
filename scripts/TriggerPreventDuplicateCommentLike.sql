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
        RAISERROR ('Já existe um CommentLike para este IdentityUserId e CommentId.', 16, 1);
        RETURN; -- Retorna sem fazer a inserção
    END;

    -- Se não houver duplicatas, faz a inserção normalmente
    INSERT INTO CommentLike (LikeId, IdentityUserId, CommentId)
    SELECT LikeId, IdentityUserId, CommentId
    FROM inserted;
END;
