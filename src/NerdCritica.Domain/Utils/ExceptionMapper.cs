using NerdCritica.Domain.Utils.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace NerdCritica.Domain.Utils;

public class CreateUserErrorHelper
{
    [return: NotNull]
    public static Exception GetExceptionFromResult(Result result)
    {
        if (result.Errors == null || !result.Errors.Any())
        {
            throw new ArgumentException("O método foi chamado de forma errada: a lista de erros está vazia.");
        }

        var exception = result.Errors.Select(error =>
              error.Description switch
              {
                  "DuplicateUserName" => new CreateUserException("O nome de usuário já está em uso. Escolha outro nome de usuário."),
                  "DuplicateEmail" => new CreateUserException("O e-mail já está em uso. Utilize outro endereço de e-mail."),
                  _ => null // Caso padrão para garantir que sempre haja uma exceção
              }).FirstOrDefault();

        return exception ?? new CreateUserException("Algo deu errado ao criar o usuário.");
    }
}
