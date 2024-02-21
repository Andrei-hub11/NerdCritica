

using NerdCritica.Domain.Utils;
using System.Text.RegularExpressions;

namespace NerdCritica.Domain.Entities;

public class ExtensionUserIdentity
{
    public string UserName { get; set; }
    public string Email { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public string ProfileImagePath { get; private set; } = string.Empty;
    public byte[] ProfileImage { get; private set; } = new byte[0];
    public List<string> Roles { get; private set; } = new List<string>();
    public DateTime LastAccessDate { get; private set; } = DateTimeHelper.NowInBrasilia();

    private ExtensionUserIdentity(string userName,string email, string password, string profileImagePath,
        byte[] profileImage, List<string> roles)
    {
        UserName = userName;
        Email = email;
        Password = password;
        ProfileImagePath = profileImagePath;
        ProfileImage = profileImage;
        Roles = roles;
    }

    public static Result<ExtensionUserIdentity> Create(string userName, string email, string password, byte[] profileImage, 
        string profileImagePath, List<string> roles)
    {
        var result = UserValidation(userName, email, password, roles, profileImage);

        if (result.Count > 0)
        {
            var emptyUser = new ExtensionUserIdentity(string.Empty, string.Empty, string.Empty, 
                string.Empty, new byte[0], new List<string>());
            return Result.AddErrors(result, emptyUser);
        }

        var user = new ExtensionUserIdentity(userName, email, password, profileImagePath, 
            profileImage, roles);

        return Result.Ok(user);
    }

    private static List<Error> UserValidation(string userName, string email, string password, List<string> roles,
        byte[] imageProfile)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(userName))
        {
            errors.Add(new Error("O nome do usúario é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            errors.Add(new Error("O email é obrigatório."));
        }

        if (!Regex.IsMatch(email, @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$"))
        {
            errors.Add(new Error("Email inválido."));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add(new Error("A senha é obrigatória."));
        }

        if (password.Length < 8)
        {
            errors.Add(new Error("A senha deve ter pelo menos oito caracteres."));
        }

        if (!Regex.IsMatch(password,
            @"(?:.*[!@#$%^&*]){2,}"))
        {
            errors.Add(new Error("Senha inválida. A senha deve ter pelo menos dois caracteres especiais."));
        }

        if (imageProfile?.Length > 2 * 1024 * 1024)
        {
            errors.Add(new Error("A imagem não pode ter mais que dois 2 megabytes de tamanho."));
        }

        if (!roles.Contains("User") && !roles.Contains("Moderator") && !roles.Contains("Admin"))
        {
            errors.Add(new Error("O role fornecido não é válido."));
        }

        return errors;
    }
}
