namespace Generator.Common.Records;

public class GeneratorResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }

    public static GeneratorResponse Ok(string? message = null) =>
        new() { Success = true, Message = message };
    
    public static GeneratorResponse Error(string message) =>
        new() { Success = false, Message = message };
}

public class GeneratorResponse<T> : GeneratorResponse
{
    public T? Data { get; set; }

    public static GeneratorResponse<T> Ok(T data, string? message = null) =>
        new() { Success = true, Message = message, Data = data };

    public static new GeneratorResponse<T> Error(string message) =>
        new() { Success = false, Message = message };
}
