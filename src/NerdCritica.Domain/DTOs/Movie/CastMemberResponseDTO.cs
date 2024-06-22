namespace NerdCritica.Domain.DTOs.Movie;

public record CastMemberResponseDTO(
    Guid CastMemberId, 
    string MemberName, 
    string CharacterName, 
    string MemberImagePath, 
    string RoleInMovie, 
    int RoleType
    );