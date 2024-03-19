CREATE TRIGGER DeleteMoviepostRelated
ON MoviePost
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Deletar registros de Comment relacionados aos MovieRating
    DELETE FROM Comment 
    WHERE RatingId IN (SELECT RatingId FROM MovieRating WHERE MoviePostId IN (SELECT deleted.MoviePostId FROM deleted));

    -- Deletar registros de MovieRating relacionados ao MoviePost
    DELETE FROM MovieRating 
    WHERE MoviePostId IN (SELECT MoviePostId FROM deleted);

    -- Deletar registros de CastMember relacionados ao MoviePost
    DELETE FROM CastMember 
    WHERE MoviePostId IN (SELECT MoviePostId FROM deleted);

	 -- Deletar MoviePost
    DELETE FROM MoviePost 
    WHERE MoviePostId IN (SELECT MoviePostId FROM deleted);
END;

