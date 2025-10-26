using System.Net;

namespace Minimal.API.Endpoints;

/// <summary>
/// Represents the result of a HTTP operation.
/// Use ONLY inside API
/// </summary>
public class ApiResponse
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }

    public static ApiResponse Ok(string? message = null) =>
        new() { Success = true, Message = message, StatusCode = (int)HttpStatusCode.OK };

    public static ApiResponse NotFound(string message) =>
        new() { Success = false, Message = message, StatusCode = (int)HttpStatusCode.NotFound };

    public static ApiResponse BadRequest(string message) =>
        new() { Success = false, Message = message, StatusCode = (int)HttpStatusCode.BadRequest };

    public static ApiResponse Conflict(string message) =>
        new() { Success = false, Message = message, StatusCode = (int)HttpStatusCode.Conflict };

    public static ApiResponse Unauthorized(string message) =>
        new() { Success = false, Message = message, StatusCode = (int)HttpStatusCode.Unauthorized };

    public static ApiResponse Forbidden(string message) =>
        new() { Success = false, Message = message, StatusCode = (int)HttpStatusCode.Forbidden };

    public static ApiResponse InternalServerError(string message) =>
        new() { Success = false, Message = message, StatusCode = (int)HttpStatusCode.InternalServerError };

    public static ApiResponse ValidatorError(IEnumerable<string> message) =>
        new() { Success = false, Message = string.Join(" \n ", message), StatusCode = (int)HttpStatusCode.Conflict };

    public static ApiResponse Error(string message, HttpStatusCode statusCode) =>
        new() { Success = false, Message = message, StatusCode = (int)statusCode };
}

public sealed class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null) =>
        new() { Success = true, Message = message, Data = data, StatusCode = (int)HttpStatusCode.OK };

    public static new ApiResponse<T> Error(string message, HttpStatusCode statusCode) =>
        new() { Success = false, Message = message, StatusCode = (int)statusCode };
}