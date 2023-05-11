using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.TimeSheet;
using AspNetIdentity.WebApi.Models;
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
using static AspNetIdentity.WebApi.Controllers.DashboardController;
using static AspNetIdentity.WebApi.Controllers.TimeSheet.TaskCreationController;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.TimeSheet
{
    /// <summary>
    /// Created By Ravi Vyas On 28-11-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/tasklog")]
    public class TaskLogController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        #region API TO GET ASSIGNED PROJECT
        /// <summary>
        /// Created By Ravi Vyas On 28-11-2022
        /// API>>GET>>api/tasklog/getallassignproject
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallassignproject")]
        public async Task<IHttpActionResult> GetAllAssignedTask()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            TotaTaskCountAndEmployee response = new TotaTaskCountAndEmployee();
            try
            {
                List<GetProjectClass> getProjects = new List<GetProjectClass>();
                if (tokenData.IsAdminInCompany)
                {
                    var forAdmin = await (from p in _db.ProjectLists
                                          join a in _db.AssignProjects on p.ID equals a.ProjectId
                                          where a.IsActive && !a.IsDeleted && a.CompanyId == tokenData.companyId && p.ProjectStatus == ProjectStatusConstants.Live
                                          select new GetProjectClass
                                          {
                                              Project = p,
                                              AssignProject = a,
                                          })
                                            .ToListAsync();
                    getProjects = forAdmin;
                }
                else
                {
                    var forEmployee = await (from p in _db.ProjectLists
                                             join a in _db.AssignProjects on p.ID equals a.ProjectId
                                             where a.EmployeeId == tokenData.employeeId && a.IsActive &&
                                             !a.IsDeleted && a.CompanyId == tokenData.companyId && p.ProjectStatus == ProjectStatusConstants.Live
                                             select new GetProjectClass
                                             {
                                                 Project = p,
                                                 AssignProject = a,
                                             })
                                            .ToListAsync();
                    getProjects = forEmployee;
                }
                var projectId = getProjects.Select(x => x.Project.ID).Distinct().ToList();
                var taskPermission = await _db.TaskPermissions.Where(x => projectId.Contains(x.ProjectId)).ToListAsync();
                var responseData = projectId
                    .Select(z => new ProjectResponseModel
                    {
                        ProjectId = z,
                        ProjectName = getProjects.Where(x => x.Project.ID == z).Select(x => x.Project.ProjectName).FirstOrDefault(),
                        ProjectDiscription = getProjects.Where(x => x.Project.ID == z).Select(x => x.Project.ProjectDiscription).FirstOrDefault(),
                        IsTaskCreate = taskPermission.Where(x => x.ProjectId == z && x.AssigneEmployeeId == tokenData.employeeId).Select(x => x.IsCreateTask).FirstOrDefault(),
                        IsApproved = taskPermission.Where(x => x.ProjectId == z && x.AssigneEmployeeId == tokenData.employeeId).Select(x => x.IsApprovedTask).FirstOrDefault(),
                        EmployeeName = _db.AssignProjects.Where(x => x.ProjectId == z && x.IsActive && !x.IsDeleted)
                            //.Select(x => x.AssignProject.EmployeeId)
                            .Distinct()
                            .Select(x => new EmployeeData1
                            {
                                EmployeeId = x.EmployeeId,
                                FullName = _db.Employee.Where(a => a.EmployeeId == x.EmployeeId).Select(a => a.DisplayName).FirstOrDefault(),
                            })
                            .Distinct()
                            .ToList(),
                        EmployeeCount = _db.AssignProjects.Where(c => c.ProjectId == z).Select(c => c.EmployeeId).Count(),
                        IsMore = false,
                        PendingTaskCount = _db.TaskApprovels.
                                              Count(x => x.ProjectId == z && x.IsActive &&
                                              !x.IsDeleted && x.IsApproved == false &&
                                              x.IsRe_Evaluate == false && x.CompanyId == tokenData.companyId),
                    })
                    .OrderByDescending(x => x.ProjectId)
                    .ToList();
                response.ProjectResponseModel = responseData;
                if (response != null)
                {
                    res.Message = "All Task Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = new
                    {
                        IsAdmin = tokenData.IsAdminInCompany,
                        ProjectList = response,
                    };
                    return Ok(res);
                }
                else
                {
                    res.Message = "No Task  Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = new
                    {
                        IsAdmin = tokenData.IsAdminInCompany,
                        ProjectList = response,
                    };
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/tasklog/getallassignproject", ex.Message);
                return BadRequest("Failed");
            }
        }
        public class GetProjectClass
        {
            public ProjectList Project { get; set; }
            public AssignProject AssignProject { get; set; }
        }
        public class ProjectResponseModel
        {
            public int ProjectId { get; set; }
            public string ProjectName { get; set; }
            public string ProjectDiscription { get; set; }
            public List<EmployeeData1> EmployeeName { get; set; }
            public int EmployeeCount { get; set; }
            public bool IsMore { get; set; }
            public bool IsTaskCreate { get; set; }
            public int PendingTaskCount { get; set; }
            public bool IsApproved { get; set; }
        }

        public class TotaTaskCountAndEmployee
        {
            public List<ProjectResponseModel> ProjectResponseModel { get; set; }
            public int TotalTask { get; set; } = 0;
            public int TotalClosedTask { get; set; } = 0;
        }

        #endregion

        #region API FOR GET CURRENT DATE WEEK NAME AND DATA
        /// <summary>
        ///  Created By Ravi Vyas On 30-11-2022
        /// API >> Get >> api/tasklog/getweekdaydate
        /// </summary>
        /// <returns></returns>
        [Route("getweekdaydate")]
        [HttpGet]
        public async Task<IHttpActionResult> GetWeekDateDay(int ProjectId, DateTime? dateValue = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                DateTime dateCheck = dateValue.HasValue ? (DateTime)dateValue.Value.ToUniversalTime() : DateTime.UtcNow;
                var timeZoneData = TimeZoneConvert.ConvertTimeToSelectedZone(dateCheck, tokenData.TimeZone);
                List<Root> rootObj = new List<Root>();
                List<HelpForDayDate> dateList = new List<HelpForDayDate>();
                DayDateDataResponseModelClass dayDateDataResponseModelClass = new DayDateDataResponseModelClass();

                DayOfWeek weekStart = DayOfWeek.Monday; // or Sunday, or whenever
                //DateTimeOffset startingDate = dateCheck;
                DateTimeOffset startingDate = timeZoneData;

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

                DateTimeOffset startOfWeek = startingDate.AddDays((int)FirstDayOfWeek.Monday - (int)timeZoneData.DayOfWeek);
                string[] separators = { "::" };
                var result =
                    string
                    .Join("|", Enumerable
                    .Range(0, 7)
                    .Select(i =>
                        startingDate.AddDays(i)
                    ))
                    .Split('|')
                    .Select(x => new HelpForDayDate
                    {
                        Day = (DateTimeOffset.Parse(x)).ToString("dddd"),
                        ViewDate = (DateTime.Parse(x).Date),
                        Date = (DateTimeOffset.Parse(x).Date).ToString("dd-MMM"),
                        IsCurrentDate = (DateTimeOffset.Parse(x).Date == timeZoneData.Date),
                        TotalMinutes = 0,
                        TotalWorkingTimeInWeek = String.Empty,
                        IsHide = (DateTimeOffset.Parse(x).Date > timeZoneData.Date),
                    })
                    .ToList();
                dateList.AddRange(result);

                #endregion

                var taskModel = await _db.TaskModels
                    .Where(x => x.TaskType != TaskTypeConstants.BackLog
                            && x.IsActive && !x.IsDeleted &&
                            x.AssignEmployeeId == tokenData.employeeId && x.CompanyId == tokenData.companyId && x.ProjectId == ProjectId)
                           //Where(x => (subProjectId.HasValue) ? x.SubProjectId == subProjectId :  && x.SubProjectId == 0)                                /*(subProjectId.HasValue) ? x.SubProjectId == subProjectId :*/
                           //.Where(x => DbFunctions.TruncateTime(x.EndDate) >= start.Date && DbFunctions.TruncateTime(x.StartDate) <= end.Date)
                           .Where(x => x.EndDate >= start.Date && x.StartDate <= end.Date)
                           .ToListAsync();

                #region Refactor code


                //var itemGet = await _db.TaskModels
                //    .Where(x => x.IsActive && !x.IsDeleted && x.AssignEmployeeId == tokenData.employeeId &&
                //            //((x.StartDate >= start && x.EndDate <= end) || (x.StartDate >= start && x.EndDate >= end)) &&
                //            x.EndDate > start && x.StartDate < end &&
                //            x.ProjectId == ProjectId && x.CompanyId == tokenData.companyId)
                //    //.Where(x => x.IsActive && !x.IsDeleted && x.AssignEmployeeId == tokenData.employeeId &&
                //    //            x.ProjectId == ProjectId && x.CompanyId == tokenData.companyId)
                //    //.Select(x => new
                //    //{
                //    //    TaskData = x,
                //    //    DiffrenceInDate = x.EndDate - x.StartDate,
                //    //})
                //    //.Where(x => (x.DiffrenceInDate.Days <= 7 ? (x.TaskData.StartDate >= start && x.TaskData.EndDate <= end) : x.TaskData.EndDate <= end))
                //    //.Select(x => x.TaskData)
                //    .ToListAsync();

                //var joinProject = (from t in taskModel
                //                   join p in _db.ProjectLists on t.ProjectId equals p.ID
                //                   where p.ID == ProjectId
                //                   select new
                //                   {
                //                       TaskId = t.TaskId,
                //                       ProjectId = t.ProjectId,
                //                       ProjectName = p.ProjectName,
                //                       TaskName = t.TaskTitle,
                //                       EstimateTime = string.Format("{00:00}:{01:00}", (int)t.EstimateTime / 60, t.EstimateTime % 60),
                //                       EstimateTimeLong = t.EstimateTime,
                //                       StartDate = t.StartDate,
                //                       EndDate = t.EndDate,
                //                   })
                //                   .AsEnumerable();
                //var joinTaskApprovals = (from jp in joinProject
                //                         join ta in _db.TaskApprovels on jp.TaskId equals ta.TaskId into q
                //                         from empty in q.DefaultIfEmpty(new TaskApprovel
                //                         {
                //                             TaskApprovelId = Guid.Empty,
                //                             TaskRequest = TaskRequestConstants.Not_Selectd,
                //                             TaskName = String.Empty,
                //                             ReEvaluteDiscription = String.Empty,
                //                             IsRe_Evaluate = false,
                //                             IsApproved = false,
                //                             IsSFA = false,

                //                         })
                //                         select new Root
                //                         {
                //                             TaskId = jp.TaskId,
                //                             ProjectId = jp.ProjectId,
                //                             ProjectName = jp.ProjectName,
                //                             TaskName = jp.TaskName,
                //                             EstimateTime = jp.EstimateTime,
                //                             EstimateTimeLong = jp.EstimateTimeLong,
                //                             StartDate = jp.StartDate,
                //                             EndDate = jp.EndDate,
                //                             TaskRequestId = empty.TaskRequest,
                //                             TaskRequestName = empty.TaskRequest.ToString(),
                //                             Re_EvaluteComment = empty.ReEvaluteDiscription,
                //                             IsReEvaluet = empty.IsRe_Evaluate,
                //                             IsApproved = empty.IsApproved,
                //                             IsSFA = empty.IsSFA,
                //                             Dates = (from x in result
                //                                      join tl in _db.TaskLogs on jp.TaskId equals tl.TaskId into r
                //                                      from taskEmpty in r.DefaultIfEmpty(new TaskLog
                //                                      {
                //                                          SpentTime = new TimeSpan(00, 00, 00),
                //                                          IsApproved = false,
                //                                      })
                //                                      select new Date
                //                                      {
                //                                          DateNew = x.Date,
                //                                          Day = x.Day,
                //                                          SpentTime = taskEmpty.SpentTime,
                //                                          ViewSpentTime = taskEmpty.SpentTime.ToString(@"hh\:mm"),
                //                                          IsCurrent = x.Date == indianTimeZone.Date,
                //                                          IsApproved = taskEmpty.IsApproved,
                //                                          IsHide1 = x.Date > indianTimeZone.Date,
                //                                      })
                //                                      .ToList(),
                //                             TotalWorkingTimeInProject = String.Empty,
                //                         })
                //                         .ToList();

                //joinTaskApprovals.ForEach(x =>
                //{
                //    //result.ForEach(z =>
                //    //{
                //    //    z.TotalMinutes += x.Dates
                //    //                .Where(a => a.Day == z.Day)
                //    //                .Select(a => a.SpentTime.Minutes)
                //    //                .ToList()
                //    //                .Sum();
                //    //    z.TotalWorkingTimeInWeek = TimeSpan
                //    //        .FromMinutes(
                //    //            x.Dates
                //    //                .Where(a => a.Day == z.Day)
                //    //                .Select(a => a.SpentTime.Minutes)
                //    //                .ToList()
                //    //                .Sum())
                //    //        .ToString(@"hh\:mm");
                //    //});
                //    //x.TotalWorkingTimeInProject = TimeSpan
                //    //        .FromMinutes(x.Dates.Sum(a => a.SpentTime.Minutes))
                //    //        .ToString(@"hh\:mm");

                //    double minutes = 0;
                //    foreach (var item2 in x.Dates)
                //    {
                //        minutes += item2.SpentTime.TotalMinutes;
                //        var objectData = result.Where(z => z.Day == item2.Day).FirstOrDefault();
                //        objectData.TotalMinutes += item2.SpentTime.TotalMinutes;
                //    }
                //    x.TotalWorkingTimeInProject = (int)minutes / 60 + ":" + minutes % 60;
                //    x.TotalWorkingTimeInProject = string.Format("{0:00}:{1:00}", (int)minutes / 60, minutes % 60);
                //});
                //result.ToList().ForEach(x => x.TotalWorkingTimeInWeek = string.Format("{00:00}:{01:00}", (int)x.TotalMinutes / 60, x.TotalMinutes % 60));
                //dayDateDataResponseModelClass.Root = joinTaskApprovals.OrderBy(x => x.TaskId).ToList();
                //dayDateDataResponseModelClass.helpForDayDates = result
                // .Select(x => new
                // {
                //     x.Day,
                //     x.Date,
                //     x.TotalWorkingTimeInWeek,
                //     x.IsHide
                // })
                // .ToList();
                //dayDateDataResponseModelClass.TotalWorkingTimeInWeek = TimeSpan
                //            .FromMinutes(
                //                result.Select(x => x.TotalMinutes).Sum())
                //            .ToString(@"hh\:mm");
                //if (rootObj.Count > 0)
                //{
                //    res.Message = "Data Get Succesfully !";
                //    res.Status = true;
                //    res.StatusCode = HttpStatusCode.Found;
                //    res.Data = dayDateDataResponseModelClass;
                //    return Ok(res);

                //}
                //else
                //{
                //    res.Message = "No Task Found !";
                //    res.Status = false;
                //    res.StatusCode = HttpStatusCode.NoContent;
                //    res.Data = dayDateDataResponseModelClass;
                //    return Ok(res);
                //}

                #endregion

                foreach (var t in taskModel)
                {
                    Root obj = new Root
                    {
                        TaskId = t.TaskId,
                        ProjectId = t.ProjectId,
                        //ProjectName = _db.ProjectLists.Where(p => p.ID == t.ProjectId).Select(p => p.ProjectName).FirstOrDefault(),
                        ProjectName = _db.ProjectLists.Where(p => p.ID == t.ProjectId).Select(p => p.ProjectName).FirstOrDefault(),
                        TaskName = t.TaskTitle,
                        Description = t.Discription,
                        ProjectManagerId = _db.ProjectLists.Where(p => p.ID == t.ProjectId).Select(p => p.ProjectManager).FirstOrDefault(),
                        //EstimateTime = item1.EstimateTime,
                        EstimateTime = string.Format("{00:00}:{01:00}", (int)t.EstimateTime / 60, t.EstimateTime % 60),
                        EstimateTimeLong = t.EstimateTime,
                        StartDate = t.StartDate.Value,
                        EndDate = t.EndDate.Value,
                        TaskRequestId = _db.TaskApprovels.Where(x => x.TaskId == t.TaskId).Select(x => x.TaskRequest).FirstOrDefault(),
                        TaskRequestName = _db.TaskApprovels.Where(x => x.TaskId == t.TaskId).Select(x => x.TaskRequest).FirstOrDefault().ToString(),
                        Re_EvaluteComment = _db.TaskApprovels.Where(x => x.TaskId == t.TaskId).Select(x => x.ReEvaluteDiscription).FirstOrDefault(),
                        IsReEvaluet = _db.TaskApprovels.Where(x => x.TaskId == t.TaskId).Select(x => x.IsRe_Evaluate).FirstOrDefault(),
                        IsApproved = _db.TaskApprovels.Where(x => x.TaskId == t.TaskId).Select(x => x.IsApproved).FirstOrDefault(),
                        IsSFA = _db.TaskApprovels.Where(x => x.TaskId == t.TaskId).Select(x => x.IsSFA).FirstOrDefault(),
                        Dates = dateList.Select(x => new Date
                        {
                            IsSFA = _db.TaskApprovels.Where(i => i.TaskId == t.TaskId).Select(i => i.IsSFA).FirstOrDefault(),
                            DateNew = x.ViewDate,
                            Day = x.Day,
                            SpentTime = _db.TaskLogs.Where(p => p.TaskId == t.TaskId && DbFunctions.TruncateTime(p.DueDate) == x.ViewDate.Date).Select(p => p.SpentTime).FirstOrDefault(),
                            ViewSpentTime = _db.TaskLogs.Where(p => p.TaskId == t.TaskId && DbFunctions.TruncateTime(p.DueDate) == x.ViewDate.Date).Select(p => p.SpentTime).FirstOrDefault().ToString(@"hh\:mm"),
                            IsCurrent = x.ViewDate == timeZoneData.Date,
                            IsApproved = _db.TaskApprovels.Where(a => a.TaskId == t.TaskId).Select(a => a.IsApproved).FirstOrDefault(),
                            IsHide1 = x.ViewDate > timeZoneData.Date,
                        }).ToList(),
                    };
                    double minutes = 0;

                    foreach (var item2 in obj.Dates)
                    {
                        minutes += item2.SpentTime.TotalMinutes;
                        var objectData = result.Where(x => x.Day == item2.Day).FirstOrDefault();
                        objectData.TotalMinutes += item2.SpentTime.TotalMinutes;
                    }
                    obj.TotalWorkingTimeInProject = (int)minutes / 60 + ":" + minutes % 60;
                    obj.TotalWorkingTimeInProject = string.Format("{0:00}:{1:00}", (int)minutes / 60, minutes % 60);
                    rootObj.Add(obj);
                }

                var TotalEstimateTime = rootObj.Select(x => x.EstimateTimeLong).ToList().Sum();
                string TotalEstTime = string.Format("{0:00}:{1:00}", (int)TotalEstimateTime / 60, TotalEstimateTime % 60);


                result.ToList().ForEach(x => x.TotalWorkingTimeInWeek = string.Format("{00:00}:{01:00}", (int)x.TotalMinutes / 60, x.TotalMinutes % 60));
                dayDateDataResponseModelClass.TotalWorkingEstTime = TotalEstTime;
                dayDateDataResponseModelClass.Root = rootObj.OrderByDescending(x => x.StartDate).ToList();
                dayDateDataResponseModelClass.helpForDayDates = result
                 .Select(x => new
                 {
                     x.Day,
                     x.Date,
                     x.TotalWorkingTimeInWeek,
                     x.ViewDate,
                     x.IsHide
                 })
                 .ToList();
                double totalWorkingTimeInWeek = result.Select(x => x.TotalMinutes).Sum();
                dayDateDataResponseModelClass.TotalWorkingTimeInWeek = string.Format("{00:00}:{01:00}", (int)totalWorkingTimeInWeek / 60, totalWorkingTimeInWeek % 60);

                if (rootObj.Count > 0)
                {

                    res.Message = "Data Get Successfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = dayDateDataResponseModelClass;
                    return Ok(res);

                }
                else
                {
                    res.Message = "No Task Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = dayDateDataResponseModelClass;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {

                logger.Error("api/tasklog/getweekdaydate", ex.Message);
                return BadRequest("Failed");
            }
        }



        #endregion

        #region API FOR SUBMIT LOGTIME 
        /// API>>POST>>api/tasklog/submitlogtime
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("submitlogtime")]
        public async Task<IHttpActionResult> SubmitLogTime(List<Root> model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);

                }
                else
                {
                    foreach (var item in model)
                    {
                        foreach (var item1 in item.Dates)
                        {
                            var timeArray = item1.ViewSpentTime?.Split(':')?.Select(Int32.Parse)?.ToList();
                            var check = _db.TaskLogs.Where(x => x.TaskId == item.TaskId && x.DueDate == item1.DateNew && x.CompanyId == tokenData.companyId).FirstOrDefault();
                            if (check == null)
                            {
                                TaskLog obj = new TaskLog()
                                {
                                    TaskId = item.TaskId,
                                    ProjectId = item.ProjectId,
                                    DueDate = item1.DateNew,
                                    SpentTime = new TimeSpan(timeArray[0], timeArray[1], 00),
                                    CompanyId = tokenData.companyId,
                                    OrgId = tokenData.orgId,
                                    LogEmployeeId = tokenData.employeeId,
                                    CreatedBy = tokenData.employeeId,
                                    CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                                };
                                if (obj.SpentTime != TimeSpan.Zero)
                                    _db.TaskLogs.Add(obj);
                                await _db.SaveChangesAsync();
                            }
                            else
                            {
                                check.SpentTime = new TimeSpan(timeArray[0], timeArray[1], 00);
                                check.UpdatedBy = tokenData.employeeId;
                                check.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                                _db.Entry(check).State = System.Data.Entity.EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }

                    }

                    res.Message = "Succesfully Add !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    return Ok(res);
                }

            }
            catch (Exception ex)
            {

                logger.Error("api/tasklog/submitlogtime", ex.Message, model);
                res.Message = "Time Formate Is Not Valid";
                res.Status = false;
                res.StatusCode = HttpStatusCode.BadRequest;
                return Ok(res);
            }
        }


        #endregion

        #region API FOR SUBMIT LOGTIME  AND SEND FOR APPROVED
        ///// <summary>
        ///// API>>POST>>api/tasklog/submitlogtime
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("submitlogtime")]
        //public async Task<IHttpActionResult> SubmitLogTime(List<Root> model)
        //{
        //    ResponseStatusCode res = new ResponseStatusCode();
        //    var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //        else
        //        {

        //            foreach (var item in model)
        //            {
        //                foreach (var item1 in item.Dates)
        //                {
        //                    var timeArray = item1.ViewSpentTime?.Split(':')?.Select(Int32.Parse)?.ToList();
        //                    var check = _db.TaskLogs.Where(x => x.TaskId == item.TaskId && x.DueDate == item1.DateNew && x.CompanyId == tokenData.companyId).FirstOrDefault();
        //                    if (check == null)
        //                    {
        //                        TaskLog obj = new TaskLog()
        //                        {
        //                            TaskId = item.TaskId,
        //                            ProjectId = item.ProjectId,
        //                            DueDate = item1.DateNew,
        //                            SpentTime = new TimeSpan(timeArray[0], timeArray[1], 00),
        //                            CompanyId = tokenData.companyId,
        //                            OrgId = tokenData.orgId,
        //                            LogEmployeeId = tokenData.employeeId,
        //                            CreatedBy = tokenData.employeeId,
        //                            CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
        //                        };
        //                        if (obj.SpentTime != TimeSpan.Zero)
        //                            _db.TaskLogs.Add(obj);
        //                        await _db.SaveChangesAsync();
        //                    }
        //                    else
        //                    {
        //                        check.SpentTime = new TimeSpan(timeArray[0], timeArray[1], 00);
        //                        check.UpdatedBy = tokenData.employeeId;
        //                        check.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
        //                        _db.Entry(check).State = System.Data.Entity.EntityState.Modified;
        //                        _db.SaveChanges();
        //                    }
        //                }
        //            }

        //            //if (item.StartDate == item.EndDate)
        //            //{
        //            var taskSave = model
        //                      .Select(x => new TaskApprovel
        //                      {
        //                          TaskId = x.TaskId,
        //                          ProjectId = x.ProjectId,
        //                          ProjectName = x.ProjectName,
        //                          TaskName = x.TaskName,
        //                          TotalWorkingTime = x.TotalWorkingTimeInProject,
        //                          ProjectManagerId = x.ProjectManagerId,
        //                          TaskRequest = TaskRequestConstants.Pending,
        //                          StartDate = x.StartDate,
        //                          EstimateTime = x.EstimateTimeLong,
        //                          CompanyId = tokenData.companyId,
        //                          OrgId = tokenData.orgId,
        //                          DataJson = JsonConvert.SerializeObject(x.Dates),
        //                          CreatedBy = tokenData.employeeId,
        //                          CreatedOn = DateTimeOffset.Now,
        //                          IsSFA = true,
        //                      })
        //                      .ToList();

        //            foreach (var task in taskSave)
        //            {
        //                var checkApprovel = _db.TaskApprovels.Where(a => a.TaskId == task.TaskId).FirstOrDefault();
        //                if (checkApprovel == null)
        //                {
        //                    TaskApprovel obj = new TaskApprovel()
        //                    {
        //                        TaskId = task.TaskId,
        //                        ProjectId = task.ProjectId,
        //                        CreatedBy = tokenData.employeeId,
        //                        CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow),
        //                        CompanyId = tokenData.companyId,
        //                        IsSFA = true,
        //                    };

        //                    _db.TaskApprovels.Add(obj);
        //                    await _db.SaveChangesAsync();
        //                    checkApprovel = obj;
        //                }
        //                else
        //                {
        //                    checkApprovel.UpdatedBy = tokenData.employeeId;
        //                    checkApprovel.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
        //                }
        //                checkApprovel.ProjectName = task.ProjectName;
        //                checkApprovel.TaskName = task.TaskName;
        //                checkApprovel.TotalWorkingTime = task.TotalWorkingTime;
        //                checkApprovel.ProjectManagerId = task.ProjectManagerId;
        //                checkApprovel.TaskRequest = TaskRequestConstants.Pending;
        //                checkApprovel.StartDate = task.StartDate;
        //                checkApprovel.EstimateTime = task.EstimateTime;
        //                checkApprovel.CompanyId = tokenData.companyId;
        //                checkApprovel.OrgId = tokenData.orgId;
        //                checkApprovel.DataJson = task.DataJson;
        //                checkApprovel.IsRe_Evaluate = false;
        //                checkApprovel.IsSFA = true;

        //                _db.Entry(checkApprovel).State = System.Data.Entity.EntityState.Modified;
        //                await _db.SaveChangesAsync();
        //                res.Message = "Created Succesfully !";
        //                res.Status = true;
        //                res.StatusCode = HttpStatusCode.Created;
        //                return Ok(res);
        //            }

        //        }
        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {

        //        logger.Error("api/tasklog/submitlogtime", ex.Message, model);
        //        res.Message = "Time Formate Is Not Valid";
        //        res.Status = false;
        //        res.StatusCode = HttpStatusCode.BadRequest;
        //        return Ok(res);
        //    }
        //}


        #endregion

        #region API TO GET ALL EMPLOYEE IN PROJECT BY PROJECT ID
        /// <summary>
        /// Created By Ravi Vyas on 13-12-202
        /// API >> GET >> api/tasklog/getallemployeeinproject?projectId
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [Route("getallemployeeinproject")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllEmployeeInProject(int projectId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employeeInProject = await (from ap in _db.AssignProjects
                                               join em in _db.Employee on ap.EmployeeId equals em.EmployeeId
                                               where ap.ProjectId == projectId
                                               select new
                                               {
                                                   em.EmployeeId,
                                                   EmployeeName = em.DisplayName,

                                               })
                                               .ToListAsync();
                if (employeeInProject.Count > 0)
                {
                    res.Message = "Succcesfully Get !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = employeeInProject;
                    return Ok(res);
                }
                res.Message = "Data not Founs !";
                res.Status = false;
                res.StatusCode = HttpStatusCode.NotFound;
                res.Data = employeeInProject;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("api/tasklog/getallemployeeinproject", ex.Message);
                return BadRequest("Failed");
            }
        }

        #endregion

        #region Api for Get Assign Employeee List By Project Id
        /// <summary>
        /// Created By Ravi Vyas on 13-12-202
        /// API>>GET>>api/tasklog/getassignemployebyprojectid?ProjectId
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        [Route("getassignemployebyprojectid")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAssignEmployee(int ProjectId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkPermission = await _db.TaskPermissions.
                                      Where(x => x.IsActive && !x.IsDeleted &&
                                      x.AssigneEmployeeId == tokenData.employeeId && x.ProjectId == ProjectId).
                                      FirstOrDefaultAsync();

                var getData = await _db.AssignProjects.
                                    Where(x => x.ProjectId == ProjectId && x.IsActive && !x.IsDeleted).
                                    Where(x => !x.IsDeleted && checkPermission.ViewAlProjectTask
                                    ?
                                    x.CompanyId == tokenData.companyId
                                    :
                                    x.CompanyId == tokenData.companyId && x.EmployeeId == tokenData.employeeId).
                                    Select(x => new
                                    {
                                        EmployeeId = x.EmployeeId,
                                        EmployeeName = _db.Employee.Where(e => e.EmployeeId == x.EmployeeId).Select(e => e.DisplayName).FirstOrDefault(),
                                    }).ToListAsync();
                if (getData.Count > 0)
                {
                    res.Message = "Succcesfully Get !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = getData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data not Founs !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/tasklog/getassignemployebyprojectid", ex.Message);
                return BadRequest("Failed");
            }
        }

        #endregion

        #region For Helper Model
        public class HelpForDayDate
        {
            public string Day { get; set; }
            public string Date { get; set; }
            public DateTime ViewDate { get; set; }
            public double TotalMinutes { get; set; }
            public string TotalWorkingTimeInWeek { get; set; }
            public bool IsCurrentDate { get; set; }
            public bool IsHide { get; set; }

        }
        public class TimeSpanCountClass
        {
            public List<TimeSpan> TimeSpans { get; set; }
        }
        public class Date
        {
            public string Day { get; set; }
            public DateTimeOffset DateNew { get; set; }
            public TimeSpan SpentTime { get; set; }
            public string ViewSpentTime { get; set; }
            public bool IsCurrent { get; set; }
            public bool IsApproved { get; set; }
            public bool IsHide1 { get; set; }
            public bool IsSFA { get; set; }
        }
        public class Root
        {
            public Guid TaskId { get; set; } = Guid.Empty;
            public string ProjectName { get; set; }
            public int ProjectId { get; set; } = 0;
            public string TaskName { get; set; }
            public string TotalWorkingTimeInProject { get; set; }
            public double AllWorkingTimeInProject { get; set; }
            public int ProjectManagerId { get; set; }
            public TaskRequestConstants TaskRequestId { get; set; }
            public string TaskRequestName { get; set; }
            public string EstimateTime { get; set; }
            public DateTimeOffset StartDate { get; set; }
            public DateTimeOffset EndDate { get; set; }
            public string Re_EvaluteComment { get; set; }
            public bool IsReEvaluet { get; set; }
            public bool IsApproved { get; set; }
            public bool IsSFA { get; set; }
            public long EstimateTimeLong { get; set; }
            public List<Date> Dates { get; set; }
            public string Description { get; set; }
        }

        public class TaskLogResponseModel
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
            public double EstimateTime { get; set; }
            public DateTimeOffset StartDate { get; set; }
            public DateTimeOffset EndDate { get; set; }
        }

        public class DayDateDataResponseModelClass
        {
            public string TotalWorkingTimeInWeek { get; set; }
            public object helpForDayDates { get; set; }
            public List<Root> Root { get; set; }

            public string TotalWorkingEstTime { get; set; }
        }



        #endregion

        #region Api for Update Task
        /// <summary>
        /// API>>PUT>>api/tasklog/updatetask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("updatetask")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateTaskData(TaskUpdateRequestModelClass model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkData = _db.TaskModels.Where(x => x.TaskId == model.TaskId
                && x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId).FirstOrDefault();
                var check = model.OrignalEstimateTime.ToString().Split('.');
                var timeCheck = model.OrignalEstimateTime.ToString().Contains(".") ? model.OrignalEstimateTime.ToString()
                                .Split('.').Select(long.Parse).ToList() : model.OrignalEstimateTime.ToString().Split(':')?.Select(long.Parse).ToList();
                long totalEstimageMinutes = (timeCheck[0] * 60) + timeCheck[1];
                if (checkData != null)
                {
                    checkData.TaskTitle = model.TaskTitle;
                    checkData.Discription = model.Discription;
                    checkData.Priority = model.Priority;
                    //checkData.SubProjectId = model.SubProjectId;
                    checkData.ProjectId = model.ProjectId;
                    checkData.TaskType = model.TaskType;
                    checkData.EstimateTime = totalEstimageMinutes;
                    checkData.StartDate = model.StartDate;
                    checkData.EndDate = model.EndDate;
                    checkData.Attechment = model.Attechment;
                    checkData.Image1 = model.Image1;
                    checkData.Image2 = model.Image2;
                    checkData.Image3 = model.Image3;
                    checkData.Image4 = model.Image4;
                    checkData.TaskBilling = model.TaskBilling;
                    checkData.SprintId = model.SprintId;
                    checkData.UpdatedOn = DateTimeOffset.Now;
                    checkData.AssignEmployeeId = model.AssignEmployeeId;
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

                logger.Error("api/tasklog/updatetask", ex.Message, model);
                res.Message = ex.Message;
                res.Status = false;
                return BadRequest("Failed");
            }
        }
        public class TaskUpdateRequestModelClass : CreateTaskRequest
        {
            public string OrignalEstimateTime { get; set; }
        }

        #endregion

        #region Api for Add And Update Task log Time
        /// <summary>
        /// Api>>post>>api/tasklog/addupdatetasklog
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addupdatetasklog")]
        public async Task<IHttpActionResult> AddUpdateTaskLogTime(AddUpdateTaskLog model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var timeArray = model.ViewSpentTime?.Split(':')?.Select(Int32.Parse)?.ToList();
                var checkData = await _db.TaskLogs.Where(x => x.IsActive && !x.IsDeleted &&
                                x.CompanyId == tokenData.companyId && x.TaskId == model.TaskId && x.DueDate == model.TaskLogDate).FirstOrDefaultAsync();
                if (checkData == null)
                {
                    TaskLog obj = new TaskLog
                    {
                        ProjectId = model.ProjectId,
                        TaskId = model.TaskId,
                        CreatedBy = tokenData.employeeId,
                        CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                    };
                    _db.TaskLogs.Add(obj);
                    await _db.SaveChangesAsync();
                    checkData = obj;
                }
                else
                {
                    checkData.UpdatedBy = tokenData.employeeId;
                    checkData.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                }
                checkData.SpentTime = new TimeSpan(timeArray[0], timeArray[1], 00);
                checkData.DueDate = model.TaskLogDate;
                checkData.LogEmployeeId = tokenData.employeeId;
                checkData.CompanyId = tokenData.companyId;
                _db.Entry(checkData).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = " Add Successfully !";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                res.Data = checkData;
                return Ok(res);
            }
            catch (Exception ex)
            {

                logger.Error("api/tasklog/addupdatetasklog", ex.Message, model);
                res.Status = false;
                res.StatusCode = HttpStatusCode.BadRequest;
                return Ok(res);
            }
        }

        public class AddUpdateTaskLog
        {
            public Guid TaskId { get; set; }
            public int ProjectId { get; set; }
            public DateTimeOffset TaskLogDate { get; set; }
            public TimeSpan SpentTime { get; set; } = TimeSpan.Zero;
            public string ViewSpentTime { get; set; }
            //public DateTimeOffset TaskDate { get; set; }
        }

        #endregion

        #region Api for Get Select Assignee For Assign Task By Project Id
        /// <summary>
        /// Created By Ravi Vyas on 13-12-202
        /// API>>GET>>api/tasklog/selectassignee?ProjectId
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        [Route("selectassignee")]
        [HttpGet]
        public async Task<IHttpActionResult> SelectAssignee(int ProjectId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkPermission = await _db.TaskPermissions
                                      .Where(x => x.IsActive && !x.IsDeleted &&
                                      x.AssigneEmployeeId == tokenData.employeeId && x.ProjectId == ProjectId)
                                      .FirstOrDefaultAsync();

                var getData = await _db.AssignProjects.
                                    Where(x => x.IsActive && !x.IsDeleted && x.ProjectId == ProjectId).
                                    //  Where(x => (subProjectId.HasValue) ? x.ProjectId == subProjectId :  ).
                                    Where(x => !x.IsDeleted &&
                                    checkPermission.IsOtherTaskCreate
                                    ?
                                    x.CompanyId == tokenData.companyId
                                    :
                                    x.CompanyId == tokenData.companyId && x.EmployeeId == tokenData.employeeId)
                                    .Select(x => new
                                    {
                                        EmployeeId = x.EmployeeId,
                                        EmployeeName = _db.Employee.Where(e => e.EmployeeId == x.EmployeeId).Select(e => e.DisplayName).FirstOrDefault(),
                                    })
                                    .ToListAsync();

                if (getData.Count > 0)
                {
                    res.Message = "Successfully Get !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = getData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/tasklog/getassignemployebyprojectid", ex.Message);
                return BadRequest("Failed");
            }
        }

        #endregion

        #region Api for get All Assign Project Task Count 
        /// <summary>
        /// Api >> Get >> api/tasklog/getassigntaskprojectdata
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getassigntaskprojectdata")]
        public async Task<IHttpActionResult> GetAssigneTaskProject(int? projectId = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkTask = await _db.TaskModels.Where(x => x.IsActive && !x.IsDeleted &&
                                              x.CompanyId == tokenData.companyId &&
                                              x.AssignEmployeeId == tokenData.employeeId &&
                                              (projectId.HasValue) ? x.ProjectId == projectId :
                                              x.CompanyId == tokenData.companyId
                                              )
                                             .ToListAsync();

                var SpenTime = _db.TaskModels.Where(x => x.IsActive && !x.IsDeleted &&
                                              x.CompanyId == tokenData.companyId)
                                              .Select(x => new
                                              {
                                                  SpentTime = _db.TaskLogs.Where(t => t.TaskId == x.TaskId).Select(t => t.SpentTime).ToList(),
                                              }).ToList();




                res.Data = new
                {
                    totalTaskCount = checkTask.Count(),
                    totalPendingTask = checkTask.Count(x => x.Status == TaskStatusConstants.Pending),
                    totalClosedTask = checkTask.Count(x => x.Status == TaskStatusConstants.Closed),
                    EstimateTime = string.Format("{00:00}:{01:00}", (int)(checkTask.Select(z => z.EstimateTime).ToList().Sum() / 60), (int)(checkTask.Select(z => z.EstimateTime).ToList().Sum() % 60))
                };
                return Ok();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public class EmployeeTaskCountRequestBody
        {
            public int ProjectId { get; set; } = 0;
        }

        #endregion

        #region Api for Get All Assign project for add SubProject
        /// <summary>
        /// Created By Ravi Vyas On 03-04-2023
        /// API >> GET >>  api/tasklog/getallactiveproject
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallactiveproject")]
        public async Task<IHttpActionResult> GetAllAssignProjectForSubProject()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {

                var getData = await (from p in _db.ProjectLists
                                     join a in _db.AssignProjects on p.ID equals a.ProjectId
                                     //join per in _db.TaskPermissions on p.ID equals per.ProjectId
                                     //into r
                                     //from emptyPermission in r.DefaultIfEmpty()
                                     where p.IsActive && !p.IsDeleted && p.CompanyId == tokenData.companyId &&
                                     a.EmployeeId == tokenData.employeeId && p.SubProjectId == 0
                                     select new
                                     {
                                         ProjectId = p.ID,
                                         ProjectName = p.ProjectName,
                                         //IsTaskCreate = emptyPermission == null ? false : emptyPermission.IsCreateTask,
                                         //IsApproved = emptyPermission == null ? false : emptyPermission.IsApprovedTask,
                                         IsTaskCreate = _db.TaskPermissions.Where(x => x.ProjectId == p.ID && x.AssigneEmployeeId == tokenData.employeeId).Select(x => x.IsCreateTask).FirstOrDefault(),
                                         IsApproved = _db.TaskPermissions.Where(x => x.ProjectId == p.ID && x.AssigneEmployeeId == tokenData.employeeId).Select(x => x.IsApprovedTask).FirstOrDefault(),

                                     })
                                     .Distinct()
                                     .ToListAsync();
                if (getData.Count > 0)
                {
                    res.Message = "Project List Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = getData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Project List Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {

                logger.Error(" api/tasklog/getallactiveproject", ex.Message);
                return BadRequest("Failed");
            }
        }

        #endregion
    }
}

