using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.TimeSheet;
using AspNetIdentity.WebApi.Models;
using EASendMail;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;
using SmtpClient = EASendMail.SmtpClient;

namespace AspNetIdentity.WebApi.Controllers.TimeSheet
{
    /// <summary>
    /// Created By Ravi Vyas On 15-12-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/taskimportexport")]
    public class TaskImportExportController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API To Get Faulty Task Import Data In 
        /// <summary>
        /// Created By Ravi Vyas On 16-12-2022
        /// API >> Get >> api/taskimportexport/getfaultyimporttask
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getfaultyimporttask")]
        public async Task<IHttpActionResult> GetFultyImportTask()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faultyReport = await _db.TaskImportFaultyLogsGroups.
                                          Where(x => x.CompanyId == tokenData.companyId &&
                                          x.IsActive && !x.IsDeleted).
                                          OrderByDescending(x => x.CreatedOn).
                                          ToListAsync();
                if (faultyReport.Count > 0)
                {
                    res.Message = "Faulty Task Reports";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = faultyReport;
                    return Ok(res);
                }
                else
                {
                    res.Message = "No Faulty Assets Imported";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = faultyReport;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/taskimportexport/getfaultyimporttask", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }

        }
        #endregion

        #region API To Get Faulty Import Task Data Logs
        /// <summary>
        /// Created By Ravi vysa On 15-12-2022
        /// API >> Get >>api/taskimportexport/getfaultyimporttasklog?groupId
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getfaultyimporttasklog")]
        public async Task<IHttpActionResult> GetFultyImportLogs(Guid groupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var faultyReportLog = await _db.TaskImportFaultyLogs.
                                            Include("Group").
                                            Where(x => x.Group.FaultyLogsGroupId == groupId).
                                            ToListAsync();
                if (faultyReportLog.Count > 0)
                {
                    res.Message = "Faulty Task Log Reports";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = faultyReportLog;
                    return Ok(res);
                }
                else
                {
                    res.Message = "No Faulty Assets Imported";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = faultyReportLog;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/taskimportexport/getfaultyimporttasklog", ex.Message);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }

        }
        #endregion

        #region Api for Import Excel of Task
        /// <summary>
        /// UPDATED BY : Harshit Mitra On 01-02-2023 (Refactor Code)
        /// API>> POST >> api/taskimportexport/excelimporttask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("excelimporttask")]
        public async Task<IHttpActionResult> ImportTask(List<SheetUploadResponse> model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                long successfullImported = 0;
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                List<TaskImportFaultyLogs> falultyImportTask = new List<TaskImportFaultyLogs>();
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

                var projectData = await (from p in _db.ProjectLists
                                         join t in _db.TaskModels on p.ID equals t.ProjectId into q
                                         from empty in q.DefaultIfEmpty()
                                         where p.CompanyId == tokenData.companyId && p.IsActive && !p.IsDeleted
                                         select new
                                         {
                                             p.ID,
                                             p.ProjectName,
                                             p.ProjectManager,
                                             TaskId = empty != null ? empty.TaskId : Guid.Empty,
                                         })
                                         .Distinct()
                                         .ToListAsync();
                var employeeData = await (from e in _db.Employee
                                          join ap in _db.AssignProjects on e.EmployeeId equals ap.EmployeeId
                                          where e.CompanyId == tokenData.companyId && e.IsActive && !e.IsDeleted
                                                && e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee
                                          select new EmployeeCheckHelperClass
                                          {
                                              ProjectId = ap.ProjectId,
                                              EmployeeId = e.EmployeeId,
                                              OfficalEmail = e.OfficeEmail.ToUpper().Trim(),
                                              DisplayName = e.DisplayName.ToUpper().Trim(),

                                          }).ToListAsync();
                var projectHelperList = projectData
                    .Select(x => new TaskCountHelperClass
                    {
                        ProjectId = x.ID,
                        ProjectName = x.ProjectName.ToUpper().Trim(),
                        ProjectManagerId = x.ProjectManager,
                        TaskCount = projectData.Count(z => z.ID == x.ID),
                    })
                    .Distinct()
                    .ToList();

                model = model.OrderBy(x => x.ProjectName).ToList();
                List<TaskLog> taskLogList = new List<TaskLog>();
                List<TaskModel> taskModelList = new List<TaskModel>();
                foreach (var item in model)
                {
                    TaskModel obj = new TaskModel();
                    var timeCheck = item.EstimateTime.ToString().Contains(".") ?
                        item.EstimateTime.ToString().Split('.').Select(long.Parse).ToList() :
                        item.EstimateTime.ToString().Split(':')?.Select(long.Parse).ToList();

                    long totalEstimageMinutes = (timeCheck[0] * 60) + timeCheck[1];

                    #region Project Check Cases
                    if (String.IsNullOrEmpty(item.ProjectName))
                    {
                        item.FailReason = "Please Input Project Name.";
                        falultyImportTask.Add(CreateFaulty(item));
                        continue;
                    }
                    var checkProject = projectHelperList
                        .FirstOrDefault(x => x.ProjectName == item.ProjectName.Trim().ToUpper());
                    string[] array = checkProject.ProjectName.ToString().Split();
                    string firstAndLast = "";
                    if (array.Length == 1)
                    {
                        firstAndLast = array.First().Substring(0, 1);
                    }
                    else
                    {
                        firstAndLast = array.First().Substring(0, 1) + array.Last().Substring(0, 1);
                    }
                    if (checkProject == null)
                    {
                        item.FailReason = "Project Not Found.";
                        falultyImportTask.Add(CreateFaulty(item));
                        continue;
                    }
                    #endregion

                    #region Employee Check Cases
                    if (String.IsNullOrEmpty(item.EmployeeName))
                    {
                        item.FailReason = "Please Input Employee Name.";
                        falultyImportTask.Add(CreateFaulty(item));
                        continue;
                    }
                    var assigningEmployee = employeeData
                        .FirstOrDefault(x => x.ProjectId == checkProject.ProjectId && x.OfficalEmail == item.OfficalEmail.ToUpper().Trim());
                    if (assigningEmployee == null)
                    {
                        item.FailReason = " Assignee Employee Not Found.";
                        falultyImportTask.Add(CreateFaulty(item));
                        continue;
                    }
                    var checkAuthor = employeeData
                        .Where(x => x.ProjectId == checkProject.ProjectId && x.DisplayName == item.EmployeeName.ToUpper().Trim())
                        .Select(x => x.EmployeeId).FirstOrDefault();
                    if (checkAuthor == 0)
                        checkAuthor = checkProject.ProjectManagerId;
                    #endregion

                    #region Task Type Check Cases
                    if (String.IsNullOrEmpty(item.TaskType))
                    {
                        item.FailReason = "Please Input Proper Task Type";
                        falultyImportTask.Add(CreateFaulty(item));
                    }
                    var taskType = textInfo.ToTitleCase(item.TaskType.Trim());
                    switch (taskType)
                    {
                        case "Task": obj.TaskType = TaskTypeConstants.Task; break;
                        case "Issue": obj.TaskType = TaskTypeConstants.Issue; break;
                        case "Bug": obj.TaskType = TaskTypeConstants.Bug; break;
                        case "Leave": obj.TaskType = TaskTypeConstants.Leave; break;
                        case "Break": obj.TaskType = TaskTypeConstants.Break; break;
                        case "Deployment": obj.TaskType = TaskTypeConstants.Deployment; break;

                        default:
                            item.FailReason = "Wrong Task Type Inputed";
                            falultyImportTask.Add(CreateFaulty(item));
                            continue;
                    }
                    #endregion

                    #region Status Check Cases
                    if (String.IsNullOrEmpty(item.Status))
                    {
                        item.FailReason = "Please Input Proper Status";
                        falultyImportTask.Add(CreateFaulty(item));
                    }
                    var statusText = textInfo.ToTitleCase(item.Status.Trim());
                    switch (statusText)
                    {
                        case "Pending": obj.Status = TaskStatusConstants.Pending; break;

                        case "In_Progress": obj.Status = TaskStatusConstants.In_Progress; break;
                        case "In Progress": obj.Status = TaskStatusConstants.In_Progress; break;
                        case "InProgress": obj.Status = TaskStatusConstants.In_Progress; break;
                        case "In_progress": obj.Status = TaskStatusConstants.In_Progress; break;
                        case "In progress": obj.Status = TaskStatusConstants.In_Progress; break;
                        case "Inprogress": obj.Status = TaskStatusConstants.In_Progress; break;

                        case "Resolved": obj.Status = TaskStatusConstants.Resolved; break;

                        case "Re_Open": obj.Status = TaskStatusConstants.Re_Open; break;
                        case "Re Open": obj.Status = TaskStatusConstants.Re_Open; break;
                        case "ReOpen": obj.Status = TaskStatusConstants.Re_Open; break;
                        case "Re_open": obj.Status = TaskStatusConstants.Re_Open; break;
                        case "Re open": obj.Status = TaskStatusConstants.Re_Open; break;
                        case "Reopen": obj.Status = TaskStatusConstants.Re_Open; break;

                        case "Closed": obj.Status = TaskStatusConstants.Closed; break;
                        case "closed": obj.Status = TaskStatusConstants.Closed; break;
                        case "close": obj.Status = TaskStatusConstants.Closed; break;
                        case "Close": obj.Status = TaskStatusConstants.Closed; break;

                        default:
                            item.FailReason = "Wrong Task Status Type Inputed";
                            falultyImportTask.Add(CreateFaulty(item));
                            continue;
                    }
                    #endregion

                    #region Priority Check Cases
                    if (String.IsNullOrEmpty(item.Priority))
                    {
                        item.FailReason = "Please Input Priority";
                        falultyImportTask.Add(CreateFaulty(item));
                    }
                    var priortyText = textInfo.ToTitleCase(item.Priority.Trim());
                    switch (priortyText)
                    {
                        case "Urgent": obj.Priority = TaskPriorityConstants.Urgent; break;
                        case "Highest": obj.Priority = TaskPriorityConstants.High; break;

                        case "High": obj.Priority = TaskPriorityConstants.High; break;

                        case "Medium": obj.Priority = TaskPriorityConstants.Medium; break;

                        case "Low": obj.Priority = TaskPriorityConstants.Low; break;

                        default:
                            item.FailReason = "Wrong Task Priority Type Inputed";
                            falultyImportTask.Add(CreateFaulty(item));
                            continue;
                    }
                    #endregion

                    #region Billing Status Check Cases
                    if (String.IsNullOrEmpty(item.Billing))
                    {
                        item.FailReason = "Please Input Proper Billing Status";
                        falultyImportTask.Add(CreateFaulty(item));
                    }
                    var billingText = textInfo.ToTitleCase(item.Billing.Trim());
                    switch (billingText)
                    {
                        case "Non_Billable": obj.TaskBilling = TaskBillingConstants.Non_Billable; break;
                        case "NonBillable": obj.TaskBilling = TaskBillingConstants.Non_Billable; break;
                        case "Non-Billable": obj.TaskBilling = TaskBillingConstants.Non_Billable; break;

                        case "Billable": obj.TaskBilling = TaskBillingConstants.Billable; break;
                        case "billable": obj.TaskBilling = TaskBillingConstants.Billable; break;

                        default:
                            item.FailReason = "Wrong Task Billing Status Type Inputed";
                            falultyImportTask.Add(CreateFaulty(item));
                            continue;
                    }
                    #endregion

                    #region Sprint Check Cases
                    //if (String.IsNullOrEmpty(item.SprintName))
                    //{
                    //    item.FailReason = "Please Input SprintName .";
                    //    falultyImportTask.Add(CreateFaulty(item));
                    //    continue;
                    //}
                    var checkSprint = _db.Sprints
                        .FirstOrDefault(x => x.SprintName == item.SprintName.Trim().ToUpper() && x.ProjectId == checkProject.ProjectId);
                    //if(checkSprint == null)
                    //{
                    //    item.FailReason = "Sprint Not Found.";
                    //    falultyImportTask.Add(CreateFaulty(item));
                    //    continue;
                    //}

                    #endregion

                    var index = projectHelperList.FindIndex(x => x.ProjectId == checkProject.ProjectId);
                    var getData = projectHelperList.FirstOrDefault(x => x.ProjectId == checkProject.ProjectId);

                    obj.TaskTitle = item.TaskTitle;
                    obj.ProjectId = checkProject.ProjectId;
                    obj.Discription = item.Discription;
                    obj.TaskIdNumber = getData.TaskCount + 1;
                    obj.SprintId = checkSprint == null ? Guid.Empty : checkSprint.SprintId;
                    obj.AssignEmployeeId = assigningEmployee.EmployeeId;
                    obj.StartDate = TimeZoneConvert.ConvertTimeToSelectedZone(item.StartDate, tokenData.TimeZone);
                    obj.EndDate = TimeZoneConvert.ConvertTimeToSelectedZone(item.EndDate, tokenData.TimeZone);
                    obj.Percentage = item.Status == "Closed" ? 100 : 0;
                    obj.EstimateTime = totalEstimageMinutes;
                    obj.CompanyId = tokenData.companyId;
                    obj.CreatedBy = checkAuthor;
                    obj.CreatedOn = today;
                    obj.ProjectTaskId = firstAndLast + "" + obj.TaskIdNumber;
                    taskModelList.Add(obj);

                    TaskLog taskLog = new TaskLog();
                    taskLog.TaskId = obj.TaskId;
                    taskLog.ProjectId = checkProject.ProjectId;
                    taskLog.TaskIdNumber = item.TaskIdNumber;
                    taskLog.SpentTime = item.SpentTime;
                    taskLog.DueDate = TimeZoneConvert.ConvertTimeToSelectedZone(item.StartDate, tokenData.TimeZone);
                    taskLog.CompanyId = tokenData.companyId;
                    taskLog.CreatedBy = assigningEmployee.EmployeeId;
                    taskLog.CreatedOn = today;
                    taskLog.LogEmployeeId = assigningEmployee.EmployeeId;
                    taskLogList.Add(taskLog);

                    projectHelperList[index] = new TaskCountHelperClass
                    {
                        ProjectId = getData.ProjectId,
                        ProjectManagerId = getData.ProjectManagerId,
                        ProjectName = getData.ProjectName,
                        TaskCount = getData.TaskCount + 1,
                    };
                }
                successfullImported = taskModelList.Count;
                _db.TaskModels.AddRange(taskModelList);
                _db.TaskLogs.AddRange(taskLogList);
                await _db.SaveChangesAsync();

                if (falultyImportTask.Count > 0)
                {
                    #region Faulty Task Region
                    TaskImportFaultyLogsGroup groupObj = new TaskImportFaultyLogsGroup
                    {
                        TotalTaskImported = model.Count,
                        SuccessFullTaskImported = successfullImported,
                        UnSuccessFullTaskImported = falultyImportTask.Count,
                        CreatedBy = tokenData.employeeId,
                        CreatedOn = today,
                        CompanyId = tokenData.companyId,
                    };
                    _db.TaskImportFaultyLogsGroups.Add(groupObj);
                    await _db.SaveChangesAsync();

                    falultyImportTask.ForEach(x =>
                    {
                        x.TaskFaultyLogId = Guid.NewGuid();
                        x.Group = groupObj;

                    });
                    _db.TaskImportFaultyLogs.AddRange(falultyImportTask);
                    await _db.SaveChangesAsync();
                    #endregion

                    if ((model.Count - falultyImportTask.Count) > 0)
                    {
                        res.Message = "Task Imported Succesfull Of " +
                        (model.Count - falultyImportTask.Count) + " Fields And " +
                        falultyImportTask.Count + " Feilds Are Not Imported";
                        res.Status = true;
                        res.Data = falultyImportTask;
                    }
                    else
                    {
                        res.Message = "All Fields Are Not Imported";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.NotFound;
                        res.Data = falultyImportTask;
                    }
                    return Ok(res);
                }

                res.Message = "Data Added Successfully Of All Fields";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = falultyImportTask;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("api/taskcreation/excelimporttask", ex.Message, model);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }
        }
        public TaskImportFaultyLogs CreateFaulty(SheetUploadResponse model)
        {
            return new TaskImportFaultyLogs
            {
                ProjectName = model.ProjectName,
                TaskTitle = model.TaskTitle,
                TaskType = model.TaskType,
                Priority = model.Priority,
                Discription = model.Discription,
                EmployeeName = model.EmployeeName,
                TaskIdNumber = model.TaskIdNumber,
                Status = model.Status,
                Attechment = model.Attechment,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                SpentTime = model.SpentTime,
                IsApproved = model.IsApproved,
                FailReason = model.FailReason,
                OfficalEmail = model.OfficalEmail,
                BillingStatus = model.Billing
            };
        }
        public class SheetUploadResponse
        {
            public Guid TaskFaultyLogId { get; set; } = Guid.NewGuid();
            public virtual TaskImportFaultyLogsGroup Group { get; set; }
            public string ProjectName { get; set; } = String.Empty;
            public string TaskTitle { get; set; } = String.Empty;
            public string Discription { get; set; } = String.Empty;
            public string Priority { get; set; } = String.Empty;
            public string TaskType { get; set; } = String.Empty;
            public string Status { get; set; } = String.Empty;
            public int TaskIdNumber { get; set; } = 0;
            public string EmployeeName { get; set; } = String.Empty;
            public string Attechment { get; set; } = String.Empty;
            public string EstimateTime { get; set; }
            public TimeSpan SpentTime { get; set; }
            public DateTime StartDate { get; set; }
            public bool IsApproved { get; set; }
            public DateTime EndDate { get; set; }
            public string FailReason { get; set; } = String.Empty;
            public string OfficalEmail { get; set; } = String.Empty;
            public string Billing { get; set; } = String.Empty;
            public string SprintName { get; set; } = String.Empty;
        }
        public class TaskCountHelperClass
        {
            public int ProjectId { get; set; }
            public string ProjectName { get; set; }
            public int ProjectManagerId { get; set; }
            public int TaskCount { get; set; }
        }
        public class EmployeeCheckHelperClass
        {
            public int ProjectId { get; set; }
            public int EmployeeId { get; set; }
            public string DisplayName { get; set; }
            public string OfficalEmail { get; set; }
        }
        #endregion

