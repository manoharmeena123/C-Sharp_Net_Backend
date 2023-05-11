using AspNetIdentity.WebApi.Infrastructure;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers
{
    [RoutePrefix("api/employees")]
    public class EmployeeController : BaseApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        #region#Super User Dashboard APIs
        //#region#API to get Companywise-OrgWise Employees OnRoll
        //[HttpGet]
        //[Authorize]
        //[Route("GetCompanywiseOrgWiseEmployeeCount")]

        //public async Task<ResponseBodyModel> GetCompanywiseOrgWiseEmployeeCount()
        //{
        //    ResponseBodyModel response = new ResponseBodyModel();
        //    try
        //    {
        //        List<CompOrgData> DTO = new List<CompOrgData>();
        //        string sqlquery = "Select c.CompanyName, o.OrgName, e.CompanyId,  e.orgid, count(employeeId) as Count " +
        //                          "From Employees e inner join Companies c on e.CompanyId = c.CompanyId inner join OrgMasters o on e.OrgId = O.OrgId " +
        //                          "Where e.EmployeeTypeID <> 3 and e.IsActive = 1 and e.IsDeleted = 0 " +
        //                          "Group by c.CompanyName, o.OrgName, e.CompanyId,  e.orgid Order by E.CompanyId, E.OrgId";

        //        DTO = _db.Database.SqlQuery<CompOrgData>(sqlquery).ToList();

        //        if (DTO.Count > 0)
        //        {
        //            response.Message = "Employee count Data found!";
        //            response.Status = true;
        //            response.Data = DTO;
        //        }
        //        else
        //        {
        //            response.Message = "Data not found!";
        //            response.Status = false;
        //            response.Data = null;
        //        }

        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        {
        //            response.Message = ex.Message;
        //            response.Status = false;
        //            response.Data = null;
        //            return response;
        //        }

        //    }
        //}
        //#endregion
        //#region#API to get Monthwise Employee Added Count
        //[HttpGet]
        //[Authorize]
        //[Route("GetEmployeeAddedCount")]

        //public async Task<ResponseBodyModel> GetEmployeeAddedCount(int orgId, int intMonth, int intYear)
        //{
        //    ResponseBodyModel response = new ResponseBodyModel();
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 1;
        //        int orgid = 0;

        //        Access claims
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);
        //        if (compid == 0)
        //        { compid = 1; }
        //        DateTime currentDate = DateTime.Now;
        //        DateTime fromdate = new DateTime(intYear, intMonth, 1, 0, 0, 0);
        //        DateTime endDate = fromdate.AddMonths(1).AddDays(-1);
        //        DateTime todate = new DateTime(intYear, endDate.Month, endDate.Day, 12, 59, 59);
        //        string strMonth = endDate.ToString("MMMM");

        //        CompOrgDataForMonth DTO = new CompOrgDataForMonth();
        //        string sqlquery = "Select CompanyId, OrgId, " + intMonth.ToString() + ",'" + strMonth + "',Count(EmployeeId) As Cnt " +
        //                          "From Employees " +
        //                          "Where JoiningDate between '" + fromdate + "' and '" + endDate + "' and IsActive = 1 and IsDeleted = 0 And CompanyId = " + compid + " And OrgId = " + orgId + " Group By CompanyId, OrgId, " + intMonth.ToString();

        //        DTO = _db.Database.SqlQuery<CompOrgDataForMonth>(sqlquery).FirstOrDefault();

        //        if (DTO != null)
        //        {
        //            response.Message = "Employee count Data found!";
        //            response.Status = true;
        //            response.Data = DTO;
        //        }
        //        else
        //        {
        //            response.Message = "Data not found!";
        //            response.Status = false;
        //            response.Data = null;
        //        }

        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        {
        //            response.Message = ex.Message;
        //            response.Status = false;
        //            response.Data = null;
        //            return response;
        //        }

        //    }
        //}
        //#endregion
        #endregion

        //#region Api for Get EmployeeTree
        //[HttpGet]

        //[Route("GetEmployeeTree")]
        //[Authorize]
        //public async Task<object> GetEmployeeTree()
        //{
        //    using (var db = new ApplicationDbContext())
        //    {
        //        try
        //        {
        //            var identity = User.Identity as ClaimsIdentity;
        //            int userid = 0;
        //            int compid = 0;
        //            int orgid = 0;

        //            // Access claims

        //            if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //                userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //            if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //                compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //            if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //                orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //            List<EmpolyeTree> res = new List<EmpolyeTree>();
        //            EmpolyeTree Res = new EmpolyeTree();
        //            var Employee = (from E in db.Employee
        //                            join R in db.Role on E.RoleId equals R.RoleId
        //                            where E.ReportingManager != "" && E.ReportingManager != null
        //                            && E.EmployeeTypeId != 3
        //                            && E.CompanyId == compid && E.OrgId == orgid
        //                            select new EmpolyeTreeDC
        //                            {
        //                                Id = E.EmployeeId,
        //                                name = E.FirstName + " " + E.LastName,
        //                                title = R.RoleType,
        //                                image = E.ProfileImageUrl,
        //                                // ReportingManager = E.ReportingManager,
        //                                cssClass = "ngx-org-ceo"
        //                            }).ToList();
        //            foreach (var Emp in Employee)
        //            {
        //                var newEmp = (from E in db.Employee
        //                              join R in db.Role on E.RoleId equals R.RoleId
        //                              where E.ReportingManager == "Emp.Id" && E.CompanyId == compid && E.OrgId == orgid
        //                              select new EmpolyeTree
        //                              {
        //                                  Id = E.EmployeeId,
        //                                  name = E.FirstName + " " + E.LastName,
        //                                  title = R.RoleType,
        //                                  image = E.ProfileImageUrl,
        //                                  cssClass = "ngx-org-ceo"
        //                              }).ToList();
        //                if (newEmp.Count != 0)
        //                {
        //                    Res.Id = Emp.Id;
        //                    Res.name = Emp.name;
        //                    Res.title = Emp.title;
        //                    Res.image = Emp.image;
        //                    Res.childs = newEmp;

        //                    res.Add(Res); foreach (var emp in newEmp)
        //                    {
        //                        var newwEmp = (from E in db.Employee
        //                                       join R in db.Role on E.RoleId equals R.RoleId
        //                                       where E.ReportingManager == "emp.Id"
        //                                       select new EmpolyeTree
        //                                       {
        //                                           Id = E.EmployeeId,
        //                                           name = E.FirstName + " " + E.LastName,
        //                                           title = R.RoleType,
        //                                           image = E.ProfileImageUrl,
        //                                           cssClass = "ngx-org-ceo"
        //                                       }).ToList();
        //                        if (newwEmp.Count != 0)
        //                        {
        //                            emp.childs = newwEmp; foreach (var ep in newwEmp)
        //                            {
        //                                var newwwwEmp = (from E in db.Employee
        //                                                 join R in db.Role on E.RoleId equals R.RoleId
        //                                                 where E.ReportingManager == "ep.Id"
        //                                                 select new EmpolyeTree
        //                                                 {
        //                                                     Id = E.EmployeeId,
        //                                                     name = E.FirstName + " " + E.LastName,
        //                                                     title = R.RoleType,
        //                                                     image = E.ProfileImageUrl,
        //                                                     cssClass = "ngx-org-ceo"
        //                                                 }).ToList();
        //                                if (newwwwEmp.Count != 0)
        //                                {
        //                                    ep.childs = newwwwEmp; foreach (var epm in newwwwEmp)
        //                                    {
        //                                        var newwwwwEmp = (from E in db.Employee
        //                                                          join R in db.Role on E.RoleId equals R.RoleId
        //                                                          where E.ReportingManager == "epm.Id"
        //                                                          select new EmpolyeTree
        //                                                          {
        //                                                              Id = E.EmployeeId,
        //                                                              name = E.FirstName + " " + E.LastName,
        //                                                              title = R.RoleType,
        //                                                              image = E.ProfileImageUrl,
        //                                                              cssClass = "ngx-org-ceo"
        //                                                          }).ToList();
        //                                        if (newwwwwEmp.Count != 0)
        //                                        {
        //                                            epm.childs = newwwwwEmp;
        //                                            foreach (var empp in newwwwwEmp)
        //                                            {
        //                                                var newwwwwwEmp = (from E in db.Employee
        //                                                                   join R in db.Role on E.RoleId equals R.RoleId
        //                                                                   where E.ReportingManager == "empp.Id"
        //                                                                   select new EmpolyeTree
        //                                                                   {
        //                                                                       Id = E.EmployeeId,
        //                                                                       name = E.FirstName + " " + E.LastName,
        //                                                                       title = R.RoleType,
        //                                                                       image = E.ProfileImageUrl,
        //                                                                       cssClass = "ngx-org-ceo"
        //                                                                   }).ToList(); if (newwwwwwEmp.Count != 0)
        //                                                {
        //                                                    empp.childs = newwwwwwEmp;
        //                                                }
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                    break;
        //                }
        //            }
        //            return res;
        //        }
        //        catch (Exception ex)
        //        {
        //            return null;
        //        }
        //    }
        //}
        //#endregion

        //#region Api To Get Employee With Filter
        ///// <summary>
        ///// Created By Harshit Mitra on 07-04-2022
        ///// API >> Get >> api/employees/getallemployee
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("getallemployee")]
        //public async Task<ResponseBodyModel> GetAllEmployeesNew()
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        var employeeList = await _db.Employee.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyid &&
        //               x.OrgId == claims.orgid)
        //               .Select(x => new
        //               {
        //                   x.FirstName,
        //                   x.MiddleName,
        //                   x.LastName,
        //                   x.FatherName,
        //                   x.MotherName,
        //                   x.DateOfBirth,
        //                   x.PrimaryContact,
        //                   x.WhatsappNumber,
        //                   x.EmergencyNumber,
        //                   x.PersonalEmail,
        //                   x.OfficeEmail,
        //                   x.DesignationName,
        //                   x.JoiningDate,
        //                   x.ConfirmationDate,
        //                   x.CompanyName,
        //                   x.PermanentAddress,
        //                   x.LocalAddress,
        //                   x.BloodGroup,
        //                   x.AadharNumber,
        //                   x.IFSC,
        //                   x.Salary,
        //                   x.MaritalStatus,
        //                   x.SpouseName,
        //                   x.MedicalIssue,
        //                   x.DisplayName,
        //                   x.ReportingManager,
        //                   x.BiometricID,
        //                   x.AttendanceNumber,
        //                   x.ProbationEndDate,
        //                   x.InProbation,
        //                   x.TimeType,
        //                   x.WorkerType,
        //                   x.ShiftType,
        //                   x.WeeklyOffPolicy,
        //                   x.NoticePeriodMonths,
        //                   x.PayGroup,
        //                   x.CostCenter,
        //                   x.WorkPhone,
        //                   x.ResidenceNumber,
        //                   x.SkypeMail,
        //                   x.Band,

        //               }).ToListAsync();
        //        if (employeeList.Count > 0)
        //        {
        //            res.Message = "Employee List";
        //            res.Status = true;
        //            res.Data = employeeList;
        //        }
        //        else
        //        {
        //            res.Message = "Employee List Is Empty Found";
        //            res.Status = false;
        //            res.Data = employeeList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}
        //#endregion

        //[HttpGet]
        //[Authorize]
        //[Route("GetAllEmployees")]
        //public IHttpActionResult GetAllEmployees()
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //        var employeeData = (from ad in _db.Employee
        //                            join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                            where ad.IsDeleted == false && ad.EmployeeTypeId != 3
        //                            && ad.CompanyId == compid && ad.OrgId == orgid
        //                            select new
        //                            {
        //                                ad.EmployeeId,
        //                                n = ad.FirstName + " " + ad.LastName,
        //                                ad.PrimaryContact,
        //                                ad.PersonalEmail,
        //                                bd.RoleType,
        //                                ad.FirstName,
        //                                ad.LastName
        //                            }).ToList();
        //        foreach (var item in employeeData)
        //        {
        //            EmployeeData data = new EmployeeData();
        //            data.EmployeeId = item.EmployeeId;
        //            data.FullName = item.n;
        //            data.PrimaryContact = item.PrimaryContact;
        //            data.Email = item.PersonalEmail;
        //            data.RoleType = item.RoleType;
        //            data.FirstName = item.FirstName;
        //            data.LastName = item.LastName;
        //            employeeDataList.Add(data);
        //        }

        //        return Ok(employeeDataList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        //[HttpGet]
        //[Authorize]
        //[Route("GetAllEmployeesForLeaveBalance")]
        //public IHttpActionResult GetAllEmployeesForLeaveBalance()
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);
        //        Base response = new Base();
        //        List<EmployeeDataForLeaveBalance> employeeDataList = new List<EmployeeDataForLeaveBalance>();
        //        var employeeData = (from ad in _db.Employee
        //                            join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                            where ad.IsDeleted == false && ad.EmployeeTypeId != 3
        //                            && ad.CompanyId == compid && ad.OrgId == orgid
        //                            select new
        //                            {
        //                                ad.EmployeeId,
        //                                n = ad.FirstName.Trim() + " " + ad.LastName.Trim(),
        //                                ad.FatherName

        //                            }).ToList();
        //        foreach (var item in employeeData)
        //        {
        //            EmployeeDataForLeaveBalance data = new EmployeeDataForLeaveBalance();
        //            data.EmployeeId = item.EmployeeId;
        //            data.FullName = item.n;
        //            data.FatherName = item.FatherName;
        //            data.BereavementLeave = 0;
        //            data.CasualLeave = 0;
        //            data.CompOffs = 0;
        //            data.FloaterLeave = 0;
        //            data.MaternityLeave = 0;
        //            data.PaidLeave = 0;
        //            data.PaternityLeave = 0;
        //            data.SickLeave = 0;
        //            data.SpecialLeave = 0;
        //            data.UnpaidLeave = 0;
        //            //data.MonthYear = LeaveDate;
        //            employeeDataList.Add(data);
        //        }
        //        if (employeeDataList.Count > 0)
        //        {
        //            //response.LeaveBalanceList = employeeDataList;
        //            response.Message = "Leave Balance List found!";
        //        }
        //        return Ok(employeeDataList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpGet]
        //[Authorize]
        //[Route("GetAllEmployeesForCurrentSalaryInformation")]
        //public IHttpActionResult GetAllEmployeesForCurrentSalaryInformation()
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);
        //        Base response = new Base();
        //        List<EmployeeDataForCurrentSalaryInformation> employeeDataList = new List<EmployeeDataForCurrentSalaryInformation>();
        //        var employeeData = (from ad in _db.Employee
        //                            join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                            where ad.IsDeleted == false && ad.EmployeeTypeId != 3
        //                            && ad.CompanyId == compid && ad.OrgId == orgid
        //                            select new
        //                            {
        //                                ad.EmployeeId,
        //                                n = ad.FirstName.Trim() + " " + ad.LastName.Trim(),
        //                                ad.FatherName

        //                            }).ToList();
        //        foreach (var item in employeeData)
        //        {
        //            EmployeeDataForCurrentSalaryInformation data = new EmployeeDataForCurrentSalaryInformation();
        //            data.EmployeeId = item.EmployeeId;
        //            data.FullName = item.n;
        //            data.FatherName = item.FatherName;
        //            data.FixedAnnualGross = 0;
        //            data.CTCExludingBonus = 0;

        //            employeeDataList.Add(data);
        //        }
        //        if (employeeDataList.Count > 0)
        //        {
        //            //response.LeaveBalanceList = employeeDataList;
        //            response.Message = "Employee Current Salary List found!";
        //        }
        //        return Ok(employeeDataList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        //[HttpGet]
        //[Authorize]
        //[Route("GetAllEmployeesForSalary")]
        //public IHttpActionResult GetAllEmployeesForSalary()
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        Base response = new Base();
        //        List<EmployeeDataForSalary> employeeDataList = new List<EmployeeDataForSalary>();
        //        var employeeData = (from ad in _db.Employee
        //                            join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                            where ad.IsDeleted == false && ad.EmployeeTypeId != 3
        //                            && ad.CompanyId == compid && ad.OrgId == orgid
        //                            select new
        //                            {
        //                                ad.EmployeeId,
        //                                n = ad.FirstName.Trim() + " " + ad.LastName.Trim(),
        //                                ad.FatherName

        //                            }).ToList();
        //        foreach (var item in employeeData)
        //        {
        //            EmployeeDataForSalary data = new EmployeeDataForSalary();
        //            data.EmployeeId = item.EmployeeId;
        //            data.FullName = item.n;
        //            data.FatherName = item.FatherName;
        //            //data.FixedAnnualGross = 0;
        //            //data.CTCExludingBonus = 0;

        //            employeeDataList.Add(data);
        //        }
        //        if (employeeDataList.Count > 0)
        //        {
        //            //response.LeaveBalanceList = employeeDataList;
        //            response.Message = "Employee Salary List found!";
        //        }
        //        return Ok(employeeDataList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpGet]
        //[Authorize]
        //[Route("GetAllEmployeesForBonus")]
        //public IHttpActionResult GetAllEmployeesForBonus()
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        Base response = new Base();
        //        List<EmployeeDataForBonus> employeeDataList = new List<EmployeeDataForBonus>();
        //        var employeeData = (from ad in _db.Employee
        //                            join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                            where ad.IsDeleted == false && ad.EmployeeTypeId != 3
        //                            && ad.CompanyId == compid && ad.OrgId == orgid
        //                            select new
        //                            {
        //                                ad.EmployeeId,
        //                                n = ad.FirstName.Trim() + " " + ad.LastName.Trim(),
        //                                ad.FatherName

        //                            }).ToList();
        //        foreach (var item in employeeData)
        //        {
        //            EmployeeDataForBonus data = new EmployeeDataForBonus();
        //            data.EmployeeId = item.EmployeeId;
        //            data.FullName = item.n;
        //            data.FatherName = item.FatherName;
        //            //data.FixedAnnualGross = 0;
        //            //data.CTCExludingBonus = 0;

        //            employeeDataList.Add(data);
        //        }
        //        if (employeeDataList.Count > 0)
        //        {
        //            //response.LeaveBalanceList = employeeDataList;
        //            response.Message = "Data for Employee Bonus List found!";
        //        }
        //        return Ok(employeeDataList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        //[HttpPost]
        //[Route("UploadResumeEmployee")]
        //[Authorize]
        //public IHttpActionResult UploadResumeEmployee()
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        Base response = new Base();
        //        string fname = "";
        //        var Request = HttpContext.Current.Request;
        //        if (Request.Files.Count > 0)
        //        {
        //            HttpFileCollection files = Request.Files;
        //            for (int i = 0; i < files.Count; i++)
        //            {
        //                HttpPostedFile file = files[i];

        //                // Checking for Internet Explorer
        //                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
        //                {
        //                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
        //                    fname = testfiles[testfiles.Length - 1];
        //                }
        //                else
        //                {
        //                    fname = file.FileName;
        //                }

        //                // Get the complete folder path and store the file inside it.
        //                fname = Path.Combine(HttpContext.Current.Server.MapPath("~/EmployeeDocuments/"), fname);
        //                file.SaveAs(fname);

        //                var name = file.FileName.ToString();
        //                response.ResumeFilename = name;
        //                response.Message = "Resume Uploaded successfully";
        //                response.StatusReason = true;
        //            }

        //        }
        //        else
        //        {
        //            response.Message = "No files selected";
        //            response.StatusReason = false;
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("GetDepartmentFilter")]
        //[Authorize]
        //public IHttpActionResult GetDepartmentFilter(List<int> Id)
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        Base response = new Base();
        //        List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //        foreach (var item1 in Id)
        //        {
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                where ad.IsDeleted == false && ad.RoleId == item1 && ad.EmployeeTypeId != 3
        //                                && ad.CompanyId == compid && ad.OrgId == orgid
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                employeeDataList.Add(data);
        //            }
        //        }
        //        if (employeeDataList.Count > 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Record Found";
        //            response.employeeDataList = employeeDataList;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "No Record Found!";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpGet]
        //[Route("GetJoiningDateFilter")]
        //[Authorize]
        //public IHttpActionResult GetJoiningDateFilter(DateTime FromDate, DateTime ToDate)
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        Base response = new Base();
        //        List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //        if (FromDate != null && ToDate != null)
        //        {
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                where ad.IsDeleted == false && ad.JoiningDate >= FromDate && ad.JoiningDate <= ToDate && ad.EmployeeTypeId != 3
        //                                && ad.CompanyId == compid && ad.OrgId == orgid
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                employeeDataList.Add(data);
        //            }
        //        }

        //        if (employeeDataList.Count > 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Record Found";
        //            response.employeeDataList = employeeDataList;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "No Record Found!";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpGet]
        //[Route("GetAllEmployeeByJoiningDate")]
        //[Authorize]
        //public async Task<ResponseBodyModel> GetAllEmployeeByJoiningDate()
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        //var identity = User.Identity as ClaimsIdentity;
        //        //int userid = 0;
        //        //int compid = 0;
        //        //int orgid = 0;
        //        //if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //        //    userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        //if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //        //    compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        //if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //        //    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        // Base response = new Base();
        //        // List<EmployeeList> employeeDataList = new List<EmployeeList>();

        //        //var EmployeeList = _db.Employee.Where(x => x.IsDeleted == false).ToList();
        //        //var employeedata = (from ad in _db.Employee
        //        //                        //join bd in _db.role on ad.roleid equals bd.roleid
        //        //                        // join cd in _db.employeetype on ad.employeetypeid equals cd.employeetypeid

        //        //                    orderby ad.JoiningDate
        //        //                    where ad.IsDeleted == false && ad.CompanyId == compid && ad.OrgId == orgid
        //        //                    select new
        //        //                    {
        //        //                        ad.EmployeeId,
        //        //                        FullName = ad.FirstName + " " + ad.LastName,
        //        //                        ad.Email,
        //        //                        // bd.roletype,
        //        //                        ad.FirstName,
        //        //                        ad.LastName,
        //        //                        ad.FatherName,
        //        //                        ad.MotherName,
        //        //                        ad.MiddleName,
        //        //                        ad.PrimaryContact,
        //        //                       // ad.EmergencyNumber,
        //        //                        ad.SecondaryJobTitle,
        //        //                        ad.BiometricID,
        //        //                        ad.CompanyName,
        //        //                        ad.EmployeeTypeID,
        //        //                        ad.BloodGroup,
        //        //                        ad.MaritalStatus,
        //        //                        ad.DepartmentName,
        //        //                        ad.DesignationName,
        //        //                        ad.JoiningDate,
        //        //                        ad.ConfirmationDate,
        //        //                        ad.DateOfBirth,
        //        //                       // ad.EmergencyNumber,
        //        //                        ad.WhatsappNumber,
        //        //                        ad.AadharNumber,
        //        //                        ad.PanNumber,
        //        //                        ad.PermanentAddress,
        //        //                        ad.LocalAddress,
        //        //                        ad.MedicalIssue,
        //        //                        //  ad.salary,
        //        //                        ad.BankAccountNumber,
        //        //                        ad.IFSC,
        //        //                        ad.AccountHolderName,
        //        //                        ad.BankName,
        //        //                        ad.OfficeEmail,
        //        //                        //  cd.employeetypes,
        //        //                        //
        //        //                        //ad.displayname,
        //        //                        //ad.biometricid,
        //        //                        //ad.attendancenumber,
        //        //                        //ad.paygroup,
        //        //                        //ad.skypemail,
        //        //                        //ad.band,
        //        //                        // ad.secondaryjobtitle,
        //        //                        //  ad.probationenddate,
        //        //                        ad.ReportingManager,
        //        //                        // ad.weeklyoffpolicy,
        //        //                        // ad.residencenumber,
        //        //                        //  ad.timetype,
        //        //                        // ad.workertype,
        //        //                        //  ad.shifttype,
        //        //                        //  ad.noticeperiodmonths,
        //        //                        // ad.costcenter,
        //        //                        //ad.worknumber,
        //        //                        ad.CompanyId,
        //        //                        ad.OrgId
        //        //                    }).ToList();
        //        //foreach (var item in employeedata)
        //        //{
        //        //    EmployeeList data = new EmployeeList();
        //        //    data.EmployeeId = item.EmployeeId;
        //        //    data.FullName = item.FullName;
        //        //    //data.PrimaryContact = item.;
        //        //    data.Email = item.PersonalEmail;
        //        //    //  data.roletype = item.roletype;
        //        //    data.FirstName = item.FirstName;
        //        //    data.LastName = item.LastName;
        //        //    // data.maritalstatus = item.maritalstatus;
        //        //    // data.spousename = item.spousename;
        //        //    data.FatherName = item.FatherName;
        //        //    data.MotherName = item.MotherName;

        //        //    //  data.roletype = item.roletype;
        //        //    //data.firstname = item.firstname;
        //        //    //data.lastname = item.lastname;
        //        //    data.JoiningDate = item.JoiningDate;
        //        //    data.ConfirmationDate = item.ConfirmationDate;
        //        //    data.DateOfBirth = item.DateOfBirth;
        //        //   // data.EmergencyNumber = item.EmergencyNumber;
        //        //    data.WhatsappNumber = item.WhatsappNumber;
        //        //    data.AadharNumber = item.AadharNumber;
        //        //    data.PanNumber = item.PanNumber;
        //        //    data.PermanentAddress = item.PermanentAddress;
        //        //    data.LocalAddress = item.LocalAddress;
        //        //    data.MedicalIssue = item.MedicalIssue;
        //        //    //   data.salary = item.salary;
        //        //    data.BankAccountNumber = item.BankAccountNumber;
        //        //    data.IFSC = item.IFSC;
        //        //    data.AccountHolderName = item.AccountHolderName;
        //        //    data.BankName = item.BankName;
        //        //    data.MiddleName = item.MiddleName;
        //        //    data.PrimaryContact = item.PrimaryContact;
        //        //    data.BloodGroup = item.BloodGroup;
        //        //    data.MaritalStatus = item.MaritalStatus;
        //        //    data.DepartmentName = item.DepartmentName;
        //        //    data.DesignationName = item.DesignationName;
        //        //    data.BiometricID = item.BiometricID;
        //        //    data.SecondaryJobTitle = item.SecondaryJobTitle;
        //        //    data.EmployeeTypeID = item.EmployeeTypeID;
        //        //   // data.EmergencyNumber = item.EmergencyNumber;
        //        //    //data.EmergencyNumber = item.EmergencyNumber;
        //        //    //  data.employeetype = item.employeetypes;
        //        //    //data.companyname = item.companyname;
        //        //    //newly added
        //        //    // data.displayname = item.displayname;
        //        //    //data.biometricid = item.biometricid;
        //        //    //  data.attendancenumber = item.attendancenumber;
        //        //    // data.paygroup = item.paygroup;
        //        //    // data.skypemail = item.skypemail;
        //        //    //     data.band = item.band;
        //        //    //   data.secondaryjobtitle = item.secondaryjobtitle;
        //        //    //   data.probationenddate = datetime.now;
        //        //    //data.ReportingManager = item.ReportingManager;
        //        //    // data.weeklyoffpolicy = item.weeklyoffpolicy;
        //        //    //  data.residencenumber = item.residencenumber;
        //        //    //  data.timetype = item.timetype;
        //        //    //  data.workertype = item.workertype;
        //        //    //  data.shifttype = item.shifttype;
        //        //    //  data.noticeperiodmonths = item.noticeperiodmonths;
        //        //    //  data.costcenter = item.costcenter;
        //        //    // data.worknumber = item.worknumber;
        //        //    employeeDataList.Add(data);
        //        //}
        //        //if (employeeDataList.Count != 0)
        //        //{
        //        //    res.Status = true;
        //        //    res.Message = "Data Found";
        //        //    res.Data = employeedata;
        //        //}
        //        var EmployeeData = await _db.Employee.Select(x => new
        //        {
        //            x.EmployeeId,
        //            x.FirstName,
        //            FullName = x.FirstName + " " + x.LastName,
        //            x.EmployeeTypeId,
        //            // x.EmergencyNumber,
        //            x.DesignationName,
        //            x.DepartmentName,
        //            x.DisplayName,
        //            x.MiddleName,
        //            x.MobilePhone,
        //            x.MaritalStatus,
        //            x.LocalAddress,
        //            x.JoiningDate,
        //            x.LastName,
        //            x.PanNumber,
        //            x.PrimaryContact,
        //            x.OfficeEmail,
        //            x.Pincode,
        //            x.BiometricID,
        //            x.BloodGroup,
        //            x.CompanyName,
        //            x.ConfirmationDate,
        //            x.CompanyId,
        //            x.OrgId,
        //            x.MotherName,
        //            x.FatherName,
        //            x.WhatsappNumber,
        //            x.ReportingManager,
        //            x.PermanentAddress,

        //        }).ToListAsync();
        //        if (EmployeeData.Count > 0)
        //        {
        //            res.Message = "All Employee List";
        //            res.Status = true;
        //            res.Data = EmployeeData;
        //        }
        //        else
        //        {
        //            res.Message = "List is Empty";
        //            res.Status = false;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        //[HttpGet]
        //[Route("GetSelectRole")]
        //[Authorize]
        //public IHttpActionResult GetSelectRole(int Id)
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        if (Id == 1) //Intern
        //        {
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                where ad.EmployeeTypeId == Id && ad.IsDeleted == false
        //                                && ad.CompanyId == compid && ad.OrgId == orgid
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    // ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    // cd.EmployeeTypes,
        //                                    //ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DateOfBirth = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                // data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                //  data.EmployeeType = item.EmployeeTypes;
        //                // data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }

        //            return Ok(employeeDataList);

        //        }
        //        else if (Id == 3) //Ex-Employee
        //        {
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                where ad.EmployeeTypeId == Id && ad.IsDeleted == false
        //                                && ad.CompanyId == compid && ad.OrgId == orgid
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    // cd.EmployeeTypes,
        //                                    // ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DateOfBirth = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                //  data.EmployeeType = item.EmployeeTypes;
        //                // data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }

        //            return Ok(employeeDataList);
        //        }
        //        else if (Id == 4) //Notice Period
        //        {
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                where ad.EmployeeTypeId == Id && ad.IsDeleted == false
        //                                && ad.CompanyId == compid && ad.OrgId == orgid
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    // cd.EmployeeTypes,
        //                                    // ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DateOfBirth = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                //   data.EmployeeType = item.EmployeeTypes;
        //                //  data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }

        //            return Ok(employeeDataList);
        //        }
        //        else if (Id == 5)  //Probation Period
        //        {
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                where ad.EmployeeTypeId == Id && ad.IsDeleted == false
        //                                && ad.CompanyId == compid && ad.OrgId == orgid
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    cd.EmployeeTypes,
        //                                    ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DOB = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }

        //            return Ok(employeeDataList);
        //        }
        //        else if (Id == 6)  //Confirmed Employee
        //        {
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                //where ad.EmployeeTypeID == Id && ad.IsDeleted == false
        //                                //&& ad.CompanyId == compid && ad.OrgId == orgid
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    cd.EmployeeTypes,
        //                                    ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DOB = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }

        //            return Ok(employeeDataList);
        //        }
        //        else if (Id == 7)  //Pre Confirmed
        //        {
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                where ad.EmployeeTypeId == Id && ad.IsDeleted == false
        //                                && ad.CompanyId == compid && ad.OrgId == orgid
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    cd.EmployeeTypes,
        //                                    ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DOB = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }

        //            return Ok(employeeDataList);
        //        }
        //        else  //All
        //        {
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                where ad.IsDeleted == false
        //                                && ad.CompanyId == compid && ad.OrgId == orgid
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    cd.EmployeeTypes,
        //                                    ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DOB = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }

        //            return Ok(employeeDataList);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        //#region This Api Use Get Company
        ///// <summary>
        ///// change Response By ankit
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("GetCompany")]
        //[Authorize]
        //public async Task<ResponseBodyModel> GetCompany()
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        Base response = new Base();

        //        var data = (from ad in _db.Company where ad.IsDeleted == false && ad.CompanyId == claims.companyid select ad).ToList();
        //        if (data.Count != 0)
        //        {
        //            res.Status = true;
        //            res.Message = "Company Data Found";
        //            res.Data = data;
        //        }
        //        else
        //        {
        //            res.Status = false;
        //            res.Message = "Data Not Found";
        //            res.Data = data;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        //#endregion

        //[HttpGet]
        //[Route("GetCompanyFilter")]
        //public IHttpActionResult GetCompanyFilter(int Id)
        //{
        //    try
        //    {
        //        List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //        if (Id != 0)
        //        {
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                where ad.CompanyId == Id && ad.IsDeleted == false
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    cd.EmployeeTypes,
        //                                    ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DOB = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }
        //        }
        //        return Ok(employeeDataList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpGet]
        //[Route("GetAllEmployeesList")]
        //[Authorize]
        //public IHttpActionResult GetAllEmployeesList(int page = 1)
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //        var employeeData = (from ad in _db.Employee
        //                            join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                            where ad.IsDeleted == false && ad.EmployeeTypeId != 3
        //                            && ad.CompanyId == compid && ad.OrgId == orgid
        //                            select new
        //                            {
        //                                ad.EmployeeId,
        //                                n = ad.FirstName + " " + ad.LastName,
        //                                ad.PrimaryContact,
        //                                ad.PersonalEmail,
        //                                bd.RoleType
        //                            }).ToList();
        //        foreach (var item in employeeData)
        //        {
        //            EmployeeData data = new EmployeeData();
        //            data.EmployeeId = item.EmployeeId;
        //            data.FullName = item.n;
        //            data.PrimaryContact = item.PrimaryContact;
        //            data.Email = item.PersonalEmail;
        //            data.RoleType = item.RoleType;
        //            employeeDataList.Add(data);
        //        }
        //        int pageSize = 10;
        //        int pageNumber = page;
        //        //return Ok(objModel.objList.);
        //        //val pagedList = PagedList.Builder(ListDataSource(list), ...)
        //        return Ok(employeeDataList.ToPagedList(pageNumber, pageSize));
        //        //return Ok(employeeDataList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpGet]
        //[Route("GetAllEmployeeForTeam")]
        //[Authorize]
        //public IHttpActionResult GetAllEmployeeForTeam()
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        Base response = new Base();

        //        var employeeData = _db.Employee.Where(x => x.IsDeleted == false && x.EmployeeTypeId != 3 && x.CompanyId == compid && x.OrgId == orgid).ToList();
        //        if (employeeData.Count > 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Record Found";
        //            response.employeeData = employeeData;
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "No Record Found!";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        //[HttpGet]
        //[Route("GetAllEmployeeBySearchKeyword")]
        //[Authorize]
        //public IHttpActionResult GetAllEmployeeBySearchKeyword(string searchtext)
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        Base response = new Base();
        //        List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //        //EmpSearchData empSearchData =new EmpSearchData();
        //        if (searchtext != null)
        //        {
        //            var empSearchData = (from ad in _db.Employee
        //                                 join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                 join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                 where (ad.FirstName.Contains(searchtext) || ad.LastName.Contains(searchtext)) && ad.EmployeeTypeId != 3 && ad.IsDeleted == false
        //                                         && ad.CompanyId == compid && ad.OrgId == orgid
        //                                 select new
        //                                 {
        //                                     ad.EmployeeId,
        //                                     ad.PersonalEmail,
        //                                     ad.FirstName,
        //                                     ad.LastName,
        //                                     n = ad.FirstName + " " + ad.LastName,
        //                                     bd.RoleType,
        //                                     ad.OfficeEmail,
        //                                     cd.EmployeeTypes,
        //                                     ad.CompanyName
        //                                 });
        //            foreach (var item in empSearchData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.RoleType = item.RoleType;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }
        //        else
        //        {
        //            var empSearchData = (from ad in _db.Employee
        //                                 join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                 join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                 where ad.IsDeleted == false && ad.CompanyId == compid && ad.OrgId == orgid
        //                                 select new
        //                                 {
        //                                     ad.EmployeeId,
        //                                     ad.PersonalEmail,
        //                                     bd.RoleType,
        //                                     ad.FirstName,
        //                                     ad.LastName,
        //                                     n = ad.FirstName + " " + ad.LastName,
        //                                     ad.OfficeEmail,
        //                                     cd.EmployeeTypes,
        //                                     ad.CompanyName
        //                                 }).ToList();
        //            foreach (var item in empSearchData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("GetAllEmployeeByFilter")]
        //[Authorize]
        //public IHttpActionResult GetAllEmployeeByFilter(Employee employee)
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //        if (employee.EmployeeId != 0)
        //        {
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                where ad.EmployeeId == employee.EmployeeId && ad.IsDeleted == false
        //                                && ad.CompanyId == compid && ad.OrgId == orgid
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                employeeDataList.Add(data);
        //            }
        //            return Ok(employeeDataList);
        //        }
        //        else
        //        {
        //            return Ok(employeeDataList);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //#region Api To Get Login Role
        ///// <summary>
        ///// Created By Harshit Mitra On 22-04-2022
        ///// API >> Get >> api/employees/getloginrole
        ///// </summary>
        ///// <returns></returns>
        //[Authorize]
        //[Route("getloginrole")]
        //public async Task<GetUserLoginRoleResponse> GetUserLoginRole()
        //{
        //    GetUserLoginRoleResponse res = new GetUserLoginRoleResponse();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        var user = await _db.User.FirstOrDefaultAsync(x => x.UserId == claims.userid);
        //        if (user != null)
        //        {
        //            if (user.DepartmentId != 0)
        //            {
        //                var employee = await _db.Employee.FirstOrDefaultAsync(x =>
        //                        x.EmployeeId == user.EmployeeId);
        //                if (employee != null)
        //                {
        //                    var loginRole = _db.Department.Where(x => x.DepartmentId == employee.DepartmentId)
        //                            .Select(x => x.DepartmentName).FirstOrDefault();
        //                    res.RoleType = loginRole;
        //                }
        //                else
        //                {
        //                    res.Message = "INVALID";
        //                    res.Status = false;
        //                    res.RoleType = "INVALID";
        //                }
        //            }
        //            else
        //            {
        //                var loginRole = "SuperAdmin";
        //                res.RoleType = loginRole;
        //            }
        //            res.Message = "Login Role";
        //            res.Status = true;
        //        }
        //        else
        //        {
        //            res.Message = "INVALID";
        //            res.Status = false;
        //            res.RoleType = "INVALID";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //        res.RoleType = "INVALID";
        //    }
        //    return res;
        //}
        //public class GetUserLoginRoleResponse
        //{
        //    public string Message { get; set; }
        //    public bool Status { get; set; }
        //    public string RoleType { get; set; }

        //}
        //#endregion

        ////[HttpGet]
        ////[Route("GetEmployeeById")]
        ////public IHttpActionResult GetEmployee(int EmployeeId)
        ////{
        ////    var identity = User.Identity as ClaimsIdentity;
        ////    int userid = 0;
        ////    int compid = 0;
        ////    int orgid = 0;
        ////    // Access claims

        ////    if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        ////        userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        ////    if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        ////        compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        ////    if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        ////        orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        ////    List<EmployeeData> employeeDataList = new List<EmployeeData>();
        ////    var EmployeeData = (from ad in db.Employee
        ////                        join bd in db.Role on ad.RoleId equals bd.RoleId
        ////                        join cd in db.EmployeeType on ad.EmployeeTypeID equals cd.EmployeeTypeId
        ////                        join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        ////                        where ad.EmployeeId == EmployeeId && ad.CompanyId == compid && ad.OrgId == orgid && ad.IsDeleted == false
        ////                        select new
        ////                        {
        ////                            ad.EmployeeId,
        ////                            v = ad.FirstName + ad.LastName,
        ////                            ad.Email,
        ////                            ad.FirstName,
        ////                            ad.LastName,
        ////                            ad.PrimaryContact,
        ////                            ad.MaritalStatus,
        ////                            ad.SpouseName,
        ////                            ad.FatherName,
        ////                            ad.MotherName,
        ////                            fd.BloodGroupType,
        ////                            ad.Document,
        ////                            bd.RoleType,
        ////                            ad.Password,
        ////                            ad.JoiningDate,
        ////                            ad.ConfirmationDate,
        ////                            ad.DOB,
        ////                            ad.EmergencyNumber,
        ////                            ad.WhatsappNumber,
        ////                            ad.AadharNumber,
        ////                            ad.PanNumber,
        ////                            ad.PermanentAddress,
        ////                            ad.LocalAddress,
        ////                            ad.MedicalIssue,
        ////                            ad.Profile,
        ////                            ad.Salary,
        ////                            ad.BankAccountNumber,
        ////                            ad.IFSC,
        ////                            ad.AccountHolderName,
        ////                            ad.BankName,
        ////                            ad.OfficeEmail,
        ////                            cd.EmployeeTypes,
        ////                            ad.CompanyName,
        ////                            ad.BloodGroupId,
        ////                            ad.RoleId,
        ////                            ad.EmployeeTypeID,
        ////                            ad.CompanyId,
        ////                            ad.uploadResume,
        ////                            ad.OrgId
        ////                        }).FirstOrDefault();
        ////    return Ok(EmployeeData);
        ////}

        ////[HttpGet]
        ////[Route("GetEmployeeById")]
        ////public async Task<ResponseBodyModel> GetEmployee(int EmployeeId)
        ////{
        ////    ResponseBodyModel res = new ResponseBodyModel();
        ////    List<EmployeeData> employeeDataList = new List<EmployeeData>();
        ////    var EmployeeData = (from ad in _db.Employee
        ////                        join bd in _db.Role on ad.RoleId equals bd.RoleId
        ////                        join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        ////                        //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        ////                        where ad.EmployeeId == EmployeeId && ad.IsDeleted == false
        ////                        select new
        ////                        {
        ////                            ad.EmployeeId,
        ////                            v = ad.FirstName + ad.LastName,
        ////                            ad.PersonalEmail,
        ////                            ad.FirstName,
        ////                            ad.LastName,
        ////                            ad.PrimaryContact,
        ////                            ad.MaritalStatus,
        ////                            ad.SpouseName,
        ////                            ad.FatherName,
        ////                            ad.MotherName,
        ////                            //fd.BloodGroupType,
        ////                            ad.Document,
        ////                            bd.RoleType,
        ////                            ad.Password,
        ////                            ad.JoiningDate,
        ////                            ad.ConfirmationDate,
        ////                            ad.DateOfBirth,
        ////                            ad.EmergencyNumber,
        ////                            ad.WhatsappNumber,
        ////                            ad.AadharNumber,
        ////                            ad.PanNumber,
        ////                            ad.PermanentAddress,
        ////                            ad.LocalAddress,
        ////                            ad.MedicalIssue,
        ////                            ad.ProfileImageUrl,
        ////                            ad.Salary,
        ////                            ad.BankAccountNumber,
        ////                            ad.IFSC,
        ////                            ad.AccountHolderName,
        ////                            ad.BankName,
        ////                            ad.OfficeEmail,
        ////                            cd.EmployeeTypes,
        ////                            ad.CompanyName,
        ////                            //ad.BloodGroupId,
        ////                            ad.RoleId,
        ////                            ad.EmployeeTypeId,
        ////                            ad.CompanyId,
        ////                            ad.UploadResume,
        ////                            ad.Gender

        ////                        }).ToList();
        ////    foreach (var item in EmployeeData)
        ////    {
        ////        EmployeeData data = new EmployeeData();
        ////        data.EmployeeId = item.EmployeeId;
        ////        data.FullName = item.v;
        ////        data.Email = item.PersonalEmail;
        ////        data.PrimaryContact = item.PrimaryContact;
        ////        data.MaritalStatus = item.MaritalStatus;
        ////        data.SpouseName = item.SpouseName;
        ////        data.FatherName = item.FatherName;
        ////        data.MotherName = item.MotherName;
        ////        //data.BloodGroup = item.BloodGroupType;
        ////        data.Document = item.Document;
        ////        data.RoleType = item.RoleType;
        ////        data.FirstName = item.FirstName;
        ////        data.LastName = item.LastName;
        ////        data.Password = item.Password;
        ////        data.UploadResume = item.UploadResume;
        ////        data.Gender = item.Gender;

        ////        data.JoiningDate = item.JoiningDate;
        ////        data.ConfirmationDate = item.ConfirmationDate;
        ////        data.DOB = item.DateOfBirth;
        ////        data.EmergencyNumber = item.EmergencyNumber;
        ////        data.WhatsappNumber = item.WhatsappNumber;
        ////        data.AadharNumber = item.AadharNumber;
        ////        data.PanNumber = item.PanNumber;
        ////        data.PermanentAddress = item.PermanentAddress;
        ////        data.LocalAddress = item.LocalAddress;
        ////        data.MedicalIssue = item.MedicalIssue;
        ////        data.Profile = item.Profile;
        ////        data.Salary = item.Salary;
        ////        data.BankAccountNumber = item.BankAccountNumber;
        ////        data.IFSC = item.IFSC;
        ////        data.AccountHolderName = item.AccountHolderName;
        ////        data.BankName = item.BankName;
        ////        data.OfficeEmail = item.OfficeEmail;
        ////        data.EmployeeType = item.EmployeeTypes;
        ////        data.CompanyName = item.CompanyName;
        ////        //data.BloodGroup = item.BloodGroup;
        ////        data.RoleId = item.RoleId;
        ////        //data.EmployeeTypeID = item.EmployeeTypeID;
        ////        data.CompanyId = item.CompanyId;

        ////        employeeDataList.Add(data);
        ////    }
        ////    if (EmployeeData != null)
        ////    {
        ////        res.Message = "skillgroup List";
        ////        res.Status = true;
        ////        res.Data = EmployeeData;
        ////    }
        ////    else
        ////    {
        ////        res.Message = "skillgroup List is Empty";
        ////        res.Status = false;
        ////    }

        ////    return res;
        ////}

        //#region this api use Update Employee ProfileImage
        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="EmployeeId"></param>
        ///// <param name="ImageUrl"></param>
        ///// <returns></returns>
        //[HttpPut]
        //[Route("UpdateEmployeeForProfileImage")]
        //[Authorize]
        //public IHttpActionResult UpdateEmployeeForProfileImage(int EmployeeId, string ImageUrl)
        //{
        //    var identity = User.Identity as ClaimsIdentity;
        //    int userid = 0;
        //    int compid = 0;
        //    int orgid = 0;

        //    // Access claims

        //    if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //        userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //    if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //        compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //    if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //        orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //    Base response = new Base();

        //    var EmployeeData = _db.Employee.Where(x => x.EmployeeId == userid && x.IsDeleted == false).FirstOrDefault();
        //    if (EmployeeData != null)
        //    {
        //        EmployeeData.ProfileImageUrl = ImageUrl;
        //        EmployeeData.UpdatedOn = DateTime.Now;
        //        _db.Entry(EmployeeData).State = System.Data.Entity.EntityState.Modified;
        //        _db.SaveChanges();
        //        response.StatusReason = true;
        //        response.Message = "Profile Image Updated Successfully";
        //        response.Url = ImageUrl;
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        response.StatusReason = false;
        //        response.Message = "Profile Image not Updated.";
        //        return Ok(response);

        //    }

        //}
        //#endregion

        //[HttpPut]
        //[Route("UpdateProfileRemark")]
        //[Authorize]
        //public IHttpActionResult UpdateProfileRemark(int EmployeeId, string ColumnName, string Remark)
        //{
        //    var identity = User.Identity as ClaimsIdentity;
        //    int userid = 0;
        //    int compid = 0;
        //    int orgid = 0;

        //    // Access claims

        //    if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //        userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //    if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //        compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //    if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //        orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //    Base response = new Base();
        //    string column = "";
        //    var EmployeeData = _db.Employee.Where(x => x.IsDeleted == false && x.EmployeeId == EmployeeId).SingleOrDefault();
        //    if (EmployeeData != null)
        //    {
        //        if (ColumnName.Trim().ToUpper() == "ABOUT")
        //        {
        //            EmployeeData.AboutMeRemark = Remark;
        //        }

        //        if (ColumnName.Trim().ToUpper() == "ABOUTMYJOB")
        //        {
        //            EmployeeData.AboutMyJobRemark = Remark;
        //        }

        //        if (ColumnName.Trim().ToUpper() == "INTERESTANDHOBBIES")
        //        {
        //            EmployeeData.InterestAndHobbiesRemark = Remark;
        //        }

        //        EmployeeData.UpdatedOn = DateTime.Now;
        //        _db.Entry(EmployeeData).State = System.Data.Entity.EntityState.Modified;
        //        _db.SaveChanges();
        //        response.StatusReason = true;
        //        response.Message = ColumnName + " Updated Successfully";
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        response.StatusReason = false;
        //        response.Message = ColumnName + " not Updated.";
        //        return Ok(response);

        //    }

        //}

        //#region this Api use Upadte Employee
        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="Employee"></param>
        ///// <returns></returns>
        //[HttpPut]
        //[Route("UpdateEmployee")]
        //[Authorize]
        //public IHttpActionResult UpdateEmployee(Employee Employee)// we are using contact table as a employee
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

        //    Base response = new Base();

        //    var EmployeeData = _db.Employee.Where(x => x.IsDeleted == false && x.EmployeeId == Employee.EmployeeId).FirstOrDefault();
        //    EmployeeData.FirstName = Employee.FirstName;
        //    EmployeeData.MiddleName = Employee.MiddleName;
        //    EmployeeData.LastName = Employee.LastName;
        //    // EmployeeData.EmployeeCode = Employee.EmployeeCode;
        //    EmployeeData.PersonalEmail = Employee.PersonalEmail;
        //    EmployeeData.RoleId = Employee.RoleId;
        //    EmployeeData.PrimaryContact = Employee.PrimaryContact;
        //    EmployeeData.SecondaryContact = Employee.SecondaryContact;
        //    EmployeeData.MaritalStatus = Employee.MaritalStatus;
        //    EmployeeData.SpouseName = Employee.SpouseName;
        //    EmployeeData.FatherName = Employee.FatherName;
        //    EmployeeData.MotherName = Employee.MotherName;
        //    EmployeeData.UpdatedOn = DateTime.Now;
        //    EmployeeData.IsActive = true;
        //    EmployeeData.IsDeleted = false;
        //    //if (Employee.BloodGroupId == 0)
        //    //{
        //    //    EmployeeData.BloodGroupId = 9;
        //    //}
        //    //else
        //    //{
        //    //    EmployeeData.BloodGroupId = Employee.BloodGroupId;
        //    //}

        //    EmployeeData.Document = Employee.Document;
        //    //  EmployeeData.JoiningDate =Employee.JoiningDate != null ? Employee.JoiningDate : DateTime.Now;
        //    EmployeeData.JoiningDate = Employee.JoiningDate;

        //    // EmployeeData.JoiningDate =  Convert.ToDateTime(Employee.JoiningDate).AddDays(1);
        //    if (Employee.ConfirmationDate != null)
        //    {
        //        EmployeeData.ConfirmationDate = Convert.ToDateTime(Employee.ConfirmationDate).AddDays(1);
        //    }

        //    //EmployeeData.ConfirmationDate = Employee.JoiningDate != null ? Convert.ToDateTime(Employee.ConfirmationDate).AddDays(1) : DateTime.Now;
        //    EmployeeData.DateOfBirth = Employee.DateOfBirth;
        //    EmployeeData.EmergencyNumber = Employee.EmergencyNumber;
        //    EmployeeData.WhatsappNumber = Employee.WhatsappNumber;
        //    EmployeeData.AadharNumber = Employee.AadharNumber;
        //    EmployeeData.PanNumber = Employee.PanNumber;
        //    EmployeeData.PermanentAddress = Employee.PermanentAddress;
        //    EmployeeData.LocalAddress = Employee.LocalAddress;
        //    EmployeeData.MedicalIssue = Employee.MedicalIssue;
        //    EmployeeData.ProfileImageUrl = Employee.ProfileImageUrl;
        //    EmployeeData.Salary = Employee.Salary;
        //    EmployeeData.BankAccountNumber = Employee.BankAccountNumber;
        //    EmployeeData.Password = Employee.Password;
        //    EmployeeData.IFSC = Employee.IFSC;
        //    EmployeeData.AccountHolderName = Employee.AccountHolderName;
        //    EmployeeData.BankName = Employee.BankName;
        //    EmployeeData.OfficeEmail = Employee.OfficeEmail;
        //    EmployeeData.EmployeeTypeId = Employee.EmployeeTypeId;
        //    EmployeeData.CompanyName = Employee.CompanyName;
        //    EmployeeData.CompanyId = claims.companyid;
        //    EmployeeData.OrgId = claims.orgid;

        //    EmployeeData.UploadResume = Employee.UploadResume;
        //    EmployeeData.ReportingManager = Employee.ReportingManager;
        //    EmployeeData.BiometricID = Employee.BiometricID;
        //    EmployeeData.AttendanceNumber = Employee.AttendanceNumber;
        //    EmployeeData.ProbationEndDate = DateTime.Now;
        //    EmployeeData.InProbation = Employee.InProbation;
        //    EmployeeData.TimeType = Employee.TimeType;
        //    EmployeeData.WorkerType = Employee.WorkerType;
        //    EmployeeData.ShiftType = Employee.ShiftType;
        //    EmployeeData.WeeklyOffPolicy = Employee.WeeklyOffPolicy;
        //    EmployeeData.NoticePeriodMonths = Employee.NoticePeriodMonths;
        //    EmployeeData.PayGroup = Employee.PayGroup;
        //    EmployeeData.CostCenter = Employee.CostCenter;
        //    EmployeeData.WorkPhone = Employee.WorkPhone;
        //    EmployeeData.ResidenceNumber = Employee.ResidenceNumber;
        //    //EmployeeData.SkypeId = Employee.SkypeId;
        //    EmployeeData.Band = Employee.Band;

        //    _db.SaveChanges();

        //    var UserData = _db.User.Where(x => x.EmployeeId == Employee.EmployeeId).FirstOrDefault();
        //    if (UserData != null)
        //    {
        //        UserData.UserName = Employee.OfficeEmail;
        //        var Password = Employee.Password;

        //        var keynew = DataHelper.GeneratePasswords(10);
        //        var passw = DataHelper.EncodePassword(Password, keynew);
        //        UserData.Password = passw;
        //        UserData.HashCode = keynew;
        //        UserData.EmployeeId = EmployeeData.EmployeeId;
        //        UserData.DepartmentId = Employee.RoleId;
        //        UserData.IsDeleted = false;
        //        UserData.IsActive = true;
        //        UserData.CreatedOn = DateTime.Now;
        //        _db.SaveChanges();
        //    }
        //    else
        //    {
        //        User UserDataInsert = new User();
        //        UserDataInsert.UserName = Employee.OfficeEmail;
        //        var Password = Employee.Password;
        //        var keynew = DataHelper.GeneratePasswords(10);
        //        var passw = DataHelper.EncodePassword(Password, keynew);
        //        UserDataInsert.Password = passw;
        //        UserDataInsert.HashCode = keynew;
        //        UserDataInsert.EmployeeId = EmployeeData.EmployeeId;
        //        UserDataInsert.DepartmentId = Employee.RoleId;
        //        UserDataInsert.IsDeleted = false;
        //        UserDataInsert.IsActive = true;
        //        UserDataInsert.CreatedOn = DateTime.Now;
        //        _db.User.Add(UserDataInsert);
        //        _db.SaveChanges();
        //    }

        //    response.StatusReason = true;
        //    response.Message = "Data Updated Successfully";

        //    return Ok(response);
        //}
        //#endregion

        //[HttpDelete]
        //[Route("DeleteEmployeeById")]
        //[Authorize]
        //public IHttpActionResult DeleteEmployee(int EmployeeId)
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        Base response = new Base();
        //        // 1 => Working,
        //        // 2 => Not Working

        //        var EmployeeData = _db.Employee.Where(x => x.EmployeeId == EmployeeId).FirstOrDefault();
        //        var UserData = _db.User.Where(x => x.IsDeleted == false && x.UserId == EmployeeId).FirstOrDefault();
        //        if (EmployeeData != null)
        //        {
        //            EmployeeData.IsDeleted = true;
        //            EmployeeData.IsActive = false;
        //            _db.Entry(EmployeeData).State = System.Data.Entity.EntityState.Modified;
        //            _db.SaveChanges();

        //            var ProjectData = (from ad in _db.Team where ad.TeamMemberId == EmployeeId select ad).ToList();
        //            foreach (var item in ProjectData)
        //            {
        //                item.IsDelete = true;
        //                item.IsActive = false;
        //                _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
        //                _db.SaveChanges();
        //            }

        //            response.StatusReason = true;
        //            response.Message = "Deleted Successfully";

        //            if (UserData != null)
        //            {
        //                UserData.IsDeleted = true;
        //                UserData.IsActive = false;
        //                _db.Entry(UserData).State = System.Data.Entity.EntityState.Modified;
        //                _db.SaveChanges();
        //                response.StatusReason = true;
        //                response.Message = " Deleted Successfully";
        //            }
        //            return Ok(response);
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "Data Not Found";
        //            return Ok(response);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpGet]
        //[Route("GetAllRole")]
        //[Authorize]
        //public IHttpActionResult GetAllRole()
        //{
        //    var identity = User.Identity as ClaimsIdentity;
        //    int userid = 0;
        //    int compid = 0;
        //    int orgid = 0;

        //    // Access claims

        //    if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //        userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //    if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //        compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //    if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //        orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //    Base response = new Base();
        //    List<RoleCountData> RoleCountDataList = new List<RoleCountData>();
        //    var RoleData = (from ad in _db.Role
        //                    where ad.IsDeleted == false
        //                    select new
        //                    {
        //                        ad.RoleId,
        //                        ad.RoleType
        //                    }).ToList();
        //    foreach (var item in RoleData)
        //    {
        //        RoleCountData data = new RoleCountData();

        //        data.RoleId = item.RoleId;
        //        data.RoleName = item.RoleType;
        //        var CountData = (from ad in _db.Employee where ad.RoleId == item.RoleId && ad.IsDeleted == false && ad.EmployeeTypeId == 2 || ad.EmployeeTypeId == 4 select ad).Count();

        //        data.Count = CountData;
        //        RoleCountDataList.Add(data);
        //    }

        //    if (RoleCountDataList.Count > 0)
        //    {
        //        response.StatusReason = true;
        //        response.Message = "Record Found";
        //        response.RoleCountData = RoleCountDataList;
        //    }
        //    else
        //    {
        //        response.StatusReason = false;
        //        response.Message = "No Record Found!";

        //    }
        //    return Ok(response);

        //}

        //public class RoleCountData
        //{
        //    public int RoleId { get; set; }
        //    public string RoleName { get; set; }
        //    public int Count { get; set; }
        //}

        //[HttpGet]
        //[Route("GetRoleCount")]
        //[Authorize]
        //public IHttpActionResult GetRoleCount()
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        List<RoleCountData> RoleCountDataList = new List<RoleCountData>();
        //        var RoleCountData = (from ad in _db.Role
        //                             where ad.IsDeleted == false
        //                             select new
        //                             {
        //                                 ad.RoleId,
        //                                 ad.RoleType
        //                             }).ToList();
        //        foreach (var item in RoleCountData)
        //        {
        //            RoleCountData data = new RoleCountData();

        //            data.RoleId = item.RoleId;
        //            data.RoleName = item.RoleType;
        //            var CountData = (from ad in _db.Employee where ad.RoleId == item.RoleId && ad.IsDeleted == false select ad).Count();

        //            data.Count = CountData;

        //            RoleCountDataList.Add(data);
        //        }

        //        return Ok(RoleCountDataList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //}

        //#region This Api Use Get EmployeeTypes
        ///// <summary>
        ///// Change the Response ankit
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("GetEmployeeTypes")]
        //[Authorize]
        //public async Task<ResponseBodyModel> GetEmployeeTypes()
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        Base response = new Base();

        //        var employeeData = _db.EmployeeType.Where(x => x.IsDeleted == false).ToList();
        //        if (employeeData.Count > 0)
        //        {
        //            res.Status = true;
        //            res.Message = "Record Found";
        //            res.Data = employeeData;
        //        }
        //        else
        //        {
        //            res.Status = false;
        //            res.Message = "No Record Found!";
        //            res.Data = employeeData;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}
        //#endregion

        //#region This Api Use Get BlooodGroup
        ///// <summary>
        ///// Response change by ankit
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("GetBloodGroup")]
        //[Authorize]
        //public ResponseBodyModel GetBloodGroup()
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        Base response = new Base();

        //        var bloodGroupData = _db.BloodGroup.Where(x => x.BloodGroupId >= 0).ToList();
        //        if (bloodGroupData.Count > 0)
        //        {
        //            res.Status = true;
        //            res.Message = "Record Found";
        //            res.Data = bloodGroupData;
        //        }
        //        else
        //        {
        //            res.Status = false;
        //            res.Message = "No Record Found!";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}
        //#endregion

        //[HttpGet]
        //[Route("GetEmployeeByDepartment")]
        //[Authorize]
        //public IHttpActionResult GetEmployeeByDepartment(int Id)
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        Base response = new Base();
        //        List<DepartmentEmployeeData> employeeDataList = new List<DepartmentEmployeeData>();
        //        var employeeData = (from ad in _db.Employee
        //                            join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                            where ad.RoleId == Id && ad.IsDeleted == false && ad.EmployeeTypeId == 2 || ad.EmployeeTypeId == 4
        //                            select new
        //                            {
        //                                ad.EmployeeId,
        //                                n = ad.FirstName + " " + ad.LastName,
        //                                ad.PrimaryContact,
        //                                ad.PersonalEmail,
        //                                bd.RoleType
        //                            }).ToList();
        //        foreach (var item in employeeData)
        //        {
        //            DepartmentEmployeeData data = new DepartmentEmployeeData();
        //            data.EmployeeId = item.EmployeeId;
        //            data.FullName = item.n;
        //            data.PrimaryContact = item.PrimaryContact;
        //            data.Email = item.PersonalEmail;
        //            data.RoleType = item.RoleType;
        //            employeeDataList.Add(data);
        //        }
        //        if (employeeDataList.Count != 0)
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Data Found";
        //            response.departmentEmployeeData = employeeDataList;
        //        }
        //        else
        //        {
        //            response.StatusReason = true;
        //            response.Message = "Data Found";
        //        }

        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("CreatePerformance")]
        //[Authorize]
        //public IHttpActionResult CreatePerformance(Performance Performance)
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        Base response = new Base();
        //        Performance PerformanceData = new Performance();

        //        PerformanceData.HR = Performance.HR;
        //        PerformanceData.Employee = Performance.Employee;
        //        PerformanceData.ProjectManager = Performance.ProjectManager;
        //        PerformanceData.ProjectName = Performance.ProjectName;
        //        PerformanceData.IsActive = true;
        //        PerformanceData.IsDeleted = false;
        //        _db.Performance.Add(PerformanceData);
        //        _db.SaveChanges();

        //        var EmployeeData = _db.Employee.Where(x => x.EmployeeId == Performance.Employee).FirstOrDefault();
        //        var senderData = _db.Employee.Where(x => x.EmployeeId == Performance.HR).FirstOrDefault();
        //        var position = _db.Role.Where(x => x.RoleId == EmployeeData.RoleId).FirstOrDefault();

        //        UserEmailDTOResponse responseMail = new UserEmailDTOResponse();
        //        if (EmployeeData.PersonalEmail != null)
        //        {
        //            UserEmail MailModel = new UserEmail();
        //            MailModel.To = "sumit@moreyeahs.co";
        //            //MailModel.FromMail = senderData.Email;
        //            //MailModel.MailPassword = Performance.Password;
        //            MailModel.Subject = "Performance Feedback"; //add subject here
        //            MailModel.Body = "Hello Team,<br><br> " +
        //            "We are in the process of gathering feedback for " + EmployeeData.FirstName + " " + EmployeeData.LastName + ", working with our organisation at the position of " + position.RoleType + ".<br>" +
        //            "As " + EmployeeData.FirstName + " " + EmployeeData.LastName + " is working with you, your input will be extremely helpful to understand her overall performance.<br>" +
        //            "Please take a few minutes to elaborate about the performance of her.It would be really grateful if you can add any specific comment you have about her performance.<br>" +
        //            "Also please do not hesitate to mention any weakness along with her strengths.As she is keen to know about his areas of improvement.<br>" +
        //            "Please provide the feedback in the format mentioned below.<br>" +
        //            "Detailed comment is mandatory to fill.<br>" +
        //            "Performance:<br>" +
        //            "Detailed comment-<br><br>" +
        //            "Output:<br>" +
        //            "Detailed comment-<br><br>" +
        //            "Personality, Behavior:<br>" +
        //            "Detailed Comment:-<br><br>" +
        //            "Punctuality, Regularity:<br>" +
        //            "Detailed Comment:-<br><br>" +
        //            "Communication:<br>" +
        //            "Detailed Comment:-<br><br>" +
        //            "Technical Knowledge:<br>" +
        //            "Detailed Comment:-<br><br>" +
        //            "Understanding Level:<br>" +
        //            "Detailed Comment-<br><br>" +
        //            "Learning Skills:<br>" +
        //            "Detailed Comment-<br><br>" +
        //            "Improvement seen:<br>" +
        //            "Detailed Comment-<br><br>" +
        //            "Areas of need to improve:-<br><br><br>" +
        //            "Mark between 0 - 10:<br>" +
        //            "0 - 4 Poor 5 - Average 6 - 7 Good 8 - 9 Very Good 10 – Excellent<br>" +
        //            "We are looking forward to your quick response!"; //add body here
        //            var EmailResponse = UserEmailHelper.SendEmail(MailModel);
        //        }
        //        else
        //        {
        //            responseMail.Message = "Email Doesn't exist";
        //            responseMail.Success = false;
        //        }

        //        response.StatusReason = true;
        //        response.Message = "Data Saved Successfully";

        //        return Ok(response);
        //    }

        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpGet]
        //[Route("GetPerformanceById")]
        //[Authorize]
        //public IHttpActionResult GetPerformance()
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        List<PerformanceData> performanceDataList = new List<PerformanceData>();
        //        var PerformanceData = (from ad in _db.Performance
        //                               join bd in _db.Employee on ad.HR equals bd.EmployeeId
        //                               join cd in _db.Employee on ad.Employee equals cd.EmployeeId
        //                               join fd in _db.Employee on ad.ProjectManager equals fd.EmployeeId
        //                               join pt in _db.Project on ad.ProjectName equals pt.ProjectId
        //                               select new
        //                               {
        //                                   ad.FeedbackId,
        //                                   hr = bd.FirstName + " " + bd.LastName,
        //                                   emp = cd.FirstName + " " + cd.LastName,
        //                                   manger = fd.FirstName + " " + fd.LastName,
        //                                   pt.ProjectName
        //                               }).ToList();
        //        foreach (var item in PerformanceData)
        //        {
        //            PerformanceData data = new PerformanceData();

        //            data.FeedbackId = item.FeedbackId;
        //            data.HR = item.hr;
        //            data.Employee = item.emp;
        //            data.ProjectName = item.ProjectName;
        //            data.ProjectManager = item.manger;

        //            performanceDataList.Add(data);
        //        }

        //        return Ok(performanceDataList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //}

        //[HttpGet]
        //[Route("GetDays")]
        //[Authorize]
        //public IHttpActionResult GetDays()
        //{
        //    var identity = User.Identity as ClaimsIdentity;
        //    int userid = 0;
        //    int compid = 0;
        //    int orgid = 0;

        //    // Access claims

        //    if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //        userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //    if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //        compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //    if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //        orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //    Base response = new Base();
        //    List<Employee> birthDatalist = new List<Employee>();
        //    List<Employee> anniversaryDatalist = new List<Employee>();
        //    var employeeData = (from ad in _db.Employee where ad.EmployeeTypeId != 3 && ad.IsDeleted == false select ad).ToList();
        //    foreach (var item in employeeData)
        //    {
        //        var matchddate = DateTime.Today.ToString("dd MMMM");
        //        DateTime dt = Convert.ToDateTime(item.DateOfBirth);
        //        string month = dt.ToString("MMMM");
        //        string dates = dt.Date.ToString("dd");
        //        string final = dates + " " + month;
        //        if (matchddate == final)
        //        {
        //            Employee obj = new Employee();
        //            obj.FirstName = item.FirstName;
        //            obj.LastName = item.LastName;
        //            obj.DateOfBirth = item.DateOfBirth;
        //            obj.EmployeeId = item.EmployeeId;
        //            birthDatalist.Add(obj);
        //        }
        //    }
        //    foreach (var item in employeeData)
        //    {
        //        var matchddate = DateTime.Today.ToString("dd MMMM");
        //        DateTime dt = Convert.ToDateTime(item.JoiningDate);
        //        string month = dt.ToString("MMMM");
        //        string dates = dt.Date.ToString("dd");
        //        string final = dates + " " + month;
        //        if (matchddate == final)
        //        {
        //            Employee obj = new Employee();
        //            obj.FirstName = item.FirstName;
        //            obj.LastName = item.LastName;
        //            obj.JoiningDate = item.JoiningDate;
        //            obj.EmployeeId = item.EmployeeId;
        //            anniversaryDatalist.Add(obj);
        //        }
        //    }
        //    response.Message = "Data Found";
        //    response.StatusReason = true;
        //    response.birthData = birthDatalist;
        //    response.anniversaryData = anniversaryDatalist;
        //    return Ok(response);
        //}

        ///// <summary>
        ///// Api to get Birthdays and Anniversary for Next Seven Days
        ///// created by Nayan Pancholi
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("Getbirthdaysanniversaryfornextsevendays")]
        //[Authorize]
        //public async Task<ResponseBodyModel> Getbirthdaysanniversaryfornextsevendays()
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();

        //    GetEmpBdayandAnyModel response = new GetEmpBdayandAnyModel();
        //    try
        //    {
        //        var currentday = DateTime.Now.Date;
        //        var nextdate = DateTime.Now.Date.AddDays(1);

        //        var employeeData = _db.Employee.Where(x => x.IsActive == true && x.IsDeleted == false && x.EmployeeTypeId != 3).ToList();
        //        var nextSevenDay = DateTime.Now.Date.AddDays(7);

        //        var currentbdayEmp = employeeData.Where(x => (x.DateOfBirth.Day == DateTime.Now.Day && x.DateOfBirth.Month == DateTime.Now.Month))
        //                .Select(x => new BdayEmployee
        //                {
        //                    EmployeeId = x.EmployeeId,
        //                    EmployeeName = x.FirstName + " " + x.LastName,
        //                    DOB = x.DateOfBirth,

        //                }).ToList().OrderBy(x => x.DOB).ToList();
        //        response.CurrentBirthdatList = currentbdayEmp;

        //        var bdayEmp = employeeData.Where(x => (x.DateOfBirth.Day >= nextdate.Day && x.DateOfBirth.Month >= DateTime.Now.Month) &&
        //                (x.DateOfBirth.Day <= nextSevenDay.Date.Day && x.DateOfBirth.Month <= nextSevenDay.Date.Month))
        //                .Select(x => new BdayEmployee
        //                {
        //                    EmployeeId = x.EmployeeId,
        //                    EmployeeName = x.FirstName + " " + x.LastName,
        //                    DOB = x.DateOfBirth,

        //                }).ToList().OrderBy(x => x.DOB).ToList();
        //        response.BirthdatList = bdayEmp;

        //        var curentannyEmp = employeeData.Where(x => (x.JoiningDate.Day == DateTime.Now.Day && x.JoiningDate.Month == DateTime.Now.Month))
        //                .Select(x => new AnniEmployee
        //                {
        //                    EmployeeId = x.EmployeeId,
        //                    EmployeeName = x.FirstName + " " + x.LastName,
        //                    JoiningDate = x.JoiningDate,

        //                }).ToList().OrderBy(x => x.JoiningDate).ToList();
        //        response.CurrentAnniversaryList = curentannyEmp;

        //        var annyEmp = employeeData.Where(x => (x.JoiningDate.Day >= nextdate.Day && x.JoiningDate.Month >= DateTime.Now.Month) &&
        //                (x.JoiningDate.Day <= nextSevenDay.Date.Day && x.JoiningDate.Month <= nextSevenDay.Date.Month))
        //                .Select(x => new AnniEmployee
        //                {
        //                    EmployeeId = x.EmployeeId,
        //                    EmployeeName = x.FirstName + " " + x.LastName,
        //                    JoiningDate = x.JoiningDate,

        //                }).ToList().OrderBy(x => x.JoiningDate).ToList();
        //        response.AnniversaryList = annyEmp;

        //        res.Message = "List";
        //        res.Status = true;
        //        res.Data = response;
        //    }

        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}
        //public class BdayEmployee
        //{
        //    public int EmployeeId { get; set; }
        //    public string EmployeeName { get; set; }
        //    public DateTime? DOB { get; set; }
        //}
        //public class AnniEmployee
        //{
        //    public int EmployeeId { get; set; }
        //    public string EmployeeName { get; set; }
        //    public DateTime? JoiningDate { get; set; }
        //}
        //public class GetEmpBdayandAnyModel
        //{
        //    public List<BdayEmployee> CurrentBirthdatList { get; set; }
        //    public List<BdayEmployee> BirthdatList { get; set; }
        //    public List<AnniEmployee> CurrentAnniversaryList { get; set; }
        //    public List<AnniEmployee> AnniversaryList { get; set; }
        //}

        ////[HttpGet]
        ////[Route("GetTodaysbirthandAnni")]
        ////[Authorize]
        ////public async Task<ResponseBodyModel> GetTodaysbirthandAnni()
        ////{
        ////    ResponseBodyModel res = new ResponseBodyModel();
        ////    GetEmpBdayandAnyModel response = new GetEmpBdayandAnyModel();
        ////    try
        ////    {
        ////        var employeeData = db.Employee.Where(x => x.IsActive == true && x.IsDeleted == false && x.EmployeeTypeID != 3).ToList();
        ////        var nextSevenDay = DateTime.Now.Date;

        ////        var bdayEmp = employeeData.Where(x => (x.DOB.Day == DateTime.Now.Day && x.DOB.Month == DateTime.Now.Month))
        ////                .Select(x => new
        ////                {
        ////                    x.EmployeeId,
        ////                    EmployeeName = x.FirstName + " " + x.LastName,
        ////                    x.DOB,

        ////                }).ToList().OrderBy(x => x.DOB).ToList();
        ////        var annyEmp = employeeData.Where(x => (x.JoiningDate.Day == DateTime.Now.Day && x.JoiningDate.Month == DateTime.Now.Month))
        ////                .Select(x => new
        ////                {
        ////                    x.EmployeeId,
        ////                    EmployeeName = x.FirstName + " " + x.LastName,
        ////                    x.JoiningDate,

        ////                }).ToList().OrderBy(x => x.JoiningDate).ToList();
        ////        response.BirthdatList = bdayEmp;
        ////        response.AnniversaryList = annyEmp;
        ////        res.Message = "List";
        ////        res.Status = true;
        ////        res.Data = response;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        res.Message = ex.Message;
        ////        res.Status = false;
        ////    }
        ////    return res;
        ////}

        //[Route("GetBirthGraph")]
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult GetBirthGraph()
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        string[] monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;

        //        foreach (string m in monthNames) // writing out
        //        {
        //            Console.WriteLine(m);
        //        }
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[Route("ReadExcelFile")]
        //[HttpPost]
        //[Authorize]
        //public IHttpActionResult ReadExcelFile()
        //{
        //    try
        //    {
        //        string message = "";
        //        HttpResponseMessage ResponseMessage = null;
        //        var httpRequest = HttpContext.Current.Request;
        //        DataSet dsexcelRecords = new DataSet();
        //        IExcelDataReader reader = null;
        //        HttpPostedFile Inputfile = null;
        //        Stream FileStream = null;

        //        if (httpRequest.Files.Count > 0)
        //        {
        //            Inputfile = httpRequest.Files[0];
        //            FileStream = Inputfile.InputStream;

        //            string filePath = string.Empty;
        //            string fileExt = string.Empty;

        //            filePath = Inputfile.FileName; //get the path of the file
        //            fileExt = Path.GetExtension(filePath); //get the file extension
        //            if (fileExt.CompareTo(".xls") == 0 || fileExt.CompareTo(".xlsx") == 0)
        //            {
        //                //DataTable dtExcel = new DataTable();
        //                //dtExcel = ReadExcel(filePath, fileExt); //read excel file
        //                //dataGridView1.Visible = true;
        //                //dataGridView1.DataSource = dtExcel;

        //            }

        //            if (Inputfile != null && FileStream != null)
        //            {
        //                if (Inputfile.FileName.EndsWith(".xls"))
        //                    reader = ExcelReaderFactory.CreateBinaryReader(FileStream);
        //                else if (Inputfile.FileName.EndsWith(".xlsx"))
        //                    reader = ExcelReaderFactory.CreateOpenXmlReader(FileStream);
        //                else
        //                    message = "The file format is not supported.";

        //                //dsexcelRecords = reader.AsDataSet();
        //                reader.Close();

        //                if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
        //                {
        //                    DataTable dtStudentRecords = dsexcelRecords.Tables[0];
        //                    for (int i = 0; i < dtStudentRecords.Rows.Count; i++)
        //                    {
        //                        Employee objEmployee = new Employee();
        //                        objEmployee.FirstName = Convert.ToString(dtStudentRecords.Rows[i][0]);
        //                        objEmployee.LastName = Convert.ToString(dtStudentRecords.Rows[i][1]);
        //                        objEmployee.MiddleName = Convert.ToString(dtStudentRecords.Rows[i][2]);
        //                        //objEmployee.EmployeeCode = Convert.ToString(dtStudentRecords.Rows[i][3]);
        //                        objEmployee.PersonalEmail = Convert.ToString(dtStudentRecords.Rows[i][4]);
        //                        var objEmployee1 = _db.Role.Where(x => x.RoleType == dtStudentRecords.Rows[i][6]).FirstOrDefault();

        //                        objEmployee.RoleId = Convert.ToInt32(objEmployee1.RoleId);
        //                        objEmployee.PrimaryContact = Convert.ToString(dtStudentRecords.Rows[i][6]);
        //                        objEmployee.SecondaryContact = Convert.ToString(dtStudentRecords.Rows[i][7]);
        //                        objEmployee.MaritalStatus = Convert.ToString(dtStudentRecords.Rows[i][8]);
        //                        objEmployee.SpouseName = Convert.ToString(dtStudentRecords.Rows[i][9]);
        //                        objEmployee.FatherName = Convert.ToString(dtStudentRecords.Rows[i][10]);
        //                        objEmployee.MotherName = Convert.ToString(dtStudentRecords.Rows[i][11]);
        //                        objEmployee.CreatedOn = DateTime.Now;
        //                        objEmployee.JoiningDate = Convert.ToDateTime(dtStudentRecords.Rows[i][13]);
        //                        objEmployee.ConfirmationDate = Convert.ToDateTime(dtStudentRecords.Rows[i][14]);
        //                        objEmployee.DateOfBirth = Convert.ToDateTime(dtStudentRecords.Rows[i][15]);
        //                        //objEmployee.EmergencyNumber = Convert.ToInt32(dtStudentRecords.Rows[i][16]);
        //                        objEmployee.WhatsappNumber = Convert.ToString(dtStudentRecords.Rows[i][17]);
        //                        objEmployee.AadharNumber = Convert.ToString(dtStudentRecords.Rows[i][18]);
        //                        objEmployee.PanNumber = Convert.ToString(dtStudentRecords.Rows[i][19]);
        //                        objEmployee.PermanentAddress = Convert.ToString(dtStudentRecords.Rows[i][20]);
        //                        objEmployee.LocalAddress = Convert.ToString(dtStudentRecords.Rows[i][21]);
        //                        objEmployee.MedicalIssue = Convert.ToString(dtStudentRecords.Rows[i][22]);
        //                        objEmployee.ProfileImageUrl = Convert.ToString(dtStudentRecords.Rows[i][23]);
        //                        objEmployee.Salary = int.Parse(Convert.ToString(dtStudentRecords.Rows[i][24]));
        //                        objEmployee.BankAccountNumber = Convert.ToString(dtStudentRecords.Rows[i][25]);
        //                        objEmployee.Password = Convert.ToString(dtStudentRecords.Rows[i][26]);
        //                        objEmployee.IFSC = Convert.ToString(dtStudentRecords.Rows[i][27]);
        //                        objEmployee.AccountHolderName = Convert.ToString(dtStudentRecords.Rows[i][28]);
        //                        objEmployee.BankName = Convert.ToString(dtStudentRecords.Rows[i][29]);
        //                        objEmployee.OfficeEmail = Convert.ToString(dtStudentRecords.Rows[i][30]);
        //                        objEmployee.EmployeeTypeId = Convert.ToInt32(dtStudentRecords.Rows[i][31]);
        //                        objEmployee.CompanyName = Convert.ToString(dtStudentRecords.Rows[i][32]);
        //                        //objEmployee.Employee.Add(objEmployee);
        //                        _db.Employee.Add(objEmployee);

        //                    }

        //                    int output = _db.SaveChanges();
        //                    if (output > 0)
        //                        message = "The Excel file has been successfully uploaded.";
        //                    else
        //                        message = "Something Went Wrong!, The Excel file uploaded has fiald.";
        //                }
        //                else
        //                    message = "Selected file is empty.";
        //            }
        //            else
        //                message = "Invalid File.";
        //        }
        //        else
        //            ResponseMessage = Request.CreateResponse(HttpStatusCode.BadRequest);
        //        return Ok(ResponseMessage);
        //        // return message;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //}
        ////#region#Excel Import for Employees
        ////[HttpPost]
        ////[Route("CreateExcelEmployee")]
        ////[Authorize]
        ////public async Task<IHttpActionResult> CreateExcelEmployeeAsync(List<ImportEmployee> Employees)// we are using contact table as a employee
        ////{
        ////    try
        ////    {
        ////        var identity = User.Identity as ClaimsIdentity;
        ////        int userid = 0;
        ////        int compid = 0;
        ////        int orgid = 0;
        ////        string firstName = ""; string lastName = ""; string middleName = "";
        ////        string defaultmailid = "";
        ////        string defaultmobilenumber = "";
        ////        string password = "";
        ////        var Password = "Moreyeahs@123";
        ////        // Access claims

        ////        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        ////            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        ////        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        ////            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        ////        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        ////            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        ////        Base response = new Base();
        ////        foreach (var Employee in
        ////            Employees)
        ////        {
        ////            var emp = (from ad in _db.Employee where ad.CompanyId == compid && ad.OrgId == orgid && ad.OfficeEmail == Employee.Moreyeahs_Mail_Id select ad).FirstOrDefault();

        ////            if (emp == null)
        ////            {
        ////                Employee EmployeeData = new Employee();
        ////                EmployeeData.FirstName = Employee.First_Name;
        ////                firstName = Employee.First_Name;
        ////                EmployeeData.LastName = Employee.Last_Name;
        ////                lastName = Employee.Last_Name;
        ////                EmployeeData.Email = Employee.Email;
        ////                defaultmailid = Employee.Email;
        ////                defaultmobilenumber = Employee.Primary_Contact;
        ////                EmployeeData.OrgId = orgid;
        ////                if (Employee.Profile != null)
        ////                {
        ////                    var roleData = (from ad in _db.Role where ad.RoleType == Employee.Profile select ad).FirstOrDefault();
        ////                    if (roleData != null)
        ////                    {
        ////                        EmployeeData.RoleId = roleData.RoleId;
        ////                    }
        ////                    else
        ////                    {
        ////                        EmployeeData.RoleId = 30;
        ////                    }
        ////                }
        ////                else
        ////                {
        ////                    EmployeeData.RoleId = 30;
        ////                }
        ////                EmployeeData.PrimaryContact = Employee.Primary_Contact;
        ////                EmployeeData.MaritalStatus = Employee.Marital_Status;
        ////                EmployeeData.SpouseName = Employee.Spouse_Name;
        ////                EmployeeData.FatherName = Employee.Father_Name;
        ////                EmployeeData.MotherName = Employee.Mother_Name;
        ////                EmployeeData.CreatedOn = DateTime.Now;
        ////                EmployeeData.IsActive = true;
        ////                EmployeeData.IsDeleted = false;

        ////                EmployeeData.OrgId = orgid;

        ////                EmployeeData.PanNumber = Employee.Pan_Number;
        ////                EmployeeData.PermanentAddress = Employee.Permanent_Address;
        ////                EmployeeData.LocalAddress = Employee.Local_Address;
        ////                EmployeeData.MedicalIssue = Employee.Medical_Issue;
        ////                EmployeeData.BankAccountNumber = Employee.Bank_Account_Number;
        ////                EmployeeData.Password = "Moreyeahs@123";
        ////                EmployeeData.IFSC = Employee.IFSC;
        ////                EmployeeData.AccountHolderName = Employee.Account_Holder_Name;
        ////                EmployeeData.BankName = Employee.Bank_Name;
        ////                EmployeeData.OfficeEmail = Employee.Moreyeahs_Mail_Id;
        ////                EmployeeData.SecondaryJobTitle = Employee.SecondaryJobTitle;
        ////                EmployeeData.ReportingManager = Employee.ReportingManager;
        ////                EmployeeData.BiometricID = Employee.BiometricID;

        ////                if (Employee.Employee_Type != null)
        ////                {
        ////                    var typeData = (from ad in _db.EmployeeType where ad.EmployeeTypes == Employee.Employee_Type select ad).FirstOrDefault();
        ////                    if (typeData != null)
        ////                    {
        ////                        EmployeeData.EmployeeTypeID = typeData.EmployeeTypeId;
        ////                    }
        ////                    else
        ////                    {
        ////                        EmployeeData.EmployeeTypeID = 5;
        ////                    }
        ////                }
        ////                else
        ////                {
        ////                    EmployeeData.EmployeeTypeID = 5;
        ////                }
        ////                _db.Employee.Add(EmployeeData);
        ////                _db.SaveChanges();
        ////                User UserData = new User();
        ////                UserData.UserName = Employee.Moreyeahs_Mail_Id;
        ////                UserData.Password = "F7-19-04-CC-DC-D7-ED-1B-CD-56-E6-55-6E-1F-2F-0F";
        ////                UserData.HashCode = "ztnDfBpCbe";
        ////                UserData.EmployeeId = EmployeeData.EmployeeId;

        ////                if (Employee.Profile != null)
        ////                {
        ////                    var roleData = (from ad in _db.Role where ad.RoleType == Employee.Profile select ad).FirstOrDefault();
        ////                    if (roleData != null)
        ////                    {
        ////                        UserData.RoleId = roleData.RoleId;
        ////                    }
        ////                    else
        ////                    {
        ////                        UserData.RoleId = 30;
        ////                    }
        ////                }
        ////                else
        ////                {
        ////                    UserData.RoleId = 30;
        ////                }

        ////                UserData.IsDeleted = false;
        ////                UserData.IsActive = true;
        ////                UserData.CreatedOn = DateTime.Now;
        ////                UserData.CompanyId = compid;
        ////                UserData.OrgId = orgid;
        ////                _db.User.Add(UserData);
        ////                _db.SaveChanges();
        ////                //ASP Net User
        ////                byte Levels = 4;
        ////                //User Info
        ////                var user = new ApplicationUser()
        ////                {
        ////                    FirstName = firstName,
        ////                    LastName = lastName,
        ////                    PhoneNumber = defaultmobilenumber,
        ////                    Level = Levels,
        ////                    JoinDate = DateTime.Now.Date,
        ////                    EmailConfirmed = true,
        ////                    Email = defaultmailid,
        ////                    PasswordHash = Password,
        ////                    UserName = defaultmailid
        ////                };
        ////                //Employee.OfficeEmail = Employee.OfficeEmail;
        ////                // ApplicationUserManager Ab = new ApplicationUserManager();
        ////                string displayname = defaultmailid;
        ////                IdentityResult result = await this.AppUserManager.CreateAsync(user, Password);
        ////                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        ////                var adminUser = manager.FindByName(displayname);
        ////                var RoleData = _db.Role.Where(x => x.RoleType == Employee.Profile).FirstOrDefault();
        ////                // assign User role
        ////                // if (RoleData != null)
        ////                // {
        ////                // user.UserName = RoleData.RoleType;
        ////                // }
        ////                //else
        ////                // {
        ////                // user.UserName = Employee.Profile;
        ////                // }
        ////                //
        ////            }
        ////            else
        ////            {
        ////                emp.FirstName = Employee.First_Name;
        ////                emp.LastName = Employee.Last_Name;
        ////                emp.Email = Employee.Email;

        ////                if (Employee.Profile != null)
        ////                {
        ////                    var roleData = (from ad in _db.Role where ad.RoleType == Employee.Profile select ad).FirstOrDefault();
        ////                    if (roleData != null)
        ////                    {
        ////                        emp.RoleId = roleData.RoleId;
        ////                    }
        ////                    else
        ////                    {
        ////                        emp.RoleId = 30;
        ////                    }
        ////                }
        ////                else
        ////                {
        ////                    emp.RoleId = 30;
        ////                }

        ////                emp.PrimaryContact = Employee.Primary_Contact;
        ////                emp.MaritalStatus = Employee.Marital_Status;
        ////                emp.SpouseName = Employee.Spouse_Name;
        ////                emp.FatherName = Employee.Father_Name;
        ////                emp.MotherName = Employee.Mother_Name;
        ////                emp.CreatedOn = DateTime.Now;
        ////                emp.IsActive = true;
        ////                emp.IsDeleted = false;

        ////                //if (Employee.Blood_Group == null)
        ////                //{
        ////                //    emp.BloodGroupId = 9;
        ////                //}
        ////                //else
        ////                //{
        ////                //    var bloodData = (from ad in db.BloodGroup where ad.BloodGroupType == Employee.Blood_Group select ad).FirstOrDefault();
        ////                //    if (bloodData != null)
        ////                //    {
        ////                //        emp.BloodGroupId = bloodData.BloodGroupId;
        ////                //    }
        ////                //    else
        ////                //    {
        ////                //        emp.BloodGroupId = 9;
        ////                //    }
        ////                //}

        ////                if (Employee.Joining_Date != null)
        ////                {
        ////                    emp.JoiningDate = Convert.ToDateTime(Employee.Joining_Date).AddDays(1);
        ////                }

        ////                if (Employee.Confirmation_Date != null)
        ////                {
        ////                    emp.ConfirmationDate = Convert.ToDateTime(Employee.Confirmation_Date).AddDays(1);
        ////                }

        ////                if (Employee.DateOfBirth != null)
        ////                {
        ////                    emp.DateOfBirth = Convert.ToDateTime(Employee.DateOfBirth).AddDays(1);
        ////                }

        ////                emp.EmergencyNumber = Employee.Emergency_Number;
        ////                emp.WhatsappNumber = Employee.Whatsapp_Number;
        ////                emp.AadharNumber = Employee.Aadhar_Number;

        ////                //if (Employee.Company_Name != null)
        ////                //{
        ////                // var item = (from ad in db.Company where ad.CompanyName == Employee.Company_Name select ad).FirstOrDefault();
        ////                // if (item != null)
        ////                // {
        ////                // emp.CompanyId = item.CompanyId;
        ////                // }
        ////                // else
        ////                // {
        ////                // emp.CompanyId = 1;
        ////                // }
        ////                //}
        ////                //else
        ////                //{
        ////                // emp.CompanyId = 1;
        ////                //}

        ////                emp.CompanyId = compid;
        ////                emp.OrgId = orgid;

        ////                emp.PanNumber = Employee.Pan_Number;
        ////                emp.PermanentAddress = Employee.Permanent_Address;
        ////                emp.LocalAddress = Employee.Local_Address;
        ////                emp.MedicalIssue = Employee.Medical_Issue;
        ////                emp.Salary = int.Parse(Employee.Salary);
        ////                emp.BankAccountNumber = Employee.Bank_Account_Number;
        ////                emp.Password = "Moreyeahs@123";
        ////                emp.IFSC = Employee.IFSC;
        ////                emp.AccountHolderName = Employee.Account_Holder_Name;
        ////                emp.BankName = Employee.Bank_Name;
        ////                emp.OfficeEmail = Employee.Moreyeahs_Mail_Id;

        ////                if (Employee.Employee_Type != null)
        ////                {
        ////                    var typeData = (from ad in _db.EmployeeType where ad.EmployeeTypes == Employee.Employee_Type select ad).FirstOrDefault();
        ////                    if (typeData != null)
        ////                    {
        ////                        emp.EmployeeTypeID = typeData.EmployeeTypeId;
        ////                    }
        ////                    else
        ////                    {
        ////                        emp.EmployeeTypeID = 5;
        ////                    }
        ////                }
        ////                else
        ////                {
        ////                    emp.EmployeeTypeID = 5;
        ////                }

        ////                _db.SaveChanges();

        ////                var user = (from ad in _db.User where ad.UserName == Employee.Moreyeahs_Mail_Id select ad).FirstOrDefault();
        ////                // User UserData = new User();
        ////                user.UserName = Employee.Moreyeahs_Mail_Id;
        ////                user.Password = "F7-19-04-CC-DC-D7-ED-1B-CD-56-E6-55-6E-1F-2F-0F";
        ////                user.HashCode = "ztnDfBpCbe";
        ////                user.EmployeeId = emp.EmployeeId;

        ////                if (Employee.Profile != null)
        ////                {
        ////                    var roleData = (from ad in _db.Role where ad.RoleType == Employee.Profile select ad).FirstOrDefault();
        ////                    if (roleData != null)
        ////                    {
        ////                        user.RoleId = roleData.RoleId;
        ////                    }
        ////                    else
        ////                    {
        ////                        user.RoleId = 30;
        ////                    }
        ////                }
        ////                else
        ////                {
        ////                    user.RoleId = 30;
        ////                }

        ////                user.IsDeleted = false;
        ////                user.IsActive = true;
        ////                user.CreatedOn = DateTime.Now;

        ////                _db.SaveChanges();
        ////            }

        ////        }
        ////        response.StatusReason = true;
        ////        response.Message = "Data Saved Successfully";
        ////        return Ok(response);
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        return BadRequest(ex.Message);
        ////    }
        ////}

        ////#endregion

        ////[HttpPost]
        ////[Route("CreateExcelEmployeeV1")]
        ////[Authorize]

        ////public async Task<IHttpActionResult> CreateExcelEmployeeAsync1(List<ImportEmployee> Employees, int orgid)// we are using contact table as a employee
        ////{
        ////    try
        ////    {
        ////        var identity = User.Identity as ClaimsIdentity;
        ////        int userid = 0;
        ////        int compid = 0;

        ////        string firstName = ""; string lastName = ""; string middleName = "";
        ////        string defaultmailid = "";
        ////        string defaultmobilenumber = "";
        ////        string password = "";
        ////        var Password = "Moreyeahs@123";
        ////        // Access claims

        ////        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        ////            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        ////        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        ////            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        ////        //if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        ////        //    orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        ////        Base response = new Base();
        ////        foreach (var Employee in Employees)
        ////        {
        ////            var emp = (from ad in _db.Employee where ad.CompanyId == compid && ad.OrgId == orgid && ad.OfficeEmail == Employee.Moreyeahs_Mail_Id select ad).FirstOrDefault();

        ////            if (emp == null)
        ////            {
        ////                Employee EmployeeData = new Employee();
        ////                EmployeeData.FirstName = Employee.First_Name;
        ////                firstName = Employee.First_Name;
        ////                EmployeeData.LastName = Employee.Last_Name;
        ////                lastName = Employee.Last_Name;
        ////                EmployeeData.Email = Employee.Email;
        ////                defaultmailid = Employee.Email;
        ////                defaultmobilenumber = Employee.Primary_Contact;
        ////                EmployeeData.OrgId = orgid;
        ////                if (Employee.Profile != null)
        ////                {
        ////                    var roleData = (from ad in _db.Role where ad.RoleType == Employee.Profile select ad).FirstOrDefault();
        ////                    if (roleData != null)
        ////                    {
        ////                        EmployeeData.RoleId = roleData.RoleId;
        ////                    }
        ////                    else
        ////                    {
        ////                        EmployeeData.RoleId = 30;
        ////                    }
        ////                }
        ////                else
        ////                {
        ////                    EmployeeData.RoleId = 30;
        ////                }

        ////                EmployeeData.PrimaryContact = Employee.Primary_Contact;
        ////                EmployeeData.MaritalStatus = Employee.Marital_Status;
        ////                // EmployeeData.SpouseName = Employee.Spouse_Name;
        ////                EmployeeData.FatherName = Employee.Father_Name;
        ////                EmployeeData.MotherName = Employee.Mother_Name;
        ////                EmployeeData.CreatedOn = DateTime.Now;
        ////                EmployeeData.IsActive = true;
        ////                EmployeeData.IsDeleted = false;

        ////                //if (Employee.Blood_Group == null)
        ////                //{
        ////                //    EmployeeData.BloodGroupId = 9;
        ////                //}
        ////                //else
        ////                //{
        ////                //    var bloodData = (from ad in db.BloodGroup where ad.BloodGroupType == Employee.Blood_Group select ad).FirstOrDefault();
        ////                //    if (bloodData != null)
        ////                //    {
        ////                //        EmployeeData.BloodGroupId = bloodData.BloodGroupId;
        ////                //    }
        ////                //    else
        ////                //    {
        ////                //        EmployeeData.BloodGroupId = 9;
        ////                //    }
        ////                //}

        ////                if (Employee.Joining_Date != null)
        ////                {
        ////                    DateTime trysss = (DateTime)Employee.Joining_Date;
        ////                    // var data = trysss.ToString("dd/MM/yyyy hh:mm:ss");

        ////                    EmployeeData.JoiningDate = trysss.AddDays(1);
        ////                    //EmployeeData.JoiningDate = Convert.ToDateTime(data);

        ////                }

        ////                if (Employee.Confirmation_Date != null)
        ////                {
        ////                    // EmployeeData.ConfirmationDate = Convert.ToDateTime(Employee.Confirmation_Date);

        ////                    EmployeeData.ConfirmationDate = Convert.ToDateTime(Employee.Confirmation_Date).AddDays(1);
        ////                }

        ////                if (Employee.DateOfBirth != null)
        ////                {
        ////                    EmployeeData.DateOfBirth = Convert.ToDateTime(Employee.DateOfBirth).AddDays(1);
        ////                }

        ////                EmployeeData.EmergencyNumber = Employee.Emergency_Number;
        ////                EmployeeData.WhatsappNumber = Employee.Whatsapp_Number;
        ////                EmployeeData.AadharNumber = Employee.Aadhar_Number;
        ////                EmployeeData.CompanyId = compid;
        ////                EmployeeData.OrgId = orgid;
        ////                //if (Employee.Company_Name != null)
        ////                //{
        ////                // var item = (from ad in db.Company where ad.CompanyName == Employee.Company_Name select ad).FirstOrDefault();
        ////                // if (item != null)
        ////                // {
        ////                // EmployeeData.CompanyId = item.CompanyId;
        ////                // }
        ////                // else
        ////                // {
        ////                // EmployeeData.CompanyId = 1;
        ////                // }
        ////                //}
        ////                //else
        ////                //{
        ////                // EmployeeData.CompanyId = 1;
        ////                //}

        ////                EmployeeData.PanNumber = Employee.Pan_Number;
        ////                EmployeeData.PermanentAddress = Employee.Permanent_Address;
        ////                EmployeeData.LocalAddress = Employee.Local_Address;
        ////                EmployeeData.MedicalIssue = Employee.Medical_Issue;
        ////                // EmployeeData.Salary = int.Parse(Employee.Salary);
        ////                EmployeeData.BankAccountNumber = Employee.Bank_Account_Number;
        ////                EmployeeData.Password = "Moreyeahs@123";
        ////                EmployeeData.IFSC = Employee.IFSC;
        ////                EmployeeData.AccountHolderName = Employee.Account_Holder_Name;
        ////                EmployeeData.BankName = Employee.Bank_Name;
        ////                EmployeeData.OfficeEmail = Employee.Moreyeahs_Mail_Id;
        ////                EmployeeData.SecondaryJobTitle = Employee.SecondaryJobTitle;
        ////                EmployeeData.ReportingManager = Employee.ReportingManager;
        ////                EmployeeData.BiometricID = Employee.BiometricID;
        ////                EmployeeData.DepartmentName = Employee.DepartmentName;
        ////                EmployeeData.DesignationName = Employee.DesignationName;
        ////                //EmployeeData.DepartmentName = Employee.DepartmentName;
        ////                //EmployeeData.DesignationId = Employee.DesignationId;
        ////                //EmployeeData.AttendanceNumber = Employee.AttendanceNumber;
        ////                //EmployeeData.InProbation = Employee.InProbation;
        ////                //EmployeeData.TimeType = Employee.TimeType;
        ////                //EmployeeData.WorkerType = Employee.WorkerType;
        ////                //EmployeeData.ShiftType = Employee.ShiftType;
        ////                //EmployeeData.WeeklyOffPolicy = Employee.WeeklyOffPolicy;
        ////                //EmployeeData.NoticePeriodMonths = Employee.NoticePeriodMonths;
        ////                //EmployeeData.CostCenter = Employee.CostCenter;
        ////                //EmployeeData.WorkNumber = Employee.WorkNumber;
        ////                //EmployeeData.PayGroup = Employee.PayGroup;
        ////                //EmployeeData.ResidenceNumber = Employee.ResidenceNumber;
        ////                //EmployeeData.SkypeMail = Employee.SkypeMail;
        ////                //EmployeeData.Band = Employee.Band;

        ////                if (Employee.Employee_Type != null)
        ////                {
        ////                    var typeData = (from ad in _db.EmployeeType where ad.EmployeeTypes == Employee.Employee_Type select ad).FirstOrDefault();
        ////                    if (typeData != null)
        ////                    {
        ////                        EmployeeData.EmployeeTypeID = typeData.EmployeeTypeId;
        ////                    }
        ////                    else
        ////                    {
        ////                        EmployeeData.EmployeeTypeID = 5;
        ////                    }
        ////                }
        ////                else
        ////                {
        ////                    EmployeeData.EmployeeTypeID = 5;
        ////                }

        ////                _db.Employee.Add(EmployeeData);
        ////                _db.SaveChanges();

        ////                User UserData = new User();
        ////                UserData.UserName = Employee.Moreyeahs_Mail_Id;
        ////                UserData.Password = "F7-19-04-CC-DC-D7-ED-1B-CD-56-E6-55-6E-1F-2F-0F";
        ////                UserData.HashCode = "ztnDfBpCbe";
        ////                UserData.EmployeeId = EmployeeData.EmployeeId;

        ////                if (Employee.Profile != null)
        ////                {
        ////                    var roleData = (from ad in _db.Role where ad.RoleType == Employee.Profile select ad).FirstOrDefault();
        ////                    if (roleData != null)
        ////                    {
        ////                        UserData.RoleId = roleData.RoleId;
        ////                    }
        ////                    else
        ////                    {
        ////                        UserData.RoleId = 30;
        ////                    }
        ////                }
        ////                else
        ////                {
        ////                    UserData.RoleId = 30;
        ////                }

        ////                UserData.IsDeleted = false;
        ////                UserData.IsActive = true;
        ////                UserData.CreatedOn = DateTime.Now;
        ////                UserData.CompanyId = compid;
        ////                UserData.OrgId = orgid;
        ////                _db.User.Add(UserData);
        ////                _db.SaveChanges();

        ////                //ASP Net User
        ////                byte Levels = 4;
        ////                //User Info
        ////                var user = new ApplicationUser()
        ////                {
        ////                    FirstName = firstName,
        ////                    LastName = lastName,
        ////                    PhoneNumber = defaultmobilenumber,
        ////                    Level = Levels,
        ////                    JoinDate = DateTime.Now.Date,
        ////                    EmailConfirmed = true,
        ////                    Email = defaultmailid,
        ////                    PasswordHash = Password,
        ////                    UserName = defaultmailid
        ////                };
        ////                //Employee.OfficeEmail = Employee.OfficeEmail;
        ////                // ApplicationUserManager Ab = new ApplicationUserManager();
        ////                string displayname = defaultmailid;

        ////                IdentityResult result = await this.AppUserManager.CreateAsync(user, Password);
        ////                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        ////                var adminUser = manager.FindByName(displayname);
        ////                var RoleData = _db.Role.Where(x => x.RoleType == Employee.Profile).FirstOrDefault();
        ////                // assign User role
        ////                // if (RoleData != null)
        ////                // {
        ////                // user.UserName = RoleData.RoleType;
        ////                // }
        ////                //else
        ////                // {
        ////                // user.UserName = Employee.Profile;
        ////                // }
        ////                //
        ////            }
        ////            else
        ////            {
        ////                emp.FirstName = Employee.First_Name;
        ////                emp.LastName = Employee.Last_Name;
        ////                emp.Email = Employee.Email;

        ////                if (Employee.Profile != null)
        ////                {
        ////                    var roleData = (from ad in _db.Role where ad.RoleType == Employee.Profile select ad).FirstOrDefault();
        ////                    if (roleData != null)
        ////                    {
        ////                        emp.RoleId = roleData.RoleId;
        ////                    }
        ////                    else
        ////                    {
        ////                        emp.RoleId = 30;
        ////                    }
        ////                }
        ////                else
        ////                {
        ////                    emp.RoleId = 30;
        ////                }

        ////                emp.PrimaryContact = Employee.Primary_Contact;
        ////                emp.MaritalStatus = Employee.Marital_Status;
        ////                emp.SpouseName = Employee.Spouse_Name;
        ////                emp.FatherName = Employee.Father_Name;
        ////                emp.MotherName = Employee.Mother_Name;
        ////                emp.CreatedOn = DateTime.Now;
        ////                emp.IsActive = true;
        ////                emp.IsDeleted = false;

        ////                //if (Employee.Blood_Group == null)
        ////                //{
        ////                //    emp.BloodGroupId = 9;
        ////                //}
        ////                //else
        ////                //{
        ////                //    var bloodData = (from ad in db.BloodGroup where ad.BloodGroupType == Employee.Blood_Group select ad).FirstOrDefault();
        ////                //    if (bloodData != null)
        ////                //    {
        ////                //        emp.BloodGroupId = bloodData.BloodGroupId;
        ////                //    }
        ////                //    else
        ////                //    {
        ////                //        emp.BloodGroupId = 9;
        ////                //    }
        ////                //}

        ////                if (Employee.Joining_Date != null)
        ////                {
        ////                    emp.JoiningDate = Convert.ToDateTime(Employee.Joining_Date).AddDays(1);
        ////                }

        ////                if (Employee.Confirmation_Date != null)
        ////                {
        ////                    emp.ConfirmationDate = Convert.ToDateTime(Employee.Confirmation_Date).AddDays(1);
        ////                }

        ////                if (Employee.DateOfBirth != null)
        ////                {
        ////                    emp.DateOfBirth = Convert.ToDateTime(Employee.DateOfBirth).AddDays(1);
        ////                }

        ////                emp.EmergencyNumber = Employee.Emergency_Number;
        ////                emp.WhatsappNumber = Employee.Whatsapp_Number;
        ////                emp.AadharNumber = Employee.Aadhar_Number;

        ////                //if (Employee.Company_Name != null)
        ////                //{
        ////                // var item = (from ad in db.Company where ad.CompanyName == Employee.Company_Name select ad).FirstOrDefault();
        ////                // if (item != null)
        ////                // {
        ////                // emp.CompanyId = item.CompanyId;
        ////                // }
        ////                // else
        ////                // {
        ////                // emp.CompanyId = 1;
        ////                // }
        ////                //}
        ////                //else
        ////                //{
        ////                // emp.CompanyId = 1;
        ////                //}

        ////                emp.CompanyId = compid;
        ////                emp.OrgId = orgid;

        ////                emp.PanNumber = Employee.Pan_Number;
        ////                emp.PermanentAddress = Employee.Permanent_Address;
        ////                emp.LocalAddress = Employee.Local_Address;
        ////                emp.MedicalIssue = Employee.Medical_Issue;
        ////                //emp.Salary = int.Parse(Employee.Salary);
        ////                emp.BankAccountNumber = Employee.Bank_Account_Number;
        ////                emp.Password = "Moreyeahs@123";
        ////                emp.IFSC = Employee.IFSC;
        ////                emp.AccountHolderName = Employee.Account_Holder_Name;
        ////                emp.BankName = Employee.Bank_Name;
        ////                emp.OfficeEmail = Employee.Moreyeahs_Mail_Id;

        ////                if (Employee.Employee_Type != null)
        ////                {
        ////                    var typeData = (from ad in _db.EmployeeType where ad.EmployeeTypes == Employee.Employee_Type select ad).FirstOrDefault();
        ////                    if (typeData != null)
        ////                    {
        ////                        emp.EmployeeTypeID = typeData.EmployeeTypeId;
        ////                    }
        ////                    else
        ////                    {
        ////                        emp.EmployeeTypeID = 5;
        ////                    }
        ////                }
        ////                else
        ////                {
        ////                    emp.EmployeeTypeID = 5;
        ////                }

        ////                _db.SaveChanges();

        ////                var user = (from ad in _db.User where ad.UserName == Employee.Moreyeahs_Mail_Id select ad).FirstOrDefault();
        ////                // User UserData = new User();
        ////                //user.UserName = Employee.Moreyeahs_Mail_Id;
        ////                user.Password = "F7-19-04-CC-DC-D7-ED-1B-CD-56-E6-55-6E-1F-2F-0F";
        ////                user.HashCode = "ztnDfBpCbe";
        ////                user.EmployeeId = emp.EmployeeId;

        ////                if (Employee.Profile != null)
        ////                {
        ////                    var roleData = (from ad in _db.Role where ad.RoleType == Employee.Profile select ad).FirstOrDefault();
        ////                    if (roleData != null)
        ////                    {
        ////                        user.RoleId = roleData.RoleId;
        ////                    }
        ////                    else
        ////                    {
        ////                        user.RoleId = 30;
        ////                    }
        ////                }
        ////                else
        ////                {
        ////                    user.RoleId = 30;
        ////                }

        ////                user.IsDeleted = false;
        ////                user.IsActive = true;
        ////                user.CreatedOn = DateTime.Now;

        ////                _db.SaveChanges();
        ////            }
        ////        }
        ////        response.StatusReason = true;
        ////        response.Message = "Data Saved Successfully";

        ////        return Ok(response);
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        return BadRequest(ex.Message);
        ////    }
        ////}

        //[HttpPost]
        //[Route("AddTimesheet")]
        //[Authorize]
        //public IHttpActionResult AddTimesheet(List<Timesheet> timesheets)// we are using contact table as a employee
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        Base response = new Base();
        //        foreach (var timesheet in timesheets)
        //        {
        //            Timesheet timesheetData = new Timesheet();
        //            timesheetData.Project = timesheet.Project;
        //            timesheetData.Task = timesheet.Task;
        //            timesheetData.Date = timesheet.Date;
        //            timesheetData.Time = timesheet.Time;
        //            timesheetData.Description = timesheet.Description;
        //            timesheetData.EmployeeId = timesheet.EmployeeId;
        //            timesheetData.CreatedDate = DateTime.Now;
        //            timesheetData.IsActive = true;
        //            timesheetData.IsDeleted = false;

        //            _db.Timesheet.Add(timesheetData);
        //            _db.SaveChanges();
        //        }

        //        response.Message = "Timesheet Saved Successfully";
        //        response.StatusReason = true;
        //        return Ok(response);

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="Id"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("GetTimesheet")]
        //[Authorize]
        //public IHttpActionResult GetTimesheet(int Id)
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        Base response = new Base();
        //        // List<GetTimesheetData> employeeDataList = new List<GetTimesheetData>();
        //        var timesheetData = (from ad in _db.Timesheet
        //                             join bd in _db.Project on ad.Project equals bd.ProjectId
        //                             join cd in _db.Employee on ad.EmployeeId equals cd.EmployeeId
        //                             select new
        //                             {
        //                                 ad.TimesheetId,
        //                                 ad.Task,
        //                                 ad.Project,
        //                                 ad.Date,
        //                                 ad.Time,
        //                                 ad.Description,
        //                                 ad.CreatedDate,
        //                                 ad.IsDeleted,
        //                                 ad.IsActive,
        //                                 ad.EmployeeId,
        //                                 bd.ProjectName,
        //                                 name = cd.FirstName + " " + cd.LastName
        //                             }).ToList();
        //        foreach (var item in timesheetData)
        //        {
        //            timesheetData data = new timesheetData();
        //            data.TimesheetId = item.TimesheetId;
        //            data.Project = item.Project;
        //            data.Task = item.Task;
        //            data.Time = item.Time;
        //            data.Date = item.Date;
        //            data.Description = item.Description;
        //            data.CreatedDate = item.CreatedDate;
        //            data.IsDeleted = item.IsDeleted;
        //            data.IsActive = item.IsActive;
        //            data.EmployeeId = item.EmployeeId;
        //            data.ProjectName = item.ProjectName;
        //            data.EmployeeName = item.name;

        //            // timesheetDataList.Add(data);
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPut]
        //[Route("UpdateTimesheet")]
        //[Authorize]
        //public IHttpActionResult UpdateTimesheet(Timesheet timesheet)// we are using contact table as a employee
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        Base response = new Base();
        //        Timesheet timesheetData = new Timesheet();
        //        timesheetData.Project = timesheet.Project;
        //        timesheetData.Task = timesheet.Task;
        //        timesheetData.Date = timesheet.Date;
        //        timesheetData.Time = timesheet.Time;
        //        timesheetData.Description = timesheet.Description;
        //        timesheetData.EmployeeId = timesheet.EmployeeId;
        //        timesheetData.CreatedDate = DateTime.Now;
        //        timesheetData.IsActive = true;
        //        timesheetData.IsDeleted = false;
        //        _db.Timesheet.Add(timesheetData);
        //        _db.SaveChanges();

        //        response.Message = "Timesheet updated Successfully";
        //        response.StatusReason = true;
        //        return Ok(response);

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpDelete]
        //[Route("DeleteTimesheet")]
        //[Authorize]
        //public IHttpActionResult DeleteTimesheet(int TimesheetId)// we are using contact table as a employee
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        Base response = new Base();
        //        if (TimesheetId != null)
        //        {
        //            var TimesheetDelete = _db.Timesheet.Where(x => x.TimesheetId == TimesheetId).FirstOrDefault();
        //            TimesheetDelete.IsActive = true;
        //            TimesheetDelete.IsDeleted = false;
        //            _db.SaveChanges();
        //            response.StatusReason = true;
        //            response.Message = "Record Delete Successfully";
        //        }
        //        else
        //        {
        //            response.StatusReason = false;
        //            response.Message = "Data Not Found";
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[Route("CreateAdvaceFilter")]
        //[HttpPost]
        //[Authorize]
        //public IHttpActionResult CreateAdvanceFilter(AdvanceFilter AdvanceFilter)
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        //
        //        if (AdvanceFilter.Department.Count == 0 && AdvanceFilter.Employee_Type == 0 && AdvanceFilter.Company == 0 && (AdvanceFilter.Startjoiningdate == null && AdvanceFilter.Endjoiningdate == null))
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();

        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join ccd in _db.Company on ad.CompanyId equals ccd.CompanyId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                orderby ad.JoiningDate ascending
        //                                where ad.IsDeleted == false
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    cd.EmployeeTypes,
        //                                    ad.CompanyName
        //                                }).ToList();

        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DOB = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }

        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }
        //        else if (AdvanceFilter.Department.Count != 0 && AdvanceFilter.Employee_Type != 0 && AdvanceFilter.Company != 0 && (AdvanceFilter.Startjoiningdate != null && AdvanceFilter.Endjoiningdate != null))
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            foreach (var item1 in AdvanceFilter.Department)
        //            {
        //                var employeeData = (from ad in _db.Employee
        //                                    join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                    join ccd in _db.Company on ad.CompanyId equals ccd.CompanyId
        //                                    join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                    //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                    orderby ad.JoiningDate ascending
        //                                    where ad.RoleId == item1 && ad.EmployeeTypeId == AdvanceFilter.Employee_Type && ad.CompanyId == AdvanceFilter.Company && ad.JoiningDate >= AdvanceFilter.Startjoiningdate && ad.JoiningDate <= AdvanceFilter.Endjoiningdate && ad.IsDeleted == false
        //                                    select new
        //                                    {
        //                                        ad.EmployeeId,
        //                                        n = ad.FirstName + " " + ad.LastName,
        //                                        ad.PrimaryContact,
        //                                        ad.PersonalEmail,
        //                                        bd.RoleType,
        //                                        ad.FirstName,
        //                                        ad.LastName,
        //                                        ad.MaritalStatus,
        //                                        ad.SpouseName,
        //                                        ad.FatherName,
        //                                        ad.MotherName,
        //                                        //fd.BloodGroupType,
        //                                        ad.JoiningDate,
        //                                        ad.ConfirmationDate,
        //                                        ad.DateOfBirth,
        //                                        ad.EmergencyNumber,
        //                                        ad.WhatsappNumber,
        //                                        ad.AadharNumber,
        //                                        ad.PanNumber,
        //                                        ad.PermanentAddress,
        //                                        ad.LocalAddress,
        //                                        ad.MedicalIssue,
        //                                        ad.Salary,
        //                                        ad.BankAccountNumber,
        //                                        ad.IFSC,
        //                                        ad.AccountHolderName,
        //                                        ad.BankName,
        //                                        ad.OfficeEmail,
        //                                        cd.EmployeeTypes,
        //                                        ad.CompanyName
        //                                    }).ToList();
        //                foreach (var item in employeeData)
        //                {
        //                    EmployeeData data = new EmployeeData();
        //                    data.EmployeeId = item.EmployeeId;
        //                    data.FullName = item.n;
        //                    data.PrimaryContact = item.PrimaryContact;
        //                    data.Email = item.PersonalEmail;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.MaritalStatus = item.MaritalStatus;
        //                    data.SpouseName = item.SpouseName;
        //                    data.FatherName = item.FatherName;
        //                    data.MotherName = item.MotherName;
        //                    //data.BloodGroup = item.BloodGroupType;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.JoiningDate = item.JoiningDate;
        //                    data.ConfirmationDate = item.ConfirmationDate;
        //                    data.DOB = item.DateOfBirth;
        //                    data.EmergencyNumber = item.EmergencyNumber;
        //                    data.WhatsappNumber = item.WhatsappNumber;
        //                    data.AadharNumber = item.AadharNumber;
        //                    data.PanNumber = item.PanNumber;
        //                    data.PermanentAddress = item.PermanentAddress;
        //                    data.LocalAddress = item.LocalAddress;
        //                    data.MedicalIssue = item.MedicalIssue;
        //                    data.Salary = item.Salary;
        //                    data.BankAccountNumber = item.BankAccountNumber;
        //                    data.IFSC = item.IFSC;
        //                    data.AccountHolderName = item.AccountHolderName;
        //                    data.BankName = item.BankName;
        //                    data.OfficeEmail = item.OfficeEmail;
        //                    data.EmployeeType = item.EmployeeTypes;
        //                    data.CompanyName = item.CompanyName;
        //                    employeeDataList.Add(data);
        //                }
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }
        //        else if (AdvanceFilter.Department.Count != 0 && AdvanceFilter.Employee_Type != 0 && AdvanceFilter.Company != 0 && (AdvanceFilter.Startjoiningdate != null && AdvanceFilter.Endjoiningdate != null))
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            foreach (var item1 in AdvanceFilter.Department)
        //            {
        //                var employeeData = (from ad in _db.Employee
        //                                    join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                    join ccd in _db.Company on ad.CompanyId equals ccd.CompanyId
        //                                    join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                    //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                    orderby ad.JoiningDate ascending
        //                                    where ad.RoleId == item1 && ad.EmployeeTypeId == AdvanceFilter.Employee_Type && ad.CompanyId == AdvanceFilter.Company && ad.JoiningDate >= AdvanceFilter.Startjoiningdate && ad.JoiningDate <= AdvanceFilter.Endjoiningdate && ad.IsDeleted == false
        //                                    select new
        //                                    {
        //                                        ad.EmployeeId,
        //                                        n = ad.FirstName + " " + ad.LastName,
        //                                        ad.PrimaryContact,
        //                                        ad.PersonalEmail,
        //                                        bd.RoleType,
        //                                        ad.FirstName,
        //                                        ad.LastName,
        //                                        ad.MaritalStatus,
        //                                        ad.SpouseName,
        //                                        ad.FatherName,
        //                                        ad.MotherName,
        //                                        //fd.BloodGroupType,
        //                                        ad.JoiningDate,
        //                                        ad.ConfirmationDate,
        //                                        ad.DateOfBirth,
        //                                        ad.EmergencyNumber,
        //                                        ad.WhatsappNumber,
        //                                        ad.AadharNumber,
        //                                        ad.PanNumber,
        //                                        ad.PermanentAddress,
        //                                        ad.LocalAddress,
        //                                        ad.MedicalIssue,
        //                                        ad.Salary,
        //                                        ad.BankAccountNumber,
        //                                        ad.IFSC,
        //                                        ad.AccountHolderName,
        //                                        ad.BankName,
        //                                        ad.OfficeEmail,
        //                                        cd.EmployeeTypes,
        //                                        ad.CompanyName
        //                                    }).ToList();
        //                foreach (var item in employeeData)
        //                {
        //                    EmployeeData data = new EmployeeData();
        //                    data.EmployeeId = item.EmployeeId;
        //                    data.FullName = item.n;
        //                    data.PrimaryContact = item.PrimaryContact;
        //                    data.Email = item.PersonalEmail;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.MaritalStatus = item.MaritalStatus;
        //                    data.SpouseName = item.SpouseName;
        //                    data.FatherName = item.FatherName;
        //                    data.MotherName = item.MotherName;
        //                    //data.BloodGroup = item.BloodGroupType;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.JoiningDate = item.JoiningDate;
        //                    data.ConfirmationDate = item.ConfirmationDate;
        //                    data.DOB = item.DateOfBirth;
        //                    data.EmergencyNumber = item.EmergencyNumber;
        //                    data.WhatsappNumber = item.WhatsappNumber;
        //                    data.AadharNumber = item.AadharNumber;
        //                    data.PanNumber = item.PanNumber;
        //                    data.PermanentAddress = item.PermanentAddress;
        //                    data.LocalAddress = item.LocalAddress;
        //                    data.MedicalIssue = item.MedicalIssue;
        //                    data.Salary = item.Salary;
        //                    data.BankAccountNumber = item.BankAccountNumber;
        //                    data.IFSC = item.IFSC;
        //                    data.AccountHolderName = item.AccountHolderName;
        //                    data.BankName = item.BankName;
        //                    data.OfficeEmail = item.OfficeEmail;
        //                    data.EmployeeType = item.EmployeeTypes;
        //                    data.CompanyName = item.CompanyName;
        //                    employeeDataList.Add(data);
        //                }
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }

        //        else if (AdvanceFilter.Department.Count != 0 && AdvanceFilter.Company != 0 && (AdvanceFilter.Startjoiningdate != null && AdvanceFilter.Endjoiningdate != null))
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            foreach (var item1 in AdvanceFilter.Department)
        //            {
        //                var employeeData = (from ad in _db.Employee
        //                                    join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                    join ccd in _db.Company on ad.CompanyId equals ccd.CompanyId
        //                                    join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                    //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                    orderby ad.JoiningDate ascending
        //                                    where ad.RoleId == item1 && ad.CompanyId == AdvanceFilter.Company && ad.JoiningDate >= AdvanceFilter.Startjoiningdate && ad.JoiningDate <= AdvanceFilter.Endjoiningdate && ad.IsDeleted == false
        //                                    select new
        //                                    {
        //                                        ad.EmployeeId,
        //                                        n = ad.FirstName + " " + ad.LastName,
        //                                        ad.PrimaryContact,
        //                                        ad.PersonalEmail,
        //                                        bd.RoleType,
        //                                        ad.FirstName,
        //                                        ad.LastName,
        //                                        ad.MaritalStatus,
        //                                        ad.SpouseName,
        //                                        ad.FatherName,
        //                                        ad.MotherName,
        //                                        //fd.BloodGroupType,
        //                                        ad.JoiningDate,
        //                                        ad.ConfirmationDate,
        //                                        ad.DateOfBirth,
        //                                        ad.EmergencyNumber,
        //                                        ad.WhatsappNumber,
        //                                        ad.AadharNumber,
        //                                        ad.PanNumber,
        //                                        ad.PermanentAddress,
        //                                        ad.LocalAddress,
        //                                        ad.MedicalIssue,
        //                                        ad.Salary,
        //                                        ad.BankAccountNumber,
        //                                        ad.IFSC,
        //                                        ad.AccountHolderName,
        //                                        ad.BankName,
        //                                        ad.OfficeEmail,
        //                                        cd.EmployeeTypes,
        //                                        ad.CompanyName
        //                                    }).ToList();
        //                foreach (var item in employeeData)
        //                {
        //                    EmployeeData data = new EmployeeData();
        //                    data.EmployeeId = item.EmployeeId;
        //                    data.FullName = item.n;
        //                    data.PrimaryContact = item.PrimaryContact;
        //                    data.Email = item.PersonalEmail;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.MaritalStatus = item.MaritalStatus;
        //                    data.SpouseName = item.SpouseName;
        //                    data.FatherName = item.FatherName;
        //                    data.MotherName = item.MotherName;
        //                    //data.BloodGroup = item.BloodGroupType;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.JoiningDate = item.JoiningDate;
        //                    data.ConfirmationDate = item.ConfirmationDate;
        //                    data.DOB = item.DateOfBirth;
        //                    data.EmergencyNumber = item.EmergencyNumber;
        //                    data.WhatsappNumber = item.WhatsappNumber;
        //                    data.AadharNumber = item.AadharNumber;
        //                    data.PanNumber = item.PanNumber;
        //                    data.PermanentAddress = item.PermanentAddress;
        //                    data.LocalAddress = item.LocalAddress;
        //                    data.MedicalIssue = item.MedicalIssue;
        //                    data.Salary = item.Salary;
        //                    data.BankAccountNumber = item.BankAccountNumber;
        //                    data.IFSC = item.IFSC;
        //                    data.AccountHolderName = item.AccountHolderName;
        //                    data.BankName = item.BankName;
        //                    data.OfficeEmail = item.OfficeEmail;
        //                    data.EmployeeType = item.EmployeeTypes;
        //                    data.CompanyName = item.CompanyName;
        //                    employeeDataList.Add(data);
        //                }
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }

        //        else if (AdvanceFilter.Department.Count != 0 && AdvanceFilter.Employee_Type != 0 && (AdvanceFilter.Startjoiningdate != null && AdvanceFilter.Endjoiningdate != null))
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            foreach (var item1 in AdvanceFilter.Department)
        //            {
        //                var employeeData = (from ad in _db.Employee
        //                                    join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                    join ccd in _db.Company on ad.CompanyId equals ccd.CompanyId
        //                                    join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                    //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                    orderby ad.JoiningDate ascending
        //                                    where ad.RoleId == item1 && ad.EmployeeTypeId == AdvanceFilter.Employee_Type && ad.JoiningDate >= AdvanceFilter.Startjoiningdate && ad.JoiningDate <= AdvanceFilter.Endjoiningdate && ad.IsDeleted == false
        //                                    select new
        //                                    {
        //                                        ad.EmployeeId,
        //                                        n = ad.FirstName + " " + ad.LastName,
        //                                        ad.PrimaryContact,
        //                                        ad.PersonalEmail,
        //                                        bd.RoleType,
        //                                        ad.FirstName,
        //                                        ad.LastName,
        //                                        ad.MaritalStatus,
        //                                        ad.SpouseName,
        //                                        ad.FatherName,
        //                                        ad.MotherName,
        //                                        //fd.BloodGroupType,
        //                                        ad.JoiningDate,
        //                                        ad.ConfirmationDate,
        //                                        ad.DateOfBirth,
        //                                        ad.EmergencyNumber,
        //                                        ad.WhatsappNumber,
        //                                        ad.AadharNumber,
        //                                        ad.PanNumber,
        //                                        ad.PermanentAddress,
        //                                        ad.LocalAddress,
        //                                        ad.MedicalIssue,
        //                                        ad.Salary,
        //                                        ad.BankAccountNumber,
        //                                        ad.IFSC,
        //                                        ad.AccountHolderName,
        //                                        ad.BankName,
        //                                        ad.OfficeEmail,
        //                                        cd.EmployeeTypes,
        //                                        ad.CompanyName
        //                                    }).ToList();
        //                foreach (var item in employeeData)
        //                {
        //                    EmployeeData data = new EmployeeData();
        //                    data.EmployeeId = item.EmployeeId;
        //                    data.FullName = item.n;
        //                    data.PrimaryContact = item.PrimaryContact;
        //                    data.Email = item.PersonalEmail;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.MaritalStatus = item.MaritalStatus;
        //                    data.SpouseName = item.SpouseName;
        //                    data.FatherName = item.FatherName;
        //                    data.MotherName = item.MotherName;
        //                    //data.BloodGroup = item.BloodGroupType;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.JoiningDate = item.JoiningDate;
        //                    data.ConfirmationDate = item.ConfirmationDate;
        //                    data.DOB = item.DateOfBirth;
        //                    data.EmergencyNumber = item.EmergencyNumber;
        //                    data.WhatsappNumber = item.WhatsappNumber;
        //                    data.AadharNumber = item.AadharNumber;
        //                    data.PanNumber = item.PanNumber;
        //                    data.PermanentAddress = item.PermanentAddress;
        //                    data.LocalAddress = item.LocalAddress;
        //                    data.MedicalIssue = item.MedicalIssue;
        //                    data.Salary = item.Salary;
        //                    data.BankAccountNumber = item.BankAccountNumber;
        //                    data.IFSC = item.IFSC;
        //                    data.AccountHolderName = item.AccountHolderName;
        //                    data.BankName = item.BankName;
        //                    data.OfficeEmail = item.OfficeEmail;
        //                    data.EmployeeType = item.EmployeeTypes;
        //                    data.CompanyName = item.CompanyName;
        //                    employeeDataList.Add(data);
        //                }
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }
        //        else if (AdvanceFilter.Department.Count != 0 && AdvanceFilter.Employee_Type != 0 && AdvanceFilter.Company != 0)
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            foreach (var item1 in AdvanceFilter.Department)
        //            {
        //                var employeeData = (from ad in _db.Employee
        //                                    join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                    join ccd in _db.Company on ad.CompanyId equals ccd.CompanyId
        //                                    join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                    //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                    orderby ad.JoiningDate ascending
        //                                    where ad.RoleId == item1 && ad.EmployeeTypeId == AdvanceFilter.Employee_Type && ad.CompanyId == AdvanceFilter.Company && ad.IsDeleted == false
        //                                    select new
        //                                    {
        //                                        ad.EmployeeId,
        //                                        n = ad.FirstName + " " + ad.LastName,
        //                                        ad.PrimaryContact,
        //                                        ad.PersonalEmail,
        //                                        bd.RoleType,
        //                                        ad.FirstName,
        //                                        ad.LastName,
        //                                        ad.MaritalStatus,
        //                                        ad.SpouseName,
        //                                        ad.FatherName,
        //                                        ad.MotherName,
        //                                        //fd.BloodGroupType,
        //                                        ad.JoiningDate,
        //                                        ad.ConfirmationDate,
        //                                        ad.DateOfBirth,
        //                                        ad.EmergencyNumber,
        //                                        ad.WhatsappNumber,
        //                                        ad.AadharNumber,
        //                                        ad.PanNumber,
        //                                        ad.PermanentAddress,
        //                                        ad.LocalAddress,
        //                                        ad.MedicalIssue,
        //                                        ad.Salary,
        //                                        ad.BankAccountNumber,
        //                                        ad.IFSC,
        //                                        ad.AccountHolderName,
        //                                        ad.BankName,
        //                                        ad.OfficeEmail,
        //                                        cd.EmployeeTypes,
        //                                        ad.CompanyName
        //                                    }).ToList();
        //                foreach (var item in employeeData)
        //                {
        //                    EmployeeData data = new EmployeeData();
        //                    data.EmployeeId = item.EmployeeId;
        //                    data.FullName = item.n;
        //                    data.PrimaryContact = item.PrimaryContact;
        //                    data.Email = item.PersonalEmail;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.MaritalStatus = item.MaritalStatus;
        //                    data.SpouseName = item.SpouseName;
        //                    data.FatherName = item.FatherName;
        //                    data.MotherName = item.MotherName;
        //                    //data.BloodGroup = item.BloodGroupType;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.JoiningDate = item.JoiningDate;
        //                    data.ConfirmationDate = item.ConfirmationDate;
        //                    data.DOB = item.DateOfBirth;
        //                    data.EmergencyNumber = item.EmergencyNumber;
        //                    data.WhatsappNumber = item.WhatsappNumber;
        //                    data.AadharNumber = item.AadharNumber;
        //                    data.PanNumber = item.PanNumber;
        //                    data.PermanentAddress = item.PermanentAddress;
        //                    data.LocalAddress = item.LocalAddress;
        //                    data.MedicalIssue = item.MedicalIssue;
        //                    data.Salary = item.Salary;
        //                    data.BankAccountNumber = item.BankAccountNumber;
        //                    data.IFSC = item.IFSC;
        //                    data.AccountHolderName = item.AccountHolderName;
        //                    data.BankName = item.BankName;
        //                    data.OfficeEmail = item.OfficeEmail;
        //                    data.EmployeeType = item.EmployeeTypes;
        //                    data.CompanyName = item.CompanyName;
        //                    employeeDataList.Add(data);
        //                }
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }
        //        else if (AdvanceFilter.Employee_Type != 0 && AdvanceFilter.Company != 0 && (AdvanceFilter.Startjoiningdate != null && AdvanceFilter.Endjoiningdate != null))
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join ccd in _db.Company on ad.CompanyId equals ccd.CompanyId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                orderby ad.JoiningDate ascending
        //                                where ad.EmployeeTypeId == AdvanceFilter.Employee_Type && ad.CompanyId == AdvanceFilter.Company && ad.JoiningDate >= AdvanceFilter.Startjoiningdate && ad.JoiningDate <= AdvanceFilter.Endjoiningdate && ad.IsDeleted == false
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    cd.EmployeeTypes,
        //                                    ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DOB = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }
        //        else if (AdvanceFilter.Employee_Type != 0 && AdvanceFilter.Company != 0 && (AdvanceFilter.Startjoiningdate != null && AdvanceFilter.Endjoiningdate != null))
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join ccd in _db.Company on ad.CompanyId equals ccd.CompanyId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                orderby ad.JoiningDate ascending
        //                                where ad.EmployeeTypeId == AdvanceFilter.Employee_Type && ad.CompanyId == AdvanceFilter.Company && ad.JoiningDate >= AdvanceFilter.Startjoiningdate && ad.JoiningDate <= AdvanceFilter.Endjoiningdate && ad.IsDeleted == false
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    cd.EmployeeTypes,
        //                                    ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DOB = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }

        //        else if (AdvanceFilter.Employee_Type != 0 && (AdvanceFilter.Startjoiningdate != null && AdvanceFilter.Endjoiningdate != null))
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                orderby ad.JoiningDate ascending
        //                                where ad.EmployeeTypeId == AdvanceFilter.Employee_Type && ad.JoiningDate >= AdvanceFilter.Startjoiningdate && ad.JoiningDate <= AdvanceFilter.Endjoiningdate && ad.IsDeleted == false
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    cd.EmployeeTypes,
        //                                    ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DateOfBirth = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }

        //        else if (AdvanceFilter.Employee_Type != 0 && AdvanceFilter.Company != 0)
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join ccd in _db.Company on ad.CompanyId equals ccd.CompanyId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroup equals fd.BloodGroup
        //                                orderby ad.JoiningDate ascending
        //                                where ad.EmployeeTypeId == AdvanceFilter.Employee_Type && ad.CompanyId == AdvanceFilter.Company && ad.IsDeleted == false
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    cd.EmployeeTypes,
        //                                    ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DOB = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }
        //        else if (AdvanceFilter.Department.Count != 0 && AdvanceFilter.Employee_Type != 0 && AdvanceFilter.Company != 0)
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            foreach (var item1 in AdvanceFilter.Department)
        //            {
        //                var employeeData = (from ad in _db.Employee
        //                                    join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                    join ccd in _db.Company on ad.CompanyId equals ccd.CompanyId
        //                                    join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                    //join fd in db.BloodGroup on ad.BloodGroup equals fd.BloodGroup
        //                                    orderby ad.JoiningDate ascending
        //                                    where ad.RoleId == item1 && ad.EmployeeTypeId == AdvanceFilter.Employee_Type && ad.CompanyId == AdvanceFilter.Company && ad.IsDeleted == false
        //                                    select new
        //                                    {
        //                                        ad.EmployeeId,
        //                                        n = ad.FirstName + " " + ad.LastName,
        //                                        ad.PrimaryContact,
        //                                        ad.PersonalEmail,
        //                                        bd.RoleType,
        //                                        ad.FirstName,
        //                                        ad.LastName,
        //                                        ad.MaritalStatus,
        //                                        ad.SpouseName,
        //                                        ad.FatherName,
        //                                        ad.MotherName,
        //                                        //fd.BloodGroupType,
        //                                        ad.JoiningDate,
        //                                        ad.ConfirmationDate,
        //                                        ad.DateOfBirth,
        //                                        ad.EmergencyNumber,
        //                                        ad.WhatsappNumber,
        //                                        ad.AadharNumber,
        //                                        ad.PanNumber,
        //                                        ad.PermanentAddress,
        //                                        ad.LocalAddress,
        //                                        ad.MedicalIssue,
        //                                        ad.Salary,
        //                                        ad.BankAccountNumber,
        //                                        ad.IFSC,
        //                                        ad.AccountHolderName,
        //                                        ad.BankName,
        //                                        ad.OfficeEmail,
        //                                        cd.EmployeeTypes,
        //                                        ad.CompanyName
        //                                    }).ToList();
        //                foreach (var item in employeeData)
        //                {
        //                    EmployeeData data = new EmployeeData();
        //                    data.EmployeeId = item.EmployeeId;
        //                    data.FullName = item.n;
        //                    data.PrimaryContact = item.PrimaryContact;
        //                    data.Email = item.PersonalEmail;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.MaritalStatus = item.MaritalStatus;
        //                    data.SpouseName = item.SpouseName;
        //                    data.FatherName = item.FatherName;
        //                    data.MotherName = item.MotherName;
        //                    //data.BloodGroup = item.BloodGroupType;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.JoiningDate = item.JoiningDate;
        //                    data.ConfirmationDate = item.ConfirmationDate;
        //                    data.DOB = item.DateOfBirth;
        //                    data.EmergencyNumber = item.EmergencyNumber;
        //                    data.WhatsappNumber = item.WhatsappNumber;
        //                    data.AadharNumber = item.AadharNumber;
        //                    data.PanNumber = item.PanNumber;
        //                    data.PermanentAddress = item.PermanentAddress;
        //                    data.LocalAddress = item.LocalAddress;
        //                    data.MedicalIssue = item.MedicalIssue;
        //                    data.Salary = item.Salary;
        //                    data.BankAccountNumber = item.BankAccountNumber;
        //                    data.IFSC = item.IFSC;
        //                    data.AccountHolderName = item.AccountHolderName;
        //                    data.BankName = item.BankName;
        //                    data.OfficeEmail = item.OfficeEmail;
        //                    data.EmployeeType = item.EmployeeTypes;
        //                    data.CompanyName = item.CompanyName;
        //                    employeeDataList.Add(data);
        //                }
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }

        //        else if (AdvanceFilter.Department.Count != 0 && AdvanceFilter.Company != 0)
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            foreach (var item1 in AdvanceFilter.Department)
        //            {
        //                var employeeData = (from ad in _db.Employee
        //                                    join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                    join ccd in _db.Company on ad.CompanyId equals ccd.CompanyId
        //                                    join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                    //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                    orderby ad.JoiningDate ascending
        //                                    where ad.RoleId == item1 && ad.CompanyId == AdvanceFilter.Company && ad.IsDeleted == false
        //                                    select new
        //                                    {
        //                                        ad.EmployeeId,
        //                                        n = ad.FirstName + " " + ad.LastName,
        //                                        ad.PrimaryContact,
        //                                        ad.PersonalEmail,
        //                                        bd.RoleType,
        //                                        ad.FirstName,
        //                                        ad.LastName,
        //                                        ad.MaritalStatus,
        //                                        ad.SpouseName,
        //                                        ad.FatherName,
        //                                        ad.MotherName,
        //                                        //fd.BloodGroupType,
        //                                        ad.JoiningDate,
        //                                        ad.ConfirmationDate,
        //                                        ad.DateOfBirth,
        //                                        ad.EmergencyNumber,
        //                                        ad.WhatsappNumber,
        //                                        ad.AadharNumber,
        //                                        ad.PanNumber,
        //                                        ad.PermanentAddress,
        //                                        ad.LocalAddress,
        //                                        ad.MedicalIssue,
        //                                        ad.Salary,
        //                                        ad.BankAccountNumber,
        //                                        ad.IFSC,
        //                                        ad.AccountHolderName,
        //                                        ad.BankName,
        //                                        ad.OfficeEmail,
        //                                        cd.EmployeeTypes,
        //                                        ad.CompanyName
        //                                    }).ToList();
        //                foreach (var item in employeeData)
        //                {
        //                    EmployeeData data = new EmployeeData();
        //                    data.EmployeeId = item.EmployeeId;
        //                    data.FullName = item.n;
        //                    data.PrimaryContact = item.PrimaryContact;
        //                    data.Email = item.PersonalEmail;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.MaritalStatus = item.MaritalStatus;
        //                    data.SpouseName = item.SpouseName;
        //                    data.FatherName = item.FatherName;
        //                    data.MotherName = item.MotherName;
        //                    //data.BloodGroup = item.BloodGroupType;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.JoiningDate = item.JoiningDate;
        //                    data.ConfirmationDate = item.ConfirmationDate;
        //                    data.DOB = item.DateOfBirth;
        //                    data.EmergencyNumber = item.EmergencyNumber;
        //                    data.WhatsappNumber = item.WhatsappNumber;
        //                    data.AadharNumber = item.AadharNumber;
        //                    data.PanNumber = item.PanNumber;
        //                    data.PermanentAddress = item.PermanentAddress;
        //                    data.LocalAddress = item.LocalAddress;
        //                    data.MedicalIssue = item.MedicalIssue;
        //                    data.Salary = item.Salary;
        //                    data.BankAccountNumber = item.BankAccountNumber;
        //                    data.IFSC = item.IFSC;
        //                    data.AccountHolderName = item.AccountHolderName;
        //                    data.BankName = item.BankName;
        //                    data.OfficeEmail = item.OfficeEmail;
        //                    data.EmployeeType = item.EmployeeTypes;
        //                    data.CompanyName = item.CompanyName;
        //                    employeeDataList.Add(data);
        //                }
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }

        //        else if (AdvanceFilter.Department.Count != 0 && (AdvanceFilter.Startjoiningdate != null && AdvanceFilter.Endjoiningdate != null))
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            foreach (var item1 in AdvanceFilter.Department)
        //            {
        //                var employeeData = (from ad in _db.Employee
        //                                    join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                    join ccd in _db.Company on ad.CompanyId equals ccd.CompanyId
        //                                    join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                    //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                    orderby ad.JoiningDate ascending
        //                                    where ad.RoleId == item1 && ad.JoiningDate >= AdvanceFilter.Startjoiningdate && ad.JoiningDate <= AdvanceFilter.Endjoiningdate && ad.IsDeleted == false
        //                                    select new
        //                                    {
        //                                        ad.EmployeeId,
        //                                        n = ad.FirstName + " " + ad.LastName,
        //                                        ad.PrimaryContact,
        //                                        ad.PersonalEmail,
        //                                        bd.RoleType,
        //                                        ad.FirstName,
        //                                        ad.LastName,
        //                                        ad.MaritalStatus,
        //                                        ad.SpouseName,
        //                                        ad.FatherName,
        //                                        ad.MotherName,
        //                                        //fd.BloodGroupType,
        //                                        ad.JoiningDate,
        //                                        ad.ConfirmationDate,
        //                                        ad.DateOfBirth,
        //                                        ad.EmergencyNumber,
        //                                        ad.WhatsappNumber,
        //                                        ad.AadharNumber,
        //                                        ad.PanNumber,
        //                                        ad.PermanentAddress,
        //                                        ad.LocalAddress,
        //                                        ad.MedicalIssue,
        //                                        ad.Salary,
        //                                        ad.BankAccountNumber,
        //                                        ad.IFSC,
        //                                        ad.AccountHolderName,
        //                                        ad.BankName,
        //                                        ad.OfficeEmail,
        //                                        cd.EmployeeTypes,
        //                                        ad.CompanyName
        //                                    }).ToList();
        //                foreach (var item in employeeData)
        //                {
        //                    EmployeeData data = new EmployeeData();
        //                    data.EmployeeId = item.EmployeeId;
        //                    data.FullName = item.n;
        //                    data.PrimaryContact = item.PrimaryContact;
        //                    data.Email = item.PersonalEmail;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.MaritalStatus = item.MaritalStatus;
        //                    data.SpouseName = item.SpouseName;
        //                    data.FatherName = item.FatherName;
        //                    data.MotherName = item.MotherName;
        //                    //data.BloodGroup = item.BloodGroupType;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.JoiningDate = item.JoiningDate;
        //                    data.ConfirmationDate = item.ConfirmationDate;
        //                    data.DOB = item.DateOfBirth;
        //                    data.EmergencyNumber = item.EmergencyNumber;
        //                    data.WhatsappNumber = item.WhatsappNumber;
        //                    data.AadharNumber = item.AadharNumber;
        //                    data.PanNumber = item.PanNumber;
        //                    data.PermanentAddress = item.PermanentAddress;
        //                    data.LocalAddress = item.LocalAddress;
        //                    data.MedicalIssue = item.MedicalIssue;
        //                    data.Salary = item.Salary;
        //                    data.BankAccountNumber = item.BankAccountNumber;
        //                    data.IFSC = item.IFSC;
        //                    data.AccountHolderName = item.AccountHolderName;
        //                    data.BankName = item.BankName;
        //                    data.OfficeEmail = item.OfficeEmail;
        //                    data.EmployeeType = item.EmployeeTypes;
        //                    data.CompanyName = item.CompanyName;
        //                    employeeDataList.Add(data);
        //                }
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }

        //        else if (AdvanceFilter.Department.Count != 0 && (AdvanceFilter.Startjoiningdate != null && AdvanceFilter.Endjoiningdate != null) && AdvanceFilter.Employee_Type != 0)
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            foreach (var item1 in AdvanceFilter.Department)
        //            {
        //                var employeeData = (from ad in _db.Employee
        //                                    join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                    join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                    //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                    orderby ad.JoiningDate ascending
        //                                    where ad.RoleId == item1 && ad.EmployeeTypeId == AdvanceFilter.Employee_Type && ad.JoiningDate >= AdvanceFilter.Startjoiningdate && ad.JoiningDate <= AdvanceFilter.Endjoiningdate && ad.IsDeleted == false
        //                                    select new
        //                                    {
        //                                        ad.EmployeeId,
        //                                        n = ad.FirstName + " " + ad.LastName,
        //                                        ad.PrimaryContact,
        //                                        ad.PersonalEmail,
        //                                        bd.RoleType,
        //                                        ad.FirstName,
        //                                        ad.LastName,
        //                                        ad.MaritalStatus,
        //                                        ad.SpouseName,
        //                                        ad.FatherName,
        //                                        ad.MotherName,
        //                                        //fd.BloodGroupType,
        //                                        ad.JoiningDate,
        //                                        ad.ConfirmationDate,
        //                                        ad.DateOfBirth,
        //                                        ad.EmergencyNumber,
        //                                        ad.WhatsappNumber,
        //                                        ad.AadharNumber,
        //                                        ad.PanNumber,
        //                                        ad.PermanentAddress,
        //                                        ad.LocalAddress,
        //                                        ad.MedicalIssue,
        //                                        ad.Salary,
        //                                        ad.BankAccountNumber,
        //                                        ad.IFSC,
        //                                        ad.AccountHolderName,
        //                                        ad.BankName,
        //                                        ad.OfficeEmail,
        //                                        cd.EmployeeTypes,
        //                                        ad.CompanyName
        //                                    }).ToList();
        //                foreach (var item in employeeData)
        //                {
        //                    EmployeeData data = new EmployeeData();
        //                    data.EmployeeId = item.EmployeeId;
        //                    data.FullName = item.n;
        //                    data.PrimaryContact = item.PrimaryContact;
        //                    data.Email = item.PersonalEmail;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.MaritalStatus = item.MaritalStatus;
        //                    data.SpouseName = item.SpouseName;
        //                    data.FatherName = item.FatherName;
        //                    data.MotherName = item.MotherName;
        //                    //data.BloodGroup = item.BloodGroupType;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.JoiningDate = item.JoiningDate;
        //                    data.ConfirmationDate = item.ConfirmationDate;
        //                    data.DOB = item.DateOfBirth;
        //                    data.EmergencyNumber = item.EmergencyNumber;
        //                    data.WhatsappNumber = item.WhatsappNumber;
        //                    data.AadharNumber = item.AadharNumber;
        //                    data.PanNumber = item.PanNumber;
        //                    data.PermanentAddress = item.PermanentAddress;
        //                    data.LocalAddress = item.LocalAddress;
        //                    data.MedicalIssue = item.MedicalIssue;
        //                    data.Salary = item.Salary;
        //                    data.BankAccountNumber = item.BankAccountNumber;
        //                    data.IFSC = item.IFSC;
        //                    data.AccountHolderName = item.AccountHolderName;
        //                    data.BankName = item.BankName;
        //                    data.OfficeEmail = item.OfficeEmail;
        //                    data.EmployeeType = item.EmployeeTypes;
        //                    data.CompanyName = item.CompanyName;
        //                    employeeDataList.Add(data);
        //                }
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }

        //        else if (AdvanceFilter.Department.Count != 0 && (AdvanceFilter.Startjoiningdate != null && AdvanceFilter.Endjoiningdate != null) && AdvanceFilter.Company != 0)
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            foreach (var item1 in AdvanceFilter.Department)
        //            {
        //                var employeeData = (from ad in _db.Employee
        //                                    join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                    join ccd in _db.Company on ad.CompanyId equals ccd.CompanyId
        //                                    join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                    //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                    orderby ad.JoiningDate ascending
        //                                    where ad.RoleId == item1 && ad.CompanyId == AdvanceFilter.Company && ad.JoiningDate >= AdvanceFilter.Startjoiningdate && ad.JoiningDate <= AdvanceFilter.Endjoiningdate && ad.IsDeleted == false
        //                                    select new
        //                                    {
        //                                        ad.EmployeeId,
        //                                        n = ad.FirstName + " " + ad.LastName,
        //                                        ad.PrimaryContact,
        //                                        ad.PersonalEmail,
        //                                        bd.RoleType,
        //                                        ad.FirstName,
        //                                        ad.LastName,
        //                                        ad.MaritalStatus,
        //                                        ad.SpouseName,
        //                                        ad.FatherName,
        //                                        ad.MotherName,
        //                                        //fd.BloodGroupType,
        //                                        ad.JoiningDate,
        //                                        ad.ConfirmationDate,
        //                                        ad.DateOfBirth,
        //                                        ad.EmergencyNumber,
        //                                        ad.WhatsappNumber,
        //                                        ad.AadharNumber,
        //                                        ad.PanNumber,
        //                                        ad.PermanentAddress,
        //                                        ad.LocalAddress,
        //                                        ad.MedicalIssue,
        //                                        ad.Salary,
        //                                        ad.BankAccountNumber,
        //                                        ad.IFSC,
        //                                        ad.AccountHolderName,
        //                                        ad.BankName,
        //                                        ad.OfficeEmail,
        //                                        cd.EmployeeTypes,
        //                                        ad.CompanyName
        //                                    }).ToList();
        //                foreach (var item in employeeData)
        //                {
        //                    EmployeeData data = new EmployeeData();
        //                    data.EmployeeId = item.EmployeeId;
        //                    data.FullName = item.n;
        //                    data.PrimaryContact = item.PrimaryContact;
        //                    data.Email = item.PersonalEmail;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.MaritalStatus = item.MaritalStatus;
        //                    data.SpouseName = item.SpouseName;
        //                    data.FatherName = item.FatherName;
        //                    data.MotherName = item.MotherName;
        //                    //data.BloodGroup = item.BloodGroupType;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.JoiningDate = item.JoiningDate;
        //                    data.ConfirmationDate = item.ConfirmationDate;
        //                    data.DOB = item.DateOfBirth;
        //                    data.EmergencyNumber = item.EmergencyNumber;
        //                    data.WhatsappNumber = item.WhatsappNumber;
        //                    data.AadharNumber = item.AadharNumber;
        //                    data.PanNumber = item.PanNumber;
        //                    data.PermanentAddress = item.PermanentAddress;
        //                    data.LocalAddress = item.LocalAddress;
        //                    data.MedicalIssue = item.MedicalIssue;
        //                    data.Salary = item.Salary;
        //                    data.BankAccountNumber = item.BankAccountNumber;
        //                    data.IFSC = item.IFSC;
        //                    data.AccountHolderName = item.AccountHolderName;
        //                    data.BankName = item.BankName;
        //                    data.OfficeEmail = item.OfficeEmail;
        //                    data.EmployeeType = item.EmployeeTypes;
        //                    data.CompanyName = item.CompanyName;
        //                    employeeDataList.Add(data);
        //                }
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }

        //        else if (AdvanceFilter.Department.Count != 0 && AdvanceFilter.Employee_Type != 0)
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            foreach (var item1 in AdvanceFilter.Department)
        //            {
        //                var employeeData = (from ad in _db.Employee
        //                                    join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                    join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                    //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                    orderby ad.JoiningDate ascending
        //                                    where ad.RoleId == item1 && ad.EmployeeTypeId == AdvanceFilter.Employee_Type && ad.IsDeleted == false
        //                                    select new
        //                                    {
        //                                        ad.EmployeeId,
        //                                        n = ad.FirstName + " " + ad.LastName,
        //                                        ad.PrimaryContact,
        //                                        ad.PersonalEmail,
        //                                        bd.RoleType,
        //                                        ad.FirstName,
        //                                        ad.LastName,
        //                                        ad.MaritalStatus,
        //                                        ad.SpouseName,
        //                                        ad.FatherName,
        //                                        ad.MotherName,
        //                                        //fd.BloodGroupType,
        //                                        ad.JoiningDate,
        //                                        ad.ConfirmationDate,
        //                                        ad.DateOfBirth,
        //                                        ad.EmergencyNumber,
        //                                        ad.WhatsappNumber,
        //                                        ad.AadharNumber,
        //                                        ad.PanNumber,
        //                                        ad.PermanentAddress,
        //                                        ad.LocalAddress,
        //                                        ad.MedicalIssue,
        //                                        ad.Salary,
        //                                        ad.BankAccountNumber,
        //                                        ad.IFSC,
        //                                        ad.AccountHolderName,
        //                                        ad.BankName,
        //                                        ad.OfficeEmail,
        //                                        cd.EmployeeTypes,
        //                                        ad.CompanyName
        //                                    }).ToList();
        //                foreach (var item in employeeData)
        //                {
        //                    EmployeeData data = new EmployeeData();
        //                    data.EmployeeId = item.EmployeeId;
        //                    data.FullName = item.n;
        //                    data.PrimaryContact = item.PrimaryContact;
        //                    data.Email = item.PersonalEmail;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.MaritalStatus = item.MaritalStatus;
        //                    data.SpouseName = item.SpouseName;
        //                    data.FatherName = item.FatherName;
        //                    data.MotherName = item.MotherName;
        //                    //data.BloodGroup = item.BloodGroupType;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.JoiningDate = item.JoiningDate;
        //                    data.ConfirmationDate = item.ConfirmationDate;
        //                    data.DOB = item.DateOfBirth;
        //                    data.EmergencyNumber = item.EmergencyNumber;
        //                    data.WhatsappNumber = item.WhatsappNumber;
        //                    data.AadharNumber = item.AadharNumber;
        //                    data.PanNumber = item.PanNumber;
        //                    data.PermanentAddress = item.PermanentAddress;
        //                    data.LocalAddress = item.LocalAddress;
        //                    data.MedicalIssue = item.MedicalIssue;
        //                    data.Salary = item.Salary;
        //                    data.BankAccountNumber = item.BankAccountNumber;
        //                    data.IFSC = item.IFSC;
        //                    data.AccountHolderName = item.AccountHolderName;
        //                    data.BankName = item.BankName;
        //                    data.OfficeEmail = item.OfficeEmail;
        //                    data.EmployeeType = item.EmployeeTypes;
        //                    data.CompanyName = item.CompanyName;
        //                    employeeDataList.Add(data);
        //                }
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }
        //        else if (AdvanceFilter.Company != 0 && (AdvanceFilter.Startjoiningdate != null && AdvanceFilter.Endjoiningdate != null))
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join ccd in _db.Company on ad.CompanyId equals ccd.CompanyId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                orderby ad.JoiningDate ascending
        //                                where ad.CompanyId == AdvanceFilter.Company && ad.JoiningDate >= AdvanceFilter.Startjoiningdate && ad.JoiningDate <= AdvanceFilter.Endjoiningdate && ad.IsDeleted == false
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    cd.EmployeeTypes,
        //                                    ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DOB = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }

        //        else if (AdvanceFilter.Department.Count != 0 && AdvanceFilter.Employee_Type != 0)
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            foreach (var item1 in AdvanceFilter.Department)
        //            {
        //                var employeeData = (from ad in _db.Employee
        //                                    join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                    join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                    //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                    orderby ad.JoiningDate ascending
        //                                    where ad.RoleId == item1 && ad.EmployeeTypeId == AdvanceFilter.Employee_Type && ad.IsDeleted == false
        //                                    select new
        //                                    {
        //                                        ad.EmployeeId,
        //                                        n = ad.FirstName + " " + ad.LastName,
        //                                        ad.PrimaryContact,
        //                                        ad.PersonalEmail,
        //                                        bd.RoleType,
        //                                        ad.FirstName,
        //                                        ad.LastName,
        //                                        ad.MaritalStatus,
        //                                        ad.SpouseName,
        //                                        ad.FatherName,
        //                                        ad.MotherName,
        //                                        //fd.BloodGroupType,
        //                                        ad.JoiningDate,
        //                                        ad.ConfirmationDate,
        //                                        ad.DateOfBirth,
        //                                        ad.EmergencyNumber,
        //                                        ad.WhatsappNumber,
        //                                        ad.AadharNumber,
        //                                        ad.PanNumber,
        //                                        ad.PermanentAddress,
        //                                        ad.LocalAddress,
        //                                        ad.MedicalIssue,
        //                                        ad.Salary,
        //                                        ad.BankAccountNumber,
        //                                        ad.IFSC,
        //                                        ad.AccountHolderName,
        //                                        ad.BankName,
        //                                        ad.OfficeEmail,
        //                                        cd.EmployeeTypes,
        //                                        ad.CompanyName
        //                                    }).ToList();
        //                foreach (var item in employeeData)
        //                {
        //                    EmployeeData data = new EmployeeData();
        //                    data.EmployeeId = item.EmployeeId;
        //                    data.FullName = item.n;
        //                    data.PrimaryContact = item.PrimaryContact;
        //                    data.Email = item.PersonalEmail;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.MaritalStatus = item.MaritalStatus;
        //                    data.SpouseName = item.SpouseName;
        //                    data.FatherName = item.FatherName;
        //                    data.MotherName = item.MotherName;
        //                    //data.BloodGroup = item.BloodGroupType;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.JoiningDate = item.JoiningDate;
        //                    data.ConfirmationDate = item.ConfirmationDate;
        //                    data.DOB = item.DateOfBirth;
        //                    data.EmergencyNumber = item.EmergencyNumber;
        //                    data.WhatsappNumber = item.WhatsappNumber;
        //                    data.AadharNumber = item.AadharNumber;
        //                    data.PanNumber = item.PanNumber;
        //                    data.PermanentAddress = item.PermanentAddress;
        //                    data.LocalAddress = item.LocalAddress;
        //                    data.MedicalIssue = item.MedicalIssue;
        //                    data.Salary = item.Salary;
        //                    data.BankAccountNumber = item.BankAccountNumber;
        //                    data.IFSC = item.IFSC;
        //                    data.AccountHolderName = item.AccountHolderName;
        //                    data.BankName = item.BankName;
        //                    data.OfficeEmail = item.OfficeEmail;
        //                    data.EmployeeType = item.EmployeeTypes;
        //                    data.CompanyName = item.CompanyName;
        //                    employeeDataList.Add(data);
        //                }
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }
        //        else if (AdvanceFilter.Department.Count != 0 && AdvanceFilter.Company != 0)
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            foreach (var item1 in AdvanceFilter.Department)
        //            {
        //                var employeeData = (from ad in _db.Employee
        //                                    join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                    join ccd in _db.Company on ad.CompanyId equals ccd.CompanyId
        //                                    join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                    //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                    orderby ad.JoiningDate ascending
        //                                    where ad.RoleId == item1 && ad.CompanyId == AdvanceFilter.Company && ad.IsDeleted == false
        //                                    select new
        //                                    {
        //                                        ad.EmployeeId,
        //                                        n = ad.FirstName + " " + ad.LastName,
        //                                        ad.PrimaryContact,
        //                                        ad.PersonalEmail,
        //                                        bd.RoleType,
        //                                        ad.FirstName,
        //                                        ad.LastName,
        //                                        ad.MaritalStatus,
        //                                        ad.SpouseName,
        //                                        ad.FatherName,
        //                                        ad.MotherName,
        //                                        //fd.BloodGroupType,
        //                                        ad.JoiningDate,
        //                                        ad.ConfirmationDate,
        //                                        ad.DateOfBirth,
        //                                        ad.EmergencyNumber,
        //                                        ad.WhatsappNumber,
        //                                        ad.AadharNumber,
        //                                        ad.PanNumber,
        //                                        ad.PermanentAddress,
        //                                        ad.LocalAddress,
        //                                        ad.MedicalIssue,
        //                                        ad.Salary,
        //                                        ad.BankAccountNumber,
        //                                        ad.IFSC,
        //                                        ad.AccountHolderName,
        //                                        ad.BankName,
        //                                        ad.OfficeEmail,
        //                                        cd.EmployeeTypes,
        //                                        ad.CompanyName
        //                                    }).ToList();
        //                foreach (var item in employeeData)
        //                {
        //                    EmployeeData data = new EmployeeData();
        //                    data.EmployeeId = item.EmployeeId;
        //                    data.FullName = item.n;
        //                    data.PrimaryContact = item.PrimaryContact;
        //                    data.Email = item.PersonalEmail;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.MaritalStatus = item.MaritalStatus;
        //                    data.SpouseName = item.SpouseName;
        //                    data.FatherName = item.FatherName;
        //                    data.MotherName = item.MotherName;
        //                    //data.BloodGroup = item.BloodGroupType;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.JoiningDate = item.JoiningDate;
        //                    data.ConfirmationDate = item.ConfirmationDate;
        //                    data.DOB = item.DateOfBirth;
        //                    data.EmergencyNumber = item.EmergencyNumber;
        //                    data.WhatsappNumber = item.WhatsappNumber;
        //                    data.AadharNumber = item.AadharNumber;
        //                    data.PanNumber = item.PanNumber;
        //                    data.PermanentAddress = item.PermanentAddress;
        //                    data.LocalAddress = item.LocalAddress;
        //                    data.MedicalIssue = item.MedicalIssue;
        //                    data.Salary = item.Salary;
        //                    data.BankAccountNumber = item.BankAccountNumber;
        //                    data.IFSC = item.IFSC;
        //                    data.AccountHolderName = item.AccountHolderName;
        //                    data.BankName = item.BankName;
        //                    data.OfficeEmail = item.OfficeEmail;
        //                    data.EmployeeType = item.EmployeeTypes;
        //                    data.CompanyName = item.CompanyName;
        //                    employeeDataList.Add(data);
        //                }
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }
        //        else if (AdvanceFilter.Department.Count != 0 && (AdvanceFilter.Startjoiningdate != null && AdvanceFilter.Endjoiningdate != null))
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            foreach (var item1 in AdvanceFilter.Department)
        //            {
        //                var employeeData = (from ad in _db.Employee
        //                                    join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                    join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                    //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                    orderby ad.JoiningDate ascending
        //                                    where ad.RoleId == item1 && ad.JoiningDate >= AdvanceFilter.Startjoiningdate && ad.JoiningDate <= AdvanceFilter.Endjoiningdate && ad.IsDeleted == false
        //                                    select new
        //                                    {
        //                                        ad.EmployeeId,
        //                                        n = ad.FirstName + " " + ad.LastName,
        //                                        ad.PrimaryContact,
        //                                        ad.PersonalEmail,
        //                                        bd.RoleType,
        //                                        ad.FirstName,
        //                                        ad.LastName,
        //                                        ad.MaritalStatus,
        //                                        ad.SpouseName,
        //                                        ad.FatherName,
        //                                        ad.MotherName,
        //                                        //fd.BloodGroupType,
        //                                        ad.JoiningDate,
        //                                        ad.ConfirmationDate,
        //                                        ad.DateOfBirth,
        //                                        ad.EmergencyNumber,
        //                                        ad.WhatsappNumber,
        //                                        ad.AadharNumber,
        //                                        ad.PanNumber,
        //                                        ad.PermanentAddress,
        //                                        ad.LocalAddress,
        //                                        ad.MedicalIssue,
        //                                        ad.Salary,
        //                                        ad.BankAccountNumber,
        //                                        ad.IFSC,
        //                                        ad.AccountHolderName,
        //                                        ad.BankName,
        //                                        ad.OfficeEmail,
        //                                        cd.EmployeeTypes,
        //                                        ad.CompanyName
        //                                    }).ToList();
        //                foreach (var item in employeeData)
        //                {
        //                    EmployeeData data = new EmployeeData();
        //                    data.EmployeeId = item.EmployeeId;
        //                    data.FullName = item.n;
        //                    data.PrimaryContact = item.PrimaryContact;
        //                    data.Email = item.PersonalEmail;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.MaritalStatus = item.MaritalStatus;
        //                    data.SpouseName = item.SpouseName;
        //                    data.FatherName = item.FatherName;
        //                    data.MotherName = item.MotherName;
        //                    //data.BloodGroup = item.BloodGroupType;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.JoiningDate = item.JoiningDate;
        //                    data.ConfirmationDate = item.ConfirmationDate;
        //                    data.DOB = item.DateOfBirth;
        //                    data.EmergencyNumber = item.EmergencyNumber;
        //                    data.WhatsappNumber = item.WhatsappNumber;
        //                    data.AadharNumber = item.AadharNumber;
        //                    data.PanNumber = item.PanNumber;
        //                    data.PermanentAddress = item.PermanentAddress;
        //                    data.LocalAddress = item.LocalAddress;
        //                    data.MedicalIssue = item.MedicalIssue;
        //                    data.Salary = item.Salary;
        //                    data.BankAccountNumber = item.BankAccountNumber;
        //                    data.IFSC = item.IFSC;
        //                    data.AccountHolderName = item.AccountHolderName;
        //                    data.BankName = item.BankName;
        //                    data.OfficeEmail = item.OfficeEmail;
        //                    data.EmployeeType = item.EmployeeTypes;
        //                    data.CompanyName = item.CompanyName;
        //                    employeeDataList.Add(data);
        //                }
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }
        //        else if (AdvanceFilter.Employee_Type != 0 && AdvanceFilter.Company != 0)
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join ccd in _db.Company on ad.CompanyId equals ccd.CompanyId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                orderby ad.JoiningDate ascending
        //                                where ad.EmployeeTypeId == AdvanceFilter.Employee_Type && ad.CompanyId == AdvanceFilter.Company && ad.IsDeleted == false
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    cd.EmployeeTypes,
        //                                    ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DOB = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }
        //        else if (AdvanceFilter.Employee_Type != 0 && (AdvanceFilter.Startjoiningdate != null && AdvanceFilter.Endjoiningdate != null))
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                orderby ad.JoiningDate ascending
        //                                where ad.EmployeeTypeId == AdvanceFilter.Employee_Type && ad.JoiningDate >= AdvanceFilter.Startjoiningdate && ad.JoiningDate <= AdvanceFilter.Endjoiningdate && ad.IsDeleted == false
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    cd.EmployeeTypes,
        //                                    ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DOB = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }
        //        else if (AdvanceFilter.Company != 0 && (AdvanceFilter.Startjoiningdate != null && AdvanceFilter.Endjoiningdate != null))
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join ccd in _db.Company on ad.CompanyId equals ccd.CompanyId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                orderby ad.JoiningDate ascending
        //                                where ad.CompanyId == AdvanceFilter.Company && ad.JoiningDate >= AdvanceFilter.Startjoiningdate && ad.JoiningDate <= AdvanceFilter.Endjoiningdate && ad.IsDeleted == false
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    cd.EmployeeTypes,
        //                                    ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DOB = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }

        //        else if (AdvanceFilter.Department.Count != 0)
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            foreach (var item1 in AdvanceFilter.Department)
        //            {
        //                var employeeData = (from ad in _db.Employee
        //                                    join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                    join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                    //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                    orderby ad.JoiningDate ascending
        //                                    where ad.RoleId == item1 && ad.IsDeleted == false
        //                                    select new
        //                                    {
        //                                        ad.EmployeeId,
        //                                        n = ad.FirstName + " " + ad.LastName,
        //                                        ad.PrimaryContact,
        //                                        ad.PersonalEmail,
        //                                        bd.RoleType,
        //                                        ad.FirstName,
        //                                        ad.LastName,
        //                                        ad.MaritalStatus,
        //                                        ad.SpouseName,
        //                                        ad.FatherName,
        //                                        ad.MotherName,
        //                                        //fd.BloodGroupType,
        //                                        ad.JoiningDate,
        //                                        ad.ConfirmationDate,
        //                                        ad.DateOfBirth,
        //                                        ad.EmergencyNumber,
        //                                        ad.WhatsappNumber,
        //                                        ad.AadharNumber,
        //                                        ad.PanNumber,
        //                                        ad.PermanentAddress,
        //                                        ad.LocalAddress,
        //                                        ad.MedicalIssue,
        //                                        ad.Salary,
        //                                        ad.BankAccountNumber,
        //                                        ad.IFSC,
        //                                        ad.AccountHolderName,
        //                                        ad.BankName,
        //                                        ad.OfficeEmail,
        //                                        cd.EmployeeTypes,
        //                                        ad.CompanyName
        //                                    }).ToList();
        //                foreach (var item in employeeData)
        //                {
        //                    EmployeeData data = new EmployeeData();
        //                    data.EmployeeId = item.EmployeeId;
        //                    data.FullName = item.n;
        //                    data.PrimaryContact = item.PrimaryContact;
        //                    data.Email = item.PersonalEmail;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.MaritalStatus = item.MaritalStatus;
        //                    data.SpouseName = item.SpouseName;
        //                    data.FatherName = item.FatherName;
        //                    data.MotherName = item.MotherName;
        //                    //data.BloodGroup = item.BloodGroupType;
        //                    data.RoleType = item.RoleType;
        //                    data.FirstName = item.FirstName;
        //                    data.LastName = item.LastName;
        //                    data.JoiningDate = item.JoiningDate;
        //                    data.ConfirmationDate = item.ConfirmationDate;
        //                    data.DOB = item.DateOfBirth;
        //                    data.EmergencyNumber = item.EmergencyNumber;
        //                    data.WhatsappNumber = item.WhatsappNumber;
        //                    data.AadharNumber = item.AadharNumber;
        //                    data.PanNumber = item.PanNumber;
        //                    data.PermanentAddress = item.PermanentAddress;
        //                    data.LocalAddress = item.LocalAddress;
        //                    data.MedicalIssue = item.MedicalIssue;
        //                    data.Salary = item.Salary;
        //                    data.BankAccountNumber = item.BankAccountNumber;
        //                    data.IFSC = item.IFSC;
        //                    data.AccountHolderName = item.AccountHolderName;
        //                    data.BankName = item.BankName;
        //                    data.OfficeEmail = item.OfficeEmail;
        //                    data.EmployeeType = item.EmployeeTypes;
        //                    data.CompanyName = item.CompanyName;
        //                    employeeDataList.Add(data);
        //                }
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }
        //        else if (AdvanceFilter.Company != 0)
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join ccd in _db.Company on ad.CompanyId equals ccd.CompanyId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                orderby ad.JoiningDate ascending
        //                                where ad.CompanyId == AdvanceFilter.Company && ad.IsDeleted == false
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    cd.EmployeeTypes,
        //                                    ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DOB = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }
        //        else if (AdvanceFilter.Employee_Type != 0)
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                orderby ad.JoiningDate ascending
        //                                where ad.EmployeeTypeId == AdvanceFilter.Employee_Type && ad.IsDeleted == false
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    cd.EmployeeTypes,
        //                                    ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DOB = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }
        //        else if (AdvanceFilter.Startjoiningdate != null)
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId
        //                                //join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
        //                                orderby ad.JoiningDate ascending
        //                                where ad.JoiningDate == AdvanceFilter.Startjoiningdate && ad.JoiningDate == AdvanceFilter.Endjoiningdate && ad.IsDeleted == false
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    cd.EmployeeTypes,
        //                                    ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DOB = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }

        //        else if ((AdvanceFilter.Startjoiningdate != null && AdvanceFilter.Endjoiningdate != null))
        //        {
        //            Base response = new Base();
        //            List<EmployeeData> employeeDataList = new List<EmployeeData>();
        //            var employeeData = (from ad in _db.Employee
        //                                join bd in _db.Role on ad.RoleId equals bd.RoleId
        //                                join cd in _db.EmployeeType on ad.EmployeeTypeId equals cd.EmployeeTypeId

        //                                orderby ad.JoiningDate ascending
        //                                where ad.JoiningDate >= AdvanceFilter.Startjoiningdate && ad.JoiningDate <= AdvanceFilter.Endjoiningdate && ad.IsDeleted == false
        //                                select new
        //                                {
        //                                    ad.EmployeeId,
        //                                    n = ad.FirstName + " " + ad.LastName,
        //                                    ad.PrimaryContact,
        //                                    ad.PersonalEmail,
        //                                    bd.RoleType,
        //                                    ad.FirstName,
        //                                    ad.LastName,
        //                                    ad.MaritalStatus,
        //                                    ad.SpouseName,
        //                                    ad.FatherName,
        //                                    ad.MotherName,
        //                                    //fd.BloodGroupType,
        //                                    ad.JoiningDate,
        //                                    ad.ConfirmationDate,
        //                                    ad.DateOfBirth,
        //                                    ad.EmergencyNumber,
        //                                    ad.WhatsappNumber,
        //                                    ad.AadharNumber,
        //                                    ad.PanNumber,
        //                                    ad.PermanentAddress,
        //                                    ad.LocalAddress,
        //                                    ad.MedicalIssue,
        //                                    ad.Salary,
        //                                    ad.BankAccountNumber,
        //                                    ad.IFSC,
        //                                    ad.AccountHolderName,
        //                                    ad.BankName,
        //                                    ad.OfficeEmail,
        //                                    cd.EmployeeTypes,
        //                                    ad.CompanyName
        //                                }).ToList();
        //            foreach (var item in employeeData)
        //            {
        //                EmployeeData data = new EmployeeData();
        //                data.EmployeeId = item.EmployeeId;
        //                data.FullName = item.n;
        //                data.PrimaryContact = item.PrimaryContact;
        //                data.Email = item.PersonalEmail;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.MaritalStatus = item.MaritalStatus;
        //                data.SpouseName = item.SpouseName;
        //                data.FatherName = item.FatherName;
        //                data.MotherName = item.MotherName;
        //                //data.BloodGroup = item.BloodGroupType;
        //                data.RoleType = item.RoleType;
        //                data.FirstName = item.FirstName;
        //                data.LastName = item.LastName;
        //                data.JoiningDate = item.JoiningDate;
        //                data.ConfirmationDate = item.ConfirmationDate;
        //                data.DOB = item.DateOfBirth;
        //                data.EmergencyNumber = item.EmergencyNumber;
        //                data.WhatsappNumber = item.WhatsappNumber;
        //                data.AadharNumber = item.AadharNumber;
        //                data.PanNumber = item.PanNumber;
        //                data.PermanentAddress = item.PermanentAddress;
        //                data.LocalAddress = item.LocalAddress;
        //                data.MedicalIssue = item.MedicalIssue;
        //                data.Salary = item.Salary;
        //                data.BankAccountNumber = item.BankAccountNumber;
        //                data.IFSC = item.IFSC;
        //                data.AccountHolderName = item.AccountHolderName;
        //                data.BankName = item.BankName;
        //                data.OfficeEmail = item.OfficeEmail;
        //                data.EmployeeType = item.EmployeeTypes;
        //                data.CompanyName = item.CompanyName;
        //                employeeDataList.Add(data);
        //            }
        //            if (employeeDataList.Count != 0)
        //            {
        //                response.StatusReason = true;
        //                response.Message = "Data Found";
        //                response.employeeDataList = employeeDataList;
        //            }
        //            else
        //            {
        //                response.StatusReason = false;
        //                response.Message = "Data Not Found";
        //            }
        //            return Ok(response);
        //        }
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //public class timesheetData
        //{
        //    public int TimesheetId { get; set; }
        //    public int Project { get; set; }
        //    public string ProjectName { get; set; }
        //    public string Task { get; set; }
        //    public string Description { get; set; }
        //    public int EmployeeId { get; set; }
        //    public string EmployeeName { get; set; }
        //    public Nullable<System.DateTime> Date { get; set; }
        //    public Nullable<System.DateTime> CreatedDate { get; set; }
        //    public string Time { get; set; }
        //    public bool IsActive { get; set; }
        //    public bool IsDeleted { get; set; }
        //}

        //[HttpGet]
        //[Route("GetWeekDates")]
        //public IHttpActionResult GetWeekDates(int Id)
        //{
        //    DayOfWeek currentDay = DateTime.Now.DayOfWeek;

        //    DateTime startOfWeek = DateTime.Now.AddDays(-(currentDay - DayOfWeek.Monday));

        //    //DateTime startOfWeek = DateTime.Today.AddDays((int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)DateTime.Today.DayOfWeek);
        //    List<DateResponse> dateDataList = new List<DateResponse>();

        //    string result = string.Join(",", Enumerable
        //      .Range(0, 7)
        //      .Select(i => startOfWeek
        //         .AddDays(i)
        //         .ToString("dd-MMMM-yyyy")));

        //    string[] dates = result.Split(new string[] { "," },
        //                      StringSplitOptions.None);
        //    foreach (var item in dates)
        //    {
        //        var checkDate = Convert.ToDateTime(item);
        //        var checkData = (from ad in _db.Timesheet where ad.Date == checkDate && ad.EmployeeId == Id select ad).FirstOrDefault();
        //        if (checkData != null)
        //        {
        //            DateResponse DateResponse = new DateResponse();
        //            DateResponse.Date = checkDate;
        //            DateResponse.Status = true;
        //            dateDataList.Add(DateResponse);
        //        }
        //        else
        //        {
        //            DateResponse DateResponse = new DateResponse();
        //            DateResponse.Status = false;
        //            DateResponse.Date = checkDate;
        //            dateDataList.Add(DateResponse);
        //        }
        //    }

        //    return Ok(dateDataList);
        //}

        //public class DateResponse
        //{
        //    public bool Status { get; set; }
        //    public Nullable<System.DateTime> Date { get; set; }
        //}

        //#region Employee Document

        //[HttpPost]
        //[Route("CreateFolder")]
        //public IHttpActionResult CreateFolderName(Employee model)
        //{
        //    try
        //    {
        //        Base response = new Base();
        //        var selectedItem = _db.Employee.Where(p => p.EmployeeId == model.EmployeeId).ToList();
        //        string guid = Guid.NewGuid().ToString("N").Substring(0, 5);

        //        foreach (var item in selectedItem)
        //        {
        //            var checkFolder = (from ad in _db.EmployeeFolder where ad.EmployeeId == item.EmployeeId select ad).FirstOrDefault();
        //            if (checkFolder == null)
        //            {
        //                EmployeeFolder folderModel = new EmployeeFolder();
        //                string[] array = item.OfficeEmail.Split(new string[] { "@" }, StringSplitOptions.None);
        //                string email = array[0];
        //                string empFolderName = email + "_" + guid;

        //                folderModel.EmployeeName = item.FirstName + " " + item.LastName;
        //                folderModel.EmployeeId = item.EmployeeId;
        //                folderModel.FolderName = empFolderName;
        //                folderModel.IsFolder = true;
        //                folderModel.IsActive = true;
        //                folderModel.IsDeleted = false;
        //                folderModel.CreatedDate = DateTime.Now;

        //                _db.EmployeeFolder.Add(folderModel);
        //                _db.SaveChanges();

        //                response.Message = "Employee Folder Created Successfully";
        //                response.StatusReason = true;
        //            }
        //            else
        //            {
        //                response.Message = "Employee Folder Already Exist";
        //                response.StatusReason = false;
        //            }
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpGet]
        //[Route("GetAllFolders")]
        //public IHttpActionResult GetAllFolders()
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        Base response = new Base();
        //        var allFolders = _db.EmployeeFolder.Where(f => f.IsDeleted == false).ToList();
        //        if (allFolders.Count != 0)
        //        {
        //            response.Message = "Data Found";
        //            response.StatusReason = true;
        //            response.employeeFolderData = allFolders;
        //        }
        //        else
        //        {
        //            response.Message = "Data Not Found";
        //            response.StatusReason = false;
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("UploadFile")]
        //public IHttpActionResult UploadFile(int EmployeeId, int UploadedBy)
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        Base response = new Base();
        //        var Request = HttpContext.Current.Request;
        //        if (Request.Files.Count > 0)
        //        {
        //            HttpFileCollection files = Request.Files;
        //            for (int i = 0; i < files.Count; i++)
        //            {
        //                var saveEmpFile = _db.EmployeeFolder.Where(x => x.EmployeeId == EmployeeId).FirstOrDefault();
        //                HttpPostedFile file = files[i];
        //                string fname;

        //                // Checking for Internet Explorer
        //                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
        //                {
        //                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
        //                    fname = testfiles[testfiles.Length - 1];
        //                }
        //                else
        //                {
        //                    fname = saveEmpFile.FolderId + file.FileName;
        //                }

        //                // Get the complete folder path and store the file inside it.
        //                fname = Path.Combine(HttpContext.Current.Server.MapPath("~/EmployeeDocuments/"), fname);
        //                file.SaveAs(fname);

        //                EmployeeFiles model = new EmployeeFiles();
        //                model.EmployeeId = saveEmpFile.EmployeeId;
        //                model.FolderId = saveEmpFile.FolderId;
        //                model.FileName = fname;
        //                model.CreatedDate = DateTime.Now;
        //                model.Name = file.FileName;
        //                model.UploadedBy = UploadedBy;
        //                model.DownloadFileName = saveEmpFile.FolderId + file.FileName;

        //                _db.EmployeeFiles.Add(model);
        //                _db.SaveChanges();
        //            }
        //            response.Message = "File Uploaded successfully";
        //            response.StatusReason = true;
        //        }
        //        else
        //        {
        //            response.Message = "No files selected";
        //            response.StatusReason = false;
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpGet]
        //[Route("GetEmployeeFiles")]
        //public IHttpActionResult GetEmployeeFiles(int Id)
        //{
        //    try
        //    {
        //        var identity = User.Identity as ClaimsIdentity;
        //        int userid = 0;
        //        int compid = 0;
        //        int orgid = 0;

        //        // Access claims

        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
        //            userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
        //            compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
        //        if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
        //            orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

        //        List<EmployeeFileData> list = new List<EmployeeFileData>();
        //        Base response = new Base();
        //        var getFiles = (from ad in _db.EmployeeFiles
        //                        join bd in _db.Employee on ad.UploadedBy equals bd.EmployeeId
        //                        where ad.EmployeeId == Id
        //                        select new
        //                        {
        //                            ad.FileId,
        //                            ad.EmployeeId,
        //                            ad.FolderId,
        //                            ad.FileName,
        //                            ad.Name,
        //                            ad.CreatedDate,
        //                            ad.UploadedBy,
        //                            v = bd.FirstName + " " + bd.LastName,
        //                            ad.DownloadFileName
        //                        }).ToList();
        //        foreach (var item in getFiles)
        //        {
        //            EmployeeFileData data = new EmployeeFileData();
        //            data.FileId = item.FileId;
        //            data.EmployeeId = item.EmployeeId;
        //            data.FolderId = item.FolderId;
        //            data.FileName = item.FileName;
        //            data.Name = item.Name;
        //            data.CreatedDate = item.CreatedDate;
        //            data.UploadedBy = item.UploadedBy;
        //            data.UploadedByName = item.v;
        //            data.DownloadFileName = item.DownloadFileName;
        //            list.Add(data);
        //        }
        //        if (getFiles.Count != 0)
        //        {
        //            response.Message = "Data Found";
        //            response.StatusReason = true;
        //            response.employeeFilesData = list;
        //        }
        //        else
        //        {
        //            response.Message = "Data Not Found";
        //            response.StatusReason = false;
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);

        //    }
        //}

        //public class EmployeeFileData
        //{
        //    public int FileId { get; set; }
        //    public int EmployeeId { get; set; }
        //    public int FolderId { get; set; }
        //    public string FileName { get; set; }
        //    public string Name { get; set; }
        //    public int UploadedBy { get; set; }
        //    public string UploadedByName { get; set; }
        //    public String DownloadFileName { get; set; }
        //    public Nullable<System.DateTime> CreatedDate { get; set; }
        //}

        //#endregion

        //#region Organizational Document

        //[HttpPost]
        //[Route("CreateOrganizatioFolder")]
        //public IHttpActionResult CreateOrganizatioFolder(OrganizationFolder model)
        //{
        //    try
        //    {
        //        Base response = new Base();
        //        var checkFolder = (from ad in _db.OrganizationFolder where ad.FolderName == model.FolderName select ad).FirstOrDefault();
        //        if (checkFolder == null)
        //        {
        //            OrganizationFolder folderModel = new OrganizationFolder();
        //            folderModel.FolderName = model.FolderName;
        //            folderModel.IsFolder = true;
        //            folderModel.IsActive = true;
        //            folderModel.IsDeleted = false;
        //            folderModel.CreatedDate = DateTime.Now;

        //            _db.OrganizationFolder.Add(folderModel);
        //            _db.SaveChanges();

        //            response.Message = "Folder Created Successfully";
        //            response.StatusReason = true;
        //        }
        //        else
        //        {
        //            response.Message = "Folder Already Exist";
        //            response.StatusReason = false;
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpGet]
        //[Route("GetOrganizationFolder")]
        //public IHttpActionResult GetOrganizationFolder()
        //{
        //    try
        //    {
        //        Base response = new Base();
        //        var allFolders = _db.OrganizationFolder.Where(f => f.IsDeleted == false).ToList();
        //        if (allFolders.Count != 0)
        //        {
        //            response.Message = "Data Found";
        //            response.StatusReason = true;
        //            response.organizationFolderData = allFolders;
        //        }
        //        else
        //        {
        //            response.Message = "Data Not Found";
        //            response.StatusReason = false;
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("UploadOrganizationFile")]
        //public IHttpActionResult UploadOrganizationFile(int FolderId, int UploadedBy)
        //{
        //    try
        //    {
        //        Base response = new Base();
        //        var Request = HttpContext.Current.Request;
        //        if (Request.Files.Count > 0)
        //        {
        //            HttpFileCollection files = Request.Files;
        //            for (int i = 0; i < files.Count; i++)
        //            {
        //                HttpPostedFile file = files[i];
        //                string fname;

        //                // Checking for Internet Explorer
        //                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
        //                {
        //                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
        //                    fname = testfiles[testfiles.Length - 1];
        //                }
        //                else
        //                {
        //                    fname = FolderId + file.FileName;
        //                }

        //                // Get the complete folder path and store the file inside it.
        //                fname = Path.Combine(HttpContext.Current.Server.MapPath("~/OrganizationDocuments/"), fname);
        //                file.SaveAs(fname);

        //                OrganizationFiles model = new OrganizationFiles();
        //                model.FolderId = FolderId;
        //                model.FileName = fname;
        //                model.CreatedDate = DateTime.Now;
        //                model.Name = file.FileName;
        //                model.UploadedBy = UploadedBy;
        //                model.DownloadFileName = FolderId + file.FileName;

        //                _db.OrganizationFiles.Add(model);
        //                _db.SaveChanges();
        //            }
        //            response.Message = "File Uploaded successfully";
        //            response.StatusReason = true;
        //        }
        //        else
        //        {
        //            response.Message = "No files selected";
        //            response.StatusReason = false;
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpGet]
        //[Route("GetOrganizationFiles")]
        //public IHttpActionResult GetOrganizationFiles(int Id)
        //{
        //    try
        //    {
        //        List<OrganizationData> list = new List<OrganizationData>();
        //        Base response = new Base();
        //        var getFiles = (from ad in _db.OrganizationFiles
        //                        join bd in _db.Employee on ad.UploadedBy equals bd.EmployeeId
        //                        where ad.FolderId == Id
        //                        select new
        //                        {
        //                            ad.FileId,
        //                            ad.FolderId,
        //                            ad.FileName,
        //                            ad.Name,
        //                            ad.CreatedDate,
        //                            ad.UploadedBy,
        //                            v = bd.FirstName + " " + bd.LastName,
        //                            ad.DownloadFileName
        //                        }).ToList();
        //        foreach (var item in getFiles)
        //        {
        //            OrganizationData data = new OrganizationData();
        //            data.FileId = item.FileId;
        //            data.FolderId = item.FolderId;
        //            data.FileName = item.FileName;
        //            data.Name = item.Name;
        //            data.CreatedDate = item.CreatedDate;
        //            data.UploadedBy = item.UploadedBy;
        //            data.UploadedByName = item.v;
        //            data.DownloadFileName = item.DownloadFileName;
        //            list.Add(data);
        //        }
        //        if (getFiles.Count != 0)
        //        {
        //            response.Message = "Data Found";
        //            response.StatusReason = true;
        //            response.organizationFilesData = list;
        //        }
        //        else
        //        {
        //            response.Message = "Data Not Found";
        //            response.StatusReason = false;
        //        }
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);

        //    }
        //}

        //public class OrganizationData
        //{
        //    public int FileId { get; set; }
        //    public int FolderId { get; set; }
        //    public string FileName { get; set; }
        //    public string UploadedByName { get; set; }
        //    public string Name { get; set; }
        //    public int UploadedBy { get; set; }
        //    public String DownloadFileName { get; set; }
        //    public Nullable<System.DateTime> CreatedDate { get; set; }
        //}

        //#endregion

        //public class EmpolyeTree
        //{
        //    public int Id { get; set; }
        //    public string name { get; set; }
        //    public string title { get; set; }
        //    public string image { get; set; }
        //    public string cssClass { get; set; }
        //    public List<EmpolyeTree> childs { get; set; }
        //}
        //public class EmpolyeTreeDC
        //{
        //    public int Id { get; set; }
        //    public string name { get; set; }
        //    public string title { get; set; }
        //    public string image { get; set; }
        //    public string cssClass { get; set; }
        //    public string ReportingManager { get; set; }
        //}

        ////duplicate DTO for alternate JSON
        ////public class EmpolyeTree1
        ////{
        ////    public int Id { get; set; }
        ////    public string name { get; set; }
        ////    public string type { get; set; }
        ////    public string image { get; set; }
        ////    public string cssClass { get; set; }
        ////    public List<EmpolyeTree1> children { get; set; }
        ////}
        ////public class EmpolyeTreeDC1
        ////{
        ////    public int Id { get; set; }
        ////    public string name { get; set; }
        ////    public string type { get; set; }
        ////    public string image { get; set; }
        ////    public string cssClass { get; set; }
        ////    public int? ReportingManager { get; set; }
        ////    public List<EmpolyeTree1> OrgData { get; set; }
        ////}

        //public class Node
        //{
        //    public string Name { get; set; }
        //    public List<Node> Children { get; set; }
        //}
        //public class EmployeeDataForLeaveBalance
        //{
        //    public int EmployeeId { get; set; }
        //    public string FullName { get; set; }
        //    public string FatherName { get; set; }
        //    public int SickLeave { get; set; }
        //    public int PaidLeave { get; set; }
        //    public int UnpaidLeave { get; set; }
        //    public int FloaterLeave { get; set; }
        //    public int SpecialLeave { get; set; }
        //    public int MaternityLeave { get; set; }
        //    public int PaternityLeave { get; set; }
        //    public int BereavementLeave { get; set; }
        //    public int CasualLeave { get; set; }
        //    public int CompOffs { get; set; }
        //    public DateTime MonthYear { get; set; }
        //    public string Status { get; set; }
        //    public string Message { get; set; }

        //}
        //public class EmployeeDataForCurrentSalaryInformation
        //{
        //    public int EmployeeId { get; set; }
        //    public string FullName { get; set; }
        //    public string FatherName { get; set; }
        //    public float FixedAnnualGross { get; set; }
        //    public float CTCExludingBonus { get; set; }

        //}
        //public class EmployeeDataForSalary
        //{
        //    public int EmployeeId { get; set; }
        //    public string FullName { get; set; }
        //    public string FatherName { get; set; }
        //    public float MonthlyGross { get; set; }
        //    public float NumberOfPayDays { get; set; }
        //    public float Basic { get; set; }
        //    public float HRA { get; set; }
        //    public float MedicalAllowance { get; set; }
        //    public float ConveyanceAllowance { get; set; }
        //    public float SpecialAllowance { get; set; }
        //    public float TravelReimbursement { get; set; }
        //    public float Reimbursements { get; set; }
        //    public float Arrears { get; set; }
        //    public float ProvidentFund { get; set; }
        //    public float IncomeTax { get; set; }
        //    public float ProfessionalTax { get; set; }
        //    public float SalaryAdvance { get; set; }
        //    public float NetPay { get; set; }
        //    public DateTime SalaryDate { get; set; }

        //}
        //public class EmployeeDataForBonus
        //{
        //    public int EmployeeId { get; set; }
        //    public string FullName { get; set; }
        //    public string FatherName { get; set; }
        //    public string BunusType { get; set; }
        //    public float BonusAmount { get; set; }
        //    public DateTime PayoutDate { get; set; }

        //}
        //public class CompOrgData
        //{
        //    public string CompanyName { get; set; }
        //    public string OrgName { get; set; }
        //    public int CompanyId { get; set; }
        //    public int OrgId { get; set; }
        //    public int Count { get; set; }
        //}
        //public class CompOrgDataForMonth
        //{
        //    public int CompanyId { get; set; }
        //    public int OrgId { get; set; }
        //    public int intMonth { get; set; }
        //    public string strMonth { get; set; }
        //    public int intCount { get; set; }
        //}
        //public class PerformanceData
        //{
        //    public int FeedbackId { get; set; }

        //    public string HR { get; set; }

        //    public string Employee { get; set; }

        //    public string ProjectManager { get; set; }

        //    public string ProjectName { get; set; }

        //}

        //public class EmployeeList
        //{
        //    public int EmployeeId { get; set; }
        //    public string FullName { get; set; }
        //    public string FirstName { get; set; }
        //    public string MiddleName { get; set; }
        //    public string LastName { get; set; }
        //    public string Password { get; set; }
        //    public int CompanyId { get; set; }
        //    public string PrimaryContact { get; set; }
        //    public string Email { get; set; }
        //    public string MaritalStatus { get; set; }
        //    public string SpouseName { get; set; }
        //    public string FatherName { get; set; }
        //    public string MotherName { get; set; }
        //    public string BloodGroup { get; set; }
        //    public string Document { get; set; }
        //    public string PermanentAddress { get; set; }
        //    public string LocalAddress { get; set; }
        //    public Nullable<System.DateTime> JoiningDate { get; set; }
        //    public Nullable<System.DateTime> ConfirmationDate { get; set; }
        //    public DateTime? DateOfBirth { get; set; }
        //    public string WhatsappNumber { get; set; }

        //    public string AadharNumber { get; set; }
        //    public string PanNumber { get; set; }
        //    public string MedicalIssue { get; set; }
        //    public string BankAccountNumber { get; set; }
        //    public string IFSC { get; set; }
        //    public string AccountHolderName { get; set; }
        //    public string BankName { get; set; }
        //    public string OfficeEmail { get; set; }
        //    public string DepartmentName { get; set; }

        //    public string DesignationName { get; set; }
        //    public string SecondaryJobTitle { get; set; }
        //    public long? BiometricID { get; set; }
        //    public int EmployeeTypeID { get; set; }
        //    public int EmergencyNumber { get; set; }
        //    public string ReportingManager { get; set; }
        //}

        //public class EmployeeData
        //{
        //    public int EmployeeId { get; set; }
        //    public string FullName { get; set; }
        //    public string FirstName { get; set; }
        //    public string LastName { get; set; }
        //    public string Password { get; set; }
        //    public int CompanyId { get; set; }

        //    public string UploadResume { get; set; }
        //    public string BloodGroupType { get; set; }

        //    public string PrimaryContact { get; set; }
        //    public string RoleType { get; set; }
        //    public string Email { get; set; }

        //    public int EmployeeTypeID { get; set; }
        //    public int OrgId { get; set; }

        //    public string EmployeeCode { get; set; }
        //    public string SecondaryContact { get; set; }
        //    public string MaritalStatus { get; set; }
        //    public string SpouseName { get; set; }
        //    public string FatherName { get; set; }
        //    public string MotherName { get; set; }
        //    public Nullable<System.DateTime> CreatedOn { get; set; }
        //    public Nullable<System.DateTime> UpdatedOn { get; set; }
        //    public bool IsActive { get; set; }
        //    public bool IsDeleted { get; set; }
        //    public int BloodGroupId { get; set; }
        //    public string BloodGroup { get; set; }
        //    public string Document { get; set; }
        //    public int RoleId { get; set; }

        //    public string PermanentAddress { get; set; }
        //    public string LocalAddress { get; set; }
        //    public Nullable<System.DateTime> JoiningDate { get; set; }
        //    public Nullable<System.DateTime> ConfirmationDate { get; set; }
        //    public DateTime DOB { get; set; }
        //    public DateTime? DateOfBirth { get; set; }
        //    public string EmergencyNumber { get; set; }
        //    public string WhatsappNumber { get; set; }

        //    public string AadharNumber { get; set; }
        //    public string PanNumber { get; set; }
        //    public string MedicalIssue { get; set; }
        //    public string Profile { get; set; }
        //    public double Salary { get; set; }
        //    public string BankAccountNumber { get; set; }
        //    public string IFSC { get; set; }
        //    public string AccountHolderName { get; set; }
        //    public string BankName { get; set; }
        //    public string OfficeEmail { get; set; }
        //    public string EmployeeType { get; set; }
        //    public string CompanyName { get; set; }
        //    public string DepartmentName { get; set; }
        //    public int DepartmentId { get; set; }
        //    public string Gender { get; set; }
        //    public string DisplayName { get; set; }
        //    public long? BiometricID { get; set; }
        //    public long? AttendanceNumber { get; set; }
        //    public string PayGroup { get; set; }
        //    public string SkypeMail { get; set; }
        //    public string Band { get; set; }
        //    public string SecondaryJobTitle { get; set; }
        //    public Nullable<System.DateTime> ProbationEndDate { get; set; }
        //    public int? ReportingManager { get; set; }
        //    public string WeeklyOffPolicy { get; set; }
        //    public string ResidenceNumber { get; set; }
        //    public string TimeType { get; set; }
        //    public string WorkerType { get; set; }
        //    public string ShiftType { get; set; }
        //    public int? NoticePeriodMonths { get; set; }
        //    public int? CostCenter { get; set; }
        //    public string WorkNumber { get; set; }
        //    public string AboutMeRemark { get; set; }
        //    public string AboutMyJobRemark { get; set; }
        //    public string InterestAndHobbiesRemark { get; set; }

        //}
        //public class EmployeeDocData
        //{
        //    public int EmployeeDocId { get; set; }
        //    public int EmployeeId { get; set; }
        //    public int DocTypeId { get; set; }
        //    public string Branch { get; set; }
        //    public string Degree { get; set; }
        //    public DateTime DateOfJoining { get; set; }
        //    public DateTime DateOfCompleation { get; set; }
        //    public string PerctOrCGPA { get; set; }
        //    public string UniversityOrCollage { get; set; }
        //    public string DegreeUpload { get; set; }
        //    public string PanNumber { get; set; }
        //    public string NameOnPan { get; set; }
        //    public DateTime? DateOfBirthDateOnPan { get; set; }
        //    public string FatherNameOnPan { get; set; }
        //    public string PanUpload { get; set; }
        //    public string AadhaarCardNumber { get; set; }
        //    public DateTime? DateOfBirthOnAadhaar { get; set; }
        //    public string NameOnAadhaar { get; set; }
        //    public string FatherHusbandNameOnAadhaar { get; set; }
        //    public string GenderOnAadhaar { get; set; }
        //    public string AddressOnAadhaar { get; set; }
        //    public string FrontAadhaarUpload { get; set; }
        //    public string BackAadhaarUpload { get; set; }
        //    public string VoterIdNumber { get; set; }
        //    public DateTime? DateOfBirthOnVoterId { get; set; }
        //    public string NameOnVoterId { get; set; }
        //    public string FatherHusbandNameOnVoter { get; set; }
        //    public string AddressOnVoterId { get; set; }
        //    public string Licensenumber { get; set; }
        //    public DateTime? DateOfBirthOnDriving { get; set; }
        //    public string NameOnDriving { get; set; }
        //    public string FatherHusbandNameOnDriving { get; set; }
        //    public DateTime? ExpireOnLicense { get; set; }
        //    public string DrivingLicenseUpload { get; set; }
        //    public string CompanyName { get; set; }
        //    public string JobTitle { get; set; }
        //    public DateTime? JoiningDateExperience { get; set; }
        //    public DateTime? RelievoingDateExperience { get; set; }
        //    public string LocationExperience { get; set; }
        //    public string DescriptionExperience { get; set; }
        //    public string ExperienceUpload { get; set; }
        //    public string PaySlipsUpload { get; set; }
        //    public string Description { get; set; }
        //    public string location { get; set; }
        //    public DateTime DateOfRelieving { get; set; }
        //    public string Name { get; set; }
        //    public string Designation { get; set; }
        //    public string SignatureUpload { get; set; }

        //}

        //#region Api To Get Designation List
        ///// <summary>
        ///// Cretaed By Ankit Department List
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("getDesignationList")]
        //public async Task<ResponseBodyModel> getDesignation()
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();

        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        var DesignationData = (from d in _db.Department
        //                               join r in _db.Designation on d.DepartmentId equals r.DepartmentId
        //                               where r.IsDeleted == false && r.IsActive == true
        //                               select new
        //                               {
        //                                   d.DepartmentName,
        //                                   r.DesignationName,
        //                                   NewDesignation = d.DepartmentName + " - " + r.DesignationName,

        //                               }).ToList();

        //        if (DesignationData != null)
        //        {
        //            res.Message = "Designation Data  Found";
        //            res.Status = true;
        //            res.Data = DesignationData;
        //        }
        //        else
        //        {
        //            res.Message = "Designation Data Not Found";
        //            res.Status = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        //#endregion

        //#region Api To Get Department List
        ///// <summary>
        ///// Cretaed By Ankit Department List
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("getDepartmentList")]
        //public async Task<ResponseBodyModel> getDepartment()
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        var DepartmentData = await _db.Department.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyid &&
        //               x.OrgId == claims.orgid)
        //               .Select(x => new
        //               {
        //                   name = x.DepartmentName,
        //               }).ToListAsync();

        //        if (DepartmentData != null)
        //        {
        //            res.Message = "Department Data Found";
        //            res.Status = true;
        //            res.Data = DepartmentData;
        //        }
        //        else
        //        {
        //            res.Message = "Department Data Not Found";
        //            res.Status = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        //#endregion

        //#region Api To Get Employee Porfile Details
        //[HttpGet]
        //[Route("getempporfile")]
        //public async Task<ResponseBodyModel> GetEmployeeProile()
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    EmployeeProfileModel response = new EmployeeProfileModel();
        //    PrimaryDetailsHelper pri = new PrimaryDetailsHelper();
        //    ContactDetailsHelper con = new ContactDetailsHelper();
        //    CurrentAddress add = new CurrentAddress();
        //    ProfessionalSummaryHelper prosum = new ProfessionalSummaryHelper();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        var employeeData = await _db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == claims.userid &&
        //                    x.CompanyId == claims.companyid && x.OrgId == claims.orgid);

        //        if (employeeData != null)
        //        {
        //            pri.FirstName = employeeData.FirstName;
        //            pri.MiddleName = employeeData.MiddleName;
        //            pri.LastName = employeeData.LastName;
        //            pri.DisplayName = employeeData.DisplayName;
        //            pri.Gender = employeeData.Gender;
        //            pri.DateOfBirth = employeeData.DateOfBirth;
        //            pri.MaritalStatus = employeeData.MaritalStatus;
        //            //pri.BloodGroup = employeeData.BloodGroup;
        //            response.PrimaryDetails = pri;

        //            con.WorkEmail = employeeData.OfficeEmail;
        //            con.PersonalEmail = employeeData.PersonalEmail;
        //            con.MobilePhone = employeeData.MobilePhone;
        //            con.WorkPhone = employeeData.WorkPhone;
        //            con.ResidentPhone = employeeData.ResidentPhone;
        //            con.SkypeMail = employeeData.SkypeMail;
        //            response.ContactDetail = con;

        //            add.AddressLine1 = employeeData.AddressLine1;
        //            add.AddressLine2 = employeeData.AddressLine2;
        //            add.City = employeeData.City;
        //            add.State = employeeData.State;
        //            add.Country = employeeData.Country;
        //            add.PinCode = employeeData.Pincode;
        //            response.AddressDetail = add;

        //            prosum.Summary = employeeData.Summary;
        //            response.ProSummary = prosum;
        //            res.Message = "Employee Data Found";
        //            res.Status = true;
        //            res.Data = response;
        //        }
        //        else
        //        {
        //            res.Message = "Employee Data Not Found";
        //            res.Status = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        //#endregion

        //#region Api To Get Employee Details
        ///// <summary>
        ///// Cretaed By Ankit Employee Details
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("getemployeData")]
        //public async Task<ResponseBodyModel> getemployeData()
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();

        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        var employeeData = await _db.Employee.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyid &&
        //               x.OrgId == claims.orgid)
        //               .Select(x => new
        //               {
        //                   x.EmployeeId,
        //                   x.DepartmentName,
        //                   x.OfficeEmail,
        //                   x.PrimaryContact,
        //                   FullName = x.FirstName + "" + x.LastName
        //               }).ToListAsync();

        //        if (employeeData != null)
        //        {
        //            res.Message = "Employee Data Found";
        //            res.Status = true;
        //            res.Data = employeeData;
        //        }
        //        else
        //        {
        //            res.Message = "Employee Data Not Found";
        //            res.Status = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        //#endregion

        //#region this api use create employee
        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="Employee"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("CreateEmployee")]
        //public async Task<ResponseBodyModel> CreateEmployee(Employee Employee)// we are using contact table as a employee
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        Base response = new Base();

        //        Employee EmployeeData = new Employee();
        //        EmployeeData.FirstName = Employee.FirstName;
        //        EmployeeData.MiddleName = Employee.MiddleName;
        //        EmployeeData.LastName = Employee.LastName;
        //        //EmployeeData.EmployeeCode = Employee.EmployeeCode;
        //        EmployeeData.PersonalEmail = Employee.PersonalEmail;
        //        EmployeeData.RoleId = Employee.DepartmentId;
        //        EmployeeData.PrimaryContact = Employee.PrimaryContact;
        //        EmployeeData.SecondaryContact = Employee.SecondaryContact;
        //        EmployeeData.MaritalStatus = Employee.MaritalStatus;
        //        EmployeeData.SpouseName = Employee.SpouseName;
        //        EmployeeData.FatherName = Employee.FatherName;
        //        EmployeeData.MotherName = Employee.MotherName;
        //        EmployeeData.CreatedOn = DateTime.Now;
        //        EmployeeData.UploadResume = Employee.UploadResume;
        //        EmployeeData.DepartmentId = Employee.DepartmentId;
        //        EmployeeData.DesignationId = Employee.DesignationId;
        //        EmployeeData.DepartmentName = _db.Department.Where(x => x.DepartmentId == Employee.DepartmentId).Select(x => x.DepartmentName).FirstOrDefault();
        //        EmployeeData.DesignationName = _db.Designation.Where(x => x.DesignationId == Employee.DesignationId).Select(x => x.DesignationName).FirstOrDefault();

        //        EmployeeData.DateOfBirth = Employee.DateOfBirth;
        //        EmployeeData.IsActive = true;
        //        EmployeeData.IsDeleted = false;
        //        //if (Employee.BloodGroupId == 0)
        //        //{
        //        //    EmployeeData.BloodGroupId = 9;
        //        //}
        //        //else
        //        //{
        //        //    EmployeeData.BloodGroupId = Employee.BloodGroupId;
        //        //}

        //        EmployeeData.Document = Employee.Document;

        //        //DateTime var = Convert.ToDateTime(Employee.JoiningDate);
        //        EmployeeData.JoiningDate = Convert.ToDateTime(Employee.JoiningDate).AddDays(1);
        //        //EmployeeData.JoiningDate = Employee.JoiningDate;
        //        // EmployeeData.ConfirmationDate = Employee.ConfirmationDate;
        //        if (Employee.ConfirmationDate != null)
        //        {
        //            EmployeeData.ConfirmationDate = Convert.ToDateTime(Employee.ConfirmationDate).AddDays(1);
        //        }
        //        EmployeeData.DateOfBirth = Convert.ToDateTime(Employee.DateOfBirth).AddDays(1);
        //        EmployeeData.EmergencyNumber = Employee.EmergencyNumber;
        //        EmployeeData.WhatsappNumber = Employee.WhatsappNumber;
        //        EmployeeData.AadharNumber = Employee.AadharNumber;
        //        EmployeeData.PanNumber = Employee.PanNumber;
        //        EmployeeData.PermanentAddress = Employee.PermanentAddress;
        //        EmployeeData.LocalAddress = Employee.LocalAddress;
        //        EmployeeData.MedicalIssue = Employee.MedicalIssue;
        //        EmployeeData.ProfileImageUrl = Employee.ProfileImageUrl;
        //        EmployeeData.Salary = Employee.Salary;
        //        EmployeeData.BankAccountNumber = Employee.BankAccountNumber;
        //        EmployeeData.Password = Employee.Password;
        //        EmployeeData.OfficeEmail = Employee.OfficeEmail;
        //        EmployeeData.IFSC = Employee.IFSC;
        //        EmployeeData.AccountHolderName = Employee.AccountHolderName;
        //        EmployeeData.BankName = Employee.BankName;
        //        EmployeeData.EmployeeTypeId = Employee.EmployeeTypeId;
        //        EmployeeData.CompanyName = Employee.CompanyName;
        //        EmployeeData.CompanyId = claims.companyid;
        //        EmployeeData.OrgId = claims.orgid;
        //        EmployeeData.DisplayName = Employee.FirstName + " " + Employee.LastName;
        //        EmployeeData.ReportingManager = Employee.ReportingManager;
        //        EmployeeData.BiometricID = Employee.BiometricID;
        //        //EmployeeData.attendancenumber = employee.attendancenumber;
        //        //EmployeeData.probationenddate = datetime.now;
        //        //EmployeeData.inprobation = employee.inprobation;
        //        //EmployeeData.timetype = employee.timetype;
        //        //EmployeeData.workertype = employee.workertype;
        //        //EmployeeData.shifttype = employee.shifttype;
        //        //EmployeeData.weeklyoffpolicy = employee.weeklyoffpolicy;
        //        //EmployeeData.noticeperiodmonths = employee.noticeperiodmonths;
        //        //employeedata.paygroup = employee.paygroup;
        //        //employeedata.costcenter = employee.costcenter;
        //        EmployeeData.WorkPhone = Employee.WorkPhone;
        //        EmployeeData.ResidenceNumber = Employee.ResidenceNumber;
        //        //EmployeeData.SkypeId = Employee.SkypeId;
        //        // EmployeeData.Band = Employee.Band;
        //        EmployeeData.Gender = Employee.Gender;
        //        EmployeeData.JoiningDate = DateTime.Now;
        //        EmployeeData.ConfirmationDate = DateTime.Now;

        //        //  string[] testfiles = Employee.UploadResume.Split(new char[] { '\\' });
        //        //  var finalString = testfiles.LastOrDefault();

        //        //  Employee.UploadResume = finalString;

        //        _db.Employee.Add(EmployeeData);
        //        _db.SaveChanges();
        //        res.Status = true;
        //        res.Message = "Data Saved Successfully";

        //        var isUserExist = _db.User.Where(u => u.UserName == Employee.OfficeEmail).FirstOrDefault();
        //        if (isUserExist == null)
        //        {
        //            User UserData = new User();
        //            UserData.UserName = Employee.OfficeEmail;
        //            var Password = Employee.Password;
        //            var keynew = DataHelper.GeneratePasswords(10);
        //            var passw = DataHelper.EncodePassword(Password, keynew);
        //            UserData.Password = passw;
        //            UserData.HashCode = keynew;
        //            UserData.EmployeeId = EmployeeData.EmployeeId;
        //            UserData.DepartmentId = Employee.DepartmentId;
        //            UserData.IsDeleted = false;
        //            UserData.IsActive = true;
        //            UserData.CreatedOn = DateTime.Now;
        //            _db.User.Add(UserData);
        //        }
        //        else
        //        {
        //            isUserExist.UserName = Employee.OfficeEmail;
        //            var Password = Employee.Password;
        //            var keynew = DataHelper.GeneratePasswords(10);
        //            var passw = DataHelper.EncodePassword(Password, keynew);
        //            isUserExist.Password = passw;
        //            isUserExist.HashCode = keynew;
        //            isUserExist.EmployeeId = EmployeeData.EmployeeId;
        //            isUserExist.DepartmentId = Employee.DepartmentId;
        //            isUserExist.IsDeleted = false;
        //            isUserExist.IsActive = true;
        //            isUserExist.CreatedOn = DateTime.Now;
        //        }
        //        _db.SaveChanges();

        //        // Employee employee = new Employee();
        //        byte Levels = 4;
        //        //User Info
        //        var user = new ApplicationUser()
        //        {
        //            FirstName = Employee.FirstName,
        //            LastName = Employee.LastName,
        //            PhoneNumber = Employee.PrimaryContact,
        //            Level = Levels,
        //            JoinDate = DateTime.Now.Date,
        //            EmailConfirmed = true,
        //            Email = Employee.PersonalEmail,
        //            PasswordHash = Employee.Password,
        //            UserName = Employee.OfficeEmail
        //        };
        //        Employee.OfficeEmail = Employee.OfficeEmail;
        //        // ApplicationUserManager Ab = new ApplicationUserManager();
        //        string displayname = Employee.OfficeEmail;

        //        IdentityResult result = await this.AppUserManager.CreateAsync(user, Employee.Password);
        //        var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        //        var adminUser = manager.FindByName(displayname);
        //        var RoleData = _db.Role.Where(x => x.RoleId == Employee.RoleId).FirstOrDefault();
        //        // assign User role for super admin
        //        if (RoleData != null)
        //        {
        //            user.UserName = RoleData.RoleType;
        //        }
        //        // manager.AddToRole(adminUser.Id, RoleData.RoleType);
        //        if (!result.Succeeded)
        //        {
        //            _db.SaveChanges();
        //            res.Status = true;
        //            res.Message = "Data Saved Successfully";
        //            res.Data = EmployeeData;
        //        }

        //        //if (result.Errors.Count() > 0)
        //        //{
        //        //    IHttpActionResult errorResult1 = GetErrorResult(result);
        //        //    return Employee;
        //        //}

        //        _db.SaveChanges();
        //        res.Status = true;
        //        res.Message = "Data Saved Successfully";
        //        res.Data = EmployeeData;

        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}
        //#endregion

        //#region Create ApI In  ImportEmployee
        ///// <summary>
        ///// Create ApI In  ImportEmployee
        ///// </summary>
        ///// <param name="Employees"></param>
        ///// <param name="orgid"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("AddExcelEmployee")]
        //[Authorize]
        //public async Task<ResponseBodyModel> AddExcelEmployee(List<ImportEmployee> Employees, int orgid)
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        foreach (var Employee in Employees)
        //        {
        //            var emp = (from ad in _db.Employee where ad.CompanyId == claims.companyid && ad.OrgId == claims.orgid && ad.OfficeEmail == Employee.Moreyeahs_Mail_Id select ad).FirstOrDefault();
        //            if (emp == null)
        //            {
        //                var department = _db.Department.FirstOrDefault(x => x.DepartmentName.Trim().ToUpper() == Employee.DepartmentName.Trim().ToUpper());
        //                if (department != null)
        //                {
        //                    Employee EmployeeData = new Employee();
        //                    EmployeeData.FirstName = Employee.First_Name;
        //                    EmployeeData.LastName = Employee.Last_Name;
        //                    EmployeeData.MiddleName = Employee.Middle_Name;
        //                    EmployeeData.FatherName = Employee.Father_Name;
        //                    EmployeeData.MotherName = Employee.Mother_Name;
        //                    EmployeeData.PersonalEmail = Employee.Email;
        //                    EmployeeData.OrgId = orgid;
        //                    EmployeeData.PanNumber = Employee.Pan_Number;
        //                    EmployeeData.PermanentAddress = Employee.Permanent_Address;
        //                    EmployeeData.LocalAddress = Employee.Local_Address;
        //                    EmployeeData.PrimaryContact = Employee.Primary_Contact;
        //                    EmployeeData.MedicalIssue = Employee.Medical_Issue;
        //                    EmployeeData.BankAccountNumber = Employee.Bank_Account_Number;
        //                    EmployeeData.Password = "Moreyeahs@123";
        //                    EmployeeData.IFSC = Employee.IFSC;
        //                    EmployeeData.AccountHolderName = Employee.Account_Holder_Name;
        //                    EmployeeData.BankName = Employee.Bank_Name;
        //                    EmployeeData.OfficeEmail = Employee.Moreyeahs_Mail_Id;
        //                    EmployeeData.ReportingManager = Employee.ReportingManager;
        //                    EmployeeData.BiometricID = Employee.BiometricID;
        //                    EmployeeData.DepartmentName = department.DepartmentName;
        //                    EmployeeData.DesignationName = Employee.DesignationName;
        //                    EmployeeData.DepartmentId = department.DepartmentId;
        //                    EmployeeData.JoiningDate = (DateTime)Employee.Joining_Date;
        //                    EmployeeData.ConfirmationDate = (DateTime)Employee.Confirmation_Date;
        //                    EmployeeData.DateOfBirth = (DateTime)Employee.DateOfBirth;
        //                    //EmployeeData.BloodGroup = Enum.Parse(typeof(BloodGroupEnum), Employee.Blood_Group);
        //                    EmployeeData.CompanyName = Employee.Company_Name;
        //                    EmployeeData.WhatsappNumber = Employee.Whatsapp_Number;
        //                    // EmployeeData.EmergencyNumber = Employee.Emergency_Number;
        //                    EmployeeData.CreatedOn = DateTime.Now;
        //                    EmployeeData.CompanyId = claims.companyid;
        //                    EmployeeData.IsActive = true;
        //                    EmployeeData.IsDeleted = false;
        //                    _db.Employee.Add(EmployeeData);
        //                    _db.SaveChanges();
        //                    res.Message = "Added All Employee";
        //                    res.Status = true;

        //                    //EmployeeData.Document = Employee.Document;

        //                    //DateTime var = Convert.ToDateTime(Employee.JoiningDate);
        //                    EmployeeData.JoiningDate = Convert.ToDateTime(Employee.Joining_Date).AddDays(1);
        //                    //EmployeeData.JoiningDate = Employee.JoiningDate;
        //                    // EmployeeData.ConfirmationDate = Employee.ConfirmationDate;
        //                    if (Employee.Confirmation_Date != null)
        //                    {
        //                        EmployeeData.ConfirmationDate = Convert.ToDateTime(Employee.Confirmation_Date).AddDays(1);
        //                    }
        //                    EmployeeData.DateOfBirth = (DateTime)Employee.DateOfBirth;
        //                    EmployeeData.FirstName = Employee.First_Name;
        //                    EmployeeData.LastName = Employee.Last_Name;
        //                    EmployeeData.MiddleName = Employee.Middle_Name;
        //                    EmployeeData.FatherName = Employee.Father_Name;
        //                    EmployeeData.MotherName = Employee.Mother_Name;
        //                    EmployeeData.PersonalEmail = Employee.Email;
        //                    EmployeeData.OrgId = orgid;
        //                    EmployeeData.PanNumber = Employee.Pan_Number;
        //                    EmployeeData.PermanentAddress = Employee.Permanent_Address;
        //                    EmployeeData.LocalAddress = Employee.Local_Address;
        //                    EmployeeData.MedicalIssue = Employee.Medical_Issue;
        //                    EmployeeData.BankAccountNumber = Employee.Bank_Account_Number;
        //                    EmployeeData.Password = "Moreyeahs@123";
        //                    EmployeeData.IFSC = Employee.IFSC;
        //                    EmployeeData.AccountHolderName = Employee.Account_Holder_Name;
        //                    EmployeeData.BankName = Employee.Bank_Name;
        //                    EmployeeData.OfficeEmail = Employee.Moreyeahs_Mail_Id;
        //                    EmployeeData.ReportingManager = Employee.ReportingManager;
        //                    EmployeeData.PrimaryContact = Employee.Primary_Contact;
        //                    EmployeeData.BiometricID = Employee.BiometricID;
        //                    //EmployeeData.BloodGroup = Employee.Blood_Group;
        //                    EmployeeData.CompanyName = Employee.Company_Name;
        //                    EmployeeData.WhatsappNumber = Employee.Whatsapp_Number;
        //                    //  EmployeeData.EmergencyNumber = Employee.Emergency_Number;
        //                    //EmployeeData.DepartmentId = Employee.DepartmentId;
        //                    //EmployeeData.attendancenumber = employee.attendancenumber;
        //                    //EmployeeData.probationenddate = datetime.now;
        //                    //EmployeeData.inprobation = employee.inprobation;
        //                    //EmployeeData.timetype = employee.timetype;
        //                    //EmployeeData.workertype = employee.workertype;
        //                    //EmployeeData.shifttype = employee.shifttype;
        //                    //EmployeeData.weeklyoffpolicy = employee.weeklyoffpolicy;
        //                    //EmployeeData.noticeperiodmonths = employee.noticeperiodmonths;
        //                    //employeedata.paygroup = employee.paygroup;
        //                    //employeedata.costcenter = employee.costcenter;
        //                    // EmployeeData.WorkNumber = Employee.WorkNumber;
        //                    // EmployeeData.ResidenceNumber = Employee.ResidenceNumber;
        //                    //EmployeeData.SkypeId = Employee.SkypeId;
        //                    // EmployeeData.Band = Employee.Band;
        //                    // EmployeeData.Gender = Employee.Gender;
        //                    EmployeeData.JoiningDate = DateTime.Now;
        //                    EmployeeData.ConfirmationDate = DateTime.Now;

        //                    //  string[] testfiles = Employee.UploadResume.Split(new char[] { '\\' });
        //                    //  var finalString = testfiles.LastOrDefault();

        //                    //  Employee.UploadResume = finalString;

        //                    //_db.Employee.Add(EmployeeData);
        //                    _db.Entry(EmployeeData).State = System.Data.Entity.EntityState.Modified;
        //                    _db.SaveChanges();
        //                    res.Status = true;
        //                    res.Message = "Data Saved Successfully";
        //                    var isUserExist = _db.User.Where(u => u.UserName == EmployeeData.OfficeEmail).FirstOrDefault();
        //                    if (isUserExist == null)
        //                    {
        //                        User UserData = new User();
        //                        UserData.UserName = EmployeeData.OfficeEmail;
        //                        var Password = EmployeeData.Password;
        //                        var keynew = DataHelper.GeneratePasswords(10);
        //                        var passw = DataHelper.EncodePassword(Password, keynew);
        //                        UserData.Password = passw;
        //                        UserData.HashCode = keynew;
        //                        UserData.EmployeeId = EmployeeData.EmployeeId;
        //                        UserData.DepartmentId = department.DepartmentId;
        //                        UserData.CompanyId = EmployeeData.CompanyId;
        //                        UserData.OrgId = EmployeeData.OrgId;
        //                        UserData.IsDeleted = false;
        //                        UserData.IsActive = true;
        //                        UserData.CreatedOn = DateTime.Now;
        //                        _db.User.Add(UserData);
        //                    }
        //                    else
        //                    {
        //                        isUserExist.UserName = EmployeeData.OfficeEmail;
        //                        var Password = EmployeeData.Password;
        //                        var keynew = DataHelper.GeneratePasswords(10);
        //                        var passw = DataHelper.EncodePassword(Password, keynew);
        //                        isUserExist.Password = passw;
        //                        isUserExist.HashCode = keynew;
        //                        isUserExist.EmployeeId = EmployeeData.EmployeeId;
        //                        isUserExist.DepartmentId = department.DepartmentId;
        //                        isUserExist.CompanyId = EmployeeData.CompanyId;
        //                        isUserExist.OrgId = EmployeeData.OrgId;
        //                        isUserExist.IsDeleted = false;
        //                        isUserExist.IsActive = true;
        //                        isUserExist.CreatedOn = DateTime.Now;
        //                        _db.Entry(isUserExist).State = System.Data.Entity.EntityState.Modified;
        //                    }
        //                    _db.SaveChanges();
        //                    byte Levels = 4;
        //                    //User Info
        //                    var user = new ApplicationUser()
        //                    {
        //                        FirstName = EmployeeData.FirstName,
        //                        LastName = EmployeeData.LastName,
        //                        PhoneNumber = EmployeeData.PrimaryContact,
        //                        Level = Levels,
        //                        JoinDate = DateTime.Now.Date,
        //                        EmailConfirmed = true,
        //                        Email = EmployeeData.OfficeEmail,
        //                        PasswordHash = EmployeeData.Password,
        //                        UserName = EmployeeData.OfficeEmail
        //                    };
        //                    Employee.Moreyeahs_Mail_Id = EmployeeData.OfficeEmail;
        //                    // ApplicationUserManager Ab = new ApplicationUserManager();
        //                    string displayname = EmployeeData.OfficeEmail;

        //                    IdentityResult result = await this.AppUserManager.CreateAsync(user, EmployeeData.Password);
        //                    var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        //                    var adminUser = manager.FindByName(displayname);
        //                    //   var RoleData = _db.Role.Where(x => x.RoleId == Employee.RoleId).FirstOrDefault();
        //                    // assign User role for super admin
        //                    //if (RoleData != null)
        //                    //{
        //                    //    user.UserName = RoleData.RoleType;
        //                    //}
        //                    // manager.AddToRole(adminUser.Id, RoleData.RoleType);
        //                    if (result.Succeeded)
        //                    {
        //                        res.Status = true;
        //                        res.Message = "Data Saved Successfully";
        //                        res.Data = EmployeeData;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;

        //}

        //#endregion

        //#region This Api use By Get Department

        ///// <summary>
        ///// Created By Ankit
        ///// </summary>
        //[Route("GetDepartment")]
        //[HttpGet]
        //public async Task<ResponseBodyModel> GetDepartment()

        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        var FinalData = (from E in _db.Employee
        //                         join R in _db.Department on E.RoleId equals R.DepartmentId
        //                         where R.IsDeleted == false
        //                         select new
        //                         {
        //                             EmployeeId = E.EmployeeId,
        //                             DepartmentName = R.DepartmentName,

        //                         }).ToList();
        //        if (FinalData != null)
        //        {
        //            res.Status = true;
        //            res.Message = "Department data list Found";
        //            res.Data = FinalData;
        //        }
        //        else
        //        {
        //            res.Status = false;
        //            res.Message = "data not found";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //        return res;
        //    }
        //    return res;
        //}

        //#endregion

        //#region This Api USe GetEmployeeById
        ///// <summary>
        ///// Created By Ankit USe GetEmployeeById
        ///// </summary>
        ///// <param name="Id"></param>
        ///// <returns></returns>
        //[Route("GetEmployeeById")]
        //[HttpGet]
        //[Authorize]
        //public async Task<ResponseBodyModel> GetEmployeeById(int Id)
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        List<EmployeeList> employeeList = new List<EmployeeList>();
        //        var EmployeeData = _db.Employee.Where(x => x.EmployeeId == Id && x.IsDeleted == false && x.CompanyId == claims.companyid).ToList();
        //        foreach (var Item in EmployeeData)
        //        {
        //            EmployeeList Emp = new EmployeeList();
        //            Emp.EmployeeId = Item.EmployeeId;
        //            Emp.FatherName = Item.FatherName;
        //            Emp.LastName = Item.LastName;
        //            Emp.FirstName = Item.FirstName;
        //            Emp.MedicalIssue = Item.MedicalIssue;
        //            Emp.OfficeEmail = Item.OfficeEmail;
        //            Emp.LocalAddress = Item.LocalAddress;
        //            Emp.PanNumber = Item.PanNumber;
        //            Emp.PermanentAddress = Item.PermanentAddress;
        //            Emp.PrimaryContact = Item.PrimaryContact;
        //            Emp.AadharNumber = Item.AadharNumber;
        //            Emp.IFSC = Item.IFSC;
        //            Emp.BankAccountNumber = Item.BankAccountNumber;
        //            //Emp.BloodGroup = Item.BloodGroup;
        //            Emp.DateOfBirth = Item.DateOfBirth;
        //            Emp.MiddleName = Item.MiddleName;
        //            Emp.MaritalStatus = Item.MaritalStatus;
        //            Emp.MedicalIssue = Item.MedicalIssue;
        //            Emp.MotherName = Item.MotherName;
        //            Emp.AadharNumber = Item.MotherName;
        //            Emp.AccountHolderName = Item.AccountHolderName;

        //            employeeList.Add(Emp);
        //        }
        //        if (EmployeeData.Count != 0)
        //        {
        //            res.Status = true;
        //            res.Message = "Employee list Found";
        //            res.Data = EmployeeData;
        //        }
        //        else
        //        {
        //            res.Status = false;
        //            res.Message = "No Employee list Found";
        //            res.Data = EmployeeData;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}
        //#endregion

        //#region Api To Edit EmployeeDetails
        ///// <summary>
        ///// APi >> Post >> api/EmployeeDetails
        ///// Created By Ankit
        ///// </summary>
        ///// <returns></returns>
        //[Authorize]
        //[HttpPut]
        //[Route("editAllEmployeeDetails")]
        //public async Task<ResponseBodyModel> EditAllEmployeeDetails(Employee model)
        //{
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        if (model != null)
        //        {
        //            var employee = _db.Employee.Where(x => x.EmployeeId == model.EmployeeId && x.IsActive == true
        //                       && x.IsDeleted == false).FirstOrDefault();
        //            if (employee == null)
        //            {
        //                Employee obj = new Employee();
        //                employee.EmployeeId = model.EmployeeId;
        //                // employee.CreateDate = DateTime.Now;
        //                _db.Employee.Add(employee);
        //                await _db.SaveChangesAsync();
        //            }

        //            var empDoc = employee;
        //            empDoc.FirstName = String.IsNullOrEmpty(model.FirstName) ? empDoc.FirstName : model.FirstName;
        //            empDoc.MiddleName = String.IsNullOrEmpty(model.MiddleName) ? empDoc.MiddleName : model.MiddleName;
        //            empDoc.LastName = String.IsNullOrEmpty(model.LastName) ? empDoc.LastName : model.LastName;
        //            empDoc.DisplayName = String.IsNullOrEmpty(model.DisplayName) ? empDoc.DisplayName : model.DisplayName;
        //            empDoc.Gender = String.IsNullOrEmpty(model.Gender) ? empDoc.Gender : model.Gender;
        //            empDoc.DateOfBirth = model.DateOfBirth == null ? empDoc.DateOfBirth : model.DateOfBirth;
        //            empDoc.MaritalStatus = String.IsNullOrEmpty(model.MaritalStatus) ? empDoc.MaritalStatus : model.MaritalStatus;
        //            //empDoc.BloodGroup = String.IsNullOrEmpty(model.BloodGroup) ? empDoc.BloodGroup : model.BloodGroup;
        //            empDoc.PersonalEmail = String.IsNullOrEmpty(model.PersonalEmail) ? empDoc.PersonalEmail : model.PersonalEmail;
        //            empDoc.OfficeEmail = model.OfficeEmail == null ? empDoc.OfficeEmail : model.OfficeEmail;
        //            empDoc.MobilePhone = String.IsNullOrEmpty(model.MobilePhone) ? empDoc.MobilePhone : model.MobilePhone;
        //            empDoc.WorkPhone = String.IsNullOrEmpty(model.WorkPhone) ? empDoc.WorkPhone : model.WorkPhone;
        //            empDoc.ResidentPhone = String.IsNullOrEmpty(model.ResidentPhone) ? empDoc.ResidentPhone : model.ResidentPhone;
        //            empDoc.SkypeMail = String.IsNullOrEmpty(model.SkypeMail) ? empDoc.SkypeMail : model.SkypeMail;
        //            empDoc.CurrentAddress = String.IsNullOrEmpty(model.CurrentAddress) ? empDoc.CurrentAddress : model.CurrentAddress;
        //            empDoc.PermanentAddress = String.IsNullOrEmpty(model.PermanentAddress) ? empDoc.PermanentAddress : model.PermanentAddress;
        //            empDoc.City = String.IsNullOrEmpty(model.City) ? empDoc.City : model.City;
        //            empDoc.State = String.IsNullOrEmpty(model.State) ? empDoc.State : model.State;
        //            empDoc.Country = String.IsNullOrEmpty(model.Country) ? empDoc.Country : model.Country;
        //            empDoc.Pincode = String.IsNullOrEmpty(model.Pincode) ? empDoc.Pincode : model.Pincode;
        //            empDoc.PersonalEmail = String.IsNullOrEmpty(model.PersonalEmail) ? empDoc.PersonalEmail : model.PersonalEmail;
        //            empDoc.Summary = String.IsNullOrEmpty(model.Summary) ? empDoc.Summary : model.Summary;
        //            empDoc.AddressLine1 = String.IsNullOrEmpty(model.AddressLine1) ? empDoc.AddressLine1 : model.AddressLine1;
        //            empDoc.AddressLine2 = String.IsNullOrEmpty(model.AddressLine2) ? empDoc.AddressLine2 : model.AddressLine2;

        //            empDoc.UpdatedOn = DateTime.Now;
        //            _db.Entry(employee).State = System.Data.Entity.EntityState.Modified;
        //            _db.SaveChanges();

        //            res.Message = "Employee Details Edited successfuly";
        //            res.Status = true;
        //            res.Data = employee;
        //        }

        //        else
        //        {
        //            res.Message = "Model Is Empty";
        //            res.Status = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}
        //#endregion

        //#region Api To Get Employee Document
        ///// <summary>
        /////     this api use all the document deails show create by ankit
        ///// </summary>
        ///// <returns></returns>
        //[Authorize]
        //[HttpGet]
        //[Route("getempDocument")]
        //public async Task<ResponseBodyModel> GetEmployeeDocument()
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    EmployeeDocumentModel response = new EmployeeDocumentModel();
        //    DocumentDetailsHelper doc = new DocumentDetailsHelper();
        //    DocumentDetailsDrivingLicenseHelper ddl = new DocumentDetailsDrivingLicenseHelper();
        //    DocumentPANCardHelper pcr = new DocumentPANCardHelper();
        //    DocumentPassportHelper pass = new DocumentPassportHelper();
        //    AadhaarHelper aar = new AadhaarHelper();
        //    VoterCardHelper vot = new VoterCardHelper();
        //    SignatureHelper sig = new SignatureHelper();
        //    ExperienceHelper Exp = new ExperienceHelper();

        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        var empDocumentData = await _db.EmpDoc.FirstOrDefaultAsync(x => x.EmployeeId == claims.userid &&
        //                    x.CompanyId == claims.companyid && x.OrgId == claims.orgid);

        //        if (empDocumentData != null)
        //        {
        //            doc.Branch = empDocumentData.Branch;
        //            doc.Degree = empDocumentData.Degree;
        //            // doc.DateOfJoining = empDocumentData.DateOfJoining;
        //            //doc.DateOfCompleation = empDocumentData.DateOfCompleation;
        //            doc.PerctOrCGPA = empDocumentData.PerctOrCGPA;
        //            doc.UniversityOrCollage = empDocumentData.UniversityOrCollage;
        //            doc.DegreeUpload = empDocumentData.DegreeUpload;
        //            doc.Checked = empDocumentData.Checked;
        //            response.DocumentDetails = doc;

        //            ddl.Licensenumber = empDocumentData.Licensenumber;
        //            ddl.DateOfBirthOnDriving = empDocumentData.DateOfBirthOnDriving;
        //            ddl.NameOnDriving = empDocumentData.NameOnDriving;
        //            ddl.FatherHusbandNameOnDriving = empDocumentData.FatherHusbandNameOnDriving;
        //            ddl.ExpireOnLicense = empDocumentData.ExpireOnLicense;
        //            ddl.DrivingLicenseUpload = empDocumentData.DrivingLicenseUpload;
        //            ddl.Checked = empDocumentData.Checked;
        //            response.DrivingLicenseDetails = ddl;

        //            pcr.PanNumber = empDocumentData.PanNumber;
        //            pcr.NameOnPan = empDocumentData.NameOnPan;
        //            pcr.FatherNameOnPan = empDocumentData.FatherNameOnPan;
        //            pcr.DateOfBirthDateOnPan = empDocumentData.DateOfBirthDateOnPan;
        //            pcr.PanUpload = empDocumentData.PanUpload;
        //            pcr.Checked = empDocumentData.Checked;
        //            response.PanDetails = pcr;

        //            pass.FullName = empDocumentData.FullName;
        //            pass.FatherName = empDocumentData.FatherName;
        //            //pass.DateOfBirth = empDocumentData.DateOfBirth;
        //            pass.DateOfIssue = empDocumentData.DateOfIssue;
        //            pass.ExpiresOn = empDocumentData.ExpiresOn;
        //            pass.PlaceofBirth = empDocumentData.PlaceOfBirth;
        //            pass.PlaceOfIssue = empDocumentData.PlaceOfIssue;
        //            pass.PassportNumber = empDocumentData.PassportNumber;
        //            pass.PassportUpload = empDocumentData.PassportUpload;
        //            pass.Address = empDocumentData.Address;
        //            pass.Checked = empDocumentData.Checked;
        //            response.PassportDetails = pass;

        //            aar.AadhaarCardNumber = empDocumentData.AadhaarCardNumber;
        //            aar.AddressOnAadhaar = empDocumentData.AddressOnAadhaar;
        //            aar.DateOfBirthOnAadhaar = empDocumentData.DateOfBirthOnAadhaar;
        //            aar.FatherHusbandNameOnAadhaar = empDocumentData.FatherHusbandNameOnAadhaar;
        //            aar.NameOnAadhaar = empDocumentData.NameOnAadhaar;
        //            aar.AadhaarUpload = empDocumentData.AadhaarUpload;
        //            aar.Checked = empDocumentData.Checked;
        //            response.AadhaarDetails = aar;

        //            vot.VoterUpload = empDocumentData.VoterUpload;
        //            vot.DateOfBirthOnVoterId = empDocumentData.DateOfBirthOnVoterId;
        //            vot.FatherHusbandNameOnVoter = empDocumentData.FatherHusbandNameOnVoter;
        //            vot.NameOnVoterId = empDocumentData.NameOnVoterId;
        //            vot.VoterIdNumber = empDocumentData.VoterIdNumber;
        //            vot.Checked = empDocumentData.Checked;
        //            response.VoterCardDetails = vot;

        //            sig.Name = empDocumentData.Name;
        //            sig.Designation = empDocumentData.Designation;
        //            sig.SignatureUpload = empDocumentData.SignatureUpload;
        //            sig.Checked = empDocumentData.Checked;
        //            response.SignatureDetails = sig;

        //            Exp.CompanyName = empDocumentData.CompanyName;
        //            Exp.DescriptionExperience = empDocumentData.DescriptionExperience;
        //            Exp.ExperienceUpload = empDocumentData.ExperienceUpload;
        //            Exp.JobTitle = empDocumentData.JobTitle;
        //            Exp.JoiningDateExperience = empDocumentData.JoiningDateExperience;
        //            Exp.LocationExperience = empDocumentData.LocationExperience;
        //            Exp.Checked = empDocumentData.Checked;
        //            Exp.RelievoingDateExperience = empDocumentData.RelievoingDateExperience;
        //            response.ExperienceDetails = Exp;

        //            res.Message = "Employee Data Found";
        //            res.Status = true;
        //            res.Data = response;

        //        }

        //        else
        //        {
        //            res.Message = "Employee Data Not Found";
        //            res.Status = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        //#endregion

        ////#region This Api Use For Add UserData
        /////// <summary>
        /////// This Api created By ankit 30/04/2022
        /////// </summary>
        /////// <param name="employee"></param>
        /////// <returns></returns>
        ////[HttpPost]
        ////[Route("addUser")]
        ////public async Task<ResponseBodyModel> AddUserData(Employee model)
        ////{
        ////    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        ////    ResponseBodyModel res = new ResponseBodyModel();
        ////    try
        ////    {
        ////        if (model == null)
        ////        {
        ////            res.Message = "Model is Unvalid";
        ////            res.Status = false;
        ////        }
        ////        else
        ////        {
        ////            Employee obj = new Employee
        ////            {
        ////                FirstName = model.FirstName,
        ////                LastName = model.LastName,
        ////                MobilePhone = model.MobilePhone,
        ////                PersonalEmail = model.PersonalEmail,
        ////                WorkEmail = model.WorkEmail,
        ////                AddressLine1 = model.AddressLine1,
        ////                CompanyName = model.CompanyName,
        ////                MaritalStatus = model.MaritalStatus,
        ////                DateOfBirth = model.DateOfBirth,
        ////                Gender = model.Gender,
        ////                Password = model.Password,
        ////                DepartmentName = model.DepartmentName,
        ////                DesignationName = model.DesignationName,
        ////                CreatedBy = claims.employeeid,
        ////                CreatedOn = DateTime.Now,
        ////                IsActive = true,
        ////                IsDeleted = false,
        ////                CompanyId = claims.companyid,
        ////                OrgId = claims.orgid,
        ////            };
        ////            _db.Employee.Add(obj);
        ////            await _db.SaveChangesAsync();
        ////            res.Message = "User Added";
        ////            res.Status = true;
        ////            res.Data = obj;
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        res.Message = ex.Message;
        ////        res.Status = false;
        ////    }
        ////    return res;
        ////}

        ////#endregion

        //#region Helper Region Model

        //public class EmployeeProfileModel
        //{
        //    public PrimaryDetailsHelper PrimaryDetails { get; set; }
        //    public ContactDetailsHelper ContactDetail { get; set; }
        //    public CurrentAddress AddressDetail { get; set; }
        //    public ProfessionalSummaryHelper ProSummary { get; set; }
        //}
        //public class PrimaryDetailsHelper
        //{
        //    public string FirstName { get; set; }
        //    public string MiddleName { get; set; }
        //    public string LastName { get; set; }
        //    public string DisplayName { get; set; }
        //    public string Gender { get; set; }
        //    public DateTime? DateOfBirth { get; set; }
        //    public string MaritalStatus { get; set; }
        //    public string BloodGroup { get; set; }
        //    public bool Handicapped { get; set; }
        //}
        //public class ContactDetailsHelper
        //{
        //    public string WorkEmail { get; set; }
        //    public string PersonalEmail { get; set; }
        //    public string MobilePhone { get; set; }
        //    public string WorkPhone { get; set; }
        //    public string ResidentPhone { get; set; }
        //    public string SkypeMail { get; set; }
        //}
        //public class CurrentAddress
        //{
        //    public string AddressLine1 { get; set; }
        //    public string AddressLine2 { get; set; }
        //    public string City { get; set; }
        //    public string State { get; set; }
        //    public string Country { get; set; }
        //    public string PinCode { get; set; }
        //}

        //public class ProfessionalSummaryHelper
        //{
        //    public string Summary { get; set; }
        //}
        //public class EmployeeDocumentModel
        //{
        //    public DocumentDetailsHelper DocumentDetails { get; set; }
        //    public DocumentDetailsDrivingLicenseHelper DrivingLicenseDetails { get; set; }
        //    public DocumentPANCardHelper PanDetails { get; set; }
        //    public DocumentPassportHelper PassportDetails { get; set; }
        //    public AadhaarHelper AadhaarDetails { get; set; }
        //    public VoterCardHelper VoterCardDetails { get; set; }
        //    public SignatureHelper SignatureDetails { get; set; }
        //    public ExperienceHelper ExperienceDetails { get; set; }

        //}
        //public class DocumentDetailsHelper
        //{
        //    public string Branch { get; set; }
        //    public string Degree { get; set; }
        //    public DateTime? DateOfJoining { get; set; }
        //    public DateTime? DateOfCompleation { get; set; }
        //    public string PerctOrCGPA { get; set; }
        //    public string UniversityOrCollage { get; set; }
        //    public string DegreeUpload { get; set; }
        //    public bool Checked { get; set; }
        //}
        //public class DocumentDetailsDrivingLicenseHelper
        //{
        //    public string Licensenumber { get; set; }
        //    public DateTime? DateOfBirthOnDriving { get; set; }
        //    public string NameOnDriving { get; set; }
        //    public string FatherHusbandNameOnDriving { get; set; }
        //    public DateTime? ExpireOnLicense { get; set; }
        //    public string DrivingLicenseUpload { get; set; }
        //    public bool Checked { get; set; }
        //}
        //public class DocumentPANCardHelper
        //{
        //    public string PanNumber { get; set; }
        //    public string NameOnPan { get; set; }
        //    public DateTime? DateOfBirthDateOnPan { get; set; }
        //    public string FatherNameOnPan { get; set; }
        //    public string PanUpload { get; set; }
        //    public bool Checked { get; set; }
        //}
        //public class DocumentPassportHelper
        //{
        //    public string PassportNumber { get; set; }
        //    public DateTime? DateOfBirth { get; set; }
        //    public string FullName { get; set; }
        //    public string FatherName { get; set; }
        //    public DateTime? DateOfIssue { get; set; }
        //    public string PlaceOfIssue { get; set; }
        //    public string PlaceofBirth { get; set; }
        //    public DateTime? ExpiresOn { get; set; }
        //    public string Address { get; set; }
        //    public string PassportUpload { get; set; }
        //    public bool Checked { get; set; }
        //}
        //public class AadhaarHelper
        //{
        //    public string AadhaarCardNumber { get; set; }
        //    public DateTime? DateOfBirthOnAadhaar { get; set; }
        //    public string NameOnAadhaar { get; set; }
        //    public string FatherHusbandNameOnAadhaar { get; set; }
        //    public string AddressOnAadhaar { get; set; }
        //    public string AadhaarUpload { get; set; }
        //    public bool Checked { get; set; }

        //}
        //public class VoterCardHelper
        //{
        //    public string VoterIdNumber { get; set; }
        //    public DateTime? DateOfBirthOnVoterId { get; set; }
        //    public string NameOnVoterId { get; set; }
        //    public string FatherHusbandNameOnVoter { get; set; }
        //    public string VoterUpload { get; set; }
        //    public bool Checked { get; set; }
        //}
        //public class SignatureHelper
        //{
        //    public string Name { get; set; }
        //    public string Designation { get; set; }
        //    public string SignatureUpload { get; set; }
        //    public bool Checked { get; set; }

        //}
        //public class ExperienceHelper
        //{
        //    public string CompanyName { get; set; }
        //    public string JobTitle { get; set; }
        //    public DateTime? JoiningDateExperience { get; set; }
        //    public DateTime? RelievoingDateExperience { get; set; }
        //    public string LocationExperience { get; set; }
        //    public string DescriptionExperience { get; set; }
        //    public string ExperienceUpload { get; set; }
        //    public bool Checked { get; set; }

        //}
        //#endregion
    }
}

