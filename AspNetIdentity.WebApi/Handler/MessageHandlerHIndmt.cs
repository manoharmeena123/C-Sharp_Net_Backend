using AngularJSAuthentication.Common.Helpers;
using AngularJSAuthentication.Model.Base.ServiceRequestParam;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Script.Serialization;

namespace AngularJSAuthentication.API.Handler
{
    public abstract class MessageHandlerHIndmt : DelegatingHandler
    {
        //private static //logger //logger = LogManager.GetCurrentClass//logger();
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string IP = "unknown";
            string browser = "unknown";
            string branchId = string.Empty;
            string branchSysName = string.Empty;
            string IsCorporate = string.Empty;
            var loggedInUser = HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity != null ? HttpContext.Current.User.Identity.Name : "";
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                IEnumerable<string> userNameheader = new List<string>();
                IEnumerable<string> browserValues = new List<string>();
                request.Headers.TryGetValues("username", out userNameheader);
                request.Headers.TryGetValues("Browser", out browserValues);

                // IP = /*headerValues != null && headerValues.Count() > 0 ? headerValues.FirstOrDefault() : */IPHelper.GetVisitorIPAddress();
                browser = browserValues != null && browserValues.Count() > 0 ? browserValues.FirstOrDefault() : string.Empty;

                if ((string.IsNullOrEmpty(loggedInUser) || loggedInUser == "RetailerApp" || loggedInUser == "SalesApp" || loggedInUser == "DeliveryApp")
                     && userNameheader != null && userNameheader.Any())
                {
                    loggedInUser = userNameheader?.FirstOrDefault();
                }
            }
            catch (Exception ex) { }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used

            string otherHeaders = String.Empty;
            foreach (var key in HttpContext.Current.Request.Headers.AllKeys)
                otherHeaders += key + "=" + HttpContext.Current.Request.Headers[key] + Environment.NewLine;

            var corrId = string.Format("{0}{1}", DateTime.Now.Ticks, Thread.CurrentThread.ManagedThreadId);

            HttpContext.Current.Request.Headers.Add("CorelationId", corrId);

            var referrer = request.Headers.Referrer == null ? "unknown" : request.Headers.Referrer.AbsoluteUri;
            var requestInfo = string.Format("{0}", request.RequestUri);

            //if (HttpContext.Current.Request.Url.AbsoluteUri.ToLower().Contains("token") ||
            //    //HttpContext.Current.Request.Url.AbsoluteUri.ToLower().Contains(@"/api/history?") ||
            //    HttpContext.Current.Request.Url.AbsoluteUri.ToLower().Contains(@"/Error")
            //    || request.Headers.Contains("IsError") ||
            //    (request.Content.Headers.ContentType != null && request.Content.Headers.ContentType.MediaType.Contains("multipart/form-data")))
            //{
            //   return await base.SendAsync(request, cancellationToken);
            //}
            //else
            //{
            // //logger.Info("Before ReadAsByteArrayAsync");
            var requestMessage = await request.Content.ReadAsByteArrayAsync();
            // //logger.Info("Before IncommingMessageAsync");
            await IncommingMessageAsync(browser, IP, corrId, requestInfo, requestMessage, otherHeaders, referrer, request.Method.Method, loggedInUser);

            //  //logger.Info("Before SendAsync");
            var response = await base.SendAsync(request, cancellationToken); // change request body

            byte[] responseMessage;

            if (response.IsSuccessStatusCode && response.Content != null)
                responseMessage = await response.Content.ReadAsByteArrayAsync();
            else
                responseMessage = Encoding.UTF8.GetBytes(response.ReasonPhrase);

            ////logger.Info("Before OutgoingMessageAsync");
            await OutgoingMessageAsync(browser, IP, corrId, requestInfo, responseMessage, otherHeaders, referrer, request.Method.Method, loggedInUser);

            if (HttpContext.Current.Request.Url.AbsoluteUri.ToLower().Contains("token")
           || HttpContext.Current.Request.Url.AbsoluteUri.ToLower().Contains(@"/error")
           || !HttpContext.Current.Request.Url.AbsoluteUri.ToLower().Contains(@"/api/")
           || HttpContext.Current.Request.Url.AbsoluteUri.ToLower().Contains(@"/api/appversion")
           || request.Headers.Contains("IsError")
          )
                return response;

            ////logger.Info("return response");

