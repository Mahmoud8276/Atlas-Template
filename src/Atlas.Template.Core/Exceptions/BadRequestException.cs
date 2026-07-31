using System.Net;

namespace Atlas.Template.Core.Exceptions
{

    public sealed class BadRequestException : AppException
    {
        public BadRequestException(string message)
            : base(message, HttpStatusCode.BadRequest)
        {
        }
    }
}
