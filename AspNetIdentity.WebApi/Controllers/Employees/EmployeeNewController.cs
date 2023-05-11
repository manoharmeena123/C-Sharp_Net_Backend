using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.EmployeeModel;
using AspNetIdentity.WebApi.Model.FaultyImportLog;
using AspNetIdentity.WebApi.Model.UserAccesPermission;
using AspNetIdentity.WebApi.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.Employees
{
    /// <summary>
    /// Created By Harshit Mitra On 25-04-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/employeenew")]
    public class EmployeeNewController : BaseApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Api To Add Employee

        /// <summary>
        /// Created By Harshit Mitra on 05-04-2022
        /// API >> Post >> api/employeenew/addemployee
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addemployee")]
        public async Task<ResponseBodyModel> AddEmployee(AddEmployeeModelHelper model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model == null)
                {
                    res.Message = "Model Is Invalid";
                    res.Status = false;
                }
                else
                {
                    //if (claims.roleType == "Administrator")
                    //{
                    //    if (model.LoginType != LoginRolesConstants.Administrator)
                    //    {
                    //        if (model.OrgId == 0)
                    //        {
                    //            res.Message = "Your Have To Select Organization";
                    //            res.Status = false;
                    //            return res;
                    //        }
                    //    }
                    //}
                    var WeekOff = _db.WeekOffDays.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId
                    && x.WeekOffName == "Default Week Offs").FirstOrDefault();
                    var shiftdata = _db.ShiftGroups.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId
                    && x.ShiftName == "Default Shift").FirstOrDefault();
                    var employeeMgr = _db.Employee.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId).ToList();
                    var checkEmp = _db.Employee.Select(x => x.OfficeEmail.Trim().ToUpper())
                                .Contains(model.OfficeEmail.Trim().ToUpper());

                    if (checkEmp)
                    {
                        res.Message = "This Employee Is Allready Exist";
                        res.Status = false;
                    }
                    else
                    {
                        var roledata = _db.RoleInUserAccessPermissions.Where(x => x.IsActive && !x.IsDeleted &&
                           x.CompanyId == claims.companyId && x.RoleName == "Default Role").FirstOrDefault();
                        var firstName = model.FirstName.Trim();
                        var middleName = String.IsNullOrEmpty(model.MiddleName) ? "" : model.MiddleName;
                        var lastName = model.LastName.Trim();

                        var Password = model.Password.Trim();
                        var hashKey = DataHelper.GeneratePasswords(10);
                        var encPassword = DataHelper.EncodePassword(Password, hashKey);
                        byte Levels = 4;

                        var split = model.OfficeEmail.Split('@');
                        model.OfficeEmail = split[0] + "@" + split[1].ToLower();

                        var user = new ApplicationUser()
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            PhoneNumber = model.MobilePhone,
                            Level = Levels,
                            JoinDate = DateTime.Now,
                            EmailConfirmed = true,
                            Email = model.OfficeEmail,
                            PasswordHash = hashKey,
                            UserName = model.OfficeEmail,
                            CompanyId = claims.companyId,
                        };
                        IdentityResult result = await this.AppUserManager.CreateAsync(user, Password);
                        if (result.Succeeded)
                        {
                            Employee empObj = new Employee();
                            empObj.FirstName = firstName;
                            empObj.MiddleName = middleName;
                            empObj.LastName = lastName;
                            empObj.DisplayName = String.IsNullOrEmpty(middleName) ? firstName + " " + lastName :
                                firstName + " " + middleName + " " + lastName;
                            empObj.FatherName = model.FatherName;
                            empObj.MotherName = model.MotherName;
                            empObj.Gender = model.Gender;
                            empObj.DateOfBirth = model.DateOfBirth;
                            empObj.BloodGroup = model.BloodGroup;
                            empObj.MaritalStatus = model.MaritalStatus;
                            empObj.MedicalIssue = model.MedicalIssue;
                            empObj.AadharNumber = model.AadharNumber;
                            empObj.PanNumber = model.PanNumber;
                            empObj.MobilePhone = model.MobilePhone;
                            empObj.EmergencyNumber = model.EmergencyNumber;
                            //empObj.LoginType = model.LoginType;
                            empObj.WhatsappNumber = model.WhatsappNumber;
                            empObj.PersonalEmail = model.PersonalEmail;
                            empObj.PermanentAddress = model.PermanentAddress;
                            empObj.LocalAddress = model.LocalAddress;
                            empObj.EmployeeTypeId = model.EmployeeTypeId;
                            empObj.DepartmentId = model.DepartmentId;
                            empObj.DesignationId = model.DesignationId;
                            empObj.JoiningDate = model.JoiningDate;
                            empObj.ConfirmationDate = model.ConfirmationDate;
                            empObj.BiometricId = model.BiometricId;
                            empObj.BankAccountNumber = model.BankAccountNumber;
                            empObj.IFSC = model.IFSC;
                            empObj.AccountHolderName = model.AccountHolderName;
                            empObj.BankName = model.BankName;
                            empObj.OfficeEmail = model.OfficeEmail;
                            empObj.Password = Password;
                            empObj.GrossSalery = model.GrossSalery; //shriya
                            empObj.CreatedBy = claims.employeeId;
                            empObj.CreatedOn = DateTime.Now;
                            empObj.IsActive = true;
                            empObj.IsDeleted = false;
                            empObj.RoleId = 0;
                            empObj.CompanyId = claims.companyId;
                            empObj.OrgId = claims.roleType == "Administrator" ? model.OrgId : claims.orgId;
                            empObj.EmployeeCode = model.EmployeeCode;

                            empObj.ShiftGroupId = model.ShiftGroupId == Guid.Empty ? shiftdata.ShiftGoupId : model.ShiftGroupId;
                            empObj.WeekOffId = model.WeekOffId == Guid.Empty ? WeekOff.WeekOffId
                                : model.WeekOffId;
                            // // ---Reporting manager --//
                            empObj.ReportingManager = model.ReportingManagerId;
                            _db.Employee.Add(empObj);
                            await _db.SaveChangesAsync();

                            User userObj = new User();
                            userObj.EmployeeId = empObj.EmployeeId;
                            userObj.UserName = empObj.OfficeEmail;
                            userObj.Password = encPassword;
                            userObj.HashCode = hashKey;
                            userObj.DepartmentId = model.DepartmentId;
                            // userObj.LoginId = model.LoginType;
                            userObj.CreatedOn = DateTime.Now;
                            userObj.IsActive = true;
                            userObj.IsDeleted = false;
                            userObj.CompanyId = claims.companyId;
                            userObj.OrgId = empObj.OrgId;

                            _db.User.Add(userObj);
                            await _db.SaveChangesAsync();

                            var companyName = _db.Company.Where(x => x.CompanyId == claims.companyId).Select(x => x.RegisterCompanyName).FirstOrDefault();
                            var profilUrl = _db.Employee.Where(x => x.EmployeeId == empObj.EmployeeId && x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).Select(x => x.ProfileImageUrl).FirstOrDefault();

                            var client = new RestSharp.RestClient("http://localhost:5001/api/Accounts/register");
                            client.Timeout = -1;
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Content-Type", "application/json");
                            LMS body = new LMS();
                            body.EmployeeId = empObj.EmployeeId;
                            body.FirstName = model.FirstName;
                            body.LastName = model.LastName;
                            body.MobileNumber = model.MobilePhone;
                            body.Username = model.OfficeEmail;
                            body.MobileNumber = model.MobilePhone;
                            body.Password = model.Password;
                            body.CompanyId = claims.companyId;
                            body.OrgId = empObj.OrgId;
                            body.Picture = profilUrl;
                            body.CountryCode = "+91";
                            body.Tenant = 3;
                            body.OrganisationName = companyName;
                            body.Role = userObj.LoginId == LoginRolesConstants.HR ? 0 : 1;

                            string jsonData = JsonConvert.SerializeObject(body);
                            request.AddParameter("application/json", jsonData, ParameterType.RequestBody);
                            IRestResponse response = client.Execute(request);
                            Console.WriteLine(response.Content);

                            EmployeeInRole obj = new EmployeeInRole();
                            obj.RoleId = roledata.RoleId;
                            obj.EmployeeId = empObj.EmployeeId;
                            obj.CompanyId = empObj.CompanyId;
                            obj.OrgId = empObj.OrgId;
                            obj.CreatedOn = DateTime.Now;
                            obj.IsActive = true;
                            obj.IsDeleted = false;
                            _db.EmployeeInRoles.Add(obj);
                            await _db.SaveChangesAsync();

                            #region This Code Use In Future 


                            //var client = new RestSharp.RestClient("http://localhost:5001/api/Accounts/register");
                            //client.Timeout = -1;
                            //var request = new RestRequest(Method.POST);
                            //request.AddHeader("Content-Type", "application/json");
                            //LMS obj = new LMS();
                            //var body =
                            //@"{
                            //        " + "\n" +
                            //    @"  ""firstName"": ""  "" ,
                            //        " + "\n" +
                            //    @"  ""lastName"": ""Singh ji ko bhi kiya"",
                            //        " + "\n" +
                            //    @"  ""username"": ""ddd123@gmail.com"",
                            //        " + "\n" +
                            //    @"  ""password"": ""123456789"",
                            //        " + "\n" +
                            //    @"  ""countryCode"": ""+91"",
                            //        " + "\n" +
                            //    @"  ""mobileNumber"": ""7974698160"",
                            //        " + "\n" +
                            //    @"  ""tenant"": 3,
                            //        " + "\n" +
                            //    @"  ""picture"": ""string"",
                            //        " + "\n" +
                            //    @"  ""organisationName"": ""MoreYeahs"",
                            //        " + "\n" +
                            //    @"  ""companyId"": 1,
                            //        " + "\n" +
                            //    @"  ""orgId"": 1
                            //        " + "\n" +
                            //@"}";

                            //    request.AddParameter("application/json", body, ParameterType.RequestBody);
                            //    IRestResponse response = client.Execute(request);
                            //    Console.WriteLine(response.Content);
                            //RestClient client = new RestClient("http://localhost:5001/api/Accounts/register");
                            //RestRequest request = new RestRequest("Registration", Method.POST, DataFormat.Json);
                            //request.AddHeader("Content-Type", "application/json");
                            ////var enc = GetEncValue(LMS);                                                   
                            //request.AddParameter("myAssocKey", JsonConvert.SerializeObject(listOfObjects));
                            //var response = client.Execute(request).Content;


                            //string fullURL = "http://localhost:5001/api/Accounts/register";
                            //var client = new RestSharp.RestClient(fullURL);
                            //client.Timeout = -1;
                            //var request = new RestRequest(Method.POST);

                            //IRestResponse response = client.Execute(request);
                            //Console.WriteLine(response.Content);
                            ////CallLogDTO CallLogDetails = JsonConvert.DeserializeObject<CallLogDTO>(response.Content);
                            //Console.WriteLine(CallLogDetails);
                            //response.Content;


                            #endregion

                            res.Message = "Employee Add";
                            res.Status = true;
                            res.Data = empObj;
                        }
                        else
                        {
                            res.Message = "Offical Email Already In Use";
                            res.Status = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Add Employee

        #region Api To Get Employ On Add

        /// <summary>
        /// Created By Harshit Mitra On 04-05-2022
        /// Modified  By Suraj Bundel On 14-10-2022
        /// Modified  By Suraj Bundel On 1-11-2022
        /// API >> Get >> api/employeenew/getemployeeadd
        /// Modified by Shriya Malvi On 15-06-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployeeadd")]
        public async Task<ResponseBodyModel> GetEmployeeProfileOnAdd(int employeeId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employeedata = await _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId && x.EmployeeId == employeeId).ToListAsync();
                var employee = employeedata.FirstOrDefault(x => x.EmployeeId == employeeId);
                if (employee != null)
                {
                    GetEmployeeAfterAdd obj = new GetEmployeeAfterAdd();

                    obj.EmployeeId = employee.EmployeeId;
                    obj.FirstName = employee.FirstName;
                    obj.MiddleName = employee.MiddleName;
                    obj.LastName = employee.LastName;
                    obj.FatherName = employee.FatherName;
                    obj.MotherName = employee.MotherName;
                    obj.Gender = employee.Gender;
                    obj.DateOfBirth = employee.DateOfBirth;
                    //obj.HideDOB = employee.HideDOB;
                    obj.BloodGroup = employee.BloodGroup;
                    obj.MaritalStatus = employee.MaritalStatus;
                    obj.MedicalIssue = employee.MedicalIssue;
                    obj.AadharNumber = employee.AadharNumber;
                    obj.PanNumber = employee.PanNumber;
                    obj.MobilePhone = employee.MobilePhone;
                    obj.EmergencyNumber = employee.EmergencyNumber;
                    obj.WhatsappNumber = employee.WhatsappNumber;
                    obj.PersonalEmail = employee.PersonalEmail;
                    obj.PermanentAddress = employee.PermanentAddress;
                    obj.LocalAddress = employee.LocalAddress;
                    obj.LoginType = _db.User.Where(x => x.EmployeeId == employee.EmployeeId).Select(x => x.LoginId).FirstOrDefault();
                    obj.EmployeeTypeId = employee.EmployeeTypeId;
                    obj.DepartmentId = employee.DepartmentId;
                    obj.DesignationId = employee.DesignationId;
                    obj.BiometricId = employee.BiometricId;
                    obj.JoiningDate = employee.JoiningDate;
                    obj.BankAccountNumber = employee.BankAccountNumber;
                    obj.IFSC = employee.IFSC;
                    obj.AccountHolderName = employee.AccountHolderName;
                    obj.OfficeEmail = employee.OfficeEmail;
                    obj.Password = employee.Password;
                    obj.BankName = employee.BankName;
                    obj.ConfirmationDate = employee.ConfirmationDate;
                    obj.Salary = employee.GrossSalery; //shriya
                    obj.EmployeeCode = employee.EmployeeCode;
                    obj.OrgId = employee.OrgId;
                    obj.ReportingManagerId = employee.ReportingManager;

                    obj.ReportingManagerName = employeedata.Where(x => x.EmployeeId == x.ReportingManager).Select(x => x.DisplayName).ToList();
                    obj.ShiftGroupId = employee.ShiftGroupId;
                    obj.WeekOffId = employee.WeekOffId;
                    res.Message = "Employee Data";
                    res.Status = true;
                    res.Data = obj;
                }
                else
                {
                    res.Message = "Employee Not Found";
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

        #endregion Api To Get Employ On Add

        #region Api To Add Employee By Excel
        /// <summary>
        /// Created By Harshit Mitra on 26-04-2022
        /// API >> api/employeenew/addemployeebyimport
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addemployeebyimport")]
        public async Task<ResponseBodyModel> AddEmployeeExcel(List<EmployeeImportFaultyLog> item)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<EmployeeImportFaultyLog> faultyList = new List<EmployeeImportFaultyLog>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                long successfullImported = 0;
                if (item == null)
                {
                    res.Message = "Error";
                    res.Status = false;
                    return res;
                }
                if (item.Count == 0)
                {
                    res.Message = "0 Employee Add";
                    res.Status = false;
                }
                var roledata = await _db.RoleInUserAccessPermissions
                    .FirstOrDefaultAsync(x => x.CompanyId == claims.companyId && x.IsDefaultCreated && !x.HeadRoleInCompany);
                var OrgList = await _db.OrgMaster.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted).ToListAsync();
                var departmentList = await _db.Department.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted).ToListAsync();
                var desingationList = await _db.Designation.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted).ToListAsync();
                var shiftGroup = await _db.ShiftGroups.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted).ToListAsync();
                var weekOff = await _db.WeekOffDays.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted).ToListAsync();
                var employeeList = await _db.Employee.Where(x => x.CompanyId == claims.companyId).ToListAsync();
                foreach (var model in item)
                {
                    if (employeeList.Any(x => x.OfficeEmail.ToUpper().Trim() == model.OfficeEmail.ToUpper().Trim()))
                    {
                        res.Message = "This Offical Email Already Exist ";
                        res.Status = true;
                        faultyList.Add(model);
                    }
                    else
                    {
                        var firstName = model.FirstName.Trim();
                        var middleName = String.IsNullOrEmpty(model.MiddleName) ? "" : model.MiddleName;
                        var lastName = model.LastName.Trim();

                        var Password = model.Password.Trim();
                        var hashKey = DataHelper.GeneratePasswords(10);
                        var encPassword = DataHelper.EncodePassword(Password, hashKey);
                        byte Levels = 4;

                        var split = model.OfficeEmail.Split('@');
                        model.OfficeEmail = split[0] + "@" + split[1].ToLower();

                        var departmentId = departmentList
                            .Where(x => x.DepartmentName.Trim() == model.DepartmentName.Trim())
                            .Select(x => x.DepartmentId)
                            .FirstOrDefault();
                        var designationId = desingationList
                            .Where(x => x.DepartmentId == departmentId && x.DesignationName.Trim() == model.DesignationName.Trim())
                            .Select(x => x.DesignationId)
                            .FirstOrDefault();
                        //if (claims.orgId == 0)
                        //    if (!String.IsNullOrEmpty(model.OrganizationName) && !String.IsNullOrWhiteSpace(model.OrganizationName))
                        //        claims.orgId = OrgList.Where(x => x.OrgName == model.OrganizationName).Select(x => x.OrgId).FirstOrDefault();
                        if (/*claims.orgId != 0 &&*/ designationId != 0 && departmentId != 0)
                        {
                            var user = new ApplicationUser()
                            {
                                FirstName = firstName,
                                LastName = lastName,
                                PhoneNumber = model.MobilePhone,
                                Level = Levels,
                                JoinDate = DateTime.Now,
                                EmailConfirmed = true,
                                Email = model.OfficeEmail,
                                PasswordHash = hashKey,
                                UserName = model.OfficeEmail,
                                CompanyId = claims.companyId,
                            };
                            IdentityResult result = await this.AppUserManager.CreateAsync(user, Password);
                            try
                            {
                                if (result.Succeeded)
                                {
                                    Employee empObj = new Employee();
                                    empObj.FirstName = firstName;
                                    empObj.MiddleName = middleName;
                                    empObj.LastName = lastName;
                                    empObj.DisplayName = String.IsNullOrEmpty(middleName) ? firstName + " " + lastName :
                                        firstName + " " + middleName + " " + lastName;
                                    empObj.FatherName = model.FatherName;
                                    empObj.MotherName = model.MotherName;
                                    empObj.Gender = model.Gender;
                                    empObj.DateOfBirth = model.DateOfBirth.Date.AddDays(1);
                                    empObj.BloodGroup = (BloodGroupConstants)System.Enum.Parse(typeof(BloodGroupConstants),
                                        (model.BloodGroup.Contains("+") ? model.BloodGroup.Replace("+", "_pos") : model.BloodGroup.Replace("-", "_neg")));
                                    empObj.MaritalStatus = model.MaritalStatus;
                                    empObj.ConfirmationDate = model.ConfirmationDate.Date.AddDays(1);
                                    empObj.JoiningDate = model.JoiningDate.Date.AddDays(1);
                                    empObj.CreatedBy = claims.employeeId;
                                    empObj.CreatedOn = DateTime.Now;
                                    empObj.IsActive = true;
                                    empObj.IsDeleted = false;
                                    empObj.DepartmentId = departmentId;
                                    empObj.DesignationId = designationId;
                                    empObj.AadharNumber = model.AadharNumber;
                                    empObj.RoleId = 0;
                                    empObj.EmployeeTypeId = (EmployeeTypeConstants)(model.EmployeeType == null ?
                                            EmployeeTypeConstants.Confirmed_Employee :
                                            Enum.Parse(typeof(EmployeeTypeConstants), model.EmployeeType.Replace(" ", "_")));
                                    empObj.EmergencyNumber = model.EmergencyNumber;
                                    empObj.WhatsappNumber = model.WhatsappNumber;
                                    empObj.Password = Password;
                                    empObj.OfficeEmail = model.OfficeEmail;
                                    empObj.MobilePhone = model.MobilePhone;
                                    empObj.CompanyId = claims.companyId;
                                    empObj.OrgId = OrgList.Where(x => x.OrgName == model.OrganizationName).Select(x => x.OrgId).FirstOrDefault();
                                    empObj.PanNumber = model.PanNumber;
                                    empObj.PersonalEmail = model.PersonalEmail;
                                    empObj.PermanentAddress = model.PermanentAddress;
                                    empObj.LocalAddress = model.LocalAddress;
                                    empObj.BankAccountNumber = model.BankAccountNumber;
                                    empObj.IFSC = model.IFSC;
                                    empObj.AccountHolderName = model.AccountHolderName;
                                    empObj.BankName = model.BankName;
                                    empObj.BiometricId = model.BiometricId;
                                    empObj.GrossSalery = model.Salary; //shriya
                                    empObj.EmployeeCode = model.EmployeeCode;
                                    var shiftGroupId = shiftGroup.Where(x => x.ShiftName == model.ShiftGroup).Select(x => x.ShiftGoupId).FirstOrDefault();
                                    if (shiftGroupId == Guid.Empty)
                                        shiftGroupId = (Guid)claims.DefaultShiftGroupId;
                                    empObj.ShiftGroupId = shiftGroupId;
                                    var weekOffId = weekOff.Where(x => x.WeekOffName == model.WeekOff).Select(x => x.WeekOffId).FirstOrDefault();
                                    if (weekOffId == Guid.Empty)
                                        weekOffId = (Guid)claims.DefaultWeekOff;
                                    empObj.WeekOffId = weekOffId;

                                    _db.Employee.Add(empObj);
                                    await _db.SaveChangesAsync();

                                    User userObj = new User();
                                    userObj.EmployeeId = empObj.EmployeeId;
                                    userObj.UserName = empObj.OfficeEmail;
                                    userObj.Password = encPassword;
                                    userObj.HashCode = hashKey;
                                    userObj.DepartmentId = departmentId;
                                    //userObj.LoginId = (LoginRolesConstants)System.Enum.Parse(typeof(LoginRolesConstants), model.LoginType);
                                    userObj.CreatedOn = DateTime.Now;
                                    userObj.IsActive = true;
                                    userObj.IsDeleted = false;
                                    userObj.CompanyId = claims.companyId;
                                    userObj.OrgId = empObj.OrgId;

                                    _db.User.Add(userObj);
                                    await _db.SaveChangesAsync();

                                    res.Message = "Employee Add";
                                    res.Status = true;
                                    successfullImported += 1;

                                    EmployeeInRole obj = new EmployeeInRole();
                                    obj.RoleId = roledata.RoleId;
                                    obj.EmployeeId = empObj.EmployeeId;
                                    obj.CompanyId = empObj.CompanyId;
                                    obj.OrgId = empObj.OrgId;
                                    obj.CreatedOn = DateTime.Now;
                                    obj.IsActive = true;
                                    obj.IsDeleted = false;
                                    _db.EmployeeInRoles.Add(obj);
                                    await _db.SaveChangesAsync();

                                    employeeList.Add(empObj);
                                }
                                else
                                {
                                    faultyList.Add(model);

                                    res.Message = "Email Is Already Register In Our Data Base";
                                    res.Status = false;
                                }
                            }
                            catch (Exception)
                            {
                                faultyList.Add(model);
                            }
                        }
                        else
                        {
                            faultyList.Add(model);
                        }
                    }
                }
                if (faultyList.Count > 0)
                {

                    EmployeeImportFaultyLogGroup groupObj = new EmployeeImportFaultyLogGroup
                    {
                        TotalImported = item.Count,
                        SuccessFullImported = successfullImported,
                        UnSuccessFullImported = faultyList.Count,
                        CreatedBy = claims.employeeId,
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                        CompanyId = claims.companyId,
                        OrgId = claims.orgId,
                    };
                    _db.EmployeeFaultyLogGroups.Add(groupObj);
                    await _db.SaveChangesAsync();

                    faultyList.ForEach(x => x.Group = groupObj);
                    _db.EmployeeFaultyLogs.AddRange(faultyList);
                    await _db.SaveChangesAsync();

                    if ((item.Count - faultyList.Count) > 0)
                    {
                        res.Message = "Employee Imported Succesfull Of " +
                        (item.Count - faultyList.Count) + " Fields And " +
                        faultyList.Count + " Feilds Are Not Imported";
                        res.Status = true;
                        res.Data = faultyList;
                    }
                    else
                    {
                        res.Message = "All Fields Are Not Imported";
                        res.Status = true;
                        res.Data = faultyList;
                    }
                }
                else
                {
                    res.Message = "All Employee Import Successfull";
                    res.Status = true;
                }

            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Add Employee By Excel

        #region API to search employee on behalf of details saved in employees profile
        /// <summary>
        /// Created By Bhavendra Singh Jat on 30-09-2022
        /// Modified By Suraj Bundel on 01-11-2022
        /// API >> api/employeenew/employeesearch
        /// </summary>
        [HttpGet]
        [Route("employeesearch")]
        public async Task<ResponseBodyModel> GetEmployeeSearch(string search)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<EmployeeDirectoryHelperClass> employeeList = new List<EmployeeDirectoryHelperClass>();
                var orgList = await _db.OrgMaster.Where(x => !x.IsActive && x.IsDeleted &&
                x.CompanyId == claims.companyId).Select(x => x.OrgId).ToListAsync();
                if (claims.orgId == 0)
                {
                    employeeList = await (from emp in _db.Employee
                                          join dep in _db.Department on emp.DepartmentId equals dep.DepartmentId
                                          join des in _db.Designation on emp.DesignationId equals des.DesignationId
                                          join use in _db.User on emp.EmployeeId equals use.EmployeeId
                                          join org in _db.OrgMaster on emp.OrgId equals org.OrgId into re
                                          from result in re.DefaultIfEmpty()
                                          where emp.IsActive && !emp.IsDeleted && emp.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee &&
                                          emp.CompanyId == claims.companyId && !orgList.Contains(result.OrgId) && (emp.DisplayName.ToLower().Contains(search.ToLower()) || emp.FirstName.ToLower().Contains(search.ToLower()) ||
                                          emp.LastName.ToLower().Contains(search.ToLower()) || emp.MobilePhone.Contains(search) || emp.OfficeEmail.ToLower().Contains(search.ToLower()))
                                          select new EmployeeDirectoryHelperClass
                                          {
                                              EmployeeId = emp.EmployeeId,
                                              EmployeeCode = emp.EmployeeCode,
                                              DisplayName = emp.DisplayName,
                                              MobilePhone = emp.MobilePhone,
                                              DepartmentId = dep.DepartmentId,
                                              DepartmentName = dep.DepartmentName,
                                              DesignationId = des.DesignationId,
                                              DesignationName = des.DesignationName,
                                              OfficeEmail = emp.OfficeEmail,
                                              EmployeeTypeId = emp.EmployeeTypeId,
                                              EmployeeTypeName = emp.EmployeeTypeId.ToString().Replace("_", " "),
                                              Location = emp.CurrentAddress,
                                              ProfileImageUrl = emp.ProfileImageUrl,
                                              FirstName = emp.FirstName,
                                              LastName = emp.LastName,
                                              MiddleName = emp.MiddleName,
                                              FatherName = emp.FatherName,
                                              MotherName = emp.MotherName,
                                              Gender = emp.Gender,
                                              DateOfBirth = emp.DateOfBirth,
                                              //hideDOB = emp.HideDOB,
                                              BloodGroup = emp.BloodGroup,
                                              BloodGroupName = emp.BloodGroup.ToString().Replace("_pos", "+").Replace("_neg", "-"),
                                              MaritalStatus = emp.MaritalStatus,
                                              AadharNumber = emp.AadharNumber,
                                              PanNumber = emp.PanNumber,
                                              PrimaryContact = emp.MobilePhone,
                                              JoiningDate = emp.JoiningDate,
                                              ConfirmationDate = emp.ConfirmationDate,
                                              Password = emp.Password,
                                              BankAccountNumber = emp.BankAccountNumber,
                                              AccountHolderName = emp.AccountHolderName,
                                              IFSC = emp.IFSC,
                                              BankName = emp.BankName,
                                              PermanentAddress = emp.PermanentAddress,
                                              WhatsappNumber = emp.WhatsappNumber,
                                              EmergencyNumber = emp.EmergencyNumber,
                                              PersonalEmail = emp.PersonalEmail,
                                              LocalAddress = emp.LocalAddress,
                                              MedicalIssue = emp.MedicalIssue,
                                              GrossSalery = emp.GrossSalery,
                                              OrgId = result.OrgId,
                                              OrgName = result.OrgName ?? "Company Head",
                                              // LoginType = use.LoginId,
                                              //LoginTypeName = use.LoginId.ToString(),
                                              AddedBy = emp.CreatedBy,
                                              LastUpdatedBy = emp.UpdatedBy.HasValue ? (int)emp.UpdatedBy : 0,
                                              IsEmployeeIsLock = emp.IsEmployeeIsLock,
                                          }).ToListAsync();

                    if (employeeList.Count > 0)
                    {
                        res.Status = true;
                        res.Message = "Employee List Found in Search Results";
                        res.Data = new
                        {
                            IsAdmin = claims.IsAdminInCompany,
                            EmployeeList = employeeList,
                        };
                    }
                    else
                    {
                        res.Status = false;
                        res.Message = "No Search Results Found";
                        res.Data = new
                        {
                            IsAdmin = claims.IsAdminInCompany,
                            EmployeeList = employeeList,
                        };
                    }
                }
                else
                {
                    employeeList = await (from emp in _db.Employee
                                          join dep in _db.Department on emp.DepartmentId equals dep.DepartmentId
                                          join des in _db.Designation on emp.DesignationId equals des.DesignationId
                                          join use in _db.User on emp.EmployeeId equals use.EmployeeId
                                          join org in _db.OrgMaster on emp.OrgId equals org.OrgId into re
                                          from result in re.DefaultIfEmpty()
                                          where emp.IsActive && !emp.IsDeleted && emp.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee &&
                                          emp.CompanyId == claims.companyId && emp.OrgId == claims.orgId && !orgList.Contains(result.OrgId) && (emp.DisplayName.ToLower().Contains(search.ToLower()) || emp.FirstName.ToLower().Contains(search.ToLower()) ||
                                          emp.LastName.ToLower().Contains(search.ToLower()) || emp.MobilePhone.Contains(search) || emp.OfficeEmail.ToLower().Contains(search.ToLower()))
                                          select new EmployeeDirectoryHelperClass
                                          {
                                              EmployeeId = emp.EmployeeId,
                                              EmployeeCode = emp.EmployeeCode,
                                              DisplayName = emp.DisplayName,
                                              MobilePhone = emp.MobilePhone,
                                              DepartmentId = dep.DepartmentId,
                                              DepartmentName = dep.DepartmentName,
                                              DesignationId = des.DesignationId,
                                              DesignationName = des.DesignationName,
                                              OfficeEmail = emp.OfficeEmail,
                                              EmployeeTypeId = emp.EmployeeTypeId,
                                              EmployeeTypeName = emp.EmployeeTypeId.ToString().Replace("_", " "),
                                              Location = emp.CurrentAddress,
                                              ProfileImageUrl = emp.ProfileImageUrl,
                                              FirstName = emp.FirstName,
                                              LastName = emp.LastName,
                                              MiddleName = emp.MiddleName,
                                              FatherName = emp.FatherName,
                                              MotherName = emp.MotherName,
                                              Gender = emp.Gender,
                                              DateOfBirth = emp.DateOfBirth,
                                              //hideDOB = emp.HideDOB,
                                              BloodGroup = emp.BloodGroup,
                                              BloodGroupName = emp.BloodGroup.ToString().Replace("_pos", "+").Replace("_neg", "-"),
                                              MaritalStatus = emp.MaritalStatus,
                                              AadharNumber = emp.AadharNumber,
                                              PanNumber = emp.PanNumber,
                                              PrimaryContact = emp.MobilePhone,
                                              JoiningDate = emp.JoiningDate,
                                              ConfirmationDate = emp.ConfirmationDate,
                                              Password = emp.Password,
                                              BankAccountNumber = emp.BankAccountNumber,
                                              AccountHolderName = emp.AccountHolderName,
                                              IFSC = emp.IFSC,
                                              BankName = emp.BankName,
                                              PermanentAddress = emp.PermanentAddress,
                                              WhatsappNumber = emp.WhatsappNumber,
                                              EmergencyNumber = emp.EmergencyNumber,
                                              PersonalEmail = emp.PersonalEmail,
                                              LocalAddress = emp.LocalAddress,
                                              MedicalIssue = emp.MedicalIssue,
                                              GrossSalery = emp.GrossSalery,
                                              OrgId = result.OrgId,
                                              OrgName = result.OrgName ?? "Company Head",
                                              // LoginType = use.LoginId,
                                              // LoginTypeName = use.LoginId.ToString(),
                                              AddedBy = emp.CreatedBy,
                                              LastUpdatedBy = emp.UpdatedBy.HasValue ? (int)emp.UpdatedBy : 0,
                                              IsEmployeeIsLock = emp.IsEmployeeIsLock,
                                          }).ToListAsync();

                    if (employeeList.Count > 0)
                    {
                        res.Status = true;
                        res.Message = "Employee List Found in Search Results";
                        res.Data = new
                        {
                            IsAdmin = claims.IsAdminInCompany,
                            EmployeeList = employeeList,
                        };
                    }
                    else
                    {
                        res.Status = false;
                        res.Message = "No Search Results Found";
                        res.Data = new
                        {
                            IsAdmin = claims.IsAdminInCompany,
                            EmployeeList = employeeList,
                        }; ;
                    }
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

        #region Api To Get Employee Directory Filter (For HR)

        /// <summary>
        /// Created By Harshit Mitra on 28-04-2022
        /// Created By Suraj Bundel on 01-11-2022
        /// API >> api/employeenew/empdirectoryfilter
        /// </summary>
        [HttpPost]
        [Route("empdirectoryfilter")]
        public async Task<ResponseBodyModel> GetEmployeeDirectoryFilter(EmployeeDirectoryParameter model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<EmployeeDirectoryHelperClass> employeeList = new List<EmployeeDirectoryHelperClass>();
                var orgList = await _db.OrgMaster.Where(x => !x.IsActive && x.IsDeleted &&
                        x.CompanyId == claims.companyId).Select(x => x.OrgId).ToListAsync();
                //if (claims.orgId == 0)
                //{
                employeeList = await (from e in _db.Employee
                                      join d in _db.Department on e.DepartmentId equals d.DepartmentId
                                      join ds in _db.Designation on e.DesignationId equals ds.DesignationId
                                      join or in _db.OrgMaster on e.OrgId equals or.OrgId into q
                                      from result in q.DefaultIfEmpty()
                                      where e.IsActive && !e.IsDeleted &&
                                      e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee &&
                                      e.CompanyId == claims.companyId
                                      && !orgList.Contains(result.OrgId)
                                      //&& (result.OrgId != 0 ? result.IsActive && !result.IsDeleted : true)
                                      select new EmployeeDirectoryHelperClass
                                      {
                                          EmployeeId = e.EmployeeId,
                                          EmployeeCode = e.EmployeeCode,
                                          DisplayName = e.DisplayName,
                                          MobilePhone = e.MobilePhone,
                                          DepartmentId = d.DepartmentId,
                                          DepartmentName = d.DepartmentName,
                                          DesignationId = ds.DesignationId,
                                          DesignationName = ds.DesignationName,
                                          OfficeEmail = e.OfficeEmail,
                                          EmployeeTypeId = e.EmployeeTypeId,
                                          EmployeeTypeName = e.EmployeeTypeId.ToString().Replace("_", " "),
                                          Location = e.CurrentAddress,
                                          ProfileImageUrl = e.ProfileImageUrl,
                                          FirstName = e.FirstName,
                                          LastName = e.LastName,
                                          MiddleName = e.MiddleName,
                                          FatherName = e.FatherName,
                                          MotherName = e.MotherName,
                                          Gender = e.Gender,
                                          DateOfBirth = e.DateOfBirth,
                                          //hideDOB=e.HideDOB,
                                          BloodGroup = e.BloodGroup,
                                          BloodGroupName = e.BloodGroup.ToString().Replace("_pos", "+").Replace("_neg", "-"),
                                          MaritalStatus = e.MaritalStatus,
                                          AadharNumber = e.AadharNumber,
                                          PanNumber = e.PanNumber,
                                          PrimaryContact = e.MobilePhone,
                                          JoiningDate = e.JoiningDate,
                                          ConfirmationDate = e.ConfirmationDate,
                                          Password = e.Password,
                                          BankAccountNumber = e.BankAccountNumber,
                                          AccountHolderName = e.AccountHolderName,
                                          IFSC = e.IFSC,
                                          BankName = e.BankName,
                                          PermanentAddress = e.PermanentAddress,
                                          WhatsappNumber = e.WhatsappNumber,
                                          EmergencyNumber = e.EmergencyNumber,
                                          PersonalEmail = e.PersonalEmail,
                                          LocalAddress = e.LocalAddress,
                                          MedicalIssue = e.MedicalIssue,
                                          GrossSalery = e.GrossSalery,
                                          OrgId = result.OrgId,
                                          OrgName = result.OrgName ?? "Company Head",
                                          //  LoginType = _db.User.Where(x => x.EmployeeId == e.EmployeeId).Select(x => x.LoginId).FirstOrDefault(),
                                          AddedBy = e.CreatedBy,
                                          LastUpdatedBy = e.UpdatedBy.HasValue ? (int)e.UpdatedBy : 0,
                                          ReportingManagerId = e.ReportingManager,
                                          IsEmployeeIsLock = e.IsEmployeeIsLock,
                                      }).ToListAsync();
                //}
                //else
                //{
                //    employeeList = await (from e in _db.Employee
                //                          join d in _db.Department on e.DepartmentId equals d.DepartmentId
                //                          join ds in _db.Designation on e.DesignationId equals ds.DesignationId
                //                          join or in _db.OrgMaster on e.OrgId equals or.OrgId into q
                //                          from result in q.DefaultIfEmpty()
                //                          where e.IsActive && !e.IsDeleted &&
                //                          e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee &&
                //                          e.CompanyId == claims.companyId && claims.orgId == e.OrgId
                //                          && !orgList.Contains(result.OrgId)
                //                          select new EmployeeDirectoryHelperClass
                //                          {
                //                              EmployeeId = e.EmployeeId,
                //                              EmployeeCode = e.EmployeeCode,
                //                              DisplayName = e.DisplayName,
                //                              MobilePhone = e.MobilePhone,
                //                              DepartmentId = d.DepartmentId,
                //                              DepartmentName = d.DepartmentName,
                //                              DesignationId = ds.DesignationId,
                //                              DesignationName = ds.DesignationName,
                //                              OfficeEmail = e.OfficeEmail,
                //                              EmployeeTypeId = e.EmployeeTypeId,
                //                              EmployeeTypeName = e.EmployeeTypeId.ToString().Replace("_", " "),
                //                              Location = e.CurrentAddress,
                //                              ProfileImageUrl = e.ProfileImageUrl,
                //                              FirstName = e.FirstName,
                //                              LastName = e.LastName,
                //                              MiddleName = e.MiddleName,
                //                              FatherName = e.FatherName,
                //                              MotherName = e.MotherName,
                //                              Gender = e.Gender,
                //                              DateOfBirth = e.DateOfBirth,
                //                              //hideDOB = e.HideDOB,
                //                              BloodGroup = e.BloodGroup,
                //                              BloodGroupName = e.BloodGroup.ToString().Replace("_pos", "+").Replace("_neg", "-"),
                //                              MaritalStatus = e.MaritalStatus,
                //                              AadharNumber = e.AadharNumber,
                //                              PanNumber = e.PanNumber,
                //                              PrimaryContact = e.MobilePhone,
                //                              JoiningDate = e.JoiningDate,
                //                              ConfirmationDate = e.ConfirmationDate,
                //                              Password = e.Password,
                //                              BankAccountNumber = e.BankAccountNumber,
                //                              AccountHolderName = e.AccountHolderName,
                //                              IFSC = e.IFSC,
                //                              BankName = e.BankName,
                //                              PermanentAddress = e.PermanentAddress,
                //                              WhatsappNumber = e.WhatsappNumber,
                //                              EmergencyNumber = e.EmergencyNumber,
                //                              PersonalEmail = e.PersonalEmail,
                //                              LocalAddress = e.LocalAddress,
                //                              MedicalIssue = e.MedicalIssue,
                //                              GrossSalery = e.GrossSalery,
                //                              OrgId = result.OrgId,
                //                              OrgName = result.OrgName ?? "Company Head",
                //                              LoginType = _db.User.Where(x => x.EmployeeId == e.EmployeeId).Select(x => x.LoginId).FirstOrDefault(),
                //                              AddedBy = e.CreatedBy,
                //                              LastUpdatedBy = e.UpdatedBy.HasValue ? (int)e.UpdatedBy : 0,
                //                              ReportingManagerId = e.ReportingManager,
                //                          }).ToListAsync();
                //}
                var checkdata = _db.Employee.Where(x => x.CompanyId == claims.companyId)
                        .Select(x => new
                        {
                            x.EmployeeId,
                            x.DisplayName,
                            x.OfficeEmail,
                        }).ToList();
                var newList = employeeList
                        .Select(e => new
                        {
                            EmployeeId = e.EmployeeId,
                            EmployeeCode = e.EmployeeCode,
                            DisplayName = e.DisplayName,
                            MobilePhone = e.MobilePhone,
                            DepartmentId = e.DepartmentId,
                            DepartmentName = e.DepartmentName,
                            DesignationId = e.DesignationId,
                            DesignationName = e.DesignationName,
                            OfficeEmail = e.OfficeEmail,
                            EmployeeTypeId = e.EmployeeTypeId,
                            EmployeeTypeName = e.EmployeeTypeName,
                            Location = e.Location,
                            ProfileImageUrl = e.ProfileImageUrl,
                            FirstName = e.FirstName,
                            LastName = e.LastName,
                            MiddleName = e.MiddleName,
                            FatherName = e.FatherName,
                            MotherName = e.MotherName,
                            Gender = e.Gender,
                            DateOfBirth = e.DateOfBirth,
                            BloodGroup = e.BloodGroup,
                            BloodGroupName = e.BloodGroup.ToString().Replace("_pos", "+").Replace("_neg", "-"),
                            MaritalStatus = e.MaritalStatus,
                            AadharNumber = e.AadharNumber,
                            PanNumber = e.PanNumber,
                            PrimaryContact = e.MobilePhone,
                            JoiningDate = e.JoiningDate,
                            ConfirmationDate = e.ConfirmationDate,
                            Password = e.Password,
                            BankAccountNumber = e.BankAccountNumber,
                            AccountHolderName = e.AccountHolderName,
                            IFSC = e.IFSC,
                            BankName = e.BankName,
                            PermanentAddress = e.PermanentAddress,
                            WhatsappNumber = e.WhatsappNumber,
                            EmergencyNumber = e.EmergencyNumber,
                            PersonalEmail = e.PersonalEmail,
                            LocalAddress = e.LocalAddress,
                            MedicalIssue = e.MedicalIssue,
                            GrossSalery = e.GrossSalery,
                            //LoginType = e.LoginType,
                            OrgId = e.OrgId ?? 0,
                            OrgName = e.OrgName,
                            //  LoginTypeName = Enum.GetName(typeof(LoginRolesConstants), e.LoginType).Replace("_", " "),
                            AddedBy = e.AddedBy,
                            AddedByName = checkdata.Where(x => x.EmployeeId == e.AddedBy).Select(x => x.DisplayName).FirstOrDefault(),
                            LastUpdatedBy = e.LastUpdatedBy,
                            LastUpdatedByName = e.LastUpdatedBy != 0 ? checkdata.Where(x => x.EmployeeId == e.LastUpdatedBy).Select(x => x.DisplayName).FirstOrDefault() : null,
                            ReportingManager = checkdata.Where(x => x.EmployeeId == e.ReportingManagerId).Select(x => x.DisplayName).FirstOrDefault(),
                            ReportingManagerOfficalMail = checkdata.Where(x => x.EmployeeId == e.ReportingManagerId).Select(x => x.OfficeEmail).FirstOrDefault(),
                            e.IsEmployeeIsLock,
                        }).ToList();

                if (model.OrgId.Count == 0)
                {
                    if (model.DepartmentId.Count == 0)
                    {
                        if (model.DesignationId.Count == 0)
                        {
                            if (model.EmployeeTypeId.Count == 0)
                            {
                                newList = newList.ToList();
                            }
                            else
                            {
                                newList = newList.Where(x => model.EmployeeTypeId.Contains(x.EmployeeTypeId)).ToList();
                            }
                        }
                        else
                        {
                            if (model.EmployeeTypeId.Count == 0)
                            {
                                newList = newList.Where(x => model.DesignationId.Contains(x.DesignationId)).ToList();
                            }
                            else
                            {
                                newList = newList.Where(x => model.DesignationId.Contains(x.DesignationId) &&
                                        model.EmployeeTypeId.Contains(x.EmployeeTypeId)).ToList();
                            }
                        }
                    }
                    else
                    {
                        if (model.DesignationId.Count == 0)
                        {
                            if (model.EmployeeTypeId.Count == 0)
                            {
                                newList = newList.Where(x => model.DepartmentId.Contains(x.DepartmentId)).ToList();
                            }
                            else
                            {
                                newList = newList.Where(x => model.DepartmentId.Contains(x.DepartmentId) &&
                                    model.EmployeeTypeId.Contains(x.EmployeeTypeId)).ToList();
                            }
                        }
                        else
                        {
                            if (model.EmployeeTypeId.Count == 0)
                            {
                                newList = newList.Where(x => model.DepartmentId.Contains(x.DepartmentId) &&
                                    model.DesignationId.Contains(x.DesignationId)).ToList();
                            }
                            else
                            {
                                newList = newList.Where(x => model.DepartmentId.Contains(x.DepartmentId) &&
                                    model.DesignationId.Contains(x.DesignationId) && model.EmployeeTypeId.Contains(x.EmployeeTypeId)).ToList();
                            }
                        }
                    }
                }
                else
                {
                    if (model.DepartmentId.Count == 0)
                    {
                        if (model.DesignationId.Count == 0)
                        {
                            if (model.EmployeeTypeId.Count == 0)
                            {
                                newList = newList.Where(x => model.OrgId.Contains((int)x.OrgId)).ToList();
                            }
                            else
                            {
                                newList = newList.Where(x => model.EmployeeTypeId.Contains(x.EmployeeTypeId)
                                    && model.OrgId.Contains((int)x.OrgId)).ToList();
                            }
                        }
                        else
                        {
                            if (model.EmployeeTypeId.Count == 0)
                            {
                                newList = newList.Where(x => model.DesignationId.Contains(x.DesignationId)
                                    && model.OrgId.Contains((int)x.OrgId)).ToList();
                            }
                            else
                            {
                                newList = newList.Where(x => model.DesignationId.Contains(x.DesignationId) &&
                                        model.EmployeeTypeId.Contains(x.EmployeeTypeId) && model.OrgId.Contains((int)x.OrgId)).ToList();
                            }
                        }
                    }
                    else
                    {
                        if (model.DesignationId.Count == 0)
                        {
                            if (model.EmployeeTypeId.Count == 0)
                            {
                                newList = newList.Where(x => model.DepartmentId.Contains(x.DepartmentId)
                                    && model.OrgId.Contains((int)x.OrgId)).ToList();
                            }
                            else
                            {
                                newList = newList.Where(x => model.DepartmentId.Contains(x.DepartmentId) &&
                                    model.EmployeeTypeId.Contains(x.EmployeeTypeId) && model.OrgId.Contains((int)x.OrgId)).ToList();
                            }
                        }
                        else
                        {
                            if (model.EmployeeTypeId.Count == 0)
                            {
                                newList = newList.Where(x => model.DepartmentId.Contains(x.DepartmentId) &&
                                    model.DesignationId.Contains(x.DesignationId) && model.OrgId.Contains((int)x.OrgId)).ToList();
                            }
                            else
                            {
                                newList = newList.Where(x => model.DepartmentId.Contains(x.DepartmentId) &&
                                    model.DesignationId.Contains(x.DesignationId) && model.EmployeeTypeId.Contains(x.EmployeeTypeId)
                                    && model.OrgId.Contains((int)x.OrgId)).ToList();
                            }
                        }
                    }
                }
                if (newList.Count > 0)
                {
                    res.Message = "Employee List";
                    res.Status = true;
                    res.Data = new
                    {
                        IsAdmin = claims.IsAdminInCompany,
                        EmployeeList = newList.OrderByDescending(x => x.EmployeeId).ToList(),
                    };
                }
                else
                {
                    res.Message = "Employee List Is Empty";
                    res.Status = false;
                    res.Data = new
                    {
                        IsAdmin = claims.IsAdminInCompany,
                        EmployeeList = newList,
                    };
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Employee Directory Filter (For HR)

        #region Api To Get Employee Directory Filter (For Admin)

        /// <summary>
        /// Created By Harshit Mitra on 30-04-2022
        /// API >> api/employeenew/empdirectoryfilteradmin
        /// </summary>
        [HttpPost]
        [Route("empdirectoryfilteradmin")]
        public async Task<ResponseBodyModel> GetEmployeeDirectoryFilterAdmin(EmployeeDirectoryParameterAdmin model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var orgList = await _db.OrgMaster.Where(x => !x.IsActive && x.IsDeleted &&
                        x.CompanyId == claims.companyId).Select(x => x.OrgId).ToListAsync();
                var employeeList = await (from e in _db.Employee
                                          join d in _db.Department on e.DepartmentId equals d.DepartmentId
                                          join ds in _db.Designation on e.DesignationId equals ds.DesignationId
                                          join or in _db.OrgMaster on e.OrgId equals or.OrgId into q
                                          from result in q.DefaultIfEmpty()
                                          where e.IsActive && !e.IsDeleted &&
                                          e.CompanyId == claims.companyId && e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee
                                          && !orgList.Contains(result.OrgId)
                                          select new EmployeeDirectoryHelperClassAdmin
                                          {
                                              OrgId = e.OrgId,
                                              OrgName = result.OrgName,
                                              EmployeeId = e.EmployeeId,
                                              DisplayName = e.DisplayName,
                                              MobilePhone = e.MobilePhone,
                                              DepartmentId = d.DepartmentId,
                                              DepartmentName = d.DepartmentName,
                                              DesignationId = ds.DesignationId,
                                              DesignationName = ds.DesignationName,
                                              OfficeEmail = e.OfficeEmail,
                                              EmployeeTypeId = e.EmployeeTypeId,
                                              Location = e.City,
                                              ProfileImageUrl = e.ProfileImageUrl,
                                              AddedBy = e.CreatedBy,
                                              LastUpdatedBy = e.UpdatedBy.HasValue ? (int)e.UpdatedBy : 0,
                                              IsEmployeeIsLock = e.IsEmployeeIsLock,
                                          }).ToListAsync();
                var checkdata = _db.Employee.Where(x => x.CompanyId == claims.companyId)
                        .Select(x => new
                        {
                            x.EmployeeId,
                            x.DisplayName,
                        }).ToList();
                employeeList = employeeList
                            .Select(e => new EmployeeDirectoryHelperClassAdmin
                            {
                                OrgId = e.OrgId,
                                OrgName = e.OrgName,
                                EmployeeId = e.EmployeeId,
                                DisplayName = e.DisplayName,
                                MobilePhone = e.MobilePhone,
                                DepartmentId = e.DepartmentId,
                                DepartmentName = e.DepartmentName,
                                DesignationId = e.DesignationId,
                                DesignationName = e.DesignationName,
                                OfficeEmail = e.OfficeEmail,
                                EmployeeTypeId = e.EmployeeTypeId,
                                Location = e.Location,
                                ProfileImageUrl = e.ProfileImageUrl,
                                AddedBy = e.AddedBy,
                                AddedByName = employeeList.Where(x => x.EmployeeId == e.AddedBy).Select(x => x.DisplayName).FirstOrDefault(),
                                LastUpdatedBy = e.LastUpdatedBy,
                                LastUpdatedByName = e.LastUpdatedBy != 0 ? checkdata.Where(x => x.EmployeeId == e.LastUpdatedBy).Select(x => x.DisplayName).FirstOrDefault() : "-------",
                                IsEmployeeIsLock = e.IsEmployeeIsLock,
                            }).ToList();

                if (model.OrgId.Count == 0)
                {
                    if (model.DepartmentId.Count == 0)
                    {
                        if (model.DesignationId.Count == 0)
                        {
                            if (model.EmployeeTypeId.Count == 0)
                            {
                                employeeList = employeeList.ToList();
                            }
                            else
                            {
                                employeeList = employeeList.Where(x => model.EmployeeTypeId.Contains(x.EmployeeTypeId)).ToList();
                            }
                        }
                        else
                        {
                            if (model.EmployeeTypeId.Count == 0)
                            {
                                employeeList = employeeList.Where(x => model.DesignationId.Contains(x.DesignationId)).ToList();
                            }
                            else
                            {
                                employeeList = employeeList.Where(x => model.DesignationId.Contains(x.DesignationId) &&
                                        model.EmployeeTypeId.Contains(x.EmployeeTypeId)).ToList();
                            }
                        }
                    }
                    else
                    {
                        if (model.DesignationId.Count == 0)
                        {
                            if (model.EmployeeTypeId.Count == 0)
                            {
                                employeeList = employeeList.Where(x => model.DepartmentId.Contains(x.DepartmentId)).ToList();
                            }
                            else
                            {
                                employeeList = employeeList.Where(x => model.DepartmentId.Contains(x.DepartmentId) &&
                                    model.EmployeeTypeId.Contains(x.EmployeeTypeId)).ToList();
                            }
                        }
                        else
                        {
                            if (model.EmployeeTypeId.Count == 0)
                            {
                                employeeList = employeeList.Where(x => model.DepartmentId.Contains(x.DepartmentId) &&
                                    model.DesignationId.Contains(x.DesignationId)).ToList();
                            }
                            else
                            {
                                employeeList = employeeList.Where(x => model.DepartmentId.Contains(x.DepartmentId) &&
                                    model.DesignationId.Contains(x.DesignationId) && model.EmployeeTypeId.Contains(x.EmployeeTypeId)).ToList();
                            }
                        }
                    }
                }
                else
                {
                    if (model.DepartmentId.Count == 0)
                    {
                        if (model.DesignationId.Count == 0)
                        {
                            if (model.EmployeeTypeId.Count == 0)
                            {
                                employeeList = employeeList.Where(x => model.OrgId.Contains(x.OrgId)).ToList();
                            }
                            else
                            {
                                employeeList = employeeList.Where(x => model.EmployeeTypeId.Contains(x.EmployeeTypeId)
                                    && model.OrgId.Contains(x.OrgId)).ToList();
                            }
                        }
                        else
                        {
                            if (model.EmployeeTypeId.Count == 0)
                            {
                                employeeList = employeeList.Where(x => model.DesignationId.Contains(x.DesignationId)
                                    && model.OrgId.Contains(x.OrgId)).ToList();
                            }
                            else
                            {
                                employeeList = employeeList.Where(x => model.DesignationId.Contains(x.DesignationId) &&
                                        model.EmployeeTypeId.Contains(x.EmployeeTypeId) && model.OrgId.Contains(x.OrgId)).ToList();
                            }
                        }
                    }
                    else
                    {
                        if (model.DesignationId.Count == 0)
                        {
                            if (model.EmployeeTypeId.Count == 0)
                            {
                                employeeList = employeeList.Where(x => model.DepartmentId.Contains(x.DepartmentId)
                                    && model.OrgId.Contains(x.OrgId)).ToList();
                            }
                            else
                            {
                                employeeList = employeeList.Where(x => model.DepartmentId.Contains(x.DepartmentId) &&
                                    model.EmployeeTypeId.Contains(x.EmployeeTypeId) && model.OrgId.Contains(x.OrgId)).ToList();
                            }
                        }
                        else
                        {
                            if (model.EmployeeTypeId.Count == 0)
                            {
                                employeeList = employeeList.Where(x => model.DepartmentId.Contains(x.DepartmentId) &&
                                    model.DesignationId.Contains(x.DesignationId) && model.OrgId.Contains(x.OrgId)).ToList();
                            }
                            else
                            {
                                employeeList = employeeList.Where(x => model.DepartmentId.Contains(x.DepartmentId) &&
                                    model.DesignationId.Contains(x.DesignationId) && model.EmployeeTypeId.Contains(x.EmployeeTypeId)
                                    && model.OrgId.Contains(x.OrgId)).ToList();
                            }
                        }
                    }
                }

                if (employeeList.Count > 0)
                {
                    res.Message = "Employee List";
                    res.Status = true;
                    res.Data = new
                    {
                        IsAdmin = claims.IsAdminInCompany,
                        EmployeeList = employeeList,
                    };
                }
                else
                {
                    res.Message = "Employee List Is Empty";
                    res.Status = false;
                    res.Data = new
                    {
                        IsAdmin = claims.IsAdminInCompany,
                        EmployeeList = employeeList,
                    }; ;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Employee Directory Filter (For Admin)

        #region API To Get Employee Data On Admin (All Employee)

        /// <summary>
        /// Created By Harshit Mitra on 15-07-2022
        /// API >> Get >> api/employeenew/getallemployeeonadmin
        /// </summary>
        [HttpGet]
        [Route("getallemployeeonadmin")]
        public async Task<ResponseBodyModel> GetEmployeeDataOnAdmin()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employeeList = await (from e in _db.Employee
                                          join d in _db.Department on e.DepartmentId equals d.DepartmentId
                                          join ds in _db.Designation on e.DesignationId equals ds.DesignationId
                                          join or in _db.OrgMaster on e.OrgId equals or.OrgId into q
                                          from result in q.DefaultIfEmpty()
                                          where e.IsActive && !e.IsDeleted &&
                                          e.CompanyId == claims.companyId && e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee
                                          select new EmployeeDirectoryHelperClassAdmin
                                          {
                                              OrgId = e.OrgId,
                                              OrgName = result.OrgName,
                                              EmployeeId = e.EmployeeId,
                                              DisplayName = e.DisplayName,
                                              MobilePhone = e.MobilePhone,
                                              DepartmentId = d.DepartmentId,
                                              DepartmentName = d.DepartmentName,
                                              DesignationId = ds.DesignationId,
                                              DesignationName = ds.DesignationName,
                                              OfficeEmail = e.OfficeEmail,
                                              EmployeeTypeId = e.EmployeeTypeId,
                                              Location = e.City,
                                              ProfileImageUrl = e.ProfileImageUrl,
                                              BloodGroup = e.BloodGroup,
                                              BloodGroupName = e.BloodGroup.ToString().Replace("_pos", "+").Replace("_neg", "-"),
                                              AddedBy = e.CreatedBy,
                                              LastUpdatedBy = e.UpdatedBy.HasValue ? (int)e.UpdatedBy : 0,
                                              IsEmployeeIsLock = e.IsEmployeeIsLock,
                                          }).ToListAsync();
                var checkdata = _db.Employee.Where(x => x.CompanyId == claims.companyId)
                        .Select(x => new
                        {
                            x.EmployeeId,
                            x.DisplayName,
                        }).ToList();
                employeeList = employeeList
                            .Select(e => new EmployeeDirectoryHelperClassAdmin
                            {
                                OrgId = e.OrgId,
                                OrgName = string.IsNullOrEmpty(e.OrgName) ? "Company Head" : e.OrgName,
                                EmployeeId = e.EmployeeId,
                                DisplayName = e.DisplayName,
                                MobilePhone = e.MobilePhone,
                                DepartmentId = e.DepartmentId,
                                DepartmentName = e.DepartmentName,
                                DesignationId = e.DesignationId,
                                DesignationName = e.DesignationName,
                                OfficeEmail = e.OfficeEmail,
                                EmployeeTypeId = e.EmployeeTypeId,
                                Location = e.Location,
                                ProfileImageUrl = e.ProfileImageUrl,
                                AddedBy = e.AddedBy,
                                BloodGroup = e.BloodGroup,
                                BloodGroupName = e.BloodGroup.ToString().Replace("_pos", "+").Replace("_neg", "-"),
                                AddedByName = employeeList.Where(x => x.EmployeeId == e.AddedBy).Select(x => x.DisplayName).FirstOrDefault(),
                                LastUpdatedBy = e.LastUpdatedBy,
                                LastUpdatedByName = e.LastUpdatedBy != 0 ? checkdata.Where(x => x.EmployeeId == e.LastUpdatedBy).Select(x => x.DisplayName).FirstOrDefault() : "-------",
                                IsEmployeeIsLock = e.IsEmployeeIsLock,
                            }).ToList();

                if (employeeList.Count > 0)
                {
                    res.Message = "Employee List";
                    res.Status = true;
                    res.Data = employeeList.OrderBy(x => x.DisplayName).ToList();
                }
                else
                {
                    res.Message = "Employee List Is Empty";
                    res.Status = false;
                    res.Data = employeeList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Employee Data On Admin (All Employee)

        #region API To Get Employee (Filtered By Org)

        /// <summary>
        /// Created By Harshit Mitra on 29-04-2022
        /// Modify By Harshit Mitra on 05-08-2022
        /// API >> Get >> api/employeenew/emplistoforg
        /// </summary>
        /// <param name="count"></param>
        /// <param name="page"></param>
        /// <param name="search"></param>
        [HttpGet]
        [Route("emplistoforg")]
        public async Task<ResponseBodyModel> EmployeeListOfParticularOrg(int? page = null, int? count = null, string search = null)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<EmployeeDirectoryHelperClass> employeeList = new List<EmployeeDirectoryHelperClass>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (claims.orgId != 0)
                {
                    employeeList = await (from e in _db.Employee
                                          join d in _db.Department on e.DepartmentId equals d.DepartmentId
                                          join ds in _db.Designation on e.DesignationId equals ds.DesignationId
                                          where e.IsActive && !e.IsDeleted &&
                                          e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee &&
                                          e.CompanyId == claims.companyId && (e.OrgId == claims.orgId || e.OrgId == 0)
                                          select new EmployeeDirectoryHelperClass
                                          {
                                              EmployeeId = e.EmployeeId,
                                              DisplayName = e.DisplayName,
                                              MobilePhone = e.MobilePhone,
                                              DepartmentId = d.DepartmentId,
                                              DepartmentName = d.DepartmentName,
                                              DesignationId = ds.DesignationId,
                                              DesignationName = ds.DesignationName,
                                              OfficeEmail = e.OfficeEmail,
                                              EmployeeTypeId = e.EmployeeTypeId,
                                              Location = e.CurrentAddress,

                                          }).OrderBy(x => x.DisplayName).ToListAsync();
                }
                else
                {
                    employeeList = await (from e in _db.Employee
                                          join d in _db.Department on e.DepartmentId equals d.DepartmentId
                                          join ds in _db.Designation on e.DesignationId equals ds.DesignationId
                                          where e.IsActive && !e.IsDeleted &&
                                          e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee &&
                                          e.CompanyId == claims.companyId
                                          select new EmployeeDirectoryHelperClass
                                          {
                                              EmployeeId = e.EmployeeId,
                                              DisplayName = e.DisplayName,
                                              MobilePhone = e.MobilePhone,
                                              DepartmentId = d.DepartmentId,
                                              DepartmentName = d.DepartmentName,
                                              DesignationId = ds.DesignationId,
                                              DesignationName = ds.DesignationName,
                                              OfficeEmail = e.OfficeEmail,
                                              EmployeeTypeId = e.EmployeeTypeId,
                                              Location = e.CurrentAddress,
                                          }).ToListAsync();
                }
                if (employeeList.Count > 0)
                {
                    res.Message = "Employee List";
                    res.Status = true;
                    if (!String.IsNullOrEmpty(search))
                    {
                        employeeList = employeeList.Where(x => search.Contains(x.DisplayName)).ToList();
                    }
                    res.Data = page.HasValue && count.HasValue
                        ? new PaginationData
                        {
                            TotalData = employeeList.Count,
                            Counts = (int)count,
                            List = employeeList.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        }
                        : (object)employeeList;
                }
                else
                {
                    res.Message = "Employee List Is Empty";
                    res.Status = false;
                    if (page.HasValue && count.HasValue)
                    {
                        res.Data = new PaginationData
                        {
                            TotalData = employeeList.Count,
                            Counts = (int)count,
                            List = employeeList.Skip(((int)page - 1) * (int)count).Take((int)count).ToList(),
                        };
                    }
                    else
                        res.Data = employeeList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Employee (Filtered By Org)

        #region Api To Update Profile By Employe Id

        /// <summary>
        /// Created By Harshit Mitra On 11-05-2022
        /// Modified By Suraj Bundel On 01-11-2022
        /// API >> Put >> api/employeenew/updateemployeeprofilebyid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updateemployeeprofilebyid")]
        public async Task<ResponseBodyModel> UpdateEmployeeProfileById(UpdateEmployeeProfileSelf model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var shiftdata = _db.ShiftGroups.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId
                  && x.ShiftName == "Default Shift").FirstOrDefault();
                var Weekoff = _db.WeekOffDays.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId
                 && x.WeekOffName == "Default Week Offs").FirstOrDefault();
                var employeeMgr = _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId).ToList();
                var employee = await _db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == model.EmployeeId);
                if (employee != null)
                {
                    if (model == null)
                    {
                        res.Message = "Invalid Model";
                        res.Status = false;
                        return res;
                    }
                    var employeData = _db.Employee.Where(x => x.ReportingManager == model.EmployeeId).FirstOrDefault();

                    if (model.EmployeeTypeId == EmployeeTypeConstants.Ex_Employee && model.ReportingManagerId == employee.ReportingManager && employeData != null)
                    {
                        res.Message = "This employee is already assigned as Reporting manager";
                        res.Status = false;
                        return res;
                    }
                    else
                    {
                        employee.ProfileImageUrl = String.IsNullOrEmpty(model.ProfileImageUrl) ? employee.ProfileImageUrl : model.ProfileImageUrl;
                        employee.ProfileImageExtension = String.IsNullOrEmpty(model.ProfileImageExtension) ?
                                    employee.ProfileImageExtension : model.ProfileImageExtension;
                        employee.FirstName = String.IsNullOrEmpty(model.FirstName) ? employee.FirstName : model.FirstName;
                        //    employee.MiddleName = String.IsNullOrEmpty(model.MiddleName) ? employee.MiddleName : model.MiddleName;
                        employee.MiddleName = model.MiddleName;
                        employee.LastName = String.IsNullOrEmpty(model.LastName) ? employee.LastName : model.LastName;
                        employee.DisplayName = String.IsNullOrEmpty(employee.MiddleName) ?
                                    employee.FirstName.Trim() + " " + employee.LastName.Trim() :
                                    employee.FirstName.Trim() + " " + employee.MiddleName.Trim() + " " + employee.LastName.Trim();
                        employee.Gender = String.IsNullOrEmpty(model.Gender) ? employee.Gender : model.Gender;
                        employee.DateOfBirth = model.DateOfBirth == null ? employee.DateOfBirth : (DateTime)model.DateOfBirth;
                        //employee.HideDOB = (bool)model.HideDOB;
                        employee.MaritalStatus = String.IsNullOrEmpty(model.MaritalStatus) ? employee.MaritalStatus : model.MaritalStatus;
                        employee.BloodGroup = model.BloodGroup == null ? employee.BloodGroup : (BloodGroupConstants)model.BloodGroup;
                        employee.IsPhysicallyHandicapped = model.IsPhysicallyHandicapped == null ?
                                    employee.IsPhysicallyHandicapped : (bool)model.IsPhysicallyHandicapped;
                        employee.PersonalEmail = String.IsNullOrEmpty(model.PersonalEmail) ?
                                    employee.PersonalEmail : model.PersonalEmail;
                        employee.MobilePhone = String.IsNullOrEmpty(model.MobilePhone) ? employee.MobilePhone : model.MobilePhone;
                        employee.WorkPhone = String.IsNullOrEmpty(model.WorkPhone) ? employee.WorkPhone : model.WorkPhone;
                        employee.SkypeMail = String.IsNullOrEmpty(model.SkypeMail) ? employee.SkypeMail : model.SkypeMail;
                        employee.LocalAddress = String.IsNullOrEmpty(model.LocalAddress) ? employee.LocalAddress : model.LocalAddress;
                        employee.LocalCountryId = model.LocalCountryId == null ? employee.LocalCountryId : model.LocalCountryId;
                        employee.LocalStateId = model.LocalStateId == null ? employee.LocalStateId : model.LocalStateId;
                        employee.LocalCityId = model.LocalCityId == null ? employee.LocalCityId : model.LocalCityId;
                        employee.LocalPinCode = model.LocalPinCode == null ? employee.LocalPinCode : model.LocalPinCode;
                        employee.PermanentAddress = String.IsNullOrEmpty(model.PermanentAddress) ?
                                    employee.PermanentAddress : model.PermanentAddress;
                        employee.PermanentCountryId = model.PermanentCountryId == null ?
                                    employee.PermanentCountryId : model.PermanentCountryId;
                        employee.PermanentStateId = model.PermanentStateId == null ? employee.PermanentStateId : model.PermanentStateId;
                        employee.PermenentCityId = model.PermenentCityId == null ? employee.PermenentCityId : model.PermenentCityId;
                        employee.PermenentPinCode = model.PermenentPinCode == null ? employee.PermenentPinCode : model.PermenentPinCode;
                        employee.ProfessionalSummary = String.IsNullOrEmpty(model.ProfessionalSummary) ?
                                    employee.ProfessionalSummary : model.ProfessionalSummary;
                        employee.AboutMeRemark = String.IsNullOrEmpty(model.AboutMeRemark) ? employee.AboutMeRemark : model.AboutMeRemark;
                        employee.AboutMyJobRemark = String.IsNullOrEmpty(model.AboutMyJobRemark) ? employee.AboutMyJobRemark : model.AboutMyJobRemark;
                        employee.InterestAndHobbiesRemark = String.IsNullOrEmpty(model.InterestAndHobbiesRemark) ?
                                    employee.InterestAndHobbiesRemark : model.InterestAndHobbiesRemark;
                        //-------------added by Shriya on 11-06-2022
                        employee.GrossSalery = model.Salary == 0 ? employee.GrossSalery : model.Salary;
                        employee.PanNumber = String.IsNullOrEmpty(model.PanNumber) ? employee.PanNumber : model.PanNumber;
                        employee.AadharNumber = String.IsNullOrEmpty(model.AadharNumber) ? employee.AadharNumber : model.AadharNumber;
                        employee.FatherName = String.IsNullOrEmpty(model.FatherName) ? employee.FatherName : model.FatherName;
                        employee.MotherName = String.IsNullOrEmpty(model.MotherName) ? employee.MotherName : model.MotherName;
                        employee.BiometricId = String.IsNullOrEmpty(model.BiometricId) ? employee.BiometricId : model.BiometricId;
                        employee.BankAccountNumber = String.IsNullOrEmpty(model.BankAccountNumber) ? employee.BankAccountNumber : model.BankAccountNumber;
                        employee.IFSC = String.IsNullOrEmpty(model.IFSC) ? employee.IFSC : model.IFSC;
                        employee.AccountHolderName = String.IsNullOrEmpty(model.AccountHolderName) ? employee.AccountHolderName : model.AccountHolderName;
                        employee.BankName = String.IsNullOrEmpty(model.BankName) ? employee.BankName : model.BankName;
                        employee.MedicalIssue = String.IsNullOrEmpty(model.MedicalIssue) ? employee.MedicalIssue : model.MedicalIssue;
                        employee.EmployeeCode = String.IsNullOrEmpty(model.EmployeeCode) ? employee.EmployeeCode : model.EmployeeCode;
                        employee.OrgId = model.OrgId == 0 ? employee.OrgId : model.OrgId;
                        employee.UpdatedBy = claims.employeeId;
                        employee.UpdatedOn = DateTime.Now;
                        employee.EmployeeTypeId = model.EmployeeTypeId;
                        employee.ReportingManager = model.ReportingManagerId == 0 ? employee.ReportingManager : model.ReportingManagerId;
                        employee.ShiftGroupId = model.ShiftGroupId == Guid.Empty ? shiftdata.ShiftGoupId : model.ShiftGroupId;
                        employee.WeekOffId = model.WeekoffId == Guid.Empty ? Weekoff.WeekOffId : model.WeekoffId;
                        employee.DepartmentId = model.DepartmentId == 0 ? employee.DepartmentId : model.DepartmentId;
                        employee.DesignationId = model.DesignationId == 0 ? employee.DesignationId : model.DesignationId;

                        var user = _db.User.Where(x => x.EmployeeId == model.EmployeeId).FirstOrDefault();
                        _db.Entry(employee).State = System.Data.Entity.EntityState.Modified;
                        await _db.SaveChangesAsync();

                        if (employee.EmployeeTypeId == EmployeeTypeConstants.Ex_Employee)
                        {
                            EmployeeExits obj = new EmployeeExits()
                            {
                                DiscussionSummary = "no",
                                ReasonId = employee.EmployeeId,
                                Reason = ExitEmpReasonConstants.Other.ToString(),
                                EmployeeId = employee.EmployeeId,
                                EmployeeName = employee.DisplayName,
                                IsDiscussion = true,
                                ExitType = ExitInitingType.Employee_Want_To_Resign,
                                CompanyId = claims.companyId,
                                TerminateDate = DateTime.Now,
                                OrgId = claims.orgId,
                                CreatedBy = claims.employeeId,
                                CreatedOn = DateTime.Now,
                                Status = ExitStatusConstants.Pending,
                                IsActive = true,
                                IsDeleted = false,
                                InProgress = true,
                            };
                            _db.EmployeeExits.Add(obj);
                            _db.SaveChanges();

                            EmployeeExitsHistory histobj = new EmployeeExitsHistory()
                            {
                                DiscussionSummary = "No",
                                ReasonId = model.EmployeeId,
                                Reason = ExitEmpReasonConstants.Other.ToString(),
                                EmployeeId = claims.employeeId,
                                EmployeeName = employee.DisplayName,
                                IsDiscussion = true,
                                CompanyId = claims.companyId,
                                OrgId = claims.orgId,
                                CreatedBy = claims.employeeId,
                                CreatedOn = DateTime.Now,
                                Status = ExitStatusConstants.Pending,
                                IsActive = true,
                                IsDeleted = false,
                                Comment = "No",
                                TerminateDate = DateTime.Now,
                                InProgress = true,
                            };
                            _db.EmployeeExitsHistorys.Add(histobj);
                            _db.SaveChanges();
                        }
                        //if (model.LoginType.HasValue)
                        //{
                        //    if (user.LoginId != model.LoginType)
                        //    {
                        //        user.LoginId = (LoginRolesConstants)model.LoginType;
                        //        user.OrgId = user.LoginId == LoginRolesConstants.Administrator ? 0 : employee.OrgId;

                        //        employee.OrgId = user.LoginId == LoginRolesConstants.Administrator ? 0 : employee.OrgId;
                        //    }
                        //    user.OrgId = model.OrgId;
                        //    _db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        //    _db.SaveChanges();
                        //}

                        //var remove = _db.EmpReportingManagers.Where(x => x.EmployeeId == employee.EmployeeId).ToList();

                        //_db.EmpReportingManagers.RemoveRange(remove);
                        //_db.SaveChanges();
                        //if (model.ReportingManagerId != null)
                        //{
                        //    if (model.ReportingManagerId.Count > 0)
                        //    {
                        //        foreach (var item in model.ReportingManagerId)
                        //        {
                        //            EmployeeReportingManager emp = new EmployeeReportingManager
                        //            {
                        //                EmployeeId = employee.EmployeeId,
                        //                ManagerId = item,
                        //            };
                        //            _db.EmpReportingManagers.Add(emp);
                        //            await _db.SaveChangesAsync();
                        //        }
                        //    }
                        //}

                        res.Message = "Profile Updated";
                        res.Status = true;
                        res.Data = employee;
                    }
                }
                else
                {
                    res.Message = "Employee Not Found";
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
        #endregion Api To Update Profile By Employe Id


        #region Api To Update Employee Profile About (Self Update)

        /// <summary>
        /// Created By Harshit Mitra On 01-05-2022
        /// Modified By Suraj Bundel On 01-11-2022
        /// API >> Put >> api/employeenew/updateemployeeprofileabout
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("updateemployeeprofileabout")]
        public async Task<ResponseBodyModel> UpdateEmployeeProfileAbout(UpdateEmployeeProfileSelf model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employee = await _db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == claims.employeeId);
                if (employee != null)
                {
                    if (model == null)
                    {
                        res.Message = "Invalid Model";
                        res.Status = false;
                        return res;
                    }
                    employee.ProfileImageUrl = String.IsNullOrEmpty(model.ProfileImageUrl) ? employee.ProfileImageUrl : model.ProfileImageUrl;
                    employee.ProfileImageExtension = String.IsNullOrEmpty(model.ProfileImageExtension) ?
                                employee.ProfileImageExtension : model.ProfileImageExtension;
                    employee.FirstName = String.IsNullOrEmpty(model.FirstName) ? employee.FirstName : model.FirstName;
                    //     employee.MiddleName = String.IsNullOrEmpty(model.MiddleName) ? employee.MiddleName : model.MiddleName;
                    employee.MiddleName = model.MiddleName;
                    employee.LastName = String.IsNullOrEmpty(model.LastName) ? employee.LastName : model.LastName;
                    employee.DisplayName = String.IsNullOrEmpty(employee.MiddleName) ?
                                employee.FirstName.Trim() + " " + employee.LastName.Trim() :
                                employee.FirstName.Trim() + " " + employee.MiddleName.Trim() + " " + employee.LastName.Trim();
                    employee.Gender = String.IsNullOrEmpty(model.Gender) ? employee.Gender : model.Gender;
                    employee.DateOfBirth = model.DateOfBirth == null ? employee.DateOfBirth : (DateTime)model.DateOfBirth;
                    employee.HideDOB = model.HideDOB;
                    employee.MaritalStatus = String.IsNullOrEmpty(model.MaritalStatus) ? employee.MaritalStatus : model.MaritalStatus;
                    employee.BloodGroup = model.BloodGroup == null ? employee.BloodGroup : (BloodGroupConstants)model.BloodGroup;
                    employee.IsPhysicallyHandicapped = model.IsPhysicallyHandicapped == null ?
                                employee.IsPhysicallyHandicapped : (bool)model.IsPhysicallyHandicapped;
                    employee.PersonalEmail = String.IsNullOrEmpty(model.PersonalEmail) ?
                                employee.PersonalEmail : model.PersonalEmail;
                    employee.MobilePhone = String.IsNullOrEmpty(model.MobilePhone) ? employee.MobilePhone : model.MobilePhone;
                    employee.WorkPhone = String.IsNullOrEmpty(model.WorkPhone) ? employee.WorkPhone : model.WorkPhone;
                    employee.SkypeMail = String.IsNullOrEmpty(model.SkypeMail) ? employee.SkypeMail : model.SkypeMail;
                    employee.LocalAddress = String.IsNullOrEmpty(model.LocalAddress) ? employee.LocalAddress : model.LocalAddress;
                    employee.LocalCountryId = model.LocalCountryId == null ? employee.LocalCountryId : model.LocalCountryId;
                    employee.LocalStateId = model.LocalStateId == null ? employee.LocalStateId : model.LocalStateId;
                    employee.LocalCityId = model.LocalCityId == null ? employee.LocalCityId : model.LocalCityId;
                    employee.LocalPinCode = model.LocalPinCode == null ? employee.LocalPinCode : model.LocalPinCode;
                    employee.PermanentAddress = String.IsNullOrEmpty(model.PermanentAddress) ?
                                employee.PermanentAddress : model.PermanentAddress;
                    employee.PermanentCountryId = model.PermanentCountryId == null ?
                                employee.PermanentCountryId : model.PermanentCountryId;
                    employee.PermanentStateId = model.PermanentStateId == null ? employee.PermanentStateId : model.PermanentStateId;
                    employee.PermenentCityId = model.PermenentCityId == null ? employee.PermenentCityId : model.PermenentCityId;
                    employee.PermenentPinCode = model.PermenentPinCode == null ? employee.PermenentPinCode : model.PermenentPinCode;
                    employee.ProfessionalSummary = String.IsNullOrEmpty(model.ProfessionalSummary) ?
                                employee.ProfessionalSummary : model.ProfessionalSummary;
                    employee.AboutMeRemark = String.IsNullOrEmpty(model.AboutMeRemark) ? employee.AboutMeRemark : model.AboutMeRemark;
                    employee.AboutMyJobRemark = String.IsNullOrEmpty(model.AboutMyJobRemark) ? employee.AboutMyJobRemark : model.AboutMyJobRemark;
                    employee.InterestAndHobbiesRemark = String.IsNullOrEmpty(model.InterestAndHobbiesRemark) ?
                                employee.InterestAndHobbiesRemark : model.InterestAndHobbiesRemark;

                    _db.Entry(employee).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Profile Updated";
                    res.Status = true;
                    res.Data = employee;
                }
                else
                {
                    res.Message = "Employee Not Found";
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

        #endregion Api To Update Employee Profile About (Self Update)

        #region Api To Get Employee Profile (Self Profile)

        /// <summary>
        /// Created By Harshit Mitra On 02-05-2022
        /// Modified By Suraj Bundel On 01-11-2022
        /// API >> Get >> api/employeenew/getemployeeprofileself
        /// </summary>
        [HttpGet]
        [Route("getemployeeprofileself")]
        public async Task<ResponseBodyModel> GetEmployeeProfile()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            GetEmployeeProfileModel response = new GetEmployeeProfileModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employee = await _db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == claims.employeeId);
                if (employee != null)
                {
                    response.ProfileImageUrl = employee.ProfileImageUrl;

                    response.ProfileImageExtension = employee.ProfileImageExtension;
                    response.EmployeeId = employee.EmployeeId;
                    response.DisplayName = employee.DisplayName;
                    response.Department = _db.Department
                            .Where(x => x.DepartmentId == employee.DepartmentId)
                            .Select(x => x.DepartmentName).FirstOrDefault();
                    response.Designation = _db.Designation
                            .Where(x => x.DesignationId == employee.DesignationId)
                            .Select(x => x.DesignationName).FirstOrDefault();
                    response.BusinessUnit = _db.OrgMaster.Where(x => x.OrgId == employee.OrgId)
                            .Select(x => x.OrgName).FirstOrDefault();
                    response.EmployeeNo = employee.EmployeeCode;
                    response.ProfessionalSummary = employee.ProfessionalSummary;
                    response.IsOnExit = await _db.EmployeeExits.AnyAsync(x => x.EmployeeId == claims.employeeId && x.Status != ExitStatusConstants.Retain);
                    response.PersonalInfo = new EmployeeProfilePersonalInfo
                    {
                        FirstName = employee.FirstName,
                        MiddleName = employee.MiddleName,
                        LastName = employee.LastName,
                        Gender = employee.Gender,
                        DateOfBirth = employee.DateOfBirth,
                        HideDOB = employee.HideDOB,
                        MaritalStatus = employee.MaritalStatus,
                        MedicalIssue = employee.MedicalIssue,
                        BloodGroup = employee.BloodGroup,
                        BloodGroupName = Enum.GetName(typeof(BloodGroupConstants), employee.BloodGroup).Contains("_pos") ?
                                         Enum.GetName(typeof(BloodGroupConstants), employee.BloodGroup).Replace("_pos", "+") :
                                         Enum.GetName(typeof(BloodGroupConstants), employee.BloodGroup).Replace("_neg", "-"),
                        IsPhysicallyHandicapped = employee.IsPhysicallyHandicapped,
                    };
                    response.ConatactDetails = new EmployeeConatactDetails
                    {
                        WorkEmail = employee.OfficeEmail,
                        PersonalEmail = employee.PersonalEmail,
                        MobilePhone = employee.MobilePhone,
                        WorkPhone = employee.WorkPhone,
                        SkypeMail = employee.SkypeMail,
                    };
                    response.AddressDetails = new EmployeeAddressDetails
                    {
                        LocalAddress = employee.LocalAddress,
                        LocalCountryId = employee.LocalCountryId,
                        LocalStateId = employee.LocalStateId,
                        LocalCityId = employee.LocalCityId,
                        LocalPinCode = employee.LocalPinCode,
                        LocalCountryName = employee.LocalCountryId == null ? null : _db.Country.Where(x => x.CountryId == (int)employee.LocalCountryId).Select(x => x.CountryName).FirstOrDefault(),
                        LocalStateName = employee.LocalStateId == null ? null : _db.State.Where(x => x.StateId == (int)employee.LocalStateId).Select(x => x.StateName).FirstOrDefault(),
                        LocalCityName = employee.LocalCityId == null ? null : _db.City.Where(x => x.CityId == (int)employee.LocalCityId).Select(x => x.CityName).FirstOrDefault(),
                        PermanentAddress = employee.PermanentAddress,
                        PermanentCountryId = employee.PermanentCountryId,
                        PermanentStateId = employee.PermanentStateId,
                        PermenentCityId = employee.PermenentCityId,
                        PermenentPinCode = employee.PermenentPinCode,
                    };
                    response.RemarkDetails = new EmployeeRemarkDetails
                    {
                        AboutMeRemark = employee.AboutMeRemark,
                        AboutMyJobRemark = employee.AboutMyJobRemark,
                        InterestAndHobbiesRemark = employee.InterestAndHobbiesRemark,
                    };

                    res.Message = employee.DisplayName + " Profile";
                    res.Status = true;
                    res.Data = response;
                }
                else
                {
                    res.Message = "Employee Profile";
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

        #endregion Api To Get Employee Profile (Self Profile)

        #region Api To Get Employee Profile By Id

        /// <summary>
        /// Created By Harshit Mitra On 03-05-2022
        /// Modified By Suraj Bundel On 01-11-2022
        /// API >> Get >> api/employeenew/getemployeeprofilebyid
        /// </summary>
        [HttpGet]
        [Route("getemployeeprofilebyid")]
        public async Task<ResponseBodyModel> GetEmployeeProfileById(int employeeId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            GetEmployeeProfileModel response = new GetEmployeeProfileModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employee = await _db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
                if (employee != null)
                {
                    response.ProfileImageUrl = employee.ProfileImageUrl;
                    response.ProfileImageExtension = employee.ProfileImageExtension;
                    response.EmployeeId = employee.EmployeeId;
                    response.DisplayName = employee.DisplayName;
                    response.Department = _db.Department
                            .Where(x => x.DepartmentId == employee.DepartmentId)
                            .Select(x => x.DepartmentName).FirstOrDefault();
                    response.Designation = _db.Designation
                            .Where(x => x.DesignationId == employee.DesignationId)
                            .Select(x => x.DesignationName).FirstOrDefault();
                    response.BusinessUnit = _db.OrgMaster.Where(x => x.OrgId == employee.OrgId)
                            .Select(x => x.OrgName).FirstOrDefault();
                    response.EmployeeNo = employee.EmployeeCode;
                    response.ProfessionalSummary = employee.ProfessionalSummary;
                    response.IsOnExit = await _db.EmployeeExits.AnyAsync(x => x.EmployeeId == employeeId && x.Status != ExitStatusConstants.Retain);
                    response.PersonalInfo = new EmployeeProfilePersonalInfo
                    {
                        FirstName = employee.FirstName,
                        MiddleName = employee.MiddleName,
                        LastName = employee.LastName,
                        Gender = employee.Gender,
                        DateOfBirth = employee.DateOfBirth,
                        HideDOB = employee.HideDOB,
                        MaritalStatus = employee.MaritalStatus,
                        MedicalIssue = employee.MedicalIssue,
                        BloodGroup = employee.BloodGroup,
                        BloodGroupName = Enum.GetName(typeof(BloodGroupConstants), employee.BloodGroup).Contains("_pos") ?
                                         Enum.GetName(typeof(BloodGroupConstants), employee.BloodGroup).Replace("_pos", "+") :
                                         Enum.GetName(typeof(BloodGroupConstants), employee.BloodGroup).Replace("_neg", "-"),
                        IsPhysicallyHandicapped = employee.IsPhysicallyHandicapped,
                    };
                    response.ConatactDetails = new EmployeeConatactDetails
                    {
                        WorkEmail = employee.OfficeEmail,
                        PersonalEmail = employee.PersonalEmail,
                        MobilePhone = employee.MobilePhone,
                        WorkPhone = employee.WorkPhone,
                        SkypeMail = employee.SkypeMail,
                    };
                    response.AddressDetails = new EmployeeAddressDetails
                    {
                        LocalAddress = employee.LocalAddress,
                        LocalCountryId = employee.LocalCountryId,
                        LocalStateId = employee.LocalStateId,
                        LocalCityId = employee.LocalCityId,
                        LocalPinCode = employee.LocalPinCode,
                        LocalCountryName = employee.LocalCountryId == null ? null : _db.Country.Where(x => x.CountryId == (int)employee.LocalCountryId).Select(x => x.CountryName).FirstOrDefault(),
                        LocalStateName = employee.LocalStateId == null ? null : _db.State.Where(x => x.StateId == (int)employee.LocalStateId).Select(x => x.StateName).FirstOrDefault(),
                        LocalCityName = employee.LocalCityId == null ? null : _db.City.Where(x => x.CityId == (int)employee.LocalCityId).Select(x => x.CityName).FirstOrDefault(),
                        PermanentAddress = employee.PermanentAddress,
                        PermanentCountryId = employee.PermanentCountryId,
                        PermanentStateId = employee.PermanentStateId,
                        PermenentCityId = employee.PermenentCityId,
                        PermenentPinCode = employee.PermenentPinCode,
                    };
                    response.RemarkDetails = new EmployeeRemarkDetails
                    {
                        AboutMeRemark = employee.AboutMeRemark,
                        AboutMyJobRemark = employee.AboutMyJobRemark,
                        InterestAndHobbiesRemark = employee.InterestAndHobbiesRemark,
                    };

                    res.Message = employee.DisplayName + " Profile";
                    res.Status = true;
                    res.Data = response;
                }
                else
                {
                    res.Message = "Employee Profile";
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

        #endregion Api To Get Employee Profile By Id

        #region Api To Update Employee Profile Image

        /// <summary>
        /// Created By Harshit Mitra On 02-05-2022
        /// API >> Post >> api/employeenew/uploademployeeprofileimage
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploademployeeprofileimage")]
        public async Task<UploadImageResponse> UploadEmployeeProfileImage()
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
                        if (extemtionType == "image")
                        {
                            string extension = Path.GetExtension(filename);
                            string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                            byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                            //f.byteArray = buffer;
                            string mime = filefromreq.Headers.ContentType.ToString();
                            Stream stream = new MemoryStream(buffer);
                            var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/userimages/" + claims.employeeId), dates + '.' + filename);
                            string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                            //for create new Folder
                            DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                            if (!objDirectory.Exists)
                            {
                                Directory.CreateDirectory(DirectoryURL);
                            }
                            //string path = "UploadImages\\" + compid + "\\" + filename;

                            string path = "uploadimage\\userimages\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

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
                        result.Message = "You Pass 0 Content";
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

        #endregion Api To Update Employee Profile Image

        #region This Api is use to Get User Profile Data

        /// <summary>
        /// This api created by ankit 30/04/2022
        /// Modified By Suraj Bundel On 01-11-2022
        /// </summary>Api >> Get >> api/employeenew/getuserData
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("getuserData")]
        public async Task<ResponseBodyModel> GetUserData()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var UserData = await (from e in _db.Employee
                                      join c in _db.Company on e.CompanyId equals c.CompanyId
                                      join d in _db.Department on e.CompanyId equals d.CompanyId
                                      join de in _db.Designation on e.CompanyId equals de.CompanyId
                                      join o in _db.OrgMaster on e.CompanyId equals o.CompanyId
                                      where e.IsDeleted == false && e.IsActive == true && e.EmployeeId == claims.employeeId
                                      select new
                                      {
                                          e.EmployeeId,
                                          e.FirstName,
                                          e.LastName,
                                          e.DisplayName,
                                          c.PhoneNumber,
                                          e.MobilePhone,
                                          e.PersonalEmail,
                                          c.RegisterEmail,
                                          c.RegisterAddress,
                                          c.RegisterCompanyName,
                                          e.MaritalStatus,
                                          e.DateOfBirth,
                                          //e.HideDOB,
                                          e.Gender,
                                          e.Password,
                                          o.OrgName,
                                          d.DepartmentName,
                                          de.DesignationName,
                                          e.ProfileImageUrl,
                                      }).FirstOrDefaultAsync();

                if (UserData != null)
                {
                    res.Message = "UserData Data  Found";
                    res.Status = true;
                    res.Data = UserData;
                }
                else
                {
                    res.Message = "UserData Data Not Found";
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

        #endregion This Api is use to Get User Profile Data

        #region This Api is used to Upload user Profile

        /// <summary>
        /// Created By Ankit 30/04/2022
        /// Api>> Post>> api/employeenew/UploadPrimage
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UploadPrimage")]
        public async Task<ResponseBodyModel> UploadImageProfile(string ImageUrl)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var EmployeeData = await _db.Employee.Where(x => x.EmployeeId == claims.employeeId && x.IsDeleted == false).FirstOrDefaultAsync();
                if (EmployeeData != null)
                {
                    EmployeeData.ProfileImageUrl = ImageUrl;
                    EmployeeData.UpdatedOn = DateTime.Now;
                    _db.Entry(EmployeeData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    res.Status = true;
                    res.Message = "Profile Image Added Successfully";
                    res.Data = ImageUrl;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Profile Image not Updated.";
                    res.Data = ImageUrl;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api is used to Upload user Profile

        #region Api To Get Login Role

        /// <summary>
        /// Created By Harshit Mitra On 05-04-2022
        /// API >> Get >> api/employeenew/getloginrole
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("getloginrole")]
        public async Task<GetUserLoginRoleResponse> GetUserLoginRole()
        {
            GetUserLoginRoleResponse res = new GetUserLoginRoleResponse();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var user = await _db.User.FirstOrDefaultAsync(x => x.UserId == claims.userId);
                if (user != null)
                {
                    if (user.DepartmentId != 0)
                    {
                        var employee = await _db.Employee.FirstOrDefaultAsync(x =>
                                x.EmployeeId == user.EmployeeId);
                        if (employee != null)
                        {
                            var loginRole = _db.Department.Where(x => x.DepartmentId == employee.DepartmentId)
                                    .Select(x => x.DepartmentName).FirstOrDefault();
                            res.RoleType = loginRole;
                            res.DisplayName = employee.DisplayName;
                            res.ProfileImageUrl = String.IsNullOrEmpty(employee.ProfileImageUrl) ? null : employee.ProfileImageUrl;
                        }
                        else
                        {
                            res.Message = "INVALID";
                            res.Status = false;
                            res.RoleType = "INVALID";
                        }
                    }
                    else
                    {
                        var loginRole = "SuperAdmin";
                        res.RoleType = loginRole;
                    }
                    res.Message = "Login Role";
                    res.Status = true;
                }
                else
                {
                    res.Message = "INVALID";
                    res.Status = false;
                    res.RoleType = "INVALID";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
                res.RoleType = "INVALID";
            }
            return res;
        }

        #endregion Api To Get Login Role

        #region Api To Update Employee Code And Employee Salery By Import

        /// <summary>
        /// Created By Harshit Mitra on 21-06-2022
        /// API >> Put >> api/employeenew/updateemployeeexcel
        /// </summary>
        /// <param name="item"></param>
        [HttpPut]
        [Route("updateemployeeexcel")]
        public async Task<ResponseBodyModel> UpdateEmployeeExcel(List<UpdateEmployeeCodeAndSaleryModel> item)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            List<UpdateEmployeeCodeAndSaleryModel> faultyEmployee = new List<UpdateEmployeeCodeAndSaleryModel>();
            try
            {
                var OrgList = await _db.OrgMaster.Where(x => x.CompanyId == claims.companyId && x.IsActive && !x.IsDeleted).ToListAsync();
                var departmentList = await _db.Department.Where(x => x.CompanyId == claims.companyId).ToListAsync();
                var desingationList = await _db.Designation.Where(x => x.CompanyId == claims.companyId).ToListAsync();
                var empType = Enum.GetValues(typeof(LoginRolesConstants))
                                    .Cast<LoginRolesConstants>()
                                    .Where(x => x != LoginRolesConstants.SuperAdmin)
                                    .Select(x => new
                                    {
                                        EnumValue = x,
                                        EmployeeTypeId = (int)x,
                                        EmployeeTypeName = Enum.GetName(typeof(LoginRolesConstants), x).Replace("_", " "),
                                    }).ToList();
                List<string> notadded = new List<string>();
                if (item == null)
                {
                    res.Message = "Error";
                    res.Status = false;
                    return res;
                }
                else
                {
                    foreach (var model in item)
                    {
                        var departmentId = departmentList.Where(x => x.DepartmentName.Trim() == model.DepartmentName.Trim()).Select(x => x.DepartmentId).FirstOrDefault();
                        var designationId = desingationList.Where(x => x.DepartmentId == departmentId && x.DesignationName.Trim() == model.DesignationName.Trim()).Select(x => x.DesignationId).FirstOrDefault();
                        if (claims.orgId == 0)
                            if (!String.IsNullOrEmpty(model.OrganizationName) && !String.IsNullOrWhiteSpace(model.OrganizationName))
                            {
                                claims.orgId = OrgList.Where(x => x.OrgName == model.OrganizationName).Select(x => x.OrgId).FirstOrDefault();
                                empType.RemoveAll(x => x.EnumValue == LoginRolesConstants.Administrator);
                            }

                        if (claims.orgId != 0 && designationId != 0 && departmentId != 0)
                        {
                            var empObj = await _db.Employee.FirstOrDefaultAsync(x => x.OfficeEmail == model.OfficeEmail);
                            if (empObj != null)
                            {
                                var user = await _db.User.Where(x => x.EmployeeId == empObj.EmployeeId).FirstOrDefaultAsync();
                                if (user != null)
                                {
                                    if (!String.IsNullOrEmpty(model.LoginType) && !String.IsNullOrWhiteSpace(model.LoginType))
                                    {
                                        var loginTypeEnum = empType.Where(x => x.EmployeeTypeName == model.LoginType).FirstOrDefault();
                                        if (loginTypeEnum == null)
                                        {
                                            notadded.Add(model.OfficeEmail);
                                            continue;
                                        }
                                        else
                                        {
                                            user.LoginId = loginTypeEnum.EnumValue;
                                            _db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                                        }
                                    }
                                    try
                                    {
                                        var firstName = String.IsNullOrEmpty(model.FirstName) ? empObj.FirstName : model.FirstName;
                                        var middleName = String.IsNullOrEmpty(model.MiddleName) ? (String.IsNullOrEmpty(empObj.MiddleName) ? "" : empObj.FirstName) : model.MiddleName;
                                        var lastName = String.IsNullOrEmpty(model.LastName) ? empObj.LastName : model.LastName; ;

                                        empObj.FirstName = firstName;
                                        empObj.MiddleName = middleName;
                                        empObj.LastName = lastName;
                                        empObj.DisplayName = String.IsNullOrEmpty(middleName) ? firstName + " " + lastName :
                                            firstName + " " + middleName + " " + lastName;
                                        empObj.FatherName = String.IsNullOrEmpty(model.FatherName) ? empObj.FatherName : model.FatherName;
                                        empObj.MotherName = String.IsNullOrEmpty(model.MotherName) ? empObj.MotherName : model.MotherName;
                                        empObj.Gender = String.IsNullOrEmpty(model.Gender) ? empObj.Gender : model.Gender;

                                        empObj.DateOfBirth = model.DateOfBirth.Date == empObj.DateOfBirth.Date ? empObj.DateOfBirth.Date : model.DateOfBirth.Date.AddDays(1);

                                        empObj.BloodGroup = String.IsNullOrEmpty(model.BloodGroup) ? empObj.BloodGroup : (
                                                    (BloodGroupConstants)System.Enum.Parse(typeof(BloodGroupConstants),
                                                    (model.BloodGroup.Contains("+") ? model.BloodGroup.Replace("+", "_pos") :
                                                    model.BloodGroup.Replace("-", "_neg"))));
                                        empObj.MaritalStatus = String.IsNullOrEmpty(model.MaritalStatus) ? empObj.MaritalStatus : model.MaritalStatus;

                                        if (model.ConfirmationDate.HasValue)
                                            empObj.ConfirmationDate = ((DateTime)model.ConfirmationDate).Date.AddDays(1);

                                        empObj.JoiningDate = model.JoiningDate.Date == empObj.JoiningDate.Date ? empObj.JoiningDate.Date : model.JoiningDate.Date.AddDays(1);

                                        empObj.UpdatedBy = claims.employeeId;
                                        empObj.UpdatedOn = DateTime.Now;
                                        empObj.DepartmentId = departmentId;
                                        empObj.DesignationId = designationId;
                                        empObj.AadharNumber = String.IsNullOrEmpty(model.AadharNumber) ? empObj.AadharNumber : model.AadharNumber;
                                        empObj.EmployeeTypeId = (EmployeeTypeConstants)(String.IsNullOrEmpty(model.EmployeeType) ? empObj.EmployeeTypeId : model.EmployeeType == null ?
                                                EmployeeTypeConstants.Confirmed_Employee :
                                                Enum.Parse(typeof(EmployeeTypeConstants), model.EmployeeType.Replace(" ", "_")));

                                        empObj.EmergencyNumber = String.IsNullOrEmpty(model.EmergencyNumber) ? empObj.EmergencyNumber : model.EmergencyNumber;
                                        empObj.WhatsappNumber = String.IsNullOrEmpty(model.WhatsappNumber) ? empObj.WhatsappNumber : model.WhatsappNumber;
                                        empObj.MobilePhone = String.IsNullOrEmpty(model.MobilePhone) ? empObj.MobilePhone : model.MobilePhone;
                                        empObj.PanNumber = String.IsNullOrEmpty(model.PanNumber) ? empObj.PanNumber : model.PanNumber;
                                        empObj.PersonalEmail = String.IsNullOrEmpty(model.PersonalEmail) ? empObj.PersonalEmail : model.PersonalEmail;
                                        empObj.PermanentAddress = String.IsNullOrEmpty(model.PermanentAddress) ? empObj.PermanentAddress : model.PermanentAddress;
                                        empObj.LocalAddress = String.IsNullOrEmpty(model.LocalAddress) ? empObj.LocalAddress : model.LocalAddress;
                                        empObj.BankAccountNumber = String.IsNullOrEmpty(model.BankAccountNumber) ? empObj.BankAccountNumber : model.BankAccountNumber;
                                        empObj.IFSC = String.IsNullOrEmpty(model.IFSC) ? empObj.IFSC : model.IFSC;
                                        empObj.AccountHolderName = String.IsNullOrEmpty(model.AccountHolderName) ? empObj.AccountHolderName : model.AccountHolderName;
                                        empObj.BankName = String.IsNullOrEmpty(model.BankName) ? empObj.BankName : model.BankName;
                                        empObj.GrossSalery = model.Salary == 0 ? empObj.GrossSalery : model.Salary;
                                        empObj.EmployeeCode = String.IsNullOrEmpty(model.EmployeeCode) ? empObj.EmployeeCode : model.EmployeeCode;
                                        empObj.OrgId = claims.orgId;
                                        //empObj.ReportingManager = model.
                                        if (!String.IsNullOrEmpty(model.ReportingManager) && !String.IsNullOrWhiteSpace(model.ReportingManager))
                                        {
                                            var reportingManager = _db.Employee.Where(x => x.OfficeEmail == model.ReportingManager).FirstOrDefault();
                                            empObj.ReportingManager = reportingManager != null ? reportingManager.EmployeeId : 0;
                                        }

                                        _db.Entry(empObj).State = System.Data.Entity.EntityState.Modified;
                                        await _db.SaveChangesAsync();
                                    }
                                    catch (Exception)
                                    {
                                        notadded.Add(model.OfficeEmail);
                                    }
                                }
                                else
                                {
                                    notadded.Add(model.OfficeEmail);
                                }
                            }
                            else
                            {
                                notadded.Add(model.OfficeEmail);
                            }
                        }
                        else
                        {
                            notadded.Add(model.OfficeEmail);
                        }
                    }
                    var data = String.Join(",", notadded);
                    res.Message = "Updated";
                    res.Status = true;
                    res.Data = data;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Update Employee Code And Employee Salery By Import

        // docment section start from here

        #region This api use for upload documents for employee document

        /// <summary>
        /// Created By Harshit Mitra On 09-05-2022
        /// Changes by shriya on 30-04-2022
        /// API >> Post >> api/employeenew/uploademployeedocuments
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploademployeedocuments")]
        public async Task<UploadImageResponse> UploadEmployeeDocments()
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

                        string extension = Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        //f.byteArray = buffer;
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/userdocuments/" + claims.employeeId), dates + '.' + filename);
                        string DirectoryURL = (FileUrl.Split(new string[] { claims.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.employeeId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        string path = "uploadimage\\userdocuments\\" + claims.employeeId + "\\" + dates + '.' + Fileresult + extension;

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
                        result.Message = "You Pass 0 Content";
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

        #endregion This api use for upload documents for employee document

        #region This Api used To Add DocumentDtails

        /// <summary>
        /// created by Ankit 11/05/2022
        /// </summary>route api/employeenew/addDocumentDtails
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("addDocumentDtails")]
        public async Task<ResponseBodyModel> AddDocumentDtails(EmployeeDocument model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (model != null)
                {
                    var employee = _db.EmpDoc.Where(x => x.EmployeeId == claims.employeeId && x.IsActive == true
                               && x.IsDeleted == false).FirstOrDefault();
                    if (employee == null)
                    {
                        EmployeeDocument obj = new EmployeeDocument();
                        obj.EmployeeId = claims.employeeId;
                        obj.CompanyId = claims.companyId;
                        obj.OrgId = claims.orgId;
                        obj.IsActive = true;
                        obj.IsDeleted = false;
                        obj.CreatedBy = claims.employeeId;
                        obj.CreatedOn = DateTime.Now;
                        _db.EmpDoc.Add(obj);
                        await _db.SaveChangesAsync();
                    }

                    var empDoc = _db.EmpDoc.Where(x => x.EmployeeId == claims.employeeId && x.IsActive == true
                               && x.IsDeleted == false).FirstOrDefault();

                    empDoc.Degree = String.IsNullOrEmpty(model.Degree) ? empDoc.Degree : model.Degree;
                    empDoc.Branch = String.IsNullOrEmpty(model.Branch) ? empDoc.Branch : model.Branch;
                    empDoc.DateOfJoining = model.DateOfJoining == null ? empDoc.DateOfJoining : model.DateOfJoining;
                    empDoc.DateOfCompleation = model.DateOfCompleation == null ? empDoc.DateOfCompleation : model.DateOfCompleation;
                    empDoc.PerctOrCGPA = String.IsNullOrEmpty(model.PerctOrCGPA) ? empDoc.PerctOrCGPA : model.PerctOrCGPA;
                    empDoc.UniversityOrCollage = String.IsNullOrEmpty(model.UniversityOrCollage) ? empDoc.UniversityOrCollage : model.UniversityOrCollage;
                    empDoc.DegreeUpload = String.IsNullOrEmpty(model.DegreeUpload) ? empDoc.DegreeUpload : model.DegreeUpload;
                    /// For Pan Card
                    empDoc.PanNumber = String.IsNullOrEmpty(model.PanNumber) ? empDoc.PanNumber : model.PanNumber;
                    empDoc.NameOnPan = String.IsNullOrEmpty(model.NameOnPan) ? empDoc.NameOnPan : model.NameOnPan;
                    empDoc.DateOfBirthDateOnPan = model.DateOfBirthDateOnPan == null ? empDoc.DateOfBirthDateOnPan : model.DateOfBirthDateOnPan;
                    empDoc.FatherNameOnPan = String.IsNullOrEmpty(model.FatherNameOnPan) ? empDoc.FatherNameOnPan : model.FatherNameOnPan;
                    empDoc.PanUpload = String.IsNullOrEmpty(model.PanUpload) ? empDoc.PanUpload : model.PanUpload;
                    /// For Aadhaar
                    empDoc.AadhaarCardNumber = String.IsNullOrEmpty(model.AadhaarCardNumber) ? empDoc.AadhaarCardNumber : model.AadhaarCardNumber;
                    empDoc.DateOfBirthOnAadhaar = model.DateOfBirthOnAadhaar == null ? empDoc.DateOfBirthOnAadhaar : model.DateOfBirthOnAadhaar;
                    empDoc.FatherHusbandNameOnAadhaar = String.IsNullOrEmpty(model.FatherHusbandNameOnAadhaar) ? empDoc.FatherHusbandNameOnAadhaar : model.FatherHusbandNameOnAadhaar;
                    empDoc.AddressOnAadhaar = String.IsNullOrEmpty(model.AddressOnAadhaar) ? empDoc.AddressOnAadhaar : model.AddressOnAadhaar;
                    empDoc.AadhaarUpload = String.IsNullOrEmpty(model.AadhaarUpload) ? empDoc.AadhaarUpload : model.AadhaarUpload;
                    empDoc.NameOnAadhaar = String.IsNullOrEmpty(model.NameOnAadhaar) ? empDoc.NameOnAadhaar : model.NameOnAadhaar;

                    /// For Voter Id
                    empDoc.VoterIdNumber = String.IsNullOrEmpty(model.VoterIdNumber) ? empDoc.VoterIdNumber : model.VoterIdNumber;
                    empDoc.DateOfBirthOnVoterId = model.DateOfBirthOnVoterId == null ? empDoc.DateOfBirthOnVoterId : model.DateOfBirthOnVoterId;
                    empDoc.NameOnVoterId = String.IsNullOrEmpty(model.NameOnVoterId) ? empDoc.NameOnVoterId : model.NameOnVoterId;
                    empDoc.FatherHusbandNameOnVoter = String.IsNullOrEmpty(model.FatherHusbandNameOnVoter) ? empDoc.FatherHusbandNameOnVoter : model.FatherHusbandNameOnVoter;
                    empDoc.VoterUpload = String.IsNullOrEmpty(model.VoterUpload) ? empDoc.VoterUpload : model.VoterUpload;
                    /// For Driving License
                    empDoc.Licensenumber = String.IsNullOrEmpty(model.Licensenumber) ? empDoc.Licensenumber : model.Licensenumber;
                    empDoc.DateOfBirthOnDriving = model.DateOfBirthOnDriving == null ? empDoc.DateOfBirthOnDriving : model.DateOfBirthOnDriving;
                    empDoc.NameOnDriving = String.IsNullOrEmpty(model.NameOnDriving) ? empDoc.NameOnDriving : model.NameOnDriving;
                    empDoc.FatherHusbandNameOnDriving = String.IsNullOrEmpty(model.FatherHusbandNameOnDriving) ? empDoc.FatherHusbandNameOnDriving : model.FatherHusbandNameOnDriving;
                    empDoc.ExpireOnLicense = model.ExpireOnLicense == null ? empDoc.ExpireOnLicense : model.ExpireOnLicense;
                    empDoc.DrivingLicenseUpload = String.IsNullOrEmpty(model.DrivingLicenseUpload) ? empDoc.DrivingLicenseUpload : model.DrivingLicenseUpload;
                    /// For Passport
                    empDoc.PassportNumber = String.IsNullOrEmpty(model.PassportNumber) ? empDoc.PassportNumber : model.PassportNumber;
                    empDoc.DateOfBirth = model.DateOfBirth == null ? empDoc.DateOfBirth : model.DateOfBirth;
                    empDoc.FullName = String.IsNullOrEmpty(model.FullName) ? empDoc.FullName : model.FullName;
                    empDoc.FatherName = String.IsNullOrEmpty(model.FatherName) ? empDoc.FatherName : model.FatherName;
                    empDoc.DateOfIssue = model.DateOfIssue == null ? empDoc.DateOfIssue : model.DateOfIssue;
                    empDoc.PlaceOfIssue = String.IsNullOrEmpty(model.PlaceOfIssue) ? empDoc.PlaceOfIssue : model.PlaceOfIssue;
                    empDoc.PlaceOfBirth = String.IsNullOrEmpty(model.PlaceOfBirth) ? empDoc.PlaceOfBirth : model.PlaceOfBirth;
                    empDoc.ExpiresOn = model.ExpiresOn == null ? empDoc.ExpiresOn : model.ExpiresOn;
                    empDoc.Address = String.IsNullOrEmpty(model.Address) ? empDoc.Address : model.Address;
                    empDoc.PassportUpload = String.IsNullOrEmpty(model.PassportUpload) ? empDoc.PassportUpload : model.PassportUpload;
                    /// For Previous Experience
                    empDoc.CompanyName = String.IsNullOrEmpty(model.CompanyName) ? empDoc.CompanyName : model.CompanyName;
                    empDoc.JobTitle = String.IsNullOrEmpty(model.JobTitle) ? empDoc.JobTitle : model.JobTitle;
                    empDoc.JoiningDateExperience = model.JoiningDateExperience == null ? empDoc.JoiningDateExperience : model.JoiningDateExperience;
                    empDoc.RelievoingDateExperience = model.RelievoingDateExperience == null ? empDoc.RelievoingDateExperience : model.RelievoingDateExperience;
                    empDoc.LocationExperience = String.IsNullOrEmpty(model.LocationExperience) ? empDoc.LocationExperience : model.LocationExperience;
                    empDoc.DescriptionExperience = String.IsNullOrEmpty(model.DescriptionExperience) ? empDoc.DescriptionExperience : model.DescriptionExperience;
                    empDoc.ExperienceUpload = String.IsNullOrEmpty(model.ExperienceUpload) ? empDoc.ExperienceUpload : model.ExperienceUpload;
                    /// For Pay Slip
                    empDoc.PaySlipsUpload = String.IsNullOrEmpty(model.PaySlipsUpload) ? empDoc.PaySlipsUpload : model.PaySlipsUpload;
                    ///  Signature
                    empDoc.SignatureUpload = String.IsNullOrEmpty(model.SignatureUpload) ? empDoc.SignatureUpload : model.SignatureUpload;
                    empDoc.Name = String.IsNullOrEmpty(model.Name) ? empDoc.CompanyName : model.Name;
                    empDoc.Designation = String.IsNullOrEmpty(model.Designation) ? empDoc.Designation : model.Designation;
                    empDoc.Checked = true;

                    empDoc.UpdatedBy = claims.employeeId;
                    empDoc.UpdatedOn = DateTime.Now;
                    _db.Entry(empDoc).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "DocumentDtails Added";
                    res.Status = true;
                    res.Data = empDoc;
                }
                else
                {
                    res.Message = "Model Is Empty";
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

        #endregion This Api used To Add DocumentDtails

        #region This Api is used to Upload user Profile

        /// <summary>
        /// Created By shriya 30/04/2022
        /// </summary>api/employeenew/UploadPrimage
        [HttpPost]
        [Authorize]
        [Route("uploaddocumentpath")]
        public async Task<ResponseBodyModel> UploadDocmentPath(string ImageUrl)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var EmployeeData = await _db.Employee.Where(x => x.EmployeeId == claims.employeeId && x.IsDeleted == false).FirstOrDefaultAsync();
                if (EmployeeData != null)
                {
                    EmployeeData.ProfileImageUrl = ImageUrl;
                    EmployeeData.UpdatedOn = DateTime.Now;
                    _db.Entry(EmployeeData).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    res.Status = true;
                    res.Message = "Profile Image Addeed Successfully";
                    res.Data = ImageUrl;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Profile Image not Updated.";
                    res.Data = ImageUrl;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api is used to Upload user Profile

        #region This Api is used to Get previous experience detail

        ///<summary>
        ///create by shriya craete on 09-05-2022
        ///</summary>
        [HttpGet]
        [Authorize]
        [Route("getpreviousexpdetail")]
        public async Task<ResponseBodyModel> GetPreviousExpDetail()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                List<GetEmpPreviousExp> getEmpPreviousExps = new List<GetEmpPreviousExp>();
                var expdetail = await _db.EmpDoc.Where(x => x.EmployeeId == claims.employeeId && x.IsDeleted == false && x.IsActive == true).FirstOrDefaultAsync();
                if (expdetail != null)
                {
                    GetEmpPreviousExp exp = new GetEmpPreviousExp();
                    exp.EmployeeDocId = expdetail.EmployeeDocId;
                    exp.CompanyName = expdetail.CompanyName;
                    exp.JobTitle = expdetail.JobTitle;
                    exp.JoiningDateExperience = expdetail.JoiningDateExperience;
                    exp.RelievoingDateExperience = expdetail.RelievoingDateExperience;
                    exp.LocationExperience = expdetail.LocationExperience;
                    exp.DescriptionExperience = expdetail.DescriptionExperience;
                    exp.ExperienceUpload = expdetail.ExperienceUpload;
                    getEmpPreviousExps.Add(exp);
                    res.Status = true;
                    res.Message = "get previous experience detail";
                    res.Data = getEmpPreviousExps;
                }
                else
                {
                    res.Status = false;
                    res.Message = "detail not get";
                    res.Data = getEmpPreviousExps;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api is used to Get previous experience detail

        #region Api To Get Employee Document

        /// <summary>
        ///This api used to show  all the document details
        ///API >> Get >> api/employeenew/getempdocument
        ///Created By shriya
        ///Created on 14-05-2022
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("getempdocument")]
        public async Task<ResponseBodyModel> GetEmployeeDocument()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            EmployeeDocumentModel response = new EmployeeDocumentModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var empDocumentData = await _db.EmpDoc.FirstOrDefaultAsync(x => x.EmployeeId == claims.employeeId &&
                            x.CompanyId == claims.companyId && x.OrgId == claims.orgId);

                if (empDocumentData != null)
                {
                    // document
                    if (empDocumentData.Degree != null)
                    {
                        DocumentDetailsHelper documentdetail = new DocumentDetailsHelper();
                        documentdetail.Branch = empDocumentData.Branch;
                        documentdetail.Degree = empDocumentData.Degree;
                        documentdetail.DateOfJoining = empDocumentData.DateOfJoining;
                        documentdetail.DateOfCompleation = empDocumentData.DateOfCompleation;
                        documentdetail.PerctOrCGPA = empDocumentData.PerctOrCGPA;
                        documentdetail.UniversityOrCollage = empDocumentData.UniversityOrCollage;
                        documentdetail.DegreeUpload = empDocumentData.DegreeUpload;
                        documentdetail.Checked = empDocumentData.Checked;
                        response.DocumentDetails = documentdetail;
                    }

                    // License
                    if (empDocumentData.Licensenumber != null)
                    {
                        DocumentDetailsDrivingLicenseHelper drivingL = new DocumentDetailsDrivingLicenseHelper();
                        drivingL.Licensenumber = empDocumentData.Licensenumber;
                        drivingL.DateOfBirthOnDriving = empDocumentData.DateOfBirthOnDriving;
                        drivingL.NameOnDriving = empDocumentData.NameOnDriving;
                        drivingL.FatherHusbandNameOnDriving = empDocumentData.FatherHusbandNameOnDriving;
                        drivingL.ExpireOnLicense = empDocumentData.ExpireOnLicense;
                        drivingL.DrivingLicenseUpload = empDocumentData.DrivingLicenseUpload;
                        drivingL.Checked = empDocumentData.Checked;

                        response.DrivingLicenseDetails = drivingL;
                    }

                    // pan
                    if (empDocumentData.PanNumber != null)
                    {
                        DocumentPANCardHelper pandetail = new DocumentPANCardHelper();
                        pandetail.PanNumber = empDocumentData.PanNumber;
                        pandetail.NameOnPan = empDocumentData.NameOnPan;
                        pandetail.FatherNameOnPan = empDocumentData.FatherNameOnPan;
                        pandetail.DateOfBirthDateOnPan = empDocumentData.DateOfBirthDateOnPan;
                        pandetail.PanUpload = empDocumentData.PanUpload;
                        pandetail.Checked = empDocumentData.Checked;
                        response.PanDetails = pandetail;
                    }

                    //passport
                    if (empDocumentData.PassportNumber != null)
                    {
                        DocumentPassportHelper pass = new DocumentPassportHelper();
                        pass.FullName = empDocumentData.FullName;
                        pass.FatherName = empDocumentData.FatherName;
                        pass.DateOfBirth = empDocumentData.DateOfBirth;
                        pass.DateOfIssue = empDocumentData.DateOfIssue;
                        pass.ExpiresOn = empDocumentData.ExpiresOn;
                        pass.PlaceofBirth = empDocumentData.PlaceOfBirth;
                        pass.PlaceOfIssue = empDocumentData.PlaceOfIssue;
                        pass.PassportNumber = empDocumentData.PassportNumber;
                        pass.PassportUpload = empDocumentData.PassportUpload;
                        pass.Address = empDocumentData.Address;
                        pass.Checked = empDocumentData.Checked;
                        response.PassportDetails = pass;
                    }

                    //aadhar
                    if (empDocumentData.AadhaarCardNumber != null)
                    {
                        AadhaarHelper adhardetail = new AadhaarHelper();
                        adhardetail.AadhaarCardNumber = empDocumentData.AadhaarCardNumber;
                        adhardetail.AddressOnAadhaar = empDocumentData.AddressOnAadhaar;
                        adhardetail.DateOfBirthOnAadhaar = empDocumentData.DateOfBirthOnAadhaar;
                        adhardetail.FatherHusbandNameOnAadhaar = empDocumentData.FatherHusbandNameOnAadhaar;
                        adhardetail.NameOnAadhaar = empDocumentData.NameOnAadhaar;
                        adhardetail.AadhaarUpload = empDocumentData.AadhaarUpload;
                        adhardetail.Checked = empDocumentData.Checked;
                        response.AadhaarDetails = adhardetail;
                    }

                    //voter
                    if (empDocumentData.VoterIdNumber != null)
                    {
                        VoterCardHelper voterdetail = new VoterCardHelper();
                        voterdetail.VoterUpload = empDocumentData.VoterUpload;
                        voterdetail.DateOfBirthOnVoterId = empDocumentData.DateOfBirthOnVoterId;
                        voterdetail.FatherHusbandNameOnVoter = empDocumentData.FatherHusbandNameOnVoter;
                        voterdetail.NameOnVoterId = empDocumentData.NameOnVoterId;
                        voterdetail.VoterIdNumber = empDocumentData.VoterIdNumber;
                        voterdetail.Checked = empDocumentData.Checked;
                        response.VoterCardDetails = voterdetail;
                    }

                    //signature
                    if (empDocumentData.Name != null)
                    {
                        SignatureHelper signdetail = new SignatureHelper();
                        signdetail.Name = empDocumentData.Name;
                        signdetail.Designation = empDocumentData.Designation;
                        signdetail.SignatureUpload = empDocumentData.SignatureUpload;
                        signdetail.Checked = empDocumentData.Checked;
                        response.SignatureDetails = signdetail;
                    }
                    //

                    if (empDocumentData.JobTitle != null)
                    {
                        GetEmpPreviousExp previousexpdetail = new GetEmpPreviousExp();
                        previousexpdetail.EmployeeDocId = empDocumentData.EmployeeDocId;
                        previousexpdetail.CompanyName = empDocumentData.CompanyName;
                        previousexpdetail.JobTitle = empDocumentData.JobTitle;
                        previousexpdetail.JoiningDateExperience = empDocumentData.JoiningDateExperience;
                        previousexpdetail.RelievoingDateExperience = empDocumentData.RelievoingDateExperience;
                        previousexpdetail.LocationExperience = empDocumentData.LocationExperience;
                        previousexpdetail.DescriptionExperience = empDocumentData.DescriptionExperience;
                        previousexpdetail.ExperienceUpload = empDocumentData.ExperienceUpload;
                        previousexpdetail.Checked = empDocumentData.Checked;
                        response.PreviousExpDetail = previousexpdetail;
                    }
                    res.Message = "Employee Data Found";
                    res.Status = true;
                    res.Data = response;
                }
                else
                {
                    res.Message = "Employee Data Not Found";
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

        #endregion Api To Get Employee Document

        #region This Api is used to Add Relation of employee

        /// <summary>
        /// API >> Post >> api/employeenew/addrelationofemp
        ///Created by Shriya
        ///Created on 13-05-2022
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addrelationofemp")]
        public async Task<ResponseBodyModel> AddRelationOfEmp(RelationOfEmp modal)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                if (modal == null)
                {
                    res.Status = false;
                    res.Message = "Model Is Invalid";
                }
                else
                {
                    RelationOfEmp relation = new RelationOfEmp();
                    relation.EmployeeId = claims.employeeId;
                    relation.Gender = System.Enum.GetName(typeof(EnumClass.GenderConstants), Convert.ToInt32(modal.Gender));
                    relation.Relation = System.Enum.GetName(typeof(RelationShipTypeConstants), Convert.ToInt32(modal.Relation));
                    relation.FirstName = modal.FirstName;
                    relation.LastName = modal.LastName;
                    relation.Email = modal.Email;
                    relation.MobilePhone = modal.MobilePhone;
                    relation.Profession = modal.Profession;
                    relation.DateOfBirth = modal.DateOfBirth;
                    relation.CreatedBy = claims.employeeId;
                    relation.CreatedOn = DateTime.Now;
                    relation.IsActive = true;
                    relation.IsDeleted = false;
                    relation.CompanyId = claims.companyId;
                    relation.OrgId = claims.orgId;
                    _db.RelationOfEmp.Add(relation);
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "Realation added succesfully";
                    res.Data = relation;
                }

            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion This Api is used to Add Relation of employee

        #region This api for Get All Relation Of Employee

        /// <summary>
        /// API >> Get >>  api/employeenew/getrelations
        /// Created By Shriya
        /// Created on 13-05-2022
        /// </summary>
        [HttpGet]
        [Route("getrelations")]
        public async Task<ResponseBodyModel> GetRelationEmp()
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            List<EmpRelationsModel> Relation = new List<EmpRelationsModel>();
            try
            {
                var relations = await _db.RelationOfEmp.Where(x => x.EmployeeId == claims.employeeId && x.IsActive == true && x.IsDeleted == false).ToListAsync();
                if (relations != null)
                {
                    foreach (var item in relations)
                    {
                        EmpRelationsModel empRel = new EmpRelationsModel();
                        empRel.RelationId = item.RelationId;
                        empRel.Relation = item.Relation;
                        empRel.FirstName = item.FirstName;
                        empRel.LastName = item.LastName;
                        empRel.Gender = item.Gender;
                        empRel.Profession = item.Profession;
                        empRel.Email = item.Email;
                        empRel.MobilePhone = item.MobilePhone;
                        empRel.DateOfBirth = item.DateOfBirth;

                        Relation.Add(empRel);
                    }
                    res.Status = true;
                    res.Message = "List Found";
                    res.Data = Relation;
                }
                else
                {
                    res.Status = true;
                    res.Message = "List Not Found";
                    res.Data = Relation;
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion This api for Get All Relation Of Employee

        #region This Api is used to Get By relation Id

        /// <summary>
        ///  API >> Get >>  api/employeenew/getrelationbyid
        ///  Created by Shriya
        ///  Created on 13-05-2022
        /// </summary>
        /// <param name="RelationId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getrelationbyid")]
        public async Task<ResponseBodyModel> GetRelationById(int RelationId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            List<EmpRelationsModel> Relation = new List<EmpRelationsModel>();
            try
            {
                var relations = await _db.RelationOfEmp.Where(x => x.RelationId == RelationId
                && x.IsActive == true && x.IsDeleted == false).FirstOrDefaultAsync();
                if (relations != null)
                {
                    EditRelationsModel empRel = new EditRelationsModel();
                    empRel.RelationId = relations.RelationId;
                    empRel.Relations = relations.Relation;
                    empRel.Genders = relations.Gender;
                    empRel.Gender = (int)System.Enum.Parse(typeof(GenderConstants), relations.Gender);
                    empRel.Relation = (int)System.Enum.Parse(typeof(RelationShipTypeConstants), relations.Relation);
                    empRel.FirstName = relations.FirstName;
                    empRel.LastName = relations.LastName;
                    empRel.Profession = relations.Profession;
                    empRel.Email = relations.Email;
                    empRel.MobilePhone = relations.MobilePhone;
                    empRel.DateOfBirth = relations.DateOfBirth;

                    res.Status = true;
                    res.Message = "List Found";
                    res.Data = empRel;
                }
                else
                {
                    res.Status = true;
                    res.Message = "List Not Found";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion This Api is used to Get By relation Id

        #region This api is used to Edit Relation Detail

        /// <summary>
        /// API >> Put >> api/employeenew/editrelations
        /// Created By Shriya
        /// Created On 13-05-2022
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("editrelations")]
        public async Task<ResponseBodyModel> EditRelation(RelationOfEmp modal)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var detailRel = await _db.RelationOfEmp.Where(x => x.RelationId == modal.RelationId
                && x.EmployeeId == claims.employeeId && x.IsActive == true && x.IsDeleted == false).FirstOrDefaultAsync();
                if (detailRel != null)
                {
                    detailRel.Gender = System.Enum.GetName(typeof(GenderConstants), Convert.ToInt32(modal.Gender));
                    detailRel.Relation = System.Enum.GetName(typeof(RelationShipTypeConstants), Convert.ToInt32(modal.Relation));
                    detailRel.FirstName = modal.FirstName;
                    detailRel.LastName = modal.LastName;
                    detailRel.Email = modal.Email;
                    detailRel.MobilePhone = modal.MobilePhone;
                    detailRel.Profession = modal.Profession;
                    detailRel.DateOfBirth = modal.DateOfBirth;
                    detailRel.UpdatedBy = claims.employeeId;
                    detailRel.UpdatedOn = DateTime.Now;

                    _db.Entry(detailRel).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "Relation update succesfully";
                    res.Data = detailRel;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Relation not updated";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion This api is used to Edit Relation Detail

        #region This api is used to Delete Relation Detail

        /// <summary>
        /// Api >> Put >> api/employeenew/deleterelationbyid?RelationId=
        /// Created by Shriya
        /// Created on 13-05-2022
        /// </summary>
        /// <param name="RelationId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleterelationbyid")]
        public async Task<ResponseBodyModel> DeleteRelationById(int RelationId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var relation = await _db.RelationOfEmp.Where(x => x.RelationId == RelationId && x.IsDeleted == false && x.IsActive == true).FirstOrDefaultAsync();
                if (relation != null)
                {
                    relation.IsActive = false;
                    relation.IsDeleted = true;
                    relation.DeletedBy = claims.employeeId;
                    relation.DeletedOn = DateTime.Now;
                    _db.Entry(relation).State = System.Data.Entity.EntityState.Modified;
                    await _db.SaveChangesAsync();
                    res.Status = true;
                    res.Message = "Relation deleted succesfully";
                    res.Data = relation;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Relation not deleted";
                }
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = ex.Message;
            }
            return res;
        }

        #endregion This api is used to Delete Relation Detail

        #region API To Get Heriachy

        [HttpGet]
        [Route("hierarchy")]
        public async Task<CustomHierarchy> EmployeeHierarchy(int EmpId)
        {
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseBodyModel res = new ResponseBodyModel();

            CustomHierarchy customHierarchy = new CustomHierarchy();
            customHierarchy.childs = new List<NewCustomHierarchy>();

            NewCustomHierarchy newCustomHierarchy = new NewCustomHierarchy();
            List<NewCustomHierarchy> lists = new List<NewCustomHierarchy>();

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {
                    var list = await (from b in db.Employee
                                      join c in db.Designation on b.DesignationId equals c.DesignationId
                                      where (b.EmployeeId == EmpId || b.ReportingManager == EmpId)
                                      select new CustomHierarchy
                                      {
                                          EmpId = b.EmployeeId,
                                          Name = b.FirstName + " " + b.LastName,
                                          MgrId = b.ReportingManager,
                                          MgrName = db.Employee.Where(x => x.EmployeeId == b.ReportingManager).Select(x => x.DisplayName).FirstOrDefault(),
                                          title = c.DesignationName
                                      }).ToListAsync();

                    var loggedUser = await (from b in db.Employee
                                            join c in db.Designation on b.DesignationId equals c.DesignationId
                                            where b.EmployeeId == EmpId
                                            select new CustomHierarchy
                                            {
                                                EmpId = b.EmployeeId,
                                                Name = b.FirstName + " " + b.LastName,
                                                MgrId = b.ReportingManager,
                                                title = c.DesignationName,
                                                MgrName = db.Employee.Where(x => x.EmployeeId == b.ReportingManager).Select(x => x.DisplayName).FirstOrDefault()
                                            }).ToListAsync();

                    var myInt = loggedUser[0];
                    var managerEmp = db.Employee.Where(x => x.EmployeeId == myInt.MgrId).FirstOrDefault();
                    var managerDesignation = db.Designation.Where(x => x.DesignationId == managerEmp.DesignationId).FirstOrDefault().DesignationName;

                    if (managerEmp != null)
                    {
                        customHierarchy.Name = managerEmp.DisplayName;
                        customHierarchy.title = managerDesignation;
                        customHierarchy.EmpId = managerEmp.EmployeeId;
                        customHierarchy.cssClass = "cssClass: ngx-org-ceo";
                    }

                    if (loggedUser != null)
                    {
                        NewCustomHierarchy test = new NewCustomHierarchy();
                        foreach (var item in loggedUser)
                        {
                            test.Name = item.Name;
                            test.MgrName = item.MgrName;
                            test.title = item.title;
                            test.cssClass = "cssClass: ngx-org-ceo";

                            test.childs = new List<CustomHierarchy>();
                        }
                        lists.Add(test);
                    }

                    customHierarchy.childs = lists;

                    foreach (var item in list)
                    {
                        if (item.MgrId == EmpId)
                        {
                            customHierarchy.childs[0].childs.Add(new CustomHierarchy
                            {
                                Name = item.Name,
                                MgrName = item.MgrName,
                                title = item.title,
                                EmpId = item.EmpId,
                                MgrId = item.MgrId,
                                cssClass = "cssClass: ngx-org-ceo"
                            });
                        }
                    }

                    return customHierarchy;
                }
                catch (Exception)
                {
                    return customHierarchy;
                }
            }
        }

        public class CustomHierarchy
        {
            public string Name { get; set; }
            public string MgrName { get; set; }
            public string title { get; set; }
            public int EmpId { get; set; }
            public int MgrId { get; set; }
            public string cssClass { get; set; }
            public List<NewCustomHierarchy> childs { get; set; }
        }

        public class NewCustomHierarchy
        {
            public string Name { get; set; }
            public string MgrName { get; set; }
            public string title { get; set; }
            public string cssClass { get; set; }
            public List<CustomHierarchy> childs { get; set; }
        }

        #endregion API To Get Heriachy

        #region Get Employee Designation and Department on Behalf Of Employee Id

        /// <summary>
        /// Created By Harshit Mitra on 13-07-2022
        /// API >> Get >> api/employeenew/getemployeedesignationdepartment
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployeedesignationdepartment")]
        public async Task<ResponseBodyModel> GetEmployeePositionbyId(int employeeId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var empdesignation = await (from e in _db.Employee
                                            join d in _db.Department on e.DepartmentId equals d.DepartmentId
                                            join ds in _db.Designation on e.DesignationId equals ds.DesignationId
                                            where e.IsActive == true && e.IsDeleted == false && e.EmployeeId == employeeId
                                            select new
                                            {
                                                e.EmployeeId,
                                                e.DisplayName,
                                                d.DepartmentId,
                                                d.DepartmentName,
                                                ds.DesignationId,
                                                ds.DesignationName,
                                            }).FirstOrDefaultAsync();
                if (empdesignation != null)
                {
                    res.Message = "Employee Found";
                    res.Status = true;
                    res.Data = empdesignation;
                }
                else
                {
                    res.Message = "Not Found";
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

        #endregion Get Employee Designation and Department on Behalf Of Employee Id

        #region API To Get Employee List On Reporting Manager

        /// <summary>
        /// Created By Harshit Mitra On 04-08-2022
        /// API >> Get >> api/employeenew/emplistnotself
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("emplistnotself")]
        public async Task<ResponseBodyModel> GetEmployeeNameRemovingEdtiable(int employeeId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<EmployeeDirectoryHelperClass> employeeList = new List<EmployeeDirectoryHelperClass>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (claims.orgId != 0)
                {
                    employeeList = await (from e in _db.Employee
                                          join d in _db.Department on e.DepartmentId equals d.DepartmentId
                                          join ds in _db.Designation on e.DesignationId equals ds.DesignationId
                                          where e.IsActive == true && e.IsDeleted == false && e.EmployeeId != employeeId &&
                                          e.CompanyId == claims.companyId && (e.OrgId == claims.orgId || e.OrgId == 0)
                                          select new EmployeeDirectoryHelperClass
                                          {
                                              EmployeeId = e.EmployeeId,
                                              DisplayName = e.DisplayName,
                                              MobilePhone = e.MobilePhone,
                                              DepartmentId = d.DepartmentId,
                                              DepartmentName = d.DepartmentName,
                                              DesignationId = ds.DesignationId,
                                              DesignationName = ds.DesignationName,
                                              OfficeEmail = e.OfficeEmail,
                                              EmployeeTypeId = e.EmployeeTypeId,
                                              Location = e.CurrentAddress,
                                          }).OrderBy(x => x.DisplayName).ToListAsync();
                }
                else
                {
                    employeeList = await (from e in _db.Employee
                                          join d in _db.Department on e.DepartmentId equals d.DepartmentId
                                          join ds in _db.Designation on e.DesignationId equals ds.DesignationId
                                          where e.IsActive == true && e.IsDeleted == false &&
                                          e.CompanyId == claims.companyId && e.EmployeeId != e.ReportingManager
                                          select new EmployeeDirectoryHelperClass
                                          {
                                              EmployeeId = e.EmployeeId,
                                              DisplayName = e.DisplayName,
                                              MobilePhone = e.MobilePhone,
                                              DepartmentId = d.DepartmentId,
                                              DepartmentName = d.DepartmentName,
                                              DesignationId = ds.DesignationId,
                                              DesignationName = ds.DesignationName,
                                              OfficeEmail = e.OfficeEmail,
                                              EmployeeTypeId = e.EmployeeTypeId,
                                              Location = e.CurrentAddress,
                                          }).ToListAsync();
                }
                if (employeeList.Count > 0)
                {
                    res.Message = "Employee List";
                    res.Status = true;
                    res.Data = employeeList;
                }
                else
                {
                    res.Message = "Employee List Is Empty";
                    res.Status = false;
                    res.Data = employeeList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion API To Get Employee List On Reporting Manager

        #region Helper Model Class

        /// <summary>
        /// Created By Harshit Mitra on 21-06-2022
        /// </summary>
        public class UpdateEmployeeCodeAndSaleryModel
        {
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string FatherName { get; set; }
            public string MotherName { get; set; }
            public string Gender { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string BloodGroup { get; set; }
            public string MaritalStatus { get; set; }
            public string MedicalIssue { get; set; }
            public string AadharNumber { get; set; }
            public string PanNumber { get; set; }
            public string MobilePhone { get; set; }
            public string EmergencyNumber { get; set; }
            public string WhatsappNumber { get; set; }
            public string PersonalEmail { get; set; }
            public string PermanentAddress { get; set; }
            public string LocalAddress { get; set; }
            public string EmployeeType { get; set; }
            public string DepartmentName { get; set; }
            public DateTime JoiningDate { get; set; }
            public DateTime? ConfirmationDate { get; set; }
            public string DesignationName { get; set; }
            public string BiometricId { get; set; }
            public string BankAccountNumber { get; set; }
            public string IFSC { get; set; }
            public string AccountHolderName { get; set; }
            public string BankName { get; set; }
            public string OfficeEmail { get; set; }
            public string EmployeeCode { get; set; }
            public double Salary { get; set; }
            public string OrganizationName { get; set; }
            public string LoginType { get; set; }
            public string ReportingManager { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra On 28-04-2022
        /// </summary>
        public class AddEmployeeImport
        {
            public int EmployeeId { get; set; }

            /// <summary>
            /// --------------------------------------  Personal Information
            /// </summary>
            public string FirstName { get; set; }

            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string FatherName { get; set; }
            public string MotherName { get; set; }
            public string Gender { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string BloodGroup { get; set; }
            public string MaritalStatus { get; set; }
            public string AadharNumber { get; set; }
            public string PanNumber { get; set; }
            public string MobilePhone { get; set; }
            public string EmergencyNumber { get; set; }
            public string WhatsappNumber { get; set; }
            public string PersonalEmail { get; set; }
            public string PermanentAddress { get; set; }
            public string LocalAddress { get; set; }

            /// <summary>
            /// ------------------------------------------  Official Details
            /// </summary>
            public string LoginType { get; set; }

            public string EmployeeType { get; set; }
            public string DepartmentName { get; set; }
            public DateTime JoiningDate { get; set; }
            public DateTime ConfirmationDate { get; set; }
            public string DesignationName { get; set; }
            public string BiometricId { get; set; }

            /// <summary>
            /// ------------------------------------------  Bank Info Details
            /// </summary>
            public string BankAccountNumber { get; set; }
            public string IFSC { get; set; }
            public string AccountHolderName { get; set; }
            public string BankName { get; set; }

            /// <summary>
            /// ------------------------------------------  Credentials
            /// </summary>
            public string OfficeEmail { get; set; }
            public string Password { get; set; }
            public double Salary { get; set; }
            public string EmployeeCode { get; set; }
            public string ShiftGroup { get; set; }
            public string WeekOff { get; set; }
            public string OrganizationName { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra On 28-04-2022
        /// </summary>
        public class EmployeeDirectoryHelperClass
        {
            public int EmployeeId { get; set; }
            public string EmployeeCode { get; set; }
            public string DisplayName { get; set; }
            public string MobilePhone { get; set; }
            public int DepartmentId { get; set; }
            public string DepartmentName { get; set; }
            public int DesignationId { get; set; }
            public string DesignationName { get; set; }
            public string OfficeEmail { get; set; }
            public string Location { get; set; }
            public EmployeeTypeConstants EmployeeTypeId { get; set; }
            public string EmployeeTypeName { get; set; }
            public string ProfileImageUrl { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string FatherName { get; set; }
            public string MotherName { get; set; }
            public string Gender { get; set; }
            public DateTime DateOfBirth { get; set; }
            public bool hideDOB { get; set; }
            public BloodGroupConstants BloodGroup { get; set; }
            public string BloodGroupName { get; set; }
            public string MaritalStatus { get; set; }
            public string MedicalIssue { get; set; }
            public string AadharNumber { get; set; }
            public string PanNumber { get; set; }
            public string PrimaryContact { get; set; }
            public DateTimeOffset JoiningDate { get; set; }
            public DateTimeOffset? ConfirmationDate { get; set; }
            public string Password { get; set; }
            public string BankAccountNumber { get; set; }
            public string IFSC { get; set; }
            public string AccountHolderName { get; set; }
            public string BankName { get; set; }
            public string PermanentAddress { get; set; }
            public string WhatsappNumber { get; set; }
            public string EmergencyNumber { get; set; }
            public string PersonalEmail { get; set; }
            public string LocalAddress { get; set; }
            public LoginRolesConstants LoginType { get; set; }
            public string LoginTypeName { get; set; }
            public double GrossSalery { get; set; }
            public int? OrgId { get; set; }
            public string OrgName { get; set; }
            public int AddedBy { get; set; }
            public string AddedByName { get; set; }
            public int LastUpdatedBy { get; set; }
            public string LastUpdatedByName { get; set; }
            public int ReportingManagerId { get; set; }
            public bool IsEmployeeIsLock { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra On 28-04-2022
        /// </summary>
        public class EmployeeDirectoryParameter
        {
            public List<int> OrgId { get; set; }
            public List<int> DepartmentId { get; set; }
            public List<int> DesignationId { get; set; }
            public List<EmployeeTypeConstants> EmployeeTypeId { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra On 30-04-2022
        /// </summary>
        public class EmployeeDirectoryParameterAdmin
        {
            public List<int> OrgId { get; set; }
            public List<int> DepartmentId { get; set; }
            public List<int> DesignationId { get; set; }
            public List<EmployeeTypeConstants> EmployeeTypeId { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra On 30-04-2022
        /// </summary>
        public class EmployeeDirectoryHelperClassAdmin
        {
            public int EmployeeId { get; set; }
            public string DisplayName { get; set; }
            public string MobilePhone { get; set; }
            public int DepartmentId { get; set; }
            public string DepartmentName { get; set; }
            public int DesignationId { get; set; }
            public string DesignationName { get; set; }
            public string OfficeEmail { get; set; }
            public string Location { get; set; }
            public EmployeeTypeConstants EmployeeTypeId { get; set; }
            public int OrgId { get; set; }
            public string OrgName { get; set; }
            public string ProfileImageUrl { get; set; }
            public BloodGroupConstants BloodGroup { get; set; }
            public string BloodGroupName { get; set; }
            public int? AddedBy { get; set; }
            public string AddedByName { get; set; }
            public int? LastUpdatedBy { get; set; }
            public string LastUpdatedByName { get; set; }
            public bool IsEmployeeIsLock { get; set; }
        }

        public class UpdateEmployeeProfileSelf
        {
            public int EmployeeId { get; set; }
            public string ProfileImageUrl { get; set; }
            public string ProfileImageExtension { get; set; }

            //----- Personal Details -----//
            public string FirstName { get; set; }

            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string Gender { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public bool HideDOB { get; set; }
            public string MaritalStatus { get; set; }
            public string MedicalIssue { get; set; }
            public BloodGroupConstants? BloodGroup { get; set; }
            public bool? IsPhysicallyHandicapped { get; set; }

            //------ Conatact Details -----//
            public string WorkEmail { get; set; }

            public string PersonalEmail { get; set; }
            public string MobilePhone { get; set; }
            public string WorkPhone { get; set; }
            public string SkypeMail { get; set; }

            //----- Address Details -----//
            public string LocalAddress { get; set; }

            public int? LocalCountryId { get; set; }
            public int? LocalStateId { get; set; }
            public int? LocalCityId { get; set; }
            public int? LocalPinCode { get; set; }
            public string PermanentAddress { get; set; }
            public int? PermanentCountryId { get; set; }
            public int? PermanentStateId { get; set; }
            public int? PermenentCityId { get; set; }
            public int? PermenentPinCode { get; set; }

            //----- Professional Summary -----//
            public string ProfessionalSummary { get; set; }

            //----- Remark Part -----//
            public string AboutMeRemark { get; set; }

            public string AboutMyJobRemark { get; set; }
            public string InterestAndHobbiesRemark { get; set; }
            //----- Login Type -----//
            public LoginRolesConstants? LoginType { get; set; }

            public double Salary { get; set; } //shriya
            public string AadharNumber { get; set; }//shriya
            public string PanNumber { get; set; }//shriya
            public string FatherName { get; set; }//shriya
            public string MotherName { get; set; }//shriya
            public string BiometricId { get; set; }//shriya
            public string BankAccountNumber { get; set; }//shriya
            public string IFSC { get; set; }//shriya
            public string AccountHolderName { get; set; }//shriya
            public string BankName { get; set; }//shriya
                                                // public List<int> ReportingManagerId { get; set; } //shriya
            public int ReportingManagerId { get; set; } //shriya
            public string EmployeeCode { get; set; }
            public int OrgId { get; set; }
            public Guid? ShiftGroupId { get; set; }
            public Guid? WeekoffId { get; set; }
            public EmployeeTypeConstants EmployeeTypeId { get; set; }

            public int DesignationId { get; set; }
            public string DesignationName { get; set; }
            public int DepartmentId { get; set; }
            public string DepartmentName { get; set; }
        }

        public class GetEmployeeProfileModel
        {
            public string ProfileImageUrl { get; set; }
            public string ProfileImageExtension { get; set; }
            public int EmployeeId { get; set; }
            public string DisplayName { get; set; }
            public string Department { get; set; }
            public string Designation { get; set; }
            public string BusinessUnit { get; set; }
            public string EmployeeNo { get; set; }
            public string ProfessionalSummary { get; set; }
            public bool IsOnExit { get; set; }
            public EmployeeProfilePersonalInfo PersonalInfo { get; set; }
            public EmployeeConatactDetails ConatactDetails { get; set; }
            public EmployeeAddressDetails AddressDetails { get; set; }
            public EmployeeRemarkDetails RemarkDetails { get; set; }
        }

        public class EmployeeProfilePersonalInfo
        {
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string Gender { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public bool HideDOB { get; set; }
            public string MaritalStatus { get; set; }
            public string MedicalIssue { get; set; }
            public BloodGroupConstants BloodGroup { get; set; }
            public string BloodGroupName { get; set; }
            public bool? IsPhysicallyHandicapped { get; set; }
        }

        public class EmployeeConatactDetails
        {
            public string WorkEmail { get; set; }
            public string PersonalEmail { get; set; }
            public string MobilePhone { get; set; }
            public string WorkPhone { get; set; }
            public string SkypeMail { get; set; }
        }

        public class EmployeeAddressDetails
        {
            public string LocalAddress { get; set; }
            public int? LocalCountryId { get; set; }
            public int? LocalStateId { get; set; }
            public int? LocalCityId { get; set; }
            public int? LocalPinCode { get; set; }
            public string LocalCountryName { get; set; }
            public string LocalStateName { get; set; }
            public string LocalCityName { get; set; }
            public string PermanentAddress { get; set; }
            public int? PermanentCountryId { get; set; }
            public int? PermanentStateId { get; set; }
            public int? PermenentCityId { get; set; }
            public int? PermenentPinCode { get; set; }
        }

        public class EmployeeRemarkDetails
        {
            public string AboutMeRemark { get; set; }
            public string AboutMyJobRemark { get; set; }
            public string InterestAndHobbiesRemark { get; set; }
        }

        public class GetEmployeeAfterAdd
        {
            public int EmployeeId { get; set; }

            /// <summary>
            /// --------------------------------------  Personal Information
            /// </summary>
            public string FirstName { get; set; }

            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string DisplayName { get; set; }
            public string FatherName { get; set; }
            public string MotherName { get; set; }
            public string Gender { get; set; }
            public DateTime DateOfBirth { get; set; }
            public bool HideDOB { get; set; }
            public BloodGroupConstants BloodGroup { get; set; }
            public string MaritalStatus { get; set; }
            public string MedicalIssue { get; set; }
            public string AadharNumber { get; set; }
            public string PanNumber { get; set; }
            public string MobilePhone { get; set; }
            public string EmergencyNumber { get; set; }
            public string WhatsappNumber { get; set; }
            public string PersonalEmail { get; set; }
            public string LocalAddress { get; set; }
            public string PermanentAddress { get; set; }
            public LoginRolesConstants LoginType { get; set; }
            public EmployeeTypeConstants EmployeeTypeId { get; set; }
            public int DepartmentId { get; set; }
            public int DesignationId { get; set; }
            public DateTimeOffset JoiningDate { get; set; }
            public DateTimeOffset? ConfirmationDate { get; set; }
            public string BiometricId { get; set; }
            public string BankAccountNumber { get; set; }
            public string IFSC { get; set; }
            public string AccountHolderName { get; set; }
            public string BankName { get; set; }
            public string OfficeEmail { get; set; }
            public string Password { get; set; }
            public double Salary { get; set; } // shriya
            public string EmployeeCode { get; set; }
            public int OrgId { get; set; }
            //public List<int> ReportingManagerId { get; set; }
            public int ReportingManagerId { get; set; }
            public List<string> ReportingManagerName { get; set; }
            //public List<GetReportingManagers> Reportingmanagers { get; set; }  // shriya
            public Guid? ShiftGroupId { get; set; }
            public Guid? WeekOffId { get; set; }
        }

        public class GetReportingManagers
        {
            public int ReportingManagerId { get; set; }
            public string ReportingManagerName { get; set; }
        }

        public class GetUserLoginRoleResponse
        {
            public string Message { get; set; }
            public bool Status { get; set; }
            public string RoleType { get; set; }
            public string DisplayName { get; set; }
            public string ProfileImageUrl { get; set; }
        }

        /// <summary>
        /// created by shriya on 09-05-2022
        /// </summary>
        public class GetEmpPreviousExp
        {
            public int EmployeeDocId { get; set; }
            public string CompanyName { get; set; }
            public string JobTitle { get; set; }
            public DateTime? JoiningDateExperience { get; set; }
            public DateTime? RelievoingDateExperience { get; set; }
            public string LocationExperience { get; set; }
            public string DescriptionExperience { get; set; }
            public string ExperienceUpload { get; set; }
            public bool Checked { get; set; }
        }

        /// <summary>
        /// created by shriya on 13-0-2022
        /// </summary>
        public class EmpRelationsModel
        {
            public int RelationId { get; set; }
            public string Relation { get; set; }
            public string Gender { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string MobilePhone { get; set; }
            public string Profession { get; set; }
            public DateTime? DateOfBirth { get; set; }
        }

        public class EditRelationsModel
        {
            public int RelationId { get; set; }
            public int Relation { get; set; }
            public int Gender { get; set; }
            public string Relations { get; set; }
            public string Genders { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string MobilePhone { get; set; }
            public string Profession { get; set; }
            public DateTime? DateOfBirth { get; set; }
        }

        public class EmployeeDocumentModel
        {
            public DocumentDetailsHelper DocumentDetails { get; set; }
            public DocumentDetailsDrivingLicenseHelper DrivingLicenseDetails { get; set; }
            public DocumentPANCardHelper PanDetails { get; set; }
            public DocumentPassportHelper PassportDetails { get; set; }
            public AadhaarHelper AadhaarDetails { get; set; }
            public VoterCardHelper VoterCardDetails { get; set; }
            public SignatureHelper SignatureDetails { get; set; }
            public GetEmpPreviousExp PreviousExpDetail { get; set; }
        }

        public class DocumentDetailsDrivingLicenseHelper
        {
            public string Licensenumber { get; set; }
            public DateTime? DateOfBirthOnDriving { get; set; }
            public string NameOnDriving { get; set; }
            public string FatherHusbandNameOnDriving { get; set; }
            public DateTime? ExpireOnLicense { get; set; }
            public string DrivingLicenseUpload { get; set; }
            public bool Checked { get; set; }
        }

        public class DocumentPANCardHelper
        {
            public string PanNumber { get; set; }
            public string NameOnPan { get; set; }
            public DateTime? DateOfBirthDateOnPan { get; set; }
            public string FatherNameOnPan { get; set; }
            public string PanUpload { get; set; }
            public bool Checked { get; set; }
        }

        public class DocumentPassportHelper
        {
            public string PassportNumber { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string FullName { get; set; }
            public string FatherName { get; set; }
            public DateTime? DateOfIssue { get; set; }
            public string PlaceOfIssue { get; set; }
            public string PlaceofBirth { get; set; }
            public DateTime? ExpiresOn { get; set; }
            public string Address { get; set; }
            public string PassportUpload { get; set; }
            public bool Checked { get; set; }
        }

        public class AadhaarHelper
        {
            public string AadhaarCardNumber { get; set; }
            public DateTime? DateOfBirthOnAadhaar { get; set; }
            public string NameOnAadhaar { get; set; }
            public string FatherHusbandNameOnAadhaar { get; set; }
            public string AddressOnAadhaar { get; set; }
            public string AadhaarUpload { get; set; }
            public bool Checked { get; set; }
        }

        public class VoterCardHelper
        {
            public string VoterIdNumber { get; set; }
            public DateTime? DateOfBirthOnVoterId { get; set; }
            public string NameOnVoterId { get; set; }
            public string FatherHusbandNameOnVoter { get; set; }
            public string VoterUpload { get; set; }
            public bool Checked { get; set; }
        }

        public class SignatureHelper
        {
            public string Name { get; set; }
            public string Designation { get; set; }
            public string SignatureUpload { get; set; }
            public bool Checked { get; set; }
        }

        public class DocumentDetailsHelper
        {
            public string Branch { get; set; }
            public string Degree { get; set; }
            public DateTime? DateOfJoining { get; set; }
            public DateTime? DateOfCompleation { get; set; }
            public string PerctOrCGPA { get; set; }
            public string UniversityOrCollage { get; set; }
            public string DegreeUpload { get; set; }
            public bool Checked { get; set; }
        }

        public class AddEmployeeModelHelper
        {
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string FatherName { get; set; }
            public string MotherName { get; set; }
            public string Gender { get; set; }
            public DateTime DateOfBirth { get; set; }
            public BloodGroupConstants BloodGroup { get; set; }
            public string MaritalStatus { get; set; }
            public string MedicalIssue { get; set; }
            public string AadharNumber { get; set; }
            public string PanNumber { get; set; }
            public string MobilePhone { get; set; }
            public string EmergencyNumber { get; set; }
            public string WhatsappNumber { get; set; }
            public string PersonalEmail { get; set; }
            public string PermanentAddress { get; set; }
            public string LocalAddress { get; set; }
            public double GrossSalery { get; set; }

            public string BankAccountNumber { get; set; }
            public string IFSC { get; set; }
            public string AccountHolderName { get; set; }
            public string BankName { get; set; }

            public LoginRolesConstants LoginType { get; set; }
            public EmployeeTypeConstants EmployeeTypeId { get; set; }
            public int DepartmentId { get; set; }
            public DateTime JoiningDate { get; set; }
            public DateTime? ConfirmationDate { get; set; }
            public int DesignationId { get; set; }
            public string BiometricId { get; set; }
            public int ReportingManagerId { get; set; }
            // public List<int> ReportingManagerId { get; set; }
            public string OfficeEmail { get; set; }
            public string Password { get; set; }
            public string EmployeeCode { get; set; }
            public int OrgId { get; set; }
            public Guid ShiftGroupId { get; set; }
            public Guid WeekOffId { get; set; }
        }

        #endregion Helper Model Class

        #region Helper Model For LMS
        public class LMS
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string CountryCode { get; set; }
            public string MobileNumber { get; set; }
            public int Tenant { get; set; }
            public string Picture { get; set; }
            public string OrganisationName { get; set; }
            public int CompanyId { get; set; }
            public int OrgId { get; set; }
            public int Role { get; set; }
            public int EmployeeId { get; set; }

        }

        #endregion

        #region This Api for Check Employee Exits Or Not In LMS
        /// <summary>
        /// Created By Ravi Vyas on 03-11-2022
        /// API>>POST>>api/employeenew/checkemployee
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("checkemployee")]
        public object CheckEmployeeData(AuthenticateRequest model)
        {
            var data = _db.Employee.Where(x => x.OfficeEmail == model.Username && x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
            if (data != null)
            {
                return new
                {
                    Status = true,
                    data.FirstName,
                    data.LastName,
                    data.EmployeeId,
                    data.CompanyId,
                    data.OrgId,
                    data.ProfileImageUrl,
                    data.MobilePhone,
                    data.OfficeEmail,
                    data.Password,
                };
            }
            else
            {
                return new
                {
                    Status = false,
                    FirstName = "",
                    LastName = "",
                    EmployeeId = 0,
                    CompanyId = 0,
                    OrgId = 0,
                    ProfileImageUrl = "",
                    MobilePhone = "",
                    OfficeEmail = "",
                    Password = "",
                };
            }
        }

        #endregion

        #region Response Body LMS
        public class AuthenticateRequest
        {

            public string Username { get; set; }
            // public string Password { get; set; }

        }
        #endregion


        #region API TO LOCK EMPLOYEE IN A COMPANY
        /// <summary>
        /// Created By Harshit Mitra On 16-01-2023
        /// API >> POST >> api/employeenew/lockemployee
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("lockemployee")]
        public async Task<IHttpActionResult> LockCompanyLogin(int employeeId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var emp = await _db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
                if (emp == null)
                {
                    res.Message = "Org Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;

                    return Ok(res);
                }
                emp.IsEmployeeIsLock = !emp.IsEmployeeIsLock;
                _db.Entry(emp).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                _db.Dispose();

                res.Message = "Employee Is " + (emp.IsEmployeeIsLock ? "Locked" : "Unlocked");
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/employeenew/lockemployee | " +
                    "EmployeeId : " + employeeId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

    }
}