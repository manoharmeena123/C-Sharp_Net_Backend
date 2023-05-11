using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
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
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.TimeSheet
{
    /// <summary>
    /// Created By Ravi Vyas On 11-01-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/newdashboard")]
    public class NewProjectDashboardController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Api for New Project Dashboard
        /// <summary>
        /// Created By Ravi Vyas on 12/01/2023
        /// API>>api/newdashboard/newprojectdashboard
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("newprojectdashboard")]
        public async Task<IHttpActionResult> GetProjectDashbaordData(int month = 0, int year = 0)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ProjectDashboardResponseModel response = new ProjectDashboardResponseModel();
            try
            {
                month = month == 0 ? DateTimeOffset.Now.Month : month;
                year = year == 0 ? DateTimeOffset.Now.Year : year;

                var totalTaskData = await _db.TaskModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId).ToListAsync();
                var totalProject = await _db.ProjectLists.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId).ToListAsync();
                var assignProjectEmployee = await _db.AssignProjects.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId).ToListAsync();

                response.TotalTask = totalTaskData.Count();

                #region Count For TimeSheetNotFilled 

                var getData = (from t in totalTaskData
                               join tl in _db.TaskLogs on t.TaskId equals tl.TaskId into r
                               from result in r.DefaultIfEmpty()
                               where t.CompanyId == tokenData.companyId && t.IsActive && !t.IsDeleted
                               select new
                               {

                                   SpendTime = result == null ? TimeSpan.Zero : result.SpentTime,

                               }).Where(x => x.SpendTime == TimeSpan.Zero)
                                 .Count();


                response.TimeSheetNotFilled = getData;

                #endregion

                #region Count for TimeSheetFilled 

                var getDataCount = (from t in totalTaskData
                                    join tl in _db.TaskLogs on t.TaskId equals tl.TaskId
                                    where t.CompanyId == tokenData.companyId
                                    select new
                                    {

                                    })
                                          .Count();
                response.TimeSheetFilled = getDataCount;

                #endregion

                #region Count for Total Project 

                response.TotalProjectCount = totalProject.Count();
                response.TotalAssignProject = assignProjectEmployee.Select(x => x.EmployeeId).Distinct().Count();
                response.TotalProjectNotAssign = (from e in _db.Employee
                                                  join u in _db.User on e.EmployeeId equals u.EmployeeId
                                                  join ap in _db.AssignProjects on e.EmployeeId equals ap.EmployeeId into r
                                                  from empty in r.DefaultIfEmpty()
                                                  where e.CompanyId == tokenData.companyId && e.IsActive && !e.IsDeleted
                                                  select new
                                                  {
                                                      //EmployeId = empty.EmployeeId
                                                  }).Count();
                #endregion

                #region Count for Billing Employee 

                var totalbillableEmployee = assignProjectEmployee.Count(x => x.Status == "Billable");

                var totalNonbillableEmployee = assignProjectEmployee.Count(x => x.Status == "Non-Billable");

                var totalPbillableEmployee = assignProjectEmployee.Count(x => x.Status == "Partial-Billable");

                response.NoOfBillableResource = totalbillableEmployee;
                response.NoOfNonBillableResource = totalNonbillableEmployee;
                response.NoOfPartialBillableResource = totalPbillableEmployee;
                #endregion

                #region Count For Employee In Company
                var totalEmployeeCount = (from e in _db.Employee
                                          join u in _db.User on e.EmployeeId equals u.EmployeeId
                                          where e.IsActive && !e.IsDeleted && e.CompanyId == tokenData.companyId
                                          select new
                                          {

                                          }).Count();
                response.TotalEmployeeCount = totalEmployeeCount;
                #endregion

                #region Bar Chart Data For Billable Non-Billable or Partial-Billable

                var months = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthGenitiveNames
                                          .TakeWhile(m => m != String.Empty)
                                          .Select((m, i) => new
                                          {
                                              Month = i + 1,
                                              MonthName = m
                                          }).ToList();
                var monthNames = months.ConvertAll(x => x.MonthName);
                var listDynamic = new List<dynamic>();
                var intList1 = new List<double>();
                var data1 = new
                {
                    label = "Billable",
                    stack = "1",
                    data = intList1,
                };
                var intList2 = new List<double>();
                var data2 = new
                {
                    label = "Non-Billable",
                    stack = "2",
                    data = intList2,
                };
                var intList3 = new List<double>();
                var data3 = new
                {
                    label = "Partial-Billable",
                    stack = "3",
                    data = intList3,
                };

                foreach (var item in months)
                {

                    intList1.Add(assignProjectEmployee.Where(x => x.CreatedOn.Month == item.Month && x.Status == "Billable" && x.CreatedOn.Year == year).Count());
                    intList2.Add(assignProjectEmployee.Where(x => x.CreatedOn.Month == item.Month && x.Status == "Billable" && x.CreatedOn.Year == year).Count());
                    intList3.Add(assignProjectEmployee.Where(x => x.CreatedOn.Month == item.Month && x.Status == "Billable" && x.CreatedOn.Year == year).Count());

                }
                listDynamic.Add(data1);
                listDynamic.Add(data2);
                listDynamic.Add(data3);

                response.label = monthNames;
                response.graph = listDynamic;

                #endregion Bar Chart Data

                #region Api for Pie Chart

                var monthWiseBillableCount = assignProjectEmployee.Where(x => x.CreatedOn.Month == month && x.Status == "Billable").Count();
                var monthWiseNonBillableCount = assignProjectEmployee.Where(x => x.CreatedOn.Month == month && x.Status == "Non-Billable").Count();
                var monthWisePBillableCount = assignProjectEmployee.Where(x => x.CreatedOn.Month == month && x.Status == "Partial-Billable").Count();

                response.ResourcePercentage = new List<object>()
                    {
                        new
                        {
                            name = "Billable Resource",
                            //value = totalbillableEmployee == 0 ? 0:(totalEmployeeCount/totalbillableEmployee)*100,
                            value = monthWiseBillableCount == 0 ? 0:(totalEmployeeCount/monthWiseBillableCount)*100,
                        },
                        new
                        {
                            name = "Non-Billable Resource",
                            value = monthWiseNonBillableCount == 0 ? 0: (totalEmployeeCount/monthWiseNonBillableCount)*100
                        },
                        new
                        {
                             name = "Partial-Billable Resource",
                            value = monthWisePBillableCount == 0 ? 0: (totalEmployeeCount/monthWisePBillableCount)*100
                        }
                    };

                #endregion

                #region Api for Active Hold And Pending Project

                response.TotalActiveProject = totalProject.Count(p => p.ProjectStatus == ProjectStatusConstants.Live);
                response.TotalCompletedProject = totalProject.Count(p => p.ProjectStatus == ProjectStatusConstants.Closed);
                response.TotalHoldProject = totalProject.Count(p => p.ProjectStatus == ProjectStatusConstants.Hold);

                #endregion

                #region Bar Chart Data For Active  Hold and Closed Project

                var monthsData = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthGenitiveNames
                                          .TakeWhile(m => m != String.Empty)
                                          .Select((m, i) => new
                                          {
                                              Month = i + 1,
                                              MonthName = m
                                          }).ToList();
                var monthNamesData = monthsData.ConvertAll(x => x.MonthName);

                var listDynamicList = new List<dynamic>();
                var intListData1 = new List<double>();
                var dataList1 = new
                {
                    Label = "Active Project",
                    Stack = "1",
                    Data = intListData1,
                };
                var intListData2 = new List<double>();
                var dataList2 = new
                {
                    Label = "Completed Project",
                    Stack = "2",
                    Data = intListData2,
                };
                var intListData3 = new List<double>();
                var dataList3 = new
                {
                    Label = "Hold Project",
                    Stack = "3",
                    Data = intListData3,
                };

                foreach (var item in monthsData)
                {
                    intListData1.Add(totalProject.Where(x => x.CreatedOn.Month == item.Month && x.ProjectStatus == ProjectStatusConstants.Live && x.CreatedOn.Year == year).Count());
                    intListData2.Add(totalProject.Where(x => x.CreatedOn.Month == item.Month && x.ProjectStatus == ProjectStatusConstants.Closed && x.CreatedOn.Year == year).Count());
                    intListData3.Add(totalProject.Where(x => x.CreatedOn.Month == item.Month && x.ProjectStatus == ProjectStatusConstants.Hold && x.CreatedOn.Year == year).Count());
                }
                listDynamicList.Add(dataList1);
                listDynamicList.Add(dataList2);
                listDynamicList.Add(dataList3);

                response.ProjectLabel = monthNamesData;
                response.ProjectGraph = listDynamicList;

                #endregion Bar Chart Data

                res.Message = "Get Succesfully !";
                res.Status = true;
                res.Data = response;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/newdashboard/newprojectdashboards | " +
                             "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }

        }

        #endregion

        #region Api for Get All Company Project 
        /// <summary>
        /// Created By Ravi Vyas on 12/01/2023
        /// API>>GET>>api/newdashboard/getallcomapnyprojects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallcomapnyprojects")]
        public async Task<IHttpActionResult> GetAllCompanyproject()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var projetcList = await _db.ProjectLists.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId).ToListAsync();
                if (projetcList.Count > 0)
                {
                    res.Message = "Project List Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = projetcList;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Project List Not Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = projetcList;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/newdashboard/getallcomapnyprojects | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        #endregion

        #region Api To Get Year List Of Project Dashboard

        /// <summary>
        /// Created By Ravi Vyas On 12-01-2023
        /// API>>Get>>api/newdashboard/newprojectdashboardyearlist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("newprojectdashboardyearlist")]
        public async Task<ResponseBodyModel> GetLeaveDashboardYearList()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var assignEmployeeRequest = await _db.AssignProjects.Where(x => claims.companyId == x.CompanyId).ToListAsync();
                var checkYear = assignEmployeeRequest
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

        #region Api To Get Month List Of Project Dashboard

        /// <summary>
        /// Created By Ravi Vyas On 12-01-2023
        /// API>>Get>>api/newdashboard/projectdashboardmonthlist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("projectdashboardmonthlist")]
        public async Task<ResponseBodyModel> GetNewProjectDashboardMonthListAsync()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                //var expensesRequest = await _db.ExpenseEntry.Where(x => claims.companyId == x.CompanyId && x.CreatedOn.Year == year)
                //            .Select(x => x.CreatedOn.Month).Distinct().OrderBy(x => x).FirstOrDefaultAsync();
                var months = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthGenitiveNames
                                        .TakeWhile(m => m != String.Empty)
                                        .Select((m, i) => new
                                        {
                                            Month = i + 1,
                                            MonthName = m
                                        }).ToList();
                //months = months.Skip(expensesRequest - 1).ToList();
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

        #region Api for Get All Task In Company Employee
        /// <summary>
        /// API>>api/newdashboard/taskfordashboard
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("taskfordashboard")]
        public async Task<IHttpActionResult> GetAllCompanyTask(DateTimeOffset? dateValue = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {

                DateTimeOffset startingDate = dateValue.HasValue ? (DateTimeOffset)dateValue : TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);

                DayOfWeek day = startingDate.DayOfWeek;
                int days = day - DayOfWeek.Monday;
                DateTimeOffset start = startingDate.AddDays(-days);
                DateTimeOffset end = start.AddDays(6);

                var infoData = await _db.TaskModels.Where(a => a.IsActive && !a.IsDeleted && a.CompanyId == tokenData.companyId && a.StartDate >= start && a.EndDate <= end)
                                .Select(a => new
                                {
                                    ProjectId = a.ProjectId,
                                    ProjecName = _db.ProjectLists.Where(x => x.ID == a.ProjectId).Select(x => x.ProjectName).FirstOrDefault(),
                                    EMployeeName = _db.Employee.Where(e => e.EmployeeId == a.AssignEmployeeId).Select(e => e.DisplayName).FirstOrDefault(),
                                    OrignalHours = _db.TaskModels.Where(x => x.AssignEmployeeId == a.AssignEmployeeId && x.ProjectId == a.ProjectId).Select(x => x.EstimateTime).ToList().Sum(),
                                    EmployeeId = a.AssignEmployeeId,
                                    TaskCount = _db.TaskModels.Count(x => x.AssignEmployeeId == a.AssignEmployeeId && x.ProjectId == a.ProjectId),
                                    Percantage = 0,
                                }).Distinct().ToListAsync();
                var getData = infoData.Select(x => new
                {
                    TaskCount = x.TaskCount,
                    Projectid = x.ProjectId,
                    ProjectName = x.ProjecName,
                    EmployeeName = x.EMployeeName,
                    EmployeeId = x.EmployeeId,
                    OriglHo = x.OrignalHours,
                    OrignalHours = string.Format("{00:00}:{01:00}", (int)x.OrignalHours / 60, x.OrignalHours % 60),
                    Percantage = (40 / x.OrignalHours) * 100,
                }).ToList();

                if (getData.Count > 0)
                {
                    res.Message = "Data Get Succesfuly !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = getData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = getData;
                    return Ok(res);
                }

            }

            catch (Exception ex)
            {


                logger.Error("API : api/newdashboard/taskfordashboard | " +
                    "Week date : " + dateValue + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        #endregion

        #region Helper Model Class

        public class ProjectDashboardResponseModel
        {
            public int TotalTask { get; set; } = 0;
            public int TimeSheetFilled { get; set; } = 0;
            public int TimeSheetNotFilled { get; set; } = 0;
            public int TotalProjectCount { get; set; } = 0;
            public int TotalAssignProject { get; set; } = 0;
            public int TotalProjectNotAssign { get; set; } = 0;
            public int TotalEmployeeCount { get; set; } = 0;
            public int NoOfBillableResource { get; set; } = 0;
            public int NoOfNonBillableResource { get; set; } = 0;
            public int NoOfPartialBillableResource { get; set; } = 0;
            public object label { get; set; }
            public object graph { get; set; }
            public object ResourcePercentage { get; set; }
            public int TotalActiveProject { get; set; }
            public int TotalCompletedProject { get; set; }
            public int TotalHoldProject { get; set; }
            public object ProjectLabel { get; set; }
            public object ProjectGraph { get; set; }

        }

        public class NewProjectDashboardTaskResponse
        {
            public int EmployeeId { get; set; }
            public string DishplayName { get; set; }
            public int ProjectId { get; set; }
            public string ProjectName { get; set; }
            public string WeeklyHours { get; set; }
            public string OrignalHours { get; set; }
            public int Percentage { get; set; }

        }
        #endregion

    }

}
