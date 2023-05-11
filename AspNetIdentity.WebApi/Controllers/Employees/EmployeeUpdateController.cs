using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Models;
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

namespace AspNetIdentity.WebApi.Controllers.Employees
{
    /// <summary>
    /// Created By Harshit Mitra On 15-03-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/employeeupdate")]
    public class EmployeeUpdateController : BaseApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API TO GET EMPLOYEE LIST ON EMPLOYEE UPDATE 
        /// <summary>
        /// Created By Harshit Mitra On 30-01-2023
        /// API >> POST >> api/employeeupdate/emplistonupdate
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("emplistonupdate")]
        public async Task<IHttpActionResult> GetAllEmployeeListOnUpdate(FilterRequest filterModel)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employeList = await (from e in _db.Employee
                                         join de in _db.Department on e.DepartmentId equals de.DepartmentId
                                         join ds in _db.Designation on e.DesignationId equals ds.DesignationId
                                         join ue in _db.Employee on e.UpdatedBy equals ue.EmployeeId into updEmp
                                         from u in updEmp.DefaultIfEmpty()
                                         join or in _db.OrgMaster on e.OrgId equals or.OrgId into orEmp
                                         from o in orEmp.DefaultIfEmpty()
                                         join rp in _db.Employee on e.ReportingManager equals rp.EmployeeId into rpEmp
                                         from r in rpEmp.DefaultIfEmpty()
                                         where e.IsActive && !e.IsDeleted && e.CompanyId == tokenData.companyId &&
                                            e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee
                                         select new GetEmployeeListOnUpdateResponse
                                         {
                                             EmployeeId = e.EmployeeId,
                                             DisplayName = e.DisplayName,
                                             OfficeEmail = e.OfficeEmail,
                                             DepartmentId = de.DepartmentId,
                                             DepartmentName = de.DepartmentName,
                                             DesignationId = ds.DesignationId,
                                             DesignationName = ds.DesignationName,
                                             OrgId = o != null ? o.OrgId : 0,
                                             OrgName = o != null ? o.OrgName : String.Empty,
                                             ReportingManagerId = r != null ? r.EmployeeId : 0,
                                             ReportingManagerName = r != null ? r.DisplayName : String.Empty,
                                             UpdateOn = u != null ? e.UpdatedOn : null,
                                             UpdateBy = u != null ? u.DisplayName : String.Empty,
                                         })
                                         .OrderBy(x => x.DisplayName)
                                         .ThenBy(x => x.DepartmentId)
                                         .ThenBy(x => x.DesignationName)
                                         .ToListAsync();

                employeList = GetFilterData(employeList, filterModel);

                var searchString = !String.IsNullOrEmpty(filterModel.SearchString)
                    ? filterModel.SearchString.Trim().Length > 2
                        ? filterModel.SearchString.Trim().ToUpper() : null : null;
                if (!String.IsNullOrEmpty(searchString))
                {
                    employeList = employeList
                        .Where(x =>
                            x.DisplayName.ToUpper().Contains(searchString) ||
                            x.OfficeEmail.ToUpper().Contains(searchString) ||
                            x.DepartmentName.ToUpper().Contains(searchString) ||
                            x.DesignationName.ToUpper().Contains(searchString) ||
                            x.ReportingManagerName.ToUpper().Contains(searchString)
                            )
                        .ToList();
                }
                res.Message = "Employee List For Update";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = employeList;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/employeeupdate/emplistonupdate | " +
                    "Filter Model : " + JsonConvert.SerializeObject(filterModel) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class FilterRequest
        {
            public string SearchString { get; set; } = String.Empty;
            public FilterEnum FilterFor { get; set; } = FilterEnum.SetDefault;
            public bool IsDecs { get; set; } = false;
        }
        public enum FilterEnum
        {
            SetDefault,
            DisplayName,
            OfficeEmail,
            DepartmentName,
            DesignationName,
            ReportingManagerName,
            UpdateOn,
            OrgName,
        }
        public class GetEmployeeListOnUpdateResponse
        {
            public int EmployeeId { get; set; } = 0;
            public string DisplayName { get; set; } = String.Empty;
            public string OfficeEmail { get; set; } = String.Empty;
            public int DepartmentId { get; set; } = 0;
            public string DepartmentName { get; set; } = String.Empty;
            public int DesignationId { get; set; } = 0;
            public string DesignationName { get; set; } = String.Empty;
            public int OrgId { get; set; } = 0;
            public string OrgName { get; set; } = String.Empty;
            public int ReportingManagerId { get; set; } = 0;
            public string ReportingManagerName { get; set; } = String.Empty;
            public DateTimeOffset? UpdateOn { get; set; } = null;
            public string UpdateBy { get; set; } = String.Empty;
        }
        public List<GetEmployeeListOnUpdateResponse> GetFilterData(List<GetEmployeeListOnUpdateResponse> list, FilterRequest filter)
        {
            if (!filter.IsDecs)
            {
                switch (filter.FilterFor)
                {
                    case FilterEnum.DisplayName:
                        list = list.OrderBy(x => x.DisplayName).ToList();
                        break;
                    case FilterEnum.OfficeEmail:
                        list = list.OrderBy(x => x.OfficeEmail).ToList();
                        break;
                    case FilterEnum.DepartmentName:
                        list = list.OrderBy(x => x.DepartmentName).ToList();
                        break;
                    case FilterEnum.DesignationName:
                        list = list.OrderBy(x => x.DesignationName).ToList();
                        break;
                    case FilterEnum.ReportingManagerName:
                        list = list.OrderBy(x => x.ReportingManagerName).ToList();
                        break;
                    case FilterEnum.UpdateOn:
                        list = list.OrderBy(x => x.UpdateOn).ToList();
                        break;
                    case FilterEnum.OrgName:
                        list = list.OrderBy(x => x.OrgName).ToList();
                        break;
                }
            }
            else
            {
                switch (filter.FilterFor)
                {
                    case FilterEnum.DisplayName:
                        list = list.OrderByDescending(x => x.DisplayName).ToList();
                        break;
                    case FilterEnum.OfficeEmail:
                        list = list.OrderByDescending(x => x.OfficeEmail).ToList();
                        break;
                    case FilterEnum.DepartmentName:
                        list = list.OrderByDescending(x => x.DepartmentName).ToList();
                        break;
                    case FilterEnum.DesignationName:
                        list = list.OrderByDescending(x => x.DesignationName).ToList();
                        break;
                    case FilterEnum.ReportingManagerName:
                        list = list.OrderByDescending(x => x.ReportingManagerName).ToList();
                        break;
                    case FilterEnum.UpdateOn:
                        list = list.OrderByDescending(x => x.ReportingManagerName).ToList();
                        break;
                    case FilterEnum.OrgName:
                        list = list.OrderByDescending(x => x.OrgName).ToList();
                        break;
                }
            }
            return list;
        }
        #endregion

        #region API TO GET EMPLOYEE BY ID FOR UPDATE
        /// <summary>
        /// Created By Harshit Mitra On 15-03-2023
        /// API >> GET >> api/employeeupdate/getemployeebyidforupdate
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployeebyidforupdate")]
        public async Task<IHttpActionResult> GetEmployeeByIdForUpdate(int employeeId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employee = await (from e in _db.Employee
                                      join de in _db.Department on e.DepartmentId equals de.DepartmentId
                                      join ds in _db.Designation on e.DesignationId equals ds.DesignationId
                                      join ue in _db.Employee on e.UpdatedBy equals ue.EmployeeId into updEmp
                                      from u in updEmp.DefaultIfEmpty()
                                      join or in _db.OrgMaster on e.OrgId equals or.OrgId into orEmp
                                      from o in orEmp.DefaultIfEmpty()
                                      join rp in _db.Employee on e.ReportingManager equals rp.EmployeeId into rpEmp
                                      from r in rpEmp.DefaultIfEmpty()
                                      where e.EmployeeId == employeeId
                                      select new
                                      {
                                          EmployeeId = e.EmployeeId,
                                          DisplayName = e.DisplayName,
                                          OfficeEmail = e.OfficeEmail,
                                          DepartmentId = de.DepartmentId,
                                          DepartmentName = de.DepartmentName,
                                          DesignationId = ds.DesignationId,
                                          DesignationName = ds.DesignationName,
                                          OrgId = o != null ? o.OrgId : 0,
                                          OrgName = o != null ? o.OrgName : String.Empty,
                                          ReportingManagerId = r != null ? r.EmployeeId : 0,
                                          ReportingManagerName = r != null ? r.DisplayName : String.Empty,
                                      })
                                      .FirstOrDefaultAsync();
                if (employee == null)
                {
                    res.Message = "Employee Not Found";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = employee;
                }
                res.Message = "Employee Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = employee;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/employeeupdate/getemployeebyidforupdate | " +
                    "Employee Id: " + employeeId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET EMPLOYEE WITHOUT SELF
        /// <summary>
        /// Created By Harshit Mitra On 15-03-2023
        /// API >> GET >> api/employeeupdate/getemployeebywithoutself
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployeebywithoutself")]
        public async Task<IHttpActionResult> GetEmployeeByWithoutSelf(int employeeId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employee = await (from e in _db.Employee
                                      where e.EmployeeId != employeeId && e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee && e.IsActive && !e.IsDeleted && e.CompanyId == tokenData.companyId
                                      select new
                                      {
                                          EmployeeId = e.EmployeeId,
                                          DisplayName = e.DisplayName,
                                          OfficeEmail = e.OfficeEmail,
                                      })
                                      .OrderBy(x => x.DisplayName)
                                      .ToListAsync();
                if (employee == null)
                {
                    res.Message = "Employee Not Found";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = employee;
                }
                res.Message = "Employee Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = employee;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/employeeupdate/getemployeebywithoutself | " +
                    "Employee Id: " + employeeId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET ALL DEPARTMENT 
        /// <summary>
        /// Created By Harshit Mitra On 15-03-2023
        /// API >> GET >> api/employeeupdate/getdepartmentempupdate
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getdepartmentempupdate")]
        public async Task<IHttpActionResult> GetAllDepartmentForEmpUpdate()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var department = await (from de in _db.Department
                                        where de.CompanyId == tokenData.companyId && de.IsActive && !de.IsDeleted
                                        select new
                                        {
                                            DepartmentId = de.DepartmentId,
                                            DepartmentName = de.DepartmentName,
                                        })
                                        .OrderBy(x => x.DepartmentName)
                                        .ToListAsync();
                if (department.Count == 0)
                {
                    res.Message = "Department Not Found";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = department;
                }
                res.Message = "department Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = department;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/employeeupdate/getdepartmentempupdate | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET ALL DEIGNATIONS
        /// <summary>
        /// Created By Harshit Mitra On 15-03-2023
        /// API >> GET >> api/employeeupdate/getdesignationempupdate
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getdesignationempupdate")]
        public async Task<IHttpActionResult> GetAllDesignationForEmpUpdate(int departmentId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var designation = await (from de in _db.Designation
                                         where de.CompanyId == tokenData.companyId && de.IsActive &&
                                             !de.IsDeleted && de.DepartmentId == departmentId
                                         select new
                                         {
                                             DesignationId = de.DesignationId,
                                             DesignationName = de.DesignationName,
                                         })
                                         .OrderBy(x => x.DesignationName)
                                         .ToListAsync();
                if (designation.Count == 0)
                {
                    res.Message = "Designation Not Found";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = designation;
                }
                res.Message = "Designation Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = designation;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/employeeupdate/getdesignationempupdate | " +
                    "Department Id : " + departmentId +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET ALL ORGANIZATION
        /// <summary>
        /// Created By Harshit Mitra On 15-03-2023
        /// API >> GET >> api/employeeupdate/getorganiationempupdate
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getorganiationempupdate")]
        public async Task<IHttpActionResult> GetAllOrganizationForEmpUpdate()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var organization = await (from x in _db.OrgMaster
                                          where x.CompanyId == tokenData.companyId && x.IsActive && !x.IsDeleted
                                          select new
                                          {
                                              OrgId = x.OrgId,
                                              OrgName = x.OrgName,
                                          })
                                         .OrderBy(x => x.OrgName)
                                         .ToListAsync();
                if (organization.Count == 0)
                {
                    res.Message = "Designation Not Found";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = organization;
                }
                res.Message = "Organization Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = organization;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/employeeupdate/getorganiationempupdate | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO CHANGE EMPLOYEE NEW OFFICAL MAILS, DEPARTMENT, DESEGNATION, REPORTING MANAGER
        /// <summary>
        /// Created By Harshit Mitra On 30-01-2023
        /// API >> POST >> api/employeeupdate/employeeemailchange
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("employeeemailchange")]
        public async Task<IHttpActionResult> UpdateEmployeeOfficalMail(NewEmailChangeRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employee = await _db.Employee
                    .FirstOrDefaultAsync(x => x.EmployeeId == model.EmployeeId);
                if (employee == null)
                {
                    res.Message = "Employee Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    return Ok(res);
                }
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                var users = await _db.User.FirstOrDefaultAsync(x => x.UserName == employee.OfficeEmail);
                if (await _db.Employee.AnyAsync(x => x.OfficeEmail == model.NewEmail))
                {
                    res.Message = "This Email Is Allready In Use";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotAcceptable;
                    return Ok(res);
                }
                if (!String.IsNullOrEmpty(model.NewEmail))
                {
                    var applicationUser = await this.AppUserManager.FindByNameAsync(employee.OfficeEmail);
                    applicationUser.UserName = model.NewEmail;
                    await this.AppUserManager.UpdateAsync(applicationUser);

                    users.UserName = model.NewEmail;
                    _db.Entry(users).State = EntityState.Modified;

                    employee.OfficeEmail = model.NewEmail;
                }
                if (model.OrgId != 0)
                    employee.OrgId = model.OrgId;
                if (model.DepartmentId != 0)
                    employee.DepartmentId = model.DepartmentId;
                if (model.DesignationId != 0)
                    employee.DesignationId = model.DesignationId;
                if (model.ReportingManagerId != 0)
                    employee.ReportingManager = model.ReportingManagerId;
                employee.UpdatedBy = tokenData.employeeId;
                employee.UpdatedOn = today;
                _db.Entry(employee).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                _db.Dispose();

                res.Message = "Employee Details Update Successfull";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/employeeupdate/employeeemailchange | " +
                    "Filter Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class OldEmailClass : BaseNewEmailClass
        {
            public string OldEmail { get; set; } = String.Empty;
        }

        public class BaseNewEmailClass
        {
            public string NewEmail { get; set; } = String.Empty;
            public int OrgId { get; set; } = 0;
            public int DepartmentId { get; set; } = 0;
            public int DesignationId { get; set; } = 0;
            public int ReportingManagerId { get; set; } = 0;
        }
        public class NewEmailChangeRequest : BaseNewEmailClass
        {
            public int EmployeeId { get; set; } = 0;
        }

        #endregion

        #region API TO EXPORT EMPLOYEE NEW OFFICAL MAILS, DEPARTMENT, DESEGNATION, REPORTING MANAGER EXCEL EXPORT
        /// <summary>
        /// Created By Harshit Mitra On 30-01-2023
        /// API >> POST >> api/employeeupdate/employeeemailchangeexcel
        /// </summary>
        /// <param name="modelList"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("employeeemailchangeexcel")]
        public async Task<IHttpActionResult> UpdateEmployeeOfficalMailExcel(List<ExcelExportClass> modelList)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (modelList.Count == 0)
                {
                    res.Message = "No Data In File";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    return Ok(res);
                }
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                List<FaultyListChangeEmail> faulty = new List<FaultyListChangeEmail>();
                faulty.AddRange(modelList
                    .Where(x => x.OldEmail == x.NewEmail)
                    .Select(x => new FaultyListChangeEmail
                    {
                        EmployeeName = x.EmployeeName,
                        OldEmail = x.OldEmail,
                        NewEmail = x.NewEmail,
                        OrgName = x.OrgName,
                        DepartmentName = x.DepartmentName,
                        DesignationName = x.DesignationName,
                        ReportingManagerName = x.ReportingManagerName,
                        ReportingManagerOfficalEmail = x.ReportingManagerOfficalEmail,
                        Reason = "Old Email and New Email Are Same",
                    }).ToList());
                modelList = modelList.Where(x => x.OldEmail != x.NewEmail).ToList();
                var departmentList = await _db.Department
                    .Where(x => x.IsActive && !x.IsDeleted && tokenData.companyId == x.CompanyId)
                    .Select(x => new { x.DepartmentId, DepartmentName = x.DepartmentName.ToUpper() })
                    .ToListAsync();
                var designationsList = await _db.Designation
                    .Where(x => x.IsActive && !x.IsDeleted && tokenData.companyId == x.CompanyId)
                    .Select(x => new { x.DepartmentId, x.DesignationId, DesignationName = x.DesignationName.ToUpper() })
                    .ToListAsync();
                var reportingManagersList = await _db.Employee
                    .Where(x => x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee && tokenData.companyId == x.CompanyId)
                    .Select(x => new { x.EmployeeId, DisplayName = x.DisplayName.ToUpper(), OfficeEmail = x.OfficeEmail.ToUpper() })
                    .ToListAsync();
                var organizationList = await _db.OrgMaster
                    .Where(x => x.CompanyId == tokenData.companyId && x.IsActive && !x.IsDeleted)
                    .Select(x => new { x.OrgId, OrgName = x.OrgName.ToUpper() })
                    .ToListAsync();
                foreach (var model in modelList)
                {
                    var employee = await _db.Employee
                        .FirstOrDefaultAsync(x => x.OfficeEmail == model.OldEmail);
                    if (employee != null)
                    {
                        var departmenId = departmentList.Where(x => x.DepartmentName == model.DepartmentName.ToUpper()).FirstOrDefault();
                        if (departmenId == null)
                        {
                            faulty.Add(new FaultyListChangeEmail
                            {
                                EmployeeName = model.EmployeeName,
                                OldEmail = model.OldEmail,
                                NewEmail = model.NewEmail,
                                OrgName = model.OrgName,
                                DepartmentName = model.DepartmentName,
                                DesignationName = model.DesignationName,
                                ReportingManagerName = model.ReportingManagerName,
                                ReportingManagerOfficalEmail = model.ReportingManagerOfficalEmail,
                                Reason = "Department Not Found",
                            });
                            continue;
                        }
                        var designationId = designationsList
                            .Where(x => x.DesignationName == model.DesignationName.ToUpper() && x.DepartmentId == departmenId.DepartmentId)
                            .FirstOrDefault();
                        if (designationId == null)
                        {
                            faulty.Add(new FaultyListChangeEmail
                            {
                                EmployeeName = model.EmployeeName,
                                OldEmail = model.OldEmail,
                                NewEmail = model.NewEmail,
                                OrgName = model.OrgName,
                                DepartmentName = model.DepartmentName,
                                DesignationName = model.DesignationName,
                                ReportingManagerName = model.ReportingManagerName,
                                ReportingManagerOfficalEmail = model.ReportingManagerOfficalEmail,
                                Reason = "Designation Not Found Or Not In The Selected Department",
                            });
                            continue;
                        }
                        var reportingManagersId = reportingManagersList
                            .Where(x => x.OfficeEmail == model.ReportingManagerOfficalEmail.ToUpper())
                            .FirstOrDefault();
                        if (reportingManagersId == null)
                        {
                            faulty.Add(new FaultyListChangeEmail
                            {
                                EmployeeName = model.EmployeeName,
                                OldEmail = model.OldEmail,
                                NewEmail = model.NewEmail,
                                OrgName = model.OrgName,
                                DepartmentName = model.DepartmentName,
                                DesignationName = model.DesignationName,
                                ReportingManagerName = model.ReportingManagerName,
                                ReportingManagerOfficalEmail = model.ReportingManagerOfficalEmail,
                                Reason = "Reporting Manager Not Found",
                            });
                            continue;
                        }
                        var orgId = organizationList
                            .Where(x => x.OrgName == model.OrgName.ToUpper())
                            .FirstOrDefault();
                        if (orgId == null)
                        {
                            faulty.Add(new FaultyListChangeEmail
                            {
                                EmployeeName = model.EmployeeName,
                                OldEmail = model.OldEmail,
                                NewEmail = model.NewEmail,
                                OrgName = model.OrgName,
                                DepartmentName = model.DepartmentName,
                                DesignationName = model.DesignationName,
                                ReportingManagerName = model.ReportingManagerName,
                                ReportingManagerOfficalEmail = model.ReportingManagerOfficalEmail,
                                Reason = "Orginazation Not Found",
                            });
                            continue;
                        }
                        if (await _db.Employee.AnyAsync(x => x.OfficeEmail == model.NewEmail))
                        {
                            faulty.Add(new FaultyListChangeEmail
                            {
                                EmployeeName = model.EmployeeName,
                                OldEmail = model.OldEmail,
                                NewEmail = model.NewEmail,
                                OrgName = model.OrgName,
                                DepartmentName = model.DepartmentName,
                                DesignationName = model.DesignationName,
                                ReportingManagerName = model.ReportingManagerName,
                                ReportingManagerOfficalEmail = model.ReportingManagerOfficalEmail,
                                Reason = "This Email Is Allready In Use",
                            });
                            continue;
                        }
                        try
                        {
                            if (!String.IsNullOrEmpty(model.NewEmail))
                            {
                                var applicationUser = await this.AppUserManager.FindByNameAsync(employee.OfficeEmail);
                                applicationUser.UserName = model.NewEmail;
                                await this.AppUserManager.UpdateAsync(applicationUser);

                                var users = await _db.User.FirstOrDefaultAsync(x => x.UserName == employee.OfficeEmail);
                                users.UserName = model.NewEmail;
                                _db.Entry(users).State = EntityState.Modified;

                                employee.OfficeEmail = model.NewEmail;
                            }
                            employee.DepartmentId = departmenId.DepartmentId;
                            employee.DesignationId = designationId.DesignationId;
                            employee.ReportingManager = reportingManagersId.EmployeeId;
                            employee.UpdatedBy = tokenData.employeeId;
                            employee.UpdatedOn = today;
                            _db.Entry(employee).State = EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }
                        catch (Exception)
                        {
                            faulty.Add(new FaultyListChangeEmail
                            {
                                EmployeeName = model.EmployeeName,
                                OldEmail = model.OldEmail,
                                NewEmail = model.NewEmail,
                                OrgName = model.OrgName,
                                DepartmentName = model.DepartmentName,
                                DesignationName = model.DesignationName,
                                ReportingManagerName = model.ReportingManagerName,
                                ReportingManagerOfficalEmail = model.ReportingManagerOfficalEmail,
                                Reason = "Employe Not Found"
                            });
                        }
                    }
                    else
                    {
                        faulty.Add(new FaultyListChangeEmail
                        {
                            EmployeeName = model.EmployeeName,
                            OldEmail = model.OldEmail,
                            NewEmail = model.NewEmail,
                            OrgName = model.OrgName,
                            DepartmentName = model.DepartmentName,
                            DesignationName = model.DesignationName,
                            ReportingManagerName = model.ReportingManagerName,
                            ReportingManagerOfficalEmail = model.ReportingManagerOfficalEmail,
                            Reason = "Employe Not Found"
                        });
                    }
                }
                res.Message = faulty.Count == 0 ? "E-Mail Change Successfull" :
                    "E-Mail Change Successfull But Not For " + faulty.Count + " Employees";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = faulty;
                _db.Dispose();
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/employeenew/employeeemailchangeexcel | " +
                    "Filter Model : " + JsonConvert.SerializeObject(modelList) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class ExcelExportClass
        {
            public string EmployeeName { get; set; } = String.Empty;
            public string OldEmail { get; set; } = String.Empty;
            public string NewEmail { get; set; } = String.Empty;
            public string OrgName { get; set; } = String.Empty;
            public string DepartmentName { get; set; } = String.Empty;
            public string DesignationName { get; set; } = String.Empty;
            public string ReportingManagerName { get; set; } = String.Empty;
            public string ReportingManagerOfficalEmail { get; set; } = String.Empty;
        }
        public class FaultyListChangeEmail : ExcelExportClass
        {
            public string Reason { get; set; } = String.Empty;
        }
        #endregion

    }
}
