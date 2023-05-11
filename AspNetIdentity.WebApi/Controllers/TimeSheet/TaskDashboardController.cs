using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.TimeSheet;
using AspNetIdentity.WebApi.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.TimeSheet
{
    /// <summary>
    /// Created By Ravi Vyas on 10-12-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/taskdashboard")]
    public class TaskDashboardController : ApiController
    {

        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Api for Task Dashboard

        /// <summary>
        /// API>>api/taskdashboard/dashboard?ProjectId?year?month
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [Route("dashboard")]
        [HttpGet]
        public async Task<IHttpActionResult> DashboardData(int projectId, int year = 0, int month = 0)            /*  int? subProjectId = null,*/
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            DashboradResponse response = new DashboradResponse();
            year = year == 0 ? year : DateTimeOffset.Now.Year;
            try
            {

                var getData = await _db.AssignProjects.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId)           /* x => (subProjectId.HasValue) ? x.ProjectId == subProjectId :*/
                                                      .Where(x => x.ProjectId == projectId)
                                                      .ToListAsync();
                var taskData = await _db.TaskModels.Where(x => x.IsActive && !x.IsDeleted &&
                                                   x.CompanyId == tokenData.companyId && x.ProjectId == projectId)
                                                   .ToListAsync();

                //(subProjectId.HasValue) ? x.SubProjectId == subProjectId :
                //  x.ProjectId == projectId && x.SubProjectId == 0


                #region My Performance Data

                var getTime = taskData.Where(x => x.AssignEmployeeId == tokenData.employeeId)
                .Select(x => new
                {
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    EstimateTime = x.EstimateTime,
                    SpentTime = _db.TaskLogs.Where(t => t.TaskId == x.TaskId).Select(t => t.SpentTime).FirstOrDefault()
                }).ToList();

                month = month == 0 ? DateTimeOffset.Now.Month : month;

                double totalEstimatedTime = getTime.Where(x => x.StartDate.Value.Month == month).Select(x => x.EstimateTime).Sum();
                var TotalEstimatedTime = string.Format("{00:00}.{01:00}", (int)totalEstimatedTime / 60, totalEstimatedTime % 60);
                double totalSpentTime = getTime.Where(x => x.StartDate.Value.Month == month).Select(x => x.SpentTime.TotalMinutes).Sum();
                var TotalSpentTime = string.Format("{00:00}.{01:00}", (int)totalSpentTime / 60, totalSpentTime % 60);
                response.TimeData = new List<object>()
                    {
                        new
                        {
                            name = "Total Estimate Time",
                            value = TotalEstimatedTime,
                        },
                        new
                        {
                            name = "Actual Time",
                            value = TotalSpentTime,
                        }
                    };
                #endregion

                #region Login Employee Task Details

                var taskDetails = taskData.Where(x => x.AssignEmployeeId == tokenData.employeeId && x.Status != TaskStatusConstants.Closed)
                    .Select(x => new
                    {
                        TaskIdNumber = x.TaskIdNumber,
                        TaskId = x.TaskId,
                        TaskTitile = x.TaskTitle,
                        Type = x.TaskType.ToString(),
                        Status = x.Status.ToString(),
                        StartDate = x.StartDate,
                        ProjectId = x.ProjectId,
                        CreatedDate = x.CreatedOn,
                    }).OrderBy(x => x.CreatedDate).ToList();
                response.LogInEmployeeTask = taskDetails;
                #endregion

                #region Login Employee Task Type Info

                var getTaskTypeCount = Enum.GetValues(typeof(TaskTypeConstants))
                                      .Cast<TaskTypeConstants>()
                                      .Select(x => new
                                      {
                                          TypeId = ((int)x),
                                          TypeName = x.ToString().Replace("_", " "),
                                      }).ToList();

                var workItemTypeCount = getTaskTypeCount
                                      .Select(x => new
                                      {
                                          Name = x.TypeName,
                                          Value = taskData.Where(c => c.Status != TaskStatusConstants.Closed && ((int)c.TaskType) == x.TypeId && c.AssignEmployeeId == tokenData.employeeId).Count(),
                                      })
                                      .ToList();
                response.LoginEmployeeData = workItemTypeCount;

                #endregion

                #region Api for Assigne To Data dashboard

                var empList = await (from ap in _db.AssignProjects
                                     join em in _db.Employee on ap.EmployeeId equals em.EmployeeId
                                     where ap.IsActive && !ap.IsDeleted &&
                                     ap.CompanyId == tokenData.companyId && ap.ProjectId == projectId
                                     //where (subProjectId.HasValue) ? ap.ProjectId == subProjectId :  ap.ProjectId == projectId  
                                     select new
                                     {
                                         em.EmployeeId,
                                         em.DisplayName,
                                     })
                                     .Distinct()
                                     .ToListAsync();
                var dbTaskList = empList
                    .Select(emp => new
                    {
                        EmployeeName = emp.DisplayName,
                        Task = taskData.Count(x => x.TaskType == TaskTypeConstants.Task && x.AssignEmployeeId == emp.EmployeeId),
                        Issue = taskData.Count(x => x.TaskType == TaskTypeConstants.Issue && x.AssignEmployeeId == emp.EmployeeId),
                        Bug = taskData.Count(x => x.TaskType == TaskTypeConstants.Bug && x.AssignEmployeeId == emp.EmployeeId),
                        Other = taskData.Count(x => x.AssignEmployeeId == emp.EmployeeId && (x.TaskType == TaskTypeConstants.Leave || x.TaskType == TaskTypeConstants.Break || x.TaskType == TaskTypeConstants.Deployment)),
                        Total = taskData.Count(x => x.AssignEmployeeId == emp.EmployeeId)
                    })
                    .ToList();
                response.Assigned = new
                {
                    EmployeeTaskCoutList = dbTaskList,
                    TotalUniqueIssue = new
                    {
                        TotalTask = dbTaskList.Sum(x => x.Task),
                        TotalIssue = dbTaskList.Sum(x => x.Issue),
                        TotalBug = dbTaskList.Sum(x => x.Bug),
                        OtherCount = dbTaskList.Sum(x => x.Other),
                        TotalSum = dbTaskList.Sum(x => x.Total)
                    },
                };

                #endregion

                #region Api for Issue Statistics Data 

                var empListData = await (from ap in _db.AssignProjects
                                         join em in _db.Employee on ap.EmployeeId equals em.EmployeeId
                                         where ap.IsActive && !ap.IsDeleted && ap.CompanyId == tokenData.companyId && ap.ProjectId == projectId
                                         //where (subProjectId.HasValue) ? ap.ProjectId == subProjectId : ap.ProjectId == projectId  
                                         select new
                                         {
                                             em.EmployeeId,
                                             em.DisplayName,
                                         })
                                         .Distinct()
                                         .ToListAsync();
                var IssueStatisticsResponse = empListData
                                    .Select(x => new
                                    {
                                        EmployeeName = x.DisplayName,
                                        Total = taskData.LongCount(z => z.AssignEmployeeId == x.EmployeeId),
                                        CloseTask = taskData.LongCount(z => z.AssignEmployeeId == x.EmployeeId && z.Status == TaskStatusConstants.Closed),

                                    })
                                    .Select(x => new
                                    {
                                        EmployeeName = x.EmployeeName,
                                        Total = x.Total,
                                        CloseTAsk = x.CloseTask,
                                        Percentage = (x.Total == 0 ? 0 : (x.CloseTask == 0 ? 0 : Math.Round((((double)x.CloseTask / x.Total) * 100), 2))),
                                    })
                                    .OrderBy(x => x.EmployeeName)
                                    .ToList();


                response.IssueStatistics = new
                {
                    EmployeeTaskCoutList = IssueStatisticsResponse,
                    TotalUniqueIssue = new
                    {

                        TotalData = IssueStatisticsResponse.Sum(x => x.Total),
                    },
                };

                #endregion Bar Chart Data

                #region Api for Status Count

                var StatuStatisticsResponse = taskData
                         .Select(x => new
                         {
                             Name = x.Status.ToString(),
                             x.Status,
                             Total = taskData.LongCount(),

                         })
                         .Distinct()
                         .ToList()
                         .Select(x => new
                         {
                             x.Name,
                             x.Total,
                             x.Status,
                             Value = taskData.LongCount(z => z.Status == x.Status),

                         })
                         .Select(x => new
                         {
                             x.Name,
                             x.Value,
                             x.Status,
                             Percentage = Math.Round((((double)x.Value / x.Total) * 100), 2),
                         })
                         .OrderBy(x => x.Name)
                         .ToList();
                response.StatuStatistics = new
                {
                    EmployeeTaskCoutList = StatuStatisticsResponse,
                    TotalUniqueIssue = new
                    {

                        TotalDataCount = StatuStatisticsResponse.Sum(x => x.Value),
                        ProjectName = _db.ProjectLists.Where(p => p.ID == projectId).Select(p => p.ProjectName).FirstOrDefault()
                    },
                };
                #endregion

                #region Api for My Team
                var myEmpList = await (from ap in _db.AssignProjects
                                       join em in _db.Employee on ap.EmployeeId equals em.EmployeeId
                                       where ap.CompanyId == tokenData.companyId && ap.ProjectId == projectId
                                       && ap.IsActive && !ap.IsDeleted
                                       //where (subProjectId.HasValue) ? ap.ProjectId == subProjectId :
                                       //                ap.ProjectId == projectId 
                                       select new
                                       {
                                           em.EmployeeId,
                                           em.DisplayName,
                                           em.ProfileImageUrl
                                       })
                                    .Distinct()
                                    .ToListAsync();
                response.MyTeam = myEmpList;

                #endregion


                res.Message = "Successfully Get Data !";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Found;
                res.Data = response;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("api/taskdashboard/dashboard", ex.Message);
                return BadRequest("Failed");
            }

        }

        #endregion

        #region API To Get Year List Of Task Dashboard
        /// <summary>
        /// Created By Ravi Vyas On 12-12-2022
        /// API >> Get >> api/taskdashboard/taskdashboardyearlist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("taskdashboardyearlist")]
        public async Task<ResponseBodyModel> GetLeaveDashboardYearList()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var taskRequest = await _db.TaskModels.Where(x => claims.companyId == x.CompanyId && x.IsActive && !x.IsDeleted).ToListAsync();
                var checkYear = taskRequest
                        .Select(x => new
                        {
                            Name = x.CreatedOn.Year
                        }).Distinct().ToList();
                if (checkYear.Count > 0)
                {
                    res.Message = "Year List";
                    res.Status = true;
                    res.Data = checkYear;
                }
                else
                {
                    res.Message = "No Leave Request Found";
                    res.Status = false;
                    res.Data = checkYear;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Year List Of Leave Dashboard

        #region API To Get Month List Of Task Dashboard
        /// <summary>
        /// Created By Ravi Vyas On 25-08-2022
        /// API >> Get >> api/taskdashboard/taskdashboardmonthlist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("taskdashboardmonthlist")]
        public async Task<ResponseBodyModel> GetLeaveDashboardMonthListAsync()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var expensesRequest = await _db.TaskModels.Where(x => claims.companyId == x.CompanyId /*&& x.CreatedOn.Year == year*/)
                            .Select(x => x.CreatedOn.Month).Distinct().OrderBy(x => x).FirstOrDefaultAsync();
                var months = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthGenitiveNames
                                        .TakeWhile(m => m != String.Empty)
                                        .Select((m, i) => new
                                        {
                                            Month = i + 1,
                                            MonthName = m
                                        }).ToList();
                months = months.Skip(expensesRequest - 1).ToList();
                if (months.Count > 0)
                {
                    res.Message = "Month List !";
                    res.Status = true;
                    res.Data = months;
                }
                else
                {
                    res.Message = "No Expenses Found !";
                    res.Status = false;
                    res.Data = months;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Month List Of Leave Dashboard

        #region Helper Model Class

        public class DashboradResponse
        {
            public object TimeData { get; set; }
            public object LogInEmployeeTask { get; set; }
            public object LoginEmployeeData { get; set; }
            public object Assigned { get; set; }
            public object IssueStatistics { get; set; }
            public object StatuStatistics { get; set; }
            public object MyTeam { get; set; }

        }


        #endregion

    }
}