            return GenerateResponse(request, response, corrId);
            // }
        }

        private HttpResponseMessage GenerateResponse(HttpRequestMessage request, HttpResponseMessage response, string corelationId)
        {
            HttpStatusCode statusCode = response.StatusCode;
            string doNotEncryptData = "";

            if (request.Headers.Contains("NoEncryption"))
            {
                doNotEncryptData = HttpContext.Current.Request.Headers.GetValues("NoEncryption").FirstOrDefault();
            }

            MediaTypeFormatter mediaTypeFormatter = new JsonMediaTypeFormatter();
            object responseContent;
            bool isValidContent = response.TryGetContentValue(out responseContent);
            string errorMessage = response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent ? "" : responseContent.ToString();

            dynamic content = responseContent;

            if (isValidContent)
            {
                if (responseContent is HttpError httpError)
                {
                    errorMessage = JsonConvert.SerializeObject(responseContent);
                    responseContent = null;
                }
            }

            AES256Hindmt aes = new AES256Hindmt();
            //string redisAesKey = "Sh0pK!r@n@#@!@#$";
            string redisAesKey = DateTime.Now.ToString("yyyyMMdd") + "1201"; //"12345678"; // "Sh0pK!r@n@#@!@#$";
                                                                             //   responseContent = LanguageConverter(responseContent);
            ResponseMetaDataHindmt responseMetadata = new ResponseMetaDataHindmt();
            responseMetadata.Status = (statusCode == HttpStatusCode.OK) || statusCode == HttpStatusCode.NoContent ? "OK" : "ERROR";
            responseMetadata.Data = statusCode == HttpStatusCode.OK ? string.IsNullOrEmpty(doNotEncryptData) ? aes.Encrypt(JsonConvert.SerializeObject(responseContent), redisAesKey) : responseContent : null;
            //responseMetadata.Data = statusCode == HttpStatusCode.OK ? string.IsNullOrEmpty(doNotEncryptData) ? responseContent : responseContent : null;

            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            responseMetadata.Timestamp = dt;
            responseMetadata.ErrorMessage = errorMessage;
            var result = request.CreateResponse(statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.NoContent ? HttpStatusCode.OK : statusCode, responseMetadata);
            return result;
        }

        protected abstract Task IncommingMessageAsync(string browser, string IP, string correlationId, string requestInfo, byte[] message, string otherHeaders, string referrer, string method, string userName);

        protected abstract Task OutgoingMessageAsync(string browser, string IP, string correlationId, string requestInfo, byte[] message, string otherHeaders, string referrer, string Method, string userName);

        private dynamic LanguageConverter(dynamic responsdata)
        {
            // string input = "Hello Everyone";
            string url = String.Format
         ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
          "en", "th", Uri.EscapeUriString(responsdata));
            HttpClient httpClient = new HttpClient();
            string result = httpClient.GetStringAsync(url).Result;

            // Get all json data
            var jsonData = new JavaScriptSerializer().Deserialize<List<dynamic>>(result);

            // Extract just the first array element (This is the only data we are interested in)
            var translationItems = jsonData[0];

            // Translation Data
            string translation = "";

            // Loop through the collection extracting the translated objects
            foreach (object item in translationItems)
            {
                // Convert the item array to IEnumerable
                IEnumerable translationLineObject = item as IEnumerable;

                // Convert the IEnumerable translationLineObject to a IEnumerator
                IEnumerator translationLineString = translationLineObject.GetEnumerator();

                // Get first object in IEnumerator
                translationLineString.MoveNext();

                // Save its value (translated text)
                translation += string.Format(" {0}", Convert.ToString(translationLineString.Current));
            }

            // Remove first blank character
            if (translation.Length > 1) { translation = translation.Substring(1); };

            // Return translation
            return responsdata;
        }
    }

    //protected override async Task OutgoingMessageAsync(string browser, string IP, string correlationId, string requestInfo, byte[] message, string otherHeaders, string referrer, string Method, string userName)
    //{
    //    var now = DateTime.Now;
    //    var strMessage = Encoding.UTF8.GetString(message);
    //    //BackgroundTaskManager.Run(async () =>
    //    {
    //        //LogHelper.TraceLog(string.Format("{0} Requestor Headers:: {1}  \n - Request: {2}\r\n{3}", correlationId, otherHeaders, requestInfo, ));
    //        //var result = await LogHelper.TraceLog(new TraceLog
    //        //{
    //        //    Browser = browser,
    //        //    CoRelationId = correlationId,
    //        //    Headers = otherHeaders.Replace(Environment.NewLine, "~~"),
    //        //    IP = IP,
    //        //    LogType = "Response",
    //        //    Message = strMessage,
    //        //    RequestInfo = requestInfo,
    //        //    CreatedDate = now,
    //        //    Method = Method,
    //        //    Referrer = referrer,
    //        //    UserName = userName
    //        //});
    //    });

    //    }
    //}

    //public class TraceException//logger : Exception//logger
    //{
    //    public override void Log(Exception//loggerContext context)
    //    {
    //        var now = DateTime.Now;
    //        string IP = IPHelperHindmt.GetVisitorIPAddress();

    //        string browser = "unknown";
    //        string corId = "";
    //        if (HttpContext.Current != null && HttpContext.Current.Request != null)
    //        {
    //            try
    //            {
    //                //IEnumerable<string> headerValues = HttpContext.Current.Request.Headers.GetValues("VisitorIP");
    //                //IEnumerable<string> browserValues = HttpContext.Current.Request.Headers.GetValues("Browser");
    //                corId = HttpContext.Current.Request.Headers.GetValues("CorelationId")?.FirstOrDefault();

    //                //browser = browserValues != null && browserValues.Count() > 0 ? browserValues.FirstOrDefault() : string.Empty;
    //            }
    //            catch { }
    //        }

    //        string error = context.ExceptionContext.Exception.InnerException != null ? context.ExceptionContext.Exception.ToString() + Environment.NewLine + context.ExceptionContext.Exception.InnerException.ToString() : context.ExceptionContext.Exception.ToString();
    //        BackgroundTaskManager.Run(async () =>
    //        {
    //            var result = await LogHelperHindmt.ErrorLog(new ErrorLogHindmt
    //            {
    //                CoRelationId = corId,
    //                IP = IP,
    //                Message = error,
    //                CreatedDate = now
    //            });
    //        });
    //    }
    //}

    public class CustomExceptionHandler : IExceptionHandler
    {
        public virtual Task HandleAsync(ExceptionHandlerContext context,
                                        CancellationToken cancellationToken)
        {
            return HandleAsyncCore(context, cancellationToken);
        }

        public virtual Task HandleAsyncCore(ExceptionHandlerContext context,
                                           CancellationToken cancellationToken)
        {
            HandleCore(context);
            return Task.FromResult(0);
        }

        public virtual void HandleCore(ExceptionHandlerContext context)
        {
            ResponseMetaDataHindmt responseMetadata = new ResponseMetaDataHindmt();
            responseMetadata.Status = "ERROR";
            responseMetadata.Data = null;
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            responseMetadata.Timestamp = dt;
            responseMetadata.ErrorMessage = context.ExceptionContext.Exception.Message;
            context.Result = new TextPlainErrorResult
            {
                Request = context.ExceptionContext.Request,
                Content = responseMetadata
            };

            context.Request.Headers.Add("IsError", "Yes");
            MediaTypeFormatter mediaTypeFormatter = new JsonMediaTypeFormatter();
            context.Request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"); //"application/json"
        }

        public virtual bool ShouldHandle(ExceptionHandlerContext context)
        {
            return true;
        }
    }

    //public class MessageLoggingHandler : MessageHandlerHIndmt
    //{
    //    //private static //logger //logger = LogManager.GetCurrentClass//logger();
    //    protected override async Task IncommingMessageAsync(string browser, string IP, string correlationId, string requestInfo, byte[] message, string otherHeaders, string referrer, string Method, string userName)
    //    {
    //        // //logger.Info("Inside IncommingMessageAsync");
    //        var now = DateTime.Now;
    //        var strMessage = Encoding.UTF8.GetString(message);
    //        BackgroundTaskManager.Run(async () =>
    //        {
    //            //LogHelper.TraceLog(string.Format("{0} Requestor Headers:: {1}  \n - Request: {2}\r\n{3}", correlationId, otherHeaders, requestInfo, ));
    //            //var res = await LogHelper.TraceLog(new TraceLog
    //            //{
    //            //    Browser = browser,
    //            //    CoRelationId = correlationId,
    //            //    Headers = otherHeaders.Replace(Environment.NewLine, "~~"),
    //            //    IP = IP,
    //            //    LogType = "Request",
    //            //    Message = strMessage,
    //            //    RequestInfo = requestInfo,
    //            //    CreatedDate = now,
    //            //    Method = Method,
    //            //    Referrer = referrer,
    //            //    UserName = userName
    //            //});
    //        });

    //        // //logger.Info("Returning from IncommingMessageAsync");
    //    }

    public class TextPlainErrorResult : IHttpActionResult
    {
        public HttpRequestMessage Request { get; set; }
        public ResponseMetaDataHindmt Content { get; set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response =
                             new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(Content));
            response.RequestMessage = Request;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json"); //"application/json"
            return Task.FromResult(response);
        }
    }
}