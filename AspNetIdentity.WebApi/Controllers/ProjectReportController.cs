using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.TimeSheet;
using AspNetIdentity.WebApi.Models;
using EASendMail;
using Newtonsoft.Json;
using NLog;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.UI.WebControls;
using static AspNetIdentity.WebApi.Helper.TaskHelper;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers
{
    /// <summary>
    /// Created By Harshit Mitra On 30-01-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/projectreport")]
    public class ProjectReportController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API TO GET PROJECT LIST IN DROPDOWN
        /// <summary>
        /// Created By Harshit Mitra On 30-01-2023
        /// API >> GET >> api/projectreport/getprojectdropdown
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getprojectdropdown")]
        public async Task<IHttpActionResult> GetProjectDropDown()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var projectList = await _db.ProjectLists
                    .Where(x => x.CompanyId == tokenData.companyId && !x.IsDeleted && x.IsActive)
                    .Select(x => new
                    {
                        ProjectId = x.ID,
                        ProjectName = x.ProjectName,
                    })
                    .ToListAsync();
                if (projectList.Count == 0)
                {
                    res.Message = "No Project Found!";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = projectList;
                    return Ok(res);
                }

                res.Message = "Project Report";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = projectList;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/getprojectdropdown | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET PROJECT RANGE FILTER
        /// <summary>
        /// Created By Harshit Mitra On 30-01-2023
        /// API >> GET >> api/projectreport/getprojectrangefilter
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getprojectrangefilter")]
        public IHttpActionResult GetProjectRangeFilter()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var projectRangeFilter = Enum.GetValues(typeof(ProjectFilter))
                                .Cast<ProjectFilter>()
                                .Select(x => new
                                {
                                    Id = (int)x,
                                    Value = x.ToString().Replace("_", " "),

                                }).ToList();

                res.Message = "Project Range Filter";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = projectRangeFilter;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/getprojectrangefilter | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public enum ProjectFilter
        {
            All = 0,
            Today = 1,
            Week = 2,
            Month = 3,
            Quater = 4,
            Half_Yearly = 5,
            Yearly = 6,
        }
        #endregion

        #region API TO GET PROJECT TASK BILLING FILTER
        /// <summary>
        /// Created By Harshit Mitra On 30-01-2023
        /// API >> GET >> api/projectreport/getprojecttaskbillingfilter
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetProjectTaskBillingFilter")]
        public IHttpActionResult GetProjectTaskBillingFilter()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var projectRangeFilter = Enum.GetValues(typeof(TaskBillingConstants))
                                .Cast<TaskBillingConstants>()
                                .Select(x => new
                                {
                                    Id = (int)x,
                                    Value = x.ToString().Replace("_", " "),

                                }).ToList();

                res.Message = "Project Task Billing";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = projectRangeFilter;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/getprojecttaskbillingfilter | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET PROJECT TASK STATUS FILTER
        /// <summary>
        /// Created By Harshit Mitra On 30-01-2023
        /// API >> GET >> api/projectreport/getprojecttaskstatusfilter
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getprojecttaskstatusfilter")]
        public IHttpActionResult GetProjectTaskStatusFilter()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var projectRangeFilter = Enum.GetValues(typeof(TaskStatusConstants))
                                .Cast<TaskStatusConstants>()
                                .Select(x => new
                                {
                                    Id = (int)x,
                                    Value = x.ToString().Replace("_", " "),

                                }).ToList();

                res.Message = "Project Task Status Filter";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = projectRangeFilter;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/getprojecttaskstatusfilter | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET PROJECT TASK TYPE FILTER
        /// <summary>
        /// Created By Harshit Mitra On 30-01-2023
        /// API >> GET >> api/projectreport/getprojecttasktypefilter
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getprojecttasktypefilter")]
        public IHttpActionResult GetProjectTaskTypeFilter()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var projectRangeFilter = Enum.GetValues(typeof(TaskTypeConstants))
                                .Cast<TaskTypeConstants>()
                                .Select(x => new
                                {
                                    Id = (int)x,
                                    Value = x.ToString().Replace("_", " "),

                                }).ToList();

                res.Message = "Project Task Type Filter";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = projectRangeFilter;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/getprojecttasktypefilter | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET PROJECT TASK PRIORITY FILTER
        /// <summary>
        /// Created By Harshit Mitra On 30-01-2023
        /// API >> GET >> api/projectreport/getprojecttaskpriorityfilter
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getprojecttaskpriorityfilter")]
        public IHttpActionResult GetProjectTaskPriorityFilter()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var projectRangeFilter = Enum.GetValues(typeof(TaskPriorityConstants))
                                .Cast<TaskPriorityConstants>()
                                .Select(x => new
                                {
                                    Id = (int)x,
                                    Value = x.ToString().Replace("_", " "),

                                }).ToList();

                res.Message = "Project Task Type Filter";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = projectRangeFilter;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/getprojecttaskpriorityfilter | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET PROJECT EMPLOYEE LIST 
        /// <summary>
        /// Created By Harshit Mitra On 30-01-2023
        /// API >> GET >> api/projectreport/getemployeelistinproject
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployeelistinproject")]
        public async Task<IHttpActionResult> GetEmployeeListInProjects()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var empInProject = await _db.AssignProjects
                    .Where(x => x.CompanyId == tokenData.companyId && x.IsActive && !x.IsDeleted)
                    .Select(x => x.EmployeeId)
                    .ToListAsync();
                var employeeList = await _db.Employee
                    .Where(x => empInProject.Contains(x.EmployeeId))
                    .Select(x => new
                    {
                        x.EmployeeId,
                        x.DisplayName,
                    })
                    .OrderBy(x => x.DisplayName)
                    .ToListAsync();

                res.Message = "Project Task Type Filter";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = employeeList;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/getemployeelistinproject | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET PROJECT REPORT  
        /// <summary>
        /// Created By Harshit Mitra On 30-01-2023
        /// API >> GET >> api/projectreport/getprojectreport
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getprojectreport")]
        public async Task<IHttpActionResult> GetProjectReport(FilterClassRequest filterModel)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);

                #region WHERE CLAUSE FILTER

                string whereClause = tokenData.companyId.ToString();
                if (filterModel.ProjectId.Count != 0)
                {
                    whereClause += " AND ( p.ID = ";
                    whereClause += String.Join(" OR p.ID = ", filterModel.ProjectId);
                    whereClause += " )";
                }
                if (filterModel.ProjectFilter != ProjectFilter.All)
                {
                    whereClause += " AND (";
                    switch (filterModel.ProjectFilter)
                    {
                        case ProjectFilter.Today:
                            whereClause += "CAST(t.StartDate AS DATE) = \'" + today.Date.ToString("yyyy-MM-dd") + "\'";
                            break;
                        case ProjectFilter.Week:
                            whereClause += "CAST(t.StartDate AS DATE) BETWEEN \'" + today.AddDays(-7).Date.ToString("yyyy-MM-dd")
                                + "\' AND \'"
                                + today.Date.ToString("yyyy-MM-dd") + "\'";
                            break;
                        case ProjectFilter.Month:
                            whereClause += "CAST(t.StartDate AS DATE) BETWEEN \'" + today.AddMonths(-1).Date.ToString("yyyy-MM-dd")
                                + "\' AND \'"
                                + today.Date.ToString("yyyy-MM-dd") + "\'";
                            break;
                        case ProjectFilter.Quater:
                            whereClause += "CAST(t.StartDate AS DATE) BETWEEN \'" + today.AddMonths(-3).Date.ToString("yyyy-MM-dd")
                                + "\' AND \'"
                                + today.Date.ToString("yyyy-MM-dd") + "\'";
                            break;
                        case ProjectFilter.Half_Yearly:
                            whereClause += "CAST(t.StartDate AS DATE) BETWEEN \'" + today.AddMonths(-6).Date.ToString("yyyy-MM-dd")
                                + "\' AND \'"
                                + today.Date.ToString("yyyy-MM-dd") + "\'";
                            break;
                        case ProjectFilter.Yearly:
                            whereClause += "CAST(t.StartDate AS DATE) BETWEEN \'" + today.AddYears(-1).Date.ToString("yyyy-MM-dd")
                                + "\' AND \'"
                                + today.Date.ToString("yyyy-MM-dd") + "\'";
                            break;
                    }
                    whereClause += " )";
                }
                if (filterModel.StartDate.HasValue && filterModel.EndDate.HasValue)
                {
                    whereClause += " AND (";
                    whereClause += "CAST(t.StartDate AS DATE) BETWEEN \'" + filterModel.StartDate.Value.Date.ToString("yyyy-MM-dd")
                                + "\' AND \'"
                                + filterModel.EndDate.Value.Date.ToString("yyyy-MM-dd") + "\'";
                    whereClause += " )";
                }
                if (filterModel.Billing.HasValue)
                {
                    whereClause += " AND (";
                    whereClause += "t.TaskBilling = " + (int)filterModel.Billing.Value;
                    whereClause += " )";
                }
                if (filterModel.Status.Count != 0)
                {
                    whereClause += " AND ( t.Status = ";
                    whereClause += String.Join(" OR t.Status = ", filterModel.Status.Select(x => (int)x).ToList());
                    whereClause += " )";
                }
                if (filterModel.Type.Count != 0)
                {
                    whereClause += " AND ( t.TaskType = ";
                    whereClause += String.Join(" OR t.TaskType = ", filterModel.Type.Select(x => (int)x).ToList());
                    whereClause += " )";
                }
                if (filterModel.Priority.Count != 0)
                {
                    whereClause += " AND ( t.Priority = ";
                    whereClause += String.Join(" OR t.Priority = ", filterModel.Priority.Select(x => (int)x).ToList());
                    whereClause += " )";
                }
                if (filterModel.EmployeeIds.Count != 0)
                {
                    whereClause += " AND ( a.EmployeeId = ";
                    whereClause += String.Join(" OR a.EmployeeId = ", filterModel.EmployeeIds);
                    whereClause += " )";
                }
                if (!String.IsNullOrEmpty(filterModel.SearchString) && filterModel.SearchString.Length > 2)
                {
                    whereClause += " AND ( UPPER(p.ProjectName) Like \'%" + filterModel.SearchString + "%\'";
                    whereClause += " OR UPPER(pe.DisplayName) Like \'%" + filterModel.SearchString + "%\'";
                    whereClause += " OR UPPER(t.TaskTitle) Like \'%" + filterModel.SearchString + "%\'";
                    whereClause += " )";
                }
                #endregion

                List<FilterClassResponse> projectList = await GetProjectReportFilterData(filterModel.Page, filterModel.Count, whereClause);
                long projectListCount = await GetProjectReportFilterDataCount(whereClause);

                if (projectList.Count == 0)
                {
                    res.Message = "Project Report Is Empty";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = new
                    {
                        TotalData = projectListCount,
                        Count = filterModel.Count,
                        List = projectList,
                    };
                    return Ok(res);
                }

                res.Message = "Project Report";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data =
                res.Data = new
                {
                    TotalData = projectListCount,
                    Count = filterModel.Count,
                    List = projectList
                        .Select(x => new
                        {
                            ProjectName = x.ProjectName,
                            EmpoyeeName = x.EmployeeInProjectName,
                            TaskTitle = x.TaskTitle,
                            Billing = x.TaskId != Guid.Empty ? x.Billing.ToString().Replace("_", " ") : String.Empty,
                            Status = x.TaskId != Guid.Empty ? x.Status.ToString().Replace("_", " ") : String.Empty,
                            Type = x.TaskId != Guid.Empty ? x.Type.ToString().Replace("_", " ") : String.Empty,
                            Priority = x.TaskId != Guid.Empty ? x.Priority.ToString().Replace("_", " ") : String.Empty,
                            StartDate = x.TaskId != Guid.Empty ? ((DateTimeOffset)x.TaskStartDate).ToString("dd, MMM yyyy") : String.Empty,
                            EstimateTime = x.TaskId != Guid.Empty ? TimeSpan.FromMinutes(x.EstimateTime).ToString(@"hh\:mm") : String.Empty,
                            SpendTime = x.SpendTime != TimeSpan.Zero ? x.SpendTime.ToString(@"hh\:mm") : String.Empty,
                        })
                        .ToList(),
                };

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/getprojectreport | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class FilterClassRequest
        {
            public int Page { get; set; } = 1;
            public int Count { get; set; } = 10;
            public List<int> ProjectId { get; set; } = new List<int>();
            public ProjectFilter ProjectFilter { get; set; } = ProjectFilter.All;
            public DateTime? StartDate { get; set; } = null;
            public DateTime? EndDate { get; set; } = null;
            public TaskBillingConstants? Billing { get; set; }
            public List<TaskStatusConstants> Status { get; set; } = new List<TaskStatusConstants>();
            public List<TaskTypeConstants> Type { get; set; } = new List<TaskTypeConstants>();
            public List<TaskPriorityConstants> Priority { get; set; } = new List<TaskPriorityConstants>();
            public List<int> EmployeeIds { get; set; } = new List<int>();
            public string SearchString { get; set; } = String.Empty;
        }
        public class FilterClassResponse
        {
            public int ProjectId { get; set; } = 0;
            public string ProjectName { get; set; } = String.Empty;
            public int EmployeeInProjectId { get; set; } = 0;
            public string EmployeeInProjectName { get; set; } = String.Empty;
            public Guid TaskId { get; set; } = Guid.Empty;
            public string TaskTitle { get; set; } = String.Empty;
            public TaskBillingConstants? Billing { get; set; }
            public TaskStatusConstants? Status { get; set; }
            public TaskTypeConstants? Type { get; set; }
            public TaskPriorityConstants? Priority { get; set; }
            public DateTimeOffset? TaskStartDate { get; set; }
            public long EstimateTime { get; set; } = 0;
            public TimeSpan SpendTime { get; set; } = TimeSpan.Zero;
        }
        public async Task<List<FilterClassResponse>> GetProjectReportFilterData(int page, int count, string whereClause)
        {
            try
            {

                List<FilterClassResponse> projectList = new List<FilterClassResponse>();
                var _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                using (var con = new SqlConnection(_connectionString.ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SP_GetProjectReportPaginationFilter", con);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@page", SqlDbType.NVarChar).Value = page.ToString();
                    cmd.Parameters.Add("@count", SqlDbType.NVarChar).Value = count.ToString();
                    cmd.Parameters.Add("@whereClause", SqlDbType.NVarChar).Value = whereClause;

                    con.Open();
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    while (rdr.Read())
                    {
                        projectList.Add(new FilterClassResponse
                        {
                            ProjectId = Convert.ToInt32(rdr[0]),
                            ProjectName = (rdr[1]).ToString(),
                            EmployeeInProjectId = Convert.ToInt32(rdr[2]),
                            EmployeeInProjectName = rdr[3].ToString(),
                            TaskId = !rdr.IsDBNull(4) ? (Guid)rdr[4] : Guid.Empty,
                            TaskTitle = !rdr.IsDBNull(5) ? rdr[5].ToString() : String.Empty,
                            Billing = !rdr.IsDBNull(6) ? (TaskBillingConstants)Convert.ToInt32(rdr[6]) : 0,
                            Status = !rdr.IsDBNull(7) ? (TaskStatusConstants)Convert.ToInt32(rdr[7]) : 0,
                            Type = !rdr.IsDBNull(8) ? (TaskTypeConstants)Convert.ToInt32(rdr[8]) : 0,
                            Priority = !rdr.IsDBNull(9) ? (TaskPriorityConstants)Convert.ToInt32(rdr[9]) : 0,
                            TaskStartDate = !rdr.IsDBNull(10) ? (DateTimeOffset?)rdr[10] : null,
                            EstimateTime = !rdr.IsDBNull(11) ? Convert.ToInt64(rdr[11]) : 0,
                            SpendTime = !rdr.IsDBNull(12) ? (TimeSpan)rdr[12] : TimeSpan.Zero,
                        });
                    }
                    con.Close();
                }
                return projectList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<long> GetProjectReportFilterDataCount(string whereClause)
        {
            long returnValue = 0;
            var _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            using (var con = new SqlConnection(_connectionString.ToString()))
            {
                SqlCommand cmd = new SqlCommand("SP_GetCountOfTotalProjectReportWithFilter", con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@whereClause", SqlDbType.NVarChar).Value = whereClause;

                var returnParameter = cmd.Parameters.Add("@TotalCount", SqlDbType.BigInt);
                returnParameter.Direction = ParameterDirection.ReturnValue;
                con.Open();
                await cmd.ExecuteNonQueryAsync();
                con.Close();
                returnValue = Convert.ToInt64(returnParameter.Value);
            }
            return returnValue;
        }
        #endregion

        #region API TO GET PROJECT REPORT COLOUR CODE FILTER 
        /// <summary>
        /// Created By Harshit Mitra On 30-01-2023
        /// API >> GET >> api/projectreport/getprojectcolorrangefilter
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getprojectcolorrangefilter")]
        public IHttpActionResult GetProjectReportColourRangeFilter()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var projectRangeFilter = Enum.GetValues(typeof(ColorCodeRange))
                                .Cast<ColorCodeRange>()
                                .Select(x => new
                                {
                                    Id = (int)x,
                                    Value = x.ToString().Replace("_", " ").Replace("COLLON", ":"),

                                }).ToList();

                res.Message = "Project Task Type Filter";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = projectRangeFilter;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/getprojecttaskpriorityfilter | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public enum ColorCodeRange
        {
            Not_Filled_COLLON_RED = 0,
            Incomplete_Filled_COLLON_YELLOW = 1,
            Completly_Filled_COLLON_GREEN = 2,
            Working_Over_Time_COLLON_BLUE = 3,
        }
        #endregion

        #region API TO GET TIME RANGE FILTER OF NEW PROJECT REPORT COLOR CODE
        /// <summary>
        /// Created By Harshit Mitra On 30-01-2023
        /// API >> GET >> api/projectreport/timerangefiltercolorcode
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("timerangefiltercolorcode")]
        public IHttpActionResult GerTimeRangeFilterColorCode()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var projectRangeFilter = Enum.GetValues(typeof(TimeRangeFilter))
                                .Cast<TimeRangeFilter>()

                                .Select(x => new
                                {
                                    Id = (int)x,
                                    Value = x.ToString().Replace("_", " "),

                                }).ToList();

                res.Message = "Project Task Type Filter";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = projectRangeFilter;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/timerangefiltercolorcode | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET NEW PROJECT REPORT WITH COLOR CODE
        /// <summary>
        /// Created By Harshit Mitra On 17-02-2023
        /// API >> GET >> api/projectreport/getprojectreportcolorcode
        /// </summary>
        /// <param name="filterModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getprojectreportcolorcode")]
        public async Task<IHttpActionResult> GetProjectReportColorCode(ProjectReportColourFilter filterModel)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                TimeSpan checkTime = TimeSpan.Zero;
                DateTime checkDate = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone).Date;
                DateTimeOffset endDate = checkDate.AddDays(-1);
                DateTimeOffset startDate = endDate;
                switch (filterModel.TimeFilter)
                {
                    case TimeRangeFilter.Yesterday:
                        startDate = endDate;
                        checkTime = new TimeSpan(8, 0, 0);
                        break;
                    case TimeRangeFilter.Weekly:
                        startDate = endDate.AddDays(-7);
                        checkTime = new TimeSpan((8 * 7), 0, 0);
                        break;
                    case TimeRangeFilter.Monthly:
                        startDate = endDate.AddMonths(-1);
                        checkTime = new TimeSpan((8 * (int)(endDate - startDate).TotalDays), 0, 0);
                        break;
                    case TimeRangeFilter.Custom:
                        startDate = filterModel.StartDate.Date;
                        endDate = filterModel.EndDate.Date;
                        break;
                }
                string stringSearch = filterModel.SearchString.Length > 2 ? filterModel.SearchString : null;
                List<ProjectReportSPResponse> projectList = await GetProjectReportColourCodeSP(
                    companyId: tokenData.companyId,
                    start: startDate.Date,
                    end: endDate.Date,
                    checkStatus: checkTime,
                    stringSearch: stringSearch,
                    employeeIds: filterModel.EmployeeId.Count == 0 ? null : String.Join(",", filterModel.EmployeeId),
                    checkDate: checkDate,
                    orgIds: (filterModel.OrgIds.Count == 0 ? null : String.Join(",", filterModel.OrgIds)),
                    taskBillingType: filterModel.TaskBillingType
                    );

                if (projectList.Count == 0)
                {
                    res.Message = "Project Report Is Empty";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = projectList;
                    //res.Data = new {
                    //    TotalData = projectList.Count(),
                    //    Count = filterModel.Count,
                    //    List = projectList,
                    //};
                    return Ok(res);
                }

                if (filterModel.ColorFilter.HasValue)
                    projectList = projectList.Where(x => x.ColorFilter == filterModel.ColorFilter.Value).ToList();
                var min = TimeConvertHelper.ConvertStringToTimeSpan(filterModel.MinRange);
                var max = TimeConvertHelper.ConvertStringToTimeSpan(filterModel.MaxRange);
                if (min != TimeSpan.Zero && max != TimeSpan.Zero)
                    if (max >= min)
                        projectList = projectList.Where(x => x.SpendTime >= min && x.SpendTime <= max).ToList();

                res.Message = "Project Report";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = projectList;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/getprojectreportcolorcode | " +
                    "Filter Model : " + JsonConvert.SerializeObject(filterModel) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public async Task<List<ProjectReportSPResponse>> GetProjectReportColourCodeSP(int companyId, DateTime start, DateTime end,
            TimeSpan checkStatus, string stringSearch, string employeeIds, DateTime checkDate, string @orgIds, int? @taskBillingType)
        {
            List<ProjectReportSPResponse> projectList = new List<ProjectReportSPResponse>();
            try
            {
                var _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                using (var con = new SqlConnection(_connectionString.ToString()))
                {
                    var eighty = TimeSpan.FromMinutes(checkStatus.TotalMinutes * 4 / 5);
                    var hundred = TimeSpan.FromMinutes(checkStatus.TotalMinutes);
                    SqlCommand cmd = new SqlCommand("SP_ProjectReportColorCode", con);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@companyId", SqlDbType.Int).Value = companyId;
                    cmd.Parameters.Add("@start", SqlDbType.DateTime).Value = start;
                    cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    cmd.Parameters.Add("@stringSearch", SqlDbType.NVarChar).Value = stringSearch;
                    cmd.Parameters.Add("@employeeIds", SqlDbType.NVarChar).Value = employeeIds;
                    cmd.Parameters.Add("@orgIds", SqlDbType.NVarChar).Value = @orgIds;
                    cmd.Parameters.Add("@taskBillingType", SqlDbType.Int).Value = @taskBillingType;

                    con.Open();
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    while (rdr.Read())
                    {
                        projectList.Add(new ProjectReportSPResponse
                        {
                            IsCheck = false,
                            EmployeeId = Convert.ToInt32(rdr["EmployeeId"]),
                            DisplayName = (rdr["DisplayName"]).ToString(),
                            TotalLogTime = String.Format("{0:00}:{1:00}", Convert.ToDouble(rdr["TotalHours"]), Convert.ToDouble(rdr["TotalMinutes"])),
                            TotalTime = String.Format("{0:00}:{1:00}", checkStatus.TotalHours, checkStatus.TotalMinutes % 60),
                            SpendTime = new TimeSpan(Convert.ToInt32(rdr["TotalHours"]), Convert.ToInt32(rdr["TotalMinutes"]), 0),
                            TaskHasFiled = !(eighty > new TimeSpan(Convert.ToInt32(rdr["TotalHours"]), Convert.ToInt32(rdr["TotalMinutes"]), 0)),
                            ColourCode = (TimeSpan.Zero == new TimeSpan(Convert.ToInt32(rdr["TotalHours"]), Convert.ToInt32(rdr["TotalMinutes"]), 0)) ?
                                "RED" : (eighty > new TimeSpan(Convert.ToInt32(rdr["TotalHours"]), Convert.ToInt32(rdr["TotalMinutes"]), 0)) ?
                                "YELLOW" : (hundred < new TimeSpan(Convert.ToInt32(rdr["TotalHours"]), Convert.ToInt32(rdr["TotalMinutes"]), 0)) ? "BLUE" : "GREEN",
                            ColorFilter = (TimeSpan.Zero == new TimeSpan(Convert.ToInt32(rdr["TotalHours"]), Convert.ToInt32(rdr["TotalMinutes"]), 0)) ?
                                ColorCodeRange.Not_Filled_COLLON_RED : (eighty > new TimeSpan(Convert.ToInt32(rdr["TotalHours"]), Convert.ToInt32(rdr["TotalMinutes"]), 0)) ?
                                ColorCodeRange.Incomplete_Filled_COLLON_YELLOW :
                                (hundred < new TimeSpan(Convert.ToInt32(rdr["TotalHours"]), Convert.ToInt32(rdr["TotalMinutes"]), 0)) ? ColorCodeRange.Working_Over_Time_COLLON_BLUE
                                : ColorCodeRange.Completly_Filled_COLLON_GREEN,
                            ApproveTime = new TimeSpan(Convert.ToInt32(rdr["ApproveHours"]), Convert.ToInt32(rdr["ApproveMinutes"]), 0),
                            ApproveTimeString = String.Format("{0:00}:{1:00}", Convert.ToDouble(rdr["ApproveHours"]), Convert.ToDouble(rdr["ApproveMinutes"])),
                            EmployeeEmail = rdr["EmployeeEmail"].ToString(),
                            //IsReminderSent = (Convert.ToDateTime(rdr["LastDate"]).Date == checkDate.Date),
                            IsReminderSent = false,
                        });
                    }
                    con.Close();
                }
                return projectList;
            }
            catch (Exception)
            {
                return projectList;
            }
        }
        public class ProjectReportSPResponse
        {
            public bool IsCheck { get; set; } = false;
            public int EmployeeId { get; set; } = 0;
            public string DisplayName { get; set; } = String.Empty;
            public string EmployeeEmail { get; set; } = String.Empty;
            public string TotalLogTime { get; set; } = String.Empty;
            public string TotalTime { get; set; } = String.Empty;
            public TimeSpan SpendTime { get; set; } = TimeSpan.Zero;
            public TimeSpan ApproveTime { get; set; } = TimeSpan.Zero;
            public string ApproveTimeString { get; set; } = String.Empty;
            public bool TaskHasFiled { get; set; } = false;
            public string ColourCode { get; set; } = String.Empty;
            public ColorCodeRange ColorFilter { get; set; } = ColorCodeRange.Not_Filled_COLLON_RED;
            public bool IsReminderSent { get; set; } = false;
        }
        public class ProjectReportColourFilter
        {
            public int Page { get; set; } = 1;
            public int Count { get; set; } = 10;
            public string SearchString { get; set; } = String.Empty;
            public List<int> EmployeeId { get; set; } = new List<int>();
            public TimeRangeFilter TimeFilter { get; set; } = TimeRangeFilter.Yesterday;
            public ColorCodeRange? ColorFilter { get; set; }
            public string MinRange { get; set; } = String.Empty;
            public string MaxRange { get; set; } = String.Empty;
            public DateTime StartDate { get; set; } = DateTime.UtcNow;
            public DateTime EndDate { get; set; } = DateTime.UtcNow;
            public List<int> OrgIds { get; set; } = new List<int>();
            public int? TaskBillingType { get; set; } = null;
        }
        public enum TimeRangeFilter
        {
            Yesterday = 0,
            Weekly = 1,
            Monthly = 2,
            Custom = 3,
        }
        #endregion

        #region API TO SEND MAIL TO EMPLOYEE
        /// <summary>
        /// Created By Harshit Mitra On 21-02-2023
        /// API >> POST >> api/projectreport/sendmailtoemployee
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("sendmailtoemployee")]
        public async Task<IHttpActionResult> SendMailToEmployee(List<SendMailToEmplyeeRequest> model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                model = model
                    .Where(x => x.ColorFilter == ColorCodeRange.Incomplete_Filled_COLLON_YELLOW
                        || x.ColorFilter == ColorCodeRange.Not_Filled_COLLON_RED)
                    .Where(x => x.IsCheck)
                    .ToList();
                if (!model
                    .Any(x => x.ColorFilter == ColorCodeRange.Incomplete_Filled_COLLON_YELLOW
                        || x.ColorFilter == ColorCodeRange.Not_Filled_COLLON_RED))
                {
                    res.Message = "There Is No RED OR YELLOW Flag in Selected Employee";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    return Ok(res);
                }
                var companyData = await _db.Company.FirstOrDefaultAsync(y => y.CompanyId == tokenData.companyId);

                HostingEnvironment.QueueBackgroundWorkItem(ct => SendMailThread(companyData, model, tokenData.employeeId));

                res.Message = "Mail Send to " + model.Count + " People";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = "";
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/sendmailtoemployee | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class SendMailToEmplyeeRequest
        {
            public bool IsCheck { get; set; } = false;
            public int EmployeeId { get; set; } = 0;
            public string DisplayName { get; set; } = String.Empty;
            public string EmployeeEmail { get; set; } = String.Empty;
            public ColorCodeRange ColorFilter { get; set; }
        }

        public async Task SendMailThread(Company companyData, List<SendMailToEmplyeeRequest> model, int mailSendBy)
        {
            Thread.Sleep(1000);
            await SendMail(companyData, model, mailSendBy);
        }
        public async Task SendMail(Company companyData, List<SendMailToEmplyeeRequest> model, int mailSendBy)
        {
            try
            {
                var todayDate = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, companyData.CompanyDefaultTimeZone).Date;
                List<int> employeesIds = new List<int>();
                foreach (var item in model)
                {
                    string attachmentPath = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, "uploadimage\\MailImages");
                    string noTaskCreateBody = ProjectReportMailHelper.TaskNotCreatedMail;
                    string taskNotProperlyFillBody = ProjectReportMailHelper.TimeSheetNotFilledProperly;
                    string htmlBody = EmailHelperClass.DefaultMailBody
                          .Replace("<|IMAGE_PATH|>", "emossy.png")
                          .Replace("<|COMPANYNAMEE|>", companyData.RegisterCompanyName)
                          .Replace("<|COMPANYADDRESS|>", companyData.RegisterAddress);
                    SmtpMail oMail = new SmtpMail("TryIt");
                    oMail.From = ConfigurationManager.AppSettings["MasterEmail"];
                    string innerBody = String.Empty;
                    switch (item.ColorFilter)
                    {
                        case ColorCodeRange.Not_Filled_COLLON_RED:
                            oMail.Subject = "Task Detail Missing";
                            oMail.To = item.EmployeeEmail;
                            innerBody = noTaskCreateBody
                                .Replace("<|EMPLOYEENAME|>", item.DisplayName);
                            htmlBody = htmlBody
                                .Replace("<|MAIL_TITLE|>", "Task Detail Missing")
                                .Replace("<|INNER_BODY|>", innerBody);
                            oMail.ImportHtml(htmlBody, attachmentPath, ImportHtmlBodyOptions.ImportLocalPictures | ImportHtmlBodyOptions.ImportCss);
                            break;
                        case ColorCodeRange.Incomplete_Filled_COLLON_YELLOW:
                            oMail.Subject = "Task Created Not Fill Properly";
                            oMail.To = item.EmployeeEmail;
                            innerBody = taskNotProperlyFillBody
                                .Replace("<|EMPLOYEENAME|>", item.DisplayName);
                            htmlBody = htmlBody
                                .Replace("<|MAIL_TITLE|>", "Task Created Not Fill Properly")
                                .Replace("<|INNER_BODY|>", innerBody);
                            oMail.ImportHtml(htmlBody, attachmentPath, ImportHtmlBodyOptions.ImportLocalPictures | ImportHtmlBodyOptions.ImportCss);
                            break;
                    }
                    SmtpServer oServer = new SmtpServer("smtp.office365.com");
                    oServer.User = ConfigurationManager.AppSettings["MailUser"];
                    oServer.Password = ConfigurationManager.AppSettings["MailPassword"];
                    oServer.Port = 587;
                    oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                    SmtpClient oSmtp = new SmtpClient();
                    await oSmtp.SendMailAsync(oServer, oMail);
                    employeesIds.Add(item.EmployeeId);
                }
                await UpdateRemiender(employeesIds, mailSendBy, todayDate);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task UpdateRemiender(List<int> employeeIds, int reminderBy, DateTime lastDate)
        {
            var checkRemiender = await _db.TaskReminder
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .ToListAsync();
            foreach (var employeeId in employeeIds)
            {
                var check = checkRemiender.FirstOrDefault(x => x.EmployeeId == employeeId);
                if (check == null)
                {
                    EmployeeTaskReminderHistory obj = new EmployeeTaskReminderHistory
                    {
                        EmployeeId = employeeId,
                        ReminderSendBy = reminderBy,
                        LastDate = lastDate.Date,
                    };
                    _db.TaskReminder.Add(obj);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    check.LastDate = lastDate;
                    check.ReminderSendBy = reminderBy;
                    _db.Entry(check).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                }
            }
        }
        #endregion

        #region API TO GET PROJECT BY EMPLOYEE ID
        /// <summary>
        /// Created By Harshit Mitra On 22-02-2023
        /// API >> GET >> api/projectreport/getprojectbyemployeeid
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getprojectbyemployeeid")]
        public async Task<IHttpActionResult> GetProjectByEmployeeId(int employeeId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var projectList = await (from e in _db.AssignProjects
                                         join p in _db.ProjectLists on e.ProjectId equals p.ID
                                         where e.EmployeeId == employeeId && e.IsActive && !e.IsDeleted
                                         select new
                                         {
                                             ProjectId = p.ID,
                                             ProjectName = p.ProjectName,
                                         })
                                         .Distinct()
                                         .OrderBy(x => x.ProjectName)
                                         .ToListAsync();

                if (projectList.Count == 0)
                {
                    res.Message = "Employee Not Assign To Any Project";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = projectList;
                }

                res.Message = "Project List Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = projectList;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/timerangefiltercolorcode | " +
                    "Employee Id: " + employeeId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET EMPLOYEE DETAILS ON POJECT REPORT COLOR CODE 
        /// <summary>
        /// Created By Harshit Mitra On 30-01-2023
        /// API >> POST >> api/projectreport/empdetailreport
        /// </summary>
        /// <param name="filterModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("empdetailreport")]
        public async Task<IHttpActionResult> GetEmployeeDetailsOnProjectReport(EmployeeDetailsReportRequest filterModel)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                DateTimeOffset endDate = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone).AddDays(-1);
                DateTimeOffset startDate = endDate;
                switch (filterModel.TimeRangeFilter)
                {
                    case TimeRangeFilter.Yesterday:
                        startDate = endDate;
                        break;
                    case TimeRangeFilter.Weekly:
                        startDate = endDate.AddDays(-7);
                        break;
                    case TimeRangeFilter.Monthly:
                        startDate = endDate.AddMonths(-1);
                        break;
                }
                var employeeDetails = await GetEmployeeReportDetails(
                    tokenData.companyId,
                    filterModel.EmployeeId,
                    start: startDate.Date,
                    end: endDate.Date,
                    filterModel.ProjectIds.Count == 0 ?
                        null : String.Join(",", filterModel.ProjectIds)
                );

                if (employeeDetails.Count == 0)
                {
                    res.Message = "No Task Created";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = new
                    {
                        TotalData = employeeDetails.Count,
                        Count = filterModel.Count,
                        List = employeeDetails.ToPagedList(filterModel.Page, filterModel.Count),
                    };
                    return Ok(res);
                }
                res.Message = "Project Task Type Filter";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = new
                {
                    TotalData = employeeDetails.Count,
                    Count = filterModel.Count,
                    List = employeeDetails.ToPagedList(filterModel.Page, filterModel.Count),
                };
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/timerangefiltercolorcode | " +
                    "Filter Model : " + JsonConvert.SerializeObject(filterModel) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class EmployeeDetailsReportRequest
        {
            public int Page { get; set; } = 1;
            public int Count { get; set; } = 10;
            public int EmployeeId { get; set; } = 0;
            public List<int> ProjectIds { get; set; } = new List<int>();
            public TimeRangeFilter TimeRangeFilter { get; set; } = TimeRangeFilter.Yesterday;
        }
        public class EmployeeDetailReportResponse
        {
            public int ProjectId { get; set; } = 0;
            public string ProjectName { get; set; } = String.Empty;
            public Guid TaskId { get; set; } = Guid.Empty;
            public string TaskTitle { get; set; } = String.Empty;
            public DateTime StartDate { get; set; } = DateTime.MinValue;
            public DateTime EndDate { get; set; } = DateTime.MinValue;
            public bool TimesheetFill { get; set; } = false;
            public TimeSpan SpentTime { get; set; } = TimeSpan.Zero;
            public string Discription { get; set; } = String.Empty;
            public int TaskIdShow { get; set; } = 0;
        }
        public async Task<List<EmployeeDetailReportResponse>> GetEmployeeReportDetails(int companyId, int employeeId, DateTime start, DateTime end, string projectIds)
        {
            List<EmployeeDetailReportResponse> detailList = new List<EmployeeDetailReportResponse>();
            try
            {
                var _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                using (var con = new SqlConnection(_connectionString.ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SP_GetEmployeeDetails", con);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@companyId", SqlDbType.Int).Value = companyId;
                    cmd.Parameters.Add("@employeeId", SqlDbType.Int).Value = employeeId;
                    cmd.Parameters.Add("@start", SqlDbType.DateTime).Value = start;
                    cmd.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
                    cmd.Parameters.Add("@projectsIds", SqlDbType.NVarChar).Value = projectIds;

                    con.Open();
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    while (rdr.Read())
                    {
                        detailList.Add(new EmployeeDetailReportResponse
                        {
                            ProjectId = Convert.ToInt32(rdr[0]),
                            ProjectName = Convert.ToString(rdr[1]),
                            TaskId = Guid.Parse(rdr[2].ToString()),
                            TaskTitle = Convert.ToString(rdr[3]),
                            StartDate = Convert.ToDateTime(rdr[4]),
                            EndDate = Convert.ToDateTime(rdr[5]),
                            TimesheetFill = (TimeSpan.Parse(rdr[6].ToString()) != TimeSpan.Zero),
                            SpentTime = TimeSpan.Parse(rdr[6].ToString()),
                            Discription = rdr[7].ToString(),
                            TaskIdShow = Convert.ToInt32(rdr[8]),
                        });
                    }
                    con.Close();
                }
                return detailList;
            }
            catch (Exception)
            {
                return detailList;
            }
        }
        #endregion

        // ----------------------------- NEW REPORT -----------------------//

        //#region API TO GET NEW PROJECT REPORTS
        ///// <summary>
        ///// Created By Harshit Mitra On 30-01-2023
        ///// API >> GET >> api/projectreport/getnewprojectreports
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("getnewprojectreports")]
        //public async Task<IHttpActionResult> GetNewProjectReports()
        //{
        //    ResponseStatusCode res = new ResponseStatusCode();
        //    var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {


        //        res.Message = "Project Reports";
        //        res.Status = true;
        //        res.StatusCode = HttpStatusCode.OK;
        //        res.Data = "";
        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("API : api/projectreport/getnewprojectreports | " +
        //            //"Model : " + JsonConvert.SerializeObject(model) + " | " +
        //            "Exception : " + JsonConvert.SerializeObject(ex));
        //        return BadRequest("Failed");
        //    }
        //}
        //#endregion

        #region API TO GET YEAR LIST IN PROJECT DASHBOARD 
        /// <summary>
        /// Created By Harshit Mitra On 02-02-2023
        /// API >> GET >> api/projectreport/getyearlistinprojectdashboard
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getyearlistinprojectdashboard")]
        public async Task<IHttpActionResult> GetYearListInProjectDashboard()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var yearList = await _db.ProjectLists
                    .Where(x => x.CompanyId == tokenData.companyId)
                    .Select(x => new
                    {
                        Id = x.CreatedOn.Year,
                        Value = x.CreatedOn.Year.ToString(),
                    })
                    .OrderByDescending(x => x.Id)
                    .Distinct()
                    .ToListAsync();

                res.Message = "Year List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = yearList;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/getyearlistinprojectdashboard | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET MONTH LIST IN PROJECT DASHBOARD 
        /// <summary>
        /// Created By Harshit Mitra On 02-02-2023
        /// API >> GET >> api/projectreport/getmonthlistinprojectdashboard
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getmonthlistinprojectdashboard")]
        public async Task<IHttpActionResult> GetMonthListInProjectDashboard(int year)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var monthData = await _db.ProjectLists
                    .Where(x => x.CompanyId == tokenData.companyId && x.CreatedOn.Year == year)
                    .Select(x => x.CreatedOn.Month)
                    .Distinct()
                    .ToListAsync();
                int month = monthData.Count == 0 ? 1 : monthData.Min();
                List<dynamic> monthList = new List<dynamic>();
                if (year != DateTime.Now.Year)
                {
                    for (int i = month; i != 13; i++)
                    {
                        monthList.Add(new
                        {
                            Id = i,
                            Value = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                        });
                    }
                }
                else
                {
                    var checkValue = DateTime.Today.Month + 1;
                    for (int i = month; i != checkValue; i++)
                    {
                        monthList.Add(new
                        {
                            Id = i,
                            Value = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                        });
                    }
                }

                res.Message = "Year List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = monthList
                    .OrderBy(x => x.Id)
                    .ToList();
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/getmonthlistinprojectdashboard | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET TIME RANGE LIST IN PROJECT DASHBOARD
        /// <summary>
        /// Created By Harshit Mitra On 02-02-2023
        /// API >> GET >> api/projectreport/dashboarddatedropdown
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("dashboarddatedropdown")]
        public IHttpActionResult DashboardDropdown()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var projectRangeFilter = Enum.GetValues(typeof(ProjectFilter))
                                .Cast<ProjectFilter>()
                                .Where(x => x != ProjectFilter.All)
                                .Select(x => new
                                {
                                    Id = (int)x,
                                    Value = x.ToString().Replace("_", " "),

                                }).ToList();

                res.Message = "Project Date Filter";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = projectRangeFilter;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/dashboarddatedropdown | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET PROJECT DASHBOARD 
        /// <summary>
        /// Created By Harshit Mitra On 02-02-2023
        /// API >> GET >> api/projectreport/getprojectdashboard
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getprojectdashboard")]
        public async Task<IHttpActionResult> GetProjectDashBoard(ProjectDashboardFilter modelFilter)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var project = await _db.ProjectLists
                    .Where(x => x.CompanyId == tokenData.companyId && x.IsActive &&
                        !x.IsDeleted)
                    .ToListAsync();
                var projectIds = project.Select(x => x.ID).Distinct().ToList();
                var employeeData = await (from e in _db.Employee
                                          join ae in _db.AssignProjects on e.EmployeeId equals ae.EmployeeId into empty
                                          from q in empty.DefaultIfEmpty()
                                          where e.CompanyId == tokenData.companyId && e.IsActive && !e.IsDeleted
                                          select new
                                          {
                                              e.EmployeeId,
                                              e.DisplayName,
                                              BillibleType = q == null ? String.Empty : q.Status.ToUpper(),
                                              ProjectId = q == null ? 0 : q.ProjectId,
                                              IsAssignToProject = q == null ? 0 : q.EmployeeId,
                                          })
                                          .ToListAsync();
                List<TimeSheetData> timesheetData;
                timesheetData = await (from t in _db.TaskModels
                                       join q in _db.TaskLogs on t.TaskId equals q.TaskId into empty
                                       from l in empty.DefaultIfEmpty()
                                       where t.IsActive && !t.IsDeleted && l.IsActive && !l.IsDeleted
                                            && projectIds.Contains(t.ProjectId) && t.TaskType != TaskTypeConstants.BackLog
                                       select new TimeSheetData
                                       {
                                           TaskId = t.TaskId,
                                           TaskTitle = t.TaskTitle,
                                           EstimateTime = t.EstimateTime,
                                           TaskAssignId = (int)t.AssignEmployeeId,
                                           SpendTime = l != null ? l.SpentTime : TimeSpan.Zero,
                                           IsTaskIsFilled = (l != null),
                                           StartDate = (DateTimeOffset)t.StartDate,
                                           EndDate = (DateTimeOffset)t.EndDate,
                                           TaskBilling = t.TaskBilling,
                                       })
                                       .ToListAsync();
                if (modelFilter.Year.HasValue)
                {
                    project = project.Where(x => x.CreatedOn.Year == modelFilter.Year.Value).ToList();
                    timesheetData = timesheetData.Where(x => x.StartDate.Year == modelFilter.Year).ToList();
                    if (modelFilter.Month.HasValue)
                    {
                        project = project.Where(x => x.CreatedOn.Month == modelFilter.Month.Value).ToList();
                        timesheetData = timesheetData.Where(x => x.StartDate.Month == modelFilter.Month).ToList();
                    }
                }

                #region BOX DATA 
                var boxData = new
                {
                    TotalEmployeeTaskBox = new
                    {
                        TotalEmployeeInTask = timesheetData.Select(x => x.TaskAssignId).Distinct().LongCount(),
                        TotalFilledTask = timesheetData.Where(x => x.IsTaskIsFilled).Select(x => x.TaskAssignId).Distinct().LongCount(),
                    },
                    TotalProjectBox = new
                    {
                        TotalProjecCount = project.LongCount(),
                        ClosedProjectCount = project.LongCount(x => x.ProjectStatus == ProjectStatusConstants.Closed),
                    },
                    //BillableResourceCount = employeeData.LongCount(x => x.BillibleType == "BILLABLE"),
                    //NoBillableResourceCount = employeeData.LongCount(x => x.BillibleType == "NON-BILLABLE"),
                    //PartialBillableResourceCount = employeeData.LongCount(x => x.BillibleType == "PARTIAL-BILLABLE"),
                    //TimeSheetBox = new
                    //{
                    //    TotalTimeSheetTask = timesheetData.LongCount(),
                    //    TimeSheetFilled = timesheetData.LongCount() == 0 ? 0 : timesheetData.LongCount(x => x.SpendTime != TimeSpan.Zero),
                    //    TimeSheetNotFilled = timesheetData.LongCount() == 0 ? 0 : timesheetData.LongCount(x => x.SpendTime == TimeSpan.Zero),
                    //},
                    //ProjectBox = new
                    //{
                    //    TotalProject = project.LongCount(),
                    //    ProjectAssigned = project.LongCount(x => x.ProjectStatus == Model.EnumClass.ProjectStatusConstants.Live),
                    //    ProjectNotAssigned = project.LongCount(x => x.ProjectStatus != Model.EnumClass.ProjectStatusConstants.Live),
                    //},
                    //EmployeeBox = new
                    //{
                    //    TotalEmployee = employeeData.Select(x => x.EmployeeId).Distinct().Count(),
                    //    TotalEmployeeInProject = employeeData.Where(x => x.ProjectId != 0).Select(x => x.EmployeeId).Distinct().Count(),
                    //    TotalEmployeeNotInProject = employeeData.Where(x => x.ProjectId == 0).Select(x => x.EmployeeId).Distinct().Count(),
                    //},
                    //TotalActiveProject = project.LongCount(x => x.ProjectStatus == ProjectStatusConstants.Live),
                    //TotalCompletedProject = project.LongCount(x => x.ProjectStatus == ProjectStatusConstants.Closed),
                    //TotalHoldProject = project.LongCount(x => x.ProjectStatus == ProjectStatusConstants.Hold),
                };
                #endregion

                //#region MULTI GRAPH DATA 

                //var monthsData = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthGenitiveNames
                //                         .TakeWhile(m => m != String.Empty)
                //                         .Select((m, i) => new
                //                         {
                //                             Month = i + 1,
                //                             MonthName = m
                //                         }).ToList();
                //var monthNamesData = monthsData.ConvertAll(x => x.MonthName);

                //var listDynamicList = new List<dynamic>();
                //var intListData1 = new List<double>();
                //var dataList1 = new
                //{
                //    Label = "Billable Task",
                //    Stack = "1",
                //    Data = intListData1,
                //};
                //var intListData2 = new List<double>();
                //var dataList2 = new
                //{
                //    Label = "Non Billable Task",
                //    Stack = "2",
                //    Data = intListData2,
                //};

                //foreach (var item in monthsData)
                //{
                //    intListData1.Add(timesheetData.Where(x => x.StartDate.Value.Month == item.Month && x.TaskBilling == TaskBillingConstants.Billable && x.StartDate.Value.Year == year).Count());
                //    intListData2.Add(timesheetData.Where(x => x.StartDate.Value.Month == item.Month && x.TaskBilling == TaskBillingConstants.Non_Billable && x.StartDate.Value.Year == year).Count());
                //}
                //listDynamicList.Add(dataList1);
                //listDynamicList.Add(dataList2);

                //var barChartResponse = new
                //{
                //    Label = monthNamesData,
                //    Graph = listDynamicList,
                //};

                //#endregion

                //#region PIE CHART DATA

                //var pieChartresponse = new List<dynamic>
                //{
                //    new
                //    {
                //        Name = "Billable Task",
                //        Value = timesheetData.Where(x => x.StartDate.Value.Month == month && x.TaskBilling == TaskBillingConstants.Billable && x.StartDate.Value.Year == year).Count(),
                //    },
                //    new
                //    {
                //        Name = "Non Billable Task",
                //        Value = timesheetData.Where(x => x.StartDate.Value.Month == month && x.TaskBilling == TaskBillingConstants.Non_Billable && x.StartDate.Value.Year == year).Count(),
                //    },
                //};

                //#endregion

                //#region LINE CHART DATA

                //var listDynamicList2 = new List<dynamic>();
                //var intListData3 = new List<double>();
                //var dataList3 = new
                //{
                //    Label = "Active Project",
                //    Stack = "1",
                //    Data = intListData3,
                //};
                //var intListData4 = new List<double>();
                //var dataList4 = new
                //{
                //    Label = "Completed Project",
                //    Stack = "2",
                //    Data = intListData4,
                //};
                //var intListData5 = new List<double>();
                //var dataList5 = new
                //{
                //    Label = "Hold Project",
                //    Stack = "3",
                //    Data = intListData5,
                //};

                //foreach (var item in monthsData)
                //{
                //    intListData3.Add(project.LongCount(x => x.ProjectStatus == ProjectStatusConstants.Live && x.CreatedOn.Year == year && x.CreatedOn.Month == item.Month));
                //    intListData4.Add(project.LongCount(x => x.ProjectStatus == ProjectStatusConstants.Closed && x.CreatedOn.Year == year && x.CreatedOn.Month == item.Month));
                //    intListData5.Add(project.LongCount(x => x.ProjectStatus == ProjectStatusConstants.Hold && x.CreatedOn.Year == year && x.CreatedOn.Month == item.Month));
                //}
                //listDynamicList2.Add(dataList3);
                //listDynamicList2.Add(dataList4);
                //listDynamicList2.Add(dataList5);

                //var lineChartResponse = new
                //{
                //    Label = monthNamesData,
                //    Graph = listDynamicList2,
                //};

                //#endregion

                res.Message = "Project Task Type Filter";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = new
                {
                    BoxData = boxData,
                    //BarChartData = barChartResponse,
                    //PieChartData = pieChartresponse,
                    //LineChartData = lineChartResponse,
                };
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/getprojectdashboard | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class ProjectDashboardFilter
        {
            public int? Year { get; set; }
            public int? Month { get; set; }
        }
        public class TimeSheetData
        {
            public Guid TaskId { get; set; }
            public string TaskTitle { get; set; }
            public long EstimateTime { get; set; }
            public TimeSpan SpendTime { get; set; }
            public int TaskAssignId { get; set; }
            public bool IsTaskIsFilled { get; set; }
            public DateTimeOffset StartDate { get; set; }
            public DateTimeOffset EndDate { get; set; }
            public TaskBillingConstants TaskBilling { get; set; }

        }
        #endregion

        #region API TO GET EMPLOYEE SHORT REPORT ON DASHBOARD
        /// <summary>
        /// Created By Harshit Mitra On 02-02-2023
        /// API >> GET >> api/projectreport/geteployeeshortreport
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("geteployeeshortreport")]
        public async Task<IHttpActionResult> GetEployeeShortReport(int year, int month)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var project = await _db.ProjectLists
                    .Where(x => x.CompanyId == tokenData.companyId && x.IsActive &&
                        !x.IsDeleted && x.CreatedOn.Year == year && x.CreatedOn.Month == month)
                    .ToListAsync();
                var projectIds = project.Select(x => x.ID).Distinct().ToList();
                var employeeData = await (from e in _db.Employee
                                          join ae in _db.AssignProjects on e.EmployeeId equals ae.EmployeeId into empty
                                          from q in empty.DefaultIfEmpty()
                                          where e.CompanyId == tokenData.companyId && e.IsActive && !e.IsDeleted
                                          select new
                                          {
                                              e.EmployeeId,
                                              e.DisplayName,
                                              //BillibleType = q == null ? : q
                                              ProjectId = q == null ? 0 : q.ProjectId,
                                              IsAssignToProject = q == null ? 0 : q.EmployeeId,
                                          })
                                          .ToListAsync();
                var timesheetData = await (from t in _db.TaskModels
                                           join q in _db.TaskLogs on t.TaskId equals q.TaskId into empty
                                           from l in empty.DefaultIfEmpty()
                                           where t.IsActive && !t.IsDeleted && l.IsActive && !l.IsDeleted
                                                && projectIds.Contains(t.ProjectId)
                                           select new
                                           {
                                               t.TaskId,
                                               t.TaskTitle,
                                               t.EstimateTime,
                                               SpendTime = l != null ? l.SpentTime : TimeSpan.Zero,
                                           })
                                           .ToListAsync();

                #region BOX DATA 
                var boxData = new
                {
                    TimeSheetBox = new
                    {
                        TotalTimeSheetTask = timesheetData.LongCount(),
                        TimeSheetFilled = timesheetData.LongCount() == 0 ? 0 : timesheetData.LongCount(x => x.SpendTime != TimeSpan.Zero),
                        TimeSheetNotFilled = timesheetData.LongCount() == 0 ? 0 : timesheetData.LongCount(x => x.SpendTime == TimeSpan.Zero),
                    },
                    ProjectBox = new
                    {
                        TotalProject = project.LongCount(),
                        ProjectAssigned = project.LongCount(x => x.ProjectStatus == Model.EnumClass.ProjectStatusConstants.Live),
                        ProjectNotAssigned = project.LongCount(x => x.ProjectStatus != Model.EnumClass.ProjectStatusConstants.Live),
                    },
                    EmployeeBox = new
                    {
                        TotalEmployee = employeeData.Select(x => x.EmployeeId).Distinct().Count(),
                        TotalEmployeeInProject = employeeData.Where(x => x.ProjectId != 0).Select(x => x.EmployeeId).Distinct().Count(),
                        TotalEmployeeNotInProject = employeeData.Where(x => x.ProjectId == 0).Select(x => x.EmployeeId).Distinct().Count(),
                    },
                };
                #endregion


                res.Message = "Project Task Type Filter";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = new
                {
                    BoxData = boxData,
                };
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/geteployeeshortreport | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion


        // -------------------------  NEW PROJECT REPORT ----------------------- //

        #region API TO GET ALL EMPLOYEE LIST IN PROJECT REPORT
        /// <summary>
        /// Created BY Harshit Mitra On 13-03-2023
        /// API >> GET >> api/projectreport/getallemployeelistinprojectreport
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallemployeelistinprojectreport")]
        public async Task<IHttpActionResult> GetAllEmployeeListInProjectReport()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {

                var employeeList = await _db.Employee
                    .Where(x => x.CompanyId == tokenData.companyId &&
                        x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee &&
                        x.IsActive && !x.IsDeleted)
                    .Select(x => new
                    {
                        x.EmployeeId,
                        x.DisplayName,
                    })
                    .OrderBy(x => x.DisplayName)
                    .ToListAsync();
                if (employeeList.Count == 0)
                {
                    res.Message = "List Is Empty";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = employeeList;
                    return Ok(res);
                }
                res.Message = "Employee List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = employeeList;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/getnewprojectreports | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion


        #region API TO GET ALL ORGANIZATION LIST IN COMPANY
        /// <summary>
        /// Created BY Harshit Mitra On 13-03-2023
        /// API >> GET >> api/projectreport/getallorganizationlistincompany
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallorganizationlistincompany")]
        public async Task<IHttpActionResult> GetAllOrganizationListInCompany()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {

                var orgList = await _db.OrgMaster
                    .Where(x => x.CompanyId == tokenData.companyId &&
                        x.IsActive && !x.IsDeleted)
                    .Select(x => new
                    {
                        x.OrgId,
                        x.OrgName,
                    })
                    .OrderBy(x => x.OrgName)
                    .ToListAsync();
                if (orgList.Count == 0)
                {
                    res.Message = "List Is Empty";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = orgList;
                    return Ok(res);
                }
                res.Message = "Organization List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = orgList;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectreport/getnewprojectreports | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion
    }
}
