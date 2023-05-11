//using AspNetIdentity.WebApi.Helper;
//using AspNetIdentity.WebApi.Infrastructure;
//using AspNetIdentity.WebApi.Models;
//using Aspose.Html;
//using Aspose.Html.Converters;
//using Aspose.Html.Drawing;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Http;
//using static AspNetIdentity.WebApi.Model.EnumClass;

//namespace AspNetIdentity.WebApi.Controllers.Payroll
//{
//    /// <summary>
//    /// Created By Harshit Mitra on 08-06-2022
//    /// </summary>
//    [RoutePrefix("api/payrollcentral")]
//    public class PayRollCentralController : ApiController
//    {
//        private readonly ApplicationDbContext _db = new ApplicationDbContext();

//        #region Api To Get Pay Slip Of Employee

//        /// <summary>
//        /// Created By Harshit Mitra On 14-06-2022
//        /// API >> api/payrollcentral/getemplpayslip
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getemplpayslip")]
//        public async Task<ResponseBodyModel> GetEmployeePaySlip(int employeeId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var employee = await _db.Employee.FirstOrDefaultAsync(x => x.IsActive == true && x.IsDeleted == false &&
//                            x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee && x.EmployeeId == employeeId);
//                if (employee != null)
//                {
//                    var data = PayRollCentralHelper.Calculate(employee.StructureId, employee.GrossSalery);
//                    res.Message = "Success";
//                    res.Status = true;
//                    res.Data = data;
//                }
//                else
//                {
//                    res.Message = "Employee Not Found";
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

//        #endregion Api To Get Pay Slip Of Employee

//        #region Api To Get All Employee List on Assign Pay Roll

//        /// <summary>
//        /// Created By Harshit Mitra On 15-06-2022
//        /// API >> Get >> api/payrollcentral/getemployeelist
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getemployeelist")]
//        public async Task<ResponseBodyModel> EmployeeListOnAssignPayRoll()
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var employeeList = await (from e in _db.Employee
//                                          join or in _db.OrgMaster on e.OrgId equals or.OrgId
//                                          join pg in _db.PayGroups on e.PayGroupId equals pg.PayGroupId into q
//                                          from result in q.DefaultIfEmpty()
//                                          join st in _db.SaleryStructurePayRolls on e.StructureId equals st.StructureId into r
//                                          from newResult in r.DefaultIfEmpty()
//                                          where e.IsActive && !e.IsDeleted &&
//                                          e.CompanyId == claims.companyId && e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee
//                                          select new GetEmployeeListOnAssignPayRollModelClass
//                                          {
//                                              EmployeeId = e.EmployeeId,
//                                              DisplayName = e.DisplayName,
//                                              OrgId = or.OrgId,
//                                              OrgName = or.OrgName,
//                                              PayGroupId = e.PayGroupId,
//                                              PayGroupName = result.PayGroupName,
//                                              StructureId = e.StructureId,
//                                              StructureName = newResult.StructureName,
//                                              Status = e.StructureId == 0 ? AssignPayRollConstants.Unassign : AssignPayRollConstants.Assign,
//                                              GrossCTC = e.GrossSalery,
//                                          }).ToListAsync();

//                if (employeeList.Count > 0)
//                {
//                    employeeList = employeeList.OrderBy(z => z.DisplayName).ToList();

//                    res.Message = "Employee List";
//                    res.Status = true;
//                    res.Data = employeeList;
//                }
//                else
//                {
//                    res.Message = "Employee Not Found";
//                    res.Status = false;
//                    res.Data = employeeList;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get All Employee List on Assign Pay Roll

//        #region Api To Get Structure On Assign Pay Roll

//        /// <summary>
//        /// Created By Harshit Mitra on 15-06-2022
//        /// API >> Get >> api/payrollcentral/getstructure
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getstructure")]
//        public async Task<ResponseBodyModel> GetStructureOnAssignPayRoll(int payGroupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var structure = await _db.SaleryStructurePayRolls.Where(x => x.PayGroupId == payGroupId)
//                        .Select(x => new
//                        {
//                            x.StructureId,
//                            x.StructureName,
//                        }).ToListAsync();

//                if (structure.Count > 0)
//                {
//                    res.Message = "Structure Found";
//                    res.Status = true;
//                    res.Data = structure;
//                }
//                else
//                {
//                    res.Message = "Structure Not Found";
//                    res.Status = false;
//                    res.Data = structure;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Structure On Assign Pay Roll

//        #region Api To Revise Salary Of Employee

//        /// <summary>
//        /// Created By Harshit Mitra On 15-06-2022
//        /// API >> Put >> api/payrollcentral/updatepaygroup
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPut]
//        [Route("updatepaygroup")]
//        public async Task<ResponseBodyModel> ReviseSalaryOfEmployee(UpdatePayGroupModelClass model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var employee = await _db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == model.EmployeeId);
//                if (employee != null)
//                {
//                    var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.PayGroupId == model.PayGroupId);
//                    if (payGroup != null)
//                    {
//                        var structure = await _db.SaleryStructurePayRolls.FirstOrDefaultAsync(x => x.StructureId == model.StructureId);
//                        if (structure != null)
//                        {
//                            employee.PayGroupId = structure.PayGroupId;
//                            employee.StructureId = structure.StructureId;
//                            employee.GrossSalery = model.GrossCTC;
//                            employee.UpdatedBy = claims.employeeId;
//                            employee.UpdatedOn = DateTime.Now;

//                            _db.Entry(employee).State = System.Data.Entity.EntityState.Modified;
//                            await _db.SaveChangesAsync();

//                            res.Message = "Updated";
//                            res.Status = true;
//                        }
//                        else
//                        {
//                            res.Message = "Structure Not Found";
//                            res.Status = false;
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Group Not Found";
//                        res.Status = false;
//                    }
//                }
//                else
//                {
//                    res.Message = "Employee Not Found";
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

//        #endregion Api To Revise Salary Of Employee

//        #region Api To Pre Run Pay Roll Pre Run

//        /// <summary>
//        /// Created By Harshit Mitra On 16-05-2022
//        /// API >> Get >> api/payrollcentral/runpayrollprerun
//        /// </summary>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("runpayrollprerun")]
//        public async Task<ResponseBodyModel> RunPayRollPreRun(int payGorupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var checkPreRun = await _db.RunPayRolls.Where(x => x.PayGroupId == payGorupId).ToListAsync();
//                var lastMonth = new DateTime();
//                if (checkPreRun.Count > 0)
//                {
//                    lastMonth = checkPreRun.OrderByDescending(x => x.CreatedOn).Select(x => x.CreatedOn).First();
//                    var nextMonth = lastMonth.AddMonths(1);
//                    if (nextMonth >= DateTime.Now)
//                    {
//                        res.Message = "You Already Run Pre Pay Roll For This Month";
//                        res.Status = false;
//                        return res;
//                    }
//                }

//                if (_db.Employee.Any(x => x.PayGroupId == payGorupId))
//                {
//                    var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.PayGroupId == payGorupId);
//                    if (payGroup != null)
//                    {
//                        var structures = await _db.SaleryStructurePayRolls.Where(x => x.PayGroupId == payGroup.PayGroupId &&
//                                    x.IsActive == true && x.IsDeleted == false).ToListAsync();
//                        if (structures.Count > 0)
//                        {
//                            if (PayRollCentralHelper.CheckStructure(payGorupId, claims.employeeId))
//                            {
//                                foreach (var item in structures)
//                                {
//                                    PayRollCentralHelper.CalculatedEmployeeSalery(item.StructureId, claims);
//                                }
//                                res.Message = "Run Succesfully";
//                                res.Status = true;
//                            }
//                            else
//                            {
//                                res.Message = "You Skip Calculation Please Check Your Structure First";
//                                res.Status = false;
//                            }
//                        }
//                        else
//                        {
//                            res.Message = "Structure Not Added";
//                            res.Status = false;
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Group Not Found";
//                        res.Status = false;
//                    }
//                }
//                else
//                {
//                    res.Message = "No Employee Added In This Pay Group";
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

