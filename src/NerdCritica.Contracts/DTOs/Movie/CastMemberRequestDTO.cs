namespace NerdCritica.Contracts.DTOs.Movie;

public record CastMemberRequestDTO(string MemberName, string CharacterName, 
    string MemberImage, int RoleType);
