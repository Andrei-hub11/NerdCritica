

using NerdCritica.Domain.Utils.Exceptions;

namespace NerdCritica.Domain.Utils;

public class ExceptionMapper
{
    public static Exception GetExceptionFromResult(Result result)
    {
        var exception = result.Errors.Select(error =>
            error.Description switch
            {
                "DuplicateUserName" => new CreateUserException("O nome de usuário já está em uso. Escolha outro nome de usuário."),
                "DuplicateEmail" => new CreateUserException("O e-mail já está em uso. Utilize outro endereço de e-mail."),
                _ => new CreateUserException("Algo deu errado ao criar o usuário.")
            }).FirstOrDefault();

        return exception;
    }
}
