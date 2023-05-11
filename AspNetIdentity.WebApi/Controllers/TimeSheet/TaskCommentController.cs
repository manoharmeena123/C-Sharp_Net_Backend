using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.TimeSheet;
using AspNetIdentity.WebApi.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using static AspNetIdentity.WebApi.Helper.TimeIntervalHelper;

namespace AspNetIdentity.WebApi.Controllers.TimeSheet
{
    /// <summary>
    /// Created by Ravi Vyas on 26-12-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/taskcomment")]
    public class TaskCommentController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Api for Add Comments on Task
        /// <summary>
        /// API>>POST>>api/taskcomment/addcomments
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("addcomments")]
        [HttpPost]
        public async Task<IHttpActionResult> AddTaskComments(AddCommentRequestBody model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.BadRequest;
                    return Ok(res);
                }
                else
                {
                    var checkData = await _db.TaskComments.
                                           Where(x => x.TaskCommentId == model.TaskCommentId && x.IsActive &&
                                           !x.IsDeleted && x.CompanyId == tokenData.companyId)
                                           .FirstOrDefaultAsync();
                    if (checkData == null)
                    {
                        TaskComment obj = new TaskComment
                        {
                            TaskId = model.TaskId,
                            ProjectId = model.ProjectId,
                            CreatedBy = tokenData.employeeId,
                            CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                        };
                        _db.TaskComments.Add(obj);
                        await _db.SaveChangesAsync();
                        checkData = obj;
                    }
                    else
                    {
                        checkData.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                        checkData.UpdatedBy = tokenData.employeeId;
                    }
                    checkData.Comments = model.Comments;
                    checkData.Img1 = model.Img1;
                    checkData.Img2 = model.Img2;
                    checkData.Img3 = model.Img3;
                    checkData.Img4 = model.Img4;
                    checkData.Img5 = model.Img5;
                    checkData.CompanyId = tokenData.companyId;
                    _db.Entry(checkData).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Comment Added Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    res.Data = checkData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/taskcomment/addcomments", ex.Message, model);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }
        }

        public class AddCommentRequestBody
        {
            public Guid TaskCommentId { get; set; }
            public Guid TaskId { get; set; }
            public int ProjectId { get; set; }
            public string Img1 { get; set; }
            public string Img2 { get; set; }
            public string Img3 { get; set; }
            public string Img4 { get; set; }
            public string Img5 { get; set; }
            public string Comments { get; set; }
        }

        #endregion

        #region Api for get Comments 
        /// <summary>
        /// API>>GET>>api/taskcomment/gettaskcomment?TaskId
        /// </summary>
        /// <param name="TaskId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("gettaskcomment")]
        public async Task<IHttpActionResult> GetComments(Guid TaskId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.TaskComments.Where(x => x.IsActive && !x.IsDeleted &&
                               x.TaskId == TaskId)
                               .Select(x => new GetCommentsResponse
                               {
                                   Comment = x.Comments,
                                   Image1 = x.Img1,
                                   Image2 = x.Img2,
                                   Image3 = x.Img3,
                                   Image4 = x.Img4,
                                   Image5 = x.Img5,
                                   CreatedBy = _db.Employee.Where(e => e.EmployeeId == x.CreatedBy).Select(e => e.DisplayName).FirstOrDefault(),
                                   LastUpdatedBy = _db.Employee.Where(e => e.EmployeeId == x.UpdatedBy).Select(e => e.DisplayName).FirstOrDefault(),
                                   CreatedDate = x.CreatedOn,
                                   TimeInterval = "",
                                   Count = 0,
                               }).ToListAsync();

                //var getData = await (from t in _db.TaskComments
                //                     join e in _db.Employee on t.CreatedBy equals e.EmployeeId
                //                     join ue in _db.Employee on t.UpdatedBy equals ue.EmployeeId
                //                     where t.IsActive && !t.IsDeleted && t.TaskId == TaskId
                //                     select new GetCommentsResponse
                //                     {
                //                         Comment = t.Comments,
                //                         Image1 = t.Img1,
                //                         Image2 = t.Img2,
                //                         Image3 = t.Img3,
                //                         Image4 = t.Img4,
                //                         Image5 = t.Img5,
                //                         CreatedBy = e.DisplayName,
                //                         LastUpdatedBy = ue.DisplayName,
                //                         CreatedDate = t.CreatedOn,
                //                         TimeInterval = "",
                //                         Count = 0
                //                     })
                //                     .ToListAsync();

                if (getData.Count > 0)
                {
                    getData.ForEach(a =>
                    {
                        a.TimeInterval = GetInterval(a.CreatedDate.DateTime, DateTime.Now);
                        a.Count = getData.Count;

                    });
                    res.Message = "Data get succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = getData.ToList().OrderByDescending(x => x.TimeInterval).ToList();
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data not found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {

                logger.Error("api/taskcomment/gettaskcomment", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }
        }


        public class GetCommentsResponse
        {
            public string Comment { get; set; }
            public string Image1 { get; set; }
            public string Image2 { get; set; }
            public string Image3 { get; set; }
            public string Image4 { get; set; }
            public string Image5 { get; set; }
            public string CreatedBy { get; set; }
            public string LastUpdatedBy { get; set; }
            public DateTimeOffset UpdatedDate { get; set; }
            public DateTimeOffset CreatedDate { get; set; }
            public string TimeInterval { get; set; }
            public int Count { get; set; }

        }

        #endregion

        #region Api for Add Mention Employee in Comments on Task
        /// <summary>
        /// API>>POST>>api/taskcomment/addmentionemployee
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("addmentionemployee")]
        [HttpPost]
        public async Task<IHttpActionResult> AddMentionEmployee(AddMentionEmployeeRequestBody model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

            try
            {
                if (!ModelState.IsValid)
                {
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.BadRequest;
                    return Ok(res);
                }
                else
                {
                    List<TaskMentionEmployee> listobj = new List<TaskMentionEmployee>();
                    foreach (var item in model.EmployeeId)
                    {
                        TaskMentionEmployee obj = new TaskMentionEmployee
                        {
                            TaskId = model.TaskId,
                            EmployeeId = item,
                            Mentionby = tokenData.employeeId,
                            Comments = model.Comments,
                            CreatedBy = tokenData.employeeId,
                            CompanyId = tokenData.companyId,
                            OrgId = tokenData.orgId,
                            CreatedOn = DateTimeOffset.Now,
                        };
                        _db.TaskMentionEmployees.Add(obj);
                        await _db.SaveChangesAsync();
                        HostingEnvironment.QueueBackgroundWorkItem(ct => BackGroundTask(obj, tokenData));
                    }
                    res.Message = "Mention Added in Comment Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    //res.Data = ;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/taskcomment/addmentionemployee", ex.Message, model);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }
        }
        public void BackGroundTask(TaskMentionEmployee obj, ClaimsHelperModel tokenData)
        {
            Thread.Sleep(1000); // 1000 = 1 sec
            SendMail(obj, tokenData);
        }

        public class AddMentionEmployeeRequestBody
        {
            public Guid TaskId { get; set; } = Guid.Empty;
            public List<int> EmployeeId { get; set; }
            public int Mentionby { get; set; }
            public string Comments { get; set; }
        }
        public class employeelist
        {
            public List<int> EmployeeId { get; set; }
        }

        #endregion

        #region This Api Use To Send Task Mail
        public async Task SendMail(TaskMentionEmployee model, ClaimsHelperModel tokenData)
        {
            try
            {
                var assignEmployeeName = _db.TaskMentionEmployees.Where(x => x.TaskId == model.TaskId && x.IsActive
                            && !x.IsDeleted && x.CompanyId == tokenData.companyId).Select(x => x.EmployeeId).ToList();
                foreach (var employee in assignEmployeeName)
                {
                    var taskdata = _db.TaskModels.FirstOrDefault(x => x.TaskId == model.TaskId);
                    var comment = _db.TaskComments.Where(x => x.TaskId == model.TaskId).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                    var employeeDemo = _db.Employee.Where(x => x.EmployeeId == tokenData.employeeId).FirstOrDefault();
                    var employeeData = _db.Employee.Where(x => x.EmployeeId == employee).FirstOrDefault();
                    var EmployeeTask = _db.TaskModels.Where(x => x.CreatedBy == tokenData.employeeId && x.IsActive && !x.IsDeleted).Select(x => x.CreatedBy).FirstOrDefault();
                    var createEmployee = _db.Employee.Where(x => x.EmployeeId == EmployeeTask && x.IsActive && !x.IsDeleted).FirstOrDefault();
                    var URl = taskdata.TaskURL + model.TaskId;
                    var companylist = _db.Company.Where(y => y.CompanyId == tokenData.companyId
                    && y.IsActive && !y.IsDeleted)
                  .Select(x => new
                  {
                      x.RegisterAddress,
                      x.RegisterCompanyName

                  }).FirstOrDefault();
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
                        }
                        string htmlBody = TaskHelper.MentionInTask
                        .Replace("<|CREATEBY|>", createEmployee.DisplayName)
                        .Replace("<|CREATEFOR|>", employeeData.DisplayName)
                        .Replace("<|TASKLINK|>", URl)
                        .Replace("<|COMMENT|>", comment.Comments)
                        .Replace("<|TASKTITLE|>", taskdata.TaskTitle)
                        .Replace("<|IMAGE_PATH|>", "emossy.png")
                        .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                        .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                        SendMailModelRequest sendMailObject = new SendMailModelRequest()
                        {
                            IsCompanyHaveDefaultMail = tokenData.IsSmtpProvided,
                            Subject = "Task Created ",
                            MailBody = htmlBody,
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

        #region Api for Get Spent Time By Date
        /// <summary>
        /// API>>GET>>api/taskcomment/getdatabydateandid
        /// </summary>
        /// <param name="date"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getdatabydateandid")]
        public async Task<IHttpActionResult> GetDatabyDate(DateTimeOffset date, Guid taskId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.TaskModels.
                                     Where(x => x.TaskId == taskId && x.IsActive && !x.IsDeleted &&
                                     x.CompanyId == tokenData.companyId).
                                     Select(x => new SpentTimeResponseModel
                                     {
                                         SpentTime = _db.TaskLogs.Where(y => y.TaskId == taskId && y.DueDate == date).Select(y => y.SpentTime).FirstOrDefault(),
                                         SpentTime1 = ""
                                     })
                                     .FirstOrDefaultAsync();

                if (getData != null)
                {
                    getData.SpentTime1 = string.Format("{00:00}:{01:00}", (int)getData.SpentTime.TotalMinutes / 60, (int)getData.SpentTime.TotalMinutes % 60);

                    res.Message = "Data get Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = getData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = getData;
                    return Ok(res);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public class SpentTimeResponseModel
        {
            public TimeSpan SpentTime { get; set; }
            public string SpentTime1 { get; set; }
        }
        #endregion

    }
}
