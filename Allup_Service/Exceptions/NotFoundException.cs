
using Allup_Service.Exceptions.IException;
using System.Net;

namespace Allup_Service.Exceptions
{
    public class NotFoundException : Exception, IBaseException
    {
        public NotFoundException(string message = "NotFound"):base(message)
        {
            
        }
        public HttpStatusCode StatusCode { get; set; } =HttpStatusCode.NotFound;
    }
}
