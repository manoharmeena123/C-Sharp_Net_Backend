using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.New_Pay_Roll_Run_Model;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using static AspNetIdentity.WebApi.Controllers.NewPayRoll.Salary_Breakup.CalculationController;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.NewPayRoll.Salary_Breakup
{
    /// <summary>
    /// Created By Harshit Mitra on 23-12-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/payrollrun")]
    public class RunPayRollController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        #region API TO GET PAY GROUP LIST FOR RUN PAYROLL
        /// <summary>
        /// Created By Harshit Mitra on 23/12/2022
        /// API >> Post >> api/payrollrun/getpaygroupforrun
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getpaygroupforrun")]
        public async Task<IHttpActionResult> GetPayGroupForRun()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var payGroups = await (from p in _db.PayGroups
                                       join e in _db.Employee on p.PayGroupId equals e.PayGroupId
                                       where e.PayGroupId != Guid.Empty && p.IsCompleted
                                       select new
                                       {
                                           p.PayGroupId,
                                           p.PayGroupName,
                                       })
                                       .Distinct()
                                       .ToListAsync();
                if (payGroups.Count == 0)
                {
                    res.Message = "Pay Group Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = payGroups;

                    return Ok(res);
                }

                res.Message = "Pay Group Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = payGroups;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payrollrun/getpaygroupforrun | " +
                     //"Structure Id : " + structureId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET EMPLOYEE LIST FOR RUN PAYROLL
        /// <summary>
        /// Created By Harshit Mitra on 23/12/2022
        /// API >> Post >> api/payrollrun/emplooyeelistforrunpayroll
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("emplooyeelistforrunpayroll")]
        public async Task<IHttpActionResult> EmployeeListForRunPayRoll(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var emplist = await (from e in _db.Employee
                                     join p in _db.PayGroups on e.PayGroupId equals p.PayGroupId
                                     join s in _db.SalaryStructures on e.StructureId equals s.StructureId
                                     join sb in _db.SalaryBreakDowns on e.EmployeeId equals sb.EmployeeId
                                     where e.PayGroupId == payGroupId && sb.IsCurrentlyUse &&
                                          e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee
                                     select new
                                     {
                                         p.PayGroupId,
                                         p.PayGroupName,
                                         e.EmployeeId,
                                         e.DisplayName,
                                         s.StructureId,
                                         s.StructureName,
                                         sb.GrossMonthly,

                                     })
                                     .Distinct()
                                     .ToListAsync();

                res.Message = "Employee List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = emplist;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payrollrun/getpaygroupforrun | " +
                     "Pay Group Id : " + payGroupId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO PRE RUN PAY ROLL SETUP
        /// <summary>
        /// Created By Harshit Mitra on 23/12/2022
        /// API >> Post >> api/payrollrun/prerunpayroll
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("prerunpayroll")]
        public async Task<IHttpActionResult> PreRunPayRoll(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                var runPayRollGroup = await _db.RunPayRollGroups
                        .FirstOrDefaultAsync(x => x.Month == today.Month && x.Year == today.Year
                                && x.CompanyId == tokenData.companyId && x.PayGroupId == payGroupId);
                if (runPayRollGroup == null)
                {
                    RunPayRollGroup obj = new RunPayRollGroup
                    {
                        Year = today.Year,
                        Month = today.Month,
                        PayGroupId = payGroupId,
                        CompanyId = tokenData.companyId,
                        CreatedBy = tokenData.employeeId,
                        RunType = PayRollRunTypeConstants.OnPreRun,
                        RunCount = JsonConvert.SerializeObject(
                            new List<RunCountJson>()
                            {
                                new RunCountJson
                                {
                                    RunBy = tokenData.employeeId,
                                    RunOn = today,
                                    RunStatus = "Create",
                                    CountValue = 1,
                                }
                            }),
                    };
                    _db.RunPayRollGroups.Add(obj);
                    await _db.SaveChangesAsync();

                    runPayRollGroup = obj;
                }
                else
                {
                    runPayRollGroup.UpdatedOn = today;
                    runPayRollGroup.UpdatedBy = tokenData.employeeId;
                    var listCount = JsonConvert.DeserializeObject<List<RunCountJson>>(runPayRollGroup.RunCount);
                    listCount.Add(new RunCountJson
                    {
                        RunBy = tokenData.employeeId,
                        RunOn = today,
                        RunStatus = "Update",
                        CountValue = (listCount.Count() + 1),
                    });
                    runPayRollGroup.RunCount = JsonConvert.SerializeObject(listCount);

                    _db.Entry(runPayRollGroup).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                }
                var structureList = await (from e in _db.Employee
                                           join s in _db.SalaryBreakDowns on e.EmployeeId equals s.EmployeeId
                                           where s.IsCurrentlyUse && e.PayGroupId == payGroupId
                                           select new
                                           {
                                               e.EmployeeId,
                                               e.DisplayName,
                                               e.StructureId,
                                               s.GrossMonthly,
                                               s.Earnings,
                                               s.Deductions,
                                               s.Others,
                                               s.Bonus,
                                           })
                                           .ToListAsync();
                var netCheck = structureList
                    .Select(x => new
                    {
                        x.EmployeeId,
                        x.DisplayName,
                        x.StructureId,
                        x.GrossMonthly,
                        earnings = JsonConvert.DeserializeObject<List<GetSetupProperties>>(x.Earnings),
                        deductions = JsonConvert.DeserializeObject<List<GetSetupProperties>>(x.Deductions),
                        others = JsonConvert.DeserializeObject<List<GetSetupProperties>>(x.Others),
                        bonus = JsonConvert.DeserializeObject<List<dynamic>>(x.Bonus),

                    }).ToList();

                var calculateTax = netCheck
                    .Select(x => new TaxCalculationJson
                    {
                        Property = x,
                        TaxComponent = (from sc in _db.SalaryStructureConfigs
                                        join tc in _db.TaxDeductions on sc.ComponentId equals tc.TaxComponentId
                                        //join cp in _db.ComponentInPays on sc.ComponentId equals cp.ComponentId
                                        where sc.StructureId == x.StructureId &&
                                              sc.ComponentType == ComponentTypeInPGConstants.TaxDeductionComponent
                                        select new GetSetupProperties
                                        {
                                            PayGroupId = _db.SalaryStructures.Where(z => z.StructureId == sc.StructureId).Select(z => z.PayGroupId).FirstOrDefault(),
                                            ComponentId = sc.ComponentId,
                                            ComponentName = tc.DeductionName,
                                            ComponentCode = "[" + tc.DeductionName.Replace(" ", "").ToUpper() + "]",
                                            TaxSettings = sc.TaxSettings,
                                            CalculationType = sc.CalculationType ? "FORMULA" : "FIXED",
                                            ComponentType = sc.ComponentType,
                                            RecuringComponentType = PayRollComponentTypeConstants.Other,
                                            CalculatingValue = sc.CalculatingValue.Replace(" ", "_").ToUpper(),
                                            Formula = "",
                                            AfterCalculation = 0,
                                        })
                                        .Distinct()
                                        .ToList(),
                    })
                    .ToList();
                using (var cl = new CalculationController())
                {
                    var payGroupSetting = calculateTax
                        .Select(x => new
                        {
                            PayGroupId = x.TaxComponent.First().PayGroupId,
                        })
                        .Distinct()
                        .ToList()
                        .Select(x => new
                        {
                            x.PayGroupId,
                            CompanyInfos = _db.CompanyInfos
                                .Where(z => z.PayGroupId == x.PayGroupId)
                                .FirstOrDefault(),
                            PtSetting = _db.StatutoryFllings
                                .Where(z => z.PayGroupId == x.PayGroupId)
                                .FirstOrDefault(),
                        })
                        .FirstOrDefault();
                    var countryAndState = (from c in _db.Country
                                           join s in _db.State on c.CountryId equals s.CountryId
                                           where c.IsActive && !c.IsDeleted && s.IsActive && !s.IsDeleted
                                           select new CountryAndStates
                                           {
                                               CountryId = c.CountryId,
                                               CountryName = c.CountryName,
                                               StateId = s.StateId,
                                               StateName = s.StateName,
                                           })
                                           .ToList();
                    string month = CultureInfo
                        .CurrentCulture
                        .DateTimeFormat
                        .GetMonthName(TimeZoneConvert
                            .ConvertTimeToSelectedZone(DateTime.UtcNow)
                            .Month);
                    foreach (var item in calculateTax)
                    {
                        item.TaxComponent = cl.CalculateTaxInStructure(month, item.Property, payGroupSetting, item.TaxComponent, countryAndState);
                    }
                }

                var result = netCheck
                    .Select(x => new
                    {
                        x.EmployeeId,
                        x.DisplayName,
                        x.earnings,
                        x.deductions,
                        x.others,
                        x.bonus,
                        EarningsAmount = x.earnings.Sum(z => z.MontlyCalculation),
                        DeductionsAmount = x.deductions.Sum(z => z.MontlyCalculation),
                        OthersAmount = x.others
                                    .Where(z => z.IncludeInGross)
                                    .Sum(z => z.MontlyCalculation),
                        BonusAmount = x.bonus
                                    .Where(z => ((DateTimeOffset)z.PayOutDate).Year == today.Year && ((DateTimeOffset)z.PayOutDate).Month == today.Month)
                                    .Sum(z => z.Amount),
                        TaxDeductionAmount = calculateTax.Where(z => z.Property.EmployeeId == x.EmployeeId).Select(z => z.TaxComponent).FirstOrDefault().Sum(z => z.MontlyCalculation),
                        tax = calculateTax.Where(z => z.Property.EmployeeId == x.EmployeeId).Select(z => z.TaxComponent).FirstOrDefault(),
                    })
                    .Select(x => new PreRunResponse
                    {
                        EmployeeId = x.EmployeeId,
                        DisplayName = x.DisplayName,
                        EarningsAmount = x.EarningsAmount,
                        DeductionsAmount = x.DeductionsAmount,
                        OtherAmount = x.OthersAmount,
                        BonusAmount = x.BonusAmount,
                        MonthlyTotal = x.EarningsAmount + x.OthersAmount + x.BonusAmount - x.DeductionsAmount - x.TaxDeductionAmount,
                        TaxDeductionAmount = x.TaxDeductionAmount,
                        ComponentCheck = JsonConvert.SerializeObject(new
                        {
                            earnings = x.earnings,
                            deductions = x.deductions,
                            others = x.others.Where(z => z.IncludeInGross).ToList(),
                            bonus = x.bonus,
                            tax = x.tax,
                            monthlyTotal = x.EarningsAmount + x.OthersAmount + x.BonusAmount - x.DeductionsAmount - x.TaxDeductionAmount,
                        }),
                    })
                    .ToList();

                res.Message = "Pre Run Completed";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = result
                    .Select(x => new
                    {
                        x.EmployeeId,
                        x.DisplayName,
                        x.EarningsAmount,
                        x.DeductionsAmount,
                        x.OtherAmount,
                        x.BonusAmount,
                        x.TaxDeductionAmount,
                        x.MonthlyTotal,
                    })
                    .OrderBy(x => x.DisplayName)
                    .ToList();

                HostingEnvironment.QueueBackgroundWorkItem(qr => UpdatePreRun(runPayRollGroup.GroupId, result));

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payrollrun/prerunpayroll | " +
                     "Pay Group Id : " + payGroupId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public void UpdatePreRun(Guid groupId, List<PreRunResponse> list)
        {
            Thread.Sleep(10); // 1 sec = 1000;
            var checkData = _db.RunPayRolls.Where(x => x.GroupId == groupId).ToList();
            if (list.Count > 0)
                _db.RunPayRolls.RemoveRange(checkData);

            var addData = list
                .Select(x => new RunPayRoll
                {
                    GroupId = groupId,
                    EmployeeId = x.EmployeeId,
                    EarningsAmount = x.EarningsAmount,
                    DeductionsAmount = x.DeductionsAmount,
                    OtherAmount = x.OtherAmount,
                    BonusAmount = x.BonusAmount,
                    MonthlyTotal = x.MonthlyTotal,
                    ComponentCheck = x.ComponentCheck,
                })
                .ToList();
            _db.RunPayRolls.AddRange(addData);
            _db.SaveChangesAsync();
        }
        public class CountryAndStates
        {
            public int CountryId { get; set; }
            public string CountryName { get; set; }
            public int StateId { get; set; }
            public string StateName { get; set; }
        }
        public class RunCountJson
        {
            public int RunBy { get; set; }
            public DateTimeOffset RunOn { get; set; }
            public string RunStatus { get; set; }
            public long CountValue { get; set; }
        }
        public class TaxCalculationJson
        {
            public dynamic Property { get; set; }
            public List<GetSetupProperties> TaxComponent { get; set; }
        }
        #endregion

        #region API TO GET RUN PAY ROLL MAIN DASHBOARD

        #endregion

        #region API TO GET EMPLOYEE LIST FOR PAY ROLL RUN

        #endregion

        #region API TO GET EMPLOYEE PAY SLIP
        /// <summary>
        /// Created By Harshit Mitra on 23/12/2022
        /// API >> Post >> api/payrollrun/getemployeepayslip
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployeepayslip")]
        public async Task<IHttpActionResult> GetEmployeePaySlip(int employeeId, int month = 0, int year = 0)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (month == 0 || year == 0)
                {
                    var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);
                    month = today.Month;
                    year = today.Year;
                }
                var employeeJoin = (from e in _db.Employee
                                    join de in _db.Department on e.DepartmentId equals de.DepartmentId
                                    join ds in _db.Designation on e.DesignationId equals ds.DesignationId
                                    join c in _db.Company on e.CompanyId equals c.CompanyId
                                    where e.EmployeeId == employeeId
                                    select new
                                    {
                                        e.EmployeeId,
                                        e.EmployeeCode,
                                        e.DisplayName,
                                        e.JoiningDate,
                                        e.BankName,
                                        e.BankAccountNumber,
                                        e.PanNumber,
                                        e.IFSC,
                                        de.DepartmentName,
                                        ds.DesignationName,
                                        c.RegisterCompanyName,
                                        c.RegisterAddress,
                                    })
                                    .AsEnumerable();
                var group = await (from g in _db.RunPayRollGroups
                                   join r in _db.RunPayRolls on g.GroupId equals r.GroupId
                                   join e in employeeJoin on r.EmployeeId equals e.EmployeeId
                                   where g.Year == year && g.Month == month && r.EmployeeId == employeeId
                                   select new
                                   {
                                       g.Month,
                                       g.Year,
                                       r.EmployeeId,
                                       e.EmployeeCode,
                                       e.DisplayName,
                                       e.JoiningDate,
                                       e.BankName,
                                       e.BankAccountNumber,
                                       e.PanNumber,
                                       e.IFSC,
                                       e.DepartmentName,
                                       e.DesignationName,
                                       r.MonthlyTotal,
                                       r.ComponentCheck,
                                       e.RegisterCompanyName,
                                       e.RegisterAddress,

                                   })
                                   .FirstOrDefaultAsync();
                dynamic openComponents = JsonConvert.DeserializeObject(group.ComponentCheck);
                string netShow = String.Empty;

                #region LEFT SIDE COMPONENTS

                List<ComponentResponse> left = new List<ComponentResponse>();
                if (openComponents.earnings.Count > 0)
                {
                    netShow += "E";
                    left.Add(new ComponentResponse { ComponentName = "Earnings", Code = "E", Top = true, });
                    foreach (var item in openComponents.earnings)
                        left.Add(new ComponentResponse { ComponentName = item.ComponentName, Amount = item.MontlyCalculation });
                }
                if (openComponents.others.Count > 0)
                {
                    netShow += String.IsNullOrEmpty(netShow) ? "O" : " + O";
                    left.Add(new ComponentResponse { ComponentName = "Others", Code = "O", Top = true, });
                    foreach (var item in openComponents.others)
                        left.Add(new ComponentResponse { ComponentName = item.ComponentName, Amount = item.MontlyCalculation });
                }
                if (openComponents.bonus.Count > 0)
                {
                    netShow += String.IsNullOrEmpty(netShow) ? "SB" : " + SB";
                    left.Add(new ComponentResponse { ComponentName = "Special Bonus", Code = "SB", Top = true, });
                    foreach (var item in openComponents.bonus)
                        left.Add(new ComponentResponse { ComponentName = item.Name, Amount = (Double)item.Amount });
                }
                // AdHoc Earnings : AE
                // Rembusments  : RB


                #endregion

                #region RIGHT SIDE COMPONENTS

                List<ComponentResponse> right = new List<ComponentResponse>();
                if (openComponents.deductions.Count > 0)
                {
                    netShow += String.IsNullOrEmpty(netShow) ? "D" : " - D";
                    right.Add(new ComponentResponse { ComponentName = "Deductions", Code = "D", Top = true, });
                    foreach (var item in openComponents.deductions)
                        right.Add(new ComponentResponse { ComponentName = item.ComponentName, Amount = item.MontlyCalculation });
                }
                if (openComponents.tax.Count > 0)
                {
                    netShow += String.IsNullOrEmpty(netShow) ? "TD" : " - TD";
                    right.Add(new ComponentResponse { ComponentName = "Tax Deductions", Code = "TD", Top = true, });
                    foreach (var item in openComponents.tax)
                        right.Add(new ComponentResponse { ComponentName = item.ComponentName, Amount = item.MontlyCalculation });
                }


                #endregion

                var stringSalary = ConvertToIndianCurrency((double)openComponents.monthlyTotal);
                var result = new
                {
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(group.Month),
                    group.Year,
                    CompanyName = group.RegisterCompanyName,
                    CompanyAddress = group.RegisterAddress,
                    AcualPaybleDays = (double)DateTime.DaysInMonth(year, month),
                    TotalWorkingDays = (double)DateTime.DaysInMonth(year, month),
                    LossOfPayDays = 0.0,
                    DaysPayble = (double)DateTime.DaysInMonth(year, month),
                    group.EmployeeCode,
                    group.JoiningDate,
                    group.DisplayName,
                    group.DepartmentName,
                    group.DesignationName,
                    group.BankName,
                    group.BankAccountNumber,
                    group.PanNumber,
                    group.IFSC,
                    SalaryDetails = new
                    {
                        LeftData = new
                        {
                            Components = left,
                            Code = String.Join(" + ", left.Where(x => !String.IsNullOrEmpty(x.Code)).Select(x => x.Code).Distinct().OrderBy(x => x).ToList()),
                            Total = left.Sum(x => x.Amount),
                        },
                        RightData = new
                        {
                            Components = right,
                            Code = String.Join(" + ", right.Where(x => !String.IsNullOrEmpty(x.Code)).Select(x => x.Code).Distinct().OrderBy(x => x).ToList()),
                            Total = right.Sum(x => x.Amount),
                        }
                    },
                    NetFormula = netShow,
                    Net = openComponents.monthlyTotal,
                    StringSalary = stringSalary,
                };

                res.Message = "Employee Pay Slip";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = result;

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/payrollrun/getemployeepayslip | " +
                     "Employee Id : " + employeeId + " | " +
                     "Month : " + month + " | " +
                     "Year : " + year + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public string ConvertToIndianCurrency(double amount)
        {
            string result = "";
            using (var cal = new CalculationController())
            {
                if (amount.ToString().Contains("."))
                {
                    var amountArray = amount.ToString().Split('.')?.Select(Int64.Parse)?.ToArray();
                    result = cal.AmountToIndianCurrencyString(amountArray[0]) + " RUPEES AND " + cal.AmountToIndianCurrencyString(amountArray[1]) + " PAISA";
                }
                else
                {
                    result = cal.AmountToIndianCurrencyString((long)amount) + " RUPEES";
                }
            }
            return result;
        }
        public class ComponentResponse
        {
            public string ComponentName { get; set; } = String.Empty;
            public double Amount { get; set; } = 0.0;
            public string Code { get; set; } = String.Empty;
            public bool Top { get; set; } = false;
        }
        #endregion



        #region RESUEST AND RESPONSE
        public class PreRunResponse
        {
            public int EmployeeId { get; set; }
            public string DisplayName { get; set; }
            public double EarningsAmount { get; set; }
            public double DeductionsAmount { get; set; }
            public double OtherAmount { get; set; }
            public double BonusAmount { get; set; }
            public double TaxDeductionAmount { get; set; }
            public double MonthlyTotal { get; set; }
            public string ComponentCheck { get; set; }
        }
        #endregion
    }
}
