using AspNetIdentity.Core.Common;
using AspNetIdentity.Core.Model.TsfModule;
using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.Header;
using AspNetIdentity.WebApi.Model.NewClientRequirement.TypeofWork;
using AspNetIdentity.WebApi.Model.ShiftModel;
using AspNetIdentity.WebApi.Model.SmtpModule;
using AspNetIdentity.WebApi.Model.UserAccesPermission;
using AspNetIdentity.WebApi.Models;
using EASendMail;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using static AspNetIdentity.WebApi.Controllers.ClientNewRequirement.TypeofWorkController;
using static AspNetIdentity.WebApi.Helper.ClientHelper;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers
{
    /// <summary>
    /// Modify By Harshit Mitra on 21-04-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/company")]
    public class CompanyController : BaseApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Api To Get Company By Id

        /// <summary>
        /// Modify By Harshit Mitra on 21-04-2022
        /// API >> Get >> api/company/getcompanybyid
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcompanybyid")]
        public async Task<ResponseBodyModel> GetCompanyById(int companyId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var company = await (from c in _db.Company
                                     join cs in _db.CompanySmtpMailModels on c.CompanyId equals cs.CompanyId into r
                                     from result in r.DefaultIfEmpty()
                                     where c.IsActive && !c.IsDeleted && c.CompanyId == companyId
                                     select new
                                     {
                                         IsSmtpProvided = c.IsSmtpProvided,
                                         RegisterCompanyName = c.RegisterCompanyName,
                                         RegisterEmail = c.RegisterEmail,
                                         ConnectType = result == null ? 0 : result.ConnectType,
                                         SmtpServer = result == null ? null : result.SmtpServer,
                                         Port = result == null ? 0 : result.Port,
                                         Password = result == null ? null : result.Password,
                                         MailUser = result == null ? null : result.MailUser,
                                         From = result == null ? null : result.From,
                                         CompanyWebSiteURL = c.CompanyWebSiteURL,
                                         CompanyDefaultTimeZone = c.CompanyDefaultTimeZone,
                                         CountryId = c.CountryId,
                                         CompanyDomain = c.CompanyDomain,
                                         DefaultRole = c.DefaultRole,
                                         DefaultWeekOff = c.DefaultWeekOff,
                                         DefaultShiftId = c.DefaultShiftId,
                                         NavigationLogo = c.NavigationLogo,
                                         AppAdminLogo = c.AppAdminLogo,
                                         IncorporationCertificate = c.IncorporationCertificate,
                                         IncorporationDate = c.IncorporationDate,
                                         PhoneNumber = c.PhoneNumber,
                                         RegisterAddress = c.RegisterAddress,
                                         CIN = c.CIN,
                                         CompanyGst = c.CompanyGst,
                                         ImgKitApiKey = c.ImgKitApiKey,
                                     }).FirstOrDefaultAsync(); ;
                if (company != null)
                {
                    res.Message = "Company Found";
                    res.Status = true;
                    res.Data = company;
                }
                else
                {
                    res.Message = "No Company Found!!";
                    res.Status = false;
                    res.Data = null;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion Api To Get Company By Id

        #region Api To Get All Company

        /// <summary>
        /// Modify By Harshit Mitra on 21-04-2022
        /// API >> Get >> api/company/getallcompany
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getallcompany")]
        public async Task<ResponseBodyModel> GetAllCompany(PagingRequest model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var companyList = await _db.Company
                    .Where(x => x.IsDeleted == false && x.IsActive == true)
                    .Select(x => new GetAllCompanyResponse
                    {
                        CompanyId = x.CompanyId,
                        RegisterCompanyName = x.RegisterCompanyName,
                        CompanyGst = x.CompanyGst,
                        IncorporationDate = x.IncorporationDate,
                        RegisterAddress = x.RegisterAddress,
                        RegisterEmail = x.RegisterEmail,
                        CreatedOn = x.CreatedOn,
                        IsCompanyIsLock = x.IsCompanyIsLock,
                        IsAccessProvided = _db.CompaniesModuleAccesses.Any(z => z.CompanyId == x.CompanyId),
                    })
                    .ToListAsync();
                if (companyList.Count > 0)
                {
                    res.Message = "Company list Found";
                    res.Status = true;
                    res.Data = new PagingResponse<GetAllCompanyResponse>(companyList, model.Page, model.Count);
                }
                else
                {
                    res.Message = "No Company list Found";
                    res.Status = false;
                    res.Data = companyList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class GetAllCompanyResponse
        {
            public int CompanyId { get; set; }
            public string RegisterCompanyName { get; set; }
            public string CompanyGst { get; set; }
            public DateTimeOffset IncorporationDate { get; set; }
            public string RegisterAddress { get; set; }
            public string RegisterEmail { get; set; }
            public DateTimeOffset CreatedOn { get; set; }
            public bool IsCompanyIsLock { get; set; }
            public bool IsAccessProvided { get; set; }
        }
        #endregion Api To Get All Companypay

        #region Api to search Data from company table in superadmin
        /// <summary>
        /// created by bhavendra singh jat 11/10/2022
        ///  API >> Get >> api/company/getsearchcompanydata
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getsearchcompanydata")]

        public async Task<ResponseBodyModel> GetSearchCompany(string search = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (claims.roleType == "SuperAdmin")
                {
                    var company = await _db.Company.Where(x => !x.IsDeleted && x.IsActive).ToListAsync();
                    if (company.Count > 0)
                    {
                        var searchData = await (from com in _db.Company
                                                where com.IsActive && !com.IsDeleted &&
                                                (com.RegisterCompanyName.ToLower().Contains(search.ToLower()) ||
                                                (com.CompanyGst.ToLower().Contains(search.ToLower())) ||
                                                (com.RegisterEmail.ToLower().Contains(search.ToLower())))
                                                select new
                                                {
                                                    com.CompanyId,
                                                    com.CompanyGst,
                                                    com.RegisterCompanyName,
                                                    com.IncorporationDate,
                                                    com.RegisterEmail,
                                                    com.CreatedOn,
                                                    com.RegisterAddress,
                                                }).ToListAsync();
                        if (searchData.Count > 0)
                        {
                            res.Message = "Search result found";
                            res.Status = true;
                            res.Data = searchData;
                        }
                        else
                        {
                            res.Message = "No Search result found";
                            res.Status = true;
                            res.Data = searchData;
                        }
                    }
                    else
                    {
                        res.Message = "No Company Found";
                        res.Status = false;
                    }
                }
                else
                {
                    res.Message = "You Dont Have Authority";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }


        #endregion

        #region Api To Delete Company

        /// <summary>
        /// Modify By Harshit Mitra on 21-04-2022
        /// API >> Get >> api/company/deletecompany
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletecompany")]
        public async Task<ResponseBodyModel> DeleteCompany(int companyId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var company = await _db.Company.FirstOrDefaultAsync(x =>
                        x.CompanyId == companyId && x.IsDeleted == false && x.IsActive == true);
                if (company != null)
                {
                    company.IsDeleted = true;
                    company.IsActive = false;

                    _db.Entry(company).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    res.Status = true;
                    res.Message = "Company Deleted Successfully!";
                }
                else
                {
                    res.Message = "No Company Found!!";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Delete Company

        #region API TO CREATE COMPANY WITH ITS ADMIN
        /// <summary>
        /// Created By Harshit Mitra On 22-04-2022
        /// API >> Post >> api/company/addcompany
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addcompany")]
        public async Task<IHttpActionResult> AddCompany(AddCompanyClassRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (await _db.Employee.AnyAsync(x => x.OfficeEmail.ToUpper() == model.OfficeEmail.ToUpper()))
                {
                    res.Message = "Official Email Is Already In Use";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotAcceptable;
                    return Ok(res);
                }
                var firstName = model.FirstName.Trim();
                var middleName = String.IsNullOrEmpty(model.MiddleName) ? "" : model.MiddleName.Trim();
                var lastName = model.LastName.Trim();

                var checkCompany = await _db.Company.AnyAsync(x =>
                    x.RegisterCompanyName.Trim().ToUpper() == model.RegisterCompanyName.Trim().ToUpper());
                if (checkCompany)
                {
                    res.Message = "Company Already Exist With Same Name";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotAcceptable;
                    return Ok(res);
                }
                Company obj = new Company
                {
                    RegisterCompanyName = model.RegisterCompanyName,
                    CompanyGst = model.CompanyGst,
                    CIN = model.CIN,
                    RegisterAddress = model.RegisterAddress,
                    RegisterEmail = model.RegisterEmail,
                    PhoneNumber = model.PhoneNumber,
                    IncorporationDate = model.IncorporationDate,
                    IncorporationCertificate = model.IncorporationCertificate,
                    CreatedBy = tokenData.userId,
                    AppAdminLogo = model.adminlogo,
                    NavigationLogo = model.Navigationlogo,
                    CountryId = model.CountryId,
                    CompanyDomain = model.CompanyDomain,
                    CompanyWebSiteURL = model.CompanyWebSiteURL,
                    CompanyDefaultTimeZone = String.IsNullOrEmpty(model.CompanyDefaultTimeZone) ? "India Standard Time" : model.CompanyDefaultTimeZone,
                    IsSmtpProvided = model.IsSmtpProvided,
                    ImgKitApiKey = model.ImgKitApiKey,
                };

                obj.CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, obj.CompanyDefaultTimeZone);
                _db.Company.Add(obj);
                int check = await _db.SaveChangesAsync();

                CompanySmtpMailModel demo = new CompanySmtpMailModel
                {
                    From = model.MailUser,
                    MailUser = model.MailUser,
                    Password = model.Password,
                    Port = model.Port,
                    SmtpServer = model.SmtpServer,
                    ConnectType = model.ConnectType,
                    CompanyId = obj.CompanyId,
                };

                _db.CompanySmtpMailModels.Add(demo);
                await _db.SaveChangesAsync();

                if (check == 1)
                {
                    #region Add Department and Designation

                    Department department = new Department
                    {
                        CompanyId = obj.CompanyId,
                        DepartmentName = "Administrator",
                        Description = "Default Created Administrator Department",
                        UsedForLogin = true,
                        CreatedOn = obj.CreatedOn,
                        IsDefaultCreated = true,
                    };
                    _db.Department.Add(department);
                    await _db.SaveChangesAsync();
                    Designation designation = new Designation
                    {
                        CompanyId = obj.CompanyId,
                        DesignationName = "President",
                        Description = "Default Created President Designation",
                        DepartmentId = department.DepartmentId,
                        CreatedOn = obj.CreatedOn,
                        IsDefaultCreated = true,
                    };
                    _db.Designation.Add(designation);
                    await _db.SaveChangesAsync();

                    #endregion Add Department and Designation

                    #region Add Admin of Company

                    var Password = DataHelper.RandomStringGenerate(16);
                    var hashKey = DataHelper.GeneratePasswords(10);
                    var encPassword = DataHelper.EncodePassword(Password, hashKey);
                    byte Levels = 4;

                    var user = new ApplicationUser()
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        PhoneNumber = model.PhoneNumber,
                        Level = Levels,
                        JoinDate = obj.CreatedOn,
                        EmailConfirmed = true,
                        Email = model.OfficeEmail,
                        PasswordHash = hashKey,
                        UserName = model.OfficeEmail
                    };
                    IdentityResult result = await this.AppUserManager.CreateAsync(user, Password);
                    if (result.Succeeded)
                    {
                        Employee empObj = new Employee
                        {
                            FirstName = model.FirstName.Trim(),
                            MiddleName = String.IsNullOrEmpty(model.MiddleName) ? String.Empty : model.MiddleName.Trim(),
                            LastName = model.LastName.Trim(),
                            DisplayName = Regex.Replace(firstName + " " + middleName + " " + lastName, @"\s+", " ").Trim(),
                            DateOfBirth = model.DateOfBirth,
                            ConfirmationDate = obj.CreatedOn,
                            JoiningDate = obj.CreatedOn,
                            CreatedOn = obj.CreatedOn,
                            Gender = model.Gender,
                            DepartmentId = department.DepartmentId,
                            DesignationId = designation.DesignationId,
                            EmployeeTypeId = EmployeeTypeConstants.Confirmed_Employee,
                            EmergencyNumber = "",
                            Password = Password,
                            OfficeEmail = model.OfficeEmail,
                            MobilePhone = model.UserPhoneNumber,
                            CompanyId = obj.CompanyId,
                            PersonalEmail = model.PersonalEmail,
                            IsPresident = true,
                        };
                        _db.Employee.Add(empObj);
                        await _db.SaveChangesAsync();

                        User userObj = new User
                        {
                            EmployeeId = empObj.EmployeeId,
                            UserName = empObj.OfficeEmail,
                            Password = encPassword,
                            HashCode = hashKey,
                            DepartmentId = department.DepartmentId,
                            LoginId = LoginRolesConstants.EmossyUser,
                            CreatedOn = obj.CreatedOn,
                            CompanyId = obj.CompanyId,
                        };

                        _db.User.Add(userObj);
                        await _db.SaveChangesAsync();

                        res.Message = "Company Created";
                        res.Status = true;
                        res.Data = Password;

                        await SendResteMailForAdmin(empObj.OfficeEmail, model.FullUrl, tokenData.IsSmtpProvided, tokenData.companyId);
                        HostingEnvironment.QueueBackgroundWorkItem(ct => CreateDefault(empObj, obj));
                    }
                    else
                    {
                        _db.Company.Remove(obj);
                        await _db.SaveChangesAsync();
                    }

                    #endregion Add Admin of Company
                }
                else
                {
                    res.Message = "Failed To Create New Company";
                    res.Status = false;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/company/addcompany | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public async Task CreateDefault(Employee admin, Company comp)
        {
            await Task.Delay(2000); await AddDefaultShiftGroup(admin, comp);
            await Task.Delay(2000); await CreateNavigationFirstTime(comp);
            await Task.Delay(2000); await CreateDefaultWeekOffAndShift(admin, comp);
            await Task.Delay(2000); await CreateDefaultGlobalRole(admin, comp);
            await Task.Delay(2000); await CreateDefaultHoidayGroup(admin);
            await Task.Delay(2000); await CreateDefaultWorkType(comp);
            await Task.Delay(2000); await CreateDefaultBlogEntities(comp, admin);
        }
        #endregion Api To Create Company and Company Admin

        #region CREATE DEFAULTS
        public async Task AddDefaultShiftGroup(Employee admin, Company company)
        {
            try
            {
                var group = await _db.ShiftGroups.Where(x => x.ShiftCode == "DS" && !x.IsDeleted &&
                        x.IsActive && x.CompanyId == company.CompanyId).FirstOrDefaultAsync();
                {
                    if (group == null)
                    {
                        ShiftGroup obj = new ShiftGroup
                        {
                            ShiftName = "Default Shift",
                            ShiftCode = "DS",
                            Description = "Default Shift by Admin",
                            IsFlexible = false,
                            IsTimingDifferent = false,
                            IsDurationDifferent = false,
                            IsDefaultShiftGroup = true,
                            IsDefaultCreated = true,
                            CompanyId = company.CompanyId,
                            CreatedOn = company.CreatedOn,
                        };

                        var addshifttiming = Enum.GetValues(typeof(DayOfWeek))
                                .Cast<DayOfWeek>()
                                .Select(x => new ShiftTiming
                                {
                                    ShiftTimingId = Guid.NewGuid(),
                                    ShiftGroup = obj,
                                    WeekDay = x,
                                    WeekName = x.ToString(),
                                    CompanyId = company.CompanyId,
                                    CreatedOn = company.CreatedOn,
                                })
                                .OrderBy(x => x.WeekDay == DayOfWeek.Sunday).ThenBy(x => x.WeekDay)
                                .ToList();

                        _db.ShiftGroups.Add(obj);
                        _db.ShiftTimings.AddRange(addshifttiming);

                        await _db.SaveChangesAsync();

                        admin.ShiftGroupId = obj.ShiftGoupId;
                        company.DefaultShiftId = obj.ShiftGoupId;
                        _db.Entry(admin).State = EntityState.Modified;
                        _db.Entry(company).State = EntityState.Modified;

                        await _db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task CreateNavigationFirstTime(Company company)
        {
            var navigationTop = Enum.GetValues(typeof(NavigationConstants))
                                .Cast<NavigationConstants>()
                                .Select(x => new HomeHeader
                                {
                                    NavigationName = x.ToString().Replace("_", " "),
                                    IsDefaultCreated = true,
                                    CreatedOn = company.CreatedOn,
                                    CompanyId = company.CompanyId,

                                }).ToList();
            _db.HomeHeaders.AddRange(navigationTop);
            await _db.SaveChangesAsync();
        }
        public async Task CreateDefaultWeekOffAndShift(Employee admin, Company company)
        {
            WeekOffDaysGroup obj = new WeekOffDaysGroup
            {
                WeekOffName = "Default Week Offs",
                Description = "Default WeekOffs",
                IsDefaultCreated = true,
                CreatedOn = company.CreatedOn,
                CompanyId = company.CompanyId,
            };

            var weekOffAdd = Enum.GetValues(typeof(DayOfWeek))
                        .Cast<DayOfWeek>()
                        .Where(x => x == DayOfWeek.Sunday || x == DayOfWeek.Saturday)
                        .Select(x => new WeekOffDaysCases
                        {
                            WeekOffCaseId = Guid.NewGuid(),
                            Group = obj,
                            DayId = x,
                            CaseId = WeekOffCase.All_Week,
                            CaseResponseId = WeekOffDayConstants.Full_Day_Weekly_Off,
                            IsDefaultCreated = true,
                            CreatedOn = DateTime.Now,
                            CompanyId = company.CompanyId,

                        }).ToList();
            company.DefaultWeekOff = obj.WeekOffId;
            admin.WeekOffId = obj.WeekOffId;

            _db.WeekOffDays.Add(obj);
            _db.WeekOffDaysCases.AddRange(weekOffAdd);
            _db.Entry(admin).State = EntityState.Modified;
            _db.Entry(company).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
        public async Task CreateDefaultGlobalRole(Employee admin, Company company)
        {
            RoleInUserAccessPermission role = new RoleInUserAccessPermission
            {
                RoleName = "Global Role",
                Description = "This Is Default Admin Role",
                CompanyId = admin.CompanyId,
                IsDefaultCreated = true,
                HeadRoleInCompany = true,
                CreatedOn = admin.CreatedOn,
            };
            var moduleAndSubModule = await _db.ModuleAndSubmodules.Where(x => !x.IsDeleted && !x.IsSuperAdmin).ToListAsync();
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

            EmployeeInRole obj = new EmployeeInRole
            {
                EmployeeId = admin.EmployeeId,
                RoleId = role.RoleId,
                IsDefaultCreated = true,
                CompanyId = admin.CompanyId,
                CreatedOn = admin.CreatedOn,
            };
            _db.EmployeeInRoles.Add(obj);
            _db.RoleInUserAccessPermissions.Add(role);
            _db.PermissionInUserAccesses.AddRange(addData);
            await _db.SaveChangesAsync();
            await CreateDefaultUserRole(admin, company, moduleAndSubModule);
        }
        public async Task CreateDefaultUserRole(Employee admin, Company company, List<ModuleAndSubmodule> module)
        {
            RoleInUserAccessPermission role = new RoleInUserAccessPermission
            {
                RoleName = "Default Role",
                Description = "This Is Default Role For All Users",
                CompanyId = admin.CompanyId,
                IsDefaultCreated = true,
                CreatedOn = admin.CreatedOn,
            };
            company.DefaultRole = role.RoleId;
            var addData = module
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
            _db.Entry(company).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
        public async Task CreateDefaultHoidayGroup(Employee admin)
        {
            HolidayGroup obj = new HolidayGroup
            {
                Title = "Default Holiday Group",
                Description = "This Is Default Holiday Group For All Users",
                CompanyId = admin.CompanyId,
                IsDefaultCreated = true,
                CreatedOn = admin.CreatedOn,
            };
            _db.HolidayGroups.Add(obj);
            await _db.SaveChangesAsync();
        }
        public async Task CreateDefaultWorkType(Company company)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var getstatic = Worktype.workobj();

                foreach (var item in getstatic)
                {
                    var duplication = await _db.TypeofWorks.FirstOrDefaultAsync(x => x.WorktypeName == item.Worktype && x.CompanyId == company.CountryId);
                    if (duplication == null)
                    {
                        TypeofWork obj = new TypeofWork()
                        {
                            WorktypeName = item.Worktype,
                            Description = item.Description,
                            //   WorkTypeCode = "Default field",
                            IsActive = true,
                            IsDeleted = false,
                            CompanyId = company.CompanyId,
                            IsDefaultCreated = true,
                            CreatedBy = 0,
                            CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow),
                        };
                        _db.TypeofWorks.Add(obj);
                        _db.SaveChanges();
                        res.Message = "Default fields added successfully !";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                    }
                    else
                    {
                        res.Message = "failed to add Default fields !";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NoContent;
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
                res.StatusCode = HttpStatusCode.BadRequest;
            }
        }
        public async Task CreateDefaultBlogEntities(Company company, Employee admin)
        {
            string[] array = { "Achievement", "Enviroment", "Travel", "Culture", "People" };
            List<BlogCategories> listCategory = array
                .Select(x => new BlogCategories
                {
                    CategoryName = x,
                    Description = "For " + x,
                    CreatedOn = company.CreatedOn,
                    CreatedBy = admin.EmployeeId,
                    IsDefaultCreated = true,
                })
                .ToList();
            _db.BlogCategories.AddRange(listCategory);
            await _db.SaveChangesAsync();
        }
        #endregion

        #region API TO LOCK THE COMPANY 
        /// <summary>
        /// Created By Harshit Mitra On 16-01-2023
        /// API >> POST >> api/company/lockcompany
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("lockcompany")]
        public async Task<IHttpActionResult> LockCompanyLogin(int companyId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var company = await _db.Company.FirstOrDefaultAsync(x => x.CompanyId == companyId);
                if (company == null)
                {
                    res.Message = "Company Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;

                    return Ok(res);
                }
                company.IsCompanyIsLock = !company.IsCompanyIsLock;
                _db.Entry(company).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "Company Is " + (company.IsCompanyIsLock ? "Locked" : "Unlocked");
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerybreakdown/addpaygroup | " +
                    "CompanyId : " + companyId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region Api To Update Company
        /// <summary>
        /// Modify By Harshit Mitra on 25-04-2022
        /// API >> Post >> api/company/editcompany
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("editcompany")]
        public async Task<ResponseBodyModel> EditCompany(CompanyHelperClass model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var company = await _db.Company.FirstOrDefaultAsync(x => x.CompanyId == model.CompanyId);
                if (company != null)
                {
                    company.RegisterCompanyName = model.RegisterCompanyName;
                    company.CompanyGst = model.CompanyGst;
                    company.CIN = model.CIN;
                    company.RegisterAddress = model.RegisterAddress;
                    company.RegisterEmail = model.RegisterEmail;
                    company.PhoneNumber = model.PhoneNumber;
                    company.IncorporationDate = model.IncorporationDate;
                    company.IncorporationCertificate = model.IncorporationCertificate;
                    company.CountryId = model.CountryId;
                    company.AppAdminLogo = model.AppAdminLogo;
                    company.NavigationLogo = model.NavigationLogo;
                    company.CompanyDomain = model.CompanyDomain;
                    company.CompanyDefaultTimeZone = model.CompanyDefaultTimeZone;
                    company.CompanyWebSiteURL = model.CompanyWebSiteURL;
                    company.IsSmtpProvided = model.IsSmtpProvided;
                    company.ImgKitApiKey = model.ImgKitApiKey;
                    _db.Entry(company).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    var smtpMail = await _db.CompanySmtpMailModels.FirstOrDefaultAsync(x => x.CompanyId == company.CompanyId);
                    if (smtpMail != null)
                    {
                        smtpMail.From = model.MailUser;
                        smtpMail.MailUser = model.MailUser;
                        smtpMail.Password = model.Password;
                        smtpMail.Port = model.Port;
                        smtpMail.SmtpServer = model.SmtpServer;
                        smtpMail.ConnectType = SmtpConnectType.ConnectSSLAuto;
                        smtpMail.CompanyId = company.CompanyId;
                        _db.Entry(smtpMail).State = EntityState.Modified;
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        CompanySmtpMailModel demo = new CompanySmtpMailModel
                        {
                            From = model.MailUser,
                            MailUser = model.MailUser,
                            Password = model.Password,
                            Port = model.Port,
                            SmtpServer = model.SmtpServer,
                            ConnectType = SmtpConnectType.ConnectSSLAuto,
                            CompanyId = company.CompanyId,
                        };

                        _db.CompanySmtpMailModels.Add(demo);
                        await _db.SaveChangesAsync();
                    }
                    res.Message = "Company Edited";
                    res.Status = true;
                    res.Data = company;
                }
                else
                {
                    res.Message = "No Company Found!!";
                    res.Status = false;
                    res.Data = null;
                }

            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #region This Use For update Company
        public class CompanyHelperClass
        {
            public int CompanyId { get; set; }
            public string RegisterCompanyName { get; set; }
            public string CompanyGst { get; set; }
            public string CIN { get; set; }
            public string RegisterAddress { get; set; }
            public string RegisterEmail { get; set; }
            public string PhoneNumber { get; set; }
            public DateTime IncorporationDate { get; set; }
            public string IncorporationCertificate { get; set; }
            public string CompanyDomain { get; set; }
            public string AppAdminLogo { get; set; } = String.Empty;
            public string NavigationLogo { get; set; } = String.Empty;
            public int CountryId { get; set; } = 0;
            public string CompanyDefaultTimeZone { get; set; } = "India Standard Time";
            public string CompanyWebSiteURL { get; set; } = String.Empty;
            public string MailUser { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public int Port { get; set; } = 0;
            public string SmtpServer { get; set; } = string.Empty;
            public bool IsSmtpProvided { get; set; } = false;
            public string ImgKitApiKey { get; set; } = String.Empty;
        }
        #endregion Api To Update Company

        #endregion

        #region Api To App Admin Logo

        /// <summary>
        /// Created By Suraj Bundel On 30-05-2022
        /// API >> Post >> api/company/uploadadminlogo
        /// </summary>
        /// use to post Admin logo  on create company
        /// <returns></returns>
        [HttpPost]
        [Route("uploadadminlogo")]
        [Authorize]
        public async Task<UploadImageResponse> Uploadadminlogo()
        {
            UploadImageResponse result = new UploadImageResponse();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var dates = DateTime.Now.ToString("yyyyMMddhhmmsstt");
                var data = Request.Content.IsMimeMultipartContent();
                if (Request.Content.IsMimeMultipartContent())
                {
                    //fileList f = new fileList();
                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);
                    if (provider.Contents.Count > 0)
                    {
                        var filefromreq = provider.Contents[0];
                        Stream _id = filefromreq.ReadAsStreamAsync().Result;
                        StreamReader reader = new StreamReader(_id);
                        string filename = filefromreq.Headers.ContentDisposition.FileName.Trim('\"');

                        string extemtionType = MimeType.GetContentType(filename).Split('/').First();
                        //if (extemtionType == "image" || extemtionType=="Document"||extemtionType== "application")
                        if (extemtionType == "image")
                        {
                            string extension = Path.GetExtension(filename);
                            string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                            byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                            //f.byteArray = buffer;
                            string mime = filefromreq.Headers.ContentType.ToString();
                            Stream stream = new MemoryStream(buffer);
                            var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/companyimage/Adminlogo/" + claims.employeeId), dates + '.' + filename);
                            string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                            //for create new Folder
                            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                            if (!objDirectory.Exists)
                            {
                                Directory.CreateDirectory(DirectoryURL);
                            }
                            //string path = "UploadImages\\" + compid + "\\" + filename;

                            string path = "uploadimage\\companyimage\\Adminlogo\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

                            File.WriteAllBytes(FileUrl, buffer.ToArray());
                            result.Message = "Successful";
                            result.Status = true;
                            result.URL = FileUrl;
                            result.Path = path;
                            result.Extension = extension;
                            result.ExtensionType = extemtionType;
                        }
                        else
                        {
                            result.Message = "Only Select Image Format";
                            result.Status = false;
                        }
                    }
                    else
                    {
                        result.Message = "No content Passed ";
                        result.Status = false;
                    }
                }
                else
                {
                    result.Message = "Error";
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = false;
            }
            return result;
        }

        #endregion Api To App Admin Logo

        #region Api To Upload Navigation Logo

        /// <summary>
        /// Created By Suraj Bundel On 30-05-2022
        /// API >> Post >> api/company/uploadnavigatelogo
        /// </summary>
        /// use to post Navigation logo on create company
        /// <returns></returns>
        [HttpPost]
        [Route("uploadnavigatelogo")]
        [Authorize]
        public async Task<UploadImageResponse> Uploadnavigationlogo()
        {
            UploadImageResponse result = new UploadImageResponse();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var dates = DateTime.Now.ToString("yyyyMMddhhmmsstt");
                var data = Request.Content.IsMimeMultipartContent();
                if (Request.Content.IsMimeMultipartContent())
                {
                    //fileList f = new fileList();
                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);
                    if (provider.Contents.Count > 0)
                    {
                        var filefromreq = provider.Contents[0];
                        Stream _id = filefromreq.ReadAsStreamAsync().Result;
                        StreamReader reader = new StreamReader(_id);
                        string filename = filefromreq.Headers.ContentDisposition.FileName.Trim('\"');

                        string extemtionType = MimeType.GetContentType(filename).Split('/').First();
                        //if (extemtionType == "image" || extemtionType=="Document"||extemtionType== "application")
                        if (extemtionType == "image")
                        {
                            string extension = Path.GetExtension(filename);
                            string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                            byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                            //f.byteArray = buffer;
                            string mime = filefromreq.Headers.ContentType.ToString();
                            Stream stream = new MemoryStream(buffer);
                            var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/companyimage/Navigationlogo/" + claims.employeeId), dates + '.' + filename);
                            string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                            //for create new Folder
                            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                            if (!objDirectory.Exists)
                            {
                                Directory.CreateDirectory(DirectoryURL);
                            }
                            //string path = "UploadImages\\" + compid + "\\" + filename;

                            string path = "uploadimage\\companyimage\\Navigationlogo\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

                            File.WriteAllBytes(FileUrl, buffer.ToArray());
                            result.Message = "Successful";
                            result.Status = true;
                            result.URL = FileUrl;
                            result.Path = path;
                            result.Extension = extension;
                            result.ExtensionType = extemtionType;
                        }
                        else
                        {
                            result.Message = "Only Select Image Format";
                            result.Status = false;
                        }
                    }
                    else
                    {
                        result.Message = "No content Passed ";
                        result.Status = false;
                    }
                }
                else
                {
                    result.Message = "Error";
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = false;
            }
            return result;
        }

        #endregion Api To Upload Navigation Logo

        #region Helper Model Class
        public class AddCompanyClassRequest
        {
            /// ----- Company Info ------ /// 
            public string RegisterCompanyName { get; set; }
            public string CompanyGst { get; set; }
            public string CIN { get; set; }
            public string RegisterAddress { get; set; }
            public string RegisterEmail { get; set; }
            public string PhoneNumber { get; set; }
            public DateTime IncorporationDate { get; set; }
            public string IncorporationCertificate { get; set; }
            public string adminlogo { get; set; }
            public string Navigationlogo { get; set; }
            public string CompanyDomain { get; set; }

            /// ----- Company Admin ------- ///
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string OfficeEmail { get; set; }
            public string UserPhoneNumber { get; set; }
            public string PersonalEmail { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string Gender { get; set; }
            public string FullUrl { get; set; }
            public int CountryId { get; set; } = 0;
            public string CompanyDefaultTimeZone { get; set; } = "India Standard Time";
            public string CompanyWebSiteURL { get; set; } = String.Empty;
            public string From { get; set; } = string.Empty;
            public string MailUser { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public int Port { get; set; } = 0;
            public string SmtpServer { get; set; } = string.Empty;
            public SmtpConnectType ConnectType { get; set; } = SmtpConnectType.ConnectSSLAuto;
            public bool IsSmtpProvided { get; set; } = false;
            public string ImgKitApiKey { get; set; } = String.Empty;
        }
        #endregion Helper Model Class

        #region This Api Use To Send ForgetMaill
        ///// <summary>
        ///// Create By ankit Date-14-09-2022
        ///// </summary>
        ///// <param name="CandidateId"></param>
        ///// <returns></returns>
        public async Task SendResteMailForAdmin(string resetemail, string baseurl, bool IsSmtpProvided, int companyId)
        {
            var employee = _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.OfficeEmail == resetemail).FirstOrDefault();
            try
            {
                var key = ConfigurationManager.AppSettings["EncryptKey"];
                var data = "UserEmail=" + resetemail;
                string token = EncryptDecrypt.EncryptData(key, data);
                SmtpSendMailRequest smtpsettings = new SmtpSendMailRequest();
                if (IsSmtpProvided)
                {
                    smtpsettings = _db.CompanySmtpMailModels
                        .Where(x => x.CompanyId == companyId)
                        .Select(x => new SmtpSendMailRequest
                        {
                            From = x.From,
                            SmtpServer = x.SmtpServer,
                            MailUser = x.MailUser,
                            MailPassword = x.Password,
                            Port = x.Port,
                            ConectionType = x.ConnectType,
                        })
                        .FirstOrDefault();
                }
                string path = baseurl + "token=" + token;
                string fcode = "<body style=' display: flex;align-items: center;justify-content: center;height:100vh;'>";
                fcode += "<div class='  flextcontainer card p-2' style='text-align: center;width: 83%; min-height: 50px;position: relative;margin-bottom: 24px;border: 1px solid #f2f4f9;border-radius: 10px;box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);-webkit-box-shadow: 0 0 10px 0 rgb(183 192 206 / 20%);'>";
                fcode += "<img class='imgg mb-2' style='  width: 40%;margin: auto;display: block;' src='" + baseurl + "'+'/assets/logo-moreyeahs.png'>";
                fcode += "<hr>";
                fcode += "<h1 class='mt-2 mb-2' style='margin-top: 10px;margin-bottom:10px;'>Reset Your Password ?</h1>";
                fcode += "<div class='m-2 mb-3'>";
                fcode += "<label style='margin-top: 10px;margin-bottom:20px;' >You are receiving this email because you are requested for password create for your company. Click on the link below to create new password.</label>";
                fcode += "<br><br>";
                fcode += "</div>";
                fcode += "<div>";
                fcode += "<a  style='margin-top:20px;margin-botton:20px;background: #911924;border-color: #911924;padding: 10px;border-radius: 5px;text-decoration: none;color: #fff;text-transform: uppercase;' href='" + path + "'>Reset My Password</a>";
                fcode += "<br><br>";
                fcode += "</div>";
                fcode += "<div class='m-2 mb-3'>";
                fcode += "<label  style='margin-top: 20px;margin-bottom:10px;' ><strong>Please do not share this link with anyone.</strong></label>";
                fcode += "</div>";
                fcode += "</div>";
                fcode += "</body>";
                SendMailModelRequest sendMailObject = new SendMailModelRequest()
                {
                    IsCompanyHaveDefaultMail = IsSmtpProvided,
                    Subject = "Create Your Password",
                    MailBody = fcode,
                    MailTo = new List<string>() { resetemail },
                    SmtpSettings = smtpsettings,
                };
                await SmtpMailHelper.SendMailAsync(sendMailObject);
            }
            catch (Exception ep)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
        }
        #endregion

        #region API TO GET TIME ZONE LIST
        /// <summary>
        /// Created By Harshit Mitra On 16-01-2023
        /// API >> GET >> api/company/gettimezonelist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("gettimezonelist")]
        public IHttpActionResult GetTimeZoneList()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                ReadOnlyCollection<TimeZoneInfo> tz = TimeZoneInfo.GetSystemTimeZones();
                var response = tz
                    .Select(x => new
                    {
                        x.Id,
                        x.DisplayName,
                    })
                    .ToList();
                res.Message = "Time Zone List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = response;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/company/gettimezonelist | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

    }

}