//        #endregion Api To Pre Run Pay Roll Pre Run

//        #region Api To Get Employee By Pay Roll Id

//        /// <summary>
//        /// Created By Harshit Mitra on 17-06-2022
//        /// API >> Get >> api/payrollcentral/getemployeebypayrollid
//        /// </summary>
//        /// <param name="payGroupId"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getemployeebypayrollid")]
//        public async Task<ResponseBodyModel> GetEmployeeByPayRollId(int payGroupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var empPayRoll = await (from p in _db.PayGroups
//                                        join e in _db.Employee on p.PayGroupId equals e.PayGroupId
//                                        join s in _db.SaleryStructurePayRolls on e.StructureId equals s.StructureId
//                                        where p.IsActive == true && p.IsDeleted == false && e.IsActive == true &&
//                                        e.IsDeleted == false && s.IsActive == true && s.IsDeleted == false && p.PayGroupId == payGroupId
//                                        && p.CompanyId == claims.companyId && e.CompanyId == claims.companyId
//                                        select new
//                                        {
//                                            p.PayGroupId,
//                                            p.PayGroupName,
//                                            e.EmployeeId,
//                                            e.DisplayName,
//                                            s.StructureId,
//                                            s.StructureName,
//                                            YearlyGrossSalary = e.GrossSalery,
//                                        }).ToListAsync();
//                if (empPayRoll.Count > 0)
//                {
//                    res.Message = "Employee List";
//                    res.Status = true;
//                    res.Data = empPayRoll;
//                }
//                else
//                {
//                    res.Message = "List Is Empty";
//                    res.Status = false;
//                    res.Data = empPayRoll;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Employee By Pay Roll Id

//        #region Api To Get Employeess List On Run Pay Roll

//        /// <summary>
//        /// Created By Harshit Mitra on 20-06-2022
//        /// API >> Get >> api/payrollcentral/getrunpayrolldata
//        /// </summary>
//        /// <param name="payGroupId"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getrunpayrolldata")]
//        public async Task<ResponseBodyModel> GetRunPayRollData(int payGroupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var runPayRoll = await (from e in _db.Employee
//                                        join r in _db.RunPayRolls on e.EmployeeId equals r.EmployeeId
//                                        join p in _db.PayGroups on r.PayGroupId equals p.PayGroupId
//                                        where r.PayGroupId == payGroupId && (r.Status == RunPayRollStatusConstants.Completed
//                                                || r.Status == RunPayRollStatusConstants.Pre_Run_Complete)
//                                        && e.CompanyId == claims.companyId
//                                        select new
//                                        {
//                                            e.EmployeeId,
//                                            e.DisplayName,
//                                            p.PayGroupId,
//                                            p.PayGroupName,
//                                            r.NetSalery,
//                                            r.StructureId,
//                                            StructureName = _db.SaleryStructurePayRolls.Where(x => x.StructureId == r.StructureId).Select(x => x.StructureName).FirstOrDefault(),
//                                            r.PaySlipUrl,
//                                        }).ToListAsync();

//                if (runPayRoll.Count > 0)
//                {
//                    res.Message = "Sucess";
//                    res.Status = true;
//                    res.Data = runPayRoll;
//                }
//                else
//                {
//                    res.Message = "List Is Empty";
//                    res.Status = false;
//                    res.Data = runPayRoll;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Employeess List On Run Pay Roll

//        #region Api To Generate Pay Slip on Run Pay Roll

//        /// <summary>
//        /// Created By Harshit Mitra on 20-06-2022
//        /// API >> Post >> api/payrollcentral/runpayroll
//        /// </summary>
//        /// <param name="payGroupId"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("runpayroll")]
//        public async Task<ResponseBodyModel> RunPayRoll(int payGroupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var employeeList = await _db.RunPayRolls.Where(x => x.PayGroupId == payGroupId).ToListAsync();
//                if (employeeList.Count > 0)
//                {
//                    foreach (var item in employeeList)
//                    {
//                        UpdatePayGroupModelClass obj = new UpdatePayGroupModelClass
//                        {
//                            EmployeeId = item.EmployeeId,
//                            GrossCTC = (item.NetSalery * 12),
//                            PayGroupId = item.PayGroupId,
//                            StructureId = item.StructureId,
//                        };

//                        var payslip = await GetPaySlipPDF(obj);
//                        item.PaySlipUrl = payslip.Path;
//                        item.Status = RunPayRollStatusConstants.Completed;

//                        _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
//                        _db.SaveChanges();
//                    }
//                    res.Message = "Run Success Full";
//                    res.Status = true;
//                }
//                else
//                {
//                    res.Message = "Not Run";
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

//        #endregion Api To Generate Pay Slip on Run Pay Roll

//        #region Api To Get Pay Slip Of Employee

//        /// <summary>
//        /// Created By Harshit Mitra on 21-06-2022
//        /// API >> Get >> api/payrollcentral/getemppayslip
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getemppayslip")]
//        public async Task<ResponseBodyModel> GetEmpPaySlip()
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payslip = await _db.RunPayRolls.Where(x => x.EmployeeId == claims.employeeId &&
//                        (x.PaySlipUrl != null && x.PaySlipUrl != "")).ToListAsync();
//                var data = payslip.Select(x => new
//                {
//                    Date = x.CreatedOn.ToString("MMM yyyy"),
//                    x.PaySlipUrl,
//                }).ToList().OrderBy(x => x.Date).ToList();

//                if (data.Count > 0)
//                {
//                    res.Message = "Pay Slip";
//                    res.Status = true;
//                    res.Data = data;
//                }
//                else
//                {
//                    res.Message = "No Pay Slip Yet";
//                    res.Status = false;
//                    res.Data = data;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Pay Slip Of Employee

//        #region Api To Get Salary Breakup of Selected Employee

//        /// <summary>
//        /// Creted By Harshit Mitra on 15-06-2022
//        /// API >> Get >> api/payrollcentral/getsalerybreakupofemp
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getsalerybreakupofemp")]
//        public async Task<ResponseBodyModel> GetSaleryBreakupOfLogedEmp()
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var emp = await _db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == claims.employeeId);
//                var data = PayRollCentralHelper.Calculate(emp.StructureId, Double.Parse(Math.Round((decimal)(emp.GrossSalery / 12), 2).ToString("0.00")));
//                var response = PayRollCentralHelper.GetSalarySlip(data);
//                var saleryBreakup = PayRollCentralHelper.GetSaleryBreakup(response);
//                res.Message = "Sucess";
//                res.Status = true;
//                res.Data = saleryBreakup;
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Salary Breakup of Selected Employee

//        #region Api To Get Pay Roll Pay Slip Year

