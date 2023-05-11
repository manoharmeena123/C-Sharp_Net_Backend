using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.New_Pay_Roll;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.NewPayRoll.PayGroupAndSettings
{
    /// <summary>
    /// Created By Harshit Mitra On 16/12/2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/payroll")]
    public class PayGroupSettingController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// -------------------- Company Information APIs ---------------------- ///

        #region API TO ADD COMPANY INFOMATION 
        /// <summary>
        /// Created By Harshit Mitra On 16/12/2022
        /// API >> POST >> api/payroll/addupdatecompanyinfo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addupdatecompanyinfo")]
        public async Task<IHttpActionResult> AddCompanyInfo(CreateCompanyInfoRequest model)
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
                    var obj = await _db.CompanyInfos
                        .FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId);
                    if (obj == null)
                    {
                        CompanyInformation companyInfo = new CompanyInformation
                        {
                            PayGroupId = model.PayGroupId,
                            CreatedBy = tokenData.employeeId,
                            CompanyId = tokenData.companyId,
                        };

                        _db.CompanyInfos.Add(companyInfo);
                        await _db.SaveChangesAsync();
                        obj = companyInfo;
                    }
                    else
                    {
                        obj.UpdatedBy = tokenData.employeeId;
                        obj.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                    }
                    obj.SignatoryUrl = model.SignatoryUrl;
                    obj.LegalNameOfCompany = model.LegalNameOfCompany;
                    obj.CompanyIdentifyNumber = model.CompanyIdentifyNumber;
                    obj.DateOfIncorporation = TimeZoneConvert.ConvertTimeToSelectedZone(model.DateOfIncorporation.Date);
                    obj.TypeOfBusinessId = model.TypeOfBusinessId;
                    obj.NatureOfBusinessId = model.NatureOfBusinessId;
                    obj.SectorId = model.SectorId;
                    obj.AddressLine1 = model.AddressLine1;
                    obj.AddressLine2 = model.AddressLine2;
                    obj.CountryId = model.CountryId;
                    obj.StateId = model.StateId;
                    obj.CityId = model.CityId;
                    obj.ZipCode = model.ZipCode;

                    _db.Entry(obj).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Company Information Saved";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = obj;

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payroll/addupdatecompanyinfo | " +
                     "Model : " + JsonConvert.SerializeObject(model) + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET COMPANY INFOMATION 
        /// <summary>
        /// Created By Harshit Mitra On 16/12/2022
        /// API >> GET >> api/payroll/getcompanyinfo
        /// </summary>
        /// <param name="payGroupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcompanyinfo")]
        public async Task<IHttpActionResult> AddCompanyInfo(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var companyInfo = await _db.CompanyInfos
                    .FirstOrDefaultAsync(x => x.PayGroupId == payGroupId);
                if (companyInfo == null)
                {
                    res.Message = "Company Info Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;

                    return Ok(res);
                }

                res.Message = "Company Information Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = companyInfo;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payroll/getcompanyinfo | " +
                     "Pay Group Id : " + payGroupId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO ADD COMPANY INFORMATION BANKS
        /// <summary>
        /// Created By Harshit Mitra On 16/12/2022
        /// API >> POST >> api/payroll/addupdatecompanyinfobank
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addupdatecompanyinfobank")]
        public async Task<IHttpActionResult> AddCompanyInfoBank(AddCompanyInfoBankRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var checkBanks = await _db.CompanyInfoBanks
                    .Where(x => x.PayGroupId == model.PayGroupId)
                    .ToListAsync();
                if (checkBanks.Count > 0)
                {
                    _db.CompanyInfoBanks.RemoveRange(checkBanks);
                    await _db.SaveChangesAsync();
                }
                var adBankInfo = model.BankIds
                    .Select(x => new CompanyInfoBank
                    {
                        PayGroupId = model.PayGroupId,
                        BankId = x,

                        CompanyId = tokenData.companyId,
                        CreatedBy = tokenData.employeeId,
                    })
                    .ToList();

                _db.CompanyInfoBanks.AddRange(adBankInfo);
                await _db.SaveChangesAsync();

                res.Message = "Bank Info Updated";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                res.Data = (from ib in adBankInfo
                            join b in _db.BankMaster on ib.BankId equals b.BankId
                            select new
                            {
                                b.BankId,
                                b.BankName,
                                b.Address,
                            })
                            .OrderBy(b => b.BankName)
                            .ToList();

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payroll/addupdatecompanyinfobank | " +
                     "Model : " + JsonConvert.SerializeObject(model) + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET COMPANY INFO BANKS BY PAY GROUP ID
        /// <summary>
        /// Created By Harshit Mitra On 16/12/2022
        /// API >> POST >> api/payroll/getcomponyinfobank
        /// </summary>
        /// <param name="payGroupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcomponyinfobank")]
        public async Task<IHttpActionResult> GetComponyInfoBank(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getBanks = await (from ib in _db.CompanyInfoBanks
                                      join b in _db.BankMaster on ib.BankId equals b.BankId
                                      where ib.PayGroupId == payGroupId
                                      select new
                                      {
                                          b.BankId,
                                          b.BankName,
                                          b.Address,
                                      })
                                        .OrderBy(b => b.BankName)
                                        .ToListAsync();
                if (getBanks.Count == 0)
                {
                    res.Message = "Bank Not Added";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = getBanks;

                    return Ok(res);
                }
                res.Message = "Bank Info Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = getBanks;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payroll/getcomponyinfobank | " +
                     "PayGroupId : " + payGroupId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO ADD COMPANY INFORMATION LOCATION
        /// <summary>
        /// Created By Harshit Mitra On 16/12/2022
        /// API >> POST >> api/payroll/addupdatecompanyinfolocations
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addupdatecompanyinfolocations")]
        public async Task<IHttpActionResult> AddCompanyInfoLocation(AddCompanyInfoLocationRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var checkLocations = await _db.CompanyInfoLocations
                    .Where(x => x.PayGroupId == model.PayGroupId)
                    .ToListAsync();
                if (checkLocations.Count > 0)
                {
                    _db.CompanyInfoLocations.RemoveRange(checkLocations);
                    await _db.SaveChangesAsync();
                }
                var addLocationInfo = model.LocationsIds
                    .Select(x => new CompanyInfoLocation
                    {
                        PayGroupId = model.PayGroupId,
                        LocationId = x,

                        CompanyId = tokenData.companyId,
                        CreatedBy = tokenData.employeeId,
                    })
                    .ToList();

                var setup = await _db.PayGroupSetups
                    .FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId
                        && x.StepsInSettings == EnumClass.PayRollSetupConstants.Company_Info);
                setup.IsSetupComplete = true;
                _db.Entry(setup).State = EntityState.Modified;

                _db.CompanyInfoLocations.AddRange(addLocationInfo);
                await _db.SaveChangesAsync();

                res.Message = "Location Info Updated";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                res.Data = (from il in addLocationInfo
                            join l in _db.Locations on il.LocationId equals l.LocationId
                            where il.PayGroupId == model.PayGroupId
                            select new
                            {
                                l.LocationId,
                                l.LocationName,
                                l.Address,
                            })
                            .OrderBy(b => b.LocationName)
                            .ToList();

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payroll/addupdatecompanyinfolocations | " +
                     "Model : " + JsonConvert.SerializeObject(model) + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET COMPANY INFO LOCATION BY PAY GROUP ID
        /// <summary>
        /// Created By Harshit Mitra On 16/12/2022
        /// API >> POST >> api/payroll/getcompanylocations
        /// </summary>
        /// <param name="payGroupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcompanylocations")]
        public async Task<IHttpActionResult> GetComponyInfoLocations(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var getLocations = await (from il in _db.CompanyInfoLocations
                                          join l in _db.Locations on il.LocationId equals l.LocationId
                                          where il.PayGroupId == payGroupId
                                          select new
                                          {
                                              l.LocationId,
                                              l.LocationName,
                                              l.Address,
                                          })
                                          .OrderBy(b => b.LocationName)
                                          .ToListAsync();

                if (getLocations.Count == 0)
                {
                    res.Message = "Location Not Added";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = getLocations;

                    return Ok(res);
                }
                res.Message = "Location Info Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = getLocations;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payroll/getcomponyinfobank | " +
                     "PayGroupId : " + payGroupId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion



        /// -------------------- General Payroll Settings APIs ---------------------- ///

        #region API TO ADD GENERAL PAY ROLL SETTING 
        /// <summary>
        /// Created By Harshit Mitra On 16/12/2022
        /// API >> POST >> api/payroll/addgeneralpayrollsetting
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addgeneralpayrollsetting")]
        public async Task<IHttpActionResult> GeneralPayRollSetting(CreateGernalPayRollSetting model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var obj = await _db.GeneralPayrollSettings
                    .FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId);
                if (obj == null)
                {
                    GeneralPayrollSetting gernalPayRoll = new GeneralPayrollSetting
                    {
                        PayGroupId = model.PayGroupId,

                        CompanyId = tokenData.companyId,
                        CreatedBy = tokenData.employeeId,
                    };
                    _db.GeneralPayrollSettings.Add(gernalPayRoll);
                    await _db.SaveChangesAsync();
                    obj = gernalPayRoll;
                }
                else
                {
                    obj.UpdatedBy = tokenData.employeeId;
                    obj.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                }
                obj.PayFrequency = model.PayFrequency;
                obj.PayCycleForHRMS = model.PayCycleForHRMS;
                obj.StartPeriod = model.StartPeriod;
                obj.EndPeriod = model.EndPeriod;
                obj.TotalPayDays = model.TotalPayDays;
                obj.ExcludeWeeklyOffs = model.ExcludeWeeklyOffs;
                obj.ExcludeHolidays = model.ExcludeHolidays;
                obj.CurrencyId = model.CurrencyId;
                obj.CurrencyName = model.CurrencyName;
                obj.RemunerationMonthly = model.RemunerationMonthly;
                obj.RemunerationDaily = model.RemunerationDaily;

                var setup = await _db.PayGroupSetups
                    .FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId
                        && x.StepsInSettings == EnumClass.PayRollSetupConstants.General_Setting);
                setup.IsSetupComplete = true;
                _db.Entry(setup).State = EntityState.Modified;

                _db.Entry(obj).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "General Pay Roll Setting Updated";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;
                res.Data = obj;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payroll/addgeneralpayrollsetting | " +
                     "Model : " + JsonConvert.SerializeObject(model) + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET GENERAL PAY ROLL SETTING BY PAY GROUP ID
        /// <summary>
        /// Created By Harshit Mitra On 16/12/2022
        /// API >> GET >> api/payroll/getgernalpayrollsetting
        /// </summary>
        /// <param name="payGroupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getgernalpayrollsetting")]
        public async Task<IHttpActionResult> GetGernalPayRollSettings(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var generalSettinngs = await _db.GeneralPayrollSettings
                    .FirstOrDefaultAsync(x => x.PayGroupId == payGroupId);
                if (generalSettinngs == null)
                {
                    res.Message = "General Setting Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;

                    return Ok(res);
                }

                res.Message = "General Setting Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = generalSettinngs;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payroll/getgernalpayrollsetting | " +
                     "Pay Group Id : " + payGroupId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion



        /// -------------------- PF And ESI Settings APIs ---------------------- ///

        #region API TO ADD PF SETTING 
        /// <summary>
        /// Created By Harshit Mitra On 16/12/2022
        /// API >> POST >> api/payroll/addupdatepf
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addupdatepf")]
        public async Task<IHttpActionResult> AddUpdatePFSettings(CreatePFSettingsRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var obj = await _db.PFSettings
                    .FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId);
                if (obj == null)
                {
                    PFSettings ptSetting = new PFSettings
                    {
                        PayGroupId = model.PayGroupId,

                        CompanyId = tokenData.companyId,
                        CreatedBy = tokenData.employeeId,
                    };
                    _db.PFSettings.Add(ptSetting);
                    await _db.SaveChangesAsync();
                    obj = ptSetting;
                }
                else
                {
                    obj.UpdatedBy = tokenData.employeeId;
                    obj.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                }
                obj.IsPFRequired = model.IsPFRequired;
                obj.MinimumPFAmount = model.MinimumPFAmount;
                obj.PfCalculationType = model.PfCalculationType;
                obj.IsAllowOverridingOfPf = obj.PfCalculationType == PfCalculationType.Allow_PF_calculated_as_percentage_of_basic_salary_beyond_statutory_minimum
                                    ? model.IsAllowOverridingOfPf : false;
                obj.IsPayEmployeePFOutsideGross = model.IsPayEmployeePFOutsideGross;
                obj.IsLimitEmpPFMaxAmount = obj.IsPayEmployeePFOutsideGross ? model.IsPayEmployeePFOutsideGross : false;
                obj.LimitEmpPFMaxAmountMonthly = obj.IsLimitEmpPFMaxAmount ? model.LimitEmpPFMaxAmountMonthly : 0;
                obj.IsHideOtherChargesPFPaySlip = model.IsHideOtherChargesPFPaySlip;
                obj.IsPayOtherChargesOutsideGross = model.IsPayOtherChargesOutsideGross;
                obj.IsHidePFEmpInPaySlip = model.IsHidePFEmpInPaySlip;
                obj.IsEmpContributeVPF = model.IsEmpContributeVPF;
                obj.Is1poin16PerShareOfPension = model.Is1poin16PerShareOfPension;
                obj.IsAdminToOveridePF = model.IsAdminToOveridePF;

                _db.Entry(obj).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "PF Settings Updated";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;
                res.Data = obj;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payroll/addupdatepf | " +
                     "Model : " + JsonConvert.SerializeObject(model) + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET PF SETTINGS
        /// <summary>
        /// Created By Harshit Mitra On 16/12/2022
        /// API >> GET >> api/payroll/getpfsettings
        /// </summary>
        /// <param name="payGroupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getpfsettings")]
        public async Task<IHttpActionResult> GetPFSettings(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var pfSettings = await _db.PFSettings
                    .FirstOrDefaultAsync(x => x.PayGroupId == payGroupId);
                if (pfSettings == null)
                {
                    res.Message = "PF Setting Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;

                    return Ok(res);
                }

                res.Message = "PF Setting Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = pfSettings;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payroll/getpfsettings | " +
                     "Pay Group Id : " + payGroupId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO ADD ESI SETTING 
        /// <summary>
        /// Created By Harshit Mitra On 16/12/2022
        /// API >> POST >> api/payroll/addupdateesi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addupdateesi")]
        public async Task<IHttpActionResult> AddUpdateEsiSetting(CreateESISettingRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var obj = await _db.ESISettings
                    .FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId);
                if (obj == null)
                {
                    ESISetting esiSetting = new ESISetting
                    {
                        PayGroupId = model.PayGroupId,

                        CompanyId = tokenData.companyId,
                        CreatedBy = tokenData.employeeId,
                    };
                    _db.ESISettings.Add(esiSetting);
                    await _db.SaveChangesAsync();
                    obj = esiSetting;
                }
                else
                {
                    obj.UpdatedBy = tokenData.employeeId;
                    obj.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                }
                obj.IsESIStatus = model.IsESIStatus;
                obj.EligibleAmountForESI = model.EligibleAmountForESI;
                obj.MinESIEmpContributionOfGross = model.MinESIEmpContributionOfGross;
                obj.MaxESIEmpContributionOfGross = model.MaxESIEmpContributionOfGross;
                obj.IsAllowESIatSalary = model.IsAllowESIatSalary;
                obj.IsPayESIEmpOutsideGross = model.IsPayESIEmpOutsideGross;
                obj.IsHideESIEmpPaySlip = model.IsHideESIEmpPaySlip;
                obj.IsExcludeEmpShareFromGrossESI = model.IsExcludeEmpShareFromGrossESI;
                obj.IsExcludeEmpGratutyFromGrossESI = model.IsExcludeEmpGratutyFromGrossESI;
                obj.IsRestrictESIGrossDuringContribution = model.IsRestrictESIGrossDuringContribution;
                obj.IsIncludeBonusandOneTimePaymentForESIEligibility = model.IsIncludeBonusandOneTimePaymentForESIEligibility;
                obj.IsIncludeBonusandOneTimePaymentForESIContribution = model.IsIncludeBonusandOneTimePaymentForESIContribution;

                var setup = await _db.PayGroupSetups
                   .FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId
                       && x.StepsInSettings == EnumClass.PayRollSetupConstants.PF_ESI_Setting);
                setup.IsSetupComplete = true;
                _db.Entry(setup).State = EntityState.Modified;

                _db.Entry(obj).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "ESI Settings Updated";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;
                res.Data = obj;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payroll/addupdateesi | " +
                     "Model : " + JsonConvert.SerializeObject(model) + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET PF SETTINGS
        /// <summary>
        /// Created By Harshit Mitra On 16/12/2022
        /// API >> GET >> api/payroll/getesisettings
        /// </summary>
        /// <param name="payGroupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getesisettings")]
        public async Task<IHttpActionResult> GetESISettings(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var esiSetting = await _db.ESISettings
                    .FirstOrDefaultAsync(x => x.PayGroupId == payGroupId);
                if (esiSetting == null)
                {
                    res.Message = "ESI Setting Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;

                    return Ok(res);
                }

                res.Message = "ESI Setting Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = esiSetting;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payroll/getesisettings | " +
                     "Pay Group Id : " + payGroupId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion



        /// -------------------- Component In Pay Group APIs ---------------------- ///

        #region API TO GET RECURING COPONENT ON COMPONENT ADDING IN PAY GROUP
        /// <summary>
        /// Created By Harshit Mitra On 16/12/2022
        /// API >> GET >> api/payroll/getrconsalerycomponent
        /// </summary>
        /// <param name="payGroupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getrconsalerycomponent")]
        public async Task<IHttpActionResult> GetRecurringComponentOnComponentAddInPayGroup(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var recuringComponents = await _db.RecuringComponents
                    .Where(x => x.IsActive && !x.IsDeleted &&
                        x.CompanyId == tokenData.companyId)
                    .ToListAsync();
                var componentsInPayGroup = await _db.ComponentInPays
                    .Where(x => x.PayGroupId == payGroupId && x.ComponentType == ComponentTypeInPGConstants.RecurringComponent)
                    .Select(x => x.ComponentId)
                    .ToListAsync();
                if (componentsInPayGroup.Count > 0)
                {
                    res.Message = "Components List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = recuringComponents
                        .Select(r => new
                        {
                            r.RecuringComponentId,
                            r.ComponentName,
                            ComponentType = r.ComponentType.ToString().Replace("_", " "),
                            r.IncomeTaxSection,
                            r.IsTaxExempted,
                            r.IsAutoCalculated,
                            r.MaxiumLimitPerYear,
                            r.SectionMaxLimit,
                            r.IsDocumentRequired,
                            IsChecked = componentsInPayGroup.Contains(r.RecuringComponentId),
                            IsAutoChecked = (r.ComponentName == "Basic" || r.ComponentName == "HRA"),

                        })
                        .OrderByDescending(x => x.IsChecked)
                        .ToList();
                }
                else
                {
                    res.Message = "Components List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = recuringComponents
                        .Select(r => new
                        {
                            r.RecuringComponentId,
                            r.ComponentName,
                            ComponentType = r.ComponentType.ToString().Replace("_", " "),
                            r.IncomeTaxSection,
                            r.IsTaxExempted,
                            r.IsAutoCalculated,
                            r.MaxiumLimitPerYear,
                            r.SectionMaxLimit,
                            r.IsDocumentRequired,
                            IsChecked = (r.ComponentName == "Basic" || r.ComponentName == "HRA"),
                            IsAutoChecked = (r.ComponentName == "Basic" || r.ComponentName == "HRA"),

                        })
                        .OrderByDescending(x => x.IsChecked)
                        .ToList();
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payroll/getrconsalerycomponent | " +
                     "Pay Group Id : " + payGroupId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO ADD RECURING COMPONENTS IN PAY GROUP 
        /// <summary>
        /// Created By Harshit Mitra On 16/12/2022
        /// API >> POST >> api/payroll/addupdatesalerycomonent
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addupdatesalerycomonent")]
        public async Task<IHttpActionResult> AddUpdateSaleryCompanents(AddComponentInPayGroupRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var componentsInPayGroup = await _db.ComponentInPays
                    .Where(x => x.PayGroupId == model.PayGroupId &&
                            x.ComponentType == ComponentTypeInPGConstants.RecurringComponent)
                    .ToListAsync();
                if (componentsInPayGroup.Count != 0)
                    _db.ComponentInPays.RemoveRange(componentsInPayGroup);

                var addComponentList = model.ComponentIds
                    .Where(x => x.IsChecked)
                    .Select(x => new ComponentInPayGroup
                    {
                        PayGroupId = model.PayGroupId,
                        ComponentType = ComponentTypeInPGConstants.RecurringComponent,
                        ComponentId = x.RecuringComponentId,

                        CompanyId = tokenData.companyId,
                        CreatedBy = tokenData.employeeId,
                    })
                    .ToList();

                _db.ComponentInPays.AddRange(addComponentList);
                await _db.SaveChangesAsync();

                res.Message = "Components Updated";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payroll/addupdatesalerycomonent | " +
                     "Model : " + JsonConvert.SerializeObject(model) + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET AD-HOC COMPONENT LIST WITH BONOUS AND DEDUCTIONS
        /// <summary>
        /// Created By Harshit Mitra On 19/12/2022
        /// API >> GET >> api/payroll/getadhocosalerycomponent
        /// </summary>
        /// <param name="payGroupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getadhocosalerycomponent")]
        public async Task<IHttpActionResult> GetAdHocOnSaleryComponent(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var adHocComponents = await _db.AdHocComponents
                    .Where(x => x.IsActive && !x.IsDeleted &&
                        x.CompanyId == tokenData.companyId)
                    .ToListAsync();
                var componentsInPayGroup = await _db.ComponentInPays
                    .Where(x => x.PayGroupId == payGroupId &&
                            x.ComponentType != ComponentTypeInPGConstants.RecurringComponent)
                    .Select(x => x.ComponentId)
                    .ToListAsync();
                if (componentsInPayGroup.Count > 0)
                {
                    var componentList = adHocComponents
                        .Select(x => new
                        {
                            x.ComponentId,
                            x.Title,
                            x.Description,
                            x.HasTaxBenefits,
                            x.ComponentType,
                            IsChecked = componentsInPayGroup.Contains(x.ComponentId),
                        })
                        .OrderByDescending(x => x.IsChecked)
                        .ToList();
                    res.Message = "Components List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = new
                    {
                        AddHoc = componentList
                            .Where(x => x.ComponentType == AdHocComponentTypeConstants.Ad_Hoc_Component)
                            .OrderByDescending(x => x.IsChecked).ToList(),
                        Bonus = componentList
                            .Where(x => x.ComponentType == AdHocComponentTypeConstants.Bonus_Component)
                            .OrderByDescending(x => x.IsChecked).ToList(),
                        Deduction = componentList
                            .Where(x => x.ComponentType == AdHocComponentTypeConstants.Deduction_Component)
                            .OrderByDescending(x => x.IsChecked).ToList(),
                    };
                }
                else
                {
                    var componentList = adHocComponents
                        .Select(x => new
                        {
                            x.ComponentId,
                            x.Title,
                            x.Description,
                            x.HasTaxBenefits,
                            x.ComponentType,
                            IsChecked = false,
                        })
                        .OrderByDescending(x => x.IsChecked)
                        .ToList();
                    res.Message = "Components List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = new
                    {
                        AddHoc = componentList
                            .Where(x => x.ComponentType == AdHocComponentTypeConstants.Ad_Hoc_Component)
                            .OrderByDescending(x => x.IsChecked).ToList(),
                        Bonus = componentList
                            .Where(x => x.ComponentType == AdHocComponentTypeConstants.Bonus_Component)
                            .OrderByDescending(x => x.IsChecked).ToList(),
                        Deduction = componentList
                            .Where(x => x.ComponentType == AdHocComponentTypeConstants.Deduction_Component)
                            .OrderByDescending(x => x.IsChecked).ToList(),
                    };
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payroll/getadhocosalerycomponent | " +
                     "Pay Group Id : " + payGroupId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO ADD AD-HOC COMPONENTS IN PAY GORUP 
        /// <summary>
        /// Created By Harshit Mitra On 19/12/2022
        /// API >> POST >> api/payroll/addupdatextracomponent
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addupdatextracomponent")]
        public async Task<IHttpActionResult> AddUpdatExtraComponent(AddExtraComponentInPayGroupRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var componentsInPayGroup = await _db.ComponentInPays
                    .Where(x => x.PayGroupId == model.PayGroupId &&
                            x.ComponentType != ComponentTypeInPGConstants.RecurringComponent)
                    .ToListAsync();
                if (componentsInPayGroup.Count != 0)
                    _db.ComponentInPays.RemoveRange(componentsInPayGroup);

                var addComponentList = model.Components.AddHoc
                    .Where(x => x.IsChecked)
                    .Select(x => new ComponentInPayGroup
                    {
                        PayGroupId = model.PayGroupId,
                        ComponentType = ComponentTypeInPGConstants.AdHocComponent,
                        ComponentId = x.ComponentId,

                        CompanyId = tokenData.companyId,
                        CreatedBy = tokenData.employeeId,
                    })
                    .ToList();
                addComponentList.AddRange(model.Components.Bonus
                    .Where(x => x.IsChecked)
                    .Select(x => new ComponentInPayGroup
                    {
                        PayGroupId = model.PayGroupId,
                        ComponentType = ComponentTypeInPGConstants.BonusComponent,
                        ComponentId = x.ComponentId,

                        CompanyId = tokenData.companyId,
                        CreatedBy = tokenData.employeeId,
                    })
                    .ToList());
                addComponentList.AddRange(model.Components.Deduction
                    .Where(x => x.IsChecked)
                    .Select(x => new ComponentInPayGroup
                    {
                        PayGroupId = model.PayGroupId,
                        ComponentType = ComponentTypeInPGConstants.DeductionComponent,
                        ComponentId = x.ComponentId,

                        CompanyId = tokenData.companyId,
                        CreatedBy = tokenData.employeeId,
                    })
                    .ToList());

                var setup = await _db.PayGroupSetups
                   .FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId
                       && x.StepsInSettings == EnumClass.PayRollSetupConstants.Salary_Components);
                setup.IsSetupComplete = true;
                _db.Entry(setup).State = EntityState.Modified;

                _db.ComponentInPays.AddRange(addComponentList);
                await _db.SaveChangesAsync();

                res.Message = "Components Updated";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payroll/addupdatextracomponent | " +
                     "Model : " + JsonConvert.SerializeObject(model) + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion



        /// -------------------- Component In Pay Group APIs ---------------------- ///

        #region API TO GET TAX DEDUCTION COMPONENTS ADDING IN PAY GROUP
        /// <summary>
        /// Created By Harshit Mitra On 19/12/2022
        /// API >> GET >> api/payroll/gettaxdeductioncomponent
        /// </summary>
        /// <param name="payGroupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("gettaxdeductioncomponent")]
        public async Task<IHttpActionResult> GetTaxDeductionComponent(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var taxDeductions = await _db.TaxDeductions
                    .Where(x => x.IsActive && !x.IsDeleted &&
                        x.CompanyId == tokenData.companyId)
                    .ToListAsync();
                var componentsInPayGroup = await _db.ComponentInPays
                    .Where(x => x.PayGroupId == payGroupId && x.ComponentType == ComponentTypeInPGConstants.TaxDeductionComponent)
                    .Select(x => x.ComponentId)
                    .ToListAsync();

                var countries = await _db.Country.ToListAsync();
                var states = await _db.State.ToListAsync();

                if (componentsInPayGroup.Count > 0)
                {
                    res.Message = "Components List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = taxDeductions
                        .Select(x => new
                        {
                            IsChecked = componentsInPayGroup.Contains(x.TaxComponentId),
                            x.TaxComponentId,
                            x.DeductionName,
                            x.CountryId,
                            x.StateId,
                            CountryName = countries.Where(z => z.CountryId == x.CountryId).Select(z => z.CountryName).FirstOrDefault(),
                            StateName = states.Where(z => z.StateId == x.StateId).Select(z => z.StateName).FirstOrDefault(),
                            MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month),
                            Deductionfor = x.Deductionfor.ToString(),
                            x.IsPercentage,
                            x.Value,
                            x.Component,
                            x.Limit,
                            x.Max,
                            x.Min,
                        })
                        .OrderByDescending(x => x.IsChecked)
                        .ToList();
                }
                else
                {
                    res.Message = "Components List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = taxDeductions
                        .Select(x => new
                        {
                            IsChecked = false,
                            x.TaxComponentId,
                            x.DeductionName,
                            CountryName = countries.Where(z => z.CountryId == x.CountryId).Select(z => z.CountryName).FirstOrDefault(),
                            StateName = states.Where(z => z.StateId == x.StateId).Select(z => z.StateName).FirstOrDefault(),
                            MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month),
                            Deductionfor = x.Deductionfor.ToString(),
                            x.IsPercentage,
                            x.Value,
                            x.Component,
                            x.Limit,
                            x.Max,
                            x.Min,
                        })
                        .OrderByDescending(x => x.IsChecked)
                        .ToList();
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payroll/gettaxdeductioncomponent | " +
                     "Pay Group Id : " + payGroupId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO ADD TAX DEDUCTION COMPONENTS IN PAY GROUP
        /// <summary>
        /// Created By Harshit Mitra On 19/12/2022
        /// API >> POST >> api/payroll/addupdatetaxcomponent
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addupdatetaxcomponent")]
        public async Task<IHttpActionResult> AddUpdateTaxDeductionComponent(AddTaxDeductionComponetRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var componentsInPayGroup = await _db.ComponentInPays
                    .Where(x => x.PayGroupId == model.PayGroupId &&
                            x.ComponentType == ComponentTypeInPGConstants.TaxDeductionComponent)
                    .ToListAsync();
                if (componentsInPayGroup.Count != 0)
                    _db.ComponentInPays.RemoveRange(componentsInPayGroup);

                var addComponentList = model.ComponentIds
                    .Where(x => x.IsChecked)
                    .Select(x => new ComponentInPayGroup
                    {
                        PayGroupId = model.PayGroupId,
                        ComponentType = ComponentTypeInPGConstants.TaxDeductionComponent,
                        ComponentId = x.TaxComponentId,

                        CompanyId = tokenData.companyId,
                        CreatedBy = tokenData.employeeId,
                    })
                    .ToList();

                var setup = await _db.PayGroupSetups
                   .FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId
                       && x.StepsInSettings == EnumClass.PayRollSetupConstants.Tax_Deduction_Components);
                setup.IsSetupComplete = true;
                _db.Entry(setup).State = EntityState.Modified;

                _db.ComponentInPays.AddRange(addComponentList);
                await _db.SaveChangesAsync();

                res.Message = "Components Updated";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;

                return Ok(res);
            }
            catch (Exception ex)
            {

                logger.Error("API : api/payroll/addupdatetaxcomponent | " +
                     "Model : " + JsonConvert.SerializeObject(model) + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion



        /// -------------------- Statutory Flling Pay Group APIs ---------------------- ///

        #region API TO GET TAX DEDUCTION COMPONENTS ADDING IN PAY GROUP
        /// <summary>
        /// Created By Harshit Mitra On 19/12/2022
        /// API >> GET >> api/payroll/getstatutoryflling
        /// </summary>
        /// <param name="payGroupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getstatutoryflling")]
        public async Task<IHttpActionResult> GetStatutoryFlling(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var statuaryFllings = await _db.StatutoryFllings
                    .FirstOrDefaultAsync(x => x.PayGroupId == payGroupId);
                if (statuaryFllings == null)
                {
                    res.Message = "Statuary Filling Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;

                    return Ok(res);
                }
                res.Message = "Statuary Filling Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = statuaryFllings;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payroll/getstatutoryflling | " +
                     "Pay Group Id : " + payGroupId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO ADD STATUORY FLLINGS IN PAY GROUP
        /// <summary>
        /// Created By Harshit Mitra On 19/12/2022
        /// API >> POST >> api/payroll/addupdatestatutoryflling
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addupdatestatutoryflling")]
        public async Task<IHttpActionResult> AddUpdateIncomeTaxSections(CreateStatuaryFllingsRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var statuaryFllings = await _db.StatutoryFllings
                    .FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId);
                if (statuaryFllings == null)
                {
                    StatutoryFlling obj = new StatutoryFlling
                    {
                        PayGroupId = model.PayGroupId,

                        CompanyId = tokenData.companyId,
                        CreatedBy = tokenData.employeeId,
                    };
                    _db.StatutoryFllings.Add(obj);
                    await _db.SaveChangesAsync();
                    statuaryFllings = obj;
                }
                else
                {
                    statuaryFllings.UpdatedBy = tokenData.companyId;
                    statuaryFllings.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                }

                statuaryFllings.PanNo = String.IsNullOrEmpty(model.PanNo) ? statuaryFllings.PanNo : model.PanNo;
                statuaryFllings.TanNo = String.IsNullOrEmpty(model.TanNo) ? statuaryFllings.TanNo : model.TanNo;
                statuaryFllings.TanCircleNo = String.IsNullOrEmpty(model.TanCircleNo) ? statuaryFllings.TanCircleNo : model.TanCircleNo;
                statuaryFllings.CITLocation = String.IsNullOrEmpty(model.CITLocation) ? statuaryFllings.CITLocation : model.CITLocation;
                statuaryFllings.Signatory = String.IsNullOrEmpty(model.Signatory) ? statuaryFllings.Signatory : model.Signatory;


                statuaryFllings.PfName = String.IsNullOrEmpty(model.PfName) ? statuaryFllings.PfName : model.PfName;
                statuaryFllings.PfRegistationDate = !model.PfRegistationDate.HasValue ? statuaryFllings.PfRegistationDate :
                            TimeZoneConvert.ConvertTimeToSelectedZone((DateTime)model.PfRegistationDate);
                statuaryFllings.PFRegistationNo = String.IsNullOrEmpty(model.PFRegistationNo) ? statuaryFllings.PFRegistationNo : model.PFRegistationNo;
                statuaryFllings.SignatoryDesignation = String.IsNullOrEmpty(model.SignatoryDesignation) ? statuaryFllings.SignatoryDesignation : model.SignatoryDesignation;
                statuaryFllings.SignatoryFatherName = String.IsNullOrEmpty(model.SignatoryFatherName) ? statuaryFllings.SignatoryFatherName : model.SignatoryFatherName;


                statuaryFllings.EsiName = String.IsNullOrEmpty(model.EsiName) ? statuaryFllings.EsiName : model.EsiName;
                statuaryFllings.EsiRegistationDate = !model.EsiRegistationDate.HasValue ? statuaryFllings.EsiRegistationDate :
                            TimeZoneConvert.ConvertTimeToSelectedZone((DateTime)model.EsiRegistationDate);
                statuaryFllings.ESIRegistationNo = String.IsNullOrEmpty(model.ESIRegistationNo) ? statuaryFllings.ESIRegistationNo : model.ESIRegistationNo;
                statuaryFllings.SignatoryDesignation = String.IsNullOrEmpty(model.SignatoryDesignation) ? statuaryFllings.SignatoryDesignation : model.SignatoryDesignation;
                statuaryFllings.SignatoryFatherName = String.IsNullOrEmpty(model.SignatoryFatherName) ? statuaryFllings.SignatoryFatherName : model.SignatoryFatherName;

                statuaryFllings.EstablishmentId = String.IsNullOrEmpty(model.EstablishmentId) ? statuaryFllings.EstablishmentId : model.EstablishmentId;
                statuaryFllings.StateId = model.StateId == 0 ? statuaryFllings.StateId : model.StateId;
                statuaryFllings.PtRegistationDate = !model.PtRegistationDate.HasValue ? statuaryFllings.PtRegistationDate :
                            TimeZoneConvert.ConvertTimeToSelectedZone((DateTime)model.PtRegistationDate);
                if (model.IsSubmitted)
                {
                    var setup = await _db.PayGroupSetups
                        .FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId
                        && x.StepsInSettings == PayRollSetupConstants.Statutory_Filling);
                    var payGroup = await _db.PayGroups
                        .FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId);
                    setup.IsSetupComplete = true;
                    payGroup.IsCompleted = true;
                    _db.Entry(setup).State = EntityState.Modified;
                    _db.Entry(payGroup).State = EntityState.Modified;
                }

                _db.Entry(statuaryFllings).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "Setting Updated";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;
                res.Data = statuaryFllings;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payroll/addupdatestatutoryflling | " +
                     "Model : " + JsonConvert.SerializeObject(model) + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion





        #region REQUEST AND RESPONSE

        public class CreateCompanyInfoRequest
        {
            [Required]
            public Guid PayGroupId { get; set; }
            public string SignatoryUrl { get; set; } = String.Empty;
            public string LegalNameOfCompany { get; set; } = String.Empty;
            public string CompanyIdentifyNumber { get; set; } = String.Empty;
            public DateTime DateOfIncorporation { get; set; } = DateTime.UtcNow;
            public int TypeOfBusinessId { get; set; } = 0;
            public int SectorId { get; set; } = 0;
            public int NatureOfBusinessId { get; set; } = 0;
            public string AddressLine1 { get; set; } = String.Empty;
            public string AddressLine2 { get; set; } = String.Empty;
            public int CityId { get; set; } = 0;
            public int StateId { get; set; } = 0;
            public string ZipCode { get; set; } = String.Empty;
            public int CountryId { get; set; } = 0;
        }
        public class AddCompanyInfoBankRequest
        {
            [Required]
            public Guid PayGroupId { get; set; } = Guid.Empty;
            public List<int> BankIds { get; set; }
        }
        public class AddCompanyInfoLocationRequest
        {
            [Required]
            public Guid PayGroupId { get; set; } = Guid.Empty;
            public List<int> LocationsIds { get; set; }
        }
        public class CreateGernalPayRollSetting
        {
            [Required]
            public Guid PayGroupId { get; set; } = Guid.Empty;
            public string PayFrequency { get; set; } = String.Empty;
            public string PayCycleForHRMS { get; set; } = String.Empty;
            public string StartPeriod { get; set; } = String.Empty;
            public string EndPeriod { get; set; } = String.Empty;
            public int TotalPayDays { get; set; } = 0;
            public bool ExcludeWeeklyOffs { get; set; } = false;
            public bool ExcludeHolidays { get; set; } = false;
            public string CurrencyId { get; set; } = String.Empty;
            public string CurrencyName { get; set; } = String.Empty;
            public bool RemunerationMonthly { get; set; } = false;
            public bool RemunerationDaily { get; set; } = false;
        }
        public class CreatePFSettingsRequest
        {
            [Required]
            public Guid PayGroupId { get; set; } = Guid.Empty;
            public bool IsPFRequired { get; set; } = false;
            public double MinimumPFAmount { get; set; } = 0.0;
            public PfCalculationType PfCalculationType { get; set; } = PfCalculationType.Not_Selected;
            public bool IsAllowOverridingOfPf { get; set; } = false;
            public bool IsPayEmployeePFOutsideGross { get; set; } = false;
            public bool IsLimitEmpPFMaxAmount { get; set; } = false;
            public double LimitEmpPFMaxAmountMonthly { get; set; } = 0.0;
            public bool IsHidePFEmpInPaySlip { get; set; } = false;
            public bool IsPayOtherChargesOutsideGross { get; set; } = false;
            public bool IsHideOtherChargesPFPaySlip { get; set; } = false;
            public bool IsEmpContributeVPF { get; set; } = false;
            public bool Is1poin16PerShareOfPension { get; set; } = false;
            public bool IsAdminToOveridePF { get; set; } = false;
        }
        public class CreateESISettingRequest
        {
            [Required]
            public Guid PayGroupId { get; set; } = Guid.Empty;
            public bool IsESIStatus { get; set; } = false;
            public double EligibleAmountForESI { get; set; } = 0.0;
            public double MinESIEmpContributionOfGross { get; set; } = 0.0;
            public double MaxESIEmpContributionOfGross { get; set; } = 0.0;
            public bool IsAllowESIatSalary { get; set; } = false;
            public bool IsPayESIEmpOutsideGross { get; set; } = false;
            public bool IsHideESIEmpPaySlip { get; set; } = false;
            public bool IsExcludeEmpShareFromGrossESI { get; set; } = false;
            public bool IsExcludeEmpGratutyFromGrossESI { get; set; } = false;
            public bool IsRestrictESIGrossDuringContribution { get; set; } = false;
            public bool IsIncludeBonusandOneTimePaymentForESIEligibility { get; set; } = false;
            public bool IsIncludeBonusandOneTimePaymentForESIContribution { get; set; } = false;
        }
        public class AddComponentInPayGroupRequest
        {
            public Guid PayGroupId { get; set; } = Guid.Empty;
            public List<AddCompnentRequest> ComponentIds { get; set; }
        }
        public class AddCompnentRequest
        {
            public Guid RecuringComponentId { get; set; }
            public bool IsChecked { get; set; }
        }

        public class AddExtraComponentInPayGroupRequest
        {
            public Guid PayGroupId { get; set; } = Guid.Empty;
            public ComponentsListRequest Components { get; set; }
        }
        public class ComponentsListRequest
        {
            public List<AddExtraCompnentRequest> AddHoc { get; set; }
            public List<AddExtraCompnentRequest> Bonus { get; set; }
            public List<AddExtraCompnentRequest> Deduction { get; set; }
        }
        public class AddExtraCompnentRequest
        {
            public Guid ComponentId { get; set; }
            public bool IsChecked { get; set; }
        }
        public class AddTaxDeductionComponetRequest
        {
            public Guid PayGroupId { get; set; } = Guid.Empty;
            public List<TaxCompnentRequest> ComponentIds { get; set; }
        }
        public class TaxCompnentRequest
        {
            public Guid TaxComponentId { get; set; }
            public bool IsChecked { get; set; }
        }
        public class CreateStatuaryFllingsRequest
        {
            public Guid PayGroupId { get; set; } = Guid.Empty;
            public string PanNo { get; set; } = String.Empty;
            public string TanNo { get; set; } = String.Empty;
            public string TanCircleNo { get; set; } = String.Empty;
            public string CITLocation { get; set; } = String.Empty;
            public string Signatory { get; set; } = String.Empty;
            public string EsiName { get; set; } = String.Empty;
            public string PfName { get; set; } = String.Empty;
            public DateTime? PfRegistationDate { get; set; }
            public DateTime? EsiRegistationDate { get; set; }
            public DateTime? PtRegistationDate { get; set; }
            public string PFRegistationNo { get; set; } = String.Empty;
            public string SignatoryDesignation { get; set; } = String.Empty;
            public string SignatoryFatherName { get; set; } = String.Empty;
            public string ESIRegistationNo { get; set; } = String.Empty;
            public string EstablishmentId { get; set; } = String.Empty;
            public int StateId { get; set; } = 0;
            public bool IsSubmitted { get; set; } = false;
        }
        #endregion

    }
}
