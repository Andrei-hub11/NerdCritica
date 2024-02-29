/*
Trigger de inserção para não permitir o campo CharacterName como nulo, se for ator/atriz,
e de modo semelhante, irá atribuir um valor default ao RoleInMovie, se for atriz/ator.
Mas exigirá a atribuição manual do campo RoleInMovie, se RoleType não for 1 ou 2, ou seja, atriz/ator.
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
        CASE WHEN i.RoleType IN (1, 2, 3, 4, 5) AND i.CharacterName IS NOT NULL THEN i.CharacterName ELSE 'Não é ator/atriz.' END,
        MemberImage, 
        MemberImagePath,
		RoleInMovie,
        RoleType
    FROM inserted i
END;