//        /// <summary>
//        /// Created By Harshit Mitra on 22-06-2022
//        /// API >> Get >> api/payrollcentral/getpayslipyear
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getpayslipyear")]
//        public async Task<ResponseBodyModel> GetPaySlipYear()
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payslip = await _db.RunPayRolls.Where(x => x.EmployeeId == claims.employeeId &&
//                        (x.PaySlipUrl != null && x.PaySlipUrl != "")).ToListAsync();
//                var data = payslip.Select(x => new
//                {
//                    Year = x.CreatedOn.Year,
//                }).ToList().Distinct().ToList();

//                if (payslip.Count > 0)
//                {
//                    res.Message = "Pay Slip";
//                    res.Status = true;
//                    res.Data = data;
//                }
//                else
//                {
//                    res.Message = "No Pay Slip Yet";
//                    res.Status = false;
//                    res.Data = data;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Pay Roll Pay Slip Year

//        #region Api To Get Pay Roll Pay Slip Months

//        /// <summary>
//        /// Created By Harshit Mitra on 22-06-2022
//        /// API >> Get >> api/payrollcentral/getpayslipmonth
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getpayslipmonth")]
//        public async Task<ResponseBodyModel> GetPaySlipMonth(int year)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payslip = await _db.RunPayRolls.Where(x => x.EmployeeId == claims.employeeId &&
//                        (x.PaySlipUrl != null && x.PaySlipUrl != "") && x.CreatedOn.Year == year).ToListAsync();
//                var data = payslip.Select(x => new
//                {
//                    MonthId = x.MonthYear.Month,
//                    Month = x.MonthYear.ToString("MMM yyyy"),
//                }).ToList().Distinct().ToList();

//                if (payslip.Count > 0)
//                {
//                    res.Message = "Pay Slip";
//                    res.Status = true;
//                    res.Data = data;
//                }
//                else
//                {
//                    res.Message = "No Pay Slip Yet";
//                    res.Status = false;
//                    res.Data = data;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Pay Roll Pay Slip Months

//        #region Api To Get Pay Roll Pay Slip Month Year

//        /// <summary>
//        /// Created By Harshit Mitra on 22-06-2022
//        /// API >> Get >> api/payrollcentral/getpayslipmonthyear
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getpayslipmonthyear")]
//        public async Task<ResponseBodyModel> GetPaySlipMonthYear(int year, int month)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payslip = await _db.RunPayRolls.Where(x => x.EmployeeId == claims.employeeId &&
//                        (x.PaySlipUrl != null && x.PaySlipUrl != "") && x.CreatedOn.Year == year &&
//                        x.MonthYear.Month == month).Select(x => x.PaySlipUrl).FirstOrDefaultAsync();

//                if (!String.IsNullOrEmpty(payslip))
//                {
//                    res.Message = "Pay Slip";
//                    res.Status = true;
//                    res.Data = payslip;
//                }
//                else
//                {
//                    res.Message = "No Pay Slip Yet";
//                    res.Status = false;
//                    res.Data = payslip;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Pay Roll Pay Slip Month Year

//        #region Api To Get Salary Breakup of Employee

//        /// <summary>
//        /// Creted By Harshit Mitra on 15-06-2022
//        /// API >> Post >> api/payrollcentral/getsalerybreakup
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("getsalerybreakup")]
//        public ResponseBodyModel GetSaleryBreakup(UpdatePayGroupModelClass model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var data = PayRollCentralHelper.Calculate(model.StructureId, double.Parse(Math.Round((decimal)(model.GrossCTC / 12), 2).ToString("0.00")));
//                var response = PayRollCentralHelper.GetSalarySlip(data);
//                var saleryBreakup = PayRollCentralHelper.GetSaleryBreakup(response);
//                res.Message = "Sucess";
//                res.Status = true;
//                res.Data = saleryBreakup;
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Salary Breakup of Employee

//        #region Get payslip pdf
//        /// <summary>
//        /// Created By SurajBundel on 14-06-2022
//        /// API >> api/payrollcentral/getemployeepayslippdf
//        /// </summary>
//        /// <returns></returns>

//        [HttpPost]
//        [Route("getemployeepayslippdf")]
//        public async Task<PayslipResponseModel> GetPaySlipPDF(/*int employeeId, */UpdatePayGroupModelClass model)
//        {
//            ApplicationDbContext db = new ApplicationDbContext();
//            ResponseBodyModel res = new ResponseBodyModel();
//            PayslipResponseModel response = new PayslipResponseModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var dates = DateTime.Now.ToString("MMMM_yyyy");
//                // var Employeesdata = await db.Employee.Where(e => e.EmployeeId == employeeId).FirstOrDefaultAsync();
//                var Employeesdata = await db.Employee.Where(e => e.EmployeeId == model.EmployeeId).FirstOrDefaultAsync();
//                var Companydata = await db.Company.Where(e => e.CompanyId == Employeesdata.CompanyId).FirstOrDefaultAsync();

//                WebClient Client = new WebClient();
//                string htmlCode = Client.DownloadString(System.Web.Hosting.HostingEnvironment.MapPath(@"~\uploadimage\Template\EmployeePayslip_Template\EmployeePayslip_Template.html"));
//                //htmlCode = htmlCode.Replace("[BackGround.Image]", Convert.ToString("https://uatbuilddream.moreyeahs.in/Templates/Buildream%20Client%20contract%20Template/Buildream_Client_Contract_Template_Images_Files/background1.jpg"));
//                Client.Dispose();

//                htmlCode = htmlCode.Replace("[Client.date]", (dates));
//                htmlCode = htmlCode.Replace("[Client.CompanyName]", (Employeesdata.CompanyName));
//                htmlCode = htmlCode.Replace("[Client.RegisterAddress]", (Companydata.RegisterAddress));
//                htmlCode = htmlCode.Replace("[Client.FirstName]", (Employeesdata.FirstName));
//                htmlCode = htmlCode.Replace("[Client.LastName]", (Employeesdata.LastName));
//                htmlCode = htmlCode.Replace("[Client.EmployeeId]", Convert.ToString(Employeesdata.EmployeeId));
//                htmlCode = htmlCode.Replace("[Client.JoiningDate]", Employeesdata.JoiningDate.Date.ToString("ddd, dd MMM yyyy"));
//                htmlCode = htmlCode.Replace("[Client.DepartmentName]", (Employeesdata.DepartmentName));
//                htmlCode = htmlCode.Replace("[Client.DesignationName]", (Employeesdata.DesignationName));
//                htmlCode = htmlCode.Replace("[Client.PaymentMode]", Enum.GetName(typeof(ModeofPaymentConstants), ModeofPaymentConstants.Bank_Trasfer).Replace("_", " "));
//                htmlCode = htmlCode.Replace("[Client.BankName]", (Employeesdata.BankName));
//                htmlCode = htmlCode.Replace("[Client.IFSC]", (Employeesdata.IFSC));
//                htmlCode = htmlCode.Replace("[Client.BankAccountNumber]", Convert.ToString(Employeesdata.BankAccountNumber));
//                htmlCode = htmlCode.Replace("[Client.PanNumber]", (Employeesdata.PanNumber));
//                //htmlCode = htmlCode.Replace("[Client.Basic]", Convert.ToString(saleryBreakup.));

