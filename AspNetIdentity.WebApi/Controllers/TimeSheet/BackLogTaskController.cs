using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.TimeSheet;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.TimeSheet
{
    /// <summary>
    /// Created By Ravi Vyas on 16-01-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/taskbacklog")]
    public class BackLogTaskController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Api for Get All Backlog Task By PeojectId
        /// <summary>
        /// Created By Ravi Vyas On 16-01-2022
        /// GET >> API >> api/taskbacklog/getallbacklogtask
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallbacklogtask")]
        public async Task<IHttpActionResult> GetAllBacklogTask(int projectId, int page = 1, int count = 10)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getBacklogTask = await _db.TaskModels.
                                            Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId &&
                                            x.ProjectId == projectId && x.TaskType == TaskTypeConstants.BackLog).
                                            Select(x => new TaskBackLogResponseModel
                                            {
                                                TaskId = x.TaskId,
                                                TaskTitle = x.TaskTitle,
                                                Discription = x.Discription,
                                                CreatedDate = x.CreatedOn,
                                                CreatedByName = _db.Employee.Where(e => e.EmployeeId == x.CreatedBy).Select(e => e.DisplayName).FirstOrDefault(),
                                            })
                                           .OrderByDescending(x => x.CreatedDate)
                                           .ToListAsync();

                if (getBacklogTask.Count == 0)
                {
                    res.Message = "Backlog Task Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = new
                    {
                        Page = page,
                        Count = count,
                        BackLogList = getBacklogTask,
                    };
                    return Ok(res);
                }

                res.Message = "All Backlog Task Found !";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = new
                {
                    Page = page,
                    Count = count,
                    BackLogList = getBacklogTask,
                };
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/taskbacklog/getallbacklogtask| " +
                  "ProjectId : " + projectId + " | " +
                  "Page : " + page + " | " +
                  "Count : " + count + " | " +
                  "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        public class TaskBackLogResponseModel
        {

            public Guid TaskId { get; set; }
            public string TaskTitle { get; set; }
            public string Discription { get; set; }
            public DateTimeOffset CreatedDate { get; set; }
            public string CreatedByName { get; set; }

        }

        #endregion

        #region Api for Get Backlog Task By TaskId
        /// <summary>
        /// Created By Ravi Vyas On 16-01-2022
        /// GET>>API>>api/taskbacklog/getbacklogtaskbytaskid
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getbacklogtaskbytaskid")]
        public async Task<IHttpActionResult> GetAllBacklogTask(Guid taskId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getBacklogTask = await _db.TaskModels.
                                            Where(x => x.IsActive && !x.IsDeleted &&
                                            x.CompanyId == tokenData.companyId &&
                                            x.TaskId == taskId)
                                           .FirstOrDefaultAsync();
                if (getBacklogTask != null)
                {
                    res.Message = " Task Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = getBacklogTask;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Task Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = getBacklogTask;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/taskbacklog/getbacklogtaskbytaskid| " +
                  "ProjectId : " + taskId + " | " +
                  "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        #endregion

        #region Api for Update BackLogTask 
        /// <summary>
        /// Created By Ravi Vyas On 16-01-2022
        /// API>>POST>>api/taskbacklog/updatebacklogtask
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updatebacklogtask")]
        public async Task<IHttpActionResult> UpdateBackLogTask(UpdateBackLogTaskRequestModel model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {

                var updateData = await _db.TaskModels.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId
                                && x.TaskId == model.TaskId).FirstOrDefaultAsync();
                var check = model.EstimateTime.ToString().Split('.');
                var timeCheck = model.EstimateTime.ToString().Contains(".") ? model.EstimateTime.ToString()
                               .Split('.').Select(long.Parse).ToList() : model.EstimateTime.ToString().Split(':')?.Select(long.Parse).ToList();
                long totalEstimageMinutes = (timeCheck[0] * 60) + timeCheck[1];
                if (updateData != null)
                {
                    updateData.TaskTitle = model.TaskTitle;
                    updateData.TaskType = model.TaskType;
                    updateData.Discription = model.Discription;
                    updateData.Priority = model.Priority;
                    updateData.AssignEmployeeId = model.AssignEmployeeId;
                    updateData.Attechment = model.Attechment;
                    updateData.EstimateTime = totalEstimageMinutes;
                    updateData.StartDate = model.StartDate;
                    updateData.EndDate = model.EndDate;
                    updateData.Image1 = model.Image1;
                    updateData.Image2 = model.Image2;
                    updateData.Image3 = model.Image3;
                    updateData.Image4 = model.Image4;
                    updateData.TaskURL = model.TaskURL;
                    updateData.TaskBilling = model.TaskBilling;
                    updateData.UpdatedBy = tokenData.employeeId;
                    updateData.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                    _db.Entry(updateData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Update Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = updateData;
                    return Ok(res);

                }
                else
                {
                    res.Message = "No Data Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = updateData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {

                logger.Error("API : api/taskbacklog/updatebacklogtask | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        public class UpdateBackLogTaskRequestModel
        {
            public Guid TaskId { get; set; }
            public int ProjectId { get; set; }
            public string TaskTitle { get; set; }
            public string Discription { get; set; }
            public TaskPriorityConstants Priority { get; set; } = TaskPriorityConstants.Medium;
            public TaskTypeConstants TaskType { get; set; } = TaskTypeConstants.BackLog;
            public Guid TaskTypeId { get; set; } = Guid.Empty;
            public int? AssignEmployeeId { get; set; } = 0;
            public string Attechment { get; set; }
            //public TimeSpan EstimateTime { get; set; }
            public string EstimateTime { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public int TaskIdNumber { get; set; } = 0;
            public string Image1 { get; set; }
            public string Image2 { get; set; }
            public string Image3 { get; set; }
            public string Image4 { get; set; }
            public string TaskURL { get; set; }
            public TaskBillingConstants TaskBilling { get; set; } = TaskBillingConstants.Non_Billable;
            public bool IsMail { get; set; }

        }

        #endregion

        #region Api for Delete Backlog Task
        /// <summary>
        /// API>>DELETE>>api/taskbacklog/deletebacklogtask
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletebacklogtask")]
        public async Task<IHttpActionResult> DeleteBacklogTask(Guid taskId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkData = await _db.TaskModels.
                                       Where(x => x.TaskId == taskId && x.IsActive &&
                                       !x.IsDeleted && x.CompanyId == tokenData.companyId)
                                       .FirstOrDefaultAsync();
                if (checkData != null)
                {
                    checkData.IsActive = false;
                    checkData.IsDeleted = true;
                    checkData.DeletedBy = tokenData.employeeId;
                    checkData.DeletedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                    _db.Entry(checkData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Deleted Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = checkData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data Not Found  !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = checkData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/taskbacklog/deletebacklogtask| " +
                                  "TaskId : " + taskId +
                                  "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

    }
}
