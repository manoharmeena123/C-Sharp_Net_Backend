using AspNetIdentity.Core.Common;
using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Interface;
using AspNetIdentity.WebApi.Interface.ITsfService;
using AspNetIdentity.WebApi.Services.TsfService;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Req = AspNetIdentity.Core.ViewModel.TsfViewModel.BlogRequestClass;

namespace AspNetIdentity.WebApi.Controllers.Tsf
{
    [Authorize]
    [RoutePrefix("api/blogs")]
    public class BlogsController : ApiController
    {
        #region Properties
        private readonly IBlogsService _blogsservice;
        private readonly IShortByEnumService _shortServiceENumService;
        #endregion

        #region Constructor
        public BlogsController()
        {
            _blogsservice = new BlogsService();
            _shortServiceENumService = new ShortByEnumService();
        }
        #endregion

        #region This Api Use To Add Blogs
        /// <summary>
        /// Created By Ankit Jain 06-04-2023
        /// Api >> Post >> api/blogs/addblogs
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("addblogs")]
        public async Task<IHttpActionResult> AddBlogs(Req.CreateBlogRequestclass model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _blogsservice.AddBlogsData(tokenData, model);
            return Ok(response);
        }
        #endregion

        #region This Api Use To Get Blogs By Id
        /// <summary>
        /// Created By Ankit Jain 06-04-2023
        /// Api >> Get >> api/blogs/getblogsbyid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getblogsbyid")]
        public async Task<IHttpActionResult> GetBlogsById(Guid blogsId)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _blogsservice.GetBlogsById(tokenData, blogsId);
            return Ok(response);
        }
        #endregion

        #region This api To Upload Blog Document

        /// <summary>
        ///Created By Ankit On 30-01-2023
        /// </summary>Api>>Post>> api/blogs/uploadblogsdocuments
        /// <returns></returns>
        [HttpPost]
        [Route("uploadblogsdocuments")]
        public async Task<UploadBlogsDoc> UploadBlogsDocments()
        {
            UploadBlogsDoc result = new UploadBlogsDoc();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var dates = DateTime.Now.ToString("yyyyMMddhhmmsstt");
                var data = Request.Content.IsMimeMultipartContent();
                if (Request.Content.IsMimeMultipartContent())
                {
                    //fileList f = new fileList();
                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);
                    if (provider.Contents.Count > 0)
                    {
                        var filefromreq = provider.Contents[0];
                        Stream _id = filefromreq.ReadAsStreamAsync().Result;
                        StreamReader reader = new StreamReader(_id);
                        string filename = filefromreq.Headers.ContentDisposition.FileName.Trim('\"');

                        string extemtionType = MimeType.GetContentType(filename).Split('/').First();

                        string extension = System.IO.Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        //f.byteArray = buffer;
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        var FileUrl = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/blogdocument/" + tokenData.employeeId), dates + '.' + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { tokenData.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + tokenData.employeeId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        string path = "uploadimage\\blogdocument\\" + tokenData.employeeId + "\\" + dates + '.' + Fileresult + extension;

                        File.WriteAllBytes(FileUrl, buffer.ToArray());
                        result.Message = "Successfuly";
                        result.Status = true;
                        result.URL = FileUrl;
                        result.Path = path;
                        result.Extension = extension;
                        result.ExtensionType = extemtionType;
                    }
                    else
                    {
                        result.Message = "You Pass 0 Content";
                        result.Status = false;
                    }
                }
                else
                {
                    result.Message = "Error";
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = false;
            }
            return result;
        }


        /// <summary>
        /// Created By Ankit
        /// </summary>

        public class UploadBlogsDoc
        {
            public string Message { get; set; }
            public bool Status { get; set; }
            public string URL { get; set; }
            public string Path { get; set; }
            public string Extension { get; set; }
            public string ExtensionType { get; set; }
        }
        #endregion 

        #region This Api Use To Get Blogs By Blogtype
        /// <summary>
        /// Created By Ankit Jain 06-04-2023
        /// Api >> Get >> api/blogs/getblogs
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getblogs")]
        public async Task<IHttpActionResult> GetBlogs(Req.GetRequestClass model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _blogsservice.GetBlogsData(tokenData, model);
            return Ok(response);
        }
        #endregion

        #region This Api Use To Update Blogs
        /// <summary>
        /// Created By Ankit Jain 06-04-2023
        /// Api >> Post >> api/blogs/updateblogs
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("updateblogs")]
        public async Task<IHttpActionResult> UpdateBlogs(Req.UpdateBlogRequestclass model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _blogsservice.UpdateBlogsData(tokenData, model);
            return Ok(response);
        }
        #endregion

        #region This Api Use To Remove Blogs

        /// <summary>
        /// Created By Harshit Mitra On 18-04-2023
        /// API >> POST >> api/blogs/deleteblog
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("deleteblog")]
        public async Task<IHttpActionResult> DeleteBlog(Req.DeleteBlogRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _blogsservice.DeleteBlog(tokenData, model.BlogsIds);
            return Ok(response);
        }
        #endregion

        #region This Api Use To Get All Blogs Filter
        /// <summary>
        /// Created By Ankit Jain 06-04-2023
        /// API >> POST >> api/blogs/getblogfilter
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getblogfilter")]
        public async Task<IHttpActionResult> GetBlogFilter(RequestShortBy model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _blogsservice.GetBlogByFilter(tokenData, model.ShortBy);
            return Ok(response);
        }
        #endregion

        #region This Api Use To Get All Blogs Type
        /// <summary>
        /// Created By Ankit Jain 06-04-2023
        /// Api >> Get >> api/blogs/getblogstype
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getblogstype")]
        public IHttpActionResult GetBlogsType()
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = _shortServiceENumService.GetEnumList();
            return Ok(new ServiceResponse<object>(HttpStatusCode.OK, response));
        }
        #endregion

        #region This Api Is Use To Get Blogs In-Review
        /// <summary>
        /// Created By Bhavendra Singh Jat 24-04-2023
        /// Api >> Get >> api/blogs/getpriviewblogforreview
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getpriviewblogforreview")]
        public async Task<IHttpActionResult> GetPriviewBlogForReview(Guid blogsId)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var responce = await _blogsservice.GetPriviewBlogForReview(blogsId);
            return Ok(responce);
        }
        #endregion

        /// <summary>
        /// Created By Harshit Mitra On 11-04-2023
        /// API >> GET >> api/blogs/getblogheaderwithcount
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getblogheaderwithcount")]
        public async Task<IHttpActionResult> GetBlogHeaderWithCountData()
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _blogsservice.GetBlogHeaderWithCount(tokenData);
            return Ok(response);
        }

        /// <summary>
        /// Created By Harshit Mitra On 26-04-2023
        /// API >> GET >> api/blogs/mylatestblogs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mylatestblogs")]
        public async Task<IHttpActionResult> MyLatestBlogs()
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _blogsservice.MyLatestTopBlogs(tokenData);
            return Ok(response);
        }

    }
}