//                var data = PayRollCentralHelper.Calculate(model.StructureId, Double.Parse(Math.Round((decimal)(model.GrossCTC / 12), 2).ToString("0.00")));
//                var salarySlip = PayRollCentralHelper.GetSalarySlip(data);
//                var saleryBreakup = PayRollCentralHelper.GetSaleryBreakup(salarySlip);
//                // var netpayable = PayRollCentralHelper.CalculatedEmployeeSalery(data);
//                var innersalary = saleryBreakup.Outer;
//                foreach (var item in innersalary)
//                {
//                    foreach (var i in item.Inner)
//                    {
//                        htmlCode = htmlCode.Replace("[Client.Basic]", Convert.ToString(i.Monthly));
//                        if (i.Name == "Conveyance Allowance")
//                        {
//                            htmlCode = htmlCode.Replace("[Client.ConveyanceAllowance]", Convert.ToString(i.Monthly));
//                        }
//                        if (i.Name == "Special Allowance")
//                        {
//                            htmlCode = htmlCode.Replace("[Client.SpecialAllowance]", Convert.ToString(i.Monthly));
//                        }
//                        if (i.Name == "Hra")
//                        {
//                            htmlCode = htmlCode.Replace("[Client.HRA]", Convert.ToString(i.Monthly));
//                        }
//                    }
//                    htmlCode = htmlCode.Replace("[Client.TotalEarnings]", Convert.ToString(item.TotalMonthly));
//                    // htmlCode = htmlCode.Replace("[Client.TotalEarningsinWords]",Convert.ToString(item.TotalMonthly));
//                    var monthlysalary = PayRollCentralHelper.NumberToWords(Convert.ToInt32(item.TotalMonthly));
//                    htmlCode = htmlCode.Replace("[Client.TotalEarningsinWords]", Convert.ToString(monthlysalary));
//                    htmlCode = htmlCode.Replace("[Client.NetSalaryPayable]", Convert.ToString(item.TotalMonthly));
//                }
//                //htmlCode = htmlCode.Replace("[Client.ProfessionalTax]", (Employeesdata.ProfessionalTax));
//                //htmlCode = htmlCode.Replace("[Client.TotalDeductions]", (Employeesdata.TotalDeductions));
//                //htmlCode = htmlCode.Replace("[Client.NetSalaryPayable]", (Employeesdata.NetSalaryPayable));

//                //htmlCode = htmlCode.Replace("[Client.ProfessionalTax]", (Employeesdata.ProfessionalTax));
//                //htmlCode = htmlCode.Replace("[Client.TotalDeductions]", (Employeesdata.TotalDeductions));
//                //htmlCode = htmlCode.Replace("[Client.NetSalaryPayable]", (Employeesdata.NetSalaryPayable));

//                // htmlCode = htmlCode.Replace("[Agreement.CreatedDate]", Convert.ToString(DateTime.Now.ToString("dd MMMM, yyyy")));
//                #region
//                // var client = await db.ClientDATA.Where(c => c.Id == project.ClientId).FirstOrDefaultAsync();
//                //var vendor = await db.Vendors.Where(v => v.Id == project.VendorId).FirstOrDefaultAsync();

//                //var PayslipData = await (from e in db.Employee
//                //                         join c in db.Company on e.CompanyId equals c.CompanyId
//                //                         join s in db.SaleryStructureComponents on e.CompanyId equals s.CompanyId
//                //                         where e.EmployeeId == employeeId && e.IsDeleted == false && e.IsActive == true &&
//                //                      e.CompanyId == claims.companyid && e.EmployeeId == claims.employeeid
//                //                         select new PayslipDataModel
//                //                         {
//                //                             CompanyName = e.CompanyName,
//                //                             EmployeeName = e.FirstName + e.LastName,
//                //                             EmployeeNumber = e.EmployeeId,
//                //                             JoiningDate = e.JoiningDate,
//                //                             Department = e.DepartmentName,
//                //                             Designation = e.DesignationName,
//                //                             BankName = e.BankName,
//                //                             BankIFSC = e.IFSC,
//                //                             BankAccountNumber = e.BankAccountNumber,
//                //                             PANNumber = e.PanNumber,
//                //                             PaymentMode = "",
//                //                             //PaymentMode = Enum.GetName(typeof(ModeofPaymentEnum)).Replace("_", " ");
//                //                             //UANNumber=s.
//                //                             //PFNumber
//                //                             //ESICNumber
//                //                         }).ToListAsync();
//                //var NewPayslipData = await (from e in db.Employee
//                //                         join c in db.Company on e.CompanyId equals c.CompanyId
//                //                         join s in db.SaleryStructureComponents on e.CompanyId equals s.CompanyId
//                //                         where e.EmployeeId == employeeId && e.IsDeleted == false && e.IsActive == true &&
//                //                      e.CompanyId == claims.companyid && e.EmployeeId == claims.employeeid
//                //                         select new PayslipDataModel
//                //                         {
//                //                             CompanyName = e.CompanyName,
//                //                             EmployeeName = e.FirstName + e.LastName,
//                //                             EmployeeNumber = e.EmployeeId,
//                //                             JoiningDate = e.JoiningDate,
//                //                             Department = e.DepartmentName,
//                //                             Designation = e.DesignationName,
//                //                             BankName = e.BankName,
//                //                             BankIFSC = e.IFSC,
//                //                             BankAccountNumber = e.BankAccountNumber,
//                //                             PANNumber = e.PanNumber,
//                //                             //PaymentMode = Enum.GetName(typeof(ModeofPaymentEnum)).Replace("_", " ");
//                //                             //UANNumber=s.
//                //                             //PFNumber
//                //                             //ESICNumber
//                //                         }).ToListAsync();

//                //    {
//                //                                 ExpenseId = EE.ExpenseId,
//                //                                 CategoryId = EE.CategoryId,
//                //                                 IconImageUrl = EE.IconImageUrl,
//                //                                 ImageUrl = EE.ImageUrl,
//                //                                 ExpenseCategoryType = EE.ExpenseCategoryType,
//                //                                 ExpenseTitle = EE.ExpenseTitle,
//                //                                 ExpenseStatus = EE.ExpenseStatus,
//                //                                 ExpenseDate = EE.ExpenseDate,
//                //                                 ExpenseAmount = EE.ExpenseAmount,
//                //                                 BillNumber = EE.BillNumber,
//                //                                 MerchantName = EE.MerchantName,
//                //                                 Comment = EE.Comment,
//                //                                 FinalApproveAmount = EE.FinalApproveAmount,
//                //                                 //CurrencyName = allCurrency.Where(x=> x.ISOCurrencySymbol == EE.ISOCurrencyCode).Select(x=> x.CurrencyEnglishName).FirstOrDefault().ToString(),
//                //                                 CurrencyName = "",
//                //                                 ISOCurrencyCode = EE.ISOCurrencyCode,

//                //                                 //C.CurrencyId,
//                //                                 DisplayName = Emp.DisplayName,
//                //                                 ApproveRejectBy = EE.ApprovedRejectBy.HasValue ? db.Employee.Where(x => x.EmployeeId == (int)EE.ApprovedRejectBy)
//                //                                                          .Select(x => x.DisplayName).FirstOrDefault() : "--------",
//                //                                 CreatedOn = EE.CreatedOn,
//                //                                 UpdatedOn = EE.UpdatedOn,

//                //                             }).ToListAsync();
//                //    var newBranch = PayslipData.Select(x => new PayslipDataModel
//                //    {
//                //        ExpenseId = x.ExpenseId,
//                //        CategoryId = x.CategoryId,
//                //        IconImageUrl = x.IconImageUrl,
//                //        ImageUrl = x.ImageUrl,
//                //        ExpenseCategoryType = x.ExpenseCategoryType,
//                //        ExpenseTitle = x.ExpenseTitle,
//                //        ExpenseStatus = x.ExpenseStatus,
//                //        ExpenseDate = x.ExpenseDate,
//                //        ExpenseAmount = x.ExpenseAmount,
//                //        BillNumber = x.BillNumber,
//                //        MerchantName = x.MerchantName,
//                //        Comment = x.Comment,
//                //        FinalApproveAmount = x.FinalApproveAmount,
//                //        CurrencyName = allCurrency.Where(q => x.ISOCurrencyCode == q.ISOCurrencySymbol).Select(q => q.CurrencyEnglishName).FirstOrDefault(),
//                //        ISOCurrencyCode = x.ISOCurrencyCode,

//                //        //C.CurrencyId,
//                //        DisplayName = x.DisplayName,
//                //        ApproveRejectBy = x.ApproveRejectBy,
//                //        CreatedOn = x.CreatedOn,
//                //        UpdatedOn = x.UpdatedOn,

