namespace Minimal.Application.Common.Results;

public class ServiceResult<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string? Error { get; }
    public List<string> Errors { get; } = new();

    private ServiceResult(bool isSuccess, T? data, string? error, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
        if (errors != null) Errors = errors;
    }

    // Factory methods
    public static ServiceResult<T> Success(T data)
        => new(true, data, null);

    public static ServiceResult<T> Failure(string error)
        => new(false, default, error);

    public static ServiceResult<T> Failure(List<string> errors)
        => new(false, default, errors.FirstOrDefault(), errors);
}
