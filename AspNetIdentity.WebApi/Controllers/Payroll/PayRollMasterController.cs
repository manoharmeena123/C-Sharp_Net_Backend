//using AspNetIdentity.WebApi.Helper;
//using AspNetIdentity.WebApi.Infrastructure;
//using AspNetIdentity.WebApi.Model;
//using AspNetIdentity.WebApi.Model.PayRollModel;
//using AspNetIdentity.WebApi.Models;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using System.Web.Http;
//using static AspNetIdentity.WebApi.Model.EnumClass;

//namespace AspNetIdentity.WebApi.Controllers.Payroll
//{
//    /// <summary>
//    /// Created By Harshit Mitra on 21-02-2022
//    /// </summary>
//    [Authorize]
//    [RoutePrefix("api/payroll")]
//    public class PayRollMasterController : ApiController
//    {
//        public readonly ApplicationDbContext _db = new ApplicationDbContext();

//        ///----------- Pay Roll Main Screen -----------///

//        #region Api To Get PayRoll Setup Screen

//        /// <summary>
//        /// API >> Get >> api/payroll/payrollsetup
//        /// Created By Harshit Mitra on 21-02-2022
//        /// Changes - Apply check is previous step completed
//        /// Changed By Suraj Bundel on 09-06-2022
//        /// </summary>
//        /// <param name="payGroupId"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("payrollsetup")]
//        public async Task<ResponseBodyModel> GetPayrollSetup(int payGroupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            GetPayRollMainScreenResponseModelClass obj = new GetPayRollMainScreenResponseModelClass();
//            try
//            {
//                var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.IsActive == true &&
//                        x.IsDeleted == false && x.PayGroupId == payGroupId);
//                if (payGroup != null)
//                {
//                    var payRollSetup = await _db.PayRollSetups.Where(x => x.IsActive == true &&
//                            x.IsDeleted == false && x.PayGroupId == payGroup.PayGroupId).ToListAsync();
//                    if (payRollSetup != null)
//                    {
//                        List<GetPayRollSetupModelClass> list = new List<GetPayRollSetupModelClass>();
//                        obj.PayGroupId = payGroup.PayGroupId;
//                        obj.PayGroupName = payGroup.PayGroupName;
//                        obj.Description = payGroup.Description;
//                        obj.EmployeeCount = _db.Employee.Where(x => x.PayGroupId == payGroupId).ToList().Count;
//                        GetPayRollSetupModelClass oldStep = new GetPayRollSetupModelClass();
//                        foreach (var item in payRollSetup)
//                        {
//                            GetPayRollSetupModelClass innerObj = new GetPayRollSetupModelClass
//                            {
//                                Step = item.Step,
//                                Title = item.Title,
//                                Status = item.Status,
//                                IsCompleted = (item.Status == Enum.GetName(typeof(PayrollSetupStatus),
//                                                (int)PayrollSetupStatus.COMPLETED)),
//                                IsPrevious = (item.Status == Enum.GetName(typeof(PayrollSetupStatus),
//                                                    (int)PayrollSetupStatus.COMPLETED))
//                            };
//                            if (innerObj.Step != 1)
//                                innerObj.IsPrevious = oldStep.IsCompleted;
//                            oldStep = innerObj;
//                            list.Add(innerObj);
//                        }
//                        obj.SetupList = list.OrderBy(x => x.Step).ToList();

//                        res.Message = "List Found";
//                        res.Status = true;
//                        res.Data = obj;
//                    }
//                    else
//                    {
//                        res.Message = "Setup Not Found";
//                        res.Status = false;
//                    }
//                }
//                else
//                {
//                    res.Message = "Pay Group Not Found";
//                    res.Status = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get PayRoll Setup Screen

//        ///----------- Pay Roll Settings Company Info --------------///

//        #region Api To Add and Update Company Info

//        /// <summary>
//        /// Created By Harshit Mitra on 25-03-2022
//        /// API >> Post >> api/payroll/addupdatecompanyinfo
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("addupdatecompanyinfo")]
//        public async Task<ResponseBodyModel> AddCompanyInfo(CompanyInformation model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                if (model == null)
//                {
//                    res.Message = "Invalid Model";
//                    res.Status = false;
//                }
//                else
//                {
//                    var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId &&
//                            x.CompanyId == claims.companyId && x.OrgId == claims.orgId);
//                    if (payGroup != null)
//                    {
//                        var payRollSetup_CompanyInfo = await _db.PayRollSetups.Where(x => x.IsActive == true && x.IsDeleted == false &&
//                                    x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.CompanyInfo).FirstOrDefaultAsync();
//                        if (payRollSetup_CompanyInfo != null)
//                        {
//                            var companyInfo = await _db.CompanyInformations.FirstOrDefaultAsync(x => x.PayGroupId == payGroup.PayGroupId);
//                            if (companyInfo == null)
//                            {
//                                CompanyInformation obj = new CompanyInformation
//                                {
//                                    PayGroupId = model.PayGroupId,
//                                    SignatoryUrl = model.SignatoryUrl,
//                                    LegalNameOfCompany = model.LegalNameOfCompany,
//                                    CompanyIdentifyNumber = model.CompanyIdentifyNumber,
//                                    DateOfIncorporation = model.DateOfIncorporation,
//                                    TypeOfBusinessId = model.TypeOfBusinessId,
//                                    SectorId = model.SectorId,
//                                    NatureOfBusinessId = model.NatureOfBusinessId,
//                                    AddressLine1 = model.AddressLine1,
//                                    AddressLine2 = model.AddressLine2,
//                                    CityId = model.CityId,
//                                    StateId = model.StateId,
//                                    ZipCode = model.ZipCode,
//                                    CountryId = model.CountryId,
//                                    IsActive = true,
//                                    IsDeleted = false,
//                                    CreatedBy = claims.userId,
//                                    CreatedOn = DateTime.Now,
//                                    CompanyId = claims.companyId,
//                                    OrgId = claims.orgId,
//                                };
//                                _db.CompanyInformations.Add(obj);
//                                await _db.SaveChangesAsync();

//                                res.Message = "Company Info Added";
//                                res.Status = true;
//                                res.Data = obj;
//                            }
//                            else
//                            {
//                                companyInfo.PayGroupId = model.PayGroupId;
//                                companyInfo.SignatoryUrl = model.SignatoryUrl;
//                                companyInfo.LegalNameOfCompany = model.LegalNameOfCompany;
//                                companyInfo.CompanyIdentifyNumber = model.CompanyIdentifyNumber;
//                                companyInfo.DateOfIncorporation = model.DateOfIncorporation;
//                                companyInfo.TypeOfBusinessId = model.TypeOfBusinessId;
//                                companyInfo.SectorId = model.SectorId;
//                                companyInfo.NatureOfBusinessId = model.NatureOfBusinessId;
//                                companyInfo.AddressLine1 = model.AddressLine1;
//                                companyInfo.AddressLine2 = model.AddressLine2;
//                                companyInfo.CityId = model.CityId;
//                                companyInfo.StateId = model.StateId;
//                                companyInfo.ZipCode = model.ZipCode;
//                                companyInfo.CountryId = model.CountryId;
//                                companyInfo.IsActive = true;
//                                companyInfo.IsDeleted = false;
//                                companyInfo.CreatedBy = claims.userId;
//                                companyInfo.CreatedOn = DateTime.Now;
//                                companyInfo.CompanyId = claims.companyId;
//                                companyInfo.OrgId = claims.orgId;

//                                _db.Entry(companyInfo).State = System.Data.Entity.EntityState.Modified;
//                                await _db.SaveChangesAsync();

//                                res.Message = "Company Info Updated";
//                                res.Status = true;
//                                res.Data = companyInfo;
//                            }
//                        }
//                        else
//                        {
//                            res.Message = "Pay Roll Setup not Created";
//                            res.Status = false;
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Group Not Found";
//                        res.Status = false;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Add and Update Company Info

//        #region Api To Get Company Info

//        /// <summary>
//        /// Created By Harshit Mitra on 25-03-2022
//        /// API >> Get >> api/payroll/getcompanyinfo
//        /// </summary>
//        /// <param name="payGroupId"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getcompanyinfo")]
//        public async Task<ResponseBodyModel> GetCompanyInfo(int payGroupId)
//        {
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            ResponseBodyModel res = new ResponseBodyModel();
//            try
//            {
//                var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.PayGroupId == payGroupId &&
//                            x.CompanyId == claims.companyId && x.OrgId == claims.orgId);
//                if (payGroup != null)
//                {
//                    var payRollSetup_CompanyInfo = await _db.PayRollSetups.Where(x => x.IsActive == true && x.IsDeleted == false &&
//                                    x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.CompanyInfo).FirstOrDefaultAsync();
//                    if (payRollSetup_CompanyInfo != null)
//                    {
//                        var companyInfo = await _db.CompanyInformations.
//                                    FirstOrDefaultAsync(x => x.PayGroupId == payGroup.PayGroupId);
//                        if (companyInfo != null)
//                        {
//                            res.Message = "Company Info Found";
//                            res.Status = true;
//                            res.Data = companyInfo;
//                        }
//                        else
//                        {
//                            res.Message = "Company Info Not Found";
//                            res.Status = false;
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Roll Setup not Created";
//                        res.Status = false;
//                    }
//                }
//                else
//                {
//                    res.Message = "Pay Group Not Found";
//                    res.Status = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Company Info

//        #region Api To Add Update Bank In Company Info

