using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.NewDashboard;
using AspNetIdentity.WebApi.Models;
using EASendMail;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using static AspNetIdentity.WebApi.Helper.TimeIntervalHelper;
using static AspNetIdentity.WebApi.Model.EnumClass;
using Comment = AspNetIdentity.WebApi.Model.NewDashboard.Comment;

namespace AspNetIdentity.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/main")]
    public class PostWallController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region This Api Use for Add Post
        /// <summary>
        /// created by Mayank Prajapati on 9/7/2022
        /// Modify by Ravi Vyas on 03/09/2022
        /// Api >> Post >> api/main/wallpost
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("wallpost")]
        [Authorize]
        public async Task<ResponseBodyModel> PostWalle(PostwallRequestModel model)
        {
            ResponseBodyModel res = new ResponseBodyModel();

            var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Model is Unvalid";
                    res.Status = false;
                }
                else
                {
                    Postwall post = new Postwall
                    {
                        Content = model.Content,
                        Image = model.Image,
                        IsType = model.IsType,
                        Type = model.IsType == true ? PostTypeConstants.PostWall : PostTypeConstants.Announcement,
                        Announcement = model.Announcement,
                        AnnouncementTitle = model.AnnouncementTitle,
                        PollOption = model.PollOption,
                        AboutPoll = model.AboutPoll,
                        CreatedBy = tokendata.employeeId,
                        CompanyId = tokendata.companyId,
                        OrgId = tokendata.employeeId,
                        MentionEmployee = JsonConvert.SerializeObject(model.MentionEmployee),

                    };


                    _db.PostWalls.Add(post);
                    await _db.SaveChangesAsync();
                    HostingEnvironment.QueueBackgroundWorkItem(ct => BackGroundTask(post, tokendata));
                    res.Message = "Post Added Successfully  !";
                    res.Status = true;
                    res.Data = post;

                    PostWallHistory obj = new PostWallHistory
                    {
                        PostId = post.PostId,
                        Content = model.Content,
                        Image = model.Image,
                        IsType = model.IsType,
                        Type = model.IsType == true ? PostTypeConstants.PostWall : PostTypeConstants.Announcement,
                        Announcement = model.Announcement,
                        AnnouncementTitle = model.AnnouncementTitle,
                        PollOption = model.PollOption,
                        AboutPoll = model.AboutPoll,
                        CreatedBy = tokendata.employeeId,
                        CompanyId = tokendata.companyId,
                        OrgId = tokendata.employeeId,
                        MentionEmployee = post.MentionEmployee,

                    };
                    _db.PostWallHistorys.Add(obj);
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/main/wallpost", ex.Message, model);
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        public void BackGroundTask(Postwall post, ClaimsHelperModel tokenData)
        {
            Thread.Sleep(1000); // 1000 = 1 sec
            SendMailToMentionEmp(post, tokenData);
        }

        #region This Api Use To Send Post Mention Mail

        public async Task SendMailToMentionEmp(Postwall post, ClaimsHelperModel tokenData)
        {
            try
            {
                var emplist = _db.PostWalls.Where(x => x.PostId == post.PostId && x.IsActive
                           && !x.IsDeleted && x.CompanyId == tokenData.companyId).FirstOrDefault();

                var mentions = JsonConvert.DeserializeObject<List<int>>(emplist.MentionEmployee);

                var test = mentions.ToList();

                foreach (var employee in test)
                {
                    var postdata = _db.PostWalls.FirstOrDefault(x => x.PostId == post.PostId);
                    var comment = _db.PostWalls.Where(x => x.PostId == post.PostId).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                    //var employeeDemo = _db.Employee.Where(x => x.EmployeeId == tokenData.employeeId).FirstOrDefault();
                    var employeeData = _db.Employee.Where(x => x.EmployeeId == employee).FirstOrDefault();
                    //var EmployeeTask = _db.TaskModels.Where(x => x.CreatedBy == tokenData.employeeId && x.IsActive && !x.IsDeleted).Select(x => x.CreatedBy).FirstOrDefault();
                    var createEmployee = _db.Employee.Where(x => x.EmployeeId == post.CreatedBy && x.IsActive && !x.IsDeleted).FirstOrDefault();
                    //var URl = taskdata.TaskURL + post.TaskId;

                    if (employeeData != null)
                    {
                        // Your email address
                        SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                        if (tokenData.IsSmtpProvided)
                        {
                            smtpsettings = _db.CompanySmtpMailModels
                                .Where(x => x.CompanyId == tokenData.companyId)
                                .Select(x => new SmtpSendMailRequest
                                {
                                    From = x.From,
                                    SmtpServer = x.SmtpServer,
                                    MailUser = x.MailUser,
                                    MailPassword = x.Password,
                                    Port = x.Port,
                                    ConectionType = x.ConnectType,
                                })
                                .FirstOrDefault();
                        }
                        string HtmlBody = TaskHelper.MentionInPostwall
                       .Replace("<|MENTIONBY|>", createEmployee.DisplayName)
                       .Replace("<|MENTIONFOR|>", employeeData.DisplayName)
                       .Replace("<|CONTENT|>", postdata.Content)
                       .Replace("<|POSTTITLE|>", postdata.Content);


                        SendMailModelRequest sendMailObject = new SendMailModelRequest()
                        {
                            IsCompanyHaveDefaultMail = tokenData.IsSmtpProvided,
                            Subject = "Post Created ",
                            MailBody = HtmlBody,
                            MailTo = new List<string>() { employeeData.OfficeEmail },
                            SmtpSettings = smtpsettings,
                        };
                        await SmtpMailHelper.SendMailAsync(sendMailObject);
                    }
                }

            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
        }
        #endregion


        public class PostwallRequestModel
        {
            public int PostId { get; set; }
            public string Content { get; set; }
            public string Image { get; set; }
            public bool IsType { get; set; }
            public string Announcement { get; set; }
            public PostTypeConstants Type { get; set; }
            public string AnnouncementTitle { get; set; }
            public string AboutPoll { get; set; }
            public string PollOption { get; set; }
            public List<int> MentionEmployee { get; set; }

        }

        #endregion This Api Use for Add Post

        #region Api for upload for postwall

        /// <summary>
        /// created by Mayank Prajapati on 9/7/2022 
        /// API >> Post >> api/main/uploadimagepost
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadimagepost")]
        public async Task<UploadImageResponse1> UploadImage()
        {
            UploadImageResponse1 result = new UploadImageResponse1();
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
                            {
                                Directory.CreateDirectory(DirectoryURL);
                            }
                            //string path = "UploadImages\\" + compid + "\\" + filename;

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

        #endregion Api for upload for postwall

        #region Api for Get  Wall and Comment and like 
        /// <summary>
        /// created by Mayank Prajapati on 9/7/2022
        /// Modify by Ravi Vyas on 03/09/2022
        /// API>> GET>> api/main/wallget
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("wallget")]
        public async Task<ResponseBodyModel> GetWalle(int count = 0, int? id = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                int c = 2;
                c = count + c;
                var wallData = await (from p in _db.PostWalls
                                      where p.IsDeleted == false && p.IsActive == true &&
                                      p.CompanyId == claims.companyId /*&& p.Type == PostTypeEnum.PostWall*/
                                      select new PostWallRes
                                      {
                                          PostId = p.PostId,
                                          Image = p.Image,
                                          Announcement = p.Announcement,
                                          IsType = p.IsType,
                                          Type = p.IsType.ToString(),
                                          CreatedBy = _db.Employee.Where(b => b.EmployeeId == p.CreatedBy).Select(b => b.DisplayName).FirstOrDefault(),
                                          CreateDate = p.CreatedOn,
                                          DateInterval = "",
                                          ProfilePicture = _db.Employee.Where(a => a.EmployeeId == p.CreatedBy).Select(a => a.ProfileImageUrl).FirstOrDefault(),
                                          Content = p.Content,
                                          LikeCount = _db.Reactions.Count(r => r.PostId == p.PostId && r.IsLike == true && r.IsActive == true && r.IsDeleted == false),
                                          IsLike = _db.Reactions.Where(l => l.PostId == p.PostId && l.EmployeeId == claims.employeeId && l.IsActive == true && l.IsDeleted == false && l.IsLike == true).Select(x => x.IsLike).FirstOrDefault(),
                                          CommentCount = _db.Comments.Count(x => x.PostId == p.PostId && x.CommentType == PostTypeConstants.PostWall),
                                          AnnouncmentComment = _db.Comments.Count(x => x.PostId == p.PostId && x.CommentType == PostTypeConstants.Announcement),

                                      }).OrderByDescending(x => x.PostId).ToListAsync();

                if (wallData.Count > 0)
                {
                    wallData.ForEach(x =>
                    {
                        x.DateInterval = GetInterval(x.CreateDate, DateTime.Now);
                        //x.CommentData.ForEach(z => z.CommentInterval = GetInterval(z.CommentDate));
                    });

                    res.Message = "Post Data Found !";
                    res.Status = true;
                    res.Data = wallData;
                }
                else
                {
                    res.Message = "Failed To Get";
                    res.Status = false;
                    res.Data = wallData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get Walle

        #region Api for Get Announcement
        /// <summary>
        /// Api >> GET >> api/main/announcementget
        /// Created By Ravi Vyas 03-09-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("announcementget")]
        public async Task<ResponseBodyModel> GetAnnouncement()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var wallData = await _db.PostWalls.Where(x => x.IsDeleted == false && x.IsActive == true
                                      && x.CompanyId == claims.companyId && x.Type == PostTypeConstants.Announcement).ToListAsync();

                if (wallData.Count > 0)
                {
                    res.Message = "Post Data Found !";
                    res.Status = true;
                    res.Data = wallData;
                }
                else
                {
                    res.Message = "Failed To Get";
                    res.Status = false;
                    res.Data = wallData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get Walle

        #region Edit Wall Data
        /// <summary>
        /// created by Mayank Prajapati on 9/7/2022
        /// Api >> PUT >> api/main/wallput
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("wallput")]
        public async Task<ResponseBodyModel> EditWalleData(Postwall model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var updateData = await _db.PostWalls.FirstOrDefaultAsync(x => x.PostId == model.PostId && x.IsActive == true
                                    && x.IsDeleted == false && x.CompanyId == claims.companyId);
                if (updateData != null)
                {
                    updateData.Content = model.Content;
                    updateData.Image = model.Image;
                    updateData.UpdatedBy = claims.employeeId;
                    updateData.CompanyId = claims.companyId;
                    updateData.OrgId = claims.employeeId;
                    updateData.UpdatedOn = DateTime.Now;
                    updateData.IsActive = true;
                    updateData.IsDeleted = false;

                    _db.Entry(updateData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Post Update Successfully  !";
                    res.Status = true;
                    res.Data = updateData;

                    //var postHis = await _db.PostWallHistorys.Where(x => x.PostId == model.PostId && x.IsActive &&
                    //     !x.IsDeleted && x.CompanyId == claims.companyid).FirstOrDefaultAsync();
                    PostWallHistory obj = new PostWallHistory
                    {
                        PostId = model.PostId,
                        Content = model.Content,
                        Image = model.Image,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                        IsActive = true,
                        IsDeleted = false,
                        UpdatedOn = DateTime.Now,
                        UpdatedBy = claims.employeeId,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        //_db.Entry(postHis).State = System.Data.Entity.EntityState.Modified;

                    };
                    _db.PostWallHistorys.Add(obj);
                    _db.SaveChanges();
                }
                else
                {
                    res.Message = " Failed To Update";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Edit Walle Data

        #region Api for Delete Post Data
        /// <summary>
        /// created by Mayank Prajapati on 9/7/2022
        /// Api >> Delete >> api/main/deletewall?id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletewall")]
        public async Task<ResponseBodyModel> DeleteWalleData(int id)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var deleteData = await _db.PostWalls.FirstOrDefaultAsync(x =>
                    x.PostId == id && x.IsDeleted == false && x.IsActive == true && x.CompanyId == claims.companyId);
                if (deleteData != null)
                {
                    deleteData.IsDeleted = true;
                    deleteData.IsActive = false;
                    deleteData.DeletedOn = DateTime.Now;
                    deleteData.DeletedBy = claims.employeeId;
                    _db.Entry(deleteData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    res.Status = true;
                    res.Message = "Post Deleted Successfully !";
                }

                PostWallHistory obj = new PostWallHistory
                {
                    PostId = id,
                    Content = deleteData.Content,
                    Image = deleteData.Image,
                    IsType = deleteData.IsType,
                    Type = deleteData.IsType == true ? PostTypeConstants.PostWall : PostTypeConstants.Announcement,
                    Announcement = deleteData.Announcement,
                    AnnouncementTitle = deleteData.AnnouncementTitle,
                    IsActive = false,
                    IsDeleted = true,
                    CompanyId = claims.companyId,
                    OrgId = claims.orgId,
                    CreatedBy = claims.employeeId,
                    CreatedOn = DateTime.Now,
                    DeletedBy = claims.employeeId,
                    DeletedOn = DateTime.Now
                };
                _db.PostWallHistorys.Add(obj);
                _db.SaveChanges();

                //var hisData = await _db.PostWalls.FirstOrDefaultAsync(x =>
                // x.PostId == id && x.IsDeleted == false && x.IsActive == true && x.CompanyId == claims.companyid);
                //if (hisData != null)
                //{
                //    hisData.IsDeleted = true;
                //    hisData.IsActive = false;
                //    hisData.DeletedOn = DateTime.Now;
                //    hisData.DeletedBy = claims.employeeid;
                //    _db.Entry(hisData).State = System.Data.Entity.EntityState.Modified;
                //    await _db.SaveChangesAsync();
                //}


            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Edit Walle Data

        #region Api for Add Comment 
        /// <summary>
        /// Created By Ravi Vyas On 03-09-2022
        /// API >> POST >> api/main/postcomments
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("postcomments")]
        public async Task<ResponseBodyModel> AddComment(CommentRequestModel model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    Comment obj = new Comment
                    {
                        PostId = model.PostId,
                        EmployeeId = model.EmployeeId,
                        Comments = model.Comments,
                        IsComment = model.IsComment,
                        CommentType = model.IsComment == true ? PostTypeConstants.PostWall : PostTypeConstants.Announcement,
                        CreatedOn = DateTime.Now,
                        CreatedBy = tokendata.employeeId,
                        IsActive = true,
                        IsDeleted = false,
                        CompanyId = tokendata.companyId,
                        OrgId = tokendata.orgId,
                        MentionEmployee = JsonConvert.SerializeObject(model.MentionEmployee),
                    };
                    _db.Comments.Add(obj);
                    await _db.SaveChangesAsync();
                    res.Message = "Comment Post Successfully  !";
                    res.Status = true;
                    res.Data = obj;

                    HostingEnvironment.QueueBackgroundWorkItem(ct => PostComment(obj, tokendata));

                    CommentHistory Obj = new CommentHistory
                    {
                        CommentId = obj.CommentId,
                        PostId = model.PostId,
                        EmployeeId = model.EmployeeId,
                        Comments = model.Comments,
                        IsComment = model.IsComment,
                        CommentType = model.IsComment == true ? PostTypeConstants.PostWall : PostTypeConstants.Announcement,
                        CreatedOn = DateTime.Now,
                        CreatedBy = tokendata.employeeId,
                        IsActive = true,
                        IsDeleted = false,
                        CompanyId = tokendata.companyId,
                        OrgId = tokendata.orgId,
                        MentionEmployee = obj.MentionEmployee,
                    };
                    _db.CommentHistorys.Add(Obj);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    res.Message = "Model Is Empty !";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;

            }
            return res;
        }

        public void PostComment(Comment obj, ClaimsHelperModel tokenData)
        {
            Thread.Sleep(1000); // 1000 = 1 sec
            SendMailPostComment(obj, tokenData);
        }

        #region This Api Use To Send Task Mail

        public async Task SendMailPostComment(Comment obj, ClaimsHelperModel tokenData)
        {
            try
            {
                var emplist = _db.Comments.Where(x => x.CommentId == obj.CommentId && x.IsActive
                             && !x.IsDeleted && x.CompanyId == tokenData.companyId).FirstOrDefault();
                var mentions = JsonConvert.DeserializeObject<List<int>>(emplist.MentionEmployee);

                var test = mentions.ToList();

                foreach (var employee in test)
                {
                    var postdata = _db.Comments.FirstOrDefault(x => x.PostId == obj.PostId);
                    var comment = _db.Comments.Where(x => x.PostId == obj.PostId).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                    var employeeData = _db.Employee.Where(x => x.EmployeeId == employee).FirstOrDefault();
                    var createEmployee = _db.Employee.Where(x => x.EmployeeId == obj.CreatedBy && x.IsActive && !x.IsDeleted).FirstOrDefault();

                    if (employeeData != null)
                    {
                        SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                        if (tokenData.IsSmtpProvided)
                        {
                            smtpsettings = _db.CompanySmtpMailModels
                                .Where(x => x.CompanyId == tokenData.companyId)
                                .Select(x => new SmtpSendMailRequest
                                {
                                    From = x.From,
                                    SmtpServer = x.SmtpServer,
                                    MailUser = x.MailUser,
                                    MailPassword = x.Password,
                                    Port = x.Port,
                                    ConectionType = x.ConnectType,
                                })
                                .FirstOrDefault();
                            string HtmlBody = TaskHelper.MentionInPostwall
                           .Replace("<|MENTIONBY|>", createEmployee.DisplayName)
                           .Replace("<|MENTIONFOR|>", employeeData.DisplayName)
                           .Replace("<|CONTENT|>", postdata.Comments)
                           .Replace("<|POSTTITLE|>", postdata.Comments);
                            SendMailModelRequest sendMailObject = new SendMailModelRequest()
                            {
                                IsCompanyHaveDefaultMail = tokenData.IsSmtpProvided,
                                Subject = "Post Created ",
                                MailBody = HtmlBody,
                                MailTo = new List<string>() { employeeData.OfficeEmail },
                                SmtpSettings = smtpsettings,
                            };
                            await SmtpMailHelper.SendMailAsync(sendMailObject);
                        }
                    }
                }

            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
        }
        #endregion

        public class CommentRequestModel
        {

            public int CommentId { get; set; }
            public int EmployeeId { get; set; }
            public int PostId { get; set; }
            public bool IsComment { get; set; }
            public string Comments { get; set; }
            public PostTypeConstants CommentType { get; set; }
            public List<int> MentionEmployee { get; set; }

        }
        #endregion

        #region APi for Get Comment For PostWall
        /// <summary>
        /// Created By Ravi Vyas On 02-09-2022
        /// API >> GET >> api/main/getpostcomment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getpostcomment")]
        public async Task<ResponseBodyModel> GetComment(int postId, int count = 0)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var commentData = await (from c in _db.Comments
                                         where /*c.CommentType == PostTypeEnum.PostWall &&*/ c.IsDeleted == false && c.IsActive == true &&
                                         c.CompanyId == claims.companyId && c.PostId == postId
                                         select new CommentRes
                                         {
                                             CommentId = c.CommentId,
                                             Comments = c.Comments,
                                             PostId = c.PostId,
                                             EmployeeId = c.EmployeeId,
                                             CommentType = c.CommentType,
                                             ProfileUrl = _db.Employee.Where(a => a.EmployeeId == c.EmployeeId).Select(a => a.ProfileImageUrl).FirstOrDefault(),
                                             EmployeeName = _db.Employee.Where(b => b.EmployeeId == c.EmployeeId).Select(b => b.DisplayName).FirstOrDefault(),
                                             CommentDate = c.CreatedOn,
                                             CommentInterval = ""
                                         }).OrderByDescending(x => x.CommentId).ToListAsync();

                if (commentData.Count > 0)
                {
                    commentData.ForEach(z => z.CommentInterval = GetInterval(z.CommentDate, DateTime.Now));
                    var c = 2;
                    c = c + count;
                    res.Message = "Comment Found !";
                    res.Status = true;
                    //res.Data = commentData.Take(c);
                    res.Data = new PaginationData1
                    {
                        TotalData = commentData.Count,
                        List = commentData.Take(c),
                    };
                }
                else
                {
                    res.Message = "Comment Not Found !";
                    res.Status = false;
                    res.Data = commentData;
                }
            }
            catch (Exception ex)
            {

                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion

        #region APi for Get Comment For Announcement
        /// <summary>
        /// Created By Ravi Vyas On 02-09-2022
        /// API >> GET >> api/main/getpostcomment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getannouncementcomment")]
        public async Task<ResponseBodyModel> GetAnnouncementData()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var commentData = await _db.Comments.Where(x => x.IsActive == true && x.IsDeleted == false &&
                                      x.CompanyId == claims.companyId && x.CommentType == PostTypeConstants.Announcement).ToListAsync();
                if (commentData.Count > 0)
                {
                    res.Message = "Comment Found !";
                    res.Status = true;
                    res.Data = commentData;
                }
                else
                {

                    res.Message = "Comment Not Found !";
                    res.Status = false;
                    res.Data = commentData;
                }
            }
            catch (Exception ex)
            {

                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion

        #region This Api Use for Update Post
        /// <summary>
        /// created by Ravi Vyas on 06/09/2022
        /// Api >> Post >> api/main/updatepost
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updatepost")]
        [Authorize]
        public async Task<ResponseBodyModel> UpdateWalle(PostwallRequestModel model)
        {
            ResponseBodyModel res = new ResponseBodyModel();

            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Model is Unvalid";
                    res.Status = false;
                }
                else
                {
                    var updateData = await _db.PostWalls.Where(x => x.PostId == model.PostId && x.IsDeleted == false
                    && x.IsActive == true && x.CompanyId == claims.companyId).FirstOrDefaultAsync();
                    if (updateData != null)
                    {
                        updateData.Content = model.Content;
                        updateData.Image = model.Image;
                        updateData.IsType = model.IsType;
                        updateData.Type = model.IsType == true ? PostTypeConstants.PostWall : PostTypeConstants.Announcement;
                        updateData.Announcement = model.Announcement;
                        updateData.CreatedBy = claims.employeeId;
                        updateData.AnnouncementTitle = model.AnnouncementTitle;
                        updateData.CompanyId = claims.companyId;
                        updateData.OrgId = claims.employeeId;
                        updateData.UpdatedOn = DateTime.Now;
                        updateData.UpdatedBy = claims.employeeId;
                        updateData.IsActive = true;
                        updateData.IsDeleted = false;
                        _db.Entry(updateData).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();
                        res.Message = "Post Update Succesfully !";
                        res.Status = true;
                        res.Data = updateData;


                        //var updateHisData = await _db.PostWallHistorys.Where(x => x.PostId == model.PostId && x.IsDeleted == false
                        //            && x.IsActive == true && x.CompanyId == claims.companyid).FirstOrDefaultAsync();

                        //updateHisData.PostId = updateData.PostId;
                        //updateHisData.Content = model.Content;
                        //updateHisData.Image = model.Image;
                        //updateHisData.IsType = model.IsType;
                        //updateHisData.Type = model.IsType == true ? PostTypeEnum.PostWall : PostTypeEnum.Announcement;
                        //updateHisData.Announcement = model.Announcement;
                        //updateHisData.CreatedBy = claims.employeeid;
                        //updateHisData.CompanyId = claims.companyid;
                        //updateHisData.OrgId = claims.employeeid;
                        //updateHisData.UpdatedOn = DateTime.Now;
                        //updateHisData.UpdatedBy = claims.employeeid;
                        //updateHisData.IsActive = true;
                        //updateHisData.IsDeleted = false;

                        //_db.Entry(updateData).State = System.Data.Entity.EntityState.Modified;
                        //_db.PostWallHistorys.Add(updateHisData);
                        //_db.SaveChanges();

                        PostWallHistory Obj = new PostWallHistory
                        {
                            PostId = model.PostId,
                            Content = model.Content,
                            Image = model.Image,
                            IsType = model.IsType,
                            Type = model.IsType == true ? PostTypeConstants.PostWall : PostTypeConstants.Announcement,
                            Announcement = model.Announcement,
                            AnnouncementTitle = model.AnnouncementTitle,
                            CreatedBy = claims.employeeId,
                            CompanyId = claims.companyId,
                            OrgId = claims.employeeId,
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false,
                            UpdatedBy = claims.employeeId,
                            UpdatedOn = DateTime.Now,

                        };
                        _db.PostWallHistorys.Add(Obj);
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api Use for Add Post

        #region Api for Add Reactions On Post 
        /// <summary>
        /// Created By Ravi Vyas on 06-09-2022
        /// API >> POST>> api/main/addreaction
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>       
        [HttpPost]
        [Route("addreaction")]
        public async Task<ResponseBodyModel> AddReaction(Reaction model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    var reaction = _db.Reactions.Where(x => x.EmployeeId == model.EmployeeId && x.IsActive == true &&
                     x.IsActive == true && x.CompanyId == claims.companyId && x.PostId == model.PostId).FirstOrDefault();
                    if (reaction == null)
                    {
                        Reaction obj = new Reaction
                        {
                            PostId = model.PostId,
                            EmployeeId = model.EmployeeId,
                            IsReaction = model.IsReaction,
                            IsLike = model.IsLike,
                            Type = model.IsReaction == true ? PostTypeConstants.PostWall : PostTypeConstants.Announcement,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedOn = DateTime.Now,
                            CreatedBy = claims.employeeId,
                            CompanyId = claims.companyId,
                            OrgId = claims.orgId,
                        };
                        _db.Reactions.Add(obj);
                        await _db.SaveChangesAsync();
                        res.Message = " Reaction Added Successfully  !";
                        res.Status = true;
                        res.Data = obj;
                    }
                    else
                    {
                        //reaction.ReactionId = model.ReactionId;
                        reaction.PostId = model.PostId;
                        reaction.EmployeeId = model.EmployeeId;
                        reaction.IsReaction = model.IsReaction;
                        reaction.IsReaction = model.IsReaction;
                        reaction.IsLike = model.IsLike;
                        reaction.Type = model.IsReaction == true ? PostTypeConstants.PostWall : PostTypeConstants.Announcement;
                        reaction.IsActive = true;
                        reaction.IsDeleted = false;
                        reaction.UpdatedOn = DateTime.Now;
                        reaction.CompanyId = claims.companyId;
                        reaction.UpdatedBy = claims.employeeId;
                        reaction.OrgId = claims.orgId;
                        _db.Entry(reaction).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();
                        res.Message = " Reaction Update Successfully   !";
                        res.Status = true;
                        res.Data = reaction;
                    }
                }

                else
                {
                    res.Message = "Model IS Empty !";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion

        #region Api for Update Reactions On Post 
        /// <summary>
        /// Created By Ravi Vyas on 06-09-2022
        /// API >> PU>> api/main/updatereaction
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updatereaction")]

        public async Task<ResponseBodyModel> UpdateReaction(Reaction model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    var updateReaction = _db.Reactions.Where(x => x.PostId == model.PostId && x.IsActive == true &&
                     x.IsDeleted == false && x.CompanyId == claims.companyId).FirstOrDefault();

                    if (updateReaction != null)
                    {
                        updateReaction.ReactionId = model.ReactionId;
                        updateReaction.PostId = model.PostId;
                        updateReaction.EmployeeId = model.EmployeeId;
                        updateReaction.IsReaction = model.IsReaction;
                        updateReaction.IsLike = model.IsLike;
                        updateReaction.Type = model.IsReaction == true ? PostTypeConstants.PostWall : PostTypeConstants.Announcement;
                        updateReaction.IsActive = true;
                        updateReaction.IsDeleted = false;
                        updateReaction.UpdatedOn = DateTime.Now;
                        updateReaction.UpdatedBy = claims.employeeId;
                    }

                    _db.Entry(updateReaction).State = System.Data.Entity.EntityState.Modified;
                    _db.Reactions.Add(updateReaction);
                    await _db.SaveChangesAsync();

                    res.Message = " Reaction Update Successfully !";
                    res.Status = true;
                    res.Data = updateReaction;
                }

                else
                {
                    res.Message = "Model IS Empty !";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion

        #region Api's For Get All Employee React on post

        /// <summary>
        /// Created By RAvi VYas on 09-08-2022
        /// API>>GET>>api/main/getreactiondata
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("getreactiondata")]

        public async Task<ResponseBodyModel> GetReactionData(int postId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await (from r in _db.Reactions
                                     where r.IsLike == true && r.IsActive == true && r.IsDeleted == false
                                     && r.CompanyId == claims.companyId && r.PostId == postId && r.Type == PostTypeConstants.PostWall
                                     select new ReactonResponse
                                     {
                                         EmployeeId = r.EmployeeId,
                                         PostId = r.PostId,
                                         ReactionId = r.ReactionId,
                                         IsLike = r.IsLike,
                                         Name = _db.Employee.Where(x => x.EmployeeId == r.EmployeeId).Select(x => x.DisplayName).FirstOrDefault(),
                                         ProfileUrl = _db.Employee.Where(x => x.EmployeeId == r.EmployeeId).Select(x => x.ProfileImageUrl).FirstOrDefault(),

                                     }).ToListAsync();

                if (getData.Count > 0)
                {
                    res.Message = "Successfully  !";
                    res.Status = true;
                    res.Data = new PaginationData1
                    {
                        TotalData = getData.Count,
                        List = getData,
                    };
                }
                else
                {
                    res.Message = "No Reaction Found !";
                    res.Status = false;
                    res.Data = getData;
                }
            }
            catch (Exception ex)
            {

                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion

        #region Api for Get  Wall and Comment and like Infinity Scrolle
        /// <summary>
        /// created by Mayank Prajapati on 9/7/2022
        /// Modify by Ravi Vyas on 03/09/2022
        /// API>> GET>> api/main/wallgetscrolle
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("wallgetscrolle")]
        public async Task<ResponseBodyModel> GetWalleScrole(int? count = null, int? page = null)
        {

            ResponseBodyModel res = new ResponseBodyModel();
            var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employeeInfo = _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokendata.companyId).Select(x => new
                {
                    x.EmployeeId,
                    x.DisplayName
                }).ToList();

                var wallData = await (from p in _db.PostWalls
                                      where !p.IsDeleted && p.IsActive &&
                                      p.CompanyId == tokendata.companyId /*&& p.Type == PostTypeEnum.PostWall*/
                                      select new PostWallRes
                                      {
                                          PostId = p.PostId,
                                          Image = p.Image,
                                          Announcement = p.Announcement,
                                          AnnouncementTitle = p.AnnouncementTitle,
                                          IsType = p.IsType,
                                          Type = p.IsType.ToString(),
                                          EmployeeId = p.CreatedBy,
                                          LogEmpId = tokendata.employeeId,
                                          LogEmpProfileUrl = _db.Employee.Where(a => a.EmployeeId == tokendata.employeeId).Select(a => a.ProfileImageUrl).FirstOrDefault(),
                                          LogEmpName = _db.Employee.Where(b => b.EmployeeId == tokendata.employeeId).Select(b => b.DisplayName).FirstOrDefault(),
                                          CreatedBy = _db.Employee.Where(b => b.EmployeeId == p.CreatedBy).Select(b => b.DisplayName).FirstOrDefault(),
                                          CreateDate = p.CreatedOn,
                                          // DateInterval = "",
                                          ProfilePicture = _db.Employee.Where(a => a.EmployeeId == p.CreatedBy).Select(a => a.ProfileImageUrl).FirstOrDefault(),
                                          Content = p.Content,
                                          LikeCount = _db.Reactions.Count(r => r.PostId == p.PostId && r.IsLike == true),
                                          IsLike = _db.Reactions.Where(l => l.PostId == p.PostId && l.EmployeeId == tokendata.employeeId).Select(x => x.IsLike).FirstOrDefault(),
                                          CommentCount = _db.Comments.Count(x => x.PostId == p.PostId && x.CommentType == PostTypeConstants.PostWall),
                                          CommentInput = " ",
                                          AnnouncmentComment = _db.Comments.Count(x => x.PostId == p.PostId && x.CommentType == PostTypeConstants.Announcement),
                                          MentionEmployee = p.MentionEmployee == null ? "[]" : p.MentionEmployee,
                                          MentionName = "",
                                      }).OrderByDescending(x => x.PostId).ToListAsync();

                var getpostwall = wallData
                                .Select(x => new
                                {
                                    x.PostId,
                                    x.Image,
                                    x.Announcement,
                                    x.AnnouncementTitle,
                                    x.IsType,
                                    x.Type,
                                    x.EmployeeId,
                                    x.LogEmpId,
                                    x.LogEmpProfileUrl,
                                    x.LogEmpName,
                                    x.CreatedBy,
                                    x.CreateDate,
                                    // x.DateInterval,
                                    x.ProfilePicture,
                                    x.Content,
                                    x.LikeCount,
                                    x.IsLike,
                                    x.CommentCount,
                                    x.CommentInput,
                                    x.AnnouncmentComment,
                                    MentionEmpIds = (JsonConvert.DeserializeObject<List<int>>(x.MentionEmployee)),
                                })
                                .Select(x => new
                                {
                                    x.PostId,
                                    x.Image,
                                    x.Announcement,
                                    x.AnnouncementTitle,
                                    x.IsType,
                                    x.Type,
                                    x.EmployeeId,
                                    x.LogEmpId,
                                    x.LogEmpProfileUrl,
                                    x.LogEmpName,
                                    x.CreatedBy,
                                    x.CreateDate,
                                    // x.DateInterval,
                                    x.ProfilePicture,
                                    x.Content,
                                    x.LikeCount,
                                    x.IsLike,
                                    x.CommentCount,
                                    x.CommentInput,
                                    x.AnnouncmentComment,
                                    MentionEmployee = x.MentionEmpIds.Select(z => new
                                    {
                                        Id = z,
                                        Name = employeeInfo.Where(e => e.EmployeeId == z).Select(e => e.DisplayName).FirstOrDefault(),
                                    }),
                                    dateinterval = GetInterval(x.CreateDate, DateTime.Now)
                                    //   x.MentionName,
                                })
                                .ToList();
                var teston = getpostwall;




                if (teston.Count > 0)
                {
                    // wallData.ForEach(x =>
                    getpostwall.ForEach(y =>
                    {
                        //    y.DateInterval = GetInterval(y.CreateDate);
                        //x.DateInterval = GetInterval(x.CreateDate);
                    });
                    res.Message = "Post Data Found !";
                    res.Status = true;
                    if (page.HasValue && count.HasValue)
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = teston.Count,
                            Counts = (int)count,
                            List = teston.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else
                    {
                        res.Data = teston;
                    }
                }

                //if (wallData.Count > 0)
                //{
                //    wallData.ForEach(x =>
                //    {
                //        x.DateInterval = GetInterval(x.CreateDate);
                //        //x.MentionName = _db.Employee.Where(y => y.IsActive && x.MentionEmployee.Contains(y.EmployeeId))
                //    });
                //    res.Message = "Post Data Found !";
                //    res.Status = true;
                //    if (page.HasValue && count.HasValue)
                //    {
                //        res.Data = new PaginationData
                //        {
                //            TotalData = wallData.Count,
                //            Counts = (int)count,
                //            List = wallData.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                //        };
                //    }
                //    else
                //    {
                //        res.Data = wallData;
                //    }
                //}
                else
                {
                    res.Message = "Failed To Get";
                    res.Status = false;
                    res.Data = teston;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get Walle

        #region Api for Serach Employee On Dashabord 
        /// <summary>
        /// Created By Ravi Vyas on 08-09-2022
        /// API>>api/main/serachemployee
        /// </summary>
        /// <param name="serach"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("serachemployee")]

        public async Task<ResponseBodyModel> SerachEmployee(string serach)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var text = serach.ToUpper();
                var data = await _db.Employee
                    .Where(x => !x.IsDeleted && x.IsActive && x.CompanyId == claims.companyId &&
                        x.DisplayName.ToUpper().StartsWith(text) && x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee)
                    .ToListAsync();

                if (data.Count > 0)
                {
                    res.Message = "Employee Data Found !";
                    res.Status = true;
                    res.Data = data;
                }
                else
                {
                    res.Message = "Employee Not Found !";
                    res.Status = true;
                    res.Data = data;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }


        #endregion

        #region Api's For  Delete Comment
        /// <summary>
        /// Created By Ravi Vyas on 08-09-2022
        /// API>>api/main/deletecomment
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletecomment")]
        public async Task<ResponseBodyModel> DeleteComment(int postId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var data = await _db.Comments.Where(x => x.PostId == postId && x.IsDeleted == false && x.IsActive == true &&
                                                x.EmployeeId == claims.employeeId && x.CompanyId == claims.companyId).FirstOrDefaultAsync();
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.IsActive = false;
                    data.DeletedBy = claims.employeeId;
                    data.DeletedOn = DateTime.Now;
                    _db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Message = "Comment Deleted Successfully  :";
                    res.Status = true;
                }
                else
                {
                    res.Message = "Something Went Wrong !:";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {

                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion

        #region Api's for Get Post By Id 
        /// <summary>
        /// Created by Ravi Vyas on 09-09-2022
        /// API>>GET>>api/main/getdatabyid
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="empId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getdatabyid")]
        public async Task<ResponseBodyModel> GetDataById(int postId, int empId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var wallData = await (from p in _db.PostWalls
                                      where !p.IsDeleted && p.IsActive && p.PostId == postId && p.CreatedBy == empId &&
                                      p.CompanyId == claims.companyId /*&& p.Type == PostTypeEnum.PostWall*/
                                      select new PostWallRes
                                      {
                                          PostId = p.PostId,
                                          Image = p.Image,
                                          Announcement = p.Announcement,
                                          IsType = p.IsType,
                                          AnnouncementTitle = p.AnnouncementTitle,
                                          Type = p.IsType.ToString(),
                                          EmployeeId = p.CreatedBy,
                                          CreatedBy = _db.Employee.Where(b => b.EmployeeId == p.CreatedBy).Select(b => b.DisplayName).FirstOrDefault(),
                                          CreateDate = p.CreatedOn,
                                          DateInterval = "",
                                          ProfilePicture = _db.Employee.Where(a => a.EmployeeId == p.CreatedBy).Select(a => a.ProfileImageUrl).FirstOrDefault(),
                                          Content = p.Content,
                                          LikeCount = _db.Reactions.Count(r => r.PostId == p.PostId && r.IsLike == true),
                                          IsLike = _db.Reactions.Where(l => l.PostId == p.PostId && l.EmployeeId == claims.employeeId).Select(x => x.IsLike).FirstOrDefault(),
                                          CommentCount = _db.Comments.Count(x => x.PostId == p.PostId && x.CommentType == PostTypeConstants.PostWall),
                                          MentionEmployee = p.MentionEmployee,
                                      }).OrderByDescending(x => x.PostId).ToListAsync();

                var getwalldata = wallData
                  .Select(x => new
                  {
                      x.PostId,
                      x.Image,
                      x.Announcement,
                      x.IsType,
                      x.AnnouncementTitle,
                      x.Type,
                      x.EmployeeId,
                      x.CreatedBy,
                      x.CreateDate,
                      x.DateInterval,
                      x.ProfilePicture,
                      x.Content,
                      x.LikeCount,
                      x.IsLike,
                      x.CommentCount,
                      MentionEmployee = JsonConvert.DeserializeObject<List<int>>(x.MentionEmployee),
                  }).FirstOrDefault();

                if (getwalldata != null)
                {
                    res.Message = "Data Found Successfully  !";
                    res.Status = true;
                    res.Data = getwalldata;
                }
                else
                {
                    res.Message = "Data Not Found !";
                    res.Status = false;
                    res.Data = getwalldata;
                }
            }

            catch (Exception ex)
            {

                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion

        #region This Api Use To Send Announcement In Mail
        /// <summary>
        /// Create By Ravi Vyas -19-09-2022
        /// </summary>
        /// <param name="announcment"></param>
        /// <param name="title"></param>
        /// <param name="companyId"></param>
        /// <param name="postId"></param>
        /// <returns></returns>
        public bool SendDoc(string announcment, string title, int companyId, int postId)
        {
            try
            {
                var post = _db.PostWalls.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == companyId && x.PostId == postId && x.Type == PostTypeConstants.Announcement).Select(x => new PostWallRes
                {
                    PostId = x.PostId,
                    Announcement = x.Announcement,
                    AnnouncementTitle = x.AnnouncementTitle,
                    CreateDate = x.CreatedOn,
                    CreatedBy = _db.Employee.Where(e => e.EmployeeId == x.CreatedBy).Select(e => e.DisplayName).FirstOrDefault(),
                    Type = x.Type.ToString(),
                }).FirstOrDefault();

                var employeData = _db.Employee.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == companyId).ToList();
                //string pdflocation = ConfigurationManager.AppSettings["PDFLocation"] + CandidateId + ".pdf";
                foreach (var employe in employeData)
                {
                    SmtpMail oMail = new SmtpMail("TryIt");
                    // Your email address
                    oMail.From = "Notification@emossy.com";
                    // Set recipient email address
                    oMail.To = employe.OfficeEmail;
                    // Set email subject
                    oMail.Subject = "New Announcement";
                    string body = "<div style='background: #FAFAFA; color: #333333; padding-left: 30px;font-family: arial,sans-serif; font-size: 14px;'>";
                    body += "<h3 style='background-color: rgb(241, 89, 34);'>Reminder</h3>";
                    body += "Hello " + employe.DisplayName + ',';
                    body += "<p>A new announcement has been published by " + post.CreatedBy + " " + "on" + post.CreateDate.ToString("ddd-MM-yyy") + " . For more queries you can contact us.<p>";
                    body += post.AnnouncementTitle;
                    body += post.Announcement;
                    body += "Thanks,";
                    body += "<br />";
                    //body += "" + candidate.CompanyName + "";
                    body += "</div>";

                    oMail.HtmlBody = body;
                    // Hotmail/Outlook SMTP server address
                    SmtpServer oServer = new SmtpServer("smtp.office365.com");

                    oServer.User = "Notification@emossy.com";
                    // If you got authentication error, try to create an app password instead of your user password.
                    // https://support.microsoft.com/en-us/account-billing/using-app-passwords-with-apps-that-don-t-support-two-step-verification-5896ed9b-4263-e681-128a-a6f2979a7944
                    oServer.Password = "Emossy@2022";
                    // use 587 TLS port
                    oServer.Port = 587;
                    // detect SSL/TLS connection automatically
                    oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                    Console.WriteLine("start to send email over TLS...");
                    SmtpClient oSmtp = new SmtpClient();
                    oSmtp.SendMail(oServer, oMail);
                    Console.WriteLine("email was sent successfully!");
                }
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
            return true;
        }
        #endregion

        #region This Api Use for Update Post
        ///// <summary>
        ///// created by Ravi Vyas on 06/09/2022
        ///// Api >> Post >> api/main/updatepost
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPut]
        //[Route("updatepost")]
        //[Authorize]
        //public async Task<ResponseBodyModel> UpdateAnnouncment(Postwall model)
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();

        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        if (model == null)
        //        {
        //            res.Message = "Model is Unvalid";
        //            res.Status = false;
        //        }
        //        else
        //        {
        //            var updateData = await _db.PostWalls.Where(x => x.PostId == model.PostId && x.IsDeleted == false
        //            && x.IsActive == true && x.CompanyId == claims.companyid).FirstOrDefaultAsync();
        //            if (updateData != null)
        //            {
        //                updateData.Content = model.Content;
        //                updateData.Image = model.Image;
        //                updateData.IsType = model.IsType;
        //                updateData.Type = model.IsType == true ? PostTypeEnum.PostWall : PostTypeEnum.Announcement;
        //                updateData.Announcement = model.Announcement;
        //                updateData.CreatedBy = claims.employeeid;
        //                updateData.CompanyId = claims.companyid;
        //                updateData.OrgId = claims.employeeid;
        //                updateData.UpdatedOn = DateTime.Now;
        //                updateData.UpdatedBy = claims.employeeid;
        //                updateData.IsActive = true;
        //                updateData.IsDeleted = false;
        //                _db.Entry(updateData).State = System.Data.Entity.EntityState.Modified;
        //                await _db.SaveChangesAsync();
        //                res.Message = "Post Update Succesfully !";
        //                res.Status = true;
        //                res.Data = updateData;



        //                PostWallHistory Obj = new PostWallHistory
        //                {
        //                    PostId = model.PostId,
        //                    Content = model.Content,
        //                    Image = model.Image,
        //                    IsType = model.IsType,
        //                    Type = model.IsType == true ? PostTypeEnum.PostWall : PostTypeEnum.Announcement,
        //                    Announcement = model.Announcement,
        //                    CreatedBy = claims.employeeid,
        //                    CompanyId = claims.companyid,
        //                    OrgId = claims.employeeid,
        //                    CreatedOn = DateTime.Now,
        //                    IsActive = true,
        //                    IsDeleted = false,
        //                    UpdatedBy = claims.employeeid,
        //                    UpdatedOn = DateTime.Now,

        //                };
        //                _db.PostWallHistorys.Add(Obj);
        //                _db.SaveChanges();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        #endregion This Api Use for Add Post

        #region This Api for Add Poll
        /// <summary>
        /// Created By Ravi Vyas on 20-09-2022
        /// API>>api/main/addpoll
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addpoll")]
        public async Task<ResponseBodyModel> AddPoll(pollResponse model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<Postwall> list = new List<Postwall>();
            try
            {
                if (model != null)
                {
                    foreach (var item in model.PollOption)
                    {
                        Postwall obj = new Postwall
                        {
                            PostId = model.PostId,
                            Type = PostTypeConstants.Poll,
                            AboutPoll = model.AboutPoll,
                            PollOption = item,
                            IsActive = true,
                            IsDeleted = false,
                            CompanyId = claims.companyId,
                            OrgId = claims.orgId,
                            CreatedBy = claims.employeeId,
                            CreatedOn = DateTime.Now,
                        };

                        _db.PostWalls.Add(obj);
                        await _db.SaveChangesAsync();
                        list.Add(obj);

                        res.Message = "Poll Added Succesfully !";
                        res.Status = true;
                        res.Data = obj;

                        PostWallHistory Obj = new PostWallHistory
                        {
                            PostId = obj.PostId,
                            Type = PostTypeConstants.Poll,
                            AboutPoll = model.AboutPoll,
                            PollOption = item,
                            IsActive = true,
                            IsDeleted = false,
                            CompanyId = claims.companyId,
                            OrgId = claims.orgId,
                            CreatedBy = claims.employeeId,
                            CreatedOn = DateTime.Now,
                        };
                        _db.PostWallHistorys.Add(Obj);
                        _db.SaveChanges();

                    }
                }
                else
                {
                    res.Message = "Model Is Empty !";
                    res.Status = true;
                    res.Data = null;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion

        #region Api for Get Poll By Id 
        /// <summary>
        /// Created By Ravi Vyas on 20-09-2022
        /// API>>GET>>api/Dashboard/getpollbyid?postId
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getpollbyid")]
        public async Task<ResponseBodyModel> GetPollById(int postId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var pollData = await _db.PostWalls.Where(x => x.PostId == postId && x.IsActive && !x.IsDeleted &&
                                               x.CompanyId == claims.companyId && x.Type == PostTypeConstants.Poll).FirstOrDefaultAsync();
                if (pollData != null)
                {
                    res.Message = "Data Found !";
                    res.Status = true;
                    res.Data = pollData;
                }
                else
                {
                    res.Message = "Data Not Found !";
                    res.Status = true;
                    res.Data = pollData;
                }
            }
            catch (Exception ex)
            {

                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion

        #region Helper Model
        public class UploadImageResponse1
        {
            public string Message { get; set; }
            public bool Status { get; set; }
            public string URL { get; set; }
            public string Path { get; set; }
            public string Extension { get; set; }
            public string ExtensionType { get; set; }
        }

        public class PostWallRes
        {
            public int PostId { get; set; }
            public string Content { get; set; }
            public string MentionName { get; set; }
            public string Image { get; set; }
            public bool IsType { get; set; }
            public string Announcement { get; set; }
            public string AnnouncementTitle { get; set; }
            public string Type { get; set; }
            public string CreatedBy { get; set; }
            public DateTime CreateDate { get; set; }
            public string DateInterval { get; set; }
            public string ProfilePicture { get; set; }
            public List<CommentRes> CommentData { get; set; }
            //public List<ReactionRes> ReactionRes { get; set; }
            public bool IsLike { get; set; }
            public int LikeCount { get; set; }
            public int CommentCount { get; set; }
            public int EmployeeId { get; set; }
            public string LogEmpProfileUrl { get; set; }
            public int LogEmpId { get; set; }
            public string LogEmpName { get; set; }
            public string CommentInput { get; set; }
            public int AnnouncmentComment { get; set; }
            public string MentionEmployee { get; set; }
            //public List<int> MentionEmployeeList { get; set; }

        }

        public class CommentRes
        {
            public int CommentId { get; set; }
            public int EmployeeId { get; set; }
            public int PostId { get; set; }
            public bool IsComment { get; set; }
            public string Comments { get; set; }
            public PostTypeConstants CommentType { get; set; }
            public string EmployeeName { get; set; }
            public string ProfileUrl { get; set; }
            public DateTime CommentDate { get; set; }
            public string CommentInterval { get; set; }
        }


        public class PaginationData1
        {
            public int TotalData { get; set; }
            public object List { get; set; }

        }

        public class ReactonResponse
        {
            public int ReactionId { get; set; }
            public int PostId { get; set; }
            public int EmployeeId { get; set; }
            public bool IsLike { get; set; }
            public string Name { get; set; }
            public string ProfileUrl { get; set; }
            public string Department { get; set; }

        }

        public class pollResponse
        {
            public int PostId { get; set; }
            public string AboutPoll { get; set; }
            public List<string> PollOption { get; set; }
        }

        #endregion

        #region API TO GET POST WALL DATA 
        /// <summary>
        /// Created By Harshit Mitra On 23-02-2023
        /// API >> GET >> api/main/getwallpost
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getwallpost")]
        public async Task<IHttpActionResult> GetPostWall(int skip = 0, int take = 10)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var postData = await _db.PostWalls
                    .Where(x => x.CompanyId == tokenData.companyId)
                    .OrderByDescending(x => x.CreatedOn)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();

                res.Message = "Pay Group Created";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = postData;

                return Ok(res);

            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerybreakdown/addpaygroup | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion


        //#region Api for Get  Wall and Comment and like Infinity Scrolle New
        ///// <summary>
        ///// created by Mayank Prajapati on 9/7/2022
        ///// Modify by Ravi Vyas on 03/09/2022
        ///// API>> GET>> api/main/wallgetscrolle
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("wallgetscrolle")]
        //public async Task<ResponseBodyModel> GetWalleScroleNew(int? count = null, int? page = null)
        //{

        //    ResponseBodyModel res = new ResponseBodyModel();

        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {


        //        //var wallData = await (from p in _db.PostWalls
        //        //                      where p.IsDeleted == false && p.IsActive == true &&
        //        //                      p.CompanyId == claims.companyId /*&& p.Type == PostTypeEnum.PostWall*/
        //        //                      select new PostWallRes
        //        //                      {
        //        //                          PostId = p.PostId,
        //        //                          Image = p.Image,
        //        //                          Announcement = p.Announcement,
        //        //                          AnnouncementTitle = p.AnnouncementTitle,
        //        //                          IsType = p.IsType,
        //        //                          Type = p.IsType.ToString(),
        //        //                          EmployeeId = p.CreatedBy,
        //        //                          LogEmpId = claims.employeeId,
        //        //                          LogEmpProfileUrl = _db.Employee.Where(a => a.EmployeeId == claims.employeeId).Select(a => a.ProfileImageUrl).FirstOrDefault(),
        //        //                          LogEmpName = _db.Employee.Where(b => b.EmployeeId == claims.employeeId).Select(b => b.DisplayName).FirstOrDefault(),
        //        //                          CreatedBy = _db.Employee.Where(b => b.EmployeeId == p.CreatedBy).Select(b => b.DisplayName).FirstOrDefault(),
        //        //                          CreateDate = p.CreatedOn,
        //        //                          DateInterval = "",
        //        //                          ProfilePicture = _db.Employee.Where(a => a.EmployeeId == p.CreatedBy).Select(a => a.ProfileImageUrl).FirstOrDefault(),
        //        //                          Content = p.Content,
        //        //                          LikeCount = _db.Reactions.Count(r => r.PostId == p.PostId && r.IsLike == true),
        //        //                          IsLike = _db.Reactions.Where(l => l.PostId == p.PostId && l.EmployeeId == claims.employeeId).Select(x => x.IsLike).FirstOrDefault(),
        //        //                          CommentCount = _db.Comments.Count(x => x.PostId == p.PostId && x.CommentType == PostTypeEnum.PostWall),
        //        //                          CommentInput = " ",
        //        //                          AnnouncmentComment = _db.Comments.Count(x => x.PostId == p.PostId && x.CommentType == PostTypeEnum.Announcement),
        //        //                      }).OrderByDescending(x => x.PostId).ToListAsync();
        //        //if (wallData.Count > 0)
        //        //{
        //        //    wallData.ForEach(x =>
        //        //    {
        //        //        x.DateInterval = GetInterval(x.CreateDate);
        //        //    });

        //        //    res.Message = "Post Data Found !";
        //        //    res.Status = true;
        //        //    if (page.HasValue && count.HasValue)
        //        //    {

        //        //        res.Data = new PaginationData
        //        //        {
        //        //            TotalData = wallData.Count,
        //        //            Counts = (int)count,
        //        //            List = wallData.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
        //        //        };
        //        //    }
        //        //    else
        //        //    {
        //        //        res.Data = wallData;
        //        //    }
        //        //}
        //        //else
        //        //{
        //        //    res.Message = "Failed To Get";
        //        //    res.Status = false;
        //        //    res.Data = wallData;
        //        //}

        //        var datapost = _db.PostWalls.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).Select(x=> new PostWallRes
        //        {
        //            PostId = x.PostId,
        //            Image = x.Image,
        //            Announcement = x.Announcement,
        //            AnnouncementTitle = x.AnnouncementTitle,
        //            IsType = x.IsType,
        //            Type = x.IsType.ToString(),
        //            EmployeeId = x.CreatedBy,
        //            LogEmpId = claims.employeeId

        //        }).ToList();
        //        if (datapost.Count > 0)
        //        {
        //            //datapost.foreach (x =>
        //            //{
        //            //    x.dateinterval = getinterval(x.createdate);
        //            //});

        //            res.message = "post data found !";
        //            res.status = true;
        //            if (page.hasvalue && count.hasvalue)
        //            {

        //                res.data = new paginationdata
        //                {
        //                    totaldata = walldata.count,
        //                    counts = (int)count,
        //                    list = walldata.skip(((int)page - 1) * (int)count).take((int)count).tolist(),
        //                };
        //            }
        //            else
        //            {
        //                res.data = walldata;
        //            }
        //        }
        //        else
        //        {
        //            res.message = "failed to get";
        //            res.status = false;
        //            res.data = walldata;
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        //#endregion Get Walle

    }
}