namespace NerdCritica.Contracts.DTOs.MappingsDapper;

public class MovieRatingMapping
{
    public Guid RatingId { get; set; }
    public string IdentityUserId { get; set; } = string.Empty;
    public decimal Rating {  get; set; }
    public CommentsMapping Comment { get; set; } = new CommentsMapping();
}