//                //    }).ToList()
//                //    .OrderByDescending(x => x.UpdatedOn.HasValue ? x.UpdatedOn : x.CreatedOn)
//                //    .ToList();

//                //    if (newBranch.Count != 0)
//                //    {
//                //        res.Status = true;
//                //        res.Message = "Expense list Found";
//                //        res.Data = newBranch;
//                //    }
//                //    else
//                //{
//                //    res.Status = false;
//                //    res.Message = "No Expense list Found";
//                //    res.Data = newBranch;
//                //}
//                #endregion Get payslip pdf
//                License license = new License();
//                try
//                {
//                    var asposeLicense = Path.Combine(HttpContext.Current.Server.MapPath(@"~\uploadimage\Template\Aspose.Total.NET.lic"));
//                    license.SetLicense(asposeLicense);
//                }
//                catch (Exception ex)
//                {
//                    string mess = ex.Message;
//                }

//                HTMLDocument htmlDocument = new HTMLDocument(htmlCode, @"~\uploadimage\Templates\EmployeePayslip_Template\EmployeePayslip_Template.html");
//                Aspose.Html.Saving.PdfSaveOptions options = new Aspose.Html.Saving.PdfSaveOptions
//                {
//                    JpegQuality = 100,
//                };
//                options.PageSetup.AnyPage = new Page(new Size(800, 920), new Margin(8, 8, 8, 8));

//                string PayslipPath = System.Web.Hosting.HostingEnvironment.MapPath("~/uploadimage/PaySlips/" + Employeesdata.CompanyId + "/" +
//                                dates + "/" + "EmpId" + Employeesdata.EmployeeId + "_" + dates + ".pdf");
//                //string DirectoryURL = PayslipPath.Split(new string[] { "uploadimage\\PaySlips\\" + Employeesdata.CompanyId + "\\" +dates + "\\" + "EmpId"+ Employeesdata.EmployeeId +" " + dates + ".pdf" },StringSplitOptions.None).FirstOrDefault() + dates;
//                string DirectoryURL = PayslipPath.Split(new string[] { "uploadimage\\PaySlips\\" + Employeesdata.CompanyId + "\\" + "EmpId" + Employeesdata.EmployeeId + "_" + dates + ".pdf" }, StringSplitOptions.None).FirstOrDefault() + Employeesdata.EmployeeId;

//                DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
//                if (!objDirectory.Exists)
//                {
//                    Directory.CreateDirectory(DirectoryURL);
//                }
//                if (File.Exists(PayslipPath))
//                {
//                    // If file found, delete it
//                    File.Delete(PayslipPath);
//                }
//                Converter.ConvertHTML(htmlDocument, options, PayslipPath);
//                var dataBytes = File.ReadAllBytes(PayslipPath);

//                PayslipPath = Path.Combine(HttpContext.Current.Server.MapPath(string.Format("~/uploadimage/PaySlips/" + Employeesdata.CompanyName + "/" +
//                                dates + "/" + "EmpId" + Employeesdata.EmployeeId + "_" + dates + ".pdf")));
//                //PayslipPath = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/uploadimage/Template/EmployeePayslip" + "/" + Employeesdata.CompanyName + "/" + Employeesdata.EmployeeId + "/" + dates + ".pdf")); ;
//                string path = "uploadimage/PaySlips/" + Employeesdata.CompanyId + "/" + dates + "/" + "EmpId" + Employeesdata.EmployeeId + "_" + dates + ".pdf";

//                response.Message = "Success";
//                response.Status = true;
//                response.URL = PayslipPath;
//                response.Path = path;
//                response.Extension = ".pdf";
//            }
//            catch (Exception ex)
//            {
//                response.Message = "BackendError : " + ex.Message;
//                response.Status = false;
//            }
//            return response;
//        }

//        #endregion

//        #region Convert Number to word test //Working //Commented

//        //[HttpPost]
//        //[Route("tesword")]
//        //public ResponseBodyModel words(int number)
//        //{
//        //    ResponseBodyModel res = new ResponseBodyModel();
//        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//        //    try
//        //    {
//        //        var test = PayRollCentralHelper.NumberToWords(number);
//        //        res.Message = "Sucess";
//        //        res.Status = true;
//        //        res.Data = test;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        res.Message = ex.Message;
//        //        res.Status = false;
//        //    }
//        //    return res;
//        //}

//        #endregion

//        #region Api To Get Pay Roll DashBoard

//        /// <summary>
//        /// Created By Harshit Mitra on 23-05-2022
//        /// API >> Get >> api/payrollcentral/getdashboardpayroll
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getdashboardpayroll")]
//        public async Task<ResponseBodyModel> GetPayRollDashboard(int year, int month)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                DateTimeFormatInfo mfi = new DateTimeFormatInfo();
//                GetPayRollDashboardHelperClass response = new GetPayRollDashboardHelperClass();
//                var payRoll = await _db.RunPayRolls.Where(x => x.CompanyId == claims.companyId &&
//                            x.CreatedOn.Year == year && x.CreatedOn.Month == month && x.Status == RunPayRollStatusConstants.Completed).ToListAsync();
//                response.Heading = "Pay Roll " + mfi.GetAbbreviatedMonthName(month) + " " + year;
//                response.EmployeeDone = payRoll.Count();
//                response.TotalEmployee = _db.Employee.Where(x => x.CompanyId == claims.companyId).ToList().Count;
//                var payGroup = payRoll.Select(x => x.PayGroupId).Distinct().ToList();
//                List<TotalPayRollListClass> list = new List<TotalPayRollListClass>();
//                foreach (var item in payGroup)
//                {
//                    TotalPayRollListClass obj = new TotalPayRollListClass
//                    {
//                        PayGroup = _db.PayGroups.Where(x => x.PayGroupId == item).Select(x => x.PayGroupName).FirstOrDefault(),
//                        PayGroupCost = payRoll.Where(x => x.PayGroupId == item).Select(x => x.NetSalery).ToList().Sum(),
//                    };
//                    list.Add(obj);
//                }
//                response.PayRollList = list;
//                response.TotalPayRollCost = list.Select(x => x.PayGroupCost).ToList().Sum();

//                res.Message = "Dashboard";
//                res.Status = true;
//                res.Data = response;
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        /// <summary>
//        /// Created By Harshit Mitra on 23-06-2022
//        /// </summary>
//        public class GetPayRollDashboardHelperClass
//        {
//            public string Heading { get; set; }
//            public int EmployeeDone { get; set; }
//            public int TotalEmployee { get; set; }
//            public double TotalPayRollCost { get; set; }
//            public List<TotalPayRollListClass> PayRollList { get; set; }
//        }

//        /// <summary>
//        /// Created By Harshit Mitra on 23-06-2022
//        /// </summary>
//        public class TotalPayRollListClass
//        {
//            public string PayGroup { get; set; }
//            public double PayGroupCost { get; set; }
//        }

//        #endregion

//        #region Api To Get Pay Roll Year List

//        /// <summary>
//        /// Created By Harshit Mitra on 23-06-2022
//        /// API >> Get >> api/payrollcentral/getpayrollyear
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getpayrollyear")]
//        public async Task<ResponseBodyModel> GetYearCountOfPayRoll()
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var getYear = await _db.PayGroups.Where(x => x.CompanyId == claims.companyId).ToListAsync();
//                var list = getYear.OrderByDescending(x => x.CreatedOn)
//                        .Select(x => new
//                        {
//                            x.CreatedOn.Year,
//                        }).Distinct().ToList();

//                if (getYear.Count > 0)
//                {
//                    res.Message = "Sucess";
//                    res.Status = true;
//                    res.Data = list;
//                }
//                else
//                {
//                    res.Message = "Not Found";
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