//        /// <summary>
//        /// Created By Harshit Mitra on 03-04-2022
//        /// API >> Post >> api/payroll/addupdatecompanyinfobank
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("addupdatecompanyinfobank")]
//        public async Task<ResponseBodyModel> AddCompanyInfoBanks(AddUpdateCompanyInfoBanks model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            List<CompanyInfoBank> list = new List<CompanyInfoBank>();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                if (model == null)
//                {
//                    res.Message = "Invalid Model";
//                    res.Status = false;
//                }
//                else
//                {
//                    var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId &&
//                            x.CompanyId == claims.companyId && x.OrgId == claims.orgId);
//                    if (payGroup != null)
//                    {
//                        var payRollSetup_CompanyInfoBanks = await _db.PayRollSetups.Where(x => x.IsActive == true && x.IsDeleted == false &&
//                                    x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.CompanyInfo).FirstOrDefaultAsync();
//                        if (payRollSetup_CompanyInfoBanks != null)
//                        {
//                            var companyInfo = await _db.CompanyInformations.FirstOrDefaultAsync(x => x.PayGroupId == payGroup.PayGroupId);
//                            if (companyInfo != null)
//                            {
//                                var companyInfoBank = await _db.CompanyInfoBanks.Where(x =>
//                                            x.CompanyInfoId == companyInfo.CompanyInfoId &&
//                                            x.PayGroupId == companyInfo.PayGroupId).ToListAsync();
//                                if (companyInfoBank.Count > 0)
//                                {
//                                    if (model.BankIds.Count > 0)
//                                    {
//                                        foreach (var banks in companyInfoBank)
//                                        {
//                                            _db.Entry(banks).State = System.Data.Entity.EntityState.Deleted;
//                                            _db.SaveChanges();
//                                        }
//                                        foreach (var item in model.BankIds)
//                                        {
//                                            var bank = _db.BankMaster.FirstOrDefault(x => x.BankId == item);
//                                            CompanyInfoBank obj = new CompanyInfoBank
//                                            {
//                                                PayGroupId = payGroup.PayGroupId,
//                                                CompanyInfoId = companyInfo.CompanyInfoId,
//                                                BankName = bank.BankName,
//                                                BankId = bank.BankId,
//                                                Location = bank.Address,
//                                                IsActive = true,
//                                                IsDeleted = false,
//                                                CreatedBy = claims.userId,
//                                                CreatedOn = DateTime.Now,
//                                            };
//                                            _db.CompanyInfoBanks.Add(obj);
//                                            await _db.SaveChangesAsync();
//                                            list.Add(obj);
//                                        }
//                                        res.Message = "Company Bank Updated";
//                                        res.Status = true;
//                                        res.Data = list;
//                                    }
//                                    else
//                                    {
//                                        res.Message = "No Bank Selected";
//                                        res.Status = false;
//                                    }
//                                }
//                                else
//                                {
//                                    if (model.BankIds.Count > 0)
//                                    {
//                                        foreach (var item in model.BankIds)
//                                        {
//                                            var bank = _db.BankMaster.FirstOrDefault(x => x.BankId == item);
//                                            CompanyInfoBank obj = new CompanyInfoBank
//                                            {
//                                                PayGroupId = payGroup.PayGroupId,
//                                                CompanyInfoId = companyInfo.CompanyInfoId,
//                                                BankName = bank.BankName,
//                                                BankId = bank.BankId,
//                                                Location = bank.Address,
//                                                IsActive = true,
//                                                IsDeleted = false,
//                                                CreatedBy = claims.userId,
//                                                CreatedOn = DateTime.Now,
//                                            };
//                                            _db.CompanyInfoBanks.Add(obj);
//                                            await _db.SaveChangesAsync();
//                                            list.Add(obj);
//                                        }
//                                        res.Message = "Company Bank Added";
//                                        res.Status = true;
//                                        res.Data = list;
//                                    }
//                                    else
//                                    {
//                                        res.Message = "No Bank Selected";
//                                        res.Status = false;
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                res.Message = "You Have To Add Company Info First";
//                                res.Status = false;
//                            }
//                        }
//                        else
//                        {
//                            res.Message = "Pay Roll Setup not Created";
//                            res.Status = false;
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Group Not Found";
//                        res.Status = false;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Add Update Bank In Company Info

//        #region Api To Get Company Info Bank

//        /// <summary>
//        /// Created By Harshit Mitra on 14-04-2022
//        /// API >> Get >> api/payroll/getcomponyinfobank
//        /// </summary>
//        /// <param name="payGroupId"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getcomponyinfobank")]
//        public async Task<ResponseBodyModel> GetComponyInfoBank(int payGroupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            List<CompanyInfoBank> list = new List<CompanyInfoBank>();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.PayGroupId == payGroupId &&
//                           x.CompanyId == claims.companyId && x.OrgId == claims.orgId);
//                if (payGroup != null)
//                {
//                    var payRollSetup_CompanyInfoBanks = await _db.PayRollSetups.Where(x => x.IsActive == true && x.IsDeleted == false &&
//                                x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.CompanyInfo).FirstOrDefaultAsync();
//                    if (payRollSetup_CompanyInfoBanks != null)
//                    {
//                        var companyInfo = await _db.CompanyInformations.FirstOrDefaultAsync(x => x.PayGroupId == payGroup.PayGroupId);
//                        if (companyInfo != null)
//                        {
//                            var companyInfoBank = await _db.CompanyInfoBanks.Where(x =>
//                                        x.CompanyInfoId == companyInfo.CompanyInfoId &&
//                                        x.PayGroupId == companyInfo.PayGroupId).ToListAsync();
//                            if (companyInfoBank.Count > 0)
//                            {
//                                list = companyInfoBank;
//                                res.Message = "Company Info Bank";
//                                res.Status = true;
//                                res.Data = list;
//                            }
//                            else
//                            {
//                                res.Message = "You Didnt Added Bank Account";
//                                res.Status = false;
//                                res.Data = list;
//                            }
//                        }
//                        else
//                        {
//                            res.Message = "Company Info Not Added";
//                            res.Status = false;
//                            res.Data = list;
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Roll Setup not Created";
//                        res.Status = false;
//                        res.Data = list;
//                    }
//                }
//                else
//                {
//                    res.Message = "Pay Group Not Found";
//                    res.Status = false;
//                    res.Data = list;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Company Info Bank

//        #region Api To Add Update Locations In Company Info

//        /// <summary>
//        /// Created By Harshit Mitra on 03-04-2022
//        /// API >> Post >> api/payroll/addupdatecompanyinfolocations
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("addupdatecompanyinfolocations")]
//        public async Task<ResponseBodyModel> AddCompanyInfoLocation(AddUpdateCompanyInfoLocations model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            List<CompanyInfoLocation> list = new List<CompanyInfoLocation>();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                if (model == null)
//                {
//                    res.Message = "Invalid Model";
//                    res.Status = false;
//                }
//                else
//                {
//                    var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId &&
//                            x.CompanyId == claims.companyId && x.OrgId == claims.orgId);
//                    if (payGroup != null)
//                    {
//                        var payRollSetup_CompanyInfoLocations = await _db.PayRollSetups.Where(x => x.IsActive == true && x.IsDeleted == false &&
//                                    x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.CompanyInfo).FirstOrDefaultAsync();
//                        if (payRollSetup_CompanyInfoLocations != null)
//                        {
//                            var companyInfo = await _db.CompanyInformations.FirstOrDefaultAsync(x => x.PayGroupId == payGroup.PayGroupId);
//                            if (companyInfo != null)
//                            {
//                                var companyInfoLocations = await _db.CompanyInfoLocations.Where(x =>
//                                            x.CompanyInfoId == companyInfo.CompanyInfoId &&
//                                            x.PayGroupId == companyInfo.PayGroupId).ToListAsync();
//                                if (companyInfoLocations.Count > 0)
//                                {
//                                    if (model.LocationsIds.Count > 0)
//                                    {
//                                        foreach (var locations in companyInfoLocations)
//                                        {
//                                            _db.Entry(locations).State = System.Data.Entity.EntityState.Deleted;
//                                            _db.SaveChanges();
//                                        }
//                                        foreach (var item in model.LocationsIds)
//                                        {
//                                            var location = _db.Locations.FirstOrDefault(x => x.LocationId == item);
//                                            CompanyInfoLocation obj = new CompanyInfoLocation
//                                            {
//                                                PayGroupId = payGroup.PayGroupId,
//                                                CompanyInfoId = companyInfo.CompanyInfoId,
//                                                LocationId = location.LocationId,
//                                                LocationName = location.LocationName,
//                                                IsActive = true,
//                                                IsDeleted = false,
//                                                CreatedBy = claims.userId,
//                                                CreatedOn = DateTime.Now,
//                                            };
//                                            _db.CompanyInfoLocations.Add(obj);
//                                            await _db.SaveChangesAsync();
//                                            list.Add(obj);
//                                        }
//                                        res.Message = "Locations Updated";
//                                        res.Status = true;
//                                        res.Data = list;
//                                    }
//                                    else
//                                    {
//                                        res.Message = "No Locations Selected";
//                                        res.Status = false;
//                                    }
//                                }
//                                else
//                                {
//                                    if (model.LocationsIds.Count > 0)
//                                    {
//                                        foreach (var item in model.LocationsIds)
//                                        {
//                                            var location = _db.Locations.FirstOrDefault(x => x.LocationId == item);
//                                            CompanyInfoLocation obj = new CompanyInfoLocation
//                                            {
//                                                PayGroupId = payGroup.PayGroupId,
//                                                CompanyInfoId = companyInfo.CompanyInfoId,
//                                                LocationId = location.LocationId,
//                                                LocationName = location.LocationName,
//                                                IsActive = true,
//                                                IsDeleted = false,
//                                                CreatedBy = claims.userId,
//                                                CreatedOn = DateTime.Now,
//                                            };
//                                            _db.CompanyInfoLocations.Add(obj);
//                                            await _db.SaveChangesAsync();
//                                            list.Add(obj);
//                                        }
//                                        payRollSetup_CompanyInfoLocations.Status = Enum.GetName(typeof(PayrollSetupStatus), PayrollSetupStatus.COMPLETED);
//                                        payRollSetup_CompanyInfoLocations.UpdatedBy = claims.userId;
//                                        payRollSetup_CompanyInfoLocations.UpdatedOn = DateTime.Now;
//                                        _db.Entry(payRollSetup_CompanyInfoLocations).State = System.Data.Entity.EntityState.Modified;
//                                        await _db.SaveChangesAsync();

