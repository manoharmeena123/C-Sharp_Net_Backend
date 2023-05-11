using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.EmossyWallModel;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.EmossyWall
{
    /// <summary>
    /// Created By Harshit Mitra On 24-02-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/wall")]
    public class EmossyWallController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API TO ADD POST ON WALL 
        /// <summary>
        /// Created By Harshit Mitra On 24-02-2023
        /// API >> POST >> api/wall/addpostsonwall
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addpostsonwall")]
        public async Task<IHttpActionResult> AddPostOnWalls(AddPostOnWallRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }
                UserWall obj = new UserWall
                {
                    CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                    CreatedBy = tokenData.employeeId,
                    CompanyId = tokenData.companyId,

                    ImageURl = model.ImageURl,
                    Details = model.Details,
                };
                if (model.PostType == WallPostType.Annoucement)
                {
                    obj.PostType = WallPostType.Annoucement;
                    obj.Title = model.Title;
                    obj.NotifyToAllEmployees = model.NotifyToAllEmployees;
                }
                if (model.EmployeeIds.Count != 0)
                {
                    var addMentions = model.EmployeeIds
                        .Select(x => new PostMention
                        {
                            WallPostId = obj.WallPostId,
                            EmployeeId = x,
                        })
                        .ToList();
                    _db.PostMentions.AddRange(addMentions);
                }
                _db.UserWalls.Add(obj);
                await _db.SaveChangesAsync();

                res.Message = model.PostType.ToString() + " Created";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = obj;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/wall/addpostsonwall | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class AddPostOnWallRequest
        {
            public WallPostType PostType { get; set; } = WallPostType.Post;
            public string Details { get; set; } = String.Empty;
            public string ImageURl { get; set; } = String.Empty;
            public string Title { get; set; } = String.Empty;
            public bool NotifyToAllEmployees { get; set; } = false;
            public List<int> EmployeeIds { get; set; } = new List<int>();
        }
        #endregion

        #region API TO DELETE POST WALL 
        /// <summary>
        /// Created By Harshit Mitra On 24-02-2023
        /// API >> POST >> api/wall/deletewallpost
        /// </summary>
        /// <param name="wallPostId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deletewallpost")]
        public async Task<IHttpActionResult> DeleteWallPost(Guid wallPostId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var wallPost = await _db.UserWalls
                    .FirstOrDefaultAsync(x => x.WallPostId == wallPostId);
                if (wallPost == null)
                {
                    res.Message = "Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    return Ok(res);
                }
                wallPost.IsActive = false;
                wallPost.IsDeleted = true;

                _db.Entry(wallPost).State = System.Data.Entity.EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "Deleted";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/wall/deletewallpost | " +
                    "Wall Post Id : " + wallPostId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO UPLOAD IMAGE IN EMOSSY WALL 
        /// <summary>
        /// API >> POST >> api/wall/uploadimagepost
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadimagepost")]
        public async Task<UploadImageResponse> UploadImage()
        {
            UploadImageResponse result = new UploadImageResponse();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
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
                        if (extemtionType == "image")
                        {
                            string extension = Path.GetExtension(filename);
                            string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                            byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                            //f.byteArray = buffer;
                            string mime = filefromreq.Headers.ContentType.ToString();
                            Stream stream = new MemoryStream(buffer);
                            var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/Postwall/" + claims.employeeId), dates + '.' + filename);
                            string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                            //for create new Folder
                            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                            if (!objDirectory.Exists)
                                Directory.CreateDirectory(DirectoryURL);
                            string path = "uploadimage\\Postwall\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

                            File.WriteAllBytes(FileUrl, buffer.ToArray());
                            result.Message = "Successfully ";
                            result.Status = true;
                            result.URL = FileUrl;
                            result.Path = path;
                            result.Extension = extension;
                            result.ExtensionType = extemtionType;
                        }
                        else
                        {
                            result.Message = "Only Select Image Format";
                            result.Status = false;
                        }
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
        #endregion

        #region API TO GET POST WALL ON EMOSSY WALL
        /// <summary>
        /// Created By Harshit Mitra On 28-02-2023
        /// API >> GET >> api/wall/getpostonwalls
        /// </summary>
        /// <param name="skip"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getpostonwalls")]
        public async Task<IHttpActionResult> GetPostOnWalls(long skip = 0)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var postwallDataList = await GetEmossyWallPostsSP(
                    tokenData.companyId,
                    skip,
                    10,
                    tokenData.employeeId,
                    tokenData.TimeZone);

                res.Message = "Emossy Wall Data";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = new
                {
                    NextSkip = skip + 10,
                    EndOfStack = (postwallDataList.LongCount() < 10),
                    Data = postwallDataList,
                };

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/wall/addpostsonwall | " +
                    "Skip : " + skip + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class PostOnWallResponse
        {
            public Guid WallPostId { get; set; }
            public PostTypeConstants PostType { get; set; }
            public string Details { get; set; }
            public string ImageURl { get; set; }
            public string Title { get; set; }
            public int CreatedBy { get; set; }
            public string CreatedByName { get; set; }
            public string EmployeeImage { get; set; }
            public DateTime CreatedOn { get; set; }
            public string DateInterval { get; set; }
            public bool IsLike { get; set; }
            public long TotalLikes { get; set; }
            public long TotalComments { get; set; }
            public bool AccessToDelete { get; set; }
            public List<MentionClassReponse> Mentions { get; set; }
        }
        public class MentionClassReponse
        {
            public int EmployeeId { get; set; }
            public string DisplayName { get; set; }
        }
        public async Task<List<PostOnWallResponse>> GetEmossyWallPostsSP(int companyId, long skip, long take, int employeeId, string timeZone)
        {
            List<PostOnWallResponse> wallData = new List<PostOnWallResponse>();
            try
            {
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, timeZone);
                var _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                using (var con = new SqlConnection(_connectionString.ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SP_GetEmossyWallPosts", con);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@companyId", SqlDbType.Int).Value = companyId;
                    cmd.Parameters.Add("@skip", SqlDbType.BigInt).Value = skip;
                    cmd.Parameters.Add("@take", SqlDbType.BigInt).Value = take;
                    cmd.Parameters.Add("@currentLogEmp", SqlDbType.Int).Value = employeeId;

                    con.Open();
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    while (rdr.Read())
                    {
                        wallData.Add(new PostOnWallResponse
                        {
                            WallPostId = Guid.Parse(rdr["WallPostId"].ToString()),
                            PostType = (PostTypeConstants)Convert.ToInt32(rdr["PostType"]),
                            Details = rdr["Details"].ToString(),
                            ImageURl = rdr["ImageURl"].ToString(),
                            Title = rdr["Title"].ToString(),
                            CreatedBy = Convert.ToInt32(rdr["CreatedBy"]),
                            CreatedByName = rdr["CreatedByName"].ToString(),
                            EmployeeImage = rdr["EmployeeImage"].ToString(),
                            CreatedOn = Convert.ToDateTime(rdr["CreatedOn"]),
                            DateInterval = TimeIntervalHelper.GetInterval(Convert.ToDateTime(rdr["CreatedOn"]), today.Date),
                            IsLike = Convert.ToBoolean(rdr["LikeStatus"]),
                            TotalLikes = Convert.ToInt64(rdr["TotalLikes"]),
                            TotalComments = Convert.ToInt64(rdr["TotalComments"]),
                            AccessToDelete = false,
                            Mentions = JsonConvert.DeserializeObject<List<MentionClassReponse>>(rdr["Mentions"].ToString()),
                        });
                    }
                    con.Close();
                }
                return wallData;
            }
            catch (Exception)
            {
                return wallData;
            }
        }
        #endregion



        #region Migrate Old Wall Into New 
        /// <summary>
        /// api/wall/migrateoldwalltonew
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("migrateoldwalltonew")]
        public async Task MigrateOldWallToNew()
        {
            var companies = await _db.Company.Where(x => x.IsActive).ToListAsync();
            foreach (var comp in companies)
            {
                var posts = await _db.PostWalls.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == comp.CompanyId).ToListAsync();
                List<UserWall> listWall = new List<UserWall>();
                List<PostLike> listLikes = new List<PostLike>();
                List<PostComment> listComment = new List<PostComment>();
                List<PostMention> listMention = new List<PostMention>();
                foreach (var post in posts)
                {
                    UserWall obj = new UserWall()
                    {
                        PostType = post.Type == PostTypeConstants.PostWall ? WallPostType.Post : WallPostType.Annoucement,
                        Details = post.Content,
                        ImageURl = post.Image ?? String.Empty,
                        Title = post.AnnouncementTitle ?? String.Empty,

                        CreatedBy = post.CreatedBy,
                        CreatedOn = post.CreatedOn,
                        CompanyId = comp.CompanyId,
                    };
                    listWall.Add(obj);
                    if (!String.IsNullOrEmpty(post.MentionEmployee))
                    {
                        var idList = JsonConvert.DeserializeObject<List<int>>(post.MentionEmployee);
                        if (idList.Count > 0)
                        {
                            var addMentions = idList.Select(x => new PostMention
                            {
                                EmployeeId = x,
                                WallPostId = obj.WallPostId,
                            });
                            listMention.AddRange(addMentions);
                        }
                    }
                    var commentData = await _db.UserComments
                        .Where(x => x.PostId == post.PostId)
                        .ToListAsync();
                    var comment = commentData
                        .Select(x => new PostComment
                        {
                            CommentOn = x.CreatedOn,
                            EmployeeId = x.EmpId,
                            Comment = x.Comment,
                            WallPostId = obj.WallPostId,
                        })
                        .ToList();
                    listComment.AddRange(comment);
                    var likesData = await _db.Reactions
                        .Where(x => x.PostId == post.PostId)
                        .ToListAsync();
                    var likes = likesData
                        .Select(x => new PostLike
                        {
                            WallPostId = obj.WallPostId,
                            LikeStatus = x.IsLike,
                            EmployeeId = x.EmployeeId,
                        })
                        .ToList();
                    listLikes.AddRange(likes);
                }
                _db.UserWalls.AddRange(listWall);
                _db.PostLikes.AddRange(listLikes);
                _db.PostMentions.AddRange(listMention);
                _db.PostComments.AddRange(listComment);
                await _db.SaveChangesAsync();
            }
        }
        #endregion
    }
}
