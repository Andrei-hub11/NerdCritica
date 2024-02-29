namespace NerdCritica.Domain.DTOs.MappingsDapper;

public class MoviePostMapping
{
    public Guid MoviePostId { get; set; }
    //public string CreatorUserId { get; set; } = string.Empty;
    public string MovieImagePath { get; set; } = string.Empty;
    public string MovieBackdropImagePath { get; set; } = string.Empty;
    public string MovieTitle { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public string MovieDescription { get; set; } = string.Empty;
    public string MovieCategory { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public int Runtime { get; set; }
    public ICollection<CommentsMapping> Comments { get; set; } = new List<CommentsMapping>();
    public ICollection<CastMemberMapping> Cast { get; set; } = new List<CastMemberMapping>();
}