//                                        res.Message = "Locations Added";
//                                        res.Status = true;
//                                        res.Data = list;
//                                    }
//                                    else
//                                    {
//                                        res.Message = "No Locations Selected";
//                                        res.Status = false;
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                res.Message = "You Have To Add Company Info First";
//                                res.Status = false;
//                            }
//                        }
//                        else
//                        {
//                            res.Message = "Pay Roll Setup not Created";
//                            res.Status = false;
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Group Not Found";
//                        res.Status = false;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Add Update Locations In Company Info

//        #region Api To Get Company Info Locations

//        /// <summary>
//        /// Created By Harshit Mitra on 14-04-2022
//        /// API >> Get >> api/payroll/getcompanylocations
//        /// </summary>
//        /// <param name="payGroupId"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getcompanylocations")]
//        public async Task<ResponseBodyModel> GetComponyInfoLocations(int payGroupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            List<CompanyInfoLocation> list = new List<CompanyInfoLocation>();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.PayGroupId == payGroupId &&
//                           x.CompanyId == claims.companyId && x.OrgId == claims.orgId);
//                if (payGroup != null)
//                {
//                    var payRollSetup_CompanyInfoBanks = await _db.PayRollSetups.Where(x => x.IsActive == true && x.IsDeleted == false &&
//                                x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.CompanyInfo).FirstOrDefaultAsync();
//                    if (payRollSetup_CompanyInfoBanks != null)
//                    {
//                        var companyInfo = await _db.CompanyInformations.FirstOrDefaultAsync(x => x.PayGroupId == payGroup.PayGroupId);
//                        if (companyInfo != null)
//                        {
//                            var componyInfoLocations = await _db.CompanyInfoLocations.Where(x =>
//                                        x.CompanyInfoId == companyInfo.CompanyInfoId &&
//                                        x.PayGroupId == companyInfo.PayGroupId).ToListAsync();
//                            if (componyInfoLocations.Count > 0)
//                            {
//                                list = componyInfoLocations;
//                                res.Message = "Company Locations";
//                                res.Status = true;
//                                res.Data = list;
//                            }
//                            else
//                            {
//                                res.Message = "You Didnt Any Locations";
//                                res.Status = false;
//                                res.Data = list;
//                            }
//                        }
//                        else
//                        {
//                            res.Message = "Company Info Not Added";
//                            res.Status = false;
//                            res.Data = list;
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Roll Setup not Created";
//                        res.Status = false;
//                        res.Data = list;
//                    }
//                }
//                else
//                {
//                    res.Message = "Pay Group Not Found";
//                    res.Status = false;
//                    res.Data = list;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Company Info Locations

//        ///----------- Pay Roll Setting Gernal Pay Roll Setting --------------///

//        #region Api To Add General PayRoll Setting

//        /// <summary>
//        /// API >> Get >> api/payroll/addgeneralpayrollsetting
//        /// Created By Harshit Mitra on 24-02-2022
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("addgeneralpayrollsetting")]
//        public async Task<ResponseBodyModel> GeneralPayRollSetting(GernalPayrollSetting model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                if (model == null)
//                {
//                    res.Message = "Invalid Model";
//                    res.Status = false;
//                }
//                else
//                {
//                    var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId);
//                    if (payGroup != null)
//                    {
//                        var payRollSetupMaster_GernalPayrollSetting = await _db.PayRollSetups.Where(x => x.IsActive == true &&
//                                x.IsDeleted == false && x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.GernallSetting).ToListAsync();
//                        if (payRollSetupMaster_GernalPayrollSetting != null)
//                        {
//                            var gernalPayrollSetting = await _db.GernalPayrollSettings.FirstOrDefaultAsync(x =>
//                                    x.IsActive == true && x.IsDeleted == false && x.PayGroupId == payGroup.PayGroupId);
//                            if (gernalPayrollSetting != null)
//                            {
//                                gernalPayrollSetting.PayGroupId = model.PayGroupId;
//                                gernalPayrollSetting.PayFrequency = model.PayFrequency;
//                                gernalPayrollSetting.PayCycleForHRMS = model.PayCycleForHRMS;
//                                gernalPayrollSetting.StartPeriod = model.StartPeriod;
//                                gernalPayrollSetting.EndPeriod = model.EndPeriod;
//                                gernalPayrollSetting.TotalPayDays = model.TotalPayDays;
//                                gernalPayrollSetting.ExcludeWeeklyOffs = model.ExcludeWeeklyOffs;
//                                gernalPayrollSetting.ExcludeHolidays = model.ExcludeHolidays;
//                                gernalPayrollSetting.CurrencyId = model.CurrencyId;
//                                gernalPayrollSetting.CurrencyName = model.CurrencyName;
//                                gernalPayrollSetting.RemunerationMonthly = model.RemunerationMonthly;
//                                gernalPayrollSetting.RemunerationDaily = model.RemunerationDaily;
//                                gernalPayrollSetting.UpdatedBy = claims.userId;
//                                gernalPayrollSetting.CompanyId = claims.companyId;
//                                gernalPayrollSetting.OrgId = claims.orgId;
//                                gernalPayrollSetting.UpdatedOn = DateTime.Now;

//                                _db.Entry(gernalPayrollSetting).State = System.Data.Entity.EntityState.Modified;
//                                await _db.SaveChangesAsync();

//                                res.Message = "Gernal Payroll Setting Updated";
//                                res.Status = true;
//                                res.Data = gernalPayrollSetting;
//                            }
//                            else
//                            {
//                                GernalPayrollSetting obj = new GernalPayrollSetting()
//                                {
//                                    PayGroupId = model.PayGroupId,
//                                    PayFrequency = model.PayFrequency,
//                                    PayCycleForHRMS = model.PayCycleForHRMS,
//                                    StartPeriod = model.StartPeriod,
//                                    EndPeriod = model.EndPeriod,
//                                    TotalPayDays = model.TotalPayDays,
//                                    ExcludeWeeklyOffs = model.ExcludeWeeklyOffs,
//                                    ExcludeHolidays = model.ExcludeHolidays,
//                                    CurrencyId = model.CurrencyId,
//                                    CurrencyName = model.CurrencyName,
//                                    RemunerationMonthly = model.RemunerationMonthly,
//                                    RemunerationDaily = model.RemunerationDaily,
//                                    CreatedBy = claims.userId,
//                                    CreatedOn = DateTime.Now,
//                                    CompanyId = claims.companyId,
//                                    OrgId = claims.orgId,
//                                    IsActive = true,
//                                    IsDeleted = false,
//                                };
//                                _db.GernalPayrollSettings.Add(obj);
//                                await _db.SaveChangesAsync();

//                                var payrollSetup = payRollSetupMaster_GernalPayrollSetting.Where(x => x.Step == 2).FirstOrDefault();
//                                if (payrollSetup != null)
//                                {
//                                    payrollSetup.UpdatedOn = DateTime.Now;
//                                    payrollSetup.UpdatedBy = claims.userId;
//                                    payrollSetup.Status = Enum.GetName(typeof(PayrollSetupStatus), (int)PayrollSetupStatus.COMPLETED);

//                                    _db.Entry(payrollSetup).State = System.Data.Entity.EntityState.Modified;
//                                    await _db.SaveChangesAsync();
//                                }

//                                res.Message = "Gernal Payroll Setting Saved";
//                                res.Status = true;
//                                res.Data = obj;
//                            }
//                        }
//                        else
//                        {
//                            res.Message = "Pay Roll Setup not Created";
//                            res.Status = false;
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Group Not Found";
//                        res.Status = false;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Add General PayRoll Setting

//        #region Api To Get Gernall PayRoll Setting

//        /// <summary>
//        /// Created By Harshit Mitra on 15-04-2022
//        /// API >> Get >> api/payroll/getgernalpayrollsetting
//        /// </summary>
//        /// <param name="payGroupId"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getgernalpayrollsetting")]
//        public async Task<ResponseBodyModel> GetGernalPayRollSettings(int payGroupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.PayGroupId == payGroupId);
//                if (payGroup != null)
//                {
//                    var payRollSetupMaster_GernalPayrollSetting = await _db.PayRollSetups.Where(x => x.IsActive == true &&
//                                x.IsDeleted == false && x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.GernallSetting).ToListAsync();
//                    if (payRollSetupMaster_GernalPayrollSetting != null)
//                    {
//                        var gernalPayrollSetting = await _db.GernalPayrollSettings.FirstOrDefaultAsync(x =>
//                                    x.IsActive == true && x.IsDeleted == false && x.PayGroupId == payGroup.PayGroupId);
//                        if (gernalPayrollSetting != null)
//                        {
//                            res.Message = "Gernal Pay Roll Setting";
//                            res.Status = true;
//                            res.Data = gernalPayrollSetting;
//                        }
//                        else
//                        {
//                            res.Message = "Gernal Pay Roll Setting";
//                            res.Status = true;
//                            res.Data = gernalPayrollSetting;
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Roll Setup not Created";
//                        res.Status = false;
//                    }
//                }
//                else
//                {
//                    res.Message = "Pay Group Not Found";
//                    res.Status = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Gernall PayRoll Setting

