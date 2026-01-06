namespace HospitalAppointmentSystem.Application.Common.Models;

/// <summary>
/// Result pattern for operation outcomes
/// </summary>
public class Result
{
    public bool IsSuccess { get; protected set; }
    public string? Error { get; protected set; }
    public List<string>? Errors { get; protected set; }

    protected Result(bool isSuccess, string? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, error);
    public static Result Failure(List<string> errors) => new Result(false) { Errors = errors };
}

/// <summary>
/// Generic result with value
/// </summary>
public class Result<T> : Result
{
    public T? Value { get; protected set; }

    protected Result(bool isSuccess, T? value = default, string? error = null) : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value);
    public new static Result<T> Failure(string error) => new(false, default, error);
    public new static Result<T> Failure(List<string> errors) => new Result<T>(false) { Errors = errors };
}
