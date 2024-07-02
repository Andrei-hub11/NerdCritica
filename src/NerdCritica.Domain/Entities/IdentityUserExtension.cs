using NerdCritica.Domain.Utils;
using System.Text.RegularExpressions;

namespace NerdCritica.Domain.Entities;

public class IdentityUserExtension
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; }
    public string Email { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public string ProfileImagePath { get; private set; } = string.Empty;
    public byte[] ProfileImage { get; private set; } = new byte[0];
    public List<string> Roles { get; private set; } = new List<string>();
    public DateTime LastAccessDate { get; private set; } = DateTime.Now;

    private IdentityUserExtension(string userName,string email, string password, string profileImagePath,
        byte[] profileImage, List<string> roles)
    {
        UserName = userName;
        Email = email;
        Password = password;
        ProfileImagePath = profileImagePath;
        ProfileImage = profileImage;
        Roles = roles;
    }

    private IdentityUserExtension(string userName, string email, string profileImagePath, byte[] profileImage)
    {
        UserName = userName;
        Email = email;
        ProfileImagePath = profileImagePath;
        ProfileImage = profileImage;
    }

    public static Result<IdentityUserExtension> Create(string userName, string email, string password, byte[] profileImage, 
        string profileImagePath, List<string> roles)
    {
        var isCreate = true;
        var result = UserValidation(userName, email, profileImage, isCreate, password, roles);

        if (result.Count > 0)
        {
            return Result.Fail(result);
        }

        var user = new IdentityUserExtension(userName, email, password, profileImagePath, 
            profileImage, roles);

        return Result.Ok(user);
    }

    public static Result<IdentityUserExtension> From(string userName, 
        string email, byte[] profileImage,
     string profileImagePath)
    {
        var isCreate = false;
        var result = UserValidation(userName, email, profileImage, isCreate);

        if (result.Count > 0)
        {
            return Result.Fail(result);
        }

        return Result.Ok(new IdentityUserExtension(userName, email, profileImagePath, profileImage));
    }
        private static List<Error> UserValidation(string userName, string email,
        byte[] imageProfile, bool isCreate, string? password = null, List<string>? roles = null)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(userName))
        {
            errors.Add(new Error("O nome do usúario é obrigatório"));
        }

        errors.AddRange(ValidateEmail(email));

        if (isCreate && password != null)
        {
            errors.AddRange(ValidatePassword(password));
        }

        if (imageProfile?.Length > 2 * 1024 * 1024)
        {
            errors.Add(new Error("A imagem não pode ter mais que dois 2 megabytes de tamanho"));
        }

        if (isCreate && roles != null && !roles.Contains("User") && 
                !roles.Contains("Moderator") && !roles.Contains("Admin"))
        {
            errors.Add(new Error("O role fornecido não é válido"));
        }

        return errors;
    }

    public static List<Error> ValidateEmail(string email)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(email))
        {
            errors.Add(new Error("O email é obrigatório"));
        }

        if (!Regex.IsMatch(email, @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$"))
        {
            errors.Add(new Error("Email inválido"));
        }

        return errors;
    }

    public static List<Error> ValidatePassword(string password)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add(new Error("A senha é obrigatória"));
        }

        if (password.Length < 8)
        {
            errors.Add(new Error("A senha deve ter pelo menos oito caracteres"));
        }

        if (!Regex.IsMatch(password, @"(?:.*[!@#$%^&*]){2,}"))
        {
            errors.Add(new Error("Senha inválida. A senha deve ter pelo menos dois caracteres especiais"));
        }

        return errors;
    }
}
