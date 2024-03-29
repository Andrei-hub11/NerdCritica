/*
Trigger de inser��o para n�o permitir o campo CharacterName como nulo, se for ator/atriz,
e de modo semelhante, ir� atribuir um valor default ao RoleInMovie, se for atriz/ator.
Mas exigir� a atribui��o manual do campo RoleInMovie, se RoleType n�o for 1 ou 2, ou seja, atriz/ator.
*/
CREATE TRIGGER TriggerInsertCastMember
ON CastMember
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO CastMember (MoviePostId, MemberName, CharacterName, MemberImage, MemberImagePath, RoleInMovie, RoleType)
    SELECT 
	    MoviePostId,
        MemberName, 
        CASE WHEN i.RoleType IN (1, 2, 3, 4, 5) AND i.CharacterName IS NOT NULL THEN i.CharacterName ELSE 'N�o � ator/atriz.' END,
        MemberImage, 
        MemberImagePath,
		RoleInMovie,
        RoleType
    FROM inserted i
END;
