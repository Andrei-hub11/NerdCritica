using System.Diagnostics.CodeAnalysis;

namespace NerdCritica.Domain.Utils;

public class Result
{
    private readonly List<Error> _errors;

    protected Result(List<Error> error)
    {
        _errors = error;
    }

    public IReadOnlyList<Error> Errors => _errors;

    public static Result<List<Error>> Fail(string errorMessage)
    {
        return new Result<List<Error>>(new List<Error>(), true, new List<Error> { new Error(errorMessage) });
    }

    public static Result<List<Error>> Fail(List<Error> errors)
    {
        return new Result<List<Error>>(new List<Error>(), true, errors);
    }

    public static Result<T> Ok<T>(T value) => new Result<T>(value, false, new List<Error>());
}

public class Result<T> : Result
{
    public T? Value { get; }
    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailure { get; }
    public Error? Error
    {
        get
        {
            if (!IsFailure)
            {
                return new Error("Não há nenhum Error.");
            }

            return Errors[0];
        }
    }

    protected internal Result(T? value, bool success, List<Error> error)
        : base(error)
    {
        Value = value;
        IsFailure = success;
    }

    // Operador implícito para criar um Result<T> a partir de um valor
    public static implicit operator Result<T>(T value) => new Result<T>(value, false, new List<Error>());

    public static implicit operator Result<T>(string description) =>
     new Result<T>(default, false, new List<Error> { new Error(description) });

    public static implicit operator Result<T>(Result<List<Error>> errorResult)
    {
        return new Result<T>(default, true, errorResult.Errors.ToList());
    }
}