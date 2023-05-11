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
    /// Created By Ravi Vyas On 29-11-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/sprint")]
    public class SprintController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API TO CREATE SPRINT 
        /// <summary>
        /// Created By Ravi Vyas On 29-11-2022
        /// API>>POST>>api/sprint/createsprint
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createsprint")]
        public async Task<IHttpActionResult> CreateLeaveType(CreateSprintRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                      .SelectMany(v => v.Errors)
                      .Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }
                else
                {
                    var checkSprint = await _db.Sprints
                                      .FirstOrDefaultAsync(x => x.CompanyId == tokenData.companyId
                                      && x.SprintName == model.SprintName && x.ProjectId == model.ProjectId);

                    if (checkSprint == null)
                    {
                        Sprint obj = new Sprint

                        {
                            ProjectId = model.ProjectId,
                            SprintName = model.SprintName,
                            SprintDescription = model.SprintDescription,
                            StartDate = model.StartDate,
                            EndDate = model.EndDate,
                            CompanyId = tokenData.companyId,
                            CreatedBy = tokenData.employeeId,
                            SprintStatus = SprintStatusConstant.Draft,
                            CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),

                        };
                        _db.Sprints.Add(obj);
                        await _db.SaveChangesAsync();

                        res.Message = "Created Successfully !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                        res.Data = checkSprint;
                        return Ok(res);
                    }
                    else
                    {
                        res.Message = "Sprint Name Already Exits !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotAcceptable;
                        res.Data = checkSprint;
                        return Ok(res);
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/sprint/createsprint | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class CreateSprintRequest
        {
            public Guid SprintId { get; set; }
            public string SprintName { get; set; }
            public int ProjectId { get; set; }
            public string SprintDescription { get; set; }
            public DateTimeOffset EndDate { get; set; }
            public SprintStatusConstant SprintStatus { get; set; }
            public DateTimeOffset StartDate { get; set; }
        }

        #endregion

        #region API TO GET ALL SPRINT
        /// <summary>
        /// Created By Ravi Vyas On 29-11-2022
        /// API>>GET>>api/sprint/getsprint
        /// </summary>
        /// <returns></returns>
        [Route("getsprint")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllSprint(int projectId, int? page = null, int? count = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.Sprints.
                              Where(x => x.IsActive && !x.IsDeleted &&
                              x.CompanyId == tokenData.companyId && x.ProjectId == projectId).
                              Select(x => new
                              {
                                  SprintId = x.SprintId,
                                  SprintName = x.SprintName,
                              })
                              .ToListAsync();

                if (getData.Count > 0)
                {
                    res.Message = "Sprint Succesfully Get !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    if (page.HasValue && count.HasValue)
                    {
                        res.Data = new
                        {
                            TotalData = getData.Count,
                            Counts = (int)count,
                            List = getData.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                        return Ok(res);
                    }
                    else
                    {
                        res.Data = getData;
                        return Ok(res);
                    }
                }
                else
                {
                    res.Message = "No Sprint Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/sprint/getsprint | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API FOR GET SPRINT BY PROJECT ID
        /// <summary>
        /// Created By Ravi Vyas On 29-11-2022
        /// API>>GET>>api/sprint/getsprintbyprojectid
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        [Route("getsprintbyprojectid")]
        [HttpGet]
        public async Task<IHttpActionResult> GetSprintByProjectId(int projectId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.Sprints
                              .Where(x => x.ProjectId == projectId &&
                              x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId)
                              .Select(x => new
                              {
                                  SprintId = x.SprintId,
                                  SprintName = x.SprintName,
                                  SprintDescription = x.SprintDescription,
                                  StartDate = x.StartDate,
                                  EndDate = x.EndDate,
                                  ProjectId = x.ProjectId,
                                  ProjectName = _db.ProjectLists.Where(y => y.ID == x.ProjectId).Select(y => y.ProjectName).FirstOrDefault()
                              })
                              .FirstOrDefaultAsync();
                if (getData != null)
                {
                    res.Message = " Succesfully Get Sprint !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = getData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Sprint Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/sprint/getsprintbyprojectid | " +
                    "projectId : " + projectId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        #endregion

        #region API FOR DELETE SPRINT
        /// <summary>
        /// Created By Ravi Vyas On 29-11-2022
        /// API>>GET>>api/sprint/deletesprint
        /// </summary>
        /// <param name="SprintId"></param>
        /// <returns></returns>
        [Route("deletesprint")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteSprint(Guid sprintId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.Sprints
                    .FirstOrDefaultAsync(x => x.SprintId == sprintId
                    && x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId);

                if (getData != null)
                {
                    getData.IsActive = false;
                    getData.IsDeleted = true;
                    getData.UpdatedBy = tokenData.employeeId;
                    getData.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                    _db.Entry(getData).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Deleted Succesfully  !";
                    res.Status = true;
                    return Ok(res);
                }
                else
                {
                    res.Message = " Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/sprint/deletesprint | " +
                                    "SprintId : " + sprintId + " | " +
                                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }


        #endregion

        #region API FOR GET SPRINT BY SPRINT ID
        /// <summary>
        /// Created By Ravi Vyas On 29-11-2022
        /// API>>GET>>api/sprint/getsprintbyspeintid
        /// </summary>
        /// <param name="sprintId"></param>
        /// <returns></returns>
        [Route("getsprintbyspeintid")]
        [HttpGet]
        public async Task<IHttpActionResult> GetSprintByProjectId(Guid sprintId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.Sprints
                                       .Where(x => x.SprintId == sprintId &&
                                       x.IsActive && !x.IsDeleted)
                                       .Select(x => new
                                       {
                                           SprintId = x.SprintId,
                                           SprintName = x.SprintName,
                                           SprintDescription = x.SprintDescription,
                                           EndDate = x.EndDate,
                                           SprintStatus = x.SprintStatus.ToString(),
                                           SprintStatusId = x.SprintStatus,
                                           StartDate = x.StartDate,
                                       })
                                       .FirstOrDefaultAsync();
                if (getData != null)
                {
                    res.Message = " Succesfully Get Sprint !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = getData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Sprint Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/sprint/getsprintbyspeintid", ex.Message);
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API FOR GET ALL CLOSED SPRINT
        /// <summary>
        /// API>>GET>>api/sprint/getallclosedsprint
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallclosedsprint")]
        public async Task<IHttpActionResult> GetAllClosedSprint(int projectId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getSprint = await _db.Sprints
                                         .Where(x => x.ProjectId == projectId && x.IsActive && !x.IsDeleted &&
                                         x.SprintStatus == SprintStatusConstant.Closed && x.CompanyId == tokenData.companyId).
                                         Select(x => new
                                         {
                                             SprintId = x.SprintId,
                                             SprintName = x.SprintName,
                                         })
                                         .ToListAsync();
                if (getSprint.Count > 0)
                {
                    res.Message = "Get Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = getSprint;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = getSprint;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {

                logger.Error("API : api/sprint>>getallclosedsprint | " +
                    "projectId : " + projectId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }


        #endregion

        #region API FOR UPDATE SPRINT
        /// <summary>
        /// Created By Ravi Vyas on 20-03-2023
        /// API>>GET>>Z
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updatesprint")]
        public async Task<IHttpActionResult> UpdateSprint(CreateSprintRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkData = await _db.Sprints.Where(x => x.IsActive && !x.IsDeleted &&
                                                 x.CompanyId == tokenData.companyId &&
                                                 x.SprintId == model.SprintId)
                                                 .FirstOrDefaultAsync();
                if (checkData != null)
                {
                    checkData.SprintName = model.SprintName;
                    checkData.SprintStatus = model.SprintStatus;
                    checkData.SprintDescription = model.SprintDescription;
                    checkData.StartDate = model.StartDate;
                    checkData.EndDate = model.EndDate;
                    checkData.ProjectId = model.ProjectId;
                    checkData.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                    checkData.UpdatedBy = tokenData.employeeId;
                    _db.Entry(checkData).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Update Successfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = checkData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "No Data Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotAcceptable;
                    res.Data = checkData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/sprint/updatesprint | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }


        #endregion

        #region API FOR USE UPDATE SPRINT STATUS ACTIVE OR DEACTIVE 
        /// <summary>
        /// Created By Ravi Vyas On 20-03-2023
        /// API>>GET>>api/sprint/updatestatusactive
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updatestatusactive")]
        public async Task<IHttpActionResult> ActiveDeactiveSprint(ActiveDeactiveRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkStatus = await _db.Sprints.Where(x => x.IsActive && !x.IsDeleted &&
                                             x.CompanyId == tokenData.companyId &&
                                             x.SprintId == model.SprintId)
                                             .FirstOrDefaultAsync();
                if (checkStatus != null)
                {
                    checkStatus.SprintStatus = model.SprintStatus;
                    checkStatus.UpdatedBy = tokenData.employeeId;
                    checkStatus.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                    _db.Entry(checkStatus).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Update Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = checkStatus;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Update Faild !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotAcceptable;
                    res.Data = checkStatus;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/sprint/updatestatusactivet | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        public class ActiveDeactiveRequest
        {
            public Guid SprintId { get; set; }
            public int ProjectId { get; set; }
            public SprintStatusConstant SprintStatus { get; set; }

        }
        #endregion
    }
}
