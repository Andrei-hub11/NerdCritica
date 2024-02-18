

using NerdCritica.Domain.Utils;

namespace NerdCritica.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string IdentityUserId { get; private set; } = string.Empty;
    public string ProfileImagePath { get; private set; } = string.Empty;
    public byte[] ProfileImage { get; private set; } = new byte[0];
    public DateTime LastAccessDate { get; private set; } = DateTime.UtcNow;

    private User(string identityUserId, string profileImagePath, byte[] profileImage)
    {
        IdentityUserId = identityUserId;
        ProfileImagePath = profileImagePath;
        ProfileImage = profileImage;
    }

    public static Result<User> Create(string identityUserId, byte[] profileImage, 
        string profileImagePath)
    {
        var isCreate = true;
        var result = UserValidation(profileImage, isCreate, identityUserId);

        if (result.Count > 0)
        {
            var emptyUser = new User(string.Empty, string.Empty, new byte[0]);
            return Result.AddErrors(result, emptyUser);
        }

        var user = new User(identityUserId, profileImagePath, profileImage);

        return Result.Ok(user);
    }

    private static List<Error> UserValidation(byte[] imageProfile, bool isCreate, 
        string identityUserId)
    {
        var errors = new List<Error>();

        if (imageProfile?.Length > 2 * 1024 * 1024)
        {
            errors.Add(new Error("A imagem não pode ter mais que dois 2 megabytes de tamanho."));
        }

        if (isCreate && string.IsNullOrWhiteSpace(identityUserId))
        {
            errors.Add(new Error("O id do usuário não pode estar vazio"));
        }

        if (isCreate && !string.IsNullOrEmpty(identityUserId) &&
           !Guid.TryParse(identityUserId, out Guid result))
        {
            errors.Add(new Error($"{identityUserId} não é um id válido."));
        }

        return errors;
    }
}
