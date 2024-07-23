namespace NerdCritica.Contracts.DTOs.MappingsDapper;

public class CastMemberMapping
{
    public Guid CastMemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string CharacterName { get; set; } = string.Empty;
    public string MemberImagePath { get; set; } = string.Empty;
    public string RoleInMovie { get; set; } = string.Empty;
    public int RoleType { get; set; } = 0;
}