//        ///----------- Pay Roll Pf And ESI Settings -------------------///

//        #region Api To Add And Update Pf Settings

//        /// <summary>
//        /// Created By Harshit Mitra on 16-04-2022
//        /// API >> Get >> api/payroll/addupdatepf
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("addupdatepf")]
//        public async Task<ResponseBodyModel> AddUpdatePFSettings(PFSettings model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId);
//                if (payGroup != null)
//                {
//                    var payRollSetupMaster_PfSetting = await _db.PayRollSetups.FirstOrDefaultAsync(x => x.IsActive == true &&
//                                x.IsDeleted == false && x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.PF_ESI);
//                    if (payRollSetupMaster_PfSetting != null)
//                    {
//                        var pfSetting = await _db.PFSettings.FirstOrDefaultAsync(x => x.PayGroupId == payGroup.PayGroupId);
//                        if (pfSetting == null)
//                        {
//                            PFSettings obj = new PFSettings();
//                            obj.PayGroupId = model.PayGroupId;
//                            obj.IsPFRequired = model.IsPFRequired;
//                            obj.CompanyId = claims.companyId;
//                            obj.OrgId = claims.orgId;
//                            obj.CreatedBy = claims.employeeId;
//                            obj.CreatedOn = DateTime.Now;
//                            if (model.IsPFRequired)
//                            {
//                                obj.MinimumPFAmount = model.MinimumPFAmount;
//                                obj.PfCalculationType = model.PfCalculationType;
//                                obj.IsAllowOverridingOfPf = obj.PfCalculationType == (int)PfCalculationType.Allow_PF_calculated_as_percentage_of_basic_salary_beyond_statutory_minimum
//                                                    ? model.IsAllowOverridingOfPf : false;
//                                obj.IsPayEmployeePFOutsideGross = model.IsPayEmployeePFOutsideGross;
//                                obj.IsLimitEmpPFMaxAmount = obj.IsPayEmployeePFOutsideGross ? model.IsPayEmployeePFOutsideGross : false;
//                                obj.LimitEmpPFMaxAmountMonthly = obj.IsLimitEmpPFMaxAmount ? model.LimitEmpPFMaxAmountMonthly : 0;
//                                obj.IsHideOtherChargesPFPaySlip = model.IsHideOtherChargesPFPaySlip;
//                                obj.IsPayOtherChargesOutsideGross = model.IsPayOtherChargesOutsideGross;
//                                obj.IsHidePFEmpInPaySlip = model.IsHidePFEmpInPaySlip;
//                                obj.IsEmpContributeVPF = model.IsEmpContributeVPF;
//                                obj.Is1poin16PerShareOfPension = model.Is1poin16PerShareOfPension;
//                                obj.IsAdminToOveridePF = model.IsAdminToOveridePF;
//                            }
//                            _db.PFSettings.Add(obj);
//                            await _db.SaveChangesAsync();

//                            res.Message = "PF Payroll Setting Saved";
//                            res.Status = true;
//                            res.Data = obj;
//                        }
//                        else
//                        {
//                            pfSetting.PayGroupId = model.PayGroupId;
//                            pfSetting.IsPFRequired = model.IsPFRequired;
//                            pfSetting.CompanyId = claims.companyId;
//                            pfSetting.OrgId = claims.orgId;
//                            pfSetting.CreatedBy = claims.employeeId;
//                            pfSetting.CreatedOn = DateTime.Now;
//                            if (model.IsPFRequired)
//                            {
//                                pfSetting.MinimumPFAmount = model.MinimumPFAmount;
//                                pfSetting.PfCalculationType = model.PfCalculationType;
//                                pfSetting.IsAllowOverridingOfPf = pfSetting.PfCalculationType == (int)PfCalculationType.Allow_PF_calculated_as_percentage_of_basic_salary_beyond_statutory_minimum
//                                                    ? model.IsAllowOverridingOfPf : false;
//                                pfSetting.IsPayEmployeePFOutsideGross = model.IsPayEmployeePFOutsideGross;
//                                pfSetting.IsLimitEmpPFMaxAmount = pfSetting.IsPayEmployeePFOutsideGross ? model.IsPayEmployeePFOutsideGross : false;
//                                pfSetting.LimitEmpPFMaxAmountMonthly = pfSetting.IsLimitEmpPFMaxAmount ? model.LimitEmpPFMaxAmountMonthly : 0;
//                                pfSetting.IsHideOtherChargesPFPaySlip = model.IsHideOtherChargesPFPaySlip;
//                                pfSetting.IsPayOtherChargesOutsideGross = model.IsPayOtherChargesOutsideGross;
//                                pfSetting.IsHidePFEmpInPaySlip = model.IsHidePFEmpInPaySlip;
//                                pfSetting.IsEmpContributeVPF = model.IsEmpContributeVPF;
//                                pfSetting.Is1poin16PerShareOfPension = model.Is1poin16PerShareOfPension;
//                                pfSetting.IsAdminToOveridePF = model.IsAdminToOveridePF;
//                            }
//                            _db.Entry(pfSetting).State = System.Data.Entity.EntityState.Modified;
//                            await _db.SaveChangesAsync();

//                            res.Message = "PF Payroll Setting Updated";
//                            res.Status = true;
//                            res.Data = pfSetting;
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Roll Setup not Created";
//                        res.Status = false;
//                    }
//                }
//                else
//                {
//                    res.Message = "Pay Group Not Found";
//                    res.Status = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Add And Update Pf Settings

//        #region Api To Get Pf Setting

//        /// <summary>
//        /// Created By Harshit Mitra on 18-04-2022
//        /// API >> Get >> api/payroll/getpfsettings
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getpfsettings")]
//        public async Task<ResponseBodyModel> GetPFSettings(int payGroupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.PayGroupId == payGroupId);
//                if (payGroup != null)
//                {
//                    var payRollSetupMaster_PfSetting = await _db.PayRollSetups.FirstOrDefaultAsync(x => x.IsActive == true &&
//                                x.IsDeleted == false && x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.PF_ESI);
//                    if (payRollSetupMaster_PfSetting != null)
//                    {
//                        var pfSetting = await _db.PFSettings.FirstOrDefaultAsync(x => x.PayGroupId == payGroup.PayGroupId);
//                        if (pfSetting != null)
//                        {
//                            res.Message = "Pf Setting";
//                            res.Status = true;
//                            res.Data = pfSetting;
//                        }
//                        else
//                        {
//                            res.Message = "Pf Setting Not Add";
//                            res.Status = false;
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Roll Setup not Created";
//                        res.Status = false;
//                    }
//                }
//                else
//                {
//                    res.Message = "Pay Group Not Found";
//                    res.Status = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Pf Setting

//        #region Api To Add Update Esi Setting

//        /// <summary>
//        /// Created By Harshit Mitra on 18-04-2022
//        /// API >> Post >> api/payroll/addupdateesi
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("addupdateesi")]
//        public async Task<ResponseBodyModel> AddUpdateEsiSetting(ESISetting model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId);
//                if (payGroup != null)
//                {
//                    var payRollSetupMaster_esiSetting = await _db.PayRollSetups.FirstOrDefaultAsync(x => x.IsActive == true &&
//                                x.IsDeleted == false && x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.PF_ESI);
//                    if (payRollSetupMaster_esiSetting != null)
//                    {
//                        var esiSetting = await _db.ESISettings.FirstOrDefaultAsync(x => x.PayGroupId == payGroup.PayGroupId);
//                        if (esiSetting == null)
//                        {
//                            ESISetting obj = new ESISetting();
//                            obj.PayGroupId = payGroup.PayGroupId;
//                            obj.IsESIStatus = model.IsESIStatus;
//                            obj.CreatedBy = claims.employeeId;
//                            obj.CreatedOn = DateTime.Now;
//                            if (model.IsESIStatus)
//                            {
//                                obj.EligibleAmountForESI = model.EligibleAmountForESI;
//                                obj.MinESIEmpContributionOfGross = model.MinESIEmpContributionOfGross;
//                                obj.MaxESIEmpContributionOfGross = model.MaxESIEmpContributionOfGross;
//                                obj.IsAllowESIatSalary = model.IsAllowESIatSalary;
//                                obj.IsPayESIEmpOutsideGross = model.IsPayESIEmpOutsideGross;
//                                obj.IsHideESIEmpPaySlip = model.IsHideESIEmpPaySlip;
//                                obj.IsExcludeEmpShareFromGrossESI = model.IsExcludeEmpShareFromGrossESI;
//                                obj.IsExcludeEmpGratutyFromGrossESI = model.IsExcludeEmpGratutyFromGrossESI;
//                                obj.IsRestrictESIGrossDuringContribution = model.IsRestrictESIGrossDuringContribution;
//                                obj.IsIncludeBonusandOneTimePaymentForESIEligibility = model.IsIncludeBonusandOneTimePaymentForESIEligibility;
//                                obj.IsIncludeBonusandOneTimePaymentForESIContribution = model.IsIncludeBonusandOneTimePaymentForESIContribution;
//                            }
//                            _db.ESISettings.Add(obj);
//                            await _db.SaveChangesAsync();

//                            payRollSetupMaster_esiSetting.UpdatedOn = DateTime.Now;
//                            payRollSetupMaster_esiSetting.UpdatedBy = claims.userId;
//                            payRollSetupMaster_esiSetting.Status = Enum.GetName(typeof(PayrollSetupStatus), (int)PayrollSetupStatus.COMPLETED);

//                            _db.Entry(payRollSetupMaster_esiSetting).State = System.Data.Entity.EntityState.Modified;
//                            await _db.SaveChangesAsync();

