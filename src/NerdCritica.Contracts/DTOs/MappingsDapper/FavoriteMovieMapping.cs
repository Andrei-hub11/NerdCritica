namespace NerdCritica.Contracts.DTOs.MappingsDapper;

public class FavoriteMovieMapping
{
    public Guid MoviePostId { get; set; }
    public Guid FavoriteMovieId { get; set; }
    public string MovieImagePath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
