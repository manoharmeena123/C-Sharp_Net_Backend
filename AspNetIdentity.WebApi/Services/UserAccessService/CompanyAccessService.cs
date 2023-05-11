using AspNetIdentity.Core.Common;
using AspNetIdentity.Core.Model.UserAccesPermission;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Interface.IUserAccessService;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Req = AspNetIdentity.Core.ViewModel.UserAccessViewModel.RequestCompanyAccess;
using Res = AspNetIdentity.Core.ViewModel.UserAccessViewModel.ResponseCompanyAccess;

namespace AspNetIdentity.WebApi.Services.UserAccessService
{
    /// <summary>
    /// Created By Harshit Mitra On 03-04-2023
    /// </summary>
    public class CompanyAccessService : ICompanyAccessService
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        #endregion

        #region Constructor
        public CompanyAccessService()
        {
            _context = new ApplicationDbContext();
        }
        #endregion

        /// <summary>
        /// Created By Harshit Mitra On 03-04-2023
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<Res.GetCompanyAccessResponse>> GetModuleList(int companyId)
        {
            try
            {

                Res.GetCompanyDetailsResponse companyObj = await _context.Company
                    .Where(x => x.CompanyId == companyId)
                    .Select(x => new Res.GetCompanyDetailsResponse
                    {
                        RegisterCompanyName = x.RegisterCompanyName,
                        CompanyGst = x.CompanyGst,
                        RegisterAddress = x.RegisterAddress,
                        RegisterEmail = x.RegisterEmail
                    })
                    .FirstOrDefaultAsync();
                List<CompaniesModuleAccess> checkModule = await _context.CompaniesModuleAccesses
                    .Where(x => x.CompanyId == companyId)
                    .ToListAsync();
                List<Res.GetComanyRoleResponse> module = await _context.ModuleAndSubmodules
                    .Where(x => !x.IsSuperAdmin && !x.IsDeleted)
                    .Select(x => new Res.GetComanyRoleResponse
                    {
                        ModuleCode = x.ModuleCode,
                        ModuleName = x.ModuleName,
                        ModulePathURL = x.ModulePathURL,
                        CheckBox = false,
                    })
                    .Distinct()
                    .ToListAsync();
                module = module
                .Select(x => new Res.GetComanyRoleResponse
                {
                    ModuleCode = x.ModuleCode,
                    ModuleName = x.ModuleName,
                    ModulePathURL = x.ModulePathURL,
                    CheckBox = (checkModule.AsEnumerable().Any(z => z.ModuleCode == x.ModuleCode) ||
                            x.ModuleCode == "UA1" || x.ModuleCode == "UA16"),
                })
                .Distinct()
                .OrderByDescending(x => x.CheckBox)
                .ThenBy(x => x.ModuleName)
                .ToList();
                Res.GetCompanyAccessResponse responseObj = new Res.GetCompanyAccessResponse
                {
                    CompanyDetails = companyObj,
                    ModuleAccess = module,
                };
                return new ServiceResponse<Res.GetCompanyAccessResponse>(HttpStatusCode.OK, responseObj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Created By Harshit Mitra On 03-04-2023
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> UpdateCompanyAccess(Req.UpdateCompanyAccessRequest model)
        {
            try
            {
                List<CompaniesModuleAccess> listObject = model.ModuleAccess.Where(x => x.CheckBox)
                    .Select(x => new CompaniesModuleAccess
                    {
                        CompanyId = model.CompanyId,
                        ModuleCode = x.ModuleCode,
                        ModuleName = x.ModuleName,
                        ModulePathURL = x.ModulePathURL,

                    }).ToList();
                List<CompaniesModuleAccess> removeAccess = await _context.CompaniesModuleAccesses
                    .Where(x => x.CompanyId == model.CompanyId)
                    .ToListAsync();
                if (removeAccess.Count != 0)
                    _context.CompaniesModuleAccesses.RemoveRange(removeAccess);
                _context.CompaniesModuleAccesses.AddRange(listObject);
                await _context.SaveChangesAsync();
                return new ServiceResponse<bool>(HttpStatusCode.Accepted, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ServiceResponse<bool>> SetPermissionToAllCompanies()
        {
            var modules = await _context.ModuleAndSubmodules
                .Where(x => !x.IsDeleted && !x.IsSuperAdmin)
                .Select(x => new
                {
                    x.ModuleCode,
                    x.ModuleName,
                })
                .Distinct()
                .ToListAsync();
            var companies = await _context.Company.ToListAsync();
            List<CompaniesModuleAccess> addData = new List<CompaniesModuleAccess>();
            foreach (var item in companies)
            {
                var companiesModules = modules
                    .Select(x => new CompaniesModuleAccess
                    {
                        CompanyId = item.CompanyId,
                        ModuleCode = x.ModuleCode,
                        ModuleName = x.ModuleName,
                        ModulePathURL = "",
                    })
                    .ToList();
                addData.AddRange(companiesModules);
            }
            _context.CompaniesModuleAccesses.AddRange(addData);
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(HttpStatusCode.Created, true);
        }
    }
}