//                            res.Message = "Esi Setting Saved";
//                            res.Status = true;
//                            res.Data = obj;
//                        }
//                        else
//                        {
//                            esiSetting.PayGroupId = payGroup.PayGroupId;
//                            esiSetting.IsESIStatus = model.IsESIStatus;
//                            esiSetting.UpdatedBy = claims.employeeId;
//                            esiSetting.UpdatedOn = DateTime.Now;
//                            if (model.IsESIStatus)
//                            {
//                                esiSetting.EligibleAmountForESI = model.EligibleAmountForESI;
//                                esiSetting.MinESIEmpContributionOfGross = model.MinESIEmpContributionOfGross;
//                                esiSetting.MaxESIEmpContributionOfGross = model.MaxESIEmpContributionOfGross;
//                                esiSetting.IsAllowESIatSalary = model.IsAllowESIatSalary;
//                                esiSetting.IsPayESIEmpOutsideGross = model.IsPayESIEmpOutsideGross;
//                                esiSetting.IsHideESIEmpPaySlip = model.IsHideESIEmpPaySlip;
//                                esiSetting.IsExcludeEmpShareFromGrossESI = model.IsExcludeEmpShareFromGrossESI;
//                                esiSetting.IsExcludeEmpGratutyFromGrossESI = model.IsExcludeEmpGratutyFromGrossESI;
//                                esiSetting.IsRestrictESIGrossDuringContribution = model.IsRestrictESIGrossDuringContribution;
//                                esiSetting.IsIncludeBonusandOneTimePaymentForESIEligibility = model.IsIncludeBonusandOneTimePaymentForESIEligibility;
//                                esiSetting.IsIncludeBonusandOneTimePaymentForESIContribution = model.IsIncludeBonusandOneTimePaymentForESIContribution;
//                            }
//                            _db.Entry(esiSetting).State = System.Data.Entity.EntityState.Modified;
//                            await _db.SaveChangesAsync();

//                            res.Message = "Esi Setting Saved";
//                            res.Status = true;
//                            res.Data = esiSetting;
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Roll Setup not Created";
//                        res.Status = false;
//                    }
//                }
//                else
//                {
//                    res.Message = "Pay Group Not Found";
//                    res.Status = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Add Update Esi Setting

//        #region Api To Get ESI Setting

//        /// <summary>
//        /// Created By Harshit Mitra on 18-04-2022
//        /// API >> Get >> api/payroll/getesisettings
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getesisettings")]
//        public async Task<ResponseBodyModel> GetESISettings(int payGroupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.PayGroupId == payGroupId);
//                if (payGroup != null)
//                {
//                    var payRollSetupMaster_PfSetting = await _db.PayRollSetups.FirstOrDefaultAsync(x => x.IsActive == true &&
//                                x.IsDeleted == false && x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.PF_ESI);
//                    if (payRollSetupMaster_PfSetting != null)
//                    {
//                        var esiSetting = await _db.ESISettings.FirstOrDefaultAsync(x => x.PayGroupId == payGroup.PayGroupId);
//                        if (esiSetting != null)
//                        {
//                            res.Message = "ESI Setting";
//                            res.Status = true;
//                            res.Data = esiSetting;
//                        }
//                        else
//                        {
//                            res.Message = "ESI Setting Not Add";
//                            res.Status = false;
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Roll Setup not Created";
//                        res.Status = false;
//                    }
//                }
//                else
//                {
//                    res.Message = "Pay Group Not Found";
//                    res.Status = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get ESI Setting

//        ///----------------- Pay Roll Salery Components -------------------///

//        #region Api To Add Update Pay Roll Salery Components

//        /// <summary>
//        /// Created By Harshit Mitra on 02-06-2022
//        /// API >> Post >> api/payroll/addupdatesalerycomonent
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("addupdatesalerycomonent")]
//        public async Task<ResponseBodyModel> AddUpdateSaleryCompanents(AddSaleryComponentHelperModel model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            List<PayGroupComponent> resList = new List<PayGroupComponent>();
//            try
//            {
//                if (model == null)
//                {
//                    res.Message = "Model Is Invalid";
//                    res.Status = false;
//                }
//                else
//                {
//                    var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.CompanyId == claims.companyId &&
//                            x.OrgId == claims.orgId && x.IsActive == true && x.IsDeleted == false && x.PayGroupId == model.PayGroupId);
//                    if (payGroup != null)
//                    {
//                        var payRollComponentSetup = await _db.PayRollSetups.FirstOrDefaultAsync(x => x.IsActive == true &&
//                                    x.IsDeleted == false && x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.SaleryComponents);
//                        if (payRollComponentSetup != null)
//                        {
//                            var checkComponent = await _db.PayGroupComponents.Where(x => x.PayGroupId == payGroup.PayGroupId &&
//                                    x.ComponentType == PayGroupComponentTypeConstants.RecurringComponent).ToListAsync();
//                            if (checkComponent.Count == 0)
//                            {
//                                if (model.RecurringList.Count > 0)
//                                {
//                                    foreach (var item in model.RecurringList)
//                                    {
//                                        if (item.IsChecked)
//                                        {
//                                            PayGroupComponent obj = new PayGroupComponent
//                                            {
//                                                PayGroupId = payGroup.PayGroupId,
//                                                SelectedComponentId = item.RecuringComponentId,
//                                                ComponentType = PayGroupComponentTypeConstants.RecurringComponent,

//                                                IsActive = true,
//                                                IsDeleted = false,
//                                                CompanyId = claims.companyId,
//                                                OrgId = 0,
//                                                CreatedBy = claims.employeeId,
//                                                CreatedOn = DateTime.Now,
//                                            };
//                                            _db.PayGroupComponents.Add(obj);
//                                            await _db.SaveChangesAsync();
//                                            resList.Add(obj);
//                                        }
//                                    }
//                                    res.Message = "Recuring Component Added";
//                                    res.Status = true;
//                                    res.Data = resList;
//                                }
//                                else
//                                {
//                                    res.Message = "Select At Least One Component";
//                                    res.Status = false;
//                                }
//                            }
//                            else
//                            {
//                                if (model.RecurringList.Count > 0)
//                                {
//                                    foreach (var item in checkComponent)
//                                    {
//                                        _db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
//                                        await _db.SaveChangesAsync();
//                                    }
//                                    foreach (var item in model.RecurringList)
//                                    {
//                                        if (item.IsChecked)
//                                        {
//                                            PayGroupComponent obj = new PayGroupComponent
//                                            {
//                                                PayGroupId = payGroup.PayGroupId,
//                                                SelectedComponentId = item.RecuringComponentId,
//                                                ComponentType = PayGroupComponentTypeConstants.RecurringComponent,

//                                                IsActive = true,
//                                                IsDeleted = false,
//                                                CompanyId = claims.companyId,
//                                                OrgId = 0,
//                                                CreatedBy = claims.employeeId,
//                                                CreatedOn = DateTime.Now,
//                                            };
//                                            _db.PayGroupComponents.Add(obj);
//                                            await _db.SaveChangesAsync();
//                                            resList.Add(obj);
//                                        }
//                                    }
//                                    res.Message = "Recuring Component Updated";
//                                    res.Status = true;
//                                    res.Data = resList;
//                                }
//                                else
//                                {
//                                    res.Message = "Select At Least One Component";
//                                    res.Status = false;
//                                }
//                            }
//                        }
//                        else
//                        {
//                            res.Message = "Pay Roll Setup not Created";
//                            res.Status = false;
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Group Not Found";
//                        res.Status = false;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Add Update Pay Roll Salery Components

//        #region Api To Get Recurring Component On Salery Component

//        /// <summary>
//        /// Created By Harshit Mitra on 02-06-2022
//        /// API >> Get >> api/payroll/getrconsalerycomponent
//        /// </summary>
//        /// <param name="payGroupId"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getrconsalerycomponent")]
//        public async Task<ResponseBodyModel> GetRecurringComponentOnSaleryComponent(int payGroupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.CompanyId == claims.companyId &&
//                            x.OrgId == claims.orgId && x.IsActive == true && x.IsDeleted == false && x.PayGroupId == payGroupId);
//                if (payGroup != null)
//                {
//                    var payRollComponentSetup = await _db.PayRollSetups.FirstOrDefaultAsync(x => x.IsActive == true &&
//                                    x.IsDeleted == false && x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.SaleryComponents);
//                    if (payRollComponentSetup != null)
//                    {
//                        var checkComponent = await _db.PayGroupComponents.Where(x => x.PayGroupId == payGroup.PayGroupId &&
//                                    x.ComponentType == PayGroupComponentTypeConstants.RecurringComponent).ToListAsync();
//                        if (checkComponent.Count == 0)
//                        {
//                            var recuringComponent = await _db.RecuringComponents.Where(x => x.IsActive == true &&
//                                x.IsDeleted == false && x.CompanyId == claims.companyId)
//                                .Select(x => new
//                                {
//                                    x.RecuringComponentId,
//                                    x.ComponentName,
//                                    x.ComponentType,
//                                    x.IncomeTaxSection,
//                                    x.IsTaxExempted,
//                                    x.IsAutoCalculated,
//                                    x.MaxiumLimitPerYear,
//                                    x.SectionMaxLimit,
//                                    x.IsDocumentRequired,
//                                    IsChecked = (x.ComponentName == "Basic" || x.ComponentName == "HRA" || x.ComponentName == "Special Allowance"),
//                                    IsAutoChecked = (x.ComponentName == "Basic" || x.ComponentName == "HRA" || x.ComponentName == "Special Allowance"),
//                                }).ToListAsync();
//                            if (recuringComponent.Count > 0)
//                            {
//                                res.Message = "Recuring Component";
//                                res.Status = true;
//                                res.Data = recuringComponent;
//                            }
//                            else
//                            {
//                                res.Message = "Recuring Component Not Found";
//                                res.Status = false;
//                                res.Data = recuringComponent;
//                            }
//                        }
//                        else
//                        {
//                            var componentIds = checkComponent.Select(x => x.SelectedComponentId).ToList();
//                            var recuringComponent = await _db.RecuringComponents.Where(x => x.IsActive == true &&
//                                x.IsDeleted == false && x.CompanyId == claims.companyId)
//                                .Select(x => new
//                                {
//                                    x.RecuringComponentId,
//                                    x.ComponentName,
//                                    x.ComponentType,
//                                    x.IncomeTaxSection,
//                                    x.IsTaxExempted,
//                                    x.IsAutoCalculated,
//                                    x.MaxiumLimitPerYear,
//                                    x.SectionMaxLimit,
//                                    x.IsDocumentRequired,
//                                    IsChecked = componentIds.Contains(x.RecuringComponentId),
//                                    IsAutoChecked = (x.ComponentName == "Basic" || x.ComponentName == "HRA" || x.ComponentName == "Special Allowance"),
//                                }).ToListAsync();
//                            if (recuringComponent.Count > 0)
//                            {
//                                res.Message = "Recuring Component";
//                                res.Status = true;
//                                res.Data = recuringComponent;
//                            }
//                            else
//                            {
//                                res.Message = "Recuring Component Not Found";
//                                res.Status = false;
//                                res.Data = recuringComponent;
//                            }
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Roll Setup not Created";
//                        res.Status = false;
//                    }
//                }
//                else
//                {
//                    res.Message = "Pay Group Not Found";
//                    res.Status = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Recurring Component On Salery Component

