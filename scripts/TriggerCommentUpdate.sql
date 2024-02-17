CREATE TRIGGER TriggerCommentUpdate
ON Comment
AFTER UPDATE
AS
BEGIN
    IF UPDATE(Content) OR UPDATE(RatingId) OR UPDATE(IdentityUserId)
    BEGIN
        UPDATE Comment
        SET UpdatedAt = GETDATE()
        FROM Comment
        INNER JOIN inserted ON Comment.CommentId = inserted.CommentId;
    END
END;