//        #endregion

//        #region Api To Get Pay Roll Month List

//        /// <summary>
//        /// Created By Harshit Mitra on 23-06-2022
//        /// API >> Get >> api/payrollcentral/getpayrollmonth
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getpayrollmonth")]
//        public async Task<ResponseBodyModel> GetMonthCountOfPayRoll(int year)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var getYear = await _db.RunPayRolls.Where(x => x.CompanyId == claims.companyId &&
//                            x.CreatedOn.Year == year && x.Status == RunPayRollStatusConstants.Completed).ToListAsync();
//                var list = getYear.OrderByDescending(x => x.CreatedOn).Select(x => x.CreatedOn.Month).Distinct().ToArray();
//                List<MonthList> monthList = new List<MonthList>();

//                string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthGenitiveNames;
//                if (list.Length > 0)
//                {
//                    var ordered = monthNames.Skip(list[0] - 1)
//                                        .Concat(monthNames.Take(list[0] - 1))
//                                        .Where(s => !String.IsNullOrEmpty(s))
//                                        .ToList();
//                    int i = list[0];
//                    int grouping = 1;
//                    int check = 0;
//                    foreach (var month in ordered)
//                    {
//                        MonthList months = new MonthList
//                        {
//                            Month = i,
//                            MonthName = month,
//                            Status = i == DateTime.Now.Month ? list.Contains(i) ? "Completed" : "Current" : (list.Contains(i) ? "Completed" : "Up Coming"),
//                            Grouping = grouping,
//                        };
//                        monthList.Add(months);
//                        i = i > 12 ? 1 : i + 1;
//                        check++;
//                        if (check % 4 == 0)
//                            grouping++;
//                    }
//                }
//                else
//                {
//                    int currentmonth = DateTime.Now.Month;
//                    var ordered = monthNames.Skip(currentmonth - 1)
//                                        .Concat(monthNames.Take(currentmonth - 1))
//                                        .Where(s => !String.IsNullOrEmpty(s))
//                                        .ToList();
//                    int i = currentmonth;
//                    int grouping = 1;
//                    int check = 0;
//                    foreach (var month in ordered)
//                    {
//                        MonthList months = new MonthList
//                        {
//                            Month = i,
//                            MonthName = month,
//                            Status = i == DateTime.Now.Month ? "Current" : "Up Coming",
//                            Grouping = grouping,
//                        };
//                        monthList.Add(months);
//                        i = i > 12 ? 1 : i + 1;
//                        check++;
//                        if (check % 4 == 0)
//                            grouping++;
//                    }
//                }
//                res.Message = "Month List";
//                res.Status = true;
//                res.Data = monthList;
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        /// <summary>
//        /// Created By Harshit Mitra on 23-06-2022
//        /// </summary>
//        public class MonthList
//        {
//            public int Month { get; set; }
//            public string MonthName { get; set; }
//            public string Status { get; set; }
//            public int Grouping { get; set; }
//        }

//        #endregion

//        #region Get Pay Roll Dashboard 2

//        /// <summary>
//        /// Created By Harshit Mitra on 25-06-2022
//        /// API >> Get >> api/payrollcentral/payrolldashboard
//        /// </summary>
//        /// <param name="month"></param>
//        /// <param name="year"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("payrolldashboard")]
//        public async Task<ResponseBodyModel> PayRollDashboard(int month, int year)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                // ----- Check Status ----- //
//                var apiResponse = await GetMonthCountOfPayRoll(year);
//                List<MonthList> checkResponse = (List<MonthList>)apiResponse.Data;
//                var check = checkResponse.Where(x => x.Month == month).Select(x => x.Status).FirstOrDefault();
//                // ----- Check Status End ----- //

//                var payRoll = await _db.RunPayRolls.Where(x => x.CompanyId == claims.companyId &&
//                            x.CreatedOn.Year == year && x.CreatedOn.Month == month && x.Status == RunPayRollStatusConstants.Completed).ToListAsync();
//                var payRollEmp = payRoll.Select(x => x.EmployeeId).ToList();
//                GetDashboardCount response = new GetDashboardCount();
//                response.Status = check;
//                response.TotalEmployee = _db.Employee.Where(x => x.CompanyId == claims.companyId && x.IsActive == true &&
//                        x.IsDeleted == false && x.CreatedOn.Year <= year && x.CreatedOn.Month <= month).ToList().Count;
//                response.UnAssignEmployee = _db.Employee.Where(x => x.CompanyId == claims.companyId && x.IsActive == true &&
//                        x.IsDeleted == false && x.CreatedOn.Year <= year && x.CreatedOn.Month <= month && !payRollEmp.Contains(x.EmployeeId)).ToList().Count;

//                response.TotalDepartment = _db.Department.Where(x => x.CompanyId == claims.companyId && x.IsActive == true && x.IsDeleted == false && x.DepartmentName != "Administrator").ToList().Count;
//                response.GrossSalery = payRoll.Select(x => x.GrossSalery).ToList().Sum();
//                response.NetPay = payRoll.Select(x => x.NetSalery).ToList().Sum();
//                response.DeductionAmount = payRoll.Select(x => x.GrossDeductions).ToList().Sum();
//                response.CurrentMonth = response.NetPay;
//                var previousMonth = month - 1 == 0 ? 12 : month - 1;
//                var previousYear = previousMonth == 12 ? year - 1 : year;
//                var payRollPrevious = await _db.RunPayRolls.Where(x => x.CompanyId == claims.companyId &&
//                            x.CreatedOn.Year == previousYear && x.CreatedOn.Month == previousMonth).ToListAsync();
//                response.PreviousMonth = payRollPrevious.Select(x => x.NetSalery).ToList().Sum();
//                if (response.CurrentMonth > response.PreviousMonth)
//                {
//                    response.Changes = "Profit";
//                    response.Diffrences = response.CurrentMonth - response.PreviousMonth;
//                }
//                else if (response.CurrentMonth < response.PreviousMonth)
//                {
//                    response.Changes = "Loss";
//                    response.Diffrences = response.PreviousMonth - response.CurrentMonth;
//                }
//                else
//                {
//                    response.Changes = "Equal";
//                    response.Diffrences = 0;
//                }

//                var employee = await _db.Employee.Where(x => x.CompanyId == claims.companyId && x.IsDeleted == false && x.IsActive == true).ToListAsync();
//                var department = await _db.Department.Where(x => x.CompanyId == claims.companyId && x.IsDeleted == false && x.IsActive == true).ToListAsync();

//                res.Message = "Dashboard";
//                res.Status = true;
//                res.Data = response;
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        /// <summary>
//        /// Created By Harshit Mitra on 25-06-2022
//        /// </summary>
//        public class GetDashboardCount
//        {
//            public int TotalDepartment { get; set; }
//            public int UnAssignEmployee { get; set; }
//            public int TotalEmployee { get; set; }
//            public double GrossSalery { get; set; }
//            public double NetPay { get; set; }
//            public double DeductionAmount { get; set; }
//            public double CurrentMonth { get; set; }
//            public double PreviousMonth { get; set; }
//            public double Diffrences { get; set; }
//            public string Changes { get; set; }
//            public string Status { get; set; }
//        }

//        #endregion

//        #region Get Pay Roll Dashboard 2 Graph

//        /// <summary>
//        /// Created By Harshit Mitra on 25-06-2022
//        /// API >> Get >> api/payrollcentral/payrolldashboardgraph
//        /// </summary>
//        /// <param name="year"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("payrolldashboardgraph")]
//        public async Task<ResponseBodyModel> PayRollDashboardGraph(int year)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payRoll = await _db.RunPayRolls.Where(x => x.CompanyId == claims.companyId).ToListAsync();

