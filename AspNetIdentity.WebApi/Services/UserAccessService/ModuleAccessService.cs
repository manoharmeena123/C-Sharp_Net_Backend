using AspNetIdentity.Core.Common;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Interface.IUserAccessService;
using AspNetIdentity.WebApi.Model.UserAccesPermission;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Req = AspNetIdentity.Core.ViewModel.UserAccessViewModel.RequestModuleAccessClass;
using Res = AspNetIdentity.Core.ViewModel.UserAccessViewModel.ResponseModuleAccessClass;

namespace AspNetIdentity.WebApi.Services.UserAccessService
{
    public class ModuleAccessService : IModuleAccessService
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        #endregion

        #region Constructor
        public ModuleAccessService()
        {
            _context = new ApplicationDbContext();
        }
        #endregion


        public async Task<ServiceResponse<List<Res.GetModuleSubModuleResponse>>> GetModuleList(ClaimsHelperModel tokenData)
        {
            var moduleList = await (from m in _context.ModuleAndSubmodules
                                    join ca in _context.CompaniesModuleAccesses on m.ModuleCode equals ca.ModuleCode
                                    where !m.IsDeleted && !m.IsSuperAdmin && ca.CompanyId == tokenData.companyId
                                    select m).ToListAsync();
            List<Res.GetModuleSubModuleResponse> response =
                moduleList.GroupBy(x => x.ModuleCode)
                    .Select(x => new Res.GetModuleSubModuleResponse
                    {
                        ModuleCode = x.Key,
                        ModuleName = x.First().ModuleName,
                        IsAccess = (x.Key == "UA1"),
                        SubModuleList = x.First().SubModuleCode != "" ?
                        x.Select(z => new Res.GetSubModuleListResponse
                        {
                            ModuleCode = x.Key,
                            SubModuleCode = z.SubModuleCode,
                            SubModuleName = z.SubModuleName,
                        })
                        .ToList()
                        : new List<Res.GetSubModuleListResponse>(),
                    })
                    .ToList();
            return new ServiceResponse<List<Res.GetModuleSubModuleResponse>>(HttpStatusCode.OK, response);
        }
        public async Task<ServiceResponse<bool>> CreateRoleWithPermission(ClaimsHelperModel tokenData, Req.CreateRoleViewClassRequest model)
        {
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.Now, tokenData.TimeZone);
            if (await _context.RoleInUserAccessPermissions.AnyAsync(x => x.RoleName == model.RoleName && x.CompanyId == tokenData.companyId))
                return new ServiceResponse<bool>(HttpStatusCode.NotAcceptable, "Already Exist With Same Name");
            RoleInUserAccessPermission rolObj = new RoleInUserAccessPermission
            {
                RoleName = model.RoleName,
                Description = model.Description,
                CompanyId = tokenData.companyId,
                CreatedOn = today,
                CreatedBy = tokenData.employeeId,
            };
            _context.RoleInUserAccessPermissions.Add(rolObj);

