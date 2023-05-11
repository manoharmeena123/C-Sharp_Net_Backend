using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.UserAccesPermission;
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
using System.Web.Routing;
using System.Web.UI.WebControls;
using TextInfo = System.Globalization.TextInfo;

namespace AspNetIdentity.WebApi.Controllers.UIAccess
{
    /// <summary>
    /// Created By Ankit Jain On 14-11-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/useraccesspermissions")]
    public class UserAccessPermissionController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region This Api Use to Module and SubModule

        #region API TO Add Module And SubModule Admin Side
        /// <summary>
        /// Created By Ankit Jain On 17-11-2022
        /// API >> POST >> api/useraccesspermissions/addmodule
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addmodule")]
        public async Task<IHttpActionResult> AddModule(ModuleAndSubmodule model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var moduleandsubmodule = _db.ModuleAndSubmodules.Where(x => x.ModuleName == model.ModuleName
                && x.SubModuleName == model.SubModuleName).FirstOrDefault();
                if (moduleandsubmodule == null)
                {
                    _db.ModuleAndSubmodules.Add(model);
                    await _db.SaveChangesAsync();
                    res.Message = "Created";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                }
                else
                {
                    res.Message = "Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.Created;
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/useraccesspermissions/getallmoduleandsubmodule", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(model);
        }
        #endregion

        #region API TO Add Module And SubModule Super Admin Side
        /// <summary>
        /// Created By Ankit Jain On 17-11-2022
        /// API >> POST >> api/useraccesspermissions/createmodulesubmodule
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createmodulesubmodule")]
        public IHttpActionResult CreateModuleAndSubmodule()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                _ = UserAccessPermissionHelper.AddModuleLisDatat();
                res.Message = "Created";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                return Ok(res);

            }
            catch (Exception ex)
            {
                logger.Error("api/useraccesspermissions/createmodulesubmodule", ex.Message);
                return BadRequest("Failed");
            }
        }

        #endregion

        #region API TO Get Module And SubModule In Admin 
        /// <summary>
        /// Created By Ankit Jain On 15-11-2022
        /// API >> Get >> api/useraccesspermissions/getmodule
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getmodule")]
        public async Task<IHttpActionResult> GetModuleAsync()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var newModuleList = await (from x in _db.ModuleAndSubmodules
                                           join ca in _db.CompaniesModuleAccesses on x.ModuleCode equals ca.ModuleCode
                                           where !x.IsDeleted && !x.IsSuperAdmin && x.SubModuleCode != "UA17_1" &&
                                                x.SubModuleCode != "UA17_2" && x.SubModuleCode != "UA17_3" &&
                                                ca.CompanyId == tokenData.companyId
                                           select x).ToListAsync();

                var permissionList = newModuleList
                    .Select(x => new
                    {
                        x.ModuleName,
                        x.ModuleCode,
                    })
                    .Distinct()
                    .Select(x => new ModuleAndSubmoduleHelper
                    {
                        ModuleName = x.ModuleName,
                        ModuleCode = x.ModuleCode,
                        IsAccess = (x.ModuleCode == "UA1"),
                        Submodule = newModuleList.Where(y => y.ModuleCode == x.ModuleCode)
                        .Select(y => new SubmoduleHelper
                        {
                            ModuleCode = y.ModuleCode,
                            SubModuleName = y.SubModuleName,
                            SubModuleCode = y.SubModuleCode,
                            IsAccess = false,
                            Btn1 = false,
                            Btn2 = false,
                            Btn3 = false,
                            Btn4 = false,
                            Btn5 = false,
                        }).OrderBy(y => y.SubModuleCode).ToList(),
                    }).OrderBy(x => x.ModuleName).ToList();

                var data = permissionList;
                res.Message = "Get Module And Submodule In Admin Side";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                res.Data = data;
                return Ok(res);

            }
            catch (Exception ex)
            {
                logger.Error("api/useraccesspermissions/getmodule", ex.Message);
                return BadRequest("Failed");
            }
        }

        #endregion

        #region API TO GET All Module And Submodule In SuperAdmin
        /// <summary>
        /// Created By Ankit Jain On 17-11-2022
        /// API >> GET >> api/useraccesspermissions/getallmoduleandsubmodule
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallmoduleandsubmodule")]
        public async Task<IHttpActionResult> GetAll()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (tokenData.roleType == "SuperAdmin")
                {
                    var modulesubmodule = await _db.ModuleAndSubmodules.Where(x => !x.IsDeleted && x.IsSuperAdmin).ToListAsync();
                    var permissionList = modulesubmodule
                      .Select(x => new
                      {
                          x.ModuleName,
                          x.ModuleCode,
                      })
                      .Distinct()
                      .Select(x => new ModuleAndSubmoduleHelper
                      {
                          ModuleName = x.ModuleName,
                          ModuleCode = x.ModuleCode,
                          IsAccess = true,
                          Submodule = modulesubmodule.Where(y => y.ModuleCode == x.ModuleCode)
                          .Select(y => new SubmoduleHelper
                          {
                              ModuleCode = y.ModuleCode,
                              SubModuleName = y.SubModuleName,
                              SubModuleCode = y.SubModuleCode,
                              IsAccess = true,
                              Btn1 = true,
                              Btn2 = true,
                              Btn3 = true,
                              Btn4 = true,
                              Btn5 = true,
                          }).ToList(),
                      }).ToList();
                    if (modulesubmodule.Count > 0)
                    {
                        res.Message = "All SuperAdmin Module And Submodule";
                        res.Status = true;
                        res.Data = permissionList;
                        return Ok(res);
                    }
                    else
                    {
                        res.Message = "No SuperAdmin Module And Submodule Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = permissionList;
                        return Ok(res);
                    }
                }
                else
                {
                    var rollandPermission = await (from emp in _db.EmployeeInRoles
                                                   join rol in _db.RoleInUserAccessPermissions on emp.RoleId equals rol.RoleId
                                                   join per in _db.PermissionInUserAccesses on rol.RoleId equals per.UserAccessRoleId
                                                   join m in _db.CompaniesModuleAccesses on per.ModuleCode equals m.ModuleCode
                                                   where per.IsAccess && rol.IsActive && !rol.IsDeleted && m.CompanyId == tokenData.companyId &&
                                                        rol.CompanyId == tokenData.companyId && tokenData.employeeId == emp.EmployeeId
                                                   select new
                                                   {
                                                       RollId = rol.RoleId,
                                                       RoleName = rol.RoleName,
                                                       ModuleName = per.ModuleName,
                                                       ModuleCode = per.ModuleCode,
                                                       SubModuleName = per.SubModuleName,
                                                       SubModuleCode = per.SubModuleCode,
                                                       IsAccess = per.IsAccess,
                                                       Btn1 = per.Btn1,
                                                       Btn2 = per.Btn2,
                                                       Btn3 = per.Btn3,
                                                       Btn4 = per.Btn4,
                                                       Btn5 = per.Btn5,
                                                   }).ToListAsync();
                    var distintModule = rollandPermission
                        .Select(x => new
                        {
                            ModuleName = x.ModuleName,
                            ModuleCode = x.ModuleCode,

                        }).Distinct().ToList();

                    var permission = distintModule
                        .Select(x => new ModuleAndSubmoduleHelper
                        {
                            ModuleName = x.ModuleName,
                            ModuleCode = x.ModuleCode,
                            IsAccess = rollandPermission.Where(y => y.ModuleCode == x.ModuleCode).Any(y => y.IsAccess),
                            Submodule = rollandPermission.Where(y => y.ModuleCode == x.ModuleCode)
                                    .Select(y => new SubmoduleHelper
                                    {
                                        ModuleCode = y.ModuleCode,
                                        SubModuleName = y.SubModuleName,
                                        SubModuleCode = y.SubModuleCode,
                                        IsAccess = rollandPermission.Where(z => z.SubModuleCode == y.SubModuleCode).Any(z => z.IsAccess),
                                        Btn1 = rollandPermission.Where(z => z.SubModuleCode == y.SubModuleCode).Any(z => z.Btn1),
                                        Btn2 = rollandPermission.Where(z => z.SubModuleCode == y.SubModuleCode).Any(z => z.Btn2),
                                        Btn3 = rollandPermission.Where(z => z.SubModuleCode == y.SubModuleCode).Any(z => z.Btn3),
                                        Btn4 = rollandPermission.Where(z => z.SubModuleCode == y.SubModuleCode).Any(z => z.Btn4),
                                        Btn5 = rollandPermission.Where(z => z.SubModuleCode == y.SubModuleCode).Any(z => z.Btn5),
                                    }).ToList(),
                        }).ToList();

                    if (permission.Count > 0)
                    {
                        res.Message = "All Admin Module And Submodule";
                        res.Status = true;
                        res.Data = permission;
                        return Ok(res);
                    }
                    else
                    {
                        res.Message = "No Admin Module And Submodule Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = permission;
                        return Ok(res);
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error("api/useraccesspermissions/getallmoduleandsubmodule", ex.Message);
                return BadRequest("Failed");
            }
        }
        public class ModuleAndSubmoduleHelper
        {
            public string ModuleName { get; set; }
            public string ModuleCode { get; set; }
            public bool IsAccess { get; set; }
            public List<SubmoduleHelper> Submodule { get; set; }
        }

        public class SubmoduleHelper
        {
            public string ModuleCode { get; set; }
            public string SubModuleName { get; set; }
            public string SubModuleCode { get; set; }
            public bool IsAccess { get; set; }
            public bool Btn1 { get; set; }
            public bool Btn2 { get; set; }
            public bool Btn3 { get; set; }
            public bool Btn4 { get; set; }
            public bool Btn5 { get; set; }
        }
        #endregion

        #endregion

        #region This Api Use To Role Only

        #region API TO Get All Role 
        /// <summary>
        /// Created By Ankit Jain On 15-11-2022
        /// API >> Get >> api/useraccesspermissions/getallrole
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallrole")]
        public async Task<IHttpActionResult> GetAllRole(string searchString = null, int? page = null, int? count = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                var getallroll = await _db.RoleInUserAccessPermissions
                    .Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId /*&& x.RoleName.Contains(searchString)*/)
                    .Select(x => new GetRollHelperClass
                    {
                        RoleId = x.RoleId,
                        RoleName = x.RoleName,
                        Description = x.Description,
                        IsDefaultRole = x.IsDefaultCreated,
                        CountEmplyee = _db.EmployeeInRoles.Count(y => y.RoleId == x.RoleId && y.IsActive && !y.IsDeleted),
                        CreatedOn = x.CreatedOn
                    })
                    .OrderByDescending(x => x.CreatedOn)
                    .ToListAsync();

                if (getallroll.Count > 0)
                {
                    if (page.HasValue && count.HasValue && !String.IsNullOrEmpty(searchString))
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = getallroll.Count,
                            Counts = (int)count,
                            List = getallroll
                            .Where(x => x.RoleName.ToLower().Contains(searchString.ToLower()))
                            .Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                        res.Message = "Get All Role";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                    }
                    else if (page.HasValue && count.HasValue)
                    {

                        res.Data = new PaginationData
                        {
                            TotalData = getallroll.Count,
                            Counts = (int)count,
                            List = getallroll.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                        res.Message = "Get All Role";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                    }
                    else
                    {
                        res.Data = getallroll;
                    }

                }
                else
                {
                    res.Message = "Role List Is Empty";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = getallroll;
                    return Ok(res);
                }
            }
            catch (Exception)
            {
                //logger.Error("api/useraccesspermissions/getallrole", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class GetRollHelperClass
        {
            public Guid RoleId { get; set; }
            public string RoleName { get; set; }
            public string Description { get; set; }
            public bool IsDefaultRole { get; set; }
            public int CountEmplyee { get; set; }
            public DateTimeOffset CreatedOn { get; set; } = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
        }

        #endregion

        #region API TO Remove Role 
        /// <summary>
        /// Created By Ankit Jain On 22-11-2022
        /// API >> Delete >> api/useraccesspermissions/removerole
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("removerole")]
        public async Task<IHttpActionResult> RemoveRole(Guid roleId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    var employeedata = await _db.EmployeeInRoles.Where(x => x.IsActive && !x.IsDeleted
                    && x.RoleId == roleId).ToListAsync();
                    if (employeedata.Count == 0)
                    {
                        var roledata = await _db.RoleInUserAccessPermissions.Where(x => x.IsActive && !x.IsDeleted
                        && x.RoleId == roleId).FirstOrDefaultAsync();
                        if (roledata != null)
                        {
                            roledata.IsDeleted = true;
                            roledata.DeletedOn = DateTime.Now;
                            roledata.IsActive = false;

                            _db.Entry(roledata).State = EntityState.Modified;
                            await _db.SaveChangesAsync();

                            res.Message = "Remove Role";
                            res.Status = true;
                            res.StatusCode = HttpStatusCode.Created;
                        }
                        else
                        {
                            res.Message = "Not Role Found";
                            res.Status = false;
                            res.StatusCode = HttpStatusCode.NoContent;
                        }
                    }
                    else
                    {
                        res.Message = "Not Employee Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                    }
                }
            }
            catch (Exception)
            {
                //logger.Error("api/useraccesspermissions/addroleandpermisson", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }

        #endregion

        #endregion

        #region This Api Use To Only Permission

        # region API TO Get Module And SubModule Admin User Role and permission
        /// <summary>
        /// Created By Ankit Jain On 15-11-2022
        /// API >> Get >> api/useraccesspermissions/getallrollandpermission
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallrollandpermission")]
        public async Task<IHttpActionResult> getallrollandpermission()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getmodulesubmodule = await _db.PermissionInUserAccesses.Where(x => x.IsAccess).ToListAsync();
                var permissionList = getmodulesubmodule
                  .Select(x => new PermissionInUserAccess
                  {
                      ModuleName = x.ModuleName,
                      ModuleCode = x.ModuleCode,
                      IsAccess = false,
                      SubModuleName = x.SubModuleName,
                      SubModuleCode = x.SubModuleCode,
                  }).OrderBy(x => x.ModuleName).ToList();
                res.Message = "Get Data In Module And Submodule by Id";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                res.Data = permissionList;
                return Ok(res);

            }
            catch (Exception ex)
            {
                logger.Error("api/useraccesspermissions/getmodule", ex.Message);
                return BadRequest("Failed");
            }
        }

        #endregion

        #endregion

        #region This Api Use Only Role And Permission

        #region API TO Add Module And SubModule in Role And Permission
        /// <summary>
        /// Created By Ankit Jain On 15-11-2022
        /// API >> POST >> api/useraccesspermissions/addroleandpermisson
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addroleandpermisson")]
        public async Task<IHttpActionResult> AddRoleAndPermisson(RoleHelper model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    var roleData = await _db.RoleInUserAccessPermissions
                                  .FirstOrDefaultAsync(x => x.IsActive && !x.IsDeleted
                                   && x.RoleName == model.RoleName && x.CompanyId == tokenData.companyId);
                    if (roleData == null)
                    {
                        RoleInUserAccessPermission role = new RoleInUserAccessPermission
                        {
                            RoleName = model.RoleName,
                            Description = model.Description,
                            CompanyId = tokenData.companyId,
                            CreatedBy = tokenData.employeeId,
                            CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                        };
                        List<PermissionInUserAccess> objList = new List<PermissionInUserAccess>();

                        foreach (var item in model.Module)
                        {
                            if (item.ModuleCode == "UA1")
                            {
                                var addingPermission = item.Submodule
                                    .Select(item2 => new PermissionInUserAccess
                                    {
                                        UserAccessRoleId = role.RoleId,
                                        ModuleName = item.ModuleName,
                                        ModuleCode = item.ModuleCode,
                                        SubModuleCode = item2.SubModuleCode,
                                        SubModuleName = item2.SubModuleName,
                                        IsAccess = item.ModuleCode == "UA1" ? true : item2.IsAccess,
                                        Btn1 = item2.Btn1,
                                        Btn2 = item2.Btn2,
                                        Btn3 = item2.Btn3,
                                        Btn4 = item2.Btn4,
                                        Btn5 = item2.Btn5,
                                    })
                                    .ToList();
                                objList.AddRange(addingPermission);
                            }
                            var addPermission = item.Submodule
                                .Where(z => z.SubModuleCode != "")
                                .Select(item2 => new PermissionInUserAccess
                                {
                                    UserAccessRoleId = role.RoleId,
                                    ModuleName = item.ModuleName,
                                    ModuleCode = item.ModuleCode,
                                    SubModuleCode = item2.SubModuleCode,
                                    SubModuleName = item2.SubModuleName,
                                    IsAccess = item.ModuleCode == "UA1" ? true : item2.IsAccess,
                                    Btn1 = item2.Btn1,
                                    Btn2 = item2.Btn2,
                                    Btn3 = item2.Btn3,
                                    Btn4 = item2.Btn4,
                                    Btn5 = item2.Btn5,
                                })
                                .ToList();
                            if (addPermission.Count != 0)
                                objList.AddRange(addPermission);
                        }
                        _db.RoleInUserAccessPermissions.Add(role);
                        _db.PermissionInUserAccesses.AddRange(objList);
                        await _db.SaveChangesAsync();

                        res.Message = "Created";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                    }
                    else
                    {
                        res.Message = "Role Already Exist";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error("api/useraccesspermissions/addroleandpermisson", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }

        public class RoleHelper
        {
            public string RoleName { get; set; }
            public string Description { get; set; }
            public List<RolePermissionModuleHelper> Module { get; set; }
        }

        public class RolePermissionModuleHelper
        {
            public string ModuleName { get; set; }
            public string ModuleCode { get; set; }
            public bool IsAccess { get; set; }
            public List<RolePermissionSubmoduleHelper> Submodule { get; set; }
        }

        public class RolePermissionSubmoduleHelper
        {
            public string SubModuleName { get; set; }
            public string SubModuleCode { get; set; }
            public bool IsAccess { get; set; }
            public bool Btn1 { get; set; }
            public bool Btn2 { get; set; }
            public bool Btn3 { get; set; }
            public bool Btn4 { get; set; }
            public bool Btn5 { get; set; }
        }

        #endregion

        #region This Api Use To Update Roles And Permission Module And submodule
        /// <summary>
        /// Created By Ankit Jain On 22-11-2022
        /// API >> POST >> api/useraccesspermissions/updateroleandpermission
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updateroleandpermission")]
        public async Task<IHttpActionResult> UpdateRoleAndPermisson(UpdateRoleAndPermissionHelperClass model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    var roleName = await _db.RoleInUserAccessPermissions
                        .FirstOrDefaultAsync(x => x.RoleName.ToUpper() == model.RoleName.ToUpper() && x.CompanyId ==
                                tokenData.companyId && x.IsActive && !x.IsDeleted && x.RoleId != model.RoleId);
                    if (roleName == null)
                    {
                        var role = await _db.RoleInUserAccessPermissions
                            .FirstOrDefaultAsync(x => x.RoleId == model.RoleId && x.CompanyId ==
                                    tokenData.companyId && x.IsActive && !x.IsDeleted);
                        if (role != null)
                        {
                            role.RoleName = model.RoleName;
                            role.Description = model.Description;
                            role.UpdatedBy = tokenData.employeeId;
                            role.UpdatedOn = DateTime.UtcNow;
                            _db.Entry(role).State = EntityState.Modified;
                            await _db.SaveChangesAsync();

                            var rolepermission = _db.PermissionInUserAccesses.Where(y => y.UserAccessRoleId == role.RoleId).ToList();
                            if (rolepermission != null)
                            {
                                foreach (var demo in rolepermission)
                                {
                                    _db.PermissionInUserAccesses.Remove(demo);
                                    await _db.SaveChangesAsync();
                                }

                                List<PermissionInUserAccess> objList = new List<PermissionInUserAccess>();

                                foreach (var item in model.Module)
                                {
                                    if (item.ModuleCode == "UA1")
                                    {
                                        var addingPermission = item.Submodule
                                            .Select(item2 => new PermissionInUserAccess
                                            {
                                                UserAccessRoleId = role.RoleId,
                                                ModuleName = item.ModuleName,
                                                ModuleCode = item.ModuleCode,
                                                SubModuleCode = item2.SubModuleCode,
                                                SubModuleName = item2.SubModuleName,
                                                IsAccess = item.ModuleCode == "UA1" ? true : item2.IsAccess,
                                                Btn1 = item2.Btn1,
                                                Btn2 = item2.Btn2,
                                                Btn3 = item2.Btn3,
                                                Btn4 = item2.Btn4,
                                                Btn5 = item2.Btn5,
                                            })
                                            .ToList();
                                        objList.AddRange(addingPermission);
                                    }
                                    var addPermission = item.Submodule
                                        .Where(z => z.SubModuleCode != "")
                                        .Select(item2 => new PermissionInUserAccess
                                        {
                                            UserAccessRoleId = role.RoleId,
                                            ModuleName = item.ModuleName,
                                            ModuleCode = item.ModuleCode,
                                            SubModuleCode = item2.SubModuleCode,
                                            SubModuleName = item2.SubModuleName,
                                            IsAccess = item.ModuleCode == "UA1" ? true : item2.IsAccess,
                                            Btn1 = item2.Btn1,
                                            Btn2 = item2.Btn2,
                                            Btn3 = item2.Btn3,
                                            Btn4 = item2.Btn4,
                                            Btn5 = item2.Btn5,
                                        })
                                        .ToList();
                                    if (addPermission.Count != 0)
                                        objList.AddRange(addPermission);
                                }

                                _db.PermissionInUserAccesses.AddRange(objList);
                                await _db.SaveChangesAsync();

                                res.Message = "Updated";
                                res.Status = true;
                                res.StatusCode = HttpStatusCode.Accepted;
                            }
                            else
                            {
                                res.Message = "Permission Data Not Found";
                                res.Status = false;
                                res.StatusCode = HttpStatusCode.NoContent;
                            }
                        }
                        else
                        {
                            res.Message = "Data Not Found";
                            res.Status = false;
                            res.StatusCode = HttpStatusCode.NoContent;
                        }
                    }
                    else
                    {
                        res.Message = "Role Already Exist";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                    }
                }

                //}
            }
            catch (Exception ex)
            {
                logger.Error("api/useraccesspermissions/addroleandpermisson", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }

        public class UpdateRoleAndPermissionHelperClass : RoleHelper
        {
            public Guid RoleId { get; set; }
        }
        #endregion

        #region This Api Use To Get Roles And Permission Module And submodule By Role Id
        /// <summary>
        /// Created By Ankit Jain On 22-11-2022
        /// API >> Get >> api/useraccesspermissions/getroleandpermissionbyId
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getroleandpermissionbyId")]
        public async Task<IHttpActionResult> GetRoleAndPermissionById(Guid roleId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    var roll = await _db.RoleInUserAccessPermissions.FirstOrDefaultAsync(x => x.RoleId ==
                            roleId && x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId);
                    if (roll != null)
                    {
                        var moduleAndSubModule = await (from x in _db.ModuleAndSubmodules
                                                        join ca in _db.CompaniesModuleAccesses on x.ModuleCode equals ca.ModuleCode
                                                        where !x.IsDeleted && !x.IsSuperAdmin && x.SubModuleCode != "UA17_1" &&
                                                             x.SubModuleCode != "UA17_2" && x.SubModuleCode != "UA17_3" &&
                                                             ca.CompanyId == tokenData.companyId
                                                        select x).ToListAsync();
                        var permission = await _db.PermissionInUserAccesses
                            .Where(y => y.UserAccessRoleId == roll.RoleId && y.SubModuleCode != "UA17_1"
                                    && y.SubModuleCode != "UA17_2" && y.SubModuleCode != "UA17_3")
                            .ToListAsync();
                        RoleHelperClass response = new RoleHelperClass
                        {
                            RoleId = roll.RoleId,
                            RoleName = roll.RoleName,
                            Description = roll.Description,
                            IsDefaultRole = roll.IsDefaultCreated,
                            Module = moduleAndSubModule
                               .Select(x => new
                               {
                                   x.ModuleName,
                                   x.ModuleCode,
                               })
                               .Distinct()
                               .Select(x => new RolePermissionModuleHelperClass
                               {
                                   ModuleName = x.ModuleName,
                                   ModuleCode = x.ModuleCode,
                                   IsAccess = permission.Where(y => y.ModuleCode == x.ModuleCode).Any(y => y.IsAccess),
                                   Submodule = moduleAndSubModule
                                   .Where(y => y.ModuleCode == x.ModuleCode)
                                   .Select(y => new RolePermissionSubmoduleHelperClass
                                   {
                                       ModuleCode = y.ModuleCode,
                                       SubModuleName = y.SubModuleName,
                                       SubModuleCode = y.SubModuleCode,
                                       IsAccess = permission.Where(z => z.SubModuleCode == y.SubModuleCode).Select(z => z.IsAccess).FirstOrDefault(),
                                       Btn1 = permission.Where(z => z.SubModuleCode == y.SubModuleCode).Select(z => z.Btn1).FirstOrDefault(),
                                       Btn2 = permission.Where(z => z.SubModuleCode == y.SubModuleCode).Select(z => z.Btn2).FirstOrDefault(),
                                       Btn3 = permission.Where(z => z.SubModuleCode == y.SubModuleCode).Select(z => z.Btn3).FirstOrDefault(),
                                       Btn4 = permission.Where(z => z.SubModuleCode == y.SubModuleCode).Select(z => z.Btn4).FirstOrDefault(),
                                       Btn5 = permission.Where(z => z.SubModuleCode == y.SubModuleCode).Select(z => z.Btn5).FirstOrDefault(),
                                   })
                                   .OrderBy(y => y.SubModuleCode)
                                   .ToList(),
                               })
                               .OrderBy(x => x.ModuleName)
                               .ToList(),
                        };
                        res.Message = "Role Found";
                        res.Status = true;
                        res.Data = response;
                        return Ok(res);
                    }
                    else
                    {
                        res.Message = "Role Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = "";
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("api/useraccesspermissions/addroleandpermisson", ex.Message);
                return BadRequest("Failed");
            }
        }


        public class RoleHelperClass
        {
            public Guid RoleId { get; set; }
            public string RoleName { get; set; }
            public string Description { get; set; }
            public bool IsDefaultRole { get; set; }
            public List<RolePermissionModuleHelperClass> Module { get; set; }
        }

        public class RolePermissionModuleHelperClass
        {
            public string ModuleName { get; set; }
            public string ModuleCode { get; set; }
            public bool IsAccess { get; set; }
            public List<RolePermissionSubmoduleHelperClass> Submodule { get; set; }
        }

        public class RolePermissionSubmoduleHelperClass
        {
            public string ModuleCode { get; set; }
            public string SubModuleName { get; set; }
            public string SubModuleCode { get; set; }
            public bool IsAccess { get; set; }
            public bool Btn1 { get; set; }
            public bool Btn2 { get; set; }
            public bool Btn3 { get; set; }
            public bool Btn4 { get; set; }
            public bool Btn5 { get; set; }
        }
        #endregion

        #endregion

        #region This Api Use To Role And Employee

        #region API TO Add Role and Employee
        /// <summary>
        /// Created By Ankit Jain On 22-11-2022
        /// API >> POST >> api/useraccesspermissions/addroleandemployee
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addroleandemployee")]
        public async Task<IHttpActionResult> AddRoleAndEmployees(AddRoleAndEmployeeHelper model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    var roledata = await _db.RoleInUserAccessPermissions.Where(x => x.IsActive && !x.IsDeleted
                    && x.CompanyId == tokenData.companyId).ToListAsync();
                    if (roledata != null)
                    {
                        EmployeeInRole obj = new EmployeeInRole
                        {
                            RoleId = model.RoleId,
                            EmployeeId = model.EmployeeId,
                            CreatedBy = tokenData.employeeId,
                            CreatedOn = DateTime.Now,
                            CompanyId = tokenData.companyId,
                        };

                        _db.EmployeeInRoles.Add(obj);
                        await _db.SaveChangesAsync();

                        res.Message = "Created";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                    }
                    else
                    {
                        res.Message = "Not Created";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                    }
                }
            }
            catch (Exception)
            {
                //logger.Error("api/useraccesspermissions/addroleandpermisson", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }

        public class AddRoleAndEmployeeHelper
        {
            public Guid RoleId { get; set; }
            public int EmployeeId { get; set; }
        }

        #endregion

        #region API TO Remove Role  In  Employee
        /// <summary>
        /// Created By Ankit Jain On 22-11-2022
        /// API >> Delete >> api/useraccesspermissions/removeroleinemployee
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("removeroleinemployee")]
        public async Task<IHttpActionResult> RemoveRoleInEmployees(Guid roleId, int employeeId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    var employeedata = await _db.EmployeeInRoles.Where(x => x.IsActive && !x.IsDeleted
                    && x.RoleId == roleId && x.EmployeeId == employeeId).FirstOrDefaultAsync();
                    if (employeedata != null)
                    {
                        _db.Entry(employeedata).State = EntityState.Deleted;
                        await _db.SaveChangesAsync();

                        res.Message = "Remove Employee In Role";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;
                    }
                    else
                    {
                        res.Message = "Not Data Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                    }
                }
            }
            catch (Exception)
            {
                //logger.Error("api/useraccesspermissions/addroleandpermisson", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }

        #endregion

        #region API TO Get All Role and Employee
        /// <summary>
        /// Created By Ankit Jain On 15-11-2022
        /// API >> Get >> api/useraccesspermissions/getallemployeebyid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallemployeebyid")]
        public async Task<IHttpActionResult> GetAllEmployeeById(Guid roleId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employee = await (from e in _db.Employee
                                      join de in _db.Designation on e.DesignationId equals de.DesignationId
                                      where e.CompanyId == tokenData.companyId && e.IsActive && !e.IsDeleted
                                      select new EmployeeHeleper
                                      {
                                          EmployeeId = e.EmployeeId,
                                          DisplayName = e.DisplayName,
                                          DesignationName = de.DesignationName
                                      }).ToListAsync();
                if (employee.Count > 0)
                {
                    var employeeIdInRole = _db.EmployeeInRoles.Where(y => y.RoleId == roleId && y.IsActive && !y.IsDeleted).
                        Select(y => y.EmployeeId).ToList();


                    employee = employee.Where(x => !employeeIdInRole.Contains(x.EmployeeId)).ToList();
                    res.Message = "Employee List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = employee;
                }
                else
                {
                    res.Message = "Employee List Is Empty";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = employee;
                }

            }
            catch (Exception)
            {
                //logger.Error("api/useraccesspermissions/getallrole", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }
        public class EmployeeHeleper
        {
            public int EmployeeId { get; set; }
            public string DisplayName { get; set; }
            public string DesignationName { get; set; }
        }

        #endregion

        #region API TO Get All Role and Employee By Role Id
        /// <summary>
        /// Created By Ankit Jain On 15-11-2022
        /// API >> Get >> api/useraccesspermissions/getallemployeeandrolebyid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallemployeeandrolebyid")]
        public async Task<IHttpActionResult> GetAllEmployeeRoleById(Guid roleId, int? page = null, int? count = null, string search = null)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            try
            {
                var employee = await (from e in _db.Employee
                                      join de in _db.Designation on e.DesignationId equals de.DesignationId
                                      where e.CompanyId == tokenData.companyId && e.IsActive && !e.IsDeleted
                                      select new EmployeeHeleper
                                      {
                                          EmployeeId = e.EmployeeId,
                                          DisplayName = e.DisplayName,
                                          DesignationName = de.DesignationName
                                      }).ToListAsync();
                if (employee.Count > 0)
                {
                    var employeeIdInRole = _db.EmployeeInRoles.Where(y => y.RoleId == roleId && y.CompanyId == tokenData.companyId
                    && y.IsActive && !y.IsDeleted).Select(y => y.EmployeeId).ToList();
                    if (employeeIdInRole.Count > 0)
                    {
                        employee = employee.Where(x => employeeIdInRole.Contains(x.EmployeeId)).ToList();
                        if (page.HasValue && count.HasValue && !string.IsNullOrEmpty(search))
                        {
                            var text = textInfo.ToUpper(search);
                            res.Data = new PaginationData
                            {
                                TotalData = employee.Count,
                                Counts = (int)count,
                                List = employee.Where(x => x.DisplayName.ToUpper().Contains(text))
                                   .Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                            };
                            res.Message = "Get All Role";
                            res.Status = true;
                            res.StatusCode = HttpStatusCode.Created;
                        }
                        else if (page.HasValue && count.HasValue)
                        {
                            res.Data = new PaginationData
                            {
                                TotalData = employee.Count,
                                Counts = (int)count,
                                List = employee.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                            };
                            res.Message = "Get All Role";
                            res.Status = true;
                            res.StatusCode = HttpStatusCode.Created;
                        }
                        else
                        {
                            res.Data = employee;
                        }
                    }
                    else
                    {
                        res.Message = "Role List Is Empty";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                        res.Data = employeeIdInRole;

                    }
                }
                else
                {
                    res.Message = "Employee List Is Empty";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = employee;

                }

            }
            catch (Exception)
            {
                //logger.Error("api/useraccesspermissions/getallrole", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }

        #endregion

        #endregion

        #region API TO Add Module And SubModule Admin side and Role And Permission And Employee Role

        #region Add Default Administrator in admin side
        public void AddDefaultAdministratorRole(Employee emp)
        {
            RoleInUserAccessPermission role = new RoleInUserAccessPermission
            {
                RoleName = "Global",
                Description = "This Is Defult Admin Role",
                CompanyId = emp.CompanyId,
                CreatedBy = emp.CreatedBy,
                IsDefaultCreated = true,
                CreatedOn = DateTime.UtcNow,
            };
            var modulesubmodule = _db.ModuleAndSubmodules.Where(x => !x.IsDeleted && !x.IsSuperAdmin).ToList();
            var addData = modulesubmodule
                .Select(x => new PermissionInUserAccess
                {
                    UserAccessRoleId = role.RoleId,
                    ModuleName = x.ModuleName,
                    ModuleCode = x.ModuleCode,
                    SubModuleName = x.SubModuleName,
                    SubModuleCode = x.SubModuleCode,
                    IsAccess = !(x.SubModuleCode == "UA17_4" ||
                    x.SubModuleCode == "UA17_5"
                    || x.SubModuleCode == "UA17_6"
                    || x.SubModuleCode == "UA17_7"),
                    Btn1 = !(x.SubModuleCode == "UA17_4" ||
                    x.SubModuleCode == "UA17_5"
                    || x.SubModuleCode == "UA17_6"
                    || x.SubModuleCode == "UA17_7"),
                    Btn2 = !(x.SubModuleCode == "UA17_4" ||
                    x.SubModuleCode == "UA17_5"
                    || x.SubModuleCode == "UA17_6"
                    || x.SubModuleCode == "UA17_7"),
                    Btn3 = !(x.SubModuleCode == "UA17_4" ||
                    x.SubModuleCode == "UA17_5"
                    || x.SubModuleCode == "UA17_6"
                    || x.SubModuleCode == "UA17_7"),
                    Btn4 = !(x.SubModuleCode == "UA17_4" ||
                    x.SubModuleCode == "UA17_5"
                    || x.SubModuleCode == "UA17_6"
                    || x.SubModuleCode == "UA17_7"),
                    Btn5 = !(x.SubModuleCode == "UA17_4" ||
                    x.SubModuleCode == "UA17_5"
                    || x.SubModuleCode == "UA17_6"
                    || x.SubModuleCode == "UA17_7"),
                }).OrderBy(x => x.ModuleName).ToList();

            EmployeeInRole obj = new EmployeeInRole
            {
                EmployeeId = emp.EmployeeId,
                RoleId = role.RoleId,
                CompanyId = emp.CompanyId,
                CreatedBy = emp.CreatedBy,
                CreatedOn = DateTime.UtcNow,
            };
            _db.EmployeeInRoles.Add(obj);
            _db.RoleInUserAccessPermissions.Add(role);
            _db.PermissionInUserAccesses.AddRange(addData);
            _db.Entry(emp).State = EntityState.Modified;
            _db.SaveChanges();
            AddDefaultRole(emp);

        }
        #endregion

        #region Add Default Role In admin side
        public void AddDefaultRole(Employee emp)
        {
            RoleInUserAccessPermission role = new RoleInUserAccessPermission
            {
                RoleName = "Default Role",
                Description = "This Is Defult Role For All Users",
                CompanyId = emp.CompanyId,
                OrgId = emp.OrgId,
                CreatedBy = emp.CreatedBy,
                CreatedOn = DateTime.UtcNow,
                IsDefaultCreated = true,
            };
            var modulesubmodule = _db.ModuleAndSubmodules.Where(x => !x.IsDeleted && !x.IsSuperAdmin).ToList();
            var addData = modulesubmodule
                .Select(x => new PermissionInUserAccess
                {
                    UserAccessRoleId = role.RoleId,
                    ModuleName = x.ModuleName,
                    ModuleCode = x.ModuleCode,
                    SubModuleName = x.SubModuleName,
                    SubModuleCode = x.SubModuleCode,
                    IsAccess = (x.ModuleCode == "UA1" || x.SubModuleCode == "UA2_1" ||
                    x.SubModuleCode == "UA2_4" ||
                    x.SubModuleCode == "UA2_5" ||
                    x.SubModuleCode == "UA2_6"),
                    Btn1 = (x.ModuleCode == "UA1" || x.SubModuleCode == "UA2_1" ||
                    x.SubModuleCode == "UA2_4" ||
                    x.SubModuleCode == "UA2_5" ||
                    x.SubModuleCode == "UA2_6"),
                    Btn2 = (x.ModuleCode == "UA1" || x.SubModuleCode == "UA2_1" ||
                    x.SubModuleCode == "UA2_4" ||
                    x.SubModuleCode == "UA2_5" ||
                    x.SubModuleCode == "UA2_6"),
                    Btn3 = (x.ModuleCode == "UA1" || x.SubModuleCode == "UA2_1" ||
                    x.SubModuleCode == "UA2_4" ||
                    x.SubModuleCode == "UA2_5" ||
                    x.SubModuleCode == "UA2_6"),
                    Btn4 = (x.ModuleCode == "UA1" || x.SubModuleCode == "UA2_1" ||
                    x.SubModuleCode == "UA2_4" ||
                    x.SubModuleCode == "UA2_5" ||
                    x.SubModuleCode == "UA2_6"),
                    Btn5 = (x.ModuleCode == "UA1" || x.SubModuleCode == "UA2_1" ||
                    x.SubModuleCode == "UA2_4" ||
                    x.SubModuleCode == "UA2_5" ||
                    x.SubModuleCode == "UA2_6"),
                    IsDeletd = false,
                }).OrderBy(x => x.ModuleName).ToList();

            EmployeeInRole obj = new EmployeeInRole
            {
                EmployeeId = emp.EmployeeId,
                RoleId = role.RoleId,
                CompanyId = emp.CompanyId,
                OrgId = emp.OrgId,
                CreatedBy = emp.CreatedBy,
                CreatedOn = DateTime.UtcNow,
            };
            _db.EmployeeInRoles.Add(obj);
            _db.RoleInUserAccessPermissions.Add(role);
            _db.PermissionInUserAccesses.AddRange(addData);
            _db.SaveChanges();
        }
        #endregion

        #region This Api Use To Update Permission admin and role In Already exit Admin
        /// <summary>
        /// Created By Ankit Jain On 22-11-2022
        /// API >> Post >> api/useraccesspermissions/updatepermissionadminalready
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updatepermissionadminalready")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> UpdatePermissionAdminAlready(RolePermissionHelper model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    var admindata = await _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId != 0 && x.OrgId == 0
                     ).ToListAsync();
                    if (admindata.Count > 0)
                    {
                        foreach (var item in admindata)
                        {
                            var employeerole = _db.EmployeeInRoles.Where(x => x.EmployeeId == item.EmployeeId).FirstOrDefault();
                            if (employeerole != null)
                            {
                                var roledata = _db.RoleInUserAccessPermissions.Where(x => x.RoleId == employeerole.RoleId).FirstOrDefault();
                                if (roledata != null)
                                {
                                    var permissiondata = _db.PermissionInUserAccesses.Where(x => x.ModuleCode == model.ModuleCode
                                    && x.SubModuleCode == model.SubModuleCode).FirstOrDefault();
                                    if (permissiondata == null)
                                    {
                                        PermissionInUserAccess obj = new PermissionInUserAccess();
                                        obj.UserAccessRoleId = roledata.RoleId;
                                        obj.ModuleName = model.ModuleName;
                                        obj.SubModuleName = model.SubModuleName;
                                        obj.ModuleCode = model.ModuleCode;
                                        obj.SubModuleCode = model.SubModuleCode;
                                        obj.IsAccess = true;
                                        _db.PermissionInUserAccesses.Add(obj);
                                        await _db.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        permissiondata.UserAccessRoleId = roledata.RoleId;
                                        permissiondata.ModuleName = model.ModuleName;
                                        permissiondata.SubModuleName = model.SubModuleName;
                                        permissiondata.ModuleCode = model.ModuleCode;
                                        permissiondata.SubModuleCode = model.SubModuleCode;
                                        permissiondata.IsAccess = true;
                                        permissiondata.UpdatedOn = DateTime.Now;
                                        _db.Entry(permissiondata).State = EntityState.Modified;
                                        await _db.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    res.Message = "Not Data Found";
                                    res.Status = false;
                                    res.StatusCode = HttpStatusCode.NoContent;
                                }
                            }
                            else
                            {
                                res.Message = "Not Data Found";
                                res.Status = false;
                                res.StatusCode = HttpStatusCode.NoContent;
                            }
                        }
                    }
                    else
                    {
                        res.Message = "Not Data Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                    }
                }
            }
            catch (Exception)
            {
                //logger.Error("api/useraccesspermissions/updatepermissionadmin", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }

        public class RolePermissionHelper
        {
            public string ModuleName { get; set; }
            public string ModuleCode { get; set; }
            public string SubModuleName { get; set; }
            public string SubModuleCode { get; set; }
        }
        #endregion

        #region This Api Use To Update Permission and role admin and role In Already exit Admin
        /// <summary>
        /// Created By Ankit Jain On 10-01-2022
        /// API >> Post >> api/useraccesspermissions/updaterolepermissionadminalready
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updaterolepermissionadminalready")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> UpdateRolePermissionAdminAlready()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var admindata = await _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId != 0 && x.OrgId == 0
                 ).ToListAsync();
                if (admindata.Count > 0)
                {
                    foreach (var item2 in admindata)
                    {
                        var employeerole = _db.EmployeeInRoles.Where(x => x.EmployeeId == item2.EmployeeId).FirstOrDefault();
                        if (employeerole == null)
                        {
                            RoleInUserAccessPermission role = new RoleInUserAccessPermission
                            {

                                RoleName = "Global",
                                Description = "This Is Defult Admin Role",
                                CompanyId = item2.CompanyId,
                                CreatedBy = item2.CreatedBy,
                                IsDefaultCreated = true,
                                CreatedOn = DateTime.UtcNow,
                            };
                            var modulesubmodule = _db.ModuleAndSubmodules.Where(x => !x.IsDeleted && !x.IsSuperAdmin).ToList();
                            var addData = modulesubmodule
                                .Select(x => new PermissionInUserAccess
                                {
                                    UserAccessRoleId = role.RoleId,
                                    ModuleName = x.ModuleName,
                                    ModuleCode = x.ModuleCode,
                                    SubModuleName = x.SubModuleName,
                                    SubModuleCode = x.SubModuleCode,
                                    IsAccess = !(x.SubModuleCode == "UA17_4" ||
                                    x.SubModuleCode == "UA17_5"
                                    || x.SubModuleCode == "UA17_6"
                                    || x.SubModuleCode == "UA17_7"),
                                    Btn1 = !(x.SubModuleCode == "UA17_4" ||
                                    x.SubModuleCode == "UA17_5"
                                    || x.SubModuleCode == "UA17_6"
                                    || x.SubModuleCode == "UA17_7"),
                                    Btn2 = !(x.SubModuleCode == "UA17_4" ||
                                    x.SubModuleCode == "UA17_5"
                                    || x.SubModuleCode == "UA17_6"
                                    || x.SubModuleCode == "UA17_7"),
                                    Btn3 = !(x.SubModuleCode == "UA17_4" ||
                                    x.SubModuleCode == "UA17_5"
                                    || x.SubModuleCode == "UA17_6"
                                    || x.SubModuleCode == "UA17_7"),
                                    Btn4 = !(x.SubModuleCode == "UA17_4" ||
                                    x.SubModuleCode == "UA17_5"
                                    || x.SubModuleCode == "UA17_6"
                                    || x.SubModuleCode == "UA17_7"),
                                    Btn5 = !(x.SubModuleCode == "UA17_4" ||
                                    x.SubModuleCode == "UA17_5"
                                    || x.SubModuleCode == "UA17_6"
                                    || x.SubModuleCode == "UA17_7"),
                                }).OrderBy(x => x.ModuleName).ToList();

                            EmployeeInRole obj = new EmployeeInRole
                            {
                                EmployeeId = item2.EmployeeId,
                                RoleId = role.RoleId,
                                CompanyId = item2.CompanyId,
                                CreatedBy = item2.CreatedBy,
                                CreatedOn = DateTime.UtcNow,
                            };
                            _db.EmployeeInRoles.Add(obj);
                            _db.RoleInUserAccessPermissions.Add(role);
                            _db.PermissionInUserAccesses.AddRange(addData);

                            _db.SaveChanges();
                            //AddDefaultRole(item2);
                        }
                        else
                        {
                            var roledata = _db.RoleInUserAccessPermissions.Where(x => x.RoleId == employeerole.RoleId).FirstOrDefault();
                            {

                                roledata.RoleName = "Global";
                                roledata.Description = "This Is Defult Admin Role";
                                roledata.CompanyId = item2.CompanyId;
                                roledata.UpdatedBy = item2.UpdatedBy;
                                roledata.IsDefaultCreated = true;
                                roledata.UpdatedOn = DateTime.UtcNow;

                                _db.Entry(roledata).State = EntityState.Modified;
                                await _db.SaveChangesAsync();
                            }
                            var modulesubmodule = _db.ModuleAndSubmodules.Where(x => !x.IsDeleted && !x.IsSuperAdmin).ToList();
                            foreach (var demo in modulesubmodule)
                            {
                                var permissiondata = _db.PermissionInUserAccesses.Where(x => x.UserAccessRoleId == employeerole.RoleId).FirstOrDefault();
                                {
                                    permissiondata.UserAccessRoleId = roledata.RoleId;
                                    permissiondata.ModuleName = demo.ModuleName;
                                    permissiondata.ModuleCode = demo.ModuleCode;
                                    permissiondata.SubModuleName = demo.SubModuleName;
                                    permissiondata.SubModuleCode = demo.SubModuleCode;
                                    permissiondata.IsAccess = !(demo.SubModuleCode == "UA17_4" ||
                                    demo.SubModuleCode == "UA17_5"
                                    || demo.SubModuleCode == "UA17_6"
                                    || demo.SubModuleCode == "UA17_7");
                                    permissiondata.Btn1 = !(demo.SubModuleCode == "UA17_4" ||
                                    demo.SubModuleCode == "UA17_5"
                                    || demo.SubModuleCode == "UA17_6"
                                    || demo.SubModuleCode == "UA17_7");
                                    permissiondata.Btn2 = !(demo.SubModuleCode == "UA17_4" ||
                                    demo.SubModuleCode == "UA17_5"
                                    || demo.SubModuleCode == "UA17_6"
                                    || demo.SubModuleCode == "UA17_7");
                                    permissiondata.Btn3 = !(demo.SubModuleCode == "UA17_4" ||
                                    demo.SubModuleCode == "UA17_5"
                                    || demo.SubModuleCode == "UA17_6"
                                    || demo.SubModuleCode == "UA17_7");
                                    permissiondata.Btn4 = !(demo.SubModuleCode == "UA17_4" ||
                                    demo.SubModuleCode == "UA17_5"
                                    || demo.SubModuleCode == "UA17_6"
                                    || demo.SubModuleCode == "UA17_7");
                                    permissiondata.Btn5 = !(demo.SubModuleCode == "UA17_4" ||
                                    demo.SubModuleCode == "UA17_5"
                                    || demo.SubModuleCode == "UA17_6"
                                    || demo.SubModuleCode == "UA17_7");

                                    permissiondata.UpdatedOn = DateTime.UtcNow;

                                    _db.Entry(permissiondata).State = EntityState.Modified;
                                    await _db.SaveChangesAsync();
                                }

                            }
                            employeerole.EmployeeId = item2.EmployeeId;
                            employeerole.RoleId = roledata.RoleId;
                            employeerole.CompanyId = item2.CompanyId;
                            employeerole.UpdatedBy = item2.UpdatedBy;
                            employeerole.UpdatedOn = DateTime.UtcNow;

                            _db.Entry(employeerole).State = EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }
                    }
                }
                else
                {
                    res.Message = "Not Data Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                }

            }
            catch (Exception)
            {
                //logger.Error("api/useraccesspermissions/updatepermissionadmin", ex.Message);
                return BadRequest("Failed");
            }
            return Ok(res);
        }

        #endregion

        #region This Api Use To All Exiting Employee Add Defoult Role and Permission
        /// <summary>
        /// Created By Ankit Jain On 10-01-2022
        /// API >> Post >> api/useraccesspermissions/createadefaultroleindb
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createadefaultroleindb")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> AddRolePermissionInExitingEmployee()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var employeeData = await _db.Employee
                    .Where(x => x.IsActive && !x.IsDeleted
                        && x.CompanyId != 0 && x.OrgId != 0)
                    .Select(x => new
                    {
                        x.EmployeeId,
                        x.CompanyId,
                    })
                    .ToListAsync();
                if (employeeData.Count > 0)
                {
                    var moduleAndSubModule = _db.ModuleAndSubmodules.Where(x => !x.IsDeleted && !x.IsSuperAdmin).ToList();
                    var companyList = employeeData.Select(x => x.CompanyId).Distinct().ToList();
                    foreach (var item in companyList)
                    {
                        var company = _db.Company.FirstOrDefault(x => x.CompanyId == item);
                        Guid roleId = await _db.RoleInUserAccessPermissions
                            .Where(x => x.IsActive && !x.IsDeleted &&
                                x.CompanyId == item && x.IsDefaultCreated && !x.HeadRoleInCompany)
                            .Select(x => x.RoleId)
                            .FirstOrDefaultAsync();
                        if (roleId == Guid.Empty)
                        {
                            roleId = CreateDefaultRole(company, moduleAndSubModule);
                        }
                        var employeeRoleAssigning = employeeData
                            .Where(x => x.CompanyId == item)
                            .Select(x => new EmployeeInRole
                            {
                                EmployeeId = x.EmployeeId,
                                RoleId = roleId,
                                CompanyId = company.CompanyId,
                                CreatedOn = company.CreatedOn,
                            })
                            .ToList();

                        company.DefaultRole = roleId;
                        _db.Entry(company).State = EntityState.Modified;
                        _db.EmployeeInRoles.AddRange(employeeRoleAssigning);

                        await _db.SaveChangesAsync();
                    }
                    return Ok("Created");
                }
                return BadRequest("Failed");
            }
            catch (Exception ex)
            {
                logger.Error("api/useraccesspermissions/updatepermissionadmin", ex.Message);
                return BadRequest("Failed");
            }
        }
        public Guid CreateDefaultRole(Company company, List<ModuleAndSubmodule> moduleAndSubModule)
        {
            RoleInUserAccessPermission role = new RoleInUserAccessPermission
            {
                RoleName = "Default Role",
                Description = "This Is Default Role For All Users",
                CompanyId = company.CompanyId,
                IsDefaultCreated = true,
                CreatedOn = company.CreatedOn,
            };
            var addData = moduleAndSubModule
                .Select(x => new PermissionInUserAccess
                {
                    UserAccessRoleId = role.RoleId,
                    ModuleName = x.ModuleName,
                    ModuleCode = x.ModuleCode,
                    SubModuleName = x.SubModuleName,
                    SubModuleCode = x.SubModuleCode,
                    IsAccess = (
                        x.ModuleCode == "UA1" ||
                        x.SubModuleCode == "UA2_1" ||
                        x.SubModuleCode == "UA2_4" ||
                        x.SubModuleCode == "UA2_5" ||
                        x.SubModuleCode == "UA2_6" ||
                        x.SubModuleCode == "UA4_4" ||
                        x.SubModuleCode == "UA4_5" ||
                        x.SubModuleCode == "UA3_4" ||
                        x.SubModuleCode == "UA3_3"
                    ),
                    Btn1 = (
                        x.ModuleCode == "UA1" ||
                        x.SubModuleCode == "UA2_1" ||
                        x.SubModuleCode == "UA2_4" ||
                        x.SubModuleCode == "UA2_5" ||
                        x.SubModuleCode == "UA2_6" ||
                        x.SubModuleCode == "UA4_4" ||
                        x.SubModuleCode == "UA4_5" ||
                        x.SubModuleCode == "UA3_4" ||
                        x.SubModuleCode == "UA3_3"
                    ),
                    Btn2 = (
                        x.ModuleCode == "UA1" ||
                        x.SubModuleCode == "UA2_1" ||
                        x.SubModuleCode == "UA2_4" ||
                        x.SubModuleCode == "UA2_5" ||
                        x.SubModuleCode == "UA2_6" ||
                        x.SubModuleCode == "UA4_4" ||
                        x.SubModuleCode == "UA4_5" ||
                        x.SubModuleCode == "UA3_4" ||
                        x.SubModuleCode == "UA3_3"
                    ),
                    Btn3 = (
                        x.ModuleCode == "UA1" ||
                        x.SubModuleCode == "UA2_1" ||
                        x.SubModuleCode == "UA2_4" ||
                        x.SubModuleCode == "UA2_5" ||
                        x.SubModuleCode == "UA2_6" ||
                        x.SubModuleCode == "UA4_4" ||
                        x.SubModuleCode == "UA4_5" ||
                        x.SubModuleCode == "UA3_4" ||
                        x.SubModuleCode == "UA3_3"
                    ),
                    Btn4 = (
                        x.ModuleCode == "UA1" ||
                        x.SubModuleCode == "UA2_1" ||
                        x.SubModuleCode == "UA2_4" ||
                        x.SubModuleCode == "UA2_5" ||
                        x.SubModuleCode == "UA2_6" ||
                        x.SubModuleCode == "UA4_4" ||
                        x.SubModuleCode == "UA4_5" ||
                        x.SubModuleCode == "UA3_4" ||
                        x.SubModuleCode == "UA3_3"
                    ),
                    Btn5 = (
                        x.ModuleCode == "UA1" ||
                        x.SubModuleCode == "UA2_1" ||
                        x.SubModuleCode == "UA2_4" ||
                        x.SubModuleCode == "UA2_5" ||
                        x.SubModuleCode == "UA2_6" ||
                        x.SubModuleCode == "UA4_4" ||
                        x.SubModuleCode == "UA4_5" ||
                        x.SubModuleCode == "UA3_4" ||
                        x.SubModuleCode == "UA3_3"
                    ),
                    IsDeletd = false,
                }).OrderBy(x => x.ModuleName).ToList();
            _db.RoleInUserAccessPermissions.Add(role);
            _db.PermissionInUserAccesses.AddRange(addData);

            _db.SaveChanges();
            return role.RoleId;
        }
        #endregion

        #region This Api Use To All Exiting Employee Add Global Role and Permission
        /// <summary>
        /// Created By Ankit Jain On 10-01-2022
        /// API >> Post >> api/useraccesspermissions/createglobalroleindb
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createglobalroleindb")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> CreateGlobalRoleInDb()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var employeeData = await _db.Employee
                    .Where(x => x.IsActive && !x.IsDeleted
                        && x.CompanyId != 0 && x.OrgId == 0)
                    .Select(x => new
                    {
                        x.EmployeeId,
                        x.CompanyId,
                    })
                    .ToListAsync();
                if (employeeData.Count > 0)
                {
                    var moduleAndSubModule = _db.ModuleAndSubmodules.Where(x => !x.IsDeleted && !x.IsSuperAdmin).ToList();
                    var companyList = employeeData.Select(x => x.CompanyId).Distinct().ToList();
                    foreach (var item in companyList)
                    {
                        var company = _db.Company.FirstOrDefault(x => x.CompanyId == item);
                        Guid roleId = await _db.RoleInUserAccessPermissions
                            .Where(x => x.IsActive && !x.IsDeleted &&
                                x.CompanyId == item && x.IsDefaultCreated && x.HeadRoleInCompany)
                            .Select(x => x.RoleId)
                            .FirstOrDefaultAsync();
                        if (roleId == Guid.Empty)
                        {
                            roleId = CreateGlobalRole(company, moduleAndSubModule);
                        }
                        var employeeRoleAssigning = employeeData
                            .Where(x => x.CompanyId == item)
                            .Select(x => new EmployeeInRole
                            {
                                EmployeeId = x.EmployeeId,
                                RoleId = roleId,
                                CompanyId = company.CompanyId,
                                CreatedOn = company.CreatedOn,
                            })
                            .ToList();

                        _db.EmployeeInRoles.AddRange(employeeRoleAssigning);
                        await _db.SaveChangesAsync();
                    }
                    return Ok("Created");
                }
                return BadRequest("Failed");
            }
            catch (Exception ex)
            {
                logger.Error("api/useraccesspermissions/updatepermissionadmin", ex.Message);
                return BadRequest("Failed");
            }
        }
        public Guid CreateGlobalRole(Company company, List<ModuleAndSubmodule> moduleAndSubModule)
        {
            RoleInUserAccessPermission role = new RoleInUserAccessPermission
            {
                RoleName = "Global Role",
                Description = "This Is Default Admin Role",
                CompanyId = company.CompanyId,
                IsDefaultCreated = true,
                HeadRoleInCompany = true,
                CreatedOn = company.CreatedOn,
            };
            var addData = moduleAndSubModule
                .Select(x => new PermissionInUserAccess
                {
                    UserAccessRoleId = role.RoleId,
                    ModuleName = x.ModuleName,
                    ModuleCode = x.ModuleCode,
                    SubModuleName = x.SubModuleName,
                    SubModuleCode = x.SubModuleCode,
                    IsAccess = !(
                        x.SubModuleCode == "UA17_4" ||
                        x.SubModuleCode == "UA17_5" ||
                        x.SubModuleCode == "UA17_6" ||
                        x.SubModuleCode == "UA17_7"
                    ),
                    Btn1 = !(
                        x.SubModuleCode == "UA17_4" ||
                        x.SubModuleCode == "UA17_5" ||
                        x.SubModuleCode == "UA17_6" ||
                        x.SubModuleCode == "UA17_7"
                    ),
                    Btn2 = !(
                        x.SubModuleCode == "UA17_4" ||
                        x.SubModuleCode == "UA17_5" ||
                        x.SubModuleCode == "UA17_6" ||
                        x.SubModuleCode == "UA17_7"
                    ),
                    Btn3 = !(
                        x.SubModuleCode == "UA17_4" ||
                        x.SubModuleCode == "UA17_5" ||
                        x.SubModuleCode == "UA17_6" ||
                        x.SubModuleCode == "UA17_7"
                    ),
                    Btn4 = !(
                        x.SubModuleCode == "UA17_4" ||
                        x.SubModuleCode == "UA17_5" ||
                        x.SubModuleCode == "UA17_6" ||
                        x.SubModuleCode == "UA17_7"
                    ),
                    Btn5 = !(
                        x.SubModuleCode == "UA17_4" ||
                        x.SubModuleCode == "UA17_5" ||
                        x.SubModuleCode == "UA17_6" ||
                        x.SubModuleCode == "UA17_7"
                    ),
                }).OrderBy(x => x.ModuleName).ToList();

            _db.RoleInUserAccessPermissions.Add(role);
            _db.PermissionInUserAccesses.AddRange(addData);
            _db.SaveChanges();
            return role.RoleId;
        }
        #endregion


        #endregion

        #region Super Admin helpermethoud

        public static List<AddGetModuleClass> GetSuperAdminModuleClass()
        {
            List<AddGetModuleClass> superadminodule = new List<AddGetModuleClass>();

            superadminodule.Add(new AddGetModuleClass
            {
                ModuleName = "Home",
                ModuleCode = "SA1",
                IsAccess = true,
                IsSuperAdmin = true,
                SubModule = new List<AddGetSubModuleClass>(),
            });

            superadminodule.Add(new AddGetModuleClass
            {
                ModuleName = "Company",
                ModuleCode = "SA2",
                IsAccess = true,
                IsSuperAdmin = true,
                SubModule = new List<AddGetSubModuleClass>()
                {
                    new AddGetSubModuleClass
                    {
                        SubModuleName= "Company Details",
                        SubModuleCode = "SA2_1",
                        ModuleCode = "SA2",
                        IsSuperAdmin = true,
                        IsAccess = true,
                        Btn1 = true,
                        Btn2 = true,
                        Btn3 = true,
                        Btn4 = true,
                        Btn5 = true,
                    },
                },
            });

            superadminodule.Add(new AddGetModuleClass
            {
                ModuleName = "Lead-Type",
                ModuleCode = "SA3",
                IsAccess = true,
                IsSuperAdmin = true,
                SubModule = new List<AddGetSubModuleClass>()
                {
                    new AddGetSubModuleClass
                    {
                        SubModuleName= "Lead List",
                        SubModuleCode = "SA3_1",
                        ModuleCode = "SA3",
                        IsSuperAdmin = true,
                        IsAccess = true,
                        Btn1 = true,
                        Btn2 = true,
                        Btn3 = true,
                        Btn4 = true,
                        Btn5 = true,
                    },
                },
            });

            superadminodule.Add(new AddGetModuleClass
            {
                ModuleName = "Address",
                ModuleCode = "SA4",
                IsAccess = true,
                IsSuperAdmin = true,
                SubModule = new List<AddGetSubModuleClass>(),
            });

            superadminodule.Add(new AddGetModuleClass
            {
                ModuleName = "Business",
                ModuleCode = "SA5",
                IsAccess = true,
                IsSuperAdmin = true,
                SubModule = new List<AddGetSubModuleClass>(),
            });

            superadminodule.Add(new AddGetModuleClass
            {
                ModuleName = "Tax Master",
                ModuleCode = "SA6",
                IsAccess = true,
                IsSuperAdmin = true,
                SubModule = new List<AddGetSubModuleClass>()
                {
                    new AddGetSubModuleClass
                    {
                        SubModuleName= "Professional Tax",
                        SubModuleCode = "SA6_1",
                        ModuleCode = "SA6",
                        IsSuperAdmin = true,
                        IsAccess = true,
                        Btn1 = true,
                        Btn2 = true,
                        Btn3 = true,
                        Btn4 = true,
                        Btn5 = true,
                    },
                },
            });

            superadminodule.Add(new AddGetModuleClass
            {
                ModuleName = "FAQ",
                ModuleCode = "SA7",
                IsAccess = true,
                IsSuperAdmin = true,
                SubModule = new List<AddGetSubModuleClass>()
                {
                    new AddGetSubModuleClass
                    {
                        SubModuleName= "My Page",
                        SubModuleCode = "SA7_1",
                        ModuleCode = "SA7",
                        IsSuperAdmin = true,
                        IsAccess = true,
                        Btn1 = true,
                        Btn2 = true,
                        Btn3 = true,
                        Btn4 = true,
                        Btn5 = true,
                    },
                    new AddGetSubModuleClass
                    {
                        SubModuleName= "Drafts",
                        SubModuleCode = "SA7_2",
                        ModuleCode = "SA7",
                        IsSuperAdmin = true,
                        IsAccess = true,
                        Btn1 = true,
                        Btn2 = true,
                        Btn3 = true,
                        Btn4 = true,
                        Btn5 = true,
                    },
                },
            });

            return superadminodule;
        }

        public class AddGetModuleClass
        {
            public string ModuleName { get; set; }
            public string ModuleCode { get; set; }
            public bool IsAccess { get; set; }
            public bool IsSuperAdmin { get; set; }

            public List<AddGetSubModuleClass> SubModule { get; set; }

        }
        public class AddGetSubModuleClass
        {
            public string SubModuleName { get; set; }
            public string SubModuleCode { get; set; }
            public bool Btn1 { get; set; }
            public bool Btn2 { get; set; }
            public bool Btn3 { get; set; }
            public bool Btn4 { get; set; }
            public bool Btn5 { get; set; }
            public bool View { get; set; }
            public string ModuleCode { get; set; }
            public bool IsAccess { get; set; }
            public bool IsSuperAdmin { get; set; }
            public bool Write { get; set; }
        }

        #endregion Super Admin helpermethoud

        #region API TO IMPORT ROLES IN EMPLOYEES
        /// <summary>
        /// Created By Harshit Mitra On 08/12/2022
        /// API >> POST >> api/useraccesspermissions/importroleinemploye
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("importroleinemploye")]
        public async Task<IHttpActionResult> ImportRoleInEmploye(List<EmportRoleInEmployeeRequest> model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    return BadRequest("Model Is Invalid");
                }
                if (model.Count == 0)
                {
                    res.Message = "Sheet Is Empty";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotAcceptable;
                    return Ok(res);
                }
                List<FaultyReportsResponse> faultyReport = new List<FaultyReportsResponse>();
                List<EmployeeInRole> objList = new List<EmployeeInRole>();
                var checkRole = await _db.EmployeeInRoles.Where(x => x.CompanyId == tokenData.companyId).ToListAsync();
                var employeeList = await _db.Employee
                    .Where(x => x.CompanyId == tokenData.companyId)
                    .Select(x => new
                    {
                        x.EmployeeId,
                        x.OfficeEmail,
                    })
                    .ToListAsync();

                var roles = await _db.RoleInUserAccessPermissions
                    .Where(x => x.CompanyId == tokenData.companyId)
                    .Select(x => new
                    {
                        x.RoleId,
                        x.RoleName,
                    })
                    .ToListAsync();
                foreach (var item in model)
                {
                    var employee = employeeList.FirstOrDefault(x => x.OfficeEmail.ToUpper() == item.OfficalEmail.ToUpper());
                    if (employee == null)
                    {
                        faultyReport.Add(new FaultyReportsResponse
                        {
                            OfficalEmail = item.OfficalEmail,
                            RoleName = item.RoleName,
                            Reason = "Employee Email Is Invalid Or Not Found",
                        });
                        continue;
                    }
                    var role = roles.FirstOrDefault(x => x.RoleName.ToUpper() == item.RoleName.ToUpper());
                    if (role == null)
                    {
                        faultyReport.Add(new FaultyReportsResponse
                        {
                            OfficalEmail = item.OfficalEmail,
                            RoleName = item.RoleName,
                            Reason = "Role Is Invalid Or Not Found",
                        });
                        continue;
                    }
                    if (checkRole.Any(x => x.EmployeeId == employee.EmployeeId && x.RoleId == role.RoleId))
                    {
                        faultyReport.Add(new FaultyReportsResponse
                        {
                            OfficalEmail = item.OfficalEmail,
                            RoleName = item.RoleName,
                            Reason = "Employee Already Exist In This Role",
                        });
                        continue;
                    }
                    objList.Add(new EmployeeInRole
                    {
                        RoleId = role.RoleId,
                        EmployeeId = employee.EmployeeId,
                        CompanyId = tokenData.companyId,
                        CreatedBy = tokenData.employeeId,
                        CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                    });
                }
                if (objList.Count > 0)
                {
                    _db.EmployeeInRoles.AddRange(objList);
                    await _db.SaveChangesAsync();
                }
                res.Message = objList.Count == model.Count ? "All Roles Imported" :
                    (objList.Count == 0 ? "No Data Imported" : "Imported With Faulty Data");
                res.StatusCode = HttpStatusCode.OK;
                res.Status = true;
                res.Data = new
                {
                    Count = faultyReport.Count,
                    Data = faultyReport,
                };

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/useraccesspermissions/importroleinemploye | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class EmportRoleInEmployeeRequest
        {
            public string OfficalEmail { get; set; } = String.Empty;
            public string RoleName { get; set; } = String.Empty;
        }
        public class FaultyReportsResponse : EmportRoleInEmployeeRequest
        {
            public string Reason { get; set; } = String.Empty;
        }

        #endregion
    }
}
