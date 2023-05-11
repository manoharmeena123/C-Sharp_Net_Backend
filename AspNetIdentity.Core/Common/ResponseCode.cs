using System.Collections.Generic;
using System.Net;

namespace AspNetIdentity.Core.Common
{
    public class ResponseCode
    {
        public HttpStatusCode Key { get; set; }
        public string Value { get; set; }

    }
    public class ResponseCodeMessage
    {
        public List<ResponseCode> Code { get; set; }
        public ResponseCodeMessage()
        {
            Code = new List<ResponseCode>()
            {
                new ResponseCode(){ Key = HttpStatusCode.OK, Value = "Found"},
                new ResponseCode(){ Key = HttpStatusCode.Created, Value = "Created"},
                new ResponseCode(){ Key = HttpStatusCode.Accepted, Value = "Updated"},
                new ResponseCode(){ Key = HttpStatusCode.MovedPermanently, Value = "Removed"},
                new ResponseCode(){ Key = HttpStatusCode.BadRequest, Value = ""},
                new ResponseCode(){ Key = HttpStatusCode.BadGateway, Value = "Failed"},
                new ResponseCode(){ Key = HttpStatusCode.NoContent, Value = "Empty Data"},
                new ResponseCode(){ Key = HttpStatusCode.NotFound, Value = "Not Found"},
                new ResponseCode(){ Key = HttpStatusCode.NotAcceptable, Value = ""},
            };
        }
    }

    public class UploadResponseCodeMessage
    {
        public List<ResponseCode> Code { get; set; }
        public UploadResponseCodeMessage()
        {
            Code = new List<ResponseCode>()
            {
                new ResponseCode(){ Key = HttpStatusCode.Created, Value = "Uploaded"},
                new ResponseCode(){ Key = HttpStatusCode.NotAcceptable, Value = "Not Acceptable"},
                new ResponseCode(){ Key = HttpStatusCode.BadGateway, Value = "Extensions Not Supported"},
                new ResponseCode(){ Key = HttpStatusCode.NoContent, Value = "No Content Uploaded"},
                new ResponseCode(){ Key = HttpStatusCode.RequestedRangeNotSatisfiable, Value = "Size Exceeded"},
                new ResponseCode(){ Key = HttpStatusCode.BadRequest, Value = ""},
            };
        }
    }
}
