using AspNetIdentity.WebApi.Controllers.NewPayRoll.Salary_Breakup;
using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.New_Pay_Roll;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.Payroll
{
    /// <summary>
    /// Created By Harshit Mitra on 21/12/2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/salerybreakdown")]
    public class EmployeeSaleryBreakDownController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        /// -----------------------------  For Assigning Salary --------------------------- ///

        #region GET ALL EMPLOYEE LIST FOR SALARY ASSIGNING 
        /// <summary>
        /// Created By Harshit Mitra On 22/12/2022
        /// API >> GET >> api/salerybreakdown/getemplistforsalaryasigning
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getemplistforsalaryasigning")]
        public async Task<IHttpActionResult> GetEmployeeListForSalaryAsssigning()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employeeList = await (from e in _db.Employee
                                          join or in _db.OrgMaster on e.OrgId equals or.OrgId into p
                                          from empty in p.DefaultIfEmpty()
                                          join pg in _db.PayGroups on e.PayGroupId equals pg.PayGroupId into q
                                          from result in q.DefaultIfEmpty()
                                          join st in _db.SalaryStructures on e.StructureId equals st.StructureId into r
                                          from newResult in r.DefaultIfEmpty()
                                          where e.IsActive && !e.IsDeleted &&
                                                e.CompanyId == tokenData.companyId &&
                                                e.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee
                                          select new GetEmployeeListOnAssignPayRollResponse
                                          {
                                              EmployeeId = e.EmployeeId,
                                              DisplayName = e.DisplayName,
                                              OrgName = empty == null ? "Admin" : empty.OrgName,
                                              PayGroupName = result.PayGroupName,
                                              StructureName = newResult.StructureName,
                                              Status = (e.StructureId == Guid.Empty ? AssignPayRollConstants.Unassign : AssignPayRollConstants.Assign),
                                              GrossAmount = 0,
                                          }).ToListAsync();
                employeeList = (from e in employeeList
                                join ss in _db.SalaryBreakDowns on e.EmployeeId equals ss.EmployeeId into t
                                from thirdResult in t.DefaultIfEmpty()
                                where (thirdResult != null ? thirdResult.IsCurrentlyUse == true : thirdResult == null)
                                select new GetEmployeeListOnAssignPayRollResponse
                                {
                                    EmployeeId = e.EmployeeId,
                                    DisplayName = e.DisplayName,
                                    OrgName = e.OrgName,
                                    PayGroupName = e.PayGroupName,
                                    StructureName = e.StructureName,
                                    Status = e.Status,
                                    GrossAmount = thirdResult != null ? thirdResult.GrossYearly : 0,

                                }).ToList();

                res.Message = "Employee List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = employeeList;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerybreakdown/getemplistforsalaryasigning | " +
                    //" Model : " + JsonConvert.SerializeObject(model) + " |" +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET CURRENT SALARY INFORMATION FOR ADDINGNEW SALARYBREAKDOWN
        /// <summary>
        /// Created By Harshit Mitra On 22/12/2022
        /// API >> GET >> api/salerybreakdown/getemployeesalaryinfo
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getemployeesalaryinfo")]
        public async Task<IHttpActionResult> GetEmployeeSalaryInfo(int employeeId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var salaryBrekdown = await _db.SalaryBreakDowns
                    .FirstOrDefaultAsync(x => x.IsCurrentlyUse && x.EmployeeId == employeeId);
                var employee = await _db.Employee
                        .FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
                if (salaryBrekdown == null)
                {
                    res.Message = "Employee Current Salary";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = new
                    {
                        CurrentSalary = 0.0,
                        Bonus = 0.0,
                        Others = 0.0,
                        PreviousVersion = "",
                        JoiningDate = employee.JoiningDate,
                        employee.DisplayName,
                        Structure = new
                        {
                            PayGroupId = "",
                            StructureId = "",
                            GrossSalary = 0.0,
                            EffectiveFrom = "",
                            BonousList = new List<int>(),
                            OthersList = new List<int>(),
                            StructureDesign = new List<int>(),
                        },
                    };
                    return Ok(res);
                }
                var emp = await _db.Employee
                       .FirstOrDefaultAsync(x => x.EmployeeId == employeeId);

                List<BonusOtherRequest> bonus = JsonConvert.DeserializeObject<List<BonusOtherRequest>>(salaryBrekdown.Bonus);
                List<BonusOtherRequest> others = JsonConvert.DeserializeObject<List<BonusOtherRequest>>(salaryBrekdown.Others);
                res.Message = "Employee Current Salary";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = new
                {
                    CurrentSalary = salaryBrekdown.GrossYearly,
                    Bonus = bonus.Sum(x => x.Amount),
                    Others = others.Sum(x => x.Amount),
                    PreviousVersion = salaryBrekdown.EffectiveFrom,
                    JoiningDate = employee.JoiningDate,
                    employee.DisplayName,
                    Structure = new
                    {
                        PayGroupId = emp.PayGroupId,
                        StructureId = emp.StructureId,
                        GrossSalary = salaryBrekdown.GrossYearly,
                        EffectiveFrom = salaryBrekdown.EffectiveFrom,
                        BonousList = bonus,
                        OthersList = others,
                        StructureDesign = JsonConvert.DeserializeObject(salaryBrekdown.StructureDesign),
                    },
                };
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerybreakdown/createandupdatesalarybreakdown | " +
                                    "EmployeeId : " + employeeId + " | " +
                                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET COMPLETE PAY GROUP FOR PAY GROUP ASSIGNING 
        /// <summary>
        /// Created By Harshit Mitra On 22/12/2022
        /// API >> GET >> api/salerybreakdown/getpaygrouplistassigning
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getpaygrouplistassigning")]
        public async Task<IHttpActionResult> GetPayGroupForAssigning()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var payGroupList = await _db.PayGroups
                    .Where(x => x.IsCompleted &&
                        x.CompanyId == tokenData.companyId)
                    .Select(x => new
                    {
                        x.PayGroupId,
                        x.PayGroupName,
                    })
                    .OrderBy(x => x.PayGroupName)
                    .ToListAsync();

                if (payGroupList.Count == 0)
                {
                    res.Message = "No Pay Group Is Completed";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = payGroupList;

                    return Ok(res);
                }

                res.Message = "Pay Group List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = payGroupList;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerybreakdown/getpaygrouplistassigning | " +
                    //" Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET COMPLETE STRUCTURE FOR STRUCUTRE ASSIGNING 
        /// <summary>
        /// Created By Harshit Mitra On 22/12/2022
        /// API >> GET >> api/salerybreakdown/getstructurelistassigning
        /// </summary>
        /// <param name="payGroupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getstructurelistassigning")]
        public async Task<IHttpActionResult> GetStructureForAssigning(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var structureList = await _db.SalaryStructures
                    .Where(x => x.IsCompleted &&
                        x.CompanyId == tokenData.companyId &&
                        x.PayGroupId == payGroupId)
                    .Select(x => new
                    {
                        x.StructureId,
                        x.StructureName,
                    })
                    .OrderBy(x => x.StructureName)
                    .ToListAsync();

                if (structureList.Count == 0)
                {
                    res.Message = "No Structure Is Completed";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = structureList;

                    return Ok(res);
                }

                res.Message = "Structure List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = structureList;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerybreakdown/getstructurelistassigning | " +
                    //" Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET BONOUS LIST FOR SALARY BREAKDOWN ADDITION  
        /// <summary>
        /// Created By Harshit Mitra On 22/12/2022
        /// API >> GET >> api/salerybreakdown/getbnouscomponentslist
        /// </summary>
        /// <param name="payGroupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getbnouscomponentslist")]
        public async Task<IHttpActionResult> GetBonousComponents(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var component = await (from cp in _db.ComponentInPays
                                       join sc in _db.AdHocComponents on cp.ComponentId equals sc.ComponentId
                                       where cp.ComponentType == ComponentTypeInPGConstants.BonusComponent
                                            && cp.PayGroupId == payGroupId
                                       select new
                                       {
                                           cp.ComponentId,
                                           ComponentName = sc.Title,

                                       })
                                       .OrderBy(x => x.ComponentName)
                                       .ToListAsync();
                if (component.Count == 0)
                {
                    res.Message = "No Bonus Added In This Pay Group";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = component;

                    return Ok(res);
                }

                res.Message = "Bonus Component";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = component;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerybreakdown/getbnouscomponentslist | " +
                    //" Model : " + JsonConvert.SerializeObject(model) + " |" +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET OTHERS COMPONENTS FOR SALARY BREAKDOWN ADDITION
        /// <summary>
        /// Created By Harshit Mitra On 22/12/2022
        /// API >> GET >> api/salerybreakdown/getotherscomponentslist
        /// </summary>
        /// <param name="payGroupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getotherscomponentslist")]
        public async Task<IHttpActionResult> GetOthersComponents(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var component = await (from cp in _db.ComponentInPays
                                       join rc in _db.RecuringComponents on cp.ComponentId equals rc.RecuringComponentId
                                       where cp.ComponentType == ComponentTypeInPGConstants.RecurringComponent
                                            && rc.ComponentType == PayRollComponentTypeConstants.Other
                                            && cp.PayGroupId == payGroupId
                                       select new
                                       {
                                           cp.ComponentId,
                                           rc.ComponentName,

                                       })
                                       .OrderBy(x => x.ComponentName)
                                       .ToListAsync();
                if (component.Count == 0)
                {
                    res.Message = "No Other Added In This Pay Group";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = component;

                    return Ok(res);
                }

                res.Message = "Other Component";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = component;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerybreakdown/getbnouscomponentslist | " +
                    //" Model : " + JsonConvert.SerializeObject(model) + " |" +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion



        /// -----------------------------  Salary Break Down  --------------------------- ///

        #region API TO CHECK SALARY BREAK DOWN OF A EMPLOYEE 
        /// <summary>
        /// Created By Harshit Mitra On 22/12/2022
        /// API >> POST >> api/salerybreakdown/checksalarybreakdown
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("checksalarybreakdown")]
        public IHttpActionResult CheckSalarytructure(CheckSaleryBreakDownRequest model)
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
                    using (var cal = new CalculationController())
                    {
                        List<dynamic> response = new List<dynamic>();
                        var checkBreakdown = cal.Calculate(model.StructureId, model.GrossSalary);
                        var getDistuingishComponent = cal.SalaryBreakdownBreakInParts(checkBreakdown);

                        var forEachList = getDistuingishComponent.Select(x => x.Type).Distinct().ToList();
                        foreach (var item in forEachList)
                        {
                            var checkList = new
                            {
                                Outer = new List<string>
                                        {
                                            item.ToString(),
                                            "Monthly",
                                            "Anually"
                                        },
                                Inner = getDistuingishComponent
                                    .Where(x => x.Type == item)
                                    .Select(x => new
                                    {
                                        Name = x.ComponentName,
                                        Monthly = x.MonthlyAmount,
                                        Anually = x.YearlyAmount,
                                    })
                                    .ToList(),
                                Footer = new List<string>
                                        {
                                            "Total " + item,
                                            getDistuingishComponent.Where(x=> x.Type == item).Select(x=> x.MonthlyAmount).Sum().ToString(),
                                            getDistuingishComponent.Where(x=> x.Type == item).Select(x=> x.YearlyAmount).Sum().ToString(),
                                        },
                            };
                            response.Add(checkList);
                        }
                        res.Message = "Salary Breakup";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.OK;
                        res.Data = new
                        {
                            ComponentList = response,
                            GrossPayData = new List<String>
                                    {
                                        "Gross Pay" ,
                                        (getDistuingishComponent.Where(x=> x.Type == "Earnings").Select(x=> x.MonthlyAmount).Sum() -
                                        getDistuingishComponent.Where(x=> x.Type == "Deductions").Select(x=> x.MonthlyAmount).Sum() +
                                        getDistuingishComponent.Where(x=> x.Type == "Other Benifits" && x.IncludeInGross).Select(x=> x.MonthlyAmount).Sum())
                                            .ToString(),
                                        (getDistuingishComponent.Where(x=> x.Type == "Earnings").Select(x=> x.YearlyAmount).Sum() -
                                        getDistuingishComponent.Where(x=> x.Type == "Deductions").Select(x=> x.YearlyAmount).Sum() +
                                        getDistuingishComponent.Where(x=> x.Type == "Other Benifits" && x.IncludeInGross).Select(x=> x.YearlyAmount).Sum())
                                            .ToString()
                                    },
                            NetPayData = new List<string>
                                    {
                                        "Total Cost To Company" ,
                                        (getDistuingishComponent.Where(x=> x.Type == "Earnings").Select(x=> x.MonthlyAmount).Sum() -
                                        getDistuingishComponent.Where(x=> x.Type == "Deductions").Select(x=> x.MonthlyAmount).Sum() +
                                        getDistuingishComponent.Where(x=> x.Type == "Other Benifits").Select(x=> x.MonthlyAmount).Sum())
                                            .ToString(),
                                        (getDistuingishComponent.Where(x=> x.Type == "Earnings").Select(x=> x.YearlyAmount).Sum() -
                                        getDistuingishComponent.Where(x=> x.Type == "Deductions").Select(x=> x.YearlyAmount).Sum() +
                                        getDistuingishComponent.Where(x=> x.Type == "Other Benifits").Select(x=> x.YearlyAmount).Sum())
                                            .ToString(),
                                    }
                        };
                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerybreakdown/checksalarybreakdown | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " |" +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO UPDATE EMPLOYEE SALARY BREAKDOWN STRUCTURE
        /// <summary>
        /// Created By Harshit Mitra On 22/12/2022
        /// API >> POST >> api/salerybreakdown/createandupdatesalarybreakdown
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createandupdatesalarybreakdown")]
        public async Task<IHttpActionResult> CreateAndUpdateBreakDown(AddUpdateSalaryBreakDownClass model)
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
                    var salaryBreakDownCount = await _db.SalaryBreakDowns
                        .Where(x => x.EmployeeId == model.EmployeeId)
                        .ToListAsync();
                    using (var cal = new CalculationController())
                    {
                        var checkBreakdown = cal.Calculate(model.StructureId, model.GrossSalary);
                        var salaryStructureType = cal.GetSalaryStructureTypes(checkBreakdown);
                        var emp = await _db.Employee
                            .FirstOrDefaultAsync(x => x.EmployeeId == model.EmployeeId);
                        emp.PayGroupId = model.PayGroupId;
                        emp.StructureId = model.StructureId;
                        model.BonusList.ForEach(x => x.Name = _db.AdHocComponents.Where(z => z.ComponentId == x.Id).Select(z => z.Title).FirstOrDefault());
                        SalaryBreakDown obj = new SalaryBreakDown
                        {
                            EmployeeId = model.EmployeeId,
                            CostToCompany = 0.0,
                            GrossYearly = 0.0,
                            GrossMonthly = 0.0,
                            Earnings = JsonConvert.SerializeObject(salaryStructureType.Earnnings),
                            Deductions = JsonConvert.SerializeObject(salaryStructureType.Deductions),
                            Others = JsonConvert.SerializeObject(salaryStructureType.Others),
                            Bonus = JsonConvert.SerializeObject(model.BonusList),
                            IsCurrentlyUse = true,
                            ChangesCount = salaryBreakDownCount.LongCount(),

                            CompanyId = tokenData.companyId,
                            CreatedBy = tokenData.employeeId,
                        };
                        obj.GrossMonthly = salaryStructureType.Earnnings.Sum(x => x.MontlyCalculation) - salaryStructureType.Deductions.Sum(x => x.MontlyCalculation);
                        obj.GrossYearly = salaryStructureType.Earnnings.Sum(x => x.AfterCalculation) - salaryStructureType.Deductions.Sum(x => x.AfterCalculation);
                        obj.CostToCompany = obj.GrossYearly + model.BonusList.Sum(x => x.Amount) + salaryStructureType.Others.Sum(x => x.MontlyCalculation);
                        obj.StructureDesign = JsonConvert.SerializeObject(model.StructureDesign);
                        obj.EffectiveFrom = TimeZoneConvert.ConvertTimeToSelectedZone(model.EffectiveFrom);

                        emp.SalaryBreakDownId = obj.Id;

                        if (salaryBreakDownCount.LongCount() != 0)
                        {
                            var change = salaryBreakDownCount.FirstOrDefault(x => x.IsCurrentlyUse);
                            change.IsCurrentlyUse = false;
                            _db.Entry(change).State = EntityState.Modified;
                        }

                        _db.SalaryBreakDowns.Add(obj);
                        _db.Entry(emp).State = EntityState.Modified;

                        await _db.SaveChangesAsync();

                        res.Message = "New Salary Breakdown Save";
                        res.Status = true;
                        res.StatusCode = HttpStatusCode.Created;

                        return Ok(res);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerybreakdown/createandupdatesalarybreakdown | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " |" +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion






        #region REQUEST AND RESPONSE CLASSES
        public class CheckSaleryBreakDownRequest
        {
            [Required]
            public double GrossSalary { get; set; }
            public Guid PayGroupId { get; set; } = Guid.Empty;
            [Required]
            public Guid StructureId { get; set; }
            [Required]
            public int EmployeeId { get; set; }
        }
        public class AddUpdateSalaryBreakDownClass : CheckSaleryBreakDownRequest
        {
            public DateTime EffectiveFrom { get; set; }
            public List<BonusOtherRequest> BonusList { get; set; }
            public dynamic StructureDesign { get; set; }
        }
        public class BonusOtherRequest
        {
            public Guid Id { get; set; } = Guid.Empty;
            public string Name { get; set; } = String.Empty;
            public double Amount { get; set; } = 0;
            public DateTimeOffset PayOutDate { get; set; } = DateTimeOffset.Now;
            public string Note { get; set; } = String.Empty;
        }

        public class GetEmployeeListOnAssignPayRollResponse
        {
            public int EmployeeId { get; set; } = 0;
            public string DisplayName { get; set; } = String.Empty;
            public string OrgName { get; set; } = String.Empty;
            public AssignPayRollConstants Status { get; set; }
            public double GrossAmount { get; set; } = 0.0;
            public string PayGroupName { get; set; } = String.Empty;
            public string StructureName { get; set; } = String.Empty;
        }
        #endregion
    }
}
