using NerdCritica.Domain.Utils.Exceptions;
using NerdCritica.Domain.Utils;

namespace NerdCritica.TestProject.Helpers;

public class CreateUserErrorHelperTests
{
    [Fact]
    public void GetExceptionFromResult_NoErrors_ReturnsGenericException()
    {
        var result = Result.Ok(true);

        Assert.Throws<ArgumentException>(() => CreateUserErrorHelper.GetExceptionFromResult(result));
    }

    [Fact]
    public void GetExceptionFromResult_DuplicateUserName_ReturnsCreateUserExceptionWithCorrectMessage()
    {
        var result = Result.Fail("DuplicateUserName");

        var exception = CreateUserErrorHelper.GetExceptionFromResult(result);

        Assert.IsType<CreateUserException>(exception);
        Assert.Equal("O nome de usuário já está em uso. Escolha outro nome de usuário.", exception.Message);
    }

    [Fact]
    public void GetExceptionFromResult_DuplicateEmail_ReturnsCreateUserExceptionWithCorrectMessage()
    {
        var result = Result.Fail("DuplicateEmail");


        var exception = CreateUserErrorHelper.GetExceptionFromResult(result);

        Assert.IsType<CreateUserException>(exception);
        Assert.Equal("O e-mail já está em uso. Utilize outro endereço de e-mail.", exception.Message);
    }

    [Fact]
    public void GetExceptionFromResult_UnknownError_ReturnsGenericCreateUserException()
    {
        var result = Result.Fail("UnknownError");

        var exception = CreateUserErrorHelper.GetExceptionFromResult(result);

        Assert.IsType<CreateUserException>(exception);
        Assert.Equal("Algo deu errado ao criar o usuário.", exception.Message);
    }
}
