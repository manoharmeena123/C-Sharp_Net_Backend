using AspNetIdentity.WebApi.Infrastructure;

//using Microsoft.AspNetCore.Http;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    [RoutePrefix("api/image")]
    public class ImageUploadController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        //[HttpPost]
        //[Route("UploadFileAll")]
        //[Authorize]
        //public async Task<IHttpActionResult> UploadFileAll()
        //{
        //    Base response = new Base();
        //    try
        //    {
        //        // Access claims if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //        var dates = DateTime.Now.ToString("yyyyMMddhhmmsstt");
        //        var data = Request.Content.IsMimeMultipartContent();
        //        if (Request.Content.IsMimeMultipartContent())
        //        {
        //            //fileList f = new fileList();
        //            var provider = new MultipartMemoryStreamProvider();
        //            await Request.Content.ReadAsMultipartAsync(provider);
        //            var filefromreq = provider.Contents[0];
        //            Stream _id = filefromreq.ReadAsStreamAsync().Result;
        //            StreamReader reader = new StreamReader(_id);
        //            string filename = filefromreq.Headers.ContentDisposition.FileName.Trim('\"'); string extension = Path.GetExtension(filename);
        //            string Fileresult = filename.Substring(0, filename.Length - extension.Length);
        //            byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
        //            //f.byteArray = buffer;
        //            string mime = filefromreq.Headers.ContentType.ToString();
        //            Stream stream = new MemoryStream(buffer);
        //            var FileUrl = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/UploadImages/"), dates + '.' + filename);
        //            string DirectoryURL = (FileUrl.Split(new string[] { "\\" }, StringSplitOptions.None).FirstOrDefault()); //for create new Folder
        //            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
        //            if (!objDirectory.Exists)
        //            {
        //                Directory.CreateDirectory(DirectoryURL);
        //            }
        //            //string path = "UploadImages\\" + compid + "\\" + filename; string path = "UploadImages\\" + userid + "\\" + dates + '.' + Fileresult + extension; File.WriteAllBytes(FileUrl, buffer.ToArray());
        //            response.Message = "Successful";
        //            response.StatusReason = true;
        //            response.Url = DirectoryURL;
        //        }
        //        else
        //        {
        //            response.Message = "Error";
        //            response.StatusReason = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message = "BackHandError : " + ex.Message;
        //        response.StatusReason = false;
        //    }
        //    return Ok(response);
        //}


    }
}