CREATE TRIGGER TriggerMoviePostUpdate
ON MoviePost
AFTER UPDATE
AS
BEGIN
    IF UPDATE(MovieTitle) OR UPDATE(MovieImage) OR UPDATE(MovieImagePath) OR UPDATE(MovieDescription) 
	OR UPDATE(MovieBackdropImage) OR UPDATE(MovieBackdropImagePath) OR UPDATE(Director) OR UPDATE(MovieCategory) OR UPDATE(ReleaseDate) 
	OR UPDATE(Runtime)
    BEGIN
        UPDATE MoviePost
        SET UpdatedAt = GETDATE()
        FROM MoviePost
        INNER JOIN inserted ON MoviePost.MoviePostId = inserted.MoviePostId;
    END
END;