//        #region Api To Add Update AdHoc Components In Salery Components

//        /// <summary>
//        /// Created By Harshit Mitra on 02-06-2022
//        /// API >> Post >> api/payroll/addupdatextracomponent
//        /// </summary>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("addupdatextracomponent")]
//        public async Task<ResponseBodyModel> AddUpdatExtraComponent(AddUpdateAdHocHelperClass model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            AddGetAdHocComponentsHelperModel respone = new AddGetAdHocComponentsHelperModel();
//            try
//            {
//                if (model == null)
//                {
//                    res.Message = "Model Is Invalid";
//                    res.Status = false;
//                }
//                else
//                {
//                    var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.CompanyId == claims.companyId &&
//                            x.OrgId == claims.orgId && x.IsActive == true && x.IsDeleted == false && x.PayGroupId == model.PayGroupId);
//                    if (payGroup != null)
//                    {
//                        var payRollComponentSetup = await _db.PayRollSetups.FirstOrDefaultAsync(x => x.IsActive == true &&
//                                    x.IsDeleted == false && x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.SaleryComponents);
//                        if (payRollComponentSetup != null)
//                        {
//                            #region Ad Hoc

//                            var checkADComponent = await _db.PayGroupComponents.Where(x => x.PayGroupId == payGroup.PayGroupId &&
//                                        x.ComponentType == PayGroupComponentTypeConstants.AdHocComponent).ToListAsync();
//                            if (checkADComponent.Count == 0)
//                            {
//                                if (model.Components.AddHoc.Count > 0)
//                                {
//                                    foreach (var item in model.Components.AddHoc)
//                                    {
//                                        if (item.IsChecked)
//                                        {
//                                            PayGroupComponent obj = new PayGroupComponent
//                                            {
//                                                PayGroupId = payGroup.PayGroupId,
//                                                SelectedComponentId = item.AdHocId,
//                                                ComponentType = PayGroupComponentTypeConstants.AdHocComponent,

//                                                IsActive = true,
//                                                IsDeleted = false,
//                                                CompanyId = claims.companyId,
//                                                OrgId = 0,
//                                                CreatedBy = claims.employeeId,
//                                                CreatedOn = DateTime.Now,
//                                            };
//                                            _db.PayGroupComponents.Add(obj);
//                                            await _db.SaveChangesAsync();
//                                        }
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                foreach (var item in checkADComponent)
//                                {
//                                    _db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
//                                    await _db.SaveChangesAsync();
//                                }

//                                if (model.Components.AddHoc.Count > 0)
//                                {
//                                    foreach (var item in model.Components.AddHoc)
//                                    {
//                                        if (item.IsChecked)
//                                        {
//                                            PayGroupComponent obj = new PayGroupComponent
//                                            {
//                                                PayGroupId = payGroup.PayGroupId,
//                                                SelectedComponentId = item.AdHocId,
//                                                ComponentType = PayGroupComponentTypeConstants.AdHocComponent,

//                                                IsActive = true,
//                                                IsDeleted = false,
//                                                CompanyId = claims.companyId,
//                                                OrgId = 0,
//                                                CreatedBy = claims.employeeId,
//                                                CreatedOn = DateTime.Now,
//                                            };
//                                            _db.PayGroupComponents.Add(obj);
//                                            await _db.SaveChangesAsync();
//                                        }
//                                    }
//                                }
//                            }

//                            #endregion Ad Hoc

//                            #region Bonous

//                            var checkBNComponent = await _db.PayGroupComponents.Where(x => x.PayGroupId == payGroup.PayGroupId &&
//                                        x.ComponentType == PayGroupComponentTypeConstants.BonusComponent).ToListAsync();
//                            if (checkBNComponent.Count == 0)
//                            {
//                                if (model.Components.Bonus.Count > 0)
//                                {
//                                    foreach (var item in model.Components.Bonus)
//                                    {
//                                        if (item.IsChecked)
//                                        {
//                                            PayGroupComponent obj = new PayGroupComponent
//                                            {
//                                                PayGroupId = payGroup.PayGroupId,
//                                                SelectedComponentId = item.BonusId,
//                                                ComponentType = PayGroupComponentTypeConstants.BonusComponent,

//                                                IsActive = true,
//                                                IsDeleted = false,
//                                                CompanyId = claims.companyId,
//                                                OrgId = 0,
//                                                CreatedBy = claims.employeeId,
//                                                CreatedOn = DateTime.Now,
//                                            };
//                                            _db.PayGroupComponents.Add(obj);
//                                            await _db.SaveChangesAsync();
//                                        }
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                foreach (var item in checkBNComponent)
//                                {
//                                    _db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
//                                    await _db.SaveChangesAsync();
//                                }
//                                if (model.Components.AddHoc.Count > 0)
//                                {
//                                    foreach (var item in model.Components.Bonus)
//                                    {
//                                        if (item.IsChecked)
//                                        {
//                                            PayGroupComponent obj = new PayGroupComponent
//                                            {
//                                                PayGroupId = payGroup.PayGroupId,
//                                                SelectedComponentId = item.BonusId,
//                                                ComponentType = PayGroupComponentTypeConstants.BonusComponent,

//                                                IsActive = true,
//                                                IsDeleted = false,
//                                                CompanyId = claims.companyId,
//                                                OrgId = 0,
//                                                CreatedBy = claims.employeeId,
//                                                CreatedOn = DateTime.Now,
//                                            };
//                                            _db.PayGroupComponents.Add(obj);
//                                            await _db.SaveChangesAsync();
//                                        }
//                                    }
//                                }
//                            }

//                            #endregion Bonous

//                            #region Deduction

//                            var checkDDComponent = await _db.PayGroupComponents.Where(x => x.PayGroupId == payGroup.PayGroupId &&
//                                        x.ComponentType == PayGroupComponentTypeConstants.DeductionComponent).ToListAsync();
//                            if (checkDDComponent.Count == 0)
//                            {
//                                if (model.Components.Bonus.Count > 0)
//                                {
//                                    foreach (var item in model.Components.Deduction)
//                                    {
//                                        if (item.IsChecked)
//                                        {
//                                            PayGroupComponent obj = new PayGroupComponent
//                                            {
//                                                PayGroupId = payGroup.PayGroupId,
//                                                SelectedComponentId = item.DeductionId,
//                                                ComponentType = PayGroupComponentTypeConstants.DeductionComponent,

//                                                IsActive = true,
//                                                IsDeleted = false,
//                                                CompanyId = claims.companyId,
//                                                OrgId = 0,
//                                                CreatedBy = claims.employeeId,
//                                                CreatedOn = DateTime.Now,
//                                            };
//                                            _db.PayGroupComponents.Add(obj);
//                                            await _db.SaveChangesAsync();
//                                        }
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                foreach (var item in checkDDComponent)
//                                {
//                                    _db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
//                                    await _db.SaveChangesAsync();
//                                }

//                                if (model.Components.Deduction.Count > 0)
//                                {
//                                    foreach (var item in model.Components.Deduction)
//                                    {
//                                        if (item.IsChecked)
//                                        {
//                                            PayGroupComponent obj = new PayGroupComponent
//                                            {
//                                                PayGroupId = payGroup.PayGroupId,
//                                                SelectedComponentId = item.DeductionId,
//                                                ComponentType = PayGroupComponentTypeConstants.DeductionComponent,

//                                                IsActive = true,
//                                                IsDeleted = false,
//                                                CompanyId = claims.companyId,
//                                                OrgId = 0,
//                                                CreatedBy = claims.employeeId,
//                                                CreatedOn = DateTime.Now,
//                                            };
//                                            _db.PayGroupComponents.Add(obj);
//                                            await _db.SaveChangesAsync();
//                                        }
//                                    }
//                                }
//                            }

//                            #endregion Deduction

