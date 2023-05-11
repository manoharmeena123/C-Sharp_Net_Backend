using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.GoalManagement;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.Goalmanagement
{
    /// <summary>
    /// Created By Ankit Jain on 19-01-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/goalpermission")]
    public class GoalPermissionController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Api To Get GoalType
        /// <summary>
        /// Created By ankit Jain on 19-01-2023
        /// API >> Get >> api/goalpermission/getgoaltypeforcreate
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getgoaltypeforcreate")]
        public async Task<IHttpActionResult> GetGoalTypeForCreate()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var permission = await _db.GoalsPermissions
                    .Where(x => x.EmployeeId == tokenData.employeeId)
                    .Select(x => x.Permission).FirstOrDefaultAsync();
                var goalType = Enum.GetValues(typeof(GoalTypeConstants))
                                .Cast<GoalTypeConstants>()
                                .Select(x => new
                                {
                                    GoalTypeId = (int)x,
                                    GoalTypeName = Enum.GetName(typeof(GoalTypeConstants), x).Replace("_", " "),
                                    HasPermission = NewGoalHelper.CheckGoalPermission(permission, x, NewGoalHelper.PermissionType.Create),

                                }).ToList();

                res.Message = "Get Goal List Found !";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = goalType;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/goalpermission/getgoaltypeforcreate | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }

        }

        #endregion Api To Get GoalType

        #region API TO CREATE Permission In Employee Goal
        /// <summary>
        /// Created By Ankit Jain On 24-01-2023
        /// API >> POST >> api/goalpermission/addemployeeingoal
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addemployeeingoal")]
        public async Task<IHttpActionResult> CreateNewGoal(int employeeId)
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
                var goalPermission = await _db.GoalsPermissions.FirstOrDefaultAsync(x => x.IsActive && !x.IsDeleted
                             && x.CompanyId == tokenData.companyId && x.EmployeeId == employeeId);
                if (goalPermission == null)
                {
                    GoalsPermission obj = new GoalsPermission
                    {
                        EmployeeId = employeeId,
                        CreatedBy = tokenData.employeeId,
                        CompanyId = tokenData.companyId,
                        CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                    };
                    _db.GoalsPermissions.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "Created Succesfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                }
                else
                {
                    res.Message = "Employee already Exist !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.Created;
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/goalpermission/addemployeeingoal | " +
                   //"Model : " + JsonConvert.SerializeObject(employeeId) + " | " +
                   "Employee Id : " + employeeId + " | " +
                   "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }
        #endregion

        #region This Api To Get All Employee 
        /// <summary>
        /// Created By Ankit Jain on 19-01-2023
        /// API >>Get >> api/goalpermission/getemployee
        /// </summary>
        [HttpGet]
        [Route("getemployee")]
        public async Task<IHttpActionResult> GetEmployee()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employeeList = await (from a in _db.GoalsPermissions
                                          join e in _db.Employee on a.EmployeeId equals e.EmployeeId
                                          where a.IsActive && !a.IsDeleted
                                          select new
                                          {
                                              e.EmployeeId,
                                              e.DisplayName,
                                          }).ToListAsync();
                if (employeeList.Count > 0)
                {
                    res.Message = "Employee List Found !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = employeeList;
                }
                else
                {
                    res.Message = "List Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;

                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/goalpermission/getemployee | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
            return Ok(res);
        }
        #endregion

        #region API Use To Update Goals Permission Data
        /// <summary>
        /// Created By Ankit Jain On 23-01-2022
        /// API >> POST >> api/goalpermission/updategoalpermission
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updategoalpermission")]
        public async Task<IHttpActionResult> UpdateGoalPermission(UpdateGoalPermissionRequest model)
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
                var permission = await _db.GoalsPermissions.FirstOrDefaultAsync(x => !x.IsDeleted &&
                        x.EmployeeId == model.EmployeeId && x.IsActive && x.CompanyId == tokenData.companyId);
                if (permission == null)
                {
                    res.Message = "Permission Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    return Ok(res);
                }
                permission.Permission = JsonConvert.SerializeObject(model.Permission);
                permission.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                permission.UpdatedBy = tokenData.employeeId;

                _db.Entry(permission).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "Permission Updated";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;
                res.Data = permission;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API :api/goalpermission/updategoalpermission | " +
                   "Model : " + JsonConvert.SerializeObject(model) + " | " +
                   "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
        }
        public class UpdateGoalPermissionRequest
        {
            [Required]
            public int EmployeeId { get; set; } = 0;
            public ModuleClass Permission { get; set; }
        }
        #endregion

        #region API TO Get Permission Employee In Goal By Id

        /// <summary>
        /// Created By Ankit On 24-01-2023
        /// API >> Get >> api/goalpermission/getgoalpermissionbyid
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("getgoalpermissionbyid")]
        public async Task<IHttpActionResult> GetEmployee(int employeeId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var permission = await _db.GoalsPermissions.Where(x => !x.IsDeleted &&
                          x.IsActive && x.CompanyId == tokenData.companyId && x.EmployeeId == employeeId).ToListAsync();
                var permissionList = permission
                     .Select(x => new GetGoalPermissionResponse
                     {
                         EmployeeId = x.EmployeeId,
                         Permission = JsonConvert.DeserializeObject<ModuleClass>(x.Permission),
                     })
                     .FirstOrDefault();
                res.Message = "Get All Goal Permission";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                res.Data = permissionList;
                return Ok(res);

            }
            catch (Exception ex)
            {
                logger.Error("API : api/goalpermission/getgoalpermissionbyid | " +
                   //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                   "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest(JsonConvert.SerializeObject(ex));
            }
        }
        public class GetGoalPermissionResponse : UpdateGoalPermissionRequest { }
        #endregion

        #region API TO DELETE Permission
        /// <summary>
        /// Created By Ankit Jain On 02/02/2023
        /// API >> POST >> api/goalpermission/deletepermission
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deletepermission")]
        public async Task<IHttpActionResult> DeletePermission(int employeeId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var goalPermission = await _db.GoalsPermissions
                          .FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
                if (goalPermission == null)
                {
                    res.Message = "Goal Permission Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    goalPermission.IsActive = false;
                    goalPermission.IsDeleted = true;
                    goalPermission.DeletedBy = tokenData.employeeId;
                    goalPermission.DeletedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);

                    _db.Entry(goalPermission).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Goal Permission Deleted";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = goalPermission;
                }
                return Ok(res);

            }
            catch (Exception ex)
            {
                logger.Error("API : api/goalpermission/deletepermission | " +
                    "Employee Id : " + employeeId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion
    }
}
