/*
Trigger de inserção para não permitir o campo CharacterName como nulo, mesmo que para membros que não sejam do tipo ator;
*/
CREATE TRIGGER TriggerInsertCastMember
ON CastMember
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Inserção apenas se CharacterName não estiver presente em inserted e não for vazio ou espaço em branco
    INSERT INTO CastMember (MoviePostId, MemberName, CharacterName, MemberImage, MemberImagePath, RoleInMovie, RoleType)
    SELECT 
        i.MoviePostId,
        i.MemberName, 
        CASE WHEN i.RoleType IN (1, 2, 3, 4, 5) AND NULLIF(LTRIM(RTRIM(i.CharacterName)), '') IS NOT NULL THEN i.CharacterName 
		ELSE 'Não é ator/atriz.' END,
        i.MemberImage, 
        i.MemberImagePath,
        i.RoleInMovie,
        i.RoleType
    FROM inserted i;
END;