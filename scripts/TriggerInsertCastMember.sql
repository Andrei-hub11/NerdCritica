/*
Trigger de inser��o para n�o permitir o campo CharacterName como nulo, mesmo que para membros que n�o sejam do tipo ator;
*/
CREATE TRIGGER TriggerInsertCastMember
ON CastMember
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Inser��o apenas se CharacterName n�o estiver presente em inserted e n�o for vazio ou espa�o em branco
    INSERT INTO CastMember (MoviePostId, MemberName, CharacterName, MemberImage, MemberImagePath, RoleInMovie, RoleType)
    SELECT 
        i.MoviePostId,
        i.MemberName, 
        CASE WHEN i.RoleType IN (1, 2, 3, 4, 5) AND NULLIF(LTRIM(RTRIM(i.CharacterName)), '') IS NOT NULL THEN i.CharacterName 
		ELSE 'N�o � ator/atriz.' END,
        i.MemberImage, 
        i.MemberImagePath,
        i.RoleInMovie,
        i.RoleType
    FROM inserted i;
END;