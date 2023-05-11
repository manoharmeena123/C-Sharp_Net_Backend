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
using static AspNetIdentity.WebApi.Controllers.Employees.EmployeeExitsController;

namespace AspNetIdentity.WebApi.Controllers.TimeSheet
{
    /// <summary>
    /// Created By Ravi Vyas On 24/02/2023
    /// </summary
    [Authorize]
    [RoutePrefix("api/projectmilestone")]
    public class ProjectMilestoneController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Api for Get All Task By SprintId
        /// <summary>
        /// API>>GET>>api/projectmilestone/getallsprinttask
        /// </summary>
        /// <param name="sprintId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallsprinttask")]
        public async Task<IHttpActionResult> GetMilestoneTask(Guid sprintId, int projectId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            MileStoneResponse response = new MileStoneResponse();
            try
            {
                var getData = await _db.TaskModels.
                                        Where(x => x.IsActive && !x.IsDeleted && x.SprintId == sprintId &&
                                        x.CompanyId == tokenData.companyId && x.ProjectId == projectId).
                                        Select(x => new
                                        {
                                            SprintId = x.SprintId,
                                            TaskId = x.TaskId,
                                            ProjectTaskId = x.TaskIdNumber,
                                            TaskTitle = x.TaskTitle,
                                            Status = x.Status.ToString(),
                                        })
                                        .ToListAsync();

                response.MilestoneData = getData;

                var sprintPercantage = getData.
                                       Select(x => new
                                       {
                                           CloseTaskCount = getData.LongCount(t => t.SprintId == sprintId && t.Status.ToString() == TaskStatusConstants.Closed.ToString()),
                                           TotalTaskCount = getData.LongCount(z => z.SprintId == sprintId),
                                       })
                                      .Select(x => new
                                      {
                                          Percentage = Math.Round((((double)x.CloseTaskCount / x.TotalTaskCount) * 100), 2),
                                      }).FirstOrDefault();

                response.Percantage = sprintPercantage.Percentage;

                if (getData.Count > 0)
                {
                    res.Message = "Task Get Sucessfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = response;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Task Not Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = response;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectmilestone/getallsprinttask | " +
                    "ProjectId : " + projectId + " | " +
                    "SprintId : " + sprintId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        public class MileStoneResponse
        {
            public double Percantage { get; set; }
            public object MilestoneData { get; set; }
        }


        #endregion

        #region Api for Get All Sprint Task Of Project
        /// <summary>
        /// API>>GET>>api/projectmilestone/getallprojectsprinttask
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallprojectsprinttask")]
        public async Task<IHttpActionResult> GetAllProjectSprintTask(int projectId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            SprintResponseBodyModel response = new SprintResponseBodyModel();
            try
            {
                DateTimeOffset todayDate = DateTimeOffset.UtcNow.Date;
                var getData = await _db.Sprints
                                       .Where(x => x.IsActive && !x.IsDeleted &&
                                       x.ProjectId == projectId &&
                                       x.CompanyId == tokenData.companyId)
                                       .Select(x => new ProjectSprintTaskResponseBodyModel
                                       {
                                           SprintId = x.SprintId,
                                           ProjectId = x.ProjectId,
                                           SprintName = x.SprintName,
                                           EndDate = x.EndDate,
                                           ViewDate = "",
                                           IsDate = (x.EndDate >= todayDate) ? true : false,
                                           Discription = x.SprintDescription,
                                           SprintStatus = x.SprintStatus.ToString(),
                                           SprintPercantage = 0,
                                           CreateDate = x.CreatedOn,
                                           SprintStatusConstant = x.SprintStatus,
                                           SprintTask = _db.TaskModels.Where(s => s.SprintId == x.SprintId)
                                           .Select(s => new SprintTaskResponseBodyModel
                                           {
                                               SprintId = s.SprintId,
                                               TaskId = s.TaskId,
                                               TaskCode = s.TaskIdNumber,
                                               TaskTitle = s.TaskTitle,
                                               TaskStatus = s.Status.ToString(),
                                               StartDate = s.StartDate,
                                               EndDate = s.EndDate,
                                               TaskType = s.TaskType.ToString(),
                                               TaskPriotiy = s.Priority.ToString(),
                                               CreatedBy = _db.Employee.Where(e => e.EmployeeId == s.CreatedBy).Select(e => e.DisplayName).FirstOrDefault(),
                                               AssigneName = _db.Employee.Where(e => e.EmployeeId == s.AssignEmployeeId).Select(e => e.DisplayName).FirstOrDefault(),
                                               SpentTime = _db.TaskLogs.Where(t => t.TaskId == s.TaskId && t.IsActive && !x.IsDeleted).Select(t => t.SpentTime).ToList(),
                                               EstimateTime = s.EstimateTime,
                                               SpentTime1 = "",
                                               EstTime = "",
                                               Percantage = s.Percentage
                                           }).ToList()
                                       })
                                       .OrderByDescending(x => x.CreateDate)
                                       .ToListAsync();

                getData.ForEach(x =>
                {
                    x.ViewDate = x.EndDate.ToString("dd-MMM-yyyy");
                    x.CloseTaskCount = x.SprintTask.Where(z => z.SprintId == x.SprintId && z.TaskStatus.ToString() == TaskStatusConstants.Closed.ToString()).Count();
                    x.TotalTaskCount = x.SprintTask.Where(z => z.SprintId == x.SprintId).Count();
                    x.SprintPercantage = (int)(x.CloseTaskCount == 0 ? 0 : Math.Round((((double)x.CloseTaskCount / x.TotalTaskCount) * 100), 2));
                    x.SprintTask.ForEach(b =>
                    {
                        b.SpentTime1 = string.Format("{00:00}:{01:00}", (int)(b.SpentTime.Select(z => z.TotalMinutes).ToList().Sum() / 60),
                                                                      (int)(b.SpentTime.Select(z => z.TotalMinutes).ToList().Sum() % 60));
                        b.EstTime = string.Format("{00:00}:{01:00}", (int)b.EstimateTime / 60, b.EstimateTime % 60);
                    });
                });

                response.SprintData = getData.Where(c => c.SprintStatusConstant != SprintStatusConstant.Draft).ToList();

                #region Active Sprint

                var activeSprint = getData.Where(x => x.SprintStatusConstant == SprintStatusConstant.Active)
                                          .Select(x => new
                                          {
                                              x.SprintId,
                                              x.SprintName
                                          }).ToList();
                response.ActiveSprint = activeSprint;
                #endregion

                #region Closed Sprint

                var closedSprint = getData.Where(x => x.SprintStatusConstant == SprintStatusConstant.Closed)
                                          .Select(x => new
                                          {
                                              x.SprintId,
                                              x.SprintName
                                          }).ToList();

                response.ClosedSprint = closedSprint;

                #endregion

                #region Draft Sprint

                var draftSprint = getData.Where(x => x.SprintStatusConstant == SprintStatusConstant.Draft)
                                      .Select(x => new
                                      {
                                          x.SprintId,
                                          x.SprintName
                                      }).ToList();

                response.DraftSprint = draftSprint;

                #endregion


                if (getData.Count > 0)
                {
                    res.Message = "Task Get Sucessfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = response;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Task Not Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = response;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectmilestone/getallprojectsprinttask | " +
                    "ProjectId : " + projectId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class ProjectSprintTaskResponseBodyModel
        {
            public Guid SprintId { get; set; }
            public int ProjectId { get; set; }
            public string SprintName { get; set; }
            public DateTimeOffset EndDate { get; set; }
            public string SprintStatus { get; set; }
            public int SprintPercantage { get; set; }
            public List<SprintTaskResponseBodyModel> SprintTask { get; set; }
            public int CloseTaskCount { get; set; }
            public int TotalTaskCount { get; set; }
            public string Discription { get; set; }
            public bool IsDate { get; set; }
            public string ViewDate { get; set; }
            public DateTimeOffset CreateDate { get; set; }
            public SprintStatusConstant SprintStatusConstant { get; set; }
        }
        public class SprintTaskResponseBodyModel
        {
            public Guid SprintId { get; set; }
            public Guid TaskId { get; set; }
            public int TaskCode { get; set; }
            public string TaskTitle { get; set; }
            public string TaskStatus { get; set; }
            public DateTimeOffset? StartDate { get; set; }
            public DateTimeOffset? EndDate { get; set; }
            public string TaskType { get; set; }
            public string TaskPriotiy { get; set; }
            public string CreatedBy { get; set; }
            public string AssigneName { get; set; }
            public double EstimateTime { get; set; }
            public List<TimeSpan> SpentTime { get; set; }
            public string SpentTime1 { get; set; }
            public string EstTime { get; set; }
            public int Percantage { get; set; }
        }

        public class FinalOfSprintResponse
        {
            public List<ProjectSprintTaskResponseBodyModel> Data { get; set; }
            public object SprintPercantage { get; set; }
        }

        public class SprintResponseBodyModel
        {
            public object SprintData { get; set; }
            public object ActiveSprint { get; set; }
            public object ClosedSprint { get; set; }
            public object DraftSprint { get; set; }
        }

        #endregion

        #region Api for Get All Milestone of Project
        /// <summary>
        /// API>>GET>>api/projectmilestone/getallmilestone
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallmilestone")]
        public async Task<IHttpActionResult> GetAllSprint(int projectId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.Sprints
                                       .Where(x => x.ProjectId == projectId &&
                                       x.CompanyId == tokenData.companyId && x.SprintStatus != SprintStatusConstant.Closed)
                                       .Select(x => new
                                       {
                                           x.SprintId,
                                           x.SprintName
                                       })
                                       .ToListAsync();
                if (getData.Count > 0)
                {
                    res.Message = "Sprint Get Sucessfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = getData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Task Not Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectmilestone/getallmilestone | " +
                    "ProjectId : " + projectId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        #endregion

        #region Api for Get All Sprint Task Of Project By SprintId
        /// <summary>
        /// API>>GET>>api/projectmilestone/getallprojectsprinttaskbysprintid
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallprojectsprinttaskbysprintid")]
        public async Task<IHttpActionResult> GetAllProjectSprintTaskBySprintId(int projectId, Guid sprintId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            DateTimeOffset todayDate = DateTimeOffset.UtcNow.Date;
            try
            {
                var getData = await _db.Sprints
                                       .Where(x => x.IsActive && !x.IsDeleted &&
                                       x.ProjectId == projectId && x.SprintId == sprintId &&
                                       x.CompanyId == tokenData.companyId)
                                       .Select(x => new ProjectSprintTaskResponseBodyModel
                                       {
                                           SprintId = x.SprintId,
                                           ProjectId = x.ProjectId,
                                           SprintName = x.SprintName,
                                           EndDate = x.EndDate,
                                           ViewDate = "",
                                           IsDate = (x.EndDate >= todayDate) ? true : false,
                                           SprintStatus = x.SprintStatus.ToString(),
                                           SprintPercantage = 0,
                                           Discription = x.SprintDescription,

                                           SprintTask = _db.TaskModels.Where(s => s.SprintId == x.SprintId)
                                           .Select(s => new SprintTaskResponseBodyModel
                                           {
                                               SprintId = s.SprintId,
                                               TaskId = s.TaskId,
                                               TaskCode = s.TaskIdNumber,
                                               TaskTitle = s.TaskTitle,
                                               TaskStatus = s.Status.ToString(),
                                               StartDate = s.StartDate,
                                               EndDate = s.EndDate,
                                               TaskType = s.TaskType.ToString(),
                                               TaskPriotiy = s.Priority.ToString(),
                                               Percantage = s.Percentage,
                                               CreatedBy = _db.Employee.Where(e => e.EmployeeId == s.CreatedBy).Select(e => e.DisplayName).FirstOrDefault(),
                                               AssigneName = _db.Employee.Where(e => e.EmployeeId == s.AssignEmployeeId).Select(e => e.DisplayName).FirstOrDefault(),
                                               SpentTime = _db.TaskLogs.Where(t => t.TaskId == s.TaskId && t.IsActive && !x.IsDeleted).Select(t => t.SpentTime).ToList(),
                                               EstimateTime = s.EstimateTime,
                                               SpentTime1 = "",
                                               EstTime = ""
                                           }).ToList()
                                       })
                                       .ToListAsync();
                getData.ForEach(x =>
                {
                    x.ViewDate = x.EndDate.ToString("dd-MMM-yyyy");
                    x.CloseTaskCount = x.SprintTask.Where(z => z.SprintId == x.SprintId && z.TaskStatus.ToString() == TaskStatusConstants.Closed.ToString()).Count();
                    x.TotalTaskCount = x.SprintTask.Where(z => z.SprintId == x.SprintId).Count();
                    x.SprintPercantage = (int)(x.CloseTaskCount == 0 ? 0 : Math.Round((((double)x.CloseTaskCount / x.TotalTaskCount) * 100), 2));
                    x.SprintTask.ForEach(b =>
                    {
                        b.SpentTime1 = string.Format("{00:00}:{01:00}", (int)(b.SpentTime.Select(z => z.TotalMinutes).ToList().Sum() / 60),
                                                                      (int)(b.SpentTime.Select(z => z.TotalMinutes).ToList().Sum() % 60));
                        b.EstTime = string.Format("{00:00}:{01:00}", (int)b.EstimateTime / 60, b.EstimateTime % 60);
                    });
                });

                if (getData.Count > 0)
                {
                    res.Message = "Task Get Sucessfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = getData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Task Not Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/projectmilestone/getallprojectsprinttaskbysprintid | " +
                    "ProjectId : " + projectId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        #endregion

        #region GAT ALL SPRINT STATUS OF PROJECT  // dropdown
        /// <summary>
        /// Created By Ravi Vyas on 28-11-2022
        /// API>>Get>>api/projectmilestone/getsprintstatus
        /// </summary>
        [Route("getsprintstatus")]
        [HttpGet]
        public ResponseBodyModel TaskBilling()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var getStatus = Enum.GetValues(typeof(SprintStatusConstant))
                    .Cast<SprintStatusConstant>()
                    .Select(x => new HelperModelForEnum
                    {
                        TypeId = (int)x,
                        TypeName = Enum.GetName(typeof(SprintStatusConstant), x).Replace("_", " ")
                    }).ToList();

                res.Message = "Get Succesfully";
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

    }
}
