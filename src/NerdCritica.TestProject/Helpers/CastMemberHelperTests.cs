using NerdCritica.Domain.Common;
using NerdCritica.Domain.Utils;
using NerdCritica.Contracts.DTOs.Movie;

namespace NerdCritica.TestProject.Helpers;

public class CastMemberHelperTests
{
    [Fact]
    public void GetCast_NullCastList_ThrowsArgumentException()
    {
        List<CastMemberRequestDTO> cast = new List<CastMemberRequestDTO>();
        var castImagePaths = new Dictionary<string, CastImages>();

        Assert.Throws<ArgumentException>(() => CastMemberHelper.GetCast(cast, castImagePaths));
    }

    [Fact]
    public void GetCast_EmptyCastList_ThrowsArgumentException()
    {
        List<CastMemberRequestDTO> cast = new List<CastMemberRequestDTO>();
        var castImagePaths = new Dictionary<string, CastImages>();

        Assert.Throws<ArgumentException>(() => CastMemberHelper.GetCast(cast, castImagePaths));
    }

    [Fact]
    public void GetCast_NullCastImagePaths_ThrowsArgumentException()
    {
        var cast = new List<CastMemberRequestDTO>
    {
        new CastMemberRequestDTO
        (
            MemberName: "Actor Name",
            CharacterName: "Character Name",
            MemberImage: "image",
            RoleType: 1
        )
    };
        Dictionary<string, CastImages> castImagePaths = new Dictionary<string, CastImages>();

        Assert.Throws<ArgumentException>(() => CastMemberHelper.GetCast(cast, castImagePaths));
    }

    [Fact]
    public void GetCast_ValidInput_GeneratesCorrectCastMembers()
    {
        var cast = new List<CastMemberRequestDTO>
    {
        new CastMemberRequestDTO
        (
            MemberName: "Actor Name",
            CharacterName: "Character Name",
            MemberImage: "image",
            RoleType: 1
        )
    };

        var castImage = CastImages.Create("users/images/" + Guid.NewGuid(), new byte[10]);

        var castImagePaths = new Dictionary<string, CastImages>
    {
        { "Actor Name", castImage }
    };

        var result = CastMemberHelper.GetCast(cast, castImagePaths);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Actor Name", result[0].MemberName);
    }
}
