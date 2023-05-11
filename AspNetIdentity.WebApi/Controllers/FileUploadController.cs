//using Microsoft.AspNetCore.Http;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    [RoutePrefix("api/fileupload")]
    public class FileUploadController : ApiController
    {
        //[HttpPost]
        //[Route("Upload")]
        //[Authorize]
        //public async Task<UploadFileResNewDTO2> Upload(string FolderName)
        //{
        //    UploadFileResNewDTO2 result = new UploadFileResNewDTO2();
        //    dataFile fileRes = new dataFile();
        //    try
        //    {
        //        if (Request.Content.IsMimeMultipartContent())
        //        {
        //            var provider = new MultipartMemoryStreamProvider();
        //            await Request.Content.ReadAsMultipartAsync(provider);
        //            var filefromreq = provider.Contents[0];
        //            Stream _id = filefromreq.ReadAsStreamAsync().Result;
        //            StreamReader reader = new StreamReader(_id);
        //            string filename = filefromreq.Headers.ContentDisposition.FileName.Trim('\"');
        //            var name = filename.Split('.');
        //            byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
        //            string mime = filefromreq.Headers.ContentType.ToString();
        //            Stream stream = new MemoryStream(buffer);
        //            var fname = name[0] + DateTime.Now.ToString("yyyyMMddhhmmsstt") + '.' + name[1];
        //            var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/" + FolderName), name[0] + DateTime.Now.ToString("yyyyMMddhhmmsstt") + '.' + name[1]);
        //            string DirectoryURL = (FileUrl.Split(new string[] { FolderName + "\\" }, StringSplitOptions.None).FirstOrDefault()) + FolderName; //for create new Folder
        //            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
        //            if (!objDirectory.Exists)
        //            {
        //                Directory.CreateDirectory(DirectoryURL);
        //            }
        //            // var ServicaeBaseUrl = BaseUrl;
        //            var path = FolderName;
        //            File.WriteAllBytes(FileUrl, buffer.ToArray());
        //            fileRes.filepath = path;
        //            fileRes.fileurl = path + "/" + fname.ToString().Trim();
        //            result.Message = "Successful";
        //            result.Flag = true;
        //            result.data = fileRes;
        //        }
        //        else
        //        {
        //            result.Message = "Error";
        //            result.Flag = false;
        //            result.data = null;
        //        }
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        result.Message = e.Message;
        //        result.Flag = false;
        //        result.data = null;
        //        return result;
        //    }
        //}
        //public class UploadFileResNewDTO2
        //{
        //    public string Message { get; set; }
        //    public bool Flag { get; set; }
        //    public dataFile data { get; set; }
        //    public bool StatusReason { get; set; }
        //}
        //public class dataFile
        //{
        //    public string filepath { get; set; }
        //    public string fileurl { get; set; }
        //}
    }
}