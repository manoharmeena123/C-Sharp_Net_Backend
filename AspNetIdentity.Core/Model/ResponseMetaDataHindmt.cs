using System;

namespace AngularJSAuthentication.Model.Base.ServiceRequestParam
{
    public class ResponseMetaDataHindmt
    {
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public dynamic Data { get; set; }
        public DateTime Timestamp { get; set; }
    }
}