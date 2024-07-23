namespace NerdCritica.Contracts.DTOs.Movie;

public record UpdateCastMemberRequestDTO(Guid CastMemberId,string MemberName, string CharacterName,
    string MemberImage, int RoleType);
