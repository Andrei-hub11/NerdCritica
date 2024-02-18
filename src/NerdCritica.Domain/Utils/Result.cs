
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NerdCritica.Domain.Utils;

public class Error
{
    public string Description { get; }

    public Error(string description)
    {
        Description = description;
    }
}

public class Result
{
    private List<Error> _errors;
    protected Result(bool success, List<Error> error)
    {
        Success = success;
        _errors = error;
    }

    public bool Success { get; }
    public IReadOnlyList<Error> Errors => _errors;
    public bool IsFailure => !Success;

    public static Result AddError(List<Error> errors)
    {
        return new Result(false, errors);
    }

    public static Result<T> AddErrors<T>(List<Error> errors, T defaultValue)
    {
        return new Result<T>(defaultValue,false, errors);
    }

    public static Result Fail(string description)
    {
        return new Result(false, new List<Error> { new Error(description) });
    }

    public static Result<T> Fail<T>(string description, T defaultValue)
    {
        return new Result<T>(defaultValue, false, new List<Error> { new Error(description) });
    }

    public static Result Ok()
    {
        return new Result(true, new List<Error>());
    }

    public static Result<T> Ok<T>(T value)
    {
        return new Result<T>(value, true, new List<Error>());
    }
}

public class Result<T> : Result
{
    protected internal Result(T value, bool success, List<Error> error)
        : base(success, error)
    {
        Value = value;
    }

    public T Value { get; set; }
}
