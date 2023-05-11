using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.UserAccesPermission;
using AspNetIdentity.WebApi.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers
{
    /// <summary>
    /// Modify By Harshit Mitra on 22-04-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/orgmaster")]
    public class OrgMasterController : BaseApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Get Organization By OrgId

        /// <summary>
        /// Modify By Harshit Mitra On 22-04-2022
        /// API >> Get >> api/orgmaster/getorgbyid
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getorgbyid")]
        public async Task<ResponseBodyModel> GetOrganizationById(int orgId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var Org = await _db.OrgMaster.FirstOrDefaultAsync(x =>
                    x.OrgId == orgId && x.CompanyId == claims.companyId);
                if (Org != null)
                {
                    res.Status = true;
                    res.Message = "Organization Found";
                    res.Data = Org;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Organization Found!!";
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

        #endregion Get Organization By OrgId

        #region Get All Organization

        /// <summary>
        /// Modify By Harshit Mitra On 22-04-2022
        /// API >> Get >> api/orgmaster/getallactiveorg
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallactiveorg")]
        public async Task<ResponseBodyModel> GetAllOrganization()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var orgDa = await _db.OrgMaster.Where(x => x.CompanyId == claims.companyId &&
                    x.IsDeleted == false && x.IsActive == true).ToListAsync();
                if (orgDa.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Organaization list Found";
                    res.Data = orgDa.OrderByDescending(x => x.OrgId);
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Organaization list Found";
                    res.Data = orgDa;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get All Organization

        #region Get All Org By Company Id

        /// <summary>
        /// Modify By Harshit Mitra On 22-04-2022
        /// API >> Get >> api/orgmaster/orgbycompanyid
        /// </summary>
        /// <returns></returns>
        [Route("orgbycompanyid")]
        [HttpGet]
        [Authorize]
        public async Task<ResponseBodyModel> GetAllOrganizationByCompanyId(int companyId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var orgDat = await _db.OrgMaster.Where(x => x.CompanyId == companyId &&
                    x.IsDeleted == false && x.IsActive == true).ToListAsync();
                if (orgDat.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Organization list Found";
                    res.Data = orgDat;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Organaization list Found";
                    res.Data = orgDat;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get All Org By Company Id

        #region Get All Org By Company Id

        /// <summary>
        /// Modify By Harshit Mitra On 05-07-2022
        /// API >> Get >> api/orgmaster/orgbycompanyidclaims
        /// </summary>
        /// <returns></returns>
        [Route("orgbycompanyidclaims")]
        [HttpGet]
        [Authorize]
        public async Task<ResponseBodyModel> GetAllOrganizationByCompanyId()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var orgDat = await _db.OrgMaster.Where(x => x.CompanyId == claims.companyId &&
                    x.IsDeleted == false && x.IsActive == true).ToListAsync();
                if (orgDat.Count > 0)
                {
                    res.Status = true;
                    res.Message = "Organization list Found";
                    res.Data = orgDat;
                }
                else
                {
                    res.Status = false;
                    res.Message = "No Organaization list Found";
                    res.Data = orgDat;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Get All Org By Company Id

        #region Add Organization

        /// <summary>
        /// Modify By Harshit Mitra On 25-04-2022
        /// API >> Get >> api/orgmaster/addorg
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addorg")]
        public async Task<ResponseBodyModel> CreateOrganization(AddOrgClass model)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var checkOrg = await _db.OrgMaster.Where(x => x.CompanyId == tokenData.companyId &&
                    x.OrgName.Trim().ToUpper() == model.OrgName.Trim().ToUpper()).FirstOrDefaultAsync();
                if (checkOrg != null)
                {
                    res.Message = "Org Already Exist";
                    res.Status = false;
                }
                else
                {
                    var firstName = model.FirstName.Trim();
                    var middleName = String.IsNullOrEmpty(model.MiddleName) ? "" : model.MiddleName.Trim();
                    var lastName = model.LastName.Trim();
                    var roledata = _db.RoleInUserAccessPermissions.Where(x => x.IsActive && !x.IsDeleted &&
                            x.CompanyId == tokenData.companyId && x.RoleName == "Default Role").FirstOrDefault();
                    OrgMaster org = new OrgMaster
                    {
                        OrgName = model.OrgName,
                        OrgAddress = model.OrgAddress,
                        CompanyId = tokenData.companyId,
                        CreatedBy = tokenData.userId,
                        CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                        TimeZoneId = model.TimeZoneId,
                        CountryId = model.CountryId,
                    };
                    _db.OrgMaster.Add(org);
                    var check = await _db.SaveChangesAsync();

                    if (check == 1)
                    {
                        #region Adding Department And Designation

                        var checkDepartment = _db.Department.Where(x => x.DepartmentName == "HR Department" && x.CompanyId == tokenData.companyId).FirstOrDefault();
                        if (checkDepartment == null)
                        {
                            Department department = new Department
                            {
                                IsActive = true,
                                IsDeleted = false,
                                CreatedBy = 0,
                                CompanyId = tokenData.companyId,
                                CreatedOn = DateTime.Now,
                                DepartmentName = "HR Department",
                                Description = "Used For HR Department",
                                UsedForLogin = true,
                            };
                            _db.Department.Add(department);
                            await _db.SaveChangesAsync();
                            checkDepartment = department;
                        }

                        var checkDesignation = _db.Designation.Where(x => x.DesignationName == "HR Head" && x.CompanyId == tokenData.companyId).FirstOrDefault();
                        if (checkDesignation == null)
                        {
                            Designation designation = new Designation
                            {
                                IsActive = true,
                                IsDeleted = false,
                                CreatedBy = 0,
                                CompanyId = tokenData.companyId,
                                CreatedOn = DateTime.Now,
                                DesignationName = "HR Head",
                                DepartmentId = checkDepartment.DepartmentId,
                            };
                            _db.Designation.Add(designation);
                            await _db.SaveChangesAsync();
                            checkDesignation = designation;
                        }

                        #endregion Adding Department And Designation

                        #region Add HR of Org

                        var Password = model.Password.Trim();
                        var hashKey = DataHelper.GeneratePasswords(10);
                        var encPassword = DataHelper.EncodePassword(Password, hashKey);
                        byte Levels = 4;

                        var user = new ApplicationUser()
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            PhoneNumber = model.UserPhoneNumber,
                            Level = Levels,
                            JoinDate = DateTime.Now,
                            EmailConfirmed = true,
                            Email = model.OfficeEmail,
                            PasswordHash = hashKey,
                            UserName = model.OfficeEmail
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
                            empObj.DateOfBirth = model.DateOfBirth;
                            empObj.ConfirmationDate = DateTime.Now;
                            empObj.JoiningDate = DateTime.Now;
                            empObj.CreatedBy = 0;
                            empObj.CreatedOn = DateTime.Now;
                            empObj.Gender = model.Gender;
                            empObj.IsActive = true;
                            empObj.IsDeleted = false;
                            empObj.DepartmentId = checkDepartment.DepartmentId;
                            empObj.DesignationId = checkDesignation.DesignationId;
                            empObj.RoleId = 0;
                            empObj.EmployeeTypeId = EmployeeTypeConstants.Confirmed_Employee;
                            empObj.EmergencyNumber = "";
                            empObj.Password = Password;
                            empObj.OfficeEmail = model.OfficeEmail;
                            empObj.MobilePhone = model.UserPhoneNumber;
                            empObj.CompanyId = tokenData.companyId;
                            empObj.OrgId = org.OrgId;
                            empObj.ShiftGroupId = tokenData.DefaultShiftGroupId;
                            empObj.WeekOffId = tokenData.DefaultWeekOff;

                            _db.Employee.Add(empObj);
                            await _db.SaveChangesAsync();

                            User userObj = new User();
                            userObj.EmployeeId = empObj.EmployeeId;
                            userObj.UserName = empObj.OfficeEmail;
                            userObj.Password = encPassword;
                            userObj.HashCode = hashKey;
                            userObj.DepartmentId = checkDepartment.DepartmentId;
                            userObj.LoginId = LoginRolesConstants.HR;
                            userObj.CreatedOn = DateTime.Now;
                            userObj.IsActive = true;
                            userObj.IsDeleted = false;
                            userObj.CompanyId = tokenData.companyId;
                            userObj.OrgId = org.OrgId;

                            _db.User.Add(userObj);
                            await _db.SaveChangesAsync();

                            EmployeeInRole obj = new EmployeeInRole();
                            obj.RoleId = roledata.RoleId;
                            obj.EmployeeId = empObj.EmployeeId;
                            obj.CompanyId = empObj.CompanyId;
                            obj.OrgId = org.OrgId;
                            obj.CreatedOn = org.CreatedOn;
                            _db.EmployeeInRoles.Add(obj);
                            await _db.SaveChangesAsync();

                            ResponseAddOrg response = new ResponseAddOrg
                            {
                                LoginMailId = userObj.UserName,
                                Password = empObj.Password,
                            };
                            res.Data = response;
                        }
                        else
                        {
                            _db.OrgMaster.Remove(org);
                            await _db.SaveChangesAsync();

                            res.Message = "Org Creation Fail";
                            res.Status = false;
                        }

                        #endregion Add HR of Org

                        res.Message = "Org Created";
                        res.Status = true;
                        res.Data = checkOrg;
                    }
                    else
                    {
                        res.Message = "Org Creation Fail";
                        res.Status = false;
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

        #endregion Add Organization



        #region Update Organaization
        /// <summary>
        /// Modify By Harshit Mitra On 25-04-2022
        /// API >> POST >> api/orgmaster/editorg
        /// </summary>
        /// <param name="updateOrg"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("editorg")]
        public async Task<ResponseBodyModel> UpdateOrganaization(OrgMaster updateOrg)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var updateOrgData = await _db.OrgMaster.FirstOrDefaultAsync(x =>
                        x.OrgId == updateOrg.OrgId && x.IsDeleted == false);
                if (updateOrgData != null)
                {
                    updateOrgData.OrgName = updateOrg.OrgName;
                    updateOrgData.OrgAddress = updateOrg.OrgAddress;
                    updateOrgData.TimeZoneId = updateOrg.TimeZoneId;
                    updateOrgData.CountryId = updateOrg.CountryId;

                    updateOrgData.UpdatedBy = tokenData.employeeId;
                    updateOrgData.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                    _db.Entry(updateOrgData).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Status = true;
                    res.Message = "Organaization Updated Successfully!";
                    res.Data = updateOrgData;
                }
                else
                {
                    res.Status = false;
                    res.Message = "Organaization not updated";
                    res.Data = updateOrgData;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        #endregion Update Organaization

        #region Delete Org

        /// <summary>
        /// Modify By Harshit Mitra On 25-04-2022
        /// API >> Put >> api/orgmaster/deleteorg
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deleteorg")]
        public async Task<ResponseBodyModel> DeleteOrganaization(int orgId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var deleteData = await _db.OrgMaster.Where(x => x.OrgId == orgId)
                        .FirstOrDefaultAsync();
                if (deleteData != null)
                {
                    deleteData.IsDeleted = true;
                    deleteData.IsActive = false;
                    _db.Entry(deleteData).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    res.Status = true;
                    res.Message = "Organaization Deleted Successfully!";
                }
                else
                {
                    res.Status = false;
                    res.Message = "No organaization Found!!";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Delete Org

        #region Helper Model Class

        public class AddOrgClass
        {
            /// ----- Org Info ------ ///
            public string OrgName { get; set; }

            public string OrgAddress { get; set; }

            /// ----- Company Admin ------- ///
            public string FirstName { get; set; }

            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string UserPhoneNumber { get; set; }
            public string PersonalEmail { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string Gender { get; set; }
            public string OfficeEmail { get; set; }
            public string Password { get; set; }
            public string TimeZoneId { get; set; }
            public int CountryId { get; set; }
        }

        public class ResponseAddOrg
        {
            public string LoginMailId { get; set; }
            public string Password { get; set; }
        }

        #endregion Helper Model Class


        #region API TO LOCK ORG IN THE COMPANY
        /// <summary>
        /// Created By Harshit Mitra On 16-01-2023
        /// API >> POST >> api/orgmaster/lockorg
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("lockorg")]
        public async Task<IHttpActionResult> LockCompanyLogin(int orgId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var org = await _db.OrgMaster.FirstOrDefaultAsync(x => x.OrgId == orgId);
                if (org == null)
                {
                    res.Message = "Org Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;

                    return Ok(res);
                }
                org.IsOrgIsLock = !org.IsOrgIsLock;
                _db.Entry(org).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                _db.Dispose();

                res.Message = "Org Is " + (org.IsOrgIsLock ? "Locked" : "Unlocked");
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/orgmaster/lockorg | " +
                    "OrgId : " + orgId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET EMPLOYEE LIST WHO DONT HAVE ANY REPORTING MANAGER EXCLUDE ADMIN (PRESIDENT)
        /// <summary>
        /// Created By Harshit Mitra On 09-01-2023
        /// API >> GET >> api/orgmaster/getunassignreportingmanagers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getunassignreportingmanagers")]
        public async Task<IHttpActionResult> GetUnAssignReportingManager()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employeeList = await (from e in _db.Employee
                                          where e.IsActive && !e.IsDeleted &&
                                                e.CompanyId == tokenData.companyId &&
                                                e.ReportingManager == 0 && !e.IsPresident &&
                                                e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee
                                          select new
                                          {
                                              EmployeeId = e.EmployeeId,
                                              DisplayName = e.DisplayName,

                                          }).ToListAsync();

                res.Message = "Employee List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = new
                {
                    IsAdmin = tokenData.IsAdminInCompany,
                    EmployeeList = employeeList,
                };

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/orgmaster/getunassignreportingmanagers | " +
                    //" Model : " + JsonConvert.SerializeObject(model) + " |" +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET ALL EMPLOYEE LIST WHO ARE IN ORG TREE
        /// <summary>
        /// Created By Harshit Mitra On 09-01-2023
        /// API >> GET >> api/orgmaster/getallemployeelistwhoareinorgtree
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallemployeelistwhoareinorgtree")]
        public async Task<IHttpActionResult> GetAllEmplyeeList()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employeeList = await (from e in _db.Employee
                                          where e.IsActive && !e.IsDeleted &&
                                                e.CompanyId == tokenData.companyId &&
                                                (e.ReportingManager != 0 || e.IsPresident) &&
                                                e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee
                                          select new
                                          {
                                              EmployeeId = e.EmployeeId,
                                              DisplayName = e.DisplayName,

                                          }).ToListAsync();

                res.Message = "All Employee List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = employeeList;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/orgmaster/getunassignreportingmanagers | " +
                    //" Model : " + JsonConvert.SerializeObject(model) + " |" +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO ASSIGN REPORTING MANAGER TO EMPLOYEE
        /// <summary>
        /// Created By Harshit Mitra On 09-01-2023
        /// API >> POST >> api/orgmaster/assignreportingmanager
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("assignreportingmanager")]
        public async Task<IHttpActionResult> AssignReportingManagerToEmployee(AssignReportingManagerRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employee = await _db.Employee
                    .FirstOrDefaultAsync(x => x.EmployeeId == model.EmployeeId);
                employee.ReportingManager = model.ManagerId;
                _db.Entry(employee).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "Reporting Manager Assign";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/orgmaster/getunassignreportingmanagers | " +
                    //" Model : " + JsonConvert.SerializeObject(model) + " |" +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class AssignReportingManagerRequest
        {
            public int ManagerId { get; set; } = 0;
            public int EmployeeId { get; set; } = 0;
        }
        #endregion

        #region API TO SEARCH EMPLOYEE BY NAME 
        /// <summary>
        /// Created By Harshit Mitra On 09-01-2023
        /// API >> POST >> api/orgmaster/searchemployee
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("searchemployee")]
        public async Task<IHttpActionResult> SearchEmployeeByName(string displayName)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (String.IsNullOrEmpty(displayName))
                {
                    res.Message = "No Employee Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = new
                    {
                        IsAdmin = tokenData.IsAdminInCompany,
                        EmployeeList = new List<int>(),
                    };
                    return Ok(res);
                }

                var employee = await _db.Employee
                    .Where(x => x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee
                        && x.DisplayName.Trim().ToUpper().Contains(displayName.Trim().ToUpper()))
                    .Select(x => new
                    {
                        x.EmployeeId,
                        x.DisplayName,
                    })
                    .ToListAsync();

                if (employee.Count > 0)
                {
                    res.Message = "Employee Found";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = new
                    {
                        IsAdmin = tokenData.IsAdminInCompany,
                        EmployeeList = employee.OrderBy(x => x.DisplayName).ToList(),
                    };

                    return Ok(res);
                }
                res.Message = "No Employee Found";
                res.Status = false;
                res.StatusCode = HttpStatusCode.NotFound;
                res.Data = employee;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/orgmaster/getunassignreportingmanagers | " +
                    //" Model : " + JsonConvert.SerializeObject(model) + " |" +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API To Get Employee List For Assigning Reporting Manager

        [HttpGet]
        [Route("getemployeelistonassigningreportingmanager")]
        public async Task<ResponseBodyModel> GetEmployeeListForAssigningReportingManager()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var emp = await (from e in _db.Employee
                                 join d in _db.Designation on e.DesignationId equals d.DesignationId
                                 where e.CompanyId == claims.companyId
                                 select new HerachaiData
                                 {
                                     Id = e.EmployeeId,
                                     ParentId = e.ReportingManager,
                                     Name = e.DisplayName,
                                     Image = e.ProfileImageUrl,
                                     Title = e.EmployeeTypeId == EmployeeTypeConstants.Ex_Employee ? "Ex Employee" : d.DesignationName,



                                 }).ToListAsync();
                res.Message = "Org Chart";
                res.Status = true;
                res.Data = BuildTree(emp);
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class HierarchyMember
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string CssClass { get; set; } = "ngx-org-ceo";
            public string Image { get; set; }
            public string Title { get; set; }
            public List<HierarchyMember> Childs { get; } = new List<HierarchyMember>();
        }
        public static IEnumerable<HierarchyMember> BuildTree(List<HerachaiData> hierarchy)
        {
            var dictionary = hierarchy.ToDictionary(p => p.Id, p => new HierarchyMember
            {
                Id = p.Id,
                Name = p.Name,
                Image = p.Image,
                Title = p.Title,
            });
            foreach (var item in hierarchy)
            {
                if ((item.Id != item.ParentId) && item.ParentId != 0)
                {
                    dictionary[item.ParentId].Childs.Add(dictionary[item.Id]);
                }
            }
            var nonRoots = dictionary.Values.SelectMany(p => p.Childs).ToHashSet();
            var roots = dictionary.Values.Where(p => !nonRoots.Contains(p)).ToList();
            return roots;
        }
        public class HerachaiData
        {
            public int Id { get; set; }
            public int ParentId { get; set; }
            public string Name { get; set; }
            public string Image { get; set; }
            public string Title { get; set; }
        }
        #endregion

        #region API For New Org Tree Of Employee
        /// <summary>
        /// Created By Harshit Mitra On 10-10-2022
        /// API >> Get >> api/orgmaster/getemployeeorgtreenode
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployeeorgtreenode")]
        public async Task<ResponseBodyModel> NewOrgTreeOfEmployee(string baseURL)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                Random random = new Random();
                var emp = await (from e in _db.Employee
                                 join ds in _db.Designation on e.DesignationId equals ds.DesignationId
                                 join de in _db.Department on e.DepartmentId equals de.DepartmentId
                                 where e.CompanyId == claims.companyId
                                 select new
                                 {
                                     NodeId = e.EmployeeId,
                                     ParentNodeId = e.ReportingManager,
                                     e.Gender,
                                     e.ProfileImageUrl,
                                     e.IsPresident,
                                     Template = templateObject
                                            .Replace("<|DISPLAYNAME|>", e.DisplayName)
                                            .Replace("<|DESIGNATION|>", ds.DesignationName)
                                            .Replace("<|OFFICALEMAIL|>", e.OfficeEmail)
                                            .Replace("<|DEPARTMENT|>", de.DepartmentName),

                                 }).ToListAsync();
                List<dynamic> empList = new List<dynamic>();
                empList.AddRange(emp.Where(x => x.ParentNodeId != 0).ToList());
                var correctList = CheckAndRemoveExtraNode(
                        empList.Where(x => x.ParentNodeId != 0).ToList(),
                        emp.Where(x => x.IsPresident).Select(x => x.NodeId).FirstOrDefault()
                    );
                //var correctList = empList;
                correctList.AddRange(emp.Where(x => x.IsPresident).ToList());

                var gender = new List<string> { "male", "female" };
                var nodeList = correctList.Select(x => new NewOrgTreeResponse
                {
                    NodeId = x.NodeId.ToString(),
                    ParentNodeId = x.IsPresident ? null : x.ParentNodeId.ToString(),
                    NodeImage = new
                    {
                        Url = baseURL + (!String.IsNullOrEmpty(x.ProfileImageUrl) ? x.ProfileImageUrl :
                                                    x.Gender != GenderConstants.Other.ToString() ?
                                                    "uploadimage\\noimagefound\\" + x.Gender.ToLower() + random.Next(1, 4) + ".jpg"
                                                    : "uploadimage\\noimagefound\\" + gender[random.Next(gender.Count)] + random.Next(4) + ".jpg"),
                        Widht = 100,
                        Height = 100,
                        CenterTopDistance = 55,
                        CenterLeftDistance = 55,
                        CornerShape = "CIRCLE",
                        Shadow = false,
                        BorderWidth = 0,
                        BorderColor = new
                        {
                            Red = 19,
                            Green = 123,
                            Blue = 128,
                            Alpha = 1
                        }
                    },
                    Template = x.Template,
                }).ToList()
                .Where(x => x.ParentNodeId != "0")
                .ToList();
                if (nodeList.Count > 0 && nodeList.Any(x => x.ParentNodeId == null))
                {
                    res.Message = "Org Chart";
                    res.Status = true;
                    res.Data = nodeList;
                }
                else
                {
                    res.Message = !nodeList.Any(x => x.ParentNodeId == null) ?
                            "Top Of The Org Is Missging" :
                            "No Reporting Mannager Assign";
                    res.Status = false;
                    res.Data = nodeList;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }
        public class NewOrgTreeResponse
        {
            public string NodeId { get; set; }
            public string ParentNodeId { get; set; }
            public dynamic NodeImage { get; set; }
            public string Template { get; set; }
        }
        public static readonly string templateObject =
            "<div>  \n  <div style= \"margin-left:130px;  \n  margin-top:10px;  \n font-size:20px; \n font-weight:bold; \n color:black \n \">" + "<|DISPLAYNAME|>" +
            "</div> \n <div style=\"margin-left:130px; \n margin-top:3px; \n font-size:16px; \n color:black \n \">" + "<|DESIGNATION|>" +
            "</div> \n  \n <div style=\"margin-left:130px; \n margin-top:3px; \n font-size:14px; \n color:black \n \">" + "<|OFFICALEMAIL|>" +
            "</div> \n  \n <div style=\"margin-left:196px; \n margin-top:15px; \n font-size:13px; \n position:absolute; \n bottom:5px; \n color:black \n \"> \n <div>" +
            "</div> \n <div style=\"margin-top:5px \n color:black \n \">" + "<|DEPARTMENT|>" + "</div> \n </div> \n </div>";
        public static List<dynamic> CheckAndRemoveExtraNode(List<dynamic> nodeList, int masterNode)
        {
            var nodeIds = nodeList.Select(x => (int)x.NodeId).Distinct().ToList();
            nodeIds.Add(masterNode);
            var parentNode = nodeList.Select(x => (int)x.ParentNodeId).Distinct().ToList();

            var removeData = parentNode.Except(nodeIds).ToList();
            if (removeData.Count == 0)
                return nodeList;
            nodeList.RemoveAll(x => removeData.Contains(x.ParentNodeId));
            return CheckAndRemoveExtraNode(nodeList, masterNode);
        }
        #endregion
    }
}