//                // ----- Filtered ----- //

//                var employee = await _db.Employee.Where(x => x.CompanyId == claims.companyId && x.IsDeleted == false && x.IsActive == true).ToListAsync();
//                var department = await _db.Department.Where(x => x.CompanyId == claims.companyId && x.IsDeleted == false && x.IsActive == true).ToListAsync();
//                GetGraphData filtered = new GetGraphData()
//                {
//                    Pie = new List<PieBarChartResponse>(),
//                    Bar = new List<PieBarChartResponse>(),
//                };
//                var gender1 = employee.Where(x => x.CreatedOn.Year == year).Select(x => x.Gender).ToList().Distinct().ToList();
//                foreach (var item in gender1)
//                {
//                    PieBarChartResponse obj = new PieBarChartResponse()
//                    {
//                        Name = item,
//                        Value = employee.Where(x => x.Gender == item).ToList().Count,
//                    };
//                    filtered.Pie.Add(obj);
//                }

//                var departmentList1 = (from e in employee
//                                       join d in department on e.DepartmentId equals d.DepartmentId
//                                       where d.DepartmentName != "Administrator"
//                                       select new
//                                       {
//                                           d.DepartmentId,
//                                           d.DepartmentName,
//                                       }).ToList().Distinct().ToList();

//                foreach (var item in departmentList1)
//                {
//                    var emp = employee.Where(x => x.DepartmentId == item.DepartmentId).Select(x => x.EmployeeId).ToList();
//                    PieBarChartResponse obj = new PieBarChartResponse
//                    {
//                        Name = item.DepartmentName,
//                        Value = payRoll.Where(x => emp.Contains(x.EmployeeId) && x.CreatedOn.Year == year).Select(x => x.NetSalery).ToList().Sum(),
//                    };
//                    filtered.Bar.Add(obj);
//                }

//                res.Message = "Dashboard";
//                res.Status = true;
//                res.Data = filtered;
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        /// <summary>
//        /// Created By Harshit Mitra on 25-06-2022
//        /// </summary>
//        public class GetGraphData
//        {
//            public List<PieBarChartResponse> Pie { get; set; }
//            public List<PieBarChartResponse> Bar { get; set; }
//        }

//        /// <summary>
//        /// Created By Harshit Mitra on 25-06-2022
//        /// </summary>
//        public class PieBarChartResponse
//        {
//            public string Name { get; set; }
//            public double Value { get; set; }
//        }

//        #endregion

//        #region Get Pay Roll Dashboard 2 Graph OverAll

//        /// <summary>
//        /// Created By Harshit Mitra on 25-06-2022
//        /// API >> Get >> api/payrollcentral/payrolldashboardgraphoverall
//        /// </summary>
//        /// <param name="year"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("payrolldashboardgraphoverall")]
//        public async Task<ResponseBodyModel> PayRollDashboardGraphOverall()
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payRoll = await _db.RunPayRolls.Where(x => x.CompanyId == claims.companyId).ToListAsync();
//                var employee = await _db.Employee.Where(x => x.CompanyId == claims.companyId && x.IsDeleted == false && x.IsActive == true).ToListAsync();
//                var department = await _db.Department.Where(x => x.CompanyId == claims.companyId && x.IsDeleted == false && x.IsActive == true).ToListAsync();

//                GetGraphData overall = new GetGraphData()
//                {
//                    Pie = new List<PieBarChartResponse>(),
//                    Bar = new List<PieBarChartResponse>(),
//                };
//                var gender1 = employee.Select(x => x.Gender).ToList().Distinct().ToList();
//                foreach (var item in gender1)
//                {
//                    PieBarChartResponse obj = new PieBarChartResponse()
//                    {
//                        Name = item,
//                        Value = employee.Where(x => x.Gender == item).ToList().Count,
//                    };
//                    overall.Pie.Add(obj);
//                }

//                var departmentList1 = (from e in employee
//                                       join d in department on e.DepartmentId equals d.DepartmentId
//                                       where d.DepartmentName != "Administrator"
//                                       select new
//                                       {
//                                           d.DepartmentId,
//                                           d.DepartmentName,
//                                       }).ToList().Distinct().ToList();

//                foreach (var item in departmentList1)
//                {
//                    var emp = employee.Where(x => x.DepartmentId == item.DepartmentId).Select(x => x.EmployeeId).ToList();
//                    PieBarChartResponse obj = new PieBarChartResponse
//                    {
//                        Name = item.DepartmentName,
//                        Value = payRoll.Where(x => emp.Contains(x.EmployeeId)).Select(x => x.NetSalery).ToList().Sum(),
//                    };
//                    overall.Bar.Add(obj);
//                }

//                res.Message = "Dashboard";
//                res.Status = true;
//                res.Data = overall;
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion

//        #region API To Import Pay Slip Report Of Month

//        /// <summary>
//        /// Created By Harshit Mitra on 29-06-2022
//        /// API >> Get >> api/payrollcentral/payslipreport
//        /// </summary>
//        /// <param name="month"></param>
//        /// <param name="year"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("payslipreport")]
//        public async Task<ResponseBodyModel> GetSalaryPaySlip(int month, int year)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var data = await (from p in _db.RunPayRolls
//                                  join e in _db.Employee on p.EmployeeId equals e.EmployeeId
//                                  join pg in _db.PayGroups on e.PayGroupId equals pg.PayGroupId
//                                  join d in _db.Department on e.DepartmentId equals d.DepartmentId
//                                  join ds in _db.Designation on e.DesignationId equals ds.DesignationId
//                                  join o in _db.OrgMaster on e.OrgId equals o.OrgId
//                                  join s in _db.SaleryStructurePayRolls on e.StructureId equals s.StructureId
//                                  where p.CreatedOn.Month == month && p.CreatedOn.Year == year && p.CompanyId == claims.companyId
//                                  && p.Status == RunPayRollStatusConstants.Completed
//                                  select new GetSalerySlipModelClass
//                                  {
//                                      RunPayRollId = p.Id,
//                                      EmployeeCode = e.EmployeeCode,
//                                      EmployeeName = e.DisplayName,
//                                      DateOfJoining = e.JoiningDate,
//                                      PayGroup = pg.PayGroupName,
//                                      WorkerType = e.WorkerType,
//                                      JobTitle = ds.DesignationName,
//                                      Department = d.DepartmentName,
//                                      BusinessUnit = o.OrgName,
//                                      CostCenter = s.StructureName,
//                                      Gender = e.Gender,
//                                      DateOfBirth = e.DateOfBirth,
//                                      PanNumber = e.PanNumber,
//                                      PayRollMonthDate = p.MonthYear,
//                                      PayRollMonth = "",
//                                      Basic = 0,
//                                      ConveyanceAllowance = 0,
//                                      HRA = 0,
//                                      SpecialAllowance = 0,
//                                      Gross = p.GrossSalery,
//                                      NetPay = p.NetSalery,
//                                      PaySlipURL = p.PaySlipUrl,
//                                  }).ToListAsync();
//                foreach (var item in data)
//                {
//                    var component = _db.RunPayRollComponents.Where(x => x.RunPayRollId == item.RunPayRollId).ToList();
//                    item.PayRollMonth = item.PayRollMonthDate.ToString("MMM yyyy");
//                    item.Basic = component.Where(x => x.ComponentName == "Basic").Select(x => x.MonthlyAmount).FirstOrDefault();
//                    item.HRA = component.Where(x => x.ComponentName == "Hra").Select(x => x.MonthlyAmount).FirstOrDefault();
//                    item.SpecialAllowance = component.Where(x => x.ComponentName == "Hra").Select(x => x.MonthlyAmount).FirstOrDefault();
//                }

