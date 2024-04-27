using NerdCritica.Domain.Contracts;

namespace NerdCritica.Domain.DTOs.MappingsDapper;

public class CommentLikeMapping
{
    public Guid CommentLikeId { get; set; }
    public Guid CommentId { get; set; }
    public string IdentityUserId { get; set;} = string.Empty;
}
