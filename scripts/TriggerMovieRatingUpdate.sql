
CREATE TRIGGER TriggerMovieRatingUpdate
ON MovieRating
AFTER UPDATE
AS
BEGIN
    IF UPDATE(Rating) OR UPDATE(MoviePostId) OR UPDATE(IdentityUserId)
    BEGIN
        UPDATE MovieRating
        SET UpdatedAt = GETDATE()
        FROM MovieRating
        INNER JOIN inserted ON MovieRating.RatingId = inserted.RatingId;
    END
END;
