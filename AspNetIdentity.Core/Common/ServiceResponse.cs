using System.Linq;
using System.Net;

namespace AspNetIdentity.Core.Common
{
    public class ServiceResponse<T>
    {
        public ServiceResponse() { }
        public ServiceResponse(HttpStatusCode code, string message = "")
        {
            Status = false;
            StatusCode = code;
            Message = string.IsNullOrEmpty(message) ? SetMessage.SetResponseMessage(code) : message;
        }
        public ServiceResponse(HttpStatusCode code, T entity, bool status = true, string message = "")
        {
            Data = entity;
            Status = status;
            StatusCode = code;
            Message = string.IsNullOrEmpty(message) ? SetMessage.SetResponseMessage(code) : message;
        }
        public string Message { get; set; } = string.Empty;
        public bool Status { get; set; } = false;
        public HttpStatusCode StatusCode { get; set; }
        public T Data { get; set; }
    }
    public static class SetMessage
    {
        public static string SetResponseMessage(HttpStatusCode code)
        {
            ResponseCodeMessage obj = new ResponseCodeMessage();
            return obj.Code.First(x => x.Key == code).Value ?? string.Empty;
        }
    }
}
