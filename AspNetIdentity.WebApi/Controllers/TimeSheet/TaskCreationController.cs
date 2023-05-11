using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.TimeSheet;
using AspNetIdentity.WebApi.Model.TimeSheet.History;
using AspNetIdentity.WebApi.Models;
using Dommel;
using EASendMail;
using LinqKit;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using static AspNetIdentity.WebApi.Controllers.Asset.AssetsController;
using static AspNetIdentity.WebApi.Controllers.Employees.EmployeeExitsController;


namespace AspNetIdentity.WebApi.Controllers.TimeSheet
{
    /// <summary>
    /// Created By Ravi Vyas On 28-11-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/taskcreation")]
    public class TaskCreationController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API TO CREATE TASK 
        /// <summary>
        /// Created By Ravi Vyas On 28-11-2022
        /// API >> POST >> api/taskcreation/createtask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createtask")]
        public async Task<IHttpActionResult> CreateLeaveType(CreateTaskRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var taskIdName = _db.ProjectLists.Where(x => x.IsActive && !x.IsDeleted && x.ID == model.ProjectId).Select(x => x.ProjectName).FirstOrDefault();
                var taskIdNumberCount = _db.TaskModels.Where(y => y.IsActive && !y.IsDeleted && y.ProjectId == model.ProjectId).ToList().Count();                              /*(model.SubProjectId == 0) ? y.ProjectId == model.ProjectId : y.ProjectId == model.SubProjectId*/
                string firstAndLast = "";
                string[] array = taskIdName.ToString().Trim().Split();
                if (array.Length == 1)
                {
                    firstAndLast = array.First().Substring(0, 1);
                }
                else
                {
                    firstAndLast = array.First().Substring(0, 1) + array.Last().Substring(0, 1);
                }
                var check = model.EstimateTime.ToString().Split('.');
                var timeCheck = model.EstimateTime.ToString().Contains(".") ? model.EstimateTime.ToString()
                               .Split('.').Select(long.Parse).ToList() : model.EstimateTime.ToString().Split(':')?.Select(long.Parse).ToList();
                long totalEstimageMinutes = (timeCheck[0] * 60) + timeCheck[1];

                TaskModel obj = new TaskModel();
                if (model.TaskType != TaskTypeConstants.BackLog)
                {
                    obj.Priority = model.Priority;
                    obj.AssignEmployeeId = model.AssignEmployeeId;
                    obj.StartDate = TimeZoneConvert.ConvertTimeToSelectedZone(model.StartDate.Value, tokenData.TimeZone);
                    obj.EndDate = TimeZoneConvert.ConvertTimeToSelectedZone(model.EndDate.Value, tokenData.TimeZone);
                    obj.TaskBilling = model.TaskBilling;
                }
                obj.TaskURL = model.TaskURL;
                obj.ProjectId = model.ProjectId;
                obj.TaskTitle = model.TaskTitle;
                obj.Discription = model.Discription;
                obj.TaskType = model.TaskType;
                obj.TaskIdNumber = taskIdNumberCount + 1;
                obj.ProjectTaskId = firstAndLast + "" + obj.TaskIdNumber;
                obj.CompanyId = tokenData.companyId;
                obj.CreatedBy = tokenData.employeeId;
                obj.Image1 = model.Image1;
                obj.Image2 = model.Image2;
                obj.Image3 = model.Image3;
                obj.Image4 = model.Image4;
                obj.TaskURL = model.TaskURL;
                obj.Attechment = model.Attechment;
                obj.OrgId = tokenData.orgId;
                obj.EstimateTime = totalEstimageMinutes;
                obj.CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                obj.SprintId = model.SprintId;

                _db.TaskModels.Add(obj);
                await _db.SaveChangesAsync();
                if (model.IsMail)
                    HostingEnvironment.QueueBackgroundWorkItem(ct => AddTaskBackgroundCheck(obj, tokenData.IsSmtpProvided, tokenData.companyId));

                res.Message = "Created Successfully Id " + "" + obj.TaskIdNumber + "!" + "";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                res.Data = obj;

                return Ok(res);

            }
            catch (Exception ex)
            {
                logger.Error("api/taskcreation/createtask", ex.Message, model);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }
        }
        public async Task AddTaskBackgroundCheck(TaskModel model, bool isSmtpProvided, int companyId)
        {
            await Task.Delay(1000);
            {
                if (model.TaskType != TaskTypeConstants.BackLog)
                    await SendAssignTaskMailByTask(model, isSmtpProvided, companyId);
                else
                    await SendAssignTaskMailBackLog(model, isSmtpProvided, companyId);
            }
        }
        public class CreateTaskRequest
        {
            public Guid TaskId { get; set; }
            public int ProjectId { get; set; }
            public Guid SprintId { get; set; } = Guid.Empty;
            public string TaskTitle { get; set; }
            public string Discription { get; set; }
            public TaskPriorityConstants Priority { get; set; } = TaskPriorityConstants.Medium;
            public TaskTypeConstants TaskType { get; set; } = TaskTypeConstants.BackLog;
            //public Guid TaskTypeId { get; set; } = Guid.Empty;
            public int? AssignEmployeeId { get; set; } = 0;
            public string Attechment { get; set; }
            //public TimeSpan EstimateTime { get; set; }
            public string EstimateTime { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public int TaskIdNumber { get; set; } = 0;
            // public int SubProjectId { get; set; } = 0;
            public string Image1 { get; set; }
            public string Image2 { get; set; }
            public string Image3 { get; set; }
            public string Image4 { get; set; }
            public string TaskURL { get; set; }
            public bool IsMail { get; set; } = true;
            public TaskBillingConstants TaskBilling { get; set; } = TaskBillingConstants.Non_Billable;
        }


        #endregion

        #region API FOR GET ALL TASK
        /// <summary>
        /// Created By Ravi Vyas On 28-11-2022
        /// API>>GET>> api/taskcreation/getalltask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getalltask")]
        public async Task<IHttpActionResult> GetAllTask(int ProjectId, int? page = null, int? count = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            GetTaskDTO obj = new GetTaskDTO();
            try
            {
                if(tokenData.IsAdminInCompany)
                {
                    //var checkPermission = await _db.TaskPermissions.
                    //                      FirstOrDefaultAsync(x => x.IsActive && !x.IsDeleted &&
                    //                      x.AssigneEmployeeId == tokenData.employeeId && x.ProjectId == ProjectId);

                    var allTask = await _db.TaskModels.
                                         Where(x => x.IsActive && !x.IsDeleted && x.TaskType != TaskTypeConstants.BackLog && x.ProjectId == ProjectId && x.CompanyId == tokenData.companyId).
                                         //Where(x => !x.IsDeleted &&
                                         //checkPermission.ViewAlProjectTask
                                         //?
                                         //x.CompanyId == tokenData.companyId
                                         //:
                                         //x.AssignEmployeeId == tokenData.employeeId && x.CompanyId == tokenData.companyId).
                                         Select(x => new
                                         {
                                             TaskIdNumber = x.TaskIdNumber,
                                             TaskTitle = x.TaskTitle,
                                             TaskType = x.TaskType.ToString(),
                                             TaskId = x.TaskId,
                                             Discription = x.Discription,
                                             AssignEmployeeName = _db.Employee.Where(a => a.EmployeeId == x.AssignEmployeeId && x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId).Select(a => a.DisplayName).FirstOrDefault(),
                                             Priority = x.Priority.ToString(),
                                             Status = x.Status.ToString(),
                                             AssignEmployeeId = x.AssignEmployeeId.Value,
                                             Attechment = x.Attechment,
                                             EstimateTime = x.EstimateTime,
                                             EstTime = "",
                                             StartDate = x.StartDate.Value,
                                             EndDate = x.EndDate.Value,
                                             CreatedByName = _db.Employee.Where(a => a.EmployeeId == x.CreatedBy && x.IsActive && !x.IsDeleted).Select(a => a.DisplayName).FirstOrDefault(),
                                             TaskPercentage = x.Percentage,
                                             SpentTime = _db.TaskLogs.Where(t => t.TaskId == x.TaskId && t.IsActive && !x.IsDeleted).Select(t => t.SpentTime).ToList(),
                                             IsApproved = _db.TaskApprovels.Where(a => a.TaskId == x.TaskId).Select(a => a.IsApproved).FirstOrDefault(),
                                             ProjectTaskId = x.ProjectTaskId,
                                             BilingStatus = x.TaskBilling.ToString(),
                                             SprintId = x.SprintId,
                                             SprintName = _db.Sprints.Where(s => s.SprintId == x.SprintId).Select(s => s.SprintName).FirstOrDefault(),
                                         })
                                        .OrderByDescending(x => x.StartDate)
                                        .ToListAsync();
                    var checkTask = allTask
                            .Select(x => new TaskResponseModel
                            {
                                TaskIdNumber = x.TaskIdNumber,
                                TaskTitle = x.TaskTitle,
                                TaskType = x.TaskType,
                                TaskId = x.TaskId,
                                Discription = x.Discription,
                                AssignEmployeeName = x.AssignEmployeeName,
                                Priority = x.Priority,
                                Status = x.Status,
                                AssignEmployeeId = x.AssignEmployeeId,
                                Attechment = x.Attechment,
                                EstimateTime = x.EstimateTime,
                                EstTime = "",
                                StartDate = x.StartDate,
                                EndDate = x.EndDate,
                                CreatedByName = x.CreatedByName,
                                TaskPercentage = x.TaskPercentage,
                                SprintId = x.SprintId,
                                SprintName = x.SprintName,
                                SpentTime = new TimeSpan(00, (int)(x.SpentTime.Select(z => z.TotalMinutes).ToList().Sum()), 00),
                                SpentTime1 = string.Format("{00:00}:{01:00}", (int)(x.SpentTime.Select(z => z.TotalMinutes).ToList().Sum() / 60),
                                                                               (int)(x.SpentTime.Select(z => z.TotalMinutes).ToList().Sum() % 60)),
                                IsApproved = x.IsApproved,
                                ProjectTaskId = x.ProjectTaskId,
                                Billing = x.BilingStatus,
                            }).
                             Skip(((int)page - 1) * (int)count).Take((int)count)
                            .ToList();
                    obj.TaskResponseModel = checkTask;

                    var totalEstimatedTime = checkTask.Select(x => x.EstimateTime/*.TotalMinutes*/).Sum();
                    obj.TotalEstimatedTime = string.Format("{00:00}:{01:00}", (int)totalEstimatedTime / 60, totalEstimatedTime % 60);
                    double totalSpentTime = checkTask.Select(t => t.SpentTime.TotalMinutes).Sum();
                    obj.TotalSpentTime = string.Format("{00:00}:{01:00}", (int)totalSpentTime / 60, totalSpentTime % 60);

                    if (checkTask.Count > 0)
                    {
                        checkTask.ForEach(a =>
                        {
                            a.EstTime = string.Format("{00:00}:{01:00}", (int)a.EstimateTime / 60, a.EstimateTime % 60);
                        });
                        res.Message = "All Task Successfully Get !";
                        res.Status = true;

                        if (page.HasValue && count.HasValue)
                        {
                            res.Data = new
                            {
                                TotalData = allTask.Count,
                                Counts = (int)count,
                                List = checkTask, /*checkTask.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),*/
                                TotalEstimateTime = obj.TotalEstimatedTime,
                                TotalSpentTime = obj.TotalSpentTime
                            };
                            return Ok(res);
                        }
                        else
                        {
                            res.Data = obj;
                            return Ok(res);
                        }
                    }
                    else
                    {
                        res.Message = "No Task Found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = checkTask;
                        return Ok(res);
                    }
                }
                else
                {
                    var checkPermission = await _db.TaskPermissions.
                                         FirstOrDefaultAsync(x => x.IsActive && !x.IsDeleted &&
                                         x.AssigneEmployeeId == tokenData.employeeId && x.ProjectId == ProjectId);

                    var allTask = await _db.TaskModels.
                                         Where(x => x.IsActive && !x.IsDeleted && x.TaskType != TaskTypeConstants.BackLog && x.ProjectId == ProjectId && x.CompanyId == tokenData.companyId).
                                         Where(x => !x.IsDeleted &&
                                         checkPermission.ViewAlProjectTask
                                         ?
                                         x.CompanyId == tokenData.companyId
                                         :
                                         x.AssignEmployeeId == tokenData.employeeId && x.CompanyId == tokenData.companyId).
                                         Select(x => new
                                         {
                                             TaskIdNumber = x.TaskIdNumber,
                                             TaskTitle = x.TaskTitle,
                                             TaskType = x.TaskType.ToString(),
                                             TaskId = x.TaskId,
                                             Discription = x.Discription,
                                             AssignEmployeeName = _db.Employee.Where(a => a.EmployeeId == x.AssignEmployeeId && x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId).Select(a => a.DisplayName).FirstOrDefault(),
                                             Priority = x.Priority.ToString(),
                                             Status = x.Status.ToString(),
                                             AssignEmployeeId = x.AssignEmployeeId.Value,
                                             Attechment = x.Attechment,
                                             EstimateTime = x.EstimateTime,
                                             EstTime = "",
                                             StartDate = x.StartDate.Value,
                                             EndDate = x.EndDate.Value,
                                             CreatedByName = _db.Employee.Where(a => a.EmployeeId == x.CreatedBy && x.IsActive && !x.IsDeleted).Select(a => a.DisplayName).FirstOrDefault(),
                                             TaskPercentage = x.Percentage,
                                             SpentTime = _db.TaskLogs.Where(t => t.TaskId == x.TaskId && t.IsActive && !x.IsDeleted).Select(t => t.SpentTime).ToList(),
                                             IsApproved = _db.TaskApprovels.Where(a => a.TaskId == x.TaskId).Select(a => a.IsApproved).FirstOrDefault(),
                                             ProjectTaskId = x.ProjectTaskId,
                                             BilingStatus = x.TaskBilling.ToString(),
                                             SprintId = x.SprintId,
                                             SprintName = _db.Sprints.Where(s => s.SprintId == x.SprintId).Select(s => s.SprintName).FirstOrDefault(),
                                         })
                                        .OrderByDescending(x => x.StartDate)
                                        .ToListAsync();
                    var checkTask = allTask
                            .Select(x => new TaskResponseModel
                            {
                                TaskIdNumber = x.TaskIdNumber,
                                TaskTitle = x.TaskTitle,
                                TaskType = x.TaskType,
                                TaskId = x.TaskId,
                                Discription = x.Discription,
                                AssignEmployeeName = x.AssignEmployeeName,
                                Priority = x.Priority,
                                Status = x.Status,
                                AssignEmployeeId = x.AssignEmployeeId,
                                Attechment = x.Attechment,
                                EstimateTime = x.EstimateTime,
                                EstTime = "",
                                StartDate = x.StartDate,
                                EndDate = x.EndDate,
                                CreatedByName = x.CreatedByName,
                                TaskPercentage = x.TaskPercentage,
                                SprintId = x.SprintId,
                                SprintName = x.SprintName,
                                SpentTime = new TimeSpan(00, (int)(x.SpentTime.Select(z => z.TotalMinutes).ToList().Sum()), 00),
                                SpentTime1 = string.Format("{00:00}:{01:00}", (int)(x.SpentTime.Select(z => z.TotalMinutes).ToList().Sum() / 60),
                                                                               (int)(x.SpentTime.Select(z => z.TotalMinutes).ToList().Sum() % 60)),
                                IsApproved = x.IsApproved,
                                ProjectTaskId = x.ProjectTaskId,
                                Billing = x.BilingStatus,
                            }).
                             Skip(((int)page - 1) * (int)count).Take((int)count)
                            .ToList();
                    obj.TaskResponseModel = checkTask;

                    var totalEstimatedTime = checkTask.Select(x => x.EstimateTime/*.TotalMinutes*/).Sum();
                    obj.TotalEstimatedTime = string.Format("{00:00}:{01:00}", (int)totalEstimatedTime / 60, totalEstimatedTime % 60);
                    double totalSpentTime = checkTask.Select(t => t.SpentTime.TotalMinutes).Sum();
                    obj.TotalSpentTime = string.Format("{00:00}:{01:00}", (int)totalSpentTime / 60, totalSpentTime % 60);

                    if (checkTask.Count > 0)
                    {
                        checkTask.ForEach(a =>
                        {
                            a.EstTime = string.Format("{00:00}:{01:00}", (int)a.EstimateTime / 60, a.EstimateTime % 60);
                        });
                        res.Message = "All Task Successfully Get !";
                        res.Status = true;

                        if (page.HasValue && count.HasValue)
                        {
                            res.Data = new
                            {
                                TotalData = allTask.Count,
                                Counts = (int)count,
                                List = checkTask, /*checkTask.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),*/
                                TotalEstimateTime = obj.TotalEstimatedTime,
                                TotalSpentTime = obj.TotalSpentTime
                            };
                            return Ok(res);
                        }
                        else
                        {
                            res.Data = obj;
                            return Ok(res);
                        }
                    }
                    else
                    {
                        res.Message = "No Task Found !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = checkTask;
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {

                logger.Error("api/taskcreation/getalltask", ex.Message);
                return BadRequest("Failed");
            }

        }

        public class TaskResponseModel
        {
            public Guid TaskId { get; set; }
            public string TaskTitle { get; set; }
            public string Discription { get; set; }
            public string Priority { get; set; }
            public string TaskType { get; set; }
            public string Status { get; set; }
            public string AssignEmployeeName { get; set; }
            public int AssignEmployeeId { get; set; }
            public string Attechment { get; set; }
            //public TimeSpan EstimateTime { get; set; }
            public double EstimateTime { get; set; }
            public string EstTime { get; set; }
            public DateTimeOffset StartDate { get; set; }
            public DateTimeOffset EndDate { get; set; }
            public int TaskIdNumber { get; set; }
            public string CreatedByName { get; set; }
            public int TaskPercentage { get; set; }
            public TimeSpan SpentTime { get; set; }
            public string SpentTime1 { get; set; }
            //public double SpentTime { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public bool IsApproved { get; set; }
            public string Image1 { get; set; }
            public string Image2 { get; set; }
            public string Image3 { get; set; }
            public string Image4 { get; set; }
            public string Comments { get; set; }
            public string ProjectTaskId { get; set; }
            public string Billing { get; set; }
            public Guid SprintId { get; set; }
            public string SprintName { get; set; }
        }

        public class GetTaskDTO
        {
            public List<TaskResponseModel> TaskResponseModel { get; set; }
            public string TotalEstimatedTime { get; set; }
            public string TotalSpentTime { get; set; }
            //public double TotalEstimatedTime { get; set; }
            //public double TotalSpentTime { get; set; }
        }

        #endregion

        #region Api To Upload Attached Task File 
        /// <summary>
        /// Created By Ravi Vyas On 14-12-2022
        /// API>>Post>>api/taskcreation/uploadtaskattechment
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadtaskattechment")]
        public async Task<UploadImageResponse> UploadTaskAttechment()
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
                        //if (extemtionType == "image" || extemtionType=="Document"||extemtionType== "application")
                        if (extemtionType == "image")
                        {
                            string extension = Path.GetExtension(filename);
                            string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                            byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                            //f.byteArray = buffer;
                            string mime = filefromreq.Headers.ContentType.ToString();
                            Stream stream = new MemoryStream(buffer);
                            var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/TaskImages/"), dates + '.' + filename).Replace(" ", "");
                            string DirectoryURL = (FileUrl.Split(new string[] { "TaskImages" + "\\" }, StringSplitOptions.None).FirstOrDefault()) + "TaskImages";

                            //for create new Folder
                            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                            if (!objDirectory.Exists)
                            {
                                Directory.CreateDirectory(DirectoryURL);
                            }
                            //string path = "UploadImages\\" + compid + "\\" + filename;

                            string path = "uploadimage\\TaskImages\\" + dates + '.' + Fileresult + extension;

                            File.WriteAllBytes(FileUrl, buffer.ToArray());
                            result.Message = "Successful";
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
                        result.Message = "No content Passed ";
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

        #endregion Api To Upload Navigation Logo

        #region GAT ALL STATUS OF TASK  // dropdown
        /// <summary>
        /// Created By Ravi Vyas on 28-11-2022
        /// API >> Get >>api/taskcreation/getallstatusenum
        /// </summary>
        [Route("getallstatusenum")]
        [HttpGet]
        public ResponseBodyModel TaskStatus()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var getStatus = Enum.GetValues(typeof(TaskStatusConstants))
                    .Cast<TaskStatusConstants>()
                    .Select(x => new HelperModelForEnum
                    {
                        TypeId = (int)x,
                        TypeName = Enum.GetName(typeof(TaskStatusConstants), x).Replace("_", " ")
                    }).ToList();

                res.Message = "Status Get Successfully";
                res.Status = true;
                res.Data = getStatus;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get Reason for resignation // dropdown

        #region Get All Task Type Of Task  // dropdown
        /// <summary>
        /// Created By Ravi Vyas on 28-11-2022
        /// API >> Get >>api/taskcreation/getalltasktypeenum
        /// </summary>
        [Route("getalltasktypeenum")]
        [HttpGet]
        public ResponseBodyModel TaskType()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var getTaskType = Enum.GetValues(typeof(TaskTypeConstants))
                    .Cast<TaskTypeConstants>()
                    .Select(x => new HelperModelForEnum
                    {
                        TypeId = (int)x,
                        TypeName = Enum.GetName(typeof(TaskTypeConstants), x).Replace("_", " ")
                    }).ToList();

                res.Message = "Type Get Successfully";
                res.Status = true;
                res.Data = getTaskType;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get Reason for resignation // dropdown

        #region Get All Task  Priority Of Task  // dropdown

        /// <summary>
        /// Created By Ravi Vyas on 28-11-2022
        /// API >> Get >>api/taskcreation/getalltaskpriority
        /// </summary>
        [Route("getalltaskpriority")]
        [HttpGet]
        public ResponseBodyModel TaskPriority()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var getTaskPriority = Enum.GetValues(typeof(TaskPriorityConstants))
                    .Cast<TaskPriorityConstants>()
                    .Select(x => new HelperModelForEnum
                    {
                        TypeId = (int)x,
                        TypeName = Enum.GetName(typeof(TaskPriorityConstants), x).Replace("_", " ")
                    }).ToList();

                res.Message = "TaskPriority Get Successfully";
                res.Status = true;
                res.Data = getTaskPriority;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get Reason for resignation // dropdown

        #region GAT ALL billing STATUS OF TASK  // dropdown

        /// <summary>
        /// Created By Ravi Vyas on 28-11-2022
        /// API >> Get >>api/taskcreation/getallbillingstatus
        /// </summary>
        [Route("getallbillingstatus")]
        [HttpGet]
        public ResponseBodyModel TaskBilling()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var getStatus = Enum.GetValues(typeof(TaskBillingConstants))
                    .Cast<TaskStatusConstants>()
                    .Select(x => new HelperModelForEnum
                    {
                        TypeId = (int)x,
                        TypeName = Enum.GetName(typeof(TaskBillingConstants), x).Replace("_", " ")
                    }).ToList();

                res.Message = "Get Successfully";
                res.Status = true;
                res.Data = getStatus;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get Reason for resignation // dropdown

        #region Api for Task Filter
        /// <summary>
        /// API>POST>>api/taskcreation/taskfillters?ProjectId
        /// </summary>
        /// <param name="model"></param>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("taskfillters")]
        public async Task<IHttpActionResult> GetDataBy(FillterRequestModel model, int ProjectId, int? page = null, int? count = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                DateTimeOffset checkDate = new DateTimeOffset();
                DateTimeOffset checkDate1 = new DateTimeOffset();

                if (model.StartDate.HasValue)
                    checkDate = TimeZoneConvert.ConvertTimeToSelectedZone(model.StartDate.Value, tokenData.TimeZone);
                if (model.EndDate.HasValue)
                    checkDate1 = TimeZoneConvert.ConvertTimeToSelectedZone(model.EndDate.Value, tokenData.TimeZone);

                if (tokenData.IsAdminInCompany)
                {
                    //var checkPermison = await _db.TaskPermissions
                    //                  .Where(x => x.IsActive && !x.IsDeleted &&
                    //                  x.AssigneEmployeeId == tokenData.employeeId && x.ProjectId == ProjectId)
                    //                  .FirstOrDefaultAsync();


                    var getAll = await _db.TaskModels.
                                  Where(x => x.IsActive &&
                                  x.ProjectId == ProjectId &&
                                  x.TaskType != TaskTypeConstants.BackLog && x.CompanyId == tokenData.companyId).
                                  //Where(x => !x.IsDeleted && checkPermison.ViewAlProjectTask
                                  //?
                                  //x.CompanyId == tokenData.companyId
                                  //:
                                  //x.CompanyId == tokenData.companyId && x.AssignEmployeeId == tokenData.employeeId).
                                  Select(x => new TaskFillterResponseModel
                                  {
                                      TaskIdNumber = x.TaskIdNumber,
                                      TaskTitle = x.TaskTitle,
                                      TaskType = x.TaskType.ToString(),
                                      TaskTypeId = x.TaskType,
                                      TaskId = x.TaskId,
                                      Discription = x.Discription,
                                      AssignEmployeeName = _db.Employee.Where(a => a.EmployeeId == x.AssignEmployeeId && a.IsActive && !a.IsDeleted).Select(a => a.DisplayName).FirstOrDefault(),
                                      Priority = x.Priority.ToString(),
                                      Status = x.Status.ToString(),
                                      StatusId = x.Status,
                                      AssignEmployeeId = x.AssignEmployeeId.Value,
                                      EstimateTime = x.EstimateTime,
                                      EstTime = "",
                                      StartDate = x.StartDate.Value,
                                      EndDate = x.EndDate.Value,
                                      CreatedByName = _db.Employee.Where(a => a.EmployeeId == x.CreatedBy && a.IsActive && !a.IsDeleted).Select(a => a.DisplayName).FirstOrDefault(),
                                      TaskPercentage = x.Percentage,
                                      SpentTime = _db.TaskLogs.Where(t => t.TaskId == x.TaskId).Select(t => t.SpentTime).FirstOrDefault(),
                                      CreatedBy = x.CreatedBy,
                                      ProjectId = x.ProjectId,
                                      UpdatedBy = _db.TaskLogs.Where(t => t.TaskId == x.TaskId).Select(t => t.UpdatedBy).FirstOrDefault(),
                                      IsApproved = x.IsApproved,
                                      SpentTimeList = _db.TaskLogs.Where(t => t.TaskId == x.TaskId).Select(t => t.SpentTime).ToList(),
                                      SpentTime1 = "",
                                      ProjectTaskId = x.ProjectTaskId,
                                      BilingType = x.TaskBilling,
                                      SprintId = x.SprintId,
                                      SprintName = _db.Sprints.Where(s => s.SprintId == x.SprintId).Select(s => s.SprintName).FirstOrDefault(),
                                      Billing = x.TaskBilling.ToString()

                                  }).ToListAsync();

                    if (getAll.Count > 0)
                    {
                        getAll.ForEach(t =>
                        {
                            t.EstTime = string.Format("{00:00}:{01:00}", (int)t.EstimateTime / 60, t.EstimateTime % 60);
                            t.SpentTime1 = string.Format("{00:00}:{01:00}", (int)(t.SpentTimeList.Select(z => z.TotalMinutes).ToList().Sum() / 60),
                                                                            (int)(t.SpentTimeList.Select(z => z.TotalMinutes).ToList().Sum() % 60));
                        });
                        var predicate = PredicateBuilder.New<TaskFilterResponseModel>(x => x.IsActive && !x.IsDeleted);
                        if (model.Type.Count > 0)
                        {
                            getAll = (getAll.Where(x => model.Type.Contains(x.TaskTypeId))).ToList();
                        }
                        if (model.Status.Count > 0)
                        {
                            getAll = (getAll.Where(x => model.Status.Contains(x.StatusId))).ToList();
                        }
                        if (model.TaskIdNumber != 0 /*= TimeSpan.Zero*/)
                        {
                            getAll = (getAll.Where(x => x.TaskIdNumber == model.TaskIdNumber)).ToList();
                        }
                        if (model.AssigneeToId.Count > 0)
                        {
                            getAll = (getAll.Where(x => model.AssigneeToId.Contains(x.AssignEmployeeId))).ToList();
                        }
                        if (model.Titile != null)
                        {
                            getAll = (getAll.Where(x => x.TaskTitle.ToUpper().StartsWith(model.Titile.ToUpper())
                            || x.AssignEmployeeName.ToUpper().StartsWith(model.Titile.ToUpper())
                            || x.CreatedByName.ToUpper().StartsWith(model.Titile.ToUpper())
                            || x.TaskType.ToUpper().StartsWith(model.Titile.ToUpper()))).ToList();
                        }

                        if (model.LastUpdateId != 0)
                        {
                            getAll = (getAll.Where(x => x.UpdatedBy.Value == model.LastUpdateId)).ToList();
                        }
                        if (model.StartDate.HasValue && model.EndDate.HasValue)
                        {
                            getAll = (getAll.Where(x => x.StartDate.Date >= checkDate.Date && x.EndDate.Date <= checkDate1.Date)).ToList();
                        }
                        if (model.BilingType != 0)
                        {
                            getAll = (getAll.Where(x => x.BilingType == model.BilingType)).ToList();

                        }
                        if (model.SprintId.Count > 0)
                        {
                            getAll = (getAll.Where(x => model.SprintId.Contains(x.SprintId))).ToList();
                        }
                        res.Message = "Task list Found";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Found;
                        if (page.HasValue && count.HasValue)
                        {

                            res.Data = new PaginationData
                            {
                                TotalData = getAll.Count,
                                Counts = (int)count,
                                List = getAll.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                            };
                            return Ok(res);
                        }
                        else
                        {
                            res.Data = getAll;
                            return Ok(res);
                        }
                    }
                    res.Message = "Task list Not Found";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.NotFound;

                    return Ok(res);

                }
                else
                {
                    var checkPermison = await _db.TaskPermissions
                                      .Where(x => x.IsActive && !x.IsDeleted &&
                                      x.AssigneEmployeeId == tokenData.employeeId && x.ProjectId == ProjectId)
                                      .FirstOrDefaultAsync();


                    var getAll = await _db.TaskModels.
                                  Where(x => x.IsActive &&
                                  x.ProjectId == ProjectId &&
                                  x.TaskType != TaskTypeConstants.BackLog  ).
                                  Where(x => !x.IsDeleted && checkPermison.ViewAlProjectTask
                                  ?
                                  x.CompanyId == tokenData.companyId
                                  :
                                  x.CompanyId == tokenData.companyId && x.AssignEmployeeId == tokenData.employeeId).
                                  Select(x => new TaskFillterResponseModel
                                  {
                                      TaskIdNumber = x.TaskIdNumber,
                                      TaskTitle = x.TaskTitle,
                                      TaskType = x.TaskType.ToString(),
                                      TaskTypeId = x.TaskType,
                                      TaskId = x.TaskId,
                                      Discription = x.Discription,
                                      AssignEmployeeName = _db.Employee.Where(a => a.EmployeeId == x.AssignEmployeeId && a.IsActive && !a.IsDeleted).Select(a => a.DisplayName).FirstOrDefault(),
                                      Priority = x.Priority.ToString(),
                                      Status = x.Status.ToString(),
                                      StatusId = x.Status,
                                      AssignEmployeeId = x.AssignEmployeeId.Value,
                                      EstimateTime = x.EstimateTime,
                                      EstTime = "",
                                      StartDate = x.StartDate.Value,
                                      EndDate = x.EndDate.Value,
                                      CreatedByName = _db.Employee.Where(a => a.EmployeeId == x.CreatedBy && a.IsActive && !a.IsDeleted).Select(a => a.DisplayName).FirstOrDefault(),
                                      TaskPercentage = x.Percentage,
                                      SpentTime = _db.TaskLogs.Where(t => t.TaskId == x.TaskId).Select(t => t.SpentTime).FirstOrDefault(),
                                      CreatedBy = x.CreatedBy,
                                      ProjectId = x.ProjectId,
                                      UpdatedBy = _db.TaskLogs.Where(t => t.TaskId == x.TaskId).Select(t => t.UpdatedBy).FirstOrDefault(),
                                      IsApproved = x.IsApproved,
                                      SpentTimeList = _db.TaskLogs.Where(t => t.TaskId == x.TaskId).Select(t => t.SpentTime).ToList(),
                                      SpentTime1 = "",
                                      ProjectTaskId = x.ProjectTaskId,
                                      BilingType = x.TaskBilling,
                                      SprintId = x.SprintId,
                                      SprintName = _db.Sprints.Where(s => s.SprintId == x.SprintId).Select(s => s.SprintName).FirstOrDefault(),
                                      Billing = x.TaskBilling.ToString()

                                  }).ToListAsync();

                    if (getAll.Count > 0)
                    {
                        getAll.ForEach(t =>
                        {
                            t.EstTime = string.Format("{00:00}:{01:00}", (int)t.EstimateTime / 60, t.EstimateTime % 60);
                            t.SpentTime1 = string.Format("{00:00}:{01:00}", (int)(t.SpentTimeList.Select(z => z.TotalMinutes).ToList().Sum() / 60),
                                                                            (int)(t.SpentTimeList.Select(z => z.TotalMinutes).ToList().Sum() % 60));
                        });
                        var predicate = PredicateBuilder.New<TaskFilterResponseModel>(x => x.IsActive && !x.IsDeleted);
                        if (model.Type.Count > 0)
                        {
                            getAll = (getAll.Where(x => model.Type.Contains(x.TaskTypeId))).ToList();
                        }
                        if (model.Status.Count > 0)
                        {
                            getAll = (getAll.Where(x => model.Status.Contains(x.StatusId))).ToList();
                        }
                        if (model.TaskIdNumber != 0 /*= TimeSpan.Zero*/)
                        {
                            getAll = (getAll.Where(x => x.TaskIdNumber == model.TaskIdNumber)).ToList();
                        }
                        if (model.AssigneeToId.Count > 0)
                        {
                            getAll = (getAll.Where(x => model.AssigneeToId.Contains(x.AssignEmployeeId))).ToList();
                        }
                        if (model.Titile != null)
                        {
                            getAll = (getAll.Where(x => x.TaskTitle.ToUpper().StartsWith(model.Titile.ToUpper())
                            || x.AssignEmployeeName.ToUpper().StartsWith(model.Titile.ToUpper())
                            || x.CreatedByName.ToUpper().StartsWith(model.Titile.ToUpper())
                            || x.TaskType.ToUpper().StartsWith(model.Titile.ToUpper()))).ToList();
                        }

                        if (model.LastUpdateId != 0)
                        {
                            getAll = (getAll.Where(x => x.UpdatedBy.Value == model.LastUpdateId)).ToList();
                        }
                        if (model.StartDate.HasValue && model.EndDate.HasValue)
                        {
                            getAll = (getAll.Where(x => x.StartDate.Date >= checkDate.Date && x.EndDate.Date <= checkDate1.Date)).ToList();
                        }
                        if (model.BilingType != 0)
                        {
                            getAll = (getAll.Where(x => x.BilingType == model.BilingType)).ToList();

                        }
                        if (model.SprintId.Count > 0)
                        {
                            getAll = (getAll.Where(x => model.SprintId.Contains(x.SprintId))).ToList();
                        }
                        res.Message = "Task list Found";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Found;
                        if (page.HasValue && count.HasValue)
                        {

                            res.Data = new PaginationData
                            {
                                TotalData = getAll.Count,
                                Counts = (int)count,
                                List = getAll.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                            };
                            return Ok(res);
                        }
                        else
                        {
                            res.Data = getAll;
                            return Ok(res);
                        }
                    }
                    res.Message = "Task list Not Found";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.NotFound;

                    return Ok(res);
                }


            }
            catch (Exception ex)
            {
                logger.Error("api/taskcreation/taskfillters", ex.Message, model);
                return BadRequest("Failed");
            }

        }

        public class TaskFillterResponseModel : TaskResponseModel
        {
            public int ProjectId { get; set; }
            public TaskTypeConstants TaskTypeId { get; set; }
            public TaskStatusConstants StatusId { get; set; }
            public int CreatedBy { get; set; } = 0;
            public int? UpdatedBy { get; set; } = null;
            public TaskBillingConstants BilingType { get; set; }
            public List<TimeSpan> SpentTimeList { get; set; }

        }


        #endregion

        #region HELPER MODEL CLASS

        public class FillterRequestModel
        {

            public List<TaskTypeConstants> Type { get; set; } = new List<TaskTypeConstants>();
            public List<TaskStatusConstants> Status { get; set; } = new List<TaskStatusConstants>();
            //public int Percentage { get; set; }
            //public TimeSpan EstimateTime { get; set; }
            public int TaskIdNumber { get; set; }
            public int AssigneeId { get; set; }
            public List<int> AssigneeToId { get; set; } = new List<int>();
            public string Titile { get; set; }
            public int ProjectId { get; set; }
            public int LastUpdateId { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public bool IsApproved { get; set; }
            public TaskBillingConstants BilingType { get; set; }
            public List<Guid> SprintId { get; set; } = new List<Guid>();
        }

        public class TaskFilterResponseModel
        {
            public Guid TaskId { get; set; }
            public string TaskTitle { get; set; }
            public string Discription { get; set; }
            public string Priority { get; set; }
            //public string TaskType { get; set; }
            //public string Status { get; set; }
            public List<TaskTypeConstants> Type { get; set; }
            public List<TaskStatusConstants> Status { get; set; }

            public string AssignEmployeeName { get; set; }
            public int AssignEmployeeId { get; set; }
            public string Attechment { get; set; }
            public TimeSpan EstimateTime { get; set; }
            public DateTimeOffset StartDate { get; set; }
            public DateTimeOffset EndDate { get; set; }
            public int TaskIdNumber { get; set; }
            public string CreatedByName { get; set; }
            public int TaskPercentage { get; set; }
            public TimeSpan SpentTime { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
        }

        #endregion

        #region This Api Use To Send Task Mail For Assigend Employee
        ///// <summary>
        ///// Create By Ravi vyas Date-12-12-2022
        ///// </summary>
        ///// <returns></returns>
        public async Task SendAssignTaskMail(TaskModel model)
        {
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");
                var URl = model.TaskURL + model.TaskId;
                var employeeData = _db.Employee.Where(x => x.EmployeeId == model.AssignEmployeeId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                var createEmployee = _db.Employee.Where(x => x.EmployeeId == model.CreatedBy && x.IsActive && !x.IsDeleted).FirstOrDefault();
                string timeData = string.Format("{00:00}:{01:00}", (int)model.EstimateTime / 60, model.EstimateTime % 60);
                var companylist = _db.Company.Where(y => y.CompanyId == model.CompanyId
                    && y.IsActive && !y.IsDeleted)
                  .Select(x => new
                  {
                      x.RegisterAddress,
                      x.RegisterCompanyName

                  }).FirstOrDefault();
                var projectName = _db.ProjectLists.Where(x => x.ID == model.ProjectId && x.IsActive && !x.IsDeleted).Select(x => x.ProjectName).FirstOrDefault();
                var attachmentPath = Path.Combine(HttpRuntime.AppDomainAppPath, "uploadimage\\MailImages");
                string htmlBody = TaskHelper.CreateTaskMailBody
                    .Replace("<|CRETEBY|>", createEmployee.DisplayName)
                    .Replace("<|TASKID|>", model.TaskIdNumber.ToString())
                    .Replace("<|CREATEFOR|>", employeeData.DisplayName)
                    .Replace("<|CREATEDATE|>", model.StartDate.Value.ToString("dd-MM-yyyy"))
                    .Replace("<|TASKLINK|>", URl)
                    .Replace("<|CREATEBY|>", createEmployee.DisplayName)
                    .Replace("<|TASKTITLE|>", model.TaskTitle)
                    .Replace("<|CREATEFOR|>", employeeData.DisplayName)
                    .Replace("<|STATUS|>", model.Status.ToString())
                    .Replace("<|PRIORITY|>", model.Priority.ToString())
                    .Replace("|PROJECTNAME|>", projectName)
                    .Replace("<|ESTIMATETIME|>", timeData)
                    .Replace("<|DISCRIPTION|>", model.Discription)
                    .Replace("<|IMAGE_PATH|>", "emossy.png")
                    .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                    .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                oMail.ImportHtml(htmlBody, attachmentPath, ImportHtmlBodyOptions.ImportLocalPictures | ImportHtmlBodyOptions.ImportCss);
                oMail.From = ConfigurationManager.AppSettings["MasterEmail"];

                // Set recipient email address
                AddressCollection obj = new AddressCollection();
                obj.Add(employeeData.OfficeEmail);
                obj.Add(createEmployee.OfficeEmail);
                oMail.To = obj;

                //oMail.To = employeeData.OfficeEmail;

                // Set email subject
                oMail.Subject = model.TaskType.ToString() + " " + employeeData.DisplayName;

                // Set email body
                oMail.TextBody = "Find your Task";
                //oMail.HtmlBody = body;
                // Hotmail/Outlook SMTP server address
                SmtpServer oServer = new SmtpServer("smtp.office365.com");
                oServer.User = ConfigurationManager.AppSettings["MailUser"];

                // If you got authentication error, try to create an app password instead of your user password.
                // https://support.microsoft.com/en-us/account-billing/using-app-passwords-with-apps-that-don-t-support-two-step-verification-5896ed9b-4263-e681-128a-a6f2979a7944
                oServer.Password = ConfigurationManager.AppSettings["MailPassword"];
                oServer.Port = 587;

                // detect SSL/TLS connection automatically
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                Console.WriteLine("start to send email over TLS...");
                SmtpClient oSmtp = new SmtpClient();
                await oSmtp.SendMailAsync(oServer, oMail);
                Console.WriteLine("email was sent successfully!");

            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
        }
        #endregion

        #region This Api Use To Send Task Mail For Assigend Employee
        ///// <summary>
        ///// Create By Ravi vyas Date-12-12-2022
        ///// </summary>
        ///// <returns></returns>
        public async Task SendAssignTaskMailByTask(TaskModel model, bool isSmtpProvided, int companyId)
        {

            try
            {
                var URl = model.TaskURL + model.TaskId;
                var employeeData = _db.Employee.Where(x => x.EmployeeId == model.AssignEmployeeId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                var createEmployee = _db.Employee.Where(x => x.EmployeeId == model.CreatedBy && x.IsActive && !x.IsDeleted).FirstOrDefault();
                string timeData = string.Format("{00:00}:{01:00}", (int)model.EstimateTime / 60, model.EstimateTime % 60);
                var companylist = _db.Company.Where(y => y.CompanyId == model.CompanyId
                    && y.IsActive && !y.IsDeleted)
                  .Select(x => new
                  {
                      x.RegisterAddress,
                      x.RegisterCompanyName

                  }).FirstOrDefault();
                var projectName = _db.ProjectLists.Where(x => x.ID == model.ProjectId && x.IsActive && !x.IsDeleted).Select(x => x.ProjectName).FirstOrDefault();
                SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                if (isSmtpProvided)
                {
                    smtpsettings = await _db.CompanySmtpMailModels
                        .Where(x => x.CompanyId == companyId)
                        .Select(x => new SmtpSendMailRequest
                        {
                            From = x.From,
                            SmtpServer = x.SmtpServer,
                            MailUser = x.MailUser,
                            MailPassword = x.Password,
                            Port = x.Port,
                            ConectionType = x.ConnectType,
                        })
                        .FirstOrDefaultAsync();
                }
                string htmlBody = TaskHelper.CreateTaskMailBody
                    .Replace("<|CRETEBY|>", createEmployee.DisplayName)
                    .Replace("<|TASKID|>", model.TaskIdNumber.ToString())
                    .Replace("<|CREATEFOR|>", employeeData.DisplayName)
                    .Replace("<|CREATEDATE|>", model.StartDate.Value.ToString("dd-MM-yyyy"))
                    .Replace("<|TASKLINK|>", URl)
                    .Replace("<|CREATEBY|>", createEmployee.DisplayName)
                    .Replace("<|TASKTITLE|>", model.TaskTitle)
                    .Replace("<|CREATEFOR|>", employeeData.DisplayName)
                    .Replace("<|STATUS|>", model.Status.ToString())
                    .Replace("<|PRIORITY|>", model.Priority.ToString())
                    .Replace("<|PROJECTNAME|>", projectName)
                    .Replace("<|ESTIMATETIME|>", timeData)
                    .Replace("<|DISCRIPTION|>", model.Discription)
                    .Replace("<|IMAGE_PATH|>", "emossy.png")
                    .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                    .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                SendMailModelRequest sendMailObject = new SendMailModelRequest()
                {
                    IsCompanyHaveDefaultMail = isSmtpProvided,
                    Subject = "Find your Task",
                    MailBody = htmlBody,
                    MailTo = new List<string>() { createEmployee.OfficeEmail, employeeData.OfficeEmail },
                    SmtpSettings = smtpsettings,
                };
                await SmtpMailHelper.SendMailAsync(sendMailObject);

            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
        }
        #endregion

        #region This Api Use To Send Task Backlog Mail  
        ///// <summary>
        ///// Create By Ravi vyas Date-12-12-2022
        ///// </summary>
        ///// <returns></returns>
        public async Task<bool> SendAssignTaskMailBackLog(TaskModel model)
        {
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");
                //var assignEmployeeName = _db.TicketCategoryEmployees.Where(x => x.TicketCategoryId == model.TicketCategoryId && x.IsActive && !x.IsDeleted).Select(x => x.EmployeeId).ToList();
                //foreach (var employee in assignEmployeeName)
                //{
                var URl = model.TaskURL + model.TaskId;
                var createEmployee = _db.Employee.Where(x => x.EmployeeId == model.CreatedBy && x.IsActive && !x.IsDeleted).FirstOrDefault();
                string timeData = string.Format("{00:00}:{01:00}", (int)model.EstimateTime / 60, model.EstimateTime % 60);
                var companylist = _db.Company.Where(y => y.CompanyId == model.CompanyId
                    && y.IsActive && !y.IsDeleted)
                  .Select(x => new
                  {
                      x.RegisterAddress,
                      x.RegisterCompanyName

                  }).FirstOrDefault();
                var attachmentPath = Path.Combine(HttpRuntime.AppDomainAppPath, "uploadimage\\MailImages");
                string htmlBody = TaskHelper.CreateTaskMailBody
                    .Replace("<|CRETEBY|>", createEmployee.DisplayName)
                    .Replace("<|TASKID|>", model.TaskIdNumber.ToString())
                    .Replace("<|CREATEDATE|>", model.StartDate.Value.ToString("dd-MM-yyyy"))
                    .Replace("<|TASKLINK|>", URl)
                    .Replace("<|CREATEBY|>", createEmployee.DisplayName)
                    .Replace("<|TASKTITLE|>", model.TaskTitle)
                    .Replace("<|STATUS|>", model.Status.ToString())
                    .Replace("<|DISCRIPTION|>", model.Discription)
                    .Replace("<|IMAGE_PATH|>", "emossy.png")
                    .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                    .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                oMail.ImportHtml(htmlBody, attachmentPath, ImportHtmlBodyOptions.ImportLocalPictures | ImportHtmlBodyOptions.ImportCss);
                oMail.From = ConfigurationManager.AppSettings["MasterEmail"];

                // Set recipient email address
                AddressCollection obj = new AddressCollection();
                obj.Add(createEmployee.OfficeEmail);
                oMail.To = obj;

                //oMail.To = employeeData.OfficeEmail;

                // Set email subject
                oMail.Subject = "Back Log Task Created";

                // Set email body
                oMail.TextBody = "Find your Task";
                //oMail.HtmlBody = body;
                // Hotmail/Outlook SMTP server address
                SmtpServer oServer = new SmtpServer("smtp.office365.com");
                oServer.User = ConfigurationManager.AppSettings["MailUser"];

                // If you got authentication error, try to create an app password instead of your user password.
                // https://support.microsoft.com/en-us/account-billing/using-app-passwords-with-apps-that-don-t-support-two-step-verification-5896ed9b-4263-e681-128a-a6f2979a7944
                oServer.Password = ConfigurationManager.AppSettings["MailPassword"];
                oServer.Port = 587;

                // detect SSL/TLS connection automatically
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                Console.WriteLine("start to send email over TLS...");
                SmtpClient oSmtp = new SmtpClient();
                await oSmtp.SendMailAsync(oServer, oMail);
                Console.WriteLine("email was sent successfully!");

            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
            return true;
        }
        public async Task SendAssignTaskMailBackLog(TaskModel model, bool isSmtpProvided, int companyId)
        {
            try
            {
                string URl = model.TaskURL + model.TaskId;
                var createEmployee = _db.Employee.Where(x => x.EmployeeId == model.CreatedBy && x.IsActive && !x.IsDeleted).FirstOrDefault();
                string timeData = string.Format("{00:00}:{01:00}", (int)model.EstimateTime / 60, model.EstimateTime % 60);
                var companyData = _db.Company.Where(y => y.CompanyId == model.CompanyId
                    && y.IsActive && !y.IsDeleted)
                  .Select(x => new
                  {
                      x.RegisterAddress,
                      x.RegisterCompanyName

                  }).FirstOrDefault();
                var projectName = _db.ProjectLists.Where(x => x.ID == model.ProjectId && x.IsActive && !x.IsDeleted).Select(x => x.ProjectName).FirstOrDefault();
                SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                if (isSmtpProvided)
                {
                    smtpsettings = await _db.CompanySmtpMailModels
                        .Where(x => x.CompanyId == companyId)
                        .Select(x => new SmtpSendMailRequest
                        {
                            From = x.From,
                            SmtpServer = x.SmtpServer,
                            MailUser = x.MailUser,
                            MailPassword = x.Password,
                            Port = x.Port,
                            ConectionType = x.ConnectType,
                        })
                        .FirstOrDefaultAsync();
                }
                string htmlBody = TaskHelper.CreateTaskMailBody
                    .Replace("<|CRETEBY|>", createEmployee.DisplayName)
                    .Replace("<|TASKID|>", model.TaskIdNumber.ToString())
                    .Replace("<|CREATEDATE|>", model.StartDate.Value.ToString("dd-MM-yyyy"))
                    .Replace("<|TASKLINK|>", URl)
                    .Replace("<|CREATEBY|>", createEmployee.DisplayName)
                    .Replace("<|TASKTITLE|>", model.TaskTitle)
                    .Replace("<|STATUS|>", model.Status.ToString())
                    .Replace("<|DISCRIPTION|>", model.Discription)
                    .Replace("|PROJECTNAME|>", projectName)
                    .Replace("<|IMAGE_PATH|>", "emossy.png")
                    .Replace("<|COMPANYNAMEE|>", companyData.RegisterCompanyName)
                    .Replace("<|COMPANYADDRESS|>", companyData.RegisterAddress);
                SendMailModelRequest sendMailObject = new SendMailModelRequest()
                {
                    IsCompanyHaveDefaultMail = isSmtpProvided,
                    Subject = "Back Log Created",
                    MailBody = htmlBody,
                    MailTo = new List<string>() { createEmployee.OfficeEmail },
                    SmtpSettings = smtpsettings,
                };
                await SmtpMailHelper.SendMailAsync(sendMailObject);
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
        }
        #endregion

        #region Api for Get Task By TaskId
        /// <summary>
        /// API>>GET>>api/taskcreation/gettaskbytaskid?TaskId
        /// </summary>
        /// <param name="TaskId"></param>
        /// <returns></returns>
        [Route("gettaskbytaskid")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTaskById(Guid TaskId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {

                var time = _db.TaskModels.Where(x => x.IsActive && !x.IsDeleted &&
                              x.CompanyId == tokenData.companyId && x.TaskId == TaskId).FirstOrDefault();
                string EstimateTime = string.Format("{00:00}:{01:00}", (int)time.EstimateTime / 60, time.EstimateTime % 60);

                var allTask = await _db.TaskModels.Where(x => x.IsActive && !x.IsDeleted &&
                              x.CompanyId == tokenData.companyId && x.TaskId == TaskId)
                              .Select(x => new TaskGetByIdResponse
                              {
                                  ProjectId = x.ProjectId,
                                  TaskIdNumber = x.TaskIdNumber,
                                  //SubProjectId = x.SubProjectId,
                                  TaskTitle = x.TaskTitle,
                                  TaskType = x.TaskType.ToString(),
                                  TaskTypeEnum = x.TaskType,
                                  TaskBilling = x.TaskBilling,
                                  TaskId = x.TaskId,
                                  Discription = x.Discription,
                                  AssignEmployeeName = _db.Employee.Where(a => a.EmployeeId == x.AssignEmployeeId && x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId).Select(a => a.DisplayName).FirstOrDefault(),
                                  Priority = x.Priority.ToString(),
                                  Status = x.Status.ToString(),
                                  AssignEmployeeId = x.AssignEmployeeId.Value,
                                  Attechment = x.Attechment,
                                  EstimateTime = x.EstimateTime,
                                  TaskStartDate = x.StartDate,
                                  EstimateTime1 = EstimateTime,
                                  TaskEndDate = x.EndDate,
                                  Image1 = x.Image1,
                                  Image2 = x.Image2,
                                  Image3 = x.Image3,
                                  Image4 = x.Image4,
                                  Comments = _db.TaskLogs.Where(c => c.TaskId == x.TaskId).Select(c => c.Comment).FirstOrDefault(),
                                  CreatedByName = _db.Employee.Where(a => a.EmployeeId == x.CreatedBy && x.IsActive && !x.IsDeleted).Select(a => a.DisplayName).FirstOrDefault(),
                                  TaskPercentage = x.Percentage,
                                  TimeSpans = _db.TaskLogs.Where(t => t.TaskId == x.TaskId).Select(t => t.SpentTime).ToList(),
                                  IsApproved = _db.TaskApprovels.Where(a => a.TaskId == x.TaskId).Select(a => a.IsApproved).FirstOrDefault(),
                                  CommentCount = _db.TaskComments.Where(t => t.TaskId == TaskId && t.IsActive && !x.IsDeleted).Count(),
                                  UpdatedBy = _db.Employee.Where(a => a.EmployeeId == x.UpdatedBy && x.IsActive && !x.IsDeleted).Select(a => a.DisplayName).FirstOrDefault(),
                                  UpdatedDate = x.UpdatedOn,
                                  SpentTime1 = "",
                                  SprintId = x.SprintId,
                                  SprintName = _db.Sprints.Where(p => p.SprintId == x.SprintId).Select(p => p.SprintName).FirstOrDefault(),


                              }).FirstOrDefaultAsync();

                if (allTask != null)
                {
                    allTask.SpentTime1 = string.Format("{00:00}:{01:00}", (int)(allTask.TimeSpans.Select(z => z.TotalMinutes).ToList().Sum() / 60),
                                                                          (int)(allTask.TimeSpans.Select(z => z.TotalMinutes).ToList().Sum() % 60));

                    res.Message = "Data Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = allTask;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = allTask;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {

                logger.Error("api/taskcreation/getalltask", ex.Message);
                return BadRequest("Failed");
            }
        }

        public class TaskGetByIdResponse : TaskResponseModel
        {
            public int ProjectId { get; set; }
            public TaskTypeConstants TaskTypeEnum { get; set; }
            public TaskBillingConstants TaskBilling { get; set; }
            public TaskPriorityConstants PriorityId { get; set; }
            public string EstimateTime1 { get; set; }
            public List<TimeSpan> TimeSpans { get; set; }
            public int CommentCount { get; set; }
            public string UpdatedBy { get; set; }
            public DateTimeOffset? UpdatedDate { get; set; }
            public DateTimeOffset? TaskStartDate { get; set; }
            public DateTimeOffset? TaskEndDate { get; set; }
            public int SubProjectId { get; set; } = 0;

        }

        #endregion

        #region API TO UPLOAD TASK IMAGE MULTIPLE
        /// <summary>
        /// api/taskcreation/uploadtasktmultiple
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadtasktmultiple")]
        public async Task<HttpResponseMessageMultiple> UploadImageMullti()
        {
            HttpResponseMessageMultiple result = new HttpResponseMessageMultiple();
            List<PathLists> list = new List<PathLists>();
            try
            {
                var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
                List<string> path = new List<string>();
                var data = Request.Content.IsMimeMultipartContent();
                if (Request.Content.IsMimeMultipartContent())
                {
                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);
                    var content = provider.Contents.Count;

                    for (int i = 0; i < content; i++)
                    {
                        var dates = DateTime.Now.ToString("yyyyMMddhhmmsstt");
                        var filefromreq = provider.Contents[i];
                        Stream _id = filefromreq.ReadAsStreamAsync().Result;
                        StreamReader reader = new StreamReader(_id);
                        string filename = filefromreq.Headers.ContentDisposition.FileName.Trim('\"');

                        ////////////// Add By Mohit 12-07-2021
                        string extension = Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);

                        ////////////// Add By Mohit 12-07-2021
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        //f.byteArray = buffer;
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        //var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/TicketIages/" + claims.companyId), dates + filename);
                        //string DirectoryURL = (FileUrl.Split(new string[] { claims.companyId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.companyId;

                        var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/TaskImages/"), dates + '.' + filename).Replace(" ", "");
                        string DirectoryURL = (FileUrl.Split(new string[] { "TaskImages" + "\\" }, StringSplitOptions.None).FirstOrDefault()) + "TaskImages";

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        ////////////// old Code 12-07-2021
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        //var temp = "uploadimage\\TicketIages\\" + claims.companyId + "\\" + dates + Fileresult + extension;

                        //var temp = "uploadimage\\TaskImages\\" + claims.companyId + "\\" + dates + Fileresult + extension;
                        string temp = "uploadimage\\TaskImages\\" + dates + '.' + Fileresult + extension;

                        ////////////// old Code 12-07-2021

                        File.WriteAllBytes(FileUrl, buffer.ToArray());
                        PathLists obj = new PathLists
                        {
                            Pathurl = temp.Replace(" ", ""),
                        };
                        list.Add(obj);
                        path.Add(temp.Replace(" ", ""));
                        var listdata = String.Join(",", list);
                    }

                    result.Message = "Successful";
                    result.Success = true;
                    result.Paths = list;
                    result.PathArray = path;
                }
                else
                {
                    result.Message = "Error";
                    result.Success = false;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }
        #endregion

        #region Api for Update Task Status Or Percantage
        /// <summary>
        /// API>>PUT>>api/taskcreation/updatetaskdata
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [Route("updatetaskdata")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateTaskData(TaskUpdateRequestModel model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkData = _db.TaskModels.Where(x => x.TaskId == model.TaskId
                && x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId).FirstOrDefault();
                if (checkData != null)
                {
                    checkData.Status = model.Status;
                    checkData.Percentage = model.Percentage;
                    checkData.UpdatedOn = DateTimeOffset.Now;
                    checkData.AssignEmployeeId = model.AssignedEmployeeId;
                    checkData.UpdatedBy = tokenData.employeeId;
                    _db.Entry(checkData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Update Successfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = checkData;

                    return Ok(res);

                }
                else
                {
                    res.Message = "Data Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = checkData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {

                logger.Error("api/taskcreation/updatetaskdata", ex.Message, model);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }
        }
        public class TaskUpdateRequestModel
        {
            public Guid TaskId { get; set; }
            public TaskStatusConstants Status { get; set; }
            public int Percentage { get; set; }
            public int AssignedEmployeeId { get; set; }
            public TimeSpan SpentTime { get; set; }
        }

        #endregion

        #region Api for Delele Task
        /// <summary>
        /// Created By Shagun Moyade on 30-12-22
        /// API>>Delete>>api/taskcreation/deletetask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("deletetask")]
        [HttpPost]
        public async Task<IHttpActionResult> DeleteTaskData(Guid TaskID)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkData = _db.TaskModels.Where(x => x.TaskId == TaskID
                && x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId).FirstOrDefault();
                if (checkData != null)
                {
                    checkData.IsActive = false;
                    checkData.IsDeleted = true;
                    checkData.DeletedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                    checkData.DeletedBy = tokenData.employeeId;
                    _db.Entry(checkData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Deleted Successfully!";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = checkData;

                    TaskModelHistory history = new TaskModelHistory();
                    history.TaskId = checkData.TaskId;
                    history.TaskCode = checkData.TaskCode;
                    history.ProjectId = checkData.ProjectId;
                    history.TaskTitle = checkData.TaskTitle;
                    history.Discription = checkData.Discription;
                    history.Priority = checkData.Priority;
                    history.TaskType = checkData.TaskType;
                    history.Status = checkData.Status;
                    history.TaskTypeId = checkData.TaskTypeId;
                    history.TaskBilling = checkData.TaskBilling;
                    history.TaskIdNumber = checkData.TaskIdNumber;
                    history.AssignEmployeeId = checkData.AssignEmployeeId;
                    history.Percentage = checkData.Percentage;
                    history.Attechment = checkData.Attechment;
                    history.Image1 = checkData.Image1;
                    history.Image2 = checkData.Image2;
                    history.Image3 = checkData.Image3;
                    history.Image4 = checkData.Image4;
                    history.EstimateTime = checkData.EstimateTime;
                    history.StartDate = checkData.StartDate;
                    history.IsApproved = checkData.IsApproved;
                    history.EndDate = checkData.EndDate;
                    history.IsMail = checkData.IsMail;
                    history.TaskURL = checkData.TaskURL;
                    history.IsActive = checkData.IsActive;
                    history.IsDeleted = checkData.IsDeleted;
                    history.CompanyId = checkData.CompanyId;
                    history.OrgId = checkData.OrgId;
                    history.CreatedBy = checkData.CreatedBy;
                    history.CreatedOn = checkData.CreatedOn;
                    history.DeletedBy = checkData.DeletedBy;
                    history.DeletedOn = checkData.DeletedOn;
                    _db.TaskModelHistories.Add(history);
                    _db.SaveChanges();

                    return Ok(res);

                }
                else
                {
                    res.Message = "Task Not Found!";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    //res.Data = checkData;
                    return Ok(res);
                }

            }
            catch (Exception ex)
            {

                logger.Error("api/taskcreation/deletetask", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }
        }


        #endregion

        #region Api for get All Project Repo Links by Project ID
        /// <summary>
        /// API>>GET>>api/taskcreation/getrepolinkbyprojectid
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getrepolinkbyprojectid")]
        public async Task<IHttpActionResult> GetAllProjectLink(int projectId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getLinks = await _db.ProjectLists.
                                     Where(x => x.IsActive && !x.IsDeleted && x.ID == projectId).
                                     Select(x => new
                                     {
                                         LinkJson = x.LinkJson,
                                     })
                                     .ToListAsync();
                var checkData = getLinks.
                                Select(y => new
                                {
                                    LinkJsonData = JsonConvert.DeserializeObject<List<LinkJsonResponse>>(y.LinkJson),
                                }).FirstOrDefault();
                if (getLinks.Count > 0)
                {
                    res.Message = "Data Get Successfully!";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = checkData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = getLinks;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/taskcreation/getrepolinkbyprojectid", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }
        }

        public class RepoLinkReponse
        {
            public List<LinkJsonResponse> LinkJsonResponse { get; set; }
        }
        public class LinkJsonResponse
        {
            public string LinkName { get; set; }
            public string LinkUrl { get; set; }
        }

        #endregion

        #region Get Task History By ProjectId
        /// <summary>
        /// Created by Suraj Bundel on 07/02/2023
        /// API => GET => api/taskcreation/taskhistory?projectId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("taskhistory")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTaskHistory(int projectId, int? page = null, int? count = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokendata = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var task = await _db.TaskModelHistories.
                                  Where(x => x.ProjectId == projectId && x.CompanyId == tokendata.companyId && !x.IsActive && x.IsDeleted)
                                 .Select(x => new TaskModelHistoryResponse
                                 {
                                     TaskHistoryId = x.TaskHistoryId,
                                     TaskId = x.TaskId,
                                     ProjectId = x.ProjectId,
                                     TaskTitle = x.TaskTitle,
                                     Discription = x.Discription,
                                     Priority = x.Priority.ToString(),
                                     TaskType = x.TaskType.ToString(),
                                     Status = x.Status.ToString(),
                                     TaskBilling = x.TaskBilling.ToString(),
                                     TaskIdNumber = x.TaskIdNumber,
                                     AssignEmployeeId = x.AssignEmployeeId,
                                     EstimateTime = x.EstimateTime,
                                     StartDate = x.StartDate,
                                     EndDate = x.EndDate,
                                     CreatedBy = x.CreatedBy,
                                     DeletedBy = x.DeletedBy,
                                     CreatedOn = x.CreatedOn,
                                     DeletedOn = x.DeletedOn,
                                     AssigneName = _db.Employee.Where(y => y.EmployeeId == x.AssignEmployeeId && y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId).Select(y => y.DisplayName).FirstOrDefault(),
                                     CreatedByName = _db.Employee.Where(y => y.EmployeeId == x.CreatedBy && y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId).Select(y => y.DisplayName).FirstOrDefault(),
                                     DeletedByName = _db.Employee.Where(y => y.EmployeeId == x.DeletedBy && y.IsActive && !y.IsDeleted && y.CompanyId == tokendata.companyId).Select(y => y.DisplayName).FirstOrDefault(),
                                 })
                                 .OrderByDescending(x => x.DeletedOn)
                                 .ToListAsync();

                if (task.Count > 0)
                {
                    res.Message = "Data Get Successfully!";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    if (page.HasValue && count.HasValue)
                    {
                        res.Data = new
                        {
                            TotalData = task.Count,
                            Counts = (int)count,
                            List = task.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                        return Ok(res);
                    }
                    else
                    {
                        res.Data = task;
                        return Ok(res);
                    }
                }
                else
                {
                    res.Message = "Data Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = task;
                    return Ok(res);
                }
            }

            catch (Exception ex)
            {
                logger.Error("api/taskcreation/taskhistory", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }
        }
        #endregion

        #region Helper Model Classs For Histiry
        public class TaskModelHistoryResponse
        {
            public Guid TaskHistoryId { get; set; }
            public Guid TaskId { get; set; }
            public int ProjectId { get; set; }
            public string TaskTitle { get; set; }
            public string Discription { get; set; }
            public string Priority { get; set; }
            public string TaskType { get; set; }
            public string Status { get; set; }
            public string TaskBilling { get; set; }
            public int TaskIdNumber { get; set; }
            public int? AssignEmployeeId { get; set; }
            public long EstimateTime { get; set; }
            public DateTimeOffset? StartDate { get; set; }
            public DateTimeOffset? EndDate { get; set; }
            public int CreatedBy { get; set; }
            public int? DeletedBy { get; set; }
            public DateTimeOffset CreatedOn { get; set; }
            public DateTimeOffset? DeletedOn { get; set; }
            public string AssigneName { get; set; }
            public string CreatedByName { get; set; }
            public string DeletedByName { get; set; }
        }
        #endregion


    }
}
