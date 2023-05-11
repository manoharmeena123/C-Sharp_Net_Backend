using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.TimeSheet;
using AspNetIdentity.WebApi.Models;
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
    /// Created By Ravi Vyas On 22-12-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/taskpermissons")]
    public class TaskPermissionsController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Api for get All EmployeeList By ProjectId
        /// <summary>
        /// API>>GET>>api/taskpermissons/getallassignempinproject?ProjectId
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallassignempinproject")]
        public async Task<IHttpActionResult> GetEmpList(int ProjectId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await (from p in _db.AssignProjects
                                     join e in _db.Employee on p.EmployeeId equals e.EmployeeId
                                     join u in _db.User on p.EmployeeId equals u.EmployeeId
                                     where p.IsActive && !p.IsDeleted && p.ProjectId == ProjectId
                                     select new
                                     {
                                         EmployeeId = p.EmployeeId,
                                         EmployeeName = e.DisplayName,
                                         ProfileImg = e.ProfileImageUrl,
                                         OfficalEmail = e.OfficeEmail,
                                         EmployeeType = u.LoginId.ToString(),
                                     })
                                     .ToListAsync();

                if (getData.Count > 0)
                {
                    res.Message = "Data Get Successfully ";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = getData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data Not Found ! ";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = getData;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/taskpermissons/getallassignempinproject", ex.Message);
                return BadRequest("Failed");
            }
        }

        #endregion

        #region Api for Add And Update Permissions 
        /// <summary>
        /// API>>POST>>api/taskpermissons/permissionsallowdeny
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("permissionsallowdeny")]
        public async Task<IHttpActionResult> PermissionAllowDeny(AllowDenyPermissionRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkPermission = await _db.TaskPermissions
                        .FirstOrDefaultAsync(x => x.AssigneEmployeeId == model.AssigneEmployeeId
                                && x.CompanyId == tokenData.companyId && x.ProjectId == model.ProjectId);
                if (checkPermission == null)
                {
                    TaskPermissions obj = new TaskPermissions
                    {
                        ProjectId = model.ProjectId,
                        AssigneEmployeeId = model.AssigneEmployeeId,
                        CreatedBy = tokenData.employeeId,
                        CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                    };
                    _db.TaskPermissions.Add(obj);
                    await _db.SaveChangesAsync();
                    checkPermission = obj;
                }
                else
                {
                    checkPermission.UpdatedBy = tokenData.employeeId;
                    checkPermission.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                }
                checkPermission.IsCreateTask = model.IsCreateTask;
                checkPermission.IsDeleteTask = model.IsDeleteTask;
                checkPermission.IsApprovedTask = model.IsApprovedTask;
                checkPermission.IsExeclUploade = model.IsExeclUploade;
                checkPermission.IsReEvaluetTask = model.IsReEvaluetTask;
                checkPermission.IsUpdate = model.IsUpdate;
                checkPermission.IsOtherTaskCreate = model.IsOtherTaskCreate;
                checkPermission.IsBoardVisible = model.IsBoardVisible;
                checkPermission.ViewAlProjectTask = model.ViewAllProjectTask;
                checkPermission.CompanyId = tokenData.companyId;
                _db.Entry(checkPermission).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "Permissions Add Successfully !";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                res.Data = checkPermission;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("api/taskpermissons/permissionsallowdeny",
                            ex.Message, model);
                return BadRequest("Failed");
            }
        }

        public class AllowDenyPermissionRequest
        {
            public Guid TaskPermissionId { get; set; }
            public int ProjectId { get; set; }
            public int AssigneEmployeeId { get; set; }
            public bool IsCreateTask { get; set; }
            public bool IsDeleteTask { get; set; }
            public bool IsApprovedTask { get; set; }
            public bool IsExeclUploade { get; set; }
            public bool IsReEvaluetTask { get; set; }
            public bool IsUpdate { get; set; }
            public bool IsOtherTaskCreate { get; set; }
            public bool IsBoardVisible { get; set; }
            public bool ViewAllProjectTask { get; set; }
        }

        #endregion

        #region Api for Get EmployeeBy Id To For Check Permission
        /// <summary>
        /// API>>GET>>api/taskpermissons/getemployeeforassignpermissionbyid?EmployeeId?ProjectId
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployeeforassignpermissionbyid")]
        public async Task<IHttpActionResult> GetEmpDataForPermissions(int EmployeeId, int ProjectId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await (from p in _db.AssignProjects
                                     join e in _db.Employee on p.EmployeeId equals e.EmployeeId
                                     //join tp in _db.TaskPermissions on p.EmployeeId equals tp.AssigneEmployeeId into q
                                     //from result in q.DefaultIfEmpty()
                                     where p.IsActive && !p.IsDeleted && p.EmployeeId == EmployeeId && p.CompanyId == tokenData.companyId
                                     select new
                                     {

                                         EmployeeId = p.EmployeeId,
                                         EmployeeName = e.DisplayName,
                                         //IsCreateTask = result == null ? false: result.IsCreateTask,
                                         //IsDeleteTask = result == null ? false : result.IsDeleteTask,
                                         //IsApprovedTask = result == null ? false : result.IsApprovedTask,
                                         //IsExeclUploade = result == null ? false : result.IsExeclUploade,
                                         //IsReEvaluetTask = result == null ? false : result.IsReEvaluetTask,
                                         //IsUpdate = result == null ? false : result.IsUpdate,
                                         //IsOtherTaskCreate = result == null ? false : result.IsOtherTaskCreate,
                                         //IsBoardVisible = result == null ? false : result.IsBoardVisible,
                                         //ViewAlProjectTask = result == null ? false : result.ViewAlProjectTask,
                                         IsCreateTask = _db.TaskPermissions.Where(x => x.AssigneEmployeeId == EmployeeId && x.ProjectId == ProjectId).Select(x => x.IsCreateTask).FirstOrDefault(),
                                         IsDeleteTask = _db.TaskPermissions.Where(x => x.AssigneEmployeeId == EmployeeId && x.ProjectId == ProjectId).Select(x => x.IsDeleteTask).FirstOrDefault(),
                                         IsApprovedTask = _db.TaskPermissions.Where(x => x.AssigneEmployeeId == EmployeeId && x.ProjectId == ProjectId).Select(x => x.IsApprovedTask).FirstOrDefault(),
                                         IsExeclUploade = _db.TaskPermissions.Where(x => x.AssigneEmployeeId == EmployeeId && x.ProjectId == ProjectId).Select(x => x.IsExeclUploade).FirstOrDefault(),
                                         IsReEvaluetTask = _db.TaskPermissions.Where(x => x.AssigneEmployeeId == EmployeeId && x.ProjectId == ProjectId).Select(x => x.IsReEvaluetTask).FirstOrDefault(),
                                         IsUpdate = _db.TaskPermissions.Where(x => x.AssigneEmployeeId == EmployeeId && x.ProjectId == ProjectId).Select(x => x.IsUpdate).FirstOrDefault(),
                                         IsOtherTaskCreate = _db.TaskPermissions.Where(x => x.AssigneEmployeeId == EmployeeId && x.ProjectId == ProjectId).Select(x => x.IsOtherTaskCreate).FirstOrDefault(),
                                         IsBoardVisible = _db.TaskPermissions.Where(x => x.AssigneEmployeeId == EmployeeId && x.ProjectId == ProjectId).Select(x => x.IsBoardVisible).FirstOrDefault(),
                                         ViewAlProjectTask = _db.TaskPermissions.Where(x => x.AssigneEmployeeId == EmployeeId && x.ProjectId == ProjectId).Select(x => x.ViewAlProjectTask).FirstOrDefault(),
                                     }).FirstOrDefaultAsync();

                if (getData != null)
                {
                    res.Message = "Data Get Succesfully ";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = getData;
                    return Ok(res);
                }
                else
                {
                    res.Message = "Data Not Found ! ";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = getData;
                    return Ok(res);
                }

            }
            catch (Exception ex)
            {
                logger.Error("api/taskpermissons/getemployeeforassignpermissionbyid", ex.Message);
                return BadRequest("Failed");
            }
        }

        #endregion

        #region Api for Check Permission 
        /// <summary>
        /// API>>GET>>api/taskpermissons/checkpermissions?ProjectId
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("checkpermissions")]
        public async Task<IHttpActionResult> CheckPermission(int ProjectId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (_db.Employee.Any(x => x.EmployeeId == tokenData.employeeId && x.OrgId == 0))
                {
                    res.Message = "Data Get Successfully ";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Found;
                    res.Data = new TaskPermissionsRequestmodel
                    {
                        ProjectId = ProjectId,
                        ProjectName = "",
                        AssigneEmployeeId = tokenData.employeeId,
                        IsCreateTask = false,
                        IsDeleteTask = false,
                        IsApprovedTask = false,
                        IsExeclUploade = true,
                        IsReEvaluetTask = false,
                        IsUpdate = false,
                        IsOtherTaskCreate = false,
                        IsBoardVisible = true,
                        ViewAlProjectTask = true,
                    };
                    return Ok(res);
                }
                else
                {
                    var getData = await _db.TaskPermissions
                                        .Where(x => x.CompanyId == tokenData.companyId &&
                                        x.AssigneEmployeeId == tokenData.employeeId && x.ProjectId == ProjectId)
                                       .Select(x => new TaskPermissionsRequestmodel
                                       {
                                           TaskPermissionId = x.TaskPermissionId,
                                           ProjectId = x.ProjectId,
                                           ProjectName = _db.ProjectLists.Where(y => y.ID == ProjectId).Select(y => y.ProjectName).FirstOrDefault(),
                                           AssigneEmployeeId = x.AssigneEmployeeId,
                                           IsCreateTask = x.IsCreateTask,
                                           IsDeleteTask = x.IsDeleteTask,
                                           IsApprovedTask = x.IsApprovedTask,
                                           IsExeclUploade = x.IsExeclUploade,
                                           IsReEvaluetTask = x.IsReEvaluetTask,
                                           IsUpdate = x.IsUpdate,
                                           IsOtherTaskCreate = x.IsOtherTaskCreate,
                                           IsBoardVisible = x.IsBoardVisible,
                                           ViewAlProjectTask = x.ViewAlProjectTask,
                                       })
                                       .FirstOrDefaultAsync();
                    if (getData != null)
                    {
                        res.Message = "Data Get Succesfully ";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Found;
                        res.Data = getData;
                        return Ok(res);
                    }
                    else
                    {
                        res.Message = "Data Not Found ! ";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        res.Data = getData;
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {

                logger.Error("api/taskpermissons/checkpermissions", ex.Message);
                return BadRequest("Failed");
            }
        }

        public class TaskPermissionsRequestmodel
        {
            public Guid TaskPermissionId { get; set; } = Guid.Empty;
            public int ProjectId { get; set; }
            public string ProjectName { get; set; }
            public int AssigneEmployeeId { get; set; }
            public bool IsCreateTask { get; set; }
            public bool IsDeleteTask { get; set; }
            public bool IsApprovedTask { get; set; }
            public bool IsExeclUploade { get; set; }
            public bool IsReEvaluetTask { get; set; }
            public bool IsUpdate { get; set; }
            public bool IsOtherTaskCreate { get; set; }
            public bool IsBoardVisible { get; set; }
            public bool ViewAlProjectTask { get; set; }

        }
        #endregion

        #region API TO CHECK LOGIN USER IS MANAGER OR NOT
        /// <summary>
        /// Created By Harshit Mitra On 29/12/2022
        /// API>>GET>>api/taskpermissons/checkmanager
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("checkmanager")]
        public async Task<IHttpActionResult> CheckManager(int projectId)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getData = await _db.ProjectLists.FirstOrDefaultAsync(x => x.ID == projectId);
                if (getData != null)
                {
                    return Ok(getData.ProjectManager == tokenData.employeeId);
                }
                return Ok(false);
            }
            catch (Exception ex)
            {

                logger.Error("api/taskpermissons/checkmanager", ex.Message);
                return BadRequest("Failed");
            }
        }
        #endregion

    }
}
