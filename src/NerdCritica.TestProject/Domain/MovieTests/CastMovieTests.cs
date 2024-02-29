

using NerdCritica.Domain.Entities.Aggregates;

namespace NerdCritica.TestProject.Domain.MovieTests;

public class CastMovieTests
{
    [Theory(DisplayName = "Create should return success result with valid data")]
    [InlineData("Actor Name", "Character Name", new byte[] { 1 }, "image.jpg", 1, "Protagonista")]
    [InlineData("Actor Name", "Character Name", new byte[] { 4 }, "image.jpg", 2, "Coadjuvante")]
    [InlineData("Actor Name", "Character Name", new byte[] { 4 }, "image.jpg", 3, "Antagonista")]
    [InlineData("Actor Name", "Character Name", new byte[] { 4 }, "image.jpg", 4, "Ator/Atriz de cárater")]
    [InlineData("Actor Name", "Character Name", new byte[] { 4 }, "image.jpg", 5, "Elenco de apoio")]
    [InlineData("Actor Name", "Character Name", new byte[] { 4 }, "image.jpg", 6, "Diretor")]
    [InlineData("Actor Name", "Character Name", new byte[] { 4 }, "image.jpg", 7, "Escritor")]
    [InlineData("Actor Name", "Character Name", new byte[] { 4 }, "image.jpg", 8, "Compositor")]
    [InlineData("Actor Name", "Character Name", new byte[] { 4 }, "image.jpg", 9, "Produtor")]
    [InlineData("Actor Name", "Character Name", new byte[] { 4 }, "image.jpg", 10, "Desconhecido")]
    public void Create_WithValidData_ShouldReturnSuccessResult(string memberName, string characterName, 
        byte[] memberImage, string memberImagePath, int roleType, string expectedRoleInMovie)
    {
        var result = CastMember.Create(memberName, characterName, memberImage, memberImagePath, roleType);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(memberName, result.Value.MemberName);
        Assert.Equal(characterName, result.Value.CharacterName);
        Assert.Equal(memberImage ?? new byte[0], result.Value.MemberImage);
        Assert.Equal(memberImagePath, result.Value.MemberImagePath);
        Assert.Equal(roleType, result.Value.RoleType);
        Assert.Equal(expectedRoleInMovie, result.Value.RoleInMovie);
    }

    [Theory(DisplayName = "Create should return failure result with invalid data")]
    [MemberData(nameof(GetInvalidCastMovieTestData))]
    public void Create_WithInvalidData_ShouldReturnFailureResult(string memberName, string characterName, 
        byte[] memberImage, string memberImagePath, int roleType, string expectedErrorMessage)
    {
        var result = CastMember.Create(memberName, characterName, memberImage, memberImagePath, roleType);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(expectedErrorMessage, result.Errors.Select(e => e.Description));
    }

    public static IEnumerable<object[]> GetInvalidCastMovieTestData()
    {
        yield return new object[] { "", "Character Name", new byte[1], "image.jpg", 1, 
            "O nome do membro de elenco não pode estar vazio." };
        yield return new object[] { "Actor Name", "", new byte[1], "image.jpg", 1, 
            "O nome do personagem não pode estar vazio, porque é um membro do tipo ator." };
        yield return new object[] { "Actor Name", "Character Name", 
            new byte[2 * 1024 * 1024 + 1], "image.jpg", 1, "A imagem do membro não pode ter mais que dois 2 megabytes de tamanho." };
        yield return new object[] { "Actor Name", "Character Name", 
            new byte[1], "", 1, "O caminho da imagem do membro não pode estar vazio." };
        yield return new object[] { "Actor Name", "", 
            new byte[1], "image.jpg", 1, "O nome do personagem não pode estar vazio, porque é um membro do tipo ator." };
        yield return new object[] { "Actor Name", "", 
            new byte[1], "image.jpg", 2, "O nome do personagem não pode estar vazio, porque é um membro do tipo ator." };
        yield return new object[] { "Actor Name", "",
            new byte[1], "image.jpg", 3, "O nome do personagem não pode estar vazio, porque é um membro do tipo ator." };
        yield return new object[] { "Actor Name", "",
            new byte[1], "image.jpg", 4, "O nome do personagem não pode estar vazio, porque é um membro do tipo ator." };
        yield return new object[] { "Actor Name", "",
            new byte[1], "image.jpg", 5, "O nome do personagem não pode estar vazio, porque é um membro do tipo ator." };
    }
}
