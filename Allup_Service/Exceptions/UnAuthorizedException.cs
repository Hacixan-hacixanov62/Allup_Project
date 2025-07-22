using Allup_Service.Exceptions.IException;
using System.Net;

namespace Allup_Service.Exceptions
{
    public class UnAuthorizedException : Exception, IBaseException
    {
        public UnAuthorizedException(string message = "Qeydiyyatdan keçməyən istifadəçi") : base(message)
        {

        }

        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Unauthorized;
    }
}
