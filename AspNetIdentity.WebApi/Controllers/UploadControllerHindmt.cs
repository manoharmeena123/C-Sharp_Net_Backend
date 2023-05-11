using System.Web.Http;

namespace AngularJSAuthentication.API.Controllers
{
    [RoutePrefix("api/upload")]
    public class UploadControllerHindmt : ApiController
    {
        //[HttpPost]
        //public string UploadFile()
        //{
        //    string Logourl = "";
        //    if (HttpContext.Current.Request.Files.AllKeys.Any())
        //    {
        //        // Get the uploaded image from the Files collection
        //        var httpPostedFile = HttpContext.Current.Request.Files["file"];

        //        if (httpPostedFile != null)
        //        {
        //            // Validate the uploaded image(optional)
        //            // Get the complete file path
        //            var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/UploadedFiles"), httpPostedFile.FileName);
        //            httpPostedFile.SaveAs(FileUrl);

        //            Account account = new Account(Startup.CloudName, Startup.APIKey, Startup.APISecret);

        //            Cloudinary cloudinary = new Cloudinary(account);

        //            var uploadParams = new ImageUploadParams()
        //            {
        //                File = new FileDescription(FileUrl),
        //                PublicId = "items_images/item_1/" + httpPostedFile.FileName,
        //                Overwrite = true,
        //                Invalidate = true,
        //                Backup = false
        //            };

        //            var uploadResult = cloudinary.Upload(uploadParams);

        //            if (System.IO.File.Exists(FileUrl))
        //            {
        //                System.IO.File.Delete(FileUrl);
        //            }

        //            Logourl = uploadResult.SecureUri.ToString();
        //            return Logourl;
        //        }
        //        return Logourl;
        //    }
        //    return Logourl;
        //}
        //#region Api for Upload all file
        //[HttpPost]
        //[Route("UploadFileAll")]
        //public async Task<UploadFileResDTO> UploadFileAll(string Type)
        //{
        //    UploadFileResDTO result = new UploadFileResDTO();
        //    dataFile fileRes = new dataFile();
        //    try
        //    {
        //        // Access claims
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);

        //        var data = Request.Content.IsMimeMultipartContent();
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
        //            //f.byteArray = buffer;
        //            string mime = filefromreq.Headers.ContentType.ToString();
        //            Stream stream = new MemoryStream(buffer);
        //            // var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/UploadImages/" + Type), filename +DateTime.Now.ToString("yyyyMMddhhmmssffftt"))  ;
        //            var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/UploadImages/" + Type), name[0] + DateTime.Now.ToString("yyyyMMddhhmmsstt") + '.' + name[1]);
        //            string DirectoryURL = (FileUrl.Split(new string[] { Type + "\\" }, StringSplitOptions.None).FirstOrDefault()) + Type;

        //            //for create new Folder
        //            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
        //            if (!objDirectory.Exists)
        //            {
        //                Directory.CreateDirectory(DirectoryURL);

        //            }
        //            // string path = "UploadImages\\" + Type + "\\" + filename;

        //            string path = "UploadImages/" + Type + "/" + name[0] + DateTime.Now.ToString("yyyyMMddhhmmsstt") + '.' + name[1];
        //            var ServicaeBaseUrl = "https://Rkhata.moreyeahs.in/";
        //            //var ServicaeBaseUrl = "https://rkhataadminportal.moreyeahs.in/";
        //            //var ServicaeBaseUrl = "http://localhost:26265/";

        //            path = ServicaeBaseUrl + path;
        //            File.WriteAllBytes(FileUrl, buffer.ToArray());

        //            fileRes.filepath = path;
        //            fileRes.Type = name[1];
        //            result.Message = "Succesful";
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
        //#endregion
    }
}