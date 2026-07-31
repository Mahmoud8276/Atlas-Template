using System.Net;

namespace Atlas.Template.Core.Exceptions
{
    public sealed class NotFoundException : AppException
    {
        public NotFoundException(string resourceName, object key) 
            : base($"{resourceName} with iD {key} not found!", HttpStatusCode.NotFound)
        {
        }
    }
}