//                            payRollComponentSetup.UpdatedOn = DateTime.Now;
//                            payRollComponentSetup.UpdatedBy = claims.userId;
//                            payRollComponentSetup.Status = Enum.GetName(typeof(PayrollSetupStatus), (int)PayrollSetupStatus.COMPLETED);

//                            _db.Entry(payRollComponentSetup).State = System.Data.Entity.EntityState.Modified;
//                            await _db.SaveChangesAsync();

//                            res.Message = "Saved Succesful";
//                            res.Status = true;
//                        }
//                        else
//                        {
//                            res.Message = "Pay Roll Setup not Created";
//                            res.Status = false;
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Group Not Found";
//                        res.Status = false;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Add Update AdHoc Components In Salery Components

//        #region Api To Get Ad Hoc In Salery Components

//        /// <summary>
//        /// Created By Harshit Mitra on 02-06-2022
//        /// API >> Get >> api/payroll/getadhocosalerycomponent
//        /// </summary>
//        /// <param name="payGroupId"></param>
//        /// <returns></returns>
//        [Route("getadhocosalerycomponent")]
//        public async Task<ResponseBodyModel> GetAdHocOnSaleryComponent(int payGroupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            AddGetAdHocComponentsHelperModel respone = new AddGetAdHocComponentsHelperModel();
//            try
//            {
//                var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.CompanyId == claims.companyId &&
//                            x.OrgId == claims.orgId && x.IsActive == true && x.IsDeleted == false && x.PayGroupId == payGroupId);
//                if (payGroup != null)
//                {
//                    var payRollComponentSetup = await _db.PayRollSetups.FirstOrDefaultAsync(x => x.IsActive == true &&
//                                    x.IsDeleted == false && x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.SaleryComponents);
//                    if (payRollComponentSetup != null)
//                    {
//                        #region Ad Hoc

//                        var checkADComponent = await _db.PayGroupComponents.Where(x => x.PayGroupId == payGroup.PayGroupId &&
//                                    x.ComponentType == PayGroupComponentTypeConstants.AdHocComponent).ToListAsync();
//                        if (checkADComponent.Count == 0)
//                        {
//                            var adHocList = await _db.AdHocComponents.Where(x => x.IsDeleted == false &&
//                                    x.IsActive == true && x.CompanyId == claims.companyId)
//                                    .Select(x => new GetAddAdHocHelperModel
//                                    {
//                                        AdHocId = x.AdHocId,
//                                        Name = x.Name,
//                                        HasTaxBenefits = x.HasTaxBenefits,
//                                        IsChecked = false,
//                                    }).ToListAsync();
//                            respone.AddHoc = adHocList;
//                        }
//                        else
//                        {
//                            var ids = checkADComponent.Select(x => x.SelectedComponentId).ToList();
//                            var adHocList = await _db.AdHocComponents.Where(x => x.IsDeleted == false &&
//                                    x.IsActive == true && x.CompanyId == claims.companyId)
//                                    .Select(x => new GetAddAdHocHelperModel
//                                    {
//                                        AdHocId = x.AdHocId,
//                                        Name = x.Name,
//                                        HasTaxBenefits = x.HasTaxBenefits,
//                                        IsChecked = ids.Contains(x.AdHocId),
//                                    }).ToListAsync();
//                            respone.AddHoc = adHocList;
//                        }

//                        #endregion Ad Hoc

//                        #region Bonous

//                        var checkBNComponent = await _db.PayGroupComponents.Where(x => x.PayGroupId == payGroup.PayGroupId &&
//                                    x.ComponentType == PayGroupComponentTypeConstants.BonusComponent).ToListAsync();
//                        if (checkBNComponent.Count == 0)
//                        {
//                            var bonusList = await _db.BonusComponents.Where(x => x.IsActive == true &&
//                                    x.IsDeleted == false && x.CompanyId == claims.companyId)
//                                    .Select(x => new GetBonusHelperModel
//                                    {
//                                        BonusId = x.BonusId,
//                                        BonusName = x.BonusName,
//                                        IsChecked = false,
//                                    }).ToListAsync();
//                            respone.Bonus = bonusList;
//                        }
//                        else
//                        {
//                            var ids = checkBNComponent.Select(x => x.SelectedComponentId).ToList();
//                            var bonusList = await _db.BonusComponents.Where(x => x.IsActive == true &&
//                                    x.IsDeleted == false && x.CompanyId == claims.companyId)
//                                    .Select(x => new GetBonusHelperModel
//                                    {
//                                        BonusId = x.BonusId,
//                                        BonusName = x.BonusName,
//                                        IsChecked = ids.Contains(x.BonusId),
//                                    }).ToListAsync();
//                            respone.Bonus = bonusList;
//                        }

//                        #endregion Bonous

//                        #region Deduction

//                        var checkDDComponent = await _db.PayGroupComponents.Where(x => x.PayGroupId == payGroup.PayGroupId &&
//                                    x.ComponentType == PayGroupComponentTypeConstants.DeductionComponent).ToListAsync();
//                        if (checkDDComponent.Count == 0)
//                        {
//                            var deductionList = await _db.DeductionComponents.Where(x => x.IsActive == true &&
//                                    x.IsDeleted == false && x.CompanyId == claims.companyId)
//                                    .Select(x => new GetDeductionHelperModel
//                                    {
//                                        DeductionId = x.DeductionId,
//                                        DeductionName = x.DeductionName,
//                                        HasAffectOnGross = x.HasAffectOnGross,
//                                        IsChecked = false,
//                                    }).ToListAsync();
//                            respone.Deduction = deductionList;
//                        }
//                        else
//                        {
//                            var ids = checkDDComponent.Select(x => x.SelectedComponentId).ToList();
//                            var deductionList = await _db.DeductionComponents.Where(x => x.IsActive == true &&
//                                    x.IsDeleted == false && x.CompanyId == claims.companyId)
//                                    .Select(x => new GetDeductionHelperModel
//                                    {
//                                        DeductionId = x.DeductionId,
//                                        DeductionName = x.DeductionName,
//                                        HasAffectOnGross = x.HasAffectOnGross,
//                                        IsChecked = ids.Contains(x.DeductionId),
//                                    }).ToListAsync();
//                            respone.Deduction = deductionList;
//                        }

//                        #endregion Deduction

//                        res.Message = "Ad Hoc Component";
//                        res.Status = false;
//                        res.Data = respone;
//                    }
//                    else
//                    {
//                        res.Message = "Pay Roll Setup not Created";
//                        res.Status = false;
//                    }
//                }
//                else
//                {
//                    res.Message = "Pay Group Not Found";
//                    res.Status = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Ad Hoc In Salery Components

//        ///----------Pay Group Statutory Flling Screen --------///

//        #region API To Add Statutory Flling Section

//        /// <summary>
//        /// Created By Harshit Mitra On 24-06-2022
//        /// API >> Post >> api/payroll/addupdatestatutoryflling
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("addupdatestatutoryflling")]
//        public async Task<ResponseBodyModel> AddUpdateIncomeTaxSections(PayRollStatutoryFlling model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payGroup = await _db.PayGroups.Where(x => x.PayGroupId == model.PayGroupId && x.CompanyId == claims.companyId).FirstOrDefaultAsync();
//                if (payGroup != null)
//                {
//                    var payRollComponentSetup = await _db.PayRollSetups.FirstOrDefaultAsync(x => x.IsActive == true &&
//                                    x.IsDeleted == false && x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.StatutoryFilling);
//                    if (payRollComponentSetup != null)
//                    {
//                        var check = await _db.PayRollStatutoryFllings.FirstOrDefaultAsync(x => x.PayGroupId == payGroup.PayGroupId);
//                        if (check == null)
//                        {
//                            PayRollStatutoryFlling obj = new PayRollStatutoryFlling
//                            {
//                                PanNo = model.PanNo,
//                                TanNo = model.TanNo,
//                                TanCircleNo = model.TanCircleNo,
//                                CITLocation = model.CITLocation,
//                                Signatory = model.Signatory,
//                                PayGroupId = payGroup.PayGroupId,

//                                CreatedBy = claims.employeeId,
//                                CreatedOn = DateTime.Now,
//                                IsActive = true,
//                                IsDeleted = false,
//                                CompanyId = claims.companyId,
//                                OrgId = 0,
//                            };
//                            _db.PayRollStatutoryFllings.Add(obj);
//                            await _db.SaveChangesAsync();
//                            res.Data = obj;
//                        }
//                        else
//                        {
//                            //-- Income Tax -- //
//                            check.PanNo = String.IsNullOrEmpty(model.PanNo) ? check.PanNo : model.PanNo;
//                            check.TanNo = String.IsNullOrEmpty(model.TanNo) ? check.TanNo : model.TanNo;
//                            check.TanCircleNo = String.IsNullOrEmpty(model.TanCircleNo) ? check.TanCircleNo : model.TanCircleNo;
//                            check.CITLocation = String.IsNullOrEmpty(model.CITLocation) ? check.CITLocation : model.CITLocation;
//                            check.Signatory = String.IsNullOrEmpty(model.Signatory) ? check.Signatory : model.Signatory;

//                            // -- Provident Fund -- //
//                            check.PfName = String.IsNullOrEmpty(model.PfName) ? check.PfName : model.PfName;
//                            check.PfRegistationDate = !model.PfRegistationDate.HasValue ? check.PfRegistationDate : model.PfRegistationDate;
//                            check.PFRegistationNo = String.IsNullOrEmpty(model.PFRegistationNo) ? check.PFRegistationNo : model.PFRegistationNo;
//                            check.SignatoryDesignation = String.IsNullOrEmpty(model.SignatoryDesignation) ? check.SignatoryDesignation : model.SignatoryDesignation;
//                            check.SignatoryFatherName = String.IsNullOrEmpty(model.SignatoryFatherName) ? check.SignatoryFatherName : model.SignatoryFatherName;

