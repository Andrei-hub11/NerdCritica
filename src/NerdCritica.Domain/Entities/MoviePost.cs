using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdCritica.Domain.Entities;

public class MoviePost
{
  public Guid MoviePostId { get; private set; }
  public string MoviePostImagePath { get; private set; } = string.Empty;
  public byte[] MoviePostImage { get; private set; } = new byte[0];
  public string MoviePostTitle { get; private set; } = string.Empty;
  public string MoviePostDescription { get; private set; } = string.Empty;
  public string Category { get; private set; } = string.Empty;


    private MoviePost(Guid moviePostId, string profileImagePath, byte[] profileImage, 
        string moviePostTitle, string moviePostDescription, string category)
    {
        MoviePostId = moviePostId;
        MoviePostImagePath = profileImagePath;
        MoviePostImage = profileImage;
        MoviePostTitle = moviePostTitle;
        MoviePostDescription = moviePostDescription;
        Category = category;
    }

    public static MoviePost Create(Guid moviePostId, string moviePostImagePath, byte[] moviePostImage,
        string moviePostTitle, string moviePostDescription, string category)
    {
      return new MoviePost(moviePostId, moviePostImagePath, moviePostImage,
        moviePostTitle, moviePostDescription, category);
    }

    public static void Update(MoviePost moviePost, byte[] moviePostImage,
        string moviePostTitle, string moviePostDescription, string category)
    {
        moviePost.MoviePostTitle = moviePostTitle;
        moviePost.MoviePostImage = moviePostImage;
        moviePost.MoviePostDescription = moviePostDescription;
        moviePost.Category = category;
    }
}
