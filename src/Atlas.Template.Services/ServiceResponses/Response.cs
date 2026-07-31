using System.Net;

namespace Atlas.Template.Services.ServiceResponses
{
    public class BaseResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }

    public class Response : BaseResponse
    {
        public object Data { get; set; }

        public static Response Fail(
            string message = "Fail!", 
            string details = null, 
            HttpStatusCode statusCode = HttpStatusCode.BadRequest)
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
            HttpStatusCode statusCode = HttpStatusCode.OK)
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
        public T Data { get; set; }

        public static Response<T> Fail(
            string message = "Fail!",
            string details = null,
            HttpStatusCode statusCode = HttpStatusCode.BadRequest)
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
            HttpStatusCode statusCode = HttpStatusCode.OK)
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
