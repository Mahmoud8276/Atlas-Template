using System.Collections.Generic;
using System.Net;

namespace Atlas.Template.Core.Exceptions
{
    public sealed class ValidationException : AppException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(IDictionary<string, string[]> errors)
            : base("One or more validation errors accurred.", HttpStatusCode.BadRequest)
        {
            Errors = errors;
        }

        public ValidationException(string field, string error)
            : base("One or more validation errors accurred.", HttpStatusCode.BadRequest)
        {
            Errors = new Dictionary<string, string[]>
            {
                { field, [error] }
            };
        }

    }

}
