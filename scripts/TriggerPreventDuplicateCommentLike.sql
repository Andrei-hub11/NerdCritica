CREATE OR ALTER TRIGGER TriggerPreventDuplicateCommentLike
ON CommentLike
AFTER INSERT
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN CommentLike cl ON i.IdentityUserId = cl.IdentityUserId AND i.CommentId = cl.CommentId
    )
    BEGIN
        RAISERROR ('Já existe um CommentLike para este IdentityUserId e CommentId.', 16, 1);
        ROLLBACK TRANSACTION;
    END;
END;
