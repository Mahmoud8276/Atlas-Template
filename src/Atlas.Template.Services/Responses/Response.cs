using System.Net;
using System.Text.Json.Serialization;

namespace Atlas.Template.Services.Responses
{
    public class BaseResponse
    {
        [JsonPropertyOrder(1)]
        public bool IsSuccess { get; set; }
        
        [JsonPropertyOrder(2)]
        public string Message { get; set; }
        
        [JsonPropertyOrder(4)]
        public string Details { get; set; }
        
        [JsonPropertyOrder(3)]
        public int StatusCode { get; set; }
    }

    public class Response : BaseResponse
    {
        [JsonPropertyOrder(5)]
        public object Data { get; set; }

        public static Response Fail(
            string message = "Fail!",
            int statusCode = (int)HttpStatusCode.BadRequest,
            string details = null)
        {
            return new Response()
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Message = message,
                Details = details
            };
        }

        public static Response Success(
            object data = null,
            string message = "Success!",
            int statusCode = (int)HttpStatusCode.OK)
        {
            return new Response()
            {
                IsSuccess = true,
                StatusCode = statusCode,
                Message = message,
                Data = data
            };
        }
    }

    public class Response<T> : BaseResponse
    {
        [JsonPropertyOrder(5)]
        public T Data { get; set; }

        public static Response<T> Fail(
            string message = "Fail!",
            int statusCode = (int)HttpStatusCode.BadRequest,
            string details = null)
        {
            return new Response<T>()
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Message = message,
                Details = details
            };
        }

        public static Response<T> Success(
            T data,
            string message = "Success!",
            int statusCode = (int)HttpStatusCode.OK)
        {
            return new Response<T>()
            {
                IsSuccess = true,
                StatusCode = statusCode,
                Message = message,
                Data = data
            };
        }
    }
}
