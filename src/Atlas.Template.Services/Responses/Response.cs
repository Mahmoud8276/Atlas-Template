using System.Net;
using System.Text.Json.Serialization;

namespace Atlas.Template.Services.Responses
{
    public abstract class BaseResponse<TSelf> where TSelf : BaseResponse<TSelf>, new()
    {
        [JsonPropertyOrder(1)] public bool IsSuccess => StatusCode is >= 200 and < 300;
        [JsonPropertyOrder(2)] public string Message { get; init; } = string.Empty;
        [JsonPropertyOrder(3)] public int StatusCode { get; init; }

        [JsonPropertyOrder(4)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Details { get; init; }

        public static TSelf Fail(string message = "Fail!", int statusCode = 400, string? details = null)
            => new() { Message = message, StatusCode = statusCode, Details = details };

        public static TSelf NotFound(string message = "Not found!", string? details = null)
            => Fail(message, (int)HttpStatusCode.NotFound, details);

        public static TSelf Conflict(string message = "Conflict!", string? details = null)
            => Fail(message, (int)HttpStatusCode.Conflict, details);

        public static TSelf Unauthorized(string message = "Unauthorized!", string? details = null)
            => Fail(message, (int)HttpStatusCode.Unauthorized, details);

        public static TSelf Forbidden(string message = "Forbidden!", string? details = null)
            => Fail(message, (int)HttpStatusCode.Forbidden, details);

        public static TSelf InternalServerError(string message = "Internal Server Error!", string? details = null)
            => Fail(message, (int)HttpStatusCode.InternalServerError, details);
    }

    public class Response : BaseResponse<Response>
    {
        [JsonPropertyOrder(5)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Data { get; init; }

        public static Response Success(object? data = null, string message = "Success!", int statusCode = 200)
            => new() { Data = data, Message = message, StatusCode = statusCode };

        public static Response Created(object? data = null, string message = "Created!")
            => Success(data, message, 201);
    }

    public class Response<T> : BaseResponse<Response<T>>
    {
        [JsonPropertyOrder(5)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; init; }

        public static Response<T> Success(T data, string message = "Success!", int statusCode = 200)
            => new() { Data = data, Message = message, StatusCode = statusCode };

        public static Response<T> Created(T data, string message = "Created!")
            => Success(data, message, 201);
    }
}
