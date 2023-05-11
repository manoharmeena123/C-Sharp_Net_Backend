using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.TimeSheet;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.TimeSheet.Timesheet_New
{
    /// <summary>
    /// Created By Ravi Vyas On 16/02/2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/newtimesheet")]
    public class NewTaskCreationController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Api for add log time
        /// <summary>
        /// API>>POST>>api/newtimesheet/submittasklogtime
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("submittasklogtime")]
        public async Task<IHttpActionResult> SubmitLogTimeOfTask(NewTimesheetLogTime model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {

                var checTaskLog = await _db.TaskLogs.
                                        FirstOrDefaultAsync(x => x.TaskId == model.TaskId &&
                                        x.DueDate == model.LogDate && x.CompanyId == tokenData.companyId);

                var timeArray = model.SpentTime?.Split(':')?.Select(Int32.Parse)?.ToList();
                if (checTaskLog == null)
                {
                    TaskLog taskObj = new TaskLog
                    {
                        TaskId = model.TaskId,
                        ProjectId = model.ProjectId,
                        CreatedBy = tokenData.employeeId,
                        CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                    };
                    _db.TaskLogs.Add(taskObj);
                    await _db.SaveChangesAsync();
                    checTaskLog = taskObj;
                }
                else
                {
                    checTaskLog.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                    checTaskLog.UpdatedBy = tokenData.employeeId;
                }
                checTaskLog.LogEmployeeId = tokenData.employeeId;
                checTaskLog.DueDate = model.LogDate;
                checTaskLog.SpentTime = new TimeSpan(timeArray[0], timeArray[1], 00);
                checTaskLog.CompanyId = tokenData.companyId;
                _db.Entry(checTaskLog).State = EntityState.Modified;
                await _db.SaveChangesAsync();


                var checkApprovel = _db.TaskApprovels
                                       .Where(a => a.TaskId == model.TaskId)
                                       .FirstOrDefault();

                if (checkApprovel == null)
                {
                    TaskApprovel obj = new TaskApprovel()
                    {
                        TaskId = model.TaskId,
                        ProjectId = model.ProjectId,
                        CreatedBy = tokenData.employeeId,
                        CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow),
                        CompanyId = tokenData.companyId,
                    };
                    _db.TaskApprovels.Add(obj);
                    await _db.SaveChangesAsync();
                    checkApprovel = obj;
                }
                else
                {
                    checkApprovel.UpdatedBy = tokenData.employeeId;
                    checkApprovel.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                }
                checkApprovel.SpentTime = checTaskLog.SpentTime;
                checkApprovel.ProjectName = model.ProjectName;
                checkApprovel.TaskName = model.TaskTItle;
                checkApprovel.EstimateTime = model.EstimatedTime;
                checkApprovel.TaskRequest = TaskRequestConstants.Pending;
                checkApprovel.StartDate = model.LogDate;
                checkApprovel.CompanyId = tokenData.companyId;
                checkApprovel.OrgId = tokenData.orgId;
                checkApprovel.IsRe_Evaluate = false;
                checkApprovel.IsSFA = true;
                _db.Entry(checkApprovel).State = System.Data.Entity.EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "Log Added Succesfully !";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = checTaskLog;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/newtimesheet/submittasklogtime | " +
                                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        public class NewTimesheetLogTime
        {
            public Guid TaskLogId { get; set; }
            public Guid TaskId { get; set; } = Guid.Empty;
            public int ProjectId { get; set; } = 0;
            public DateTimeOffset LogDate { get; set; }
            public string SpentTime { get; set; }
            public string TaskTItle { get; set; }
            public string ProjectName { get; set; }
            public long EstimatedTime { get; set; }
        }


        #endregion

        #region Api for get Task By Date 
        /// <summary>
        /// API>>GET>>api/newtimesheet/gettaskbytaskdate
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("gettaskbytaskdate")]
        public async Task<IHttpActionResult> GetTaskByDate(int projectId, DateTime? date = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<HelpForDayDateForNew> dateList = new List<HelpForDayDateForNew>();
            DayDateDataResponseModelClass2 response = new DayDateDataResponseModelClass2();
            DateTimeOffset dateCheck = new DateTimeOffset();
            try
            {
                if (date.HasValue)
                    dateCheck = TimeZoneConvert.ConvertTimeToSelectedZone(date.Value, tokenData.TimeZone);
                DayOfWeek weekStart = DayOfWeek.Monday; // or Sunday, or whenever
                DateTimeOffset startingDate = dateCheck;

                while (startingDate.DayOfWeek != weekStart)
                    startingDate = startingDate.AddDays(-1);

                DateTimeOffset previousWeekStart = startingDate.AddDays(-7);
                DateTimeOffset previousWeekEnd = startingDate.AddDays(-1);

                #region This Code for Use current week date 

                DayOfWeek day = startingDate.DayOfWeek;
                int days = day - DayOfWeek.Monday;
                DateTimeOffset start = startingDate.AddDays(-days);
                DateTimeOffset end = start.AddDays(6);

                #endregion

                #region This Code use for Header

                DateTimeOffset startOfWeek = startingDate.AddDays((int)FirstDayOfWeek.Monday - (int)dateCheck.DayOfWeek);
                string[] separators = { "::" };
                var result =
                    string
                    .Join("|", Enumerable
                    .Range(0, 7)
                    .Select(i =>
                        startingDate.AddDays(i)
                    ))
                    .Split('|')
                    .Select(x => new HelpForDayDateForNew
                    {
                        Day = (DateTimeOffset.Parse(x)).ToString("dddd"),
                        Date = (DateTimeOffset.Parse(x)),
                        IsCurrentDate = (DateTimeOffset.Parse(x).Date == dateCheck.Date),
                        TotalMinutes = 0,
                        TotalWorkingTimeInWeek = String.Empty,
                        IsHide = (DateTimeOffset.Parse(x).Date > dateCheck.Date),
                    })
                    .ToList();
                dateList.AddRange(result);
                response.helpForDayDates = result;

                #endregion

                var getData = await (from t in _db.TaskModels
                                     join p in _db.ProjectLists on t.ProjectId equals p.ID
                                     where t.IsActive && !t.IsDeleted &&
                                     t.TaskType != TaskTypeConstants.BackLog && DbFunctions.TruncateTime(t.StartDate) == dateCheck.Date &&
                                     t.CompanyId == tokenData.companyId &&
                                     t.ProjectId == projectId && t.AssignEmployeeId == tokenData.employeeId
                                     select new GetTaskByDateResponse
                                     {
                                         TaskId = t.TaskId,
                                         TaskName = t.TaskTitle,
                                         StartDate = t.StartDate.Value,
                                         EndDate = t.EndDate.Value,
                                         Discription = t.Discription,
                                         ProjectName = p.ProjectName,
                                         SpentTime1 = "",
                                         SpentTime = _db.TaskLogs.Where(x => x.TaskId == t.TaskId).Select(t => t.SpentTime).FirstOrDefault(),
                                         EstimatedTime = t.EstimateTime,
                                         EstimatedTime1 = "",
                                         IsApproved = _db.TaskApprovels.Where(x => x.TaskId == t.TaskId).Select(x => x.IsApproved).FirstOrDefault(),
                                         IsSFA = _db.TaskApprovels.Where(x => x.TaskId == t.TaskId).Select(x => x.IsSFA).FirstOrDefault(),

                                     }).ToListAsync();

                double totalSpentTime = getData.Select(t => t.SpentTime.TotalMinutes).Sum();
                response.TotalWorkingTimeInWeek = string.Format("{00:00}:{01:00}", (int)totalSpentTime / 60, totalSpentTime % 60);
                response.Root = getData;

                if (getData.Count > 0)
                {
                    getData.ForEach(x =>
                    {
                        x.SpentTime1 = string.Format("{00:00}:{01:00}", (int)x.SpentTime.TotalMinutes / 60, x.SpentTime.TotalMinutes % 60);
                        x.EstimatedTime1 = string.Format("{00:00}:{01:00}", (int)x.EstimatedTime / 60, x.EstimatedTime % 60);
                    });
                    res.Message = "Succesfully Get Data !";
                    res.StatusCode = HttpStatusCode.OK;
                    res.Status = true;
                    res.Data = response;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data not found !";
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Status = false;
                    res.Data = response;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/newtimesheet/gettaskbytaskdate | " +
                   "Project Id : " + projectId + " | " +
                   "Date : " + date + " | " +
                   "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }



        public class GetTaskByDateResponse
        {
            public Guid TaskId { get; set; }
            public string TaskName { get; set; }
            public DateTimeOffset StartDate { get; set; }
            public DateTimeOffset EndDate { get; set; }
            public string Discription { get; set; }
            public string ProjectName { get; set; }
            public string SpentTime1 { get; set; }
            public TimeSpan SpentTime { get; set; }
            public double EstimatedTime { get; set; }
            public string EstimatedTime1 { get; set; }
            public bool IsSFA { get; set; }
            public bool IsApproved { get; set; }
        }
        public class HelpForDayDateForNew
        {
            public string Day { get; set; }
            public DateTimeOffset Date { get; set; }
            public double TotalMinutes { get; set; }
            public string TotalWorkingTimeInWeek { get; set; }
            public bool IsCurrentDate { get; set; }
            public bool IsHide { get; set; }

        }
        public class DayDateDataResponseModelClass2
        {
            public string TotalWorkingTimeInWeek { get; set; }
            public object helpForDayDates { get; set; }
            public List<GetTaskByDateResponse> Root { get; set; }

            //public string TotalWorkingEstTime { get; set; }
        }
        #endregion

        #region Api for Create task 
        /// <summary>
        /// API>>POST>>api/newtimesheet/createprojecttask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createprojecttask")]
        public async Task<IHttpActionResult> CreateTaskAndTaskLog(CreateTaskAndLog model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkData = await _db.TaskModels
                                   .Where(x => x.IsActive && !x.IsDeleted && x.TaskId == model.TaskId &&
                                   x.CompanyId == tokenData.companyId)
                                   .FirstOrDefaultAsync();
                var taskIdName = _db.ProjectLists.Where(x => x.IsActive && !x.IsDeleted && x.ID == model.ProjectId).Select(x => x.ProjectName).FirstOrDefault();
                var taskIdNumberCount = _db.TaskModels.Where(y => y.IsActive && !y.IsDeleted && y.ProjectId == model.ProjectId).ToList().Count();
                var check = model.EstimateTime.ToString().Split('.');
                string firstAndLast = "";
                string[] array = taskIdName.Trim().Split();
                if (array.Length == 1)
                {
                    firstAndLast = array.First().Substring(0, 1);
                }
                else
                {
                    firstAndLast = array.First().Substring(0, 1) + array.Last().Substring(0, 1);
                }


                var timeCheck = model.EstimateTime.ToString().Contains(".") ? model.EstimateTime.ToString()
                                     .Split('.').Select(long.Parse)
                                     .ToList() : model.EstimateTime.ToString()
                                     .Split(':')?.Select(long.Parse).ToList();
                long totalEstimageMinutes = (timeCheck[0] * 60) + timeCheck[1];

                if (checkData == null)
                {
                    TaskModel obj = new TaskModel
                    {
                        ProjectId = model.ProjectId,
                        CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                        CreatedBy = tokenData.employeeId,
                    };
                    _db.TaskModels.Add(obj);
                    await _db.SaveChangesAsync();
                    checkData = obj;
                }
                else
                {
                    checkData.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                    checkData.UpdatedBy = tokenData.employeeId;
                }
                checkData.SprintId = model.SprintId;
                checkData.TaskTitle = model.Title;
                checkData.AssignEmployeeId = model.AssignToId;
                checkData.TaskBilling = model.TaskBilling;
                checkData.StartDate = TimeZoneConvert.ConvertTimeToSelectedZone(model.StartDate.Value, tokenData.TimeZone);
                checkData.EndDate = TimeZoneConvert.ConvertTimeToSelectedZone(model.StartDate.Value, tokenData.TimeZone);
                checkData.Discription = model.Description;
                checkData.EstimateTime = totalEstimageMinutes;
                checkData.TaskIdNumber = taskIdNumberCount + 1;
                checkData.ProjectTaskId = firstAndLast + "" + checkData.TaskIdNumber;
                checkData.TaskType = TaskTypeConstants.Task;
                checkData.Priority = TaskPriorityConstants.Medium;
                checkData.CompanyId = tokenData.companyId;
                _db.Entry(checkData).State = EntityState.Modified;
                _db.SaveChanges();

                res.Message = "Task Created Succesfully !";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = checkData;
                return Ok(res);
            }
            catch (Exception ex)
            {

                logger.Error("API : api/projectdocument/addupdateprojectdocument | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class CreateTaskAndLog
        {
            public Guid TaskId { get; set; }
            public string Title { get; set; } = string.Empty;
            public int ProjectId { get; set; } = 0;
            public int AssignToId { get; set; } = 0;
            public TaskBillingConstants TaskBilling { get; set; } = TaskBillingConstants.Non_Billable;
            public DateTime? StartDate { get; set; }
            public string Description { get; set; } = string.Empty;
            public string EstimateTime { get; set; }
            public Guid SprintId { get; set; } = Guid.Empty;
        }

        #endregion

    }
}