            List<PermissionInUserAccess> setPermissionInList = new List<PermissionInUserAccess>();
            foreach (var module in model.ModuleList)
            {
                if (module.SubModuleList.Count == 0 && (module.IsAccess || module.ModuleCode == "UA1"))
                {
                    var addPermission = new PermissionInUserAccess
                    {
                        UserAccessRoleId = rolObj.RoleId,
                        ModuleCode = module.ModuleCode,
                        ModuleName = module.ModuleName,
                        SubModuleCode = "",
                        SubModuleName = "",
                        IsAccess = true,
                        Btn1 = false,
                        Btn2 = false,
                        Btn3 = false,
                        Btn4 = false,
                        Btn5 = false,
                    };
                    setPermissionInList.Add(addPermission);
                }
                else
                {
                    var addPermission = module.SubModuleList
                        .Where(x => x.IsAccess)
                        .Select(x => new PermissionInUserAccess
                        {
                            UserAccessRoleId = rolObj.RoleId,
                            ModuleCode = module.ModuleCode,
                            ModuleName = module.ModuleName,
                            SubModuleCode = x.SubModuleCode,
                            SubModuleName = x.SubModuleName,
                            IsAccess = true,
                            Btn1 = false,
                            Btn2 = false,
                            Btn3 = false,
                            Btn4 = false,
                            Btn5 = false,
                        })
                        .ToList();
                    setPermissionInList.AddRange(addPermission);
                }
            }
            _context.PermissionInUserAccesses.AddRange(setPermissionInList);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>(HttpStatusCode.Created, true);
        }
        public async Task<ServiceResponse<Res.GetRoleAndPrmissionByIdResponse>> GetRoleAndPermissionById(ClaimsHelperModel tokenData, Guid roleId)
        {
            Res.GetRoleAndPrmissionByIdResponse getRole =
                await _context.RoleInUserAccessPermissions
                .Where(x => x.RoleId == roleId)
                .Select(x => new Res.GetRoleAndPrmissionByIdResponse
                {
                    RoleId = x.RoleId,
                    RoleName = x.RoleName,
                    Description = x.Description,
                    //ModuleList = new List<Res.GetModuleSubModuleResponse>()
                })
                .FirstOrDefaultAsync();
            var moduleList =
                await (from m in _context.ModuleAndSubmodules
                       join ca in _context.CompaniesModuleAccesses on m.ModuleCode equals ca.ModuleCode
                       where ca.CompanyId == tokenData.companyId
                       select new
                       {
                           m
                       })
                       .Distinct()
                       .ToListAsync();
            var permission =
                await _context.PermissionInUserAccesses
                .Where(y => y.UserAccessRoleId == roleId)
                .ToListAsync();
            List<Res.GetModuleSubModuleResponse> response =
                moduleList.GroupBy(x => x.m.ModuleCode)
                    .Select(x => new Res.GetModuleSubModuleResponse
                    {
                        ModuleCode = x.Key,
                        ModuleName = x.First().m.ModuleName,
                        IsAccess = permission.Where(y => y.ModuleCode == x.Key).Any(y => y.IsAccess),
                        SubModuleList = x.First().m.SubModuleCode != "" ?
                        x.Select(z => new Res.GetSubModuleListResponse
                        {
                            ModuleCode = x.Key,
                            SubModuleCode = z.m.SubModuleCode,
                            SubModuleName = z.m.SubModuleName,
                            IsAccess = permission.Where(y => y.SubModuleCode == z.m.SubModuleCode).Any(y => y.IsAccess)
                        })
                        .Distinct()
                        .ToList()
                        : new List<Res.GetSubModuleListResponse>(),
                    })
                    .Distinct()
                    .OrderBy(x => !x.IsAccess)
                    .ToList();
            getRole.ModuleList = response;
            return new ServiceResponse<Res.GetRoleAndPrmissionByIdResponse>(HttpStatusCode.OK, getRole);
        }
        public async Task<ServiceResponse<bool>> UpdateRoleWithPermission(ClaimsHelperModel tokenData, Req.UpdateRoleViewClassRequest model)
        {
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.Now, tokenData.TimeZone);
            var role = await _context.RoleInUserAccessPermissions
                .FirstOrDefaultAsync(x => x.RoleId == model.RoleId);
            if (await _context.RoleInUserAccessPermissions
                .AnyAsync(x => x.RoleName == model.RoleName &&
                    x.CompanyId == tokenData.companyId && x.RoleId != model.RoleId))
                return new ServiceResponse<bool>(HttpStatusCode.NotAcceptable, "Already Exist With Same Name");
            role.RoleName = model.RoleName;
            role.Description = model.Description;
            role.UpdatedOn = today;
            role.UpdatedBy = tokenData.employeeId;

            _context.Entry(role).State = EntityState.Modified;

            var removePermission =
                await _context.PermissionInUserAccesses
                .Where(x => x.UserAccessRoleId == model.RoleId)
                .ToListAsync();
            _context.PermissionInUserAccesses.RemoveRange(removePermission);

            List<PermissionInUserAccess> setPermissionInList = new List<PermissionInUserAccess>();
            foreach (var module in model.ModuleList)
            {
                if (module.SubModuleList.Count == 0 && (module.IsAccess || module.ModuleCode == "UA1"))
                {
                    var addPermission = new PermissionInUserAccess
                    {
                        UserAccessRoleId = role.RoleId,
                        ModuleCode = module.ModuleCode,
                        ModuleName = module.ModuleName,
                        SubModuleCode = "",
                        SubModuleName = "",
                        IsAccess = true,
                        Btn1 = false,
                        Btn2 = false,
                        Btn3 = false,
                        Btn4 = false,
                        Btn5 = false,
                    };
                    setPermissionInList.Add(addPermission);
                }
                else
                {
                    var addPermission = module.SubModuleList
                        .Where(x => x.IsAccess)
                        .Select(x => new PermissionInUserAccess
                        {
                            UserAccessRoleId = role.RoleId,
                            ModuleCode = module.ModuleCode,
                            ModuleName = module.ModuleName,
                            SubModuleCode = x.SubModuleCode,
                            SubModuleName = x.SubModuleName,
                            IsAccess = true,
                            Btn1 = false,
                            Btn2 = false,
                            Btn3 = false,
                            Btn4 = false,
                            Btn5 = false,
                        })
                        .ToList();
                    setPermissionInList.AddRange(addPermission);
                }
            }
            _context.PermissionInUserAccesses.AddRange(setPermissionInList);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>(HttpStatusCode.Accepted, true);
        }
    }
}