//                res.Message = "Done";
//                res.Status = true;
//                res.Data = data;
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        public class GetSalerySlipModelClass
//        {
//            public int RunPayRollId { get; set; }
//            public string EmployeeCode { get; set; }
//            public string EmployeeName { get; set; }
//            public DateTime DateOfJoining { get; set; }
//            public string PayGroup { get; set; }

//            //public string EmployeeStatus { get; set; }
//            public string WorkerType { get; set; }

//            public string JobTitle { get; set; }
//            public string Department { get; set; }

//            //public string Location { get; set; }
//            public string BusinessUnit { get; set; }

//            public string CostCenter { get; set; }
//            public string Gender { get; set; }
//            public DateTime DateOfBirth { get; set; }
//            public string PanNumber { get; set; }
//            public DateTime PayRollMonthDate { get; set; }
//            public string PayRollMonth { get; set; }

//            //public string Status { get; set; }
//            //public string StatusDescription { get; set; }
//            //public string Warnings { get; set; }
//            //public int ActualPayDays { get; set; }
//            //public int WorkingDays { get; set; }
//            //public int LossOfPayDays { get; set; }
//            //public int DaysPayable { get; set; }
//            //public double RemunerationAmount { get; set; }
//            public double Basic { get; set; }

//            public double ConveyanceAllowance { get; set; }
//            public double HRA { get; set; }
//            public double SpecialAllowance { get; set; }

//            //public double Arrear { get; set; }
//            public double Gross { get; set; }

//            //public double PFEmployer { get; set; }
//            //public double ESIEmployer { get; set; }
//            //public double Total { get; set; }
//            //public double PFEmployee { get; set; }
//            //public double ESIEmployee { get; set; }
//            //public double TotalContributions { get; set; }
//            //public double ProfessionalTax { get; set; }
//            //public double TotalIncomeTax { get; set; }
//            //public double TotalDeductions { get; set; }
//            public double NetPay { get; set; }

//            //public double CashAdvance { get; set; }
//            //public double SettlementAgainstAdvance { get; set; }
//            //public double TotalReimbursements { get; set; }
//            //public double TotalPayDay { get; set; }
//            public string PaySlipURL { get; set; }
//        }

//        #endregion

//        #region API To Get Pay Roll Dashbord 3

//        /// <summary>
//        /// Created By Harshit Mitra On 04-07-2022
//        /// API >> Get >> api/payrollcentral/newpayrolldashboard
//        /// </summary>
//        /// <param name="year"></param>
//        /// <param name="month"></param>
//        /// <param name="payGroupId"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("newpayrolldashboard")]
//        public async Task<ResponseBodyModel> NewPayRollDashboard(int year, int month, int payGroupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var monthAbbv = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month);
//                var heading = monthAbbv + " - " + year + " (" + monthAbbv + " 01 - " + monthAbbv + " " + DateTime.DaysInMonth(year, month) + ")";
//                NewPayRollDashboardHelperClass obj = new NewPayRollDashboardHelperClass();
//                obj.Heading = heading;

//                var payGroup = await _db.PayGroups.Where(x => x.PayGroupId == payGroupId).FirstOrDefaultAsync();
//                var runPayRoll = await _db.RunPayRolls.Where(x => x.CreatedOn.Year == year &&
//                        x.CreatedOn.Month == month && x.PayGroupId == payGroupId).ToListAsync();
//                if (runPayRoll.Count > 0)
//                {
//                    obj.TotalEmployee = runPayRoll.Select(x => x.EmployeeId).ToList().Distinct().ToList().Count;
//                    obj.CalanderDays = DateTime.DaysInMonth(year, month);
//                    obj.TotalProcessed = runPayRoll.Where(x => x.Status == RunPayRollStatusConstants.Completed).ToList().Count;
//                    obj.TotalProcessedEmployee = runPayRoll.Select(x => x.EmployeeId).ToList().Distinct().ToList().Count;
//                    obj.TotalPayRollCost = Math.Round(runPayRoll.Select(x => x.NetSalery).ToList().Sum(), 2);
//                    obj.TotalContributions = Math.Round(runPayRoll.Select(x => x.GrossSalery).ToList().Sum(), 2);
//                    obj.TotalDeduction = Math.Round(runPayRoll.Select(x => x.GrossDeductions).ToList().Sum(), 2);
//                    obj.TotalContributions = 0;
//                    List<SteperClass> list = new List<SteperClass>();
//                    for (int i = 1; i < 7; i++)
//                    {
//                        SteperClass st = new SteperClass
//                        {
//                            StepId = i,
//                            Status = false,
//                        };
//                        list.Add(st);
//                    }
//                    obj.Steps = list;
//                }
//                else
//                {
//                    obj.TotalEmployee = 0;
//                    obj.CalanderDays = DateTime.DaysInMonth(year, month);
//                    obj.TotalProcessed = 0;
//                    obj.TotalProcessedEmployee = 0;
//                    obj.TotalPayRollCost = 0;
//                    obj.TotalContributions = 0;
//                    obj.TotalDeduction = 0;
//                    obj.TotalContributions = 0;
//                    List<SteperClass> list = new List<SteperClass>();
//                    for (int i = 1; i < 7; i++)
//                    {
//                        SteperClass st = new SteperClass
//                        {
//                            StepId = i,
//                            Status = false,
//                        };
//                        list.Add(st);
//                    }
//                    obj.Steps = list;
//                }
//                res.Message = "Dashboard";
//                res.Status = true;
//                res.Data = obj;
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        /// <summary>
//        /// Created By Harshit Mitra On 04-07-2022
//        /// </summary>
//        public class NewPayRollDashboardHelperClass
//        {
//            public string Heading { get; set; }
//            public int TotalEmployee { get; set; }
//            public int CalanderDays { get; set; }
//            public int TotalProcessed { get; set; }
//            public int TotalProcessedEmployee { get; set; }
//            public double TotalPayRollCost { get; set; }
//            public double EmployeeDeposite { get; set; }
//            public double TotalDeduction { get; set; }
//            public double TotalContributions { get; set; }
//            public List<SteperClass> Steps { get; set; }
//        }

//        public class SteperClass
//        {
//            public int StepId { get; set; }
//            public bool Status { get; set; }
//        }

//        #endregion

//        #region Helper Model Class

//        /// <summary>
//        /// Created By Harshit Mitra on 15-06-2022
//        /// </summary>
//        public class GetEmployeeListOnAssignPayRollModelClass
//        {
//            public int EmployeeId { get; set; }
//            public string DisplayName { get; set; }
//            public int OrgId { get; set; }
//            public string OrgName { get; set; }
//            public AssignPayRollConstants Status { get; set; }
//            public double GrossCTC { get; set; }
//            public int? PayGroupId { get; set; }
//            public string PayGroupName { get; set; }
//            public int? StructureId { get; set; }
//            public string StructureName { get; set; }
//        }

//        /// <summary>
//        /// Created By Harshit Mitra on 15-06-2022
//        /// </summary>
//        public class UpdatePayGroupModelClass
//        {
//            public int EmployeeId { get; set; }
//            public int PayGroupId { get; set; }
//            public int StructureId { get; set; }
//            public double GrossCTC { get; set; }
//        }

//        /// <summary>
//        /// Created By Suraj Bundel on 15-06-2022
//        /// </summary>
//        public class PayslipResponseModel
//        {
//            public string Message { get; set; }
//            public bool Status { get; set; }
//            public string URL { get; set; }
//            public string Path { get; set; }
//            public string Extension { get; set; }
//        }

//        #endregion
//    }
//}