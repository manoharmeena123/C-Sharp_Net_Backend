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
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.TimeSheet
{
    /// <summary>
    /// Created By Ravi Vyas On 26-12-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/kanbanboard")]
    public class KanbanBoardController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Api for Project Info in Kanban Dashboard

        /// <summary>
        /// API>>GET>>api/kanbanboard/getprojectinfoinkanban?ProjectId?type
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [Route("getprojectinfoinkanban")]
        public async Task<IHttpActionResult> GetDashboardData(int ProjectId, TaskTypeConstants? type = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            KanbanDashboardResponse response = new KanbanDashboardResponse();
            try
            {

                #region Api for Project Info

                var getData = await _db.ProjectLists
                              .Where(x => x.ID == ProjectId && x.IsActive && !x.IsDeleted)
                              .Select(x => new
                              {
                                  x.ID,
                                  x.ProjectName,
                                  x.ProjectDiscription
                              }).FirstOrDefaultAsync();
                response.ProjectData = getData;

                #endregion

                #region  Api for Project State

                var projectState = await _db.TaskModels
                                         .Where(x => x.IsActive && !x.IsDeleted &&
                                          x.TaskType == type && x.ProjectId == ProjectId &&
                                          x.AssignEmployeeId == tokenData.employeeId)
                                         .ToListAsync();
                var StatusData = new List<object>()
                    {
                        new
                        {
                            name = "Active",
                            value = projectState.Count(x=>x.Status != TaskStatusConstants.Closed) ,
                        },
                        new
                        {
                            name = "Pending",
                            value =  projectState.Count(x=>x.Status == TaskStatusConstants.Pending),
                        },
                        new
                        {
                            name = "Closed",
                            value =  projectState.Count(x=>x.Status == TaskStatusConstants.Closed),
                        }

                    };

                response.ProjectState = StatusData;

                #endregion

                #region Api for EmployeeList

                var empList = await (from ap in _db.AssignProjects
                                     join em in _db.Employee on ap.EmployeeId equals em.EmployeeId
                                     where ap.ProjectId == ProjectId && ap.IsActive && !ap.IsDeleted
                                     select new
                                     {
                                         em.EmployeeId,
                                         em.DisplayName,
                                         em.ProfileImageUrl
                                     })
                                      .Distinct().ToListAsync();
                response.EmployeeList = empList;

                #endregion

                res.Message = "Data Found Succesfully ! ";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = response;
                return Ok(res);
            }
            catch (Exception ex)
            {

                logger.Error("api/kanbanboard/getprojectinfoinkanban", ex.Message);
                return BadRequest("Failed");
            }
        }
        public class KanbanDashboardResponse
        {
            public object ProjectData { get; set; }
            public object ProjectState { get; set; }
            public object EmployeeList { get; set; }
        }

        #endregion

        #region Api for Issue Tracking 
        /// <summary>
        /// API>>GET>>api/kanbanboard/issuetrackingkanban?ProjectId
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("issuetrackingkanban")]
        public async Task<IHttpActionResult> GetIssueData(KanBanBoardRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getStatus = Enum.GetValues(typeof(TaskStatusConstants))
                   .Cast<TaskStatusConstants>()
                   .Select(x => new
                   {
                       TypeId = x,
                       TypeName = x.ToString().Replace("_", " "),
                   }).ToList();
                var statusType = getStatus
                                 .Select(x => new
                                 {
                                     Name = x.TypeName,
                                     Count = _db.TaskModels
                                             .Where(p => p.ProjectId == model.ProjectId && p.IsActive &&
                                             !p.IsDeleted && p.Status == x.TypeId && p.TaskType != TaskTypeConstants.BackLog)
                                             .Count(),
                                     //Value = _db.TaskModels
                                     //        .Where(p => p.ProjectId == model.ProjectId && p.IsActive &&
                                     //        !p.IsDeleted && p.Status == x.TypeId && p.TaskType != TaskTypeConstants.BackLog)
                                     //        .Select(p => new
                                     //        {
                                     //            TaskId = p.TaskId,
                                     //            TaskIdNumber = p.TaskIdNumber,
                                     //            TaskTitle = p.TaskTitle,
                                     //            AssignedTo = _db.Employee.Where(e => e.EmployeeId == p.AssignEmployeeId).Select(e => e.DisplayName).FirstOrDefault(),
                                     //            Priorit = p.Priority.ToString(),
                                     //            Status = p.Status.ToString(),
                                     //            StatusId = p.Status
                                     //        }).ToList(),
                                     Value = (from s in _db.Sprints
                                              join t in _db.TaskModels on s.SprintId equals t.SprintId
                                              where t.ProjectId == model.ProjectId && t.IsActive &&
                                              !t.IsDeleted && t.Status == x.TypeId && t.TaskType != TaskTypeConstants.BackLog &&
                                               s.SprintStatus == model.SprintStatus
                                              select new
                                              {
                                                  TaskId = t.TaskId,
                                                  TaskIdNumber = t.TaskIdNumber,
                                                  TaskTitle = t.TaskTitle,
                                                  AssignedTo = _db.Employee.Where(e => e.EmployeeId == t.AssignEmployeeId).Select(e => e.DisplayName).FirstOrDefault(),
                                                  Priorit = t.Priority.ToString(),
                                                  Status = t.Status.ToString(),
                                                  StatusId = t.Status,
                                                  SprintId = s.SprintId,
                                              })
                                              .ToList(),


                                 })
                                 .ToList();
                if (statusType.Count > 0)
                {
                    res.Message = "Data Get Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = statusType;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data Not Found Succesfully !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = statusType;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/kanbanboard/getprojectinfoinkanban", ex.Message);
                return BadRequest("Failed");
            }
        }

        public class KanBanBoardRequest
        {
            public int ProjectId { get; set; }
            public SprintStatusConstant SprintStatus { get; set; }
        }

        #endregion

        #region Api for Update task Status through Drag & Drop
        /// <summary>
        /// API>>POST>>api/kanbanboard/updatestatuskanban
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("updatestatuskanban")]
        public async Task<IHttpActionResult> UpdateTaskStatus(Updatetaskkanbanboard model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getdata = await _db.TaskModels
                                     .Where(x => x.TaskId == model.TaskId &&
                                      x.IsActive && !x.IsDeleted)
                                     .FirstOrDefaultAsync();
                if (getdata != null)
                {
                    getdata.Status = model.Status;
                    getdata.UpdatedBy = tokenData.employeeId;
                    getdata.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                    _db.Entry(getdata).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    res.Message = "Update Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = getdata;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Update Failed !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = getdata;
                    return Ok(res);
                }

            }
            catch (Exception ex)
            {

                logger.Error("api/kanbanboard/updatestatuskanban", ex.Message);
                return BadRequest("Failed");
            }
        }

        public class Updatetaskkanbanboard
        {
            public Guid TaskId { get; set; }
            public TaskStatusConstants Status { get; set; }
        }

        #endregion

    }
}
