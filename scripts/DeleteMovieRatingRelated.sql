CREATE TRIGGER DeleteMovieRatingRelated
ON MovieRating
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Deletar registros de Comment relacionados aos MovieRating
    DELETE FROM Comment 
    WHERE RatingId IN (SELECT RatingId FROM deleted);

	 -- Deletar MovieRating
    DELETE FROM MovieRating 
    WHERE RatingId IN (SELECT RatingId FROM deleted);
END;