//                            // -- ESI -- //
//                            check.EsiName = String.IsNullOrEmpty(model.EsiName) ? check.EsiName : model.EsiName;
//                            check.EsiRegistationDate = !model.EsiRegistationDate.HasValue ? check.EsiRegistationDate : model.EsiRegistationDate;
//                            check.ESIRegistationNo = String.IsNullOrEmpty(model.ESIRegistationNo) ? check.ESIRegistationNo : model.ESIRegistationNo;
//                            check.SignatoryDesignation = String.IsNullOrEmpty(model.SignatoryDesignation) ? check.SignatoryDesignation : model.SignatoryDesignation;
//                            check.SignatoryFatherName = String.IsNullOrEmpty(model.SignatoryFatherName) ? check.SignatoryFatherName : model.SignatoryFatherName;

//                            // Professional TAX -- //
//                            check.StateId = model.StateId == 0 ? check.StateId : model.StateId;
//                            check.States = _db.State.Where(x => x.StateId == check.StateId).Select(x => x.StateName).FirstOrDefault();
//                            check.EstablishmentId = model.EstablishmentId == 0 ? check.EstablishmentId : model.EstablishmentId;
//                            check.PtRegistationDate = !model.PtRegistationDate.HasValue ? check.PtRegistationDate : model.PtRegistationDate;

//                            check.UpdatedBy = claims.employeeId;
//                            check.UpdatedOn = DateTime.Now;

//                            _db.Entry(check).State = System.Data.Entity.EntityState.Modified;
//                            await _db.SaveChangesAsync();
//                            res.Data = check;
//                        }
//                        res.Message = "Saved";
//                        res.Status = true;

//                        if (model.IsCompleted)
//                        {
//                            payRollComponentSetup.UpdatedOn = DateTime.Now;
//                            payRollComponentSetup.UpdatedBy = claims.userId;
//                            payRollComponentSetup.Status = Enum.GetName(typeof(PayrollSetupStatus), (int)PayrollSetupStatus.COMPLETED);

//                            _db.Entry(payRollComponentSetup).State = System.Data.Entity.EntityState.Modified;
//                            await _db.SaveChangesAsync();

//                            payGroup.IsCompleted = true;
//                            _db.Entry(payGroup).State = System.Data.Entity.EntityState.Modified;
//                            await _db.SaveChangesAsync();
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Roll Setup not Created";
//                        res.Status = false;
//                    }
//                }
//                else
//                {
//                    res.Message = "Pay Group Not Found";
//                    res.Status = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion API To Add Statutory Flling Section

//        #region API To Get Statutory Flling

//        /// <summary>
//        /// Created By Harshit Mitra on 24-06-2022
//        /// API >> api/payroll/getstatutoryflling
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getstatutoryflling")]
//        public async Task<ResponseBodyModel> GetStatutoryFlling(int payGroupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payGroup = await _db.PayGroups.Where(x => x.PayGroupId == payGroupId && x.CompanyId == claims.companyId).FirstOrDefaultAsync();
//                if (payGroup != null)
//                {
//                    var check = await _db.PayRollStatutoryFllings.FirstOrDefaultAsync(x => x.PayGroupId == payGroup.PayGroupId);
//                    if (check != null)
//                    {
//                        res.Message = "Statutory Flling";
//                        res.Status = true;
//                        res.Data = check;
//                    }
//                    else
//                    {
//                        res.Message = "Statutory Flling Not Found";
//                        res.Status = false;
//                        res.Data = check;
//                    }
//                }
//                else
//                {
//                    res.Message = "Pay Group Not Found";
//                    res.Status = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion API To Get Statutory Flling

//        #region Helper Model Class

//        /// <summary>
//        /// Created By Harshit Mitra On 03-06-2022
//        /// </summary>
//        public class AddGetSaleryConfiguratuionHelperClass
//        {
//            public int PayGroupId { get; set; }
//            public string StructureName { get; set; }
//            public string Description { get; set; }
//        }

//        public class GetAddComponentData
//        {
//            public int ComponentId { get; set; }
//            public string ComponentName { get; set; }
//            public string AnnualCalculation { get; set; }
//            public bool IsLocked { get; set; }
//        }

//        /// <summary>
//        /// Created By Harshit Mitra on 21-02-2022
//        /// </summary>
//        public class GetPayRollSetupModelClass
//        {
//            public int Step { get; set; }
//            public string Title { get; set; }
//            public string Details { get; set; }
//            public string Status { get; set; }
//            public bool IsCompleted { get; set; }
//            public bool IsPrevious { get; set; }
//            public int prestep { get; set; }
//        }

//        /// <summary>
//        /// Created By Harshit Mitra on 21-02-2022
//        /// </summary>
//        public class GetPayRollMainScreenResponseModelClass
//        {
//            public int PayGroupId { get; set; }
//            public string PayGroupName { get; set; }
//            public string Description { get; set; }
//            public int EmployeeCount { get; set; }
//            public List<GetPayRollSetupModelClass> SetupList { get; set; }
//        }

//        /// <summary>
//        /// Created By Harshit Mitra on 14-04-2022
//        /// </summary>
//        public class AddUpdateCompanyInfoBanks
//        {
//            public int PayGroupId { get; set; }
//            public List<int> BankIds { get; set; }
//        }

//        /// <summary>
//        /// Created By Harshit Mitra on 14-04-2022
//        /// </summary>
//        public class AddUpdateCompanyInfoLocations
//        {
//            public int PayGroupId { get; set; }
//            public List<int> LocationsIds { get; set; }
//        }

//        /// <summary>
//        /// Created By Harshit Mitra on 02-06-2022
//        /// </summary>
//        public class AddSaleryComponentHelperModel
//        {
//            public int PayGroupId { get; set; }
//            public List<AddGetRecurringCmponentHelperModel> RecurringList { get; set; }
//        }

//        /// <summary>
//        /// Created By Harshit Mitra on 02-06-2022
//        /// </summary>
//        public class AddGetRecurringCmponentHelperModel
//        {
//            public int RecuringComponentId { get; set; }
//            public bool IsChecked { get; set; }
//        }

//        /// <summary>
//        /// Created By Harshit Mitra on 02-06-2022
//        /// </summary>
//        public class AddUpdateAdHocHelperClass
//        {
//            public int PayGroupId { get; set; }
//            public AddGetAdHocComponentsHelperModel Components { get; set; }
//        }

//        /// <summary>
//        /// Created By Harshit Mitra on 02-06-2022
//        /// </summary>
//        public class AddGetAdHocComponentsHelperModel
//        {
//            public List<GetAddAdHocHelperModel> AddHoc { get; set; }
//            public List<GetBonusHelperModel> Bonus { get; set; }
//            public List<GetDeductionHelperModel> Deduction { get; set; }
//        }

//        /// <summary>
//        /// Created By Harshit Mitra on 02-06-2022
//        /// </summary>
//        public class GetAddAdHocHelperModel
//        {
//            public int AdHocId { get; set; }
//            public string Name { get; set; }
//            public bool HasTaxBenefits { get; set; }
//            public bool IsChecked { get; set; }
//        }

//        /// <summary>
//        /// Created By Harshit Mitra on 02-06-2022
//        /// </summary>
//        public class GetBonusHelperModel
//        {
//            public int BonusId { get; set; }
//            public string BonusName { get; set; }
//            public bool IsChecked { get; set; }
//        }

//        /// <summary>
//        /// Created By Harshit Mitra on 02-06-2022
//        /// </summary>
//        public class GetDeductionHelperModel
//        {
//            public int DeductionId { get; set; }
//            public string DeductionName { get; set; }
//            public bool HasAffectOnGross { get; set; }
//            public bool IsChecked { get; set; }
//        }

//        /// <summary>
//        /// Created By Ankit on 07-06-2022
//        /// </summary>
//        public class AddIncometax
//        {
//            public int PayGroupId { get; set; }
//            public string PanNo { get; set; }
//            public string TanNo { get; set; }
//            public string TanCircleNo { get; set; }
//            public string CITLocation { get; set; }
//            public string Signatory { get; set; }
//        }

//        /// <summary>
//        /// Created By Ankit on 07-06-2022
//        /// </summary>
//        public class AddProvidendFund
//        {
//            public int PayGroupId { get; set; }
//            public string Name { get; set; }
//            public DateTime? PfRegistationDate { get; set; }
//            public string PFRegistationNo { get; set; }
//            public string SignatoryDesignation { get; set; }
//            public string SignatoryFatherName { get; set; }
//            public string Signatory { get; set; }
//        }

//        /// <summary>
//        /// Created By Ankit on 07-06-2022
//        /// </summary>
//        public class AddEsi
//        {
//            public int PayGroupId { get; set; }
//            public string Name { get; set; }
//            public DateTime? EsiRegistationDate { get; set; }
//            public string ESIRegistationNo { get; set; }
//            public string SignatoryDesignation { get; set; }
//            public string SignatoryFatherName { get; set; }
//            public string Signatory { get; set; }
//        }

//        /// <summary>
//        /// Created By Ankit on 07-06-2022
//        /// </summary>
//        public class AddPTRegistation
//        {
//            public int PayGroupId { get; set; }
//            public string SignatoryDesignation { get; set; }
//            public string SignatoryFatherName { get; set; }
//            public string Signatory { get; set; }
//            public string LocationName { get; set; }
//            public long EstablishmentId { get; set; }
//            public string States { get; set; }
//            public int StateId { get; set; }
//            public DateTime? PtRegistationDate { get; set; }
//        }

//        #endregion Helper Model Class
//    }
//}