//[Route("GetEmployeeById")]
//[Authorize]
//public IHttpActionResult GetEmployee()
//{
//    var identity = User.Identity as ClaimsIdentity;
//    int userid = 0;

//    int compid = 0;
//    int orgid = 0;
//    // Access claims

//    if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "userid"))
//        userid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "userid").Value);
//    if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "compid"))
//        compid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "compid").Value);
//    if (identity != null && identity.Claims != null && identity.Claims.Any(x => x.Type == "orgid"))
//        orgid = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "orgid").Value);

//    List<EmployeeData> employeeDataList = new List<EmployeeData>();
//    var EmployeeData = (from ad in db.Employee
//                        join bd in db.Role on ad.RoleId equals bd.RoleId
//                        join cd in db.EmployeeType on ad.EmployeeTypeID equals cd.EmployeeTypeId
//                        join fd in db.BloodGroup on ad.BloodGroupId equals fd.BloodGroupId
//                        //join gd in db.Department on ad.DepartmentId equals gd.DepartmentId
//                        where ad.EmployeeId == userid && ad.IsDeleted == false && ad.CompanyId == compid && ad.OrgId == orgid
//                        select new
//                        {
//                            ad.EmployeeId,
//                            v = ad.FirstName + ad.LastName,
//                            ad.Email,
//                            ad.FirstName,
//                            ad.LastName,
//                            ad.PrimaryContact,
//                            ad.MaritalStatus,
//                            ad.SpouseName,
//                            ad.FatherName,
//                            ad.MotherName,
//                            fd.BloodGroupType,
//                            ad.Document,
//                            bd.RoleType,
//                            ad.Password,
//                            ad.JoiningDate,
//                            ad.ConfirmationDate,
//                            ad.DOB,
//                            ad.EmergencyNumber,
//                            ad.WhatsappNumber,
//                            ad.AadharNumber,
//                            ad.PanNumber,
//                            ad.PermanentAddress,
//                            ad.LocalAddress,
//                            ad.MedicalIssue,
//                            ad.Profile,
//                            ad.Salary,
//                            ad.BankAccountNumber,
//                            ad.IFSC,
//                            ad.AccountHolderName,
//                            ad.BankName,
//                            ad.OfficeEmail,
//                            cd.EmployeeTypes,
//                            ad.CompanyName,
//                            ad.BloodGroupId,
//                            ad.RoleId,
//                            ad.EmployeeTypeID,
//                            ad.CompanyId,
//                            ad.uploadResume,
//                            ad.DepartmentId,
//                            //gd.DepartmentName,
//                            ad.Gender,
//                            ad.AboutMeRemark,
//                            ad.AboutMyJobRemark,
//                            ad.InterestAndHobbiesRemark,
//                            //
//                            ad.DisplayName,
//                            ad.BiometricID,
//                            ad.AttendanceNumber,
//                            ad.PayGroup,
//                            ad.SkypeId,
//                            ad.Band,
//                            ad.SecondaryJobTitle,
//                            ad.ProbationEndDate,
//                            ad.ReportingManager,
//                            ad.WeeklyOffPolicy,
//                            ad.ResidenceNumber,
//                            ad.TimeType,
//                            ad.WorkerType,
//                            ad.ShiftType,
//                            ad.NoticePeriodMonths,
//                            ad.CostCenter,
//                            ad.WorkNumber,
//                        }).ToList();
//    foreach (var item in EmployeeData)
//    {
//        EmployeeData data = new EmployeeData();
//        data.EmployeeId = item.EmployeeId;
//        data.FullName = item.v;
//        data.Email = item.Email;
//        data.PrimaryContact = item.PrimaryContact;
//        data.MaritalStatus = item.MaritalStatus;
//        data.SpouseName = item.SpouseName;
//        data.FatherName = item.FatherName;
//        data.MotherName = item.MotherName;
//        data.BloodGroup = item.BloodGroupType;
//        data.Document = item.Document;
//        data.RoleType = item.RoleType;
//        data.FirstName = item.FirstName;
//        data.LastName = item.LastName;
//        data.Password = item.Password;
//        data.UploadResume = item.uploadResume;
//        data.JoiningDate = item.JoiningDate;
//        data.ConfirmationDate = item.ConfirmationDate;
//        data.DOB = item.DateOfBirth;
//        data.EmergencyNumber = item.EmergencyNumber;
//        data.WhatsappNumber = item.WhatsappNumber;
//        data.AadharNumber = item.AadharNumber;
//        data.PanNumber = item.PanNumber;
//        data.PermanentAddress = item.PermanentAddress;
//        data.LocalAddress = item.LocalAddress;
//        data.MedicalIssue = item.MedicalIssue;
//        data.Profile = item.Profile;
//        data.Salary = item.Salary;
//        data.BankAccountNumber = item.BankAccountNumber;
//        data.IFSC = item.IFSC;
//        data.AccountHolderName = item.AccountHolderName;
//        data.BankName = item.BankName;
//        data.OfficeEmail = item.OfficeEmail;
//        data.EmployeeType = item.EmployeeTypes;
//        data.CompanyName = item.CompanyName;
//        data.BloodGroupId = item.BloodGroupId;
//        data.RoleId = item.RoleId;
//        data.EmployeeTypeID = item.EmployeeTypeID;
//        data.CompanyId = item.CompanyId;
//        data.DepartmentId = item.DepartmentId;
//        //data.DepartmentName = item.DepartmentName;
//        data.AboutMeRemark = item.AboutMeRemark;
//        data.AboutMyJobRemark = item.AboutMyJobRemark;
//        data.InterestAndHobbiesRemark = item.InterestAndHobbiesRemark;
//        data.Gender = item.Gender;
//        //Newly added
//        data.DisplayName = item.DisplayName;
//        data.BiometricID = item.BiometricID;
//        data.AttendanceNumber = item.AttendanceNumber;
//        data.PayGroup = item.PayGroup;
//        data.SkypeId = item.SkypeId;
//        data.Band = item.Band;
//        data.SecondaryJobTitle = item.SecondaryJobTitle;
//        data.ProbationEndDate = DateTime.Now;
//        data.ReportingManager = item.ReportingManager;
//        data.WeeklyOffPolicy = item.WeeklyOffPolicy;
//        data.ResidenceNumber = item.ResidenceNumber;
//        data.TimeType = item.TimeType;
//        data.WorkerType = item.WorkerType;
//        data.ShiftType = item.ShiftType;
//        data.NoticePeriodMonths = item.NoticePeriodMonths;
//        data.CostCenter = item.CostCenter;
//        data.WorkNumber = item.WorkNumber;
//        employeeDataList.Add(data);
//    }
//    return Ok(employeeDataList);
//}