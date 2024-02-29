using NerdCritica.Domain.Utils;

namespace NerdCritica.Domain.Entities.Aggregates;

public class CastMember
{
    public Guid CastMemberId { get; private set; } = Guid.Empty;
    public string MemberName { get; private set; } = string.Empty;
    public string CharacterName { get; private set; } = string.Empty;
    public byte[] MemberImage { get; private set; } = new byte[0];
    public string MemberImagePath { get; private set; } = string.Empty;
    public string RoleInMovie => RoleTypeToRoleInMovieMap.TryGetValue(RoleType, out var role) ? role : "Desconhecido";
    public int RoleType { get; private set; } = 0;

    private CastMember(string memberName, string characterName, byte[] memberImage, string memberImagePath
        , int roleType)
    {
        MemberName = memberName;
        CharacterName = characterName;
        MemberImage = memberImage;
        MemberImagePath = memberImagePath;
        RoleType = roleType;
    }

    private CastMember(string memberName, string characterName, byte[] memberImage, string memberImagePath
       , int roleType, Guid castMemberId)
    {
        MemberName = memberName;
        CharacterName = characterName;
        MemberImage = memberImage;
        MemberImagePath = memberImagePath;
        RoleType = roleType;
        CastMemberId = castMemberId;
    }

    public static Result<CastMember> Create(string memberName, string characterName, byte[] memberImage,
        string memberImagePath, int roleType)
    {
        var isCreate = true;

        var result = CommentValidation(memberName, characterName, memberImage, memberImagePath, 
            roleType, isCreate);

        if (result.Count > 0)
        {
            var emptyCastMovie = new CastMember(string.Empty, string.Empty, new byte[0], string.Empty, 0);
            return Result.AddErrors(result, emptyCastMovie);
        }

        var castMovie = new CastMember(memberName, characterName, memberImage, memberImagePath, roleType);

        return Result.Ok(castMovie);
    }

    public static Result<CastMember> From(string memberName, string characterName, byte[] memberImage,
       string memberImagePath, int roleType, Guid castMemberId)
    {
        var isCreate = false;

        var result = CommentValidation(memberName, characterName, memberImage, memberImagePath,
            roleType, isCreate, castMemberId);

        if (result.Count > 0)
        {
            var emptyCastMovie = new CastMember(string.Empty, string.Empty, new byte[0], string.Empty, 0);
            return Result.AddErrors(result, emptyCastMovie);
        }

        var castMovie = new CastMember(memberName, characterName, memberImage, memberImagePath, roleType,
            castMemberId);

        return Result.Ok(castMovie);
    }

    private static List<Error> CommentValidation(string memberName, string characterName, byte[] memberImage,
        string membroImagePath, int roleType, bool isCreate, Guid? castMemberId = null)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(memberName))
        {
            errors.Add(new Error("O nome do membro de elenco não pode estar vazio."));
        }

        if ((roleType == 1 || roleType == 2 || roleType == 3 || roleType == 4 || roleType == 5) && 
            string.IsNullOrWhiteSpace(characterName))
        {
            errors.Add(new Error("O nome do personagem não pode estar vazio, porque é um membro do tipo ator."));
        }

        if (memberImage?.Length > 2 * 1024 * 1024)
        {
            errors.Add(new Error("A imagem do membro não pode ter mais que dois 2 megabytes de tamanho."));
        }

        if (string.IsNullOrWhiteSpace(membroImagePath))
        {
            errors.Add(new Error("O caminho da imagem do membro não pode estar vazio."));
        }

        if (!isCreate && castMemberId == Guid.Empty || !isCreate && castMemberId == null)
        {
            errors.Add(new Error("O id do membro é necessario."));
        }

        return errors;
    }

    private static readonly Dictionary<int, string> RoleTypeToRoleInMovieMap = new Dictionary<int, string>
    {
        { 1, "Protagonista" },
        { 2, "Coadjuvante" },
        { 3, "Antagonista" },
        { 4, "Ator/Atriz de cárater" },
        { 5, "Elenco de apoio" },
        { 6, "Diretor" },
        { 7, "Escritor" },
        { 8, "Compositor" },
        { 9, "Produtor" }
    };
}
