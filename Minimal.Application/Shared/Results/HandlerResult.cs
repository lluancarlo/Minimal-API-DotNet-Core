namespace Minimal.Application.Shared.Results;

/// <summary>
/// Represents the result of a business operation.
/// Use ONLY inside Application
/// </summary>
public class HandlerResult<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string? Error { get; }
    public List<string> Errors { get; } = new();

    private HandlerResult(bool isSuccess, T? data, string? error, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
        if (errors != null) Errors = errors;
    }

    // Factory methods
    public static HandlerResult<T> Success(T data)
        => new(true, data, null);

    public static HandlerResult<T> Failure(string error)
        => new(false, default, error);

    public static HandlerResult<T> Failure(List<string> errors)
        => new(false, default, errors.FirstOrDefault(), errors);
}

public class HandlerResult
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public List<string> Errors { get; } = new();

    private HandlerResult(bool isSuccess, string? error, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        if (errors != null) Errors = errors;
    }

    public static HandlerResult Success() => new(true, null);
    public static HandlerResult Failure(string error) => new(false, error);
    public static HandlerResult Failure(List<string> errors)
        => new(false, errors.FirstOrDefault(), errors);
}
