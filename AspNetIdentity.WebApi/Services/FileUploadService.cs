using AspNetIdentity.Core.Common;
using AspNetIdentity.WebApi.Interface;
using AspNetIdentity.WebApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;

namespace AspNetIdentity.WebApi.Services
{
    public class FileUploadService : IFileUploadService
    {
        public async Task<UploadResponse> UploadFile(ClaimsHelperModel tokenData, HttpRequestMessage request, string urlPath)
        {
            FileUploadResponse result = new FileUploadResponse();
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.Now, tokenData.TimeZone);
            string dates = today.ToString("yyyyMMddhhmmsstt");
            var data = request.Content.IsMimeMultipartContent();
            if (!(request.Content.IsMimeMultipartContent()))
                return new UploadResponse(HttpStatusCode.NotAcceptable);

            var provider = new MultipartMemoryStreamProvider();
            await request.Content.ReadAsMultipartAsync(provider);
            if (provider.Contents.Count == 0)
                return new UploadResponse(HttpStatusCode.NoContent);

            var filefromreq = provider.Contents[0];
            Stream _id = filefromreq.ReadAsStreamAsync().Result;
            //StreamReader reader = new StreamReader(_id);
            string filename = (filefromreq.Headers.ContentDisposition.FileName.Trim('\"')).Replace(" ", "");

            //string extensionType = MimeType.GetContentType(filename).Split('/').First();

            //string extension = System.IO.Path.GetExtension(filename);
            //string fileresult = filename.Substring(0, filename.Length - extension.Length);
            byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
            //string mime = filefromreq.Headers.ContentType.ToString();
            //Stream stream = new MemoryStream(buffer);
            var baseUploadPath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath(
                "~/uploadimage/companyId" + tokenData.companyId + "/" + urlPath + "/"), dates + '.' + filename);
            string DirectoryURL = (baseUploadPath.Split(new string[] { urlPath + "\\" }, StringSplitOptions.None).FirstOrDefault()) + urlPath;

            //for create new Folder
            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
            if (!objDirectory.Exists)
            {
                Directory.CreateDirectory(DirectoryURL);
            }
            string returnPath = "uploadimage\\companyId" + tokenData.companyId + "\\" + urlPath + "\\" + dates + '.' + filename;

            File.WriteAllBytes(baseUploadPath, buffer.ToArray());
            result.Path = returnPath;
            result.Url = baseUploadPath;
            result.Extension = System.IO.Path.GetExtension(filename);
            return new UploadResponse(HttpStatusCode.Created, result);
        }
        public class UploadResponse
        {
            public UploadResponse() { }
            public UploadResponse(HttpStatusCode code, string message = "")
            {
                Status = false;
                StatusCode = code;
                Message = string.IsNullOrEmpty(message) ? UploadSetMessage.SetResponseMessage(code) : message;
            }
            public UploadResponse(HttpStatusCode code, FileUploadResponse model, string message = "")
            {
                Data = model;
                Status = true;
                StatusCode = code;
                Message = string.IsNullOrEmpty(message) ? UploadSetMessage.SetResponseMessage(code) : message;
            }
            public string Message { get; set; } = string.Empty;
            public bool Status { get; set; } = false;
            public HttpStatusCode StatusCode { get; set; }
            public FileUploadResponse Data { get; set; } = new FileUploadResponse("", "", "");
        }
        public class FileUploadResponse
        {
            public FileUploadResponse() { }
            public FileUploadResponse(string url, string path, string extension)
            {
                Url = url;
                Path = path;
                Extension = extension;
            }
            public string Url { get; set; } = string.Empty;
            public string Path { get; set; } = string.Empty;
            public string Extension { get; set; } = string.Empty;
        }
        public static class UploadSetMessage
        {
            public static string SetResponseMessage(HttpStatusCode code)
            {
                UploadResponseCodeMessage obj = new UploadResponseCodeMessage();
                return obj.Code.First(x => x.Key == code).Value ?? string.Empty;
            }
        }
    }
}