        #region Api for get data for reports
        /// <summary>
        /// API>>GET>>api/taskimportexport/taskreport?ProjectId
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("taskreport")]
        public async Task<IHttpActionResult> GetReportData(int ProjectId)
        {

            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ReportResponseModel response = new ReportResponseModel();
            try
            {
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                var getData = await (from t in _db.TaskModels
                                     join e in _db.Employee on t.AssignEmployeeId equals e.EmployeeId
                                     join tl in _db.TaskLogs on t.TaskId equals tl.TaskId into r
                                     from result in r.DefaultIfEmpty()
                                     join c in _db.Employee on t.CreatedBy equals c.EmployeeId
                                     where t.CompanyId == tokenData.companyId && t.ProjectId == ProjectId
                                     && t.IsActive && !t.IsDeleted
                                     select new
                                     {
                                         t.AssignEmployeeId,
                                         EmployeeName = e.DisplayName,
                                         Email = e.OfficeEmail,
                                         t.TaskIdNumber,
                                         t.EstimateTime,
                                         t.StartDate,
                                         t.EndDate,
                                         t.TaskTitle,
                                         SpendTime = result == null ? TimeSpan.Zero : result.SpentTime,
                                         CreatedByName = c.DisplayName,
                                     })
                                     .Where(x => x.SpendTime == TimeSpan.Zero)
                                     .ToListAsync();
                var loopMail = getData
                    .Select(x => new MailResponseBodyModel
                    {
                        EmployeeId = x.AssignEmployeeId.Value,
                        Email = x.Email,
                        DisplayName = x.EmployeeName,
                        TaskIdNumber = x.TaskIdNumber,
                        TaskTitle = x.TaskTitle,
                        StartDate = x.StartDate.Value,
                        EndDate = x.EndDate.Value,
                        EstimatedTime = x.EstimateTime,
                        CreatedByName = x.CreatedByName

                    }).ToList();

                var listData = CreatePdf2(loopMail, tokenData.companyId);

                var taskData = _db.TaskModels.Where(x => x.IsActive && !x.IsDeleted && x.ProjectId == ProjectId).AsEnumerable();
                var TaskNotAssign = await (from p in _db.ProjectLists
                                           join ap in _db.AssignProjects on p.ID equals ap.ProjectId
                                           join e in _db.Employee on ap.EmployeeId equals e.EmployeeId
                                           join t in taskData on ap.EmployeeId equals t.AssignEmployeeId into r
                                           from empty in r.DefaultIfEmpty()
                                           where p.CompanyId == tokenData.companyId && p.IsActive && !p.IsDeleted
                                                && ap.IsActive && !ap.IsDeleted && empty == null && p.ID == ProjectId
                                           select new NotAssigendMailResponse
                                           {
                                               ProjectId = ap.ProjectId,
                                               ProjectName = p.ProjectName,
                                               EmployeId = empty == null ? ap.EmployeeId : empty.AssignEmployeeId.Value,
                                               EmployeeName = e.DisplayName,
                                               OfficalEmail = e.OfficeEmail,
                                               CompanyId = e.CompanyId,
                                           })
                                            .Distinct()
                                            .ToListAsync();

                response.NotFilled = getData;
                response.TaskNotAssigend = TaskNotAssign;

                HostingEnvironment.QueueBackgroundWorkItem(ct => SendDedLineMailInTimeSheet(listData, ProjectId, today, TaskNotAssign));

                if (response.NotFilled != null)
                {
                    res.Message = "Succesfully Send  !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = response;


                    return Ok(res);
                }
                res.Message = "Data Not Found !";
                res.Status = false;
                res.StatusCode = HttpStatusCode.NotFound;
                res.Data = getData;
                return Ok(res);
            }
            catch (Exception ex)
            {

                logger.Error("api/taskimportexport/taskreport", ex.Message);
                return BadRequest("Failed");
            }
        }

        public class MailResponseBodyModel
        {
            public int EmployeeId { get; set; }
            public string Email { get; set; }
            public string DisplayName { get; set; }
            public string TaskTitle { get; set; }
            public int TaskIdNumber { get; set; }
            public DateTimeOffset StartDate { get; set; }
            public DateTimeOffset EndDate { get; set; }
            public long EstimatedTime { get; set; }
            public string CreatedByName { get; set; }
        }
        public class NotAssigendMailResponse
        {
            public int ProjectId { get; set; }
            public int EmployeId { get; set; }
            public string ProjectName { get; set; }
            public string EmployeeName { get; set; }
            public string OfficalEmail { get; set; }
            public int CompanyId { get; set; } = 0;

        }
        public class ReportResponseModel
        {
            public object NotFilled { get; set; }
            public object TaskNotAssigend { get; set; }
        }

        #endregion

        #region Function For Mail
        public async Task SendDedLineMailInTimeSheet(List<MailBodyResponse> model, int projectId, DateTimeOffset today, List<NotAssigendMailResponse> notAssign)
        {
            await UpdateProjectLastDate(projectId, today);
            if (model.Count != 0)
                foreach (var item in model)
                {
                    await Task.Delay(100); // 1000 = 1 sec;
                    _ = SendDedlineTaskMail(item);
                }
            if (notAssign.Count != 0)
                foreach (var item in notAssign)
                {
                    await Task.Delay(100); // 1000 = 1 sec;
                    _ = SendTaskNotAssignMail(item);
                }
        }

        public async Task UpdateProjectLastDate(int projectId, DateTimeOffset today)
        {
            var project = await _db.ProjectLists
                .FirstOrDefaultAsync(x => x.ID == projectId);
            project.LastMailSendDate = today.Date;
            _db.Entry(project).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
        #endregion

        #region This Api Use To Send Task Mail For Assigend Employee
        ///// <summary>
        ///// Create By Ravi vyas Date-12-12-2022
        ///// </summary>
        ///// <returns></returns>
        public async Task<bool> SendDedlineTaskMail(MailBodyResponse assignEmployeeTaskData)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var companylist = _db.Company.Where(y => y.CompanyId == assignEmployeeTaskData.CompanyId
                     && y.IsActive && !y.IsDeleted)
                   .Select(x => new
                   {
                       x.RegisterAddress,
                       x.RegisterCompanyName

                   }).FirstOrDefault();

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
                string htmlBody = TaskHelper.NewSendDeadLineMailInTimeSheet
                    .Replace("<|EMPLOYEENAME|>", assignEmployeeTaskData.DisplayName)
                    .Replace("<|IMAGE_PATH|>", "emossy.png")
                    .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                    .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                SendMailModelRequest sendMailObject = new SendMailModelRequest()
                {
                    IsCompanyHaveDefaultMail = tokenData.IsSmtpProvided,
                    Subject = "Reminder Mail",
                    MailBody = htmlBody,
                    MailTo = new List<string>() { assignEmployeeTaskData.Email },
                    SmtpSettings = smtpsettings,
                };
                await SmtpMailHelper.SendMailAsync(sendMailObject);
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
            return true;
        }

        #endregion

        #region This Api Use To Send Task Not Assigen Mail For Assigend Employee
        ///// <summary>
        ///// Create By Ravi vyas Date-12-12-2022
        ///// </summary>
        ///// <returns></returns>
        public async Task<bool> SendTaskNotAssignMail(NotAssigendMailResponse notAssigendMailResponse)
        {
            try
            {
                var companylist = _db.Company.Where(y => y.CompanyId == notAssigendMailResponse.CompanyId
                    && y.IsActive && !y.IsDeleted)
                  .Select(x => new
                  {
                      x.RegisterAddress,
                      x.RegisterCompanyName

                  }).FirstOrDefault();
                SmtpMail oMail = new SmtpMail("TryIt");
                var attachmentPath = Path.Combine(HttpRuntime.AppDomainAppPath, "uploadimage\\MailImages");
                string htmlBody = TaskHelper.InProjectNotCreatingTask
                    .Replace("<|EMPLOYEENAME|>", notAssigendMailResponse.EmployeeName)
                    .Replace("<|PROJECTNAME|>", notAssigendMailResponse.ProjectName)
                    .Replace("<|IMAGE_PATH|>", "emossy.png")
                    .Replace("<|COMPANYNAMEE|>", companylist.RegisterCompanyName)
                    .Replace("<|COMPANYADDRESS|>", companylist.RegisterAddress);
                oMail.ImportHtml(htmlBody, attachmentPath, ImportHtmlBodyOptions.ImportLocalPictures | ImportHtmlBodyOptions.ImportCss); ;

                SmtpServer oServer = new SmtpServer("smtp.office365.com");

                oMail.From = ConfigurationManager.AppSettings["MasterEmail"]; ;
                oMail.To = notAssigendMailResponse.OfficalEmail;
                oMail.Subject = "New Timesheet Reminder";
                oServer.User = ConfigurationManager.AppSettings["MailUser"];
                oServer.Password = ConfigurationManager.AppSettings["MailPassword"];
                oServer.Port = 587;
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

        #endregion

        #region This Api Use To Send A Pdf In Mail

        ///// <summary>
        ///// Created By Ankit Date-08-08-2022
        ///// Api >> Get >> api/preboard/sendcredential
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        /////
        //[Route("sendcredential")]
        //[HttpGet]
        //public Candidate SendPdf(Candidate candidate)
        //{
        //    Candidate res = new Candidate();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    // The variable 'ex' is declared but never used
        //    try
        //    {
        //        SendPdfDto2 obj;
        //        var data = _db.CredentilDatas.Where(x => x.CandidateId == candidate.CandidateId).FirstOrDefault();
        //        List<CredentilData> pod = _db.CredentilDatas.Where(a => a.CandidateId == candidate.CandidateId).ToList();
        //        obj = new SendPdfDto2()
        //        {
        //            P = data,
        //            Credential = pod,
        //        };
        //        CreatePdf2(obj, candidate);
        //        SendMail(candidate);
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //    return res;
        //}

        //public class SendPdfDto3{}   public List<CredentilData> Credential { get; set; }

        #endregion This Api Use To Send A Pdf In Mail

        #region This Api Use Create Task Pdf 
        /// <summary>
        /// Create By Ankit Date - 01/09/2022
        /// </summary>
        /// <param name="model"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public List<MailBodyResponse> CreatePdf2(List<MailResponseBodyModel> model, int companyId)
        {
            List<MailBodyResponse> list = new List<MailBodyResponse>();
            try
            {
                var checkEmployees = model.Select(x => x.EmployeeId).Distinct().ToList();
                foreach (var item in checkEmployees)
                {
                    var employeeDetails = model
                        .Where(x => x.EmployeeId == item)
                        .Select(x => new
                        {
                            x.EmployeeId,
                            x.DisplayName,
                            x.Email,
                        })
                        .FirstOrDefault();

                    #region PDF SETTINGS 

                    Document document = new Document(PageSize.A4);
                    string DirectoryURL = Path.Combine(HttpRuntime.AppDomainAppPath, "uploadimage\\TaskPDFs\\CompanyId" + companyId);
                    DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                    if (!objDirectory.Exists)
                    {
                        Directory.CreateDirectory(DirectoryURL);
                    }
                    var path = Path.Combine(HttpRuntime.AppDomainAppPath, "uploadimage\\TaskPDFs\\CompanyId" + companyId + "\\"
                            + employeeDetails.DisplayName.Replace(" ", "") + ".pdf");
                    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(path, FileMode.Create));
                    Pharsecell p = new Pharsecell();
                    PdfPTable table = null;
                    #endregion

                    #region DOCUMENT OPEN|CLOSE

                    document.Open();

                    #region DOCUMENT HEADING

                    Font headFont = new Font(Font.FontFamily.COURIER, 26f);
                    headFont.SetColor(241, 89, 34);
                    headFont.SetFamily("Courier");
                    headFont.SetStyle("bold");
                    headFont.SetStyle("underline");
                    Paragraph head = new Paragraph("Emossy TimeSheet Reminder", headFont);
                    head.Alignment = Element.ALIGN_CENTER;
                    head.SpacingAfter = 20f;
                    document.Add(head);

                    #endregion

                    #region DOCUMENT CONTENT

                    // SUB HEAD //
                    Phrase ph1 = new Phrase(new Chunk(Environment.NewLine));
                    ph1.Add(new Chunk("Dear " + employeeDetails.DisplayName + ", It Looks Like You Missed The Time Sheet.", FontFactory.GetFont("Arial", 16, 1)));
                    ph1.Add(new Chunk(Environment.NewLine));
                    Paragraph subhead = new Paragraph(ph1);
                    subhead.SpacingAfter = 1;
                    document.Add(subhead);

                    // CONTENT //
                    Phrase ph2 = new Phrase(new Chunk(Environment.NewLine));
                    ph2.Add(new Chunk("It's look like to see you forget to update you task in timesheet. " +
                        "Please check below table to get details of your pending task.", FontFactory.GetFont("Arial", 10, 1)));
                    ph2.Add(new Chunk(Environment.NewLine));
                    Paragraph para = new Paragraph(ph2);
                    para.SpacingAfter = 8;
                    document.Add(para);

                    #endregion

                    //Chunk glue = new Chunk(new VerticalPositionMark());
                    //Paragraph para = new Paragraph();
                    //table = new PdfPTable(1);
                    //table.TotalWidth = 380f;
                    //table.LockedWidth = true;
                    //table.SpacingBefore = 20f;
                    //table.HorizontalAlignment = Element.ALIGN_CENTER;
                    //Phrase Head = new Phrase();
                    //Paragraph head1 = new Paragraph();
                    //Head.Add(new Chunk(Environment.NewLine));
                    //Head.Add(new Chunk("Your Login Credential", FontFactory.GetFont("Arial", 20, 1)));
                    //head1.Alignment = Element.ALIGN_CENTER;
                    ////Head.Add(new Chunk(Environment.NewLine));
                    //head1.Add(Head);

                    //Phrase ph1 = new Phrase();
                    //Paragraph mm = new Paragraph();
                    //ph1.Add(new Chunk(Environment.NewLine));
                    //ph1.Add(new Chunk("Dear " + employeeDetails.DisplayName + "", FontFactory.GetFont("Arial", 10, 1)));
                    //ph1.Add(glue);
                    ////ph1.Add(new Chunk(Environment.NewLine));
                    ////ph1.Add(new Chunk("Welcome to the team " + item.DisplayName + "! ", FontFactory.GetFont("Arial", 10, 1)));
                    ////ph1.Add(glue);
                    //ph1.Add(new Chunk(Environment.NewLine));
                    //ph1.Add(new Chunk("We are glad to receive your acceptance. We are so excited about having you in our team! With your experience, you will be a great addition. We hope you bring a lot of positive energy with you and we can work well together and share many successes.Your documentation process is completed.Please find your login credentials below " + "", FontFactory.GetFont("Arial", 10, 1)));
                    //ph1.Add(glue);
                    ////ph1.Add(new Chunk(Environment.NewLine));
                    ////ph1.Add(new Chunk("Best Regards", FontFactory.GetFont("Arial", 10, 1)));
                    ////ph1.Add(glue);
                    ////ph1.Add(new Chunk(Environment.NewLine));
                    ////ph1.Add(new Chunk("HR Executive" + item.DisplayName + " ", FontFactory.GetFont("Arial", 10, 1)));
                    ////ph1.Add(glue);
                    //ph1.Add(new Chunk(" ", FontFactory.GetFont("Arial", 10, 1)));
                    //mm.Add(ph1);
                    //para.Add(mm);
                    //Phrase ph5 = new Phrase();
                    //Paragraph mmc = new Paragraph();
                    //ph5.Add(new Chunk(" ", FontFactory.GetFont("Arial", 10, 1)));
                    //mmc.Add(ph5);
                    //para.Add(mmc);

                    //document.Add(para);


                    table = new PdfPTable(7);
                    table.WidthPercentage = 100;
                    PdfPCell tableHead = new PdfPCell(new Phrase("Task Details", new Font(Font.FontFamily.TIMES_ROMAN, 16)));
                    tableHead.Colspan = 7;
                    tableHead.HorizontalAlignment = 1;
                    table.AddCell(tableHead);


                    table.AddCell("Task Id");
                    PdfPCell cell1 = new PdfPCell(new Phrase("Task Title"));
                    cell1.Colspan = 2;
                    table.AddCell(cell1);
                    table.AddCell("Created By");
                    table.AddCell("Start Date");
                    table.AddCell("End Date");
                    table.AddCell("Estimate Time");

                    var totalTask = model
                        .Where(x => x.EmployeeId == item)
                        .Select(x => new
                        {
                            x.TaskIdNumber,
                            x.TaskTitle,
                            x.StartDate,
                            x.EndDate,
                            x.EstimatedTime,
                            x.CreatedByName,
                        })
                        .Distinct()
                        .ToList();

                    Font tableBodyFont = new Font(Font.FontFamily.TIMES_ROMAN, 12);
                    foreach (var task in totalTask)
                    {
                        table.AddCell(new PdfPCell(new Phrase(task.TaskIdNumber.ToString(), tableBodyFont)));
                        PdfPCell cellData = new PdfPCell(new Phrase(task.TaskTitle, tableBodyFont));
                        cellData.Colspan = 2;
                        table.AddCell(cellData);
                        table.AddCell(new PdfPCell(new Phrase(task.CreatedByName, tableBodyFont)));
                        table.AddCell(new PdfPCell(new Phrase(task.StartDate.ToString("dd/MM/yyyy"), tableBodyFont)));
                        table.AddCell(new PdfPCell(new Phrase(task.EndDate.ToString("dd/MM/yyyy"), tableBodyFont)));
                        var timeSet = string.Format("{00:00}:{01:00}", task.EstimatedTime / 60, task.EstimatedTime % 60);
                        table.AddCell(new PdfPCell(new Phrase(timeSet, tableBodyFont)));
                    }
                    document.Add(table);
                    //Paragraph para1 = new Paragraph();
                    //Phrase ph2 = new Phrase();
                    //Paragraph mm1 = new Paragraph();
                    ////ph2.Add(new Chunk("Total Amount:" + obj.p.ETotalAmount, FontFactory.GetFont("Arial", 10, 1)));
                    //mm1.Add(ph2);
                    //mm1.Alignment = Element.ALIGN_RIGHT;
                    //para1.Add(mm1);
                    //document.Add(para1);

                    //Paragraph para3 = new Paragraph();
                    //Phrase ph3 = new Phrase();
                    //Paragraph mm3 = new Paragraph();
                    ////ph3.Add(new Chunk("Best Regards", FontFactory.GetFont("Arial", 10, 1)));
                    //ph3.Add(glue);
                    //ph3.Add(new Chunk(Environment.NewLine));
                    ////ph3.Add(new Chunk("HR Executive" + data.CompanyName + " ", FontFactory.GetFont("Arial", 10, 1)));
                    //ph3.Add(glue);
                    //mm3.Alignment = Element.ALIGN_RIGHT;
                    //mm3.Add(ph3);
                    //para3.Add(mm3);
                    //document.Add(para3);
                    list.Add(new MailBodyResponse
                    {
                        Email = employeeDetails.Email,
                        DisplayName = employeeDetails.DisplayName,
                        PDFUrl = path,
                        CompanyId = companyId,
                    });
                    document.Close();
                    #endregion

                }
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public class MailBodyResponse
        {
            public string Email { get; set; } = String.Empty;
            public string DisplayName { get; set; } = String.Empty;
            public string PDFUrl { get; set; } = String.Empty;
            public int CompanyId { get; set; } = 0;

        }
        public class Pharsecell
        {
            public PdfPCell Pc(Phrase phrase, int align)
            {
                PdfPCell cell = new PdfPCell(phrase);
                //cell.BorderColor = System.Drawing.Color.WHITE;
                //cell.VerticalAlignment = PdfCell.ALIGN_TOP;
                cell.HorizontalAlignment = align;
                cell.PaddingBottom = 2f;
                cell.PaddingTop = 0f;
                return cell;
            }
        }

        #endregion This Api Use Create Pdf Api in Credential

        #region Api for Get All Projeect Report By ProjectId
        /// <summary>
        /// API>>api/taskimportexport/projectreportbyprojectid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("projectreportbyprojectid")]
        public async Task<IHttpActionResult> GetAllCompanyEmployeeTask(int projectId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ProjectReportResponse response = new ProjectReportResponse();
            try
            {
                var infoData = await (from t in _db.TaskModels
                                      join e in _db.Employee on t.AssignEmployeeId equals e.EmployeeId
                                      join p in _db.ProjectLists on t.ProjectId equals p.ID
                                      join tl in _db.TaskLogs on t.TaskId equals tl.TaskId into q
                                      from emptyLog in q.DefaultIfEmpty()
                                      where t.IsActive && !t.IsDeleted && t.CompanyId == tokenData.companyId && t.ProjectId == projectId
                                      select new
                                      {
                                          ProjectId = p.ID,
                                          ProjecName = p.ProjectName,
                                          EmployeeName = e.DisplayName,
                                          OrignalEstimateHours = t.EstimateTime,
                                          EmployeeId = (int)t.AssignEmployeeId,
                                          SpentTime = emptyLog.SpentTime == null ? TimeSpan.Zero : emptyLog.SpentTime,
                                      })
                                      .ToListAsync();
                var employeeIds = infoData
                    .Select(x => new
                    {
                        x.EmployeeId,
                        x.EmployeeName,
                        x.ProjectId,
                        x.ProjecName,
                    })
                    .Distinct()
                    .ToList();
                var getData = employeeIds
                    .Select(x => new
                    {
                        x.EmployeeName,
                        x.ProjecName,
                        TotalTaskCount = infoData.Count(z => z.EmployeeId == x.EmployeeId),
                        EstimateTime = infoData.Where(z => z.EmployeeId == x.EmployeeId).Select(z => z.OrignalEstimateHours).Sum(),
                        SpendTime = (long)infoData.Where(z => z.EmployeeId == x.EmployeeId).Select(z => z.SpentTime.TotalMinutes).Sum(),
                    })
                    .Select(x => new
                    {
                        x.EmployeeName,
                        x.ProjecName,
                        x.TotalTaskCount,
                        EstimateTime = string.Format("{00:00}:{01:00}", x.EstimateTime / 60, x.EstimateTime % 60),
                        SpendTime = string.Format("{00:00}:{01:00}", x.SpendTime / 60, x.SpendTime % 60),
                    })
                    .ToList();
                var checkEmployeeId = employeeIds.Select(x => x.EmployeeId).ToList();
                var taskNotAssignToEmployee = await (from ap in _db.AssignProjects
                                                     join e in _db.Employee on ap.EmployeeId equals e.EmployeeId
                                                     where ap.ProjectId == projectId && !checkEmployeeId.Contains(e.EmployeeId)
                                                     select new
                                                     {
                                                         EmployeeName = e.DisplayName,
                                                         EmployeeEmail = e.OfficeEmail,

                                                     })
                                                     .Distinct()
                                                     .ToListAsync();

                response.ProjectListData = getData;
                response.ProjectNotAssigendTaskList = taskNotAssignToEmployee;

                if (response.ProjectListData != null)
                {

                    res.Message = "Data Get Succesfuly !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = response;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = response;
                    return Ok(res);
                }

            }

            catch (Exception ex)
            {


                logger.Error("API : api/taskimportexport/projectreportbyprojectid | " +
                    "Week date : " + projectId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        public class TimeResponse
        {
            public double TimeData { get; set; }
            public Guid TaskId { get; set; }
        }
        public class ProjectReportResponse
        {
            public object ProjectListData { get; set; }
            public object ProjectNotAssigendTaskList { get; set; }
        }
        #endregion

    }
}

