
using System.Net;

namespace Allup_Service.Exceptions.IException
{
    public interface IBaseException
    {
        public HttpStatusCode StatusCode { get; set; }
    }
}
