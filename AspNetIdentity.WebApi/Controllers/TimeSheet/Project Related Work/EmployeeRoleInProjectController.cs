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

namespace AspNetIdentity.WebApi.Controllers.TimeSheet.Project_Related_Work
{
    /// <summary>
    /// Created By Ravi Vyas On 13/02/2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/employeeroleinproject")]
    public class EmployeeRoleInProjectController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Api for Add And Update Role In Project
        /// <summary>
        /// API>>POST>>api/employeeroleinproject/addroleinproject
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addroleinproject")]
        public async Task<IHttpActionResult> AddRoleInProject(AddEmployeeRoleRequestBody model)
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
                    var checkData = await _db.EmployeeRoleInProjects
                                          .FirstOrDefaultAsync(x => x.IsActive && !x.IsDeleted &&
                                          x.CompanyId == tokenData.companyId && x.ProjectId == model.ProjectId &&
                                          x.EmployeeRoleInProjectId == model.EmployeeRoleId);
                    if (checkData == null)
                    {
                        EmployeeRoleInProject obj = new EmployeeRoleInProject
                        {
                            ProjectId = model.ProjectId,
                            CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                            CreatedBy = tokenData.employeeId,
                        };
                        _db.EmployeeRoleInProjects.Add(obj);
                        await _db.SaveChangesAsync();
                        checkData = obj;
                    }
                    else
                    {
                        checkData.UpdatedBy = tokenData.employeeId;
                        checkData.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                    }
                    checkData.RoleName = model.RoleName;
                    checkData.CompanyId = tokenData.companyId;
                    _db.Entry(checkData).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Role Added Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = checkData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/employeeroleinproject/addroleinproject | " +
                   "Model : " + JsonConvert.SerializeObject(model) + " | " +
                   "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        public class AddEmployeeRoleRequestBody
        {
            public Guid EmployeeRoleId { get; set; }
            public int EmployeeId { get; set; } = 0;
            public int ProjectId { get; set; } = 0;
            public string RoleName { get; set; } = string.Empty;
            public string RoleDescription { get; set; }

        }

        #endregion

        #region Api For Get All Role In Project By Project Id
        /// <summary>
        /// API>>GET>>api/employeeroleinproject/getroleinproject
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getroleinproject")]
        public async Task<IHttpActionResult> GetRoleInProject(int projectId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.EmployeeRoleInProjects
                                       .Where(x => x.IsActive && !x.IsDeleted && x.ProjectId == projectId &&
                                       x.CompanyId == tokenData.companyId)
                                       .Select(x => new
                                       {
                                           x.EmployeeRoleInProjectId,
                                           x.RoleName,
                                       })
                                       .ToListAsync();
                if (getData.Count > 0)
                {
                    res.Message = "Data Get Succesfully !";
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
                logger.Error("API : api/employeeroleinproject/getroleinproject | " +
                    "ProjectId : " + projectId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region Api For Delete Role
        /// <summary>
        /// API>>DELETE>>api/employeeroleinproject/deleteemployeerole
        /// </summary>
        /// <param name="employeeRoleId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteemployeerole")]
        public async Task<IHttpActionResult> DeleteRole(Guid employeeRoleId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkData = await _db.EmployeeRoleInProjects
                                         .FirstOrDefaultAsync(x => x.IsActive && !x.IsDeleted &&
                                         x.CompanyId == tokenData.companyId &&
                                         x.EmployeeRoleInProjectId == employeeRoleId);
                if (checkData != null)
                {
                    checkData.IsActive = false;
                    checkData.IsDeleted = true;
                    checkData.DeletedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                    checkData.DeletedBy = tokenData.employeeId;
                    _db.Entry(checkData).State = EntityState.Modified;

                    res.Message = "Deleted Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
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
                logger.Error("API : api/employeeroleinproject/deleteemployeerole | " +
                  "EmployeeRoleId : " + employeeRoleId + " | " +
                  "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

    }
}
