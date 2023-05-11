//using AspNetIdentity.WebApi.Infrastructure;
//using AspNetIdentity.WebApi.Model.RunPayRollModels;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Globalization;
//using System.Linq;
//using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
//using static AspNetIdentity.WebApi.Model.EnumClass;

//namespace AspNetIdentity.WebApi.Helper
//{
//    public class PayRollCentralHelper
//    {
//        private static readonly ApplicationDbContext _db = new ApplicationDbContext();

//        public static List<GetSetupComponent> Calculate(int structureId, double grossSalery)
//        {
//            var GrossSalery = grossSalery;
//            var setupComponent = (from c in _db.SaleryStructureComponents
//                                  join r in _db.RecuringComponents on c.ComponentId equals r.RecuringComponentId
//                                  where c.CalculationDone == true && c.StructureId == structureId
//                                  select new GetSetupComponent
//                                  {
//                                      ComponentId = c.ComponentId,
//                                      CalculationType = c.CalculationType,
//                                      ComponentType = r.ComponentType,
//                                      AnnulCalculation = c.AnnulCalculation.Trim().Replace(" ", "_").ToUpper(),
//                                      //AnnulCalculation = c.CalculationType == CalculationTypeEnum.Fixed ?
//                                      //   (Double.Parse(c.AnnulCalculation) / 12).ToString("00.00") :
//                                      //   c.AnnulCalculation.Replace(" ", "_").ToUpper(),
//                                      ComponentName = ("[" + r.ComponentName + "]").Replace(" ", "_").ToUpper(),
//                                      CalculationFormula = "",
//                                      CalculatedValue = 0,
//                                  }).ToList();

//            foreach (var component in setupComponent)
//            {
//                if (component.CalculationType == CalculationTypeConstants.Fixed)
//                {
//                    component.CalculationFormula = (Double.Parse(component.AnnulCalculation) / 12).ToString();
//                    component.CalculatedValue = CalculateFormula(component.CalculationFormula);
//                }
//                else
//                {
//                    component.CalculationFormula = CheckBracketComponent(component.AnnulCalculation, setupComponent, GrossSalery);
//                    component.CalculatedValue = CalculateFormula(component.CalculationFormula);
//                }
//            }
//            setupComponent.ForEach(x => x.ComponentName = CultureInfo.CurrentCulture.TextInfo
//                    .ToTitleCase(x.ComponentName.Replace("_", " ").ToLower()).Replace("[", "").Replace("]", ""));

//            //GetSalarySlipModelClass response = GetSalarySlip(setupComponent);
//            return setupComponent;
//        }

//        public static readonly double CTC = GetCTCOfEmployee();

//        public static string CheckBracketComponent(string formula, List<GetSetupComponent> componentList, double grossSalery)
//        {
//            foreach (var item in componentList)
//                formula = formula.Replace(item.ComponentName.ToUpper(), "(" + item.AnnulCalculation + ")");
//            formula = formula.Replace("[GROSS]", grossSalery.ToString());
//            formula = formula.Replace("[CTC]", CTC.ToString());
//            if (formula.Contains("[") || formula.Contains("]"))
//                return CheckBracketComponent(formula, componentList, grossSalery);
//            return formula;
//        }

//        public static double CalculateFormula(string formula)
//        {
//            DataTable dt = new DataTable();
//            var data = double.Parse(dt.Compute(formula, "").ToString());
//            return data;
//        }

//        public static GetSalarySlipModelClass GetSalarySlip(List<GetSetupComponent> model)
//        {
//            GetSalarySlipModelClass res = new GetSalarySlipModelClass
//            {
//                TotalAddition = model.Where(x => x.ComponentType == "Fixed" || x.ComponentType == "Allowance").ToList(),
//                ExtraDeduction = model.Where(x => x.ComponentType == "Recurring Deduction").ToList(),
//                ExtraBonus = model.Where(x => x.ComponentType == "Reimbursable Component" || x.ComponentType == "Reimbursable Component").ToList(),
//            };
//            return res;
//        }
//        public static List<GetSalaryBreakDownParts> SalaryBreakdownBreakInParts(List<GetSetupComponent> model)
//        {
//            return model
//                .Select(x => new GetSalaryBreakDownParts
//                {
//                    Type = x.ComponentType == "Recurring Deduction" ? "Deductions" : "Earnings",
//                    ComponentName = x.ComponentName,
//                    MonthlyAmount = Math.Round(x.CalculatedValue / 12, 2),
//                    YearlyAmount = x.CalculatedValue,
//                })
//                .ToList();
//        }

//        public static GetSaleryBreakupModelClass GetSaleryBreakup(GetSalarySlipModelClass model)
//        {
//            GetSaleryBreakupModelClass res = new GetSaleryBreakupModelClass();
//            List<SaleryBreakupOuterModel> list = new List<SaleryBreakupOuterModel>();
//            if (model.TotalAddition.Count > 0)
//            {
//                SaleryBreakupOuterModel obj = new SaleryBreakupOuterModel
//                {
//                    Title = "Details",
//                    Inner = model.TotalAddition.Select(x => new SaleryBreakupInnerModel
//                    {
//                        Name = x.ComponentName,
//                        Monthly = Math.Round(x.CalculatedValue, 2),
//                        Anually = Math.Round(x.CalculatedValue * 12, 2),
//                    }).ToList(),
//                    TotalMonthly = model.TotalAddition.Select(x => x.CalculatedValue).Sum(),
//                    TotalAnnually = (model.TotalAddition.Select(x => x.CalculatedValue).Sum() * 12),
//                };
//                list.Add(obj);
//            }
//            res.NetMonthly = list.Select(x => x.TotalMonthly).Sum();
//            res.NetAnnually = list.Select(x => x.TotalAnnually).Sum();
//            res.Outer = list;
//            return res;
//        }

//        public static double GetCTCOfEmployee()
//        {
//            return 0;
//        }

//        public static bool CheckStructure(int payGroupId, int userId)
//        {
//            var checkStructure = _db.SaleryStructurePayRolls.Where(x => x.PayGroupId == payGroupId &&
//                        x.IsActive == true && x.IsDeleted == false).Select(x => x.StructureId).ToList();
//            var checkComponents = _db.SaleryStructureComponents.Where(x => checkStructure.Contains(x.StructureId) &&
//                                x.IsActive == true && x.IsDeleted == false).ToList();
//            if (checkComponents.Count > 0)
//            {
//                var checking = checkComponents.Select(x => x.CalculationDone).ToList();
//                if (!checking.Contains(false))
//                {
//                    var payRollComponentSetup = _db.PayRollSetups.Where(x => x.IsActive == true &&
//                                x.IsDeleted == false && x.PayGroupId == payGroupId && x.Step == (int)PayrollSetupConstants.SaleryStructure ||
//                                x.Step == (int)PayrollSetupConstants.FinanceSetting).ToList();
//                    foreach (var item in payRollComponentSetup)
//                    {
//                        item.UpdatedOn = DateTime.Now;
//                        item.UpdatedBy = userId;
//                        item.Status = Enum.GetName(typeof(PayrollSetupStatus), (int)PayrollSetupStatus.COMPLETED);

//                        _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
//                        _db.SaveChanges();
//                    }
//                    return true;
//                }
//                else
//                {
//                    var payRollComponentSetup = _db.PayRollSetups.Where(x => x.IsActive == true &&
//                                x.IsDeleted == false && x.PayGroupId == payGroupId && x.Step == (int)PayrollSetupConstants.SaleryStructure ||
//                                x.Step == (int)PayrollSetupConstants.FinanceSetting).ToList();
//                    foreach (var item in payRollComponentSetup)
//                    {
//                        item.UpdatedOn = DateTime.Now;
//                        item.UpdatedBy = userId;
//                        item.Status = Enum.GetName(typeof(PayrollSetupStatus), (int)PayrollSetupStatus.PENDING);

//                        _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
//                        _db.SaveChanges();
//                    }
//                    return false;
//                }
//            }
//            else
//            {
//                return false;
//            }
//        }

//        public static void CalculatedEmployeeSalery(int structureId, ClaimsHelperModel claims)
//        {
//            var employeeList = _db.Employee.Where(x => x.StructureId == structureId).ToList();
//            foreach (var item in employeeList)
//            {
//                RunPayRoll obj = new RunPayRoll
//                {
//                    EmployeeId = item.EmployeeId,
//                    PayGroupId = item.PayGroupId,
//                    StructureId = item.StructureId,
//                    NetSalery = 0,
//                    GrossSalery = 0,
//                    GrossDeductions = 0,
//                    PaySlipUrl = "",
//                    MonthYear = DateTime.Now.Date,
//                    Status = RunPayRollStatusConstants.Pre_Run_Complete,

//                    IsActive = true,
//                    IsDeleted = false,
//                    CreatedBy = claims.employeeId,
//                    CreatedOn = DateTime.Now,
//                    CompanyId = claims.companyId,
//                    OrgId = 0,
//                };
//                _db.RunPayRolls.Add(obj);
//                _db.SaveChanges();
//                var data = Calculate(structureId, item.GrossSalery / 12);
//                var res = CheckStructureType(data, obj);
//                var check = CalculateRunPayRollComponents(res, obj);
//            }
//        }

//        public static List<RunPayRollComponent> CheckStructureType(List<GetSetupComponent> model, RunPayRoll obj)
//        {
//            List<RunPayRollComponent> list = new List<RunPayRollComponent>();
//            foreach (var item in model)
//            {
//                RunPayRollComponent res = new RunPayRollComponent
//                {
//                    RunPayRollId = obj.Id,
//                    ComponentName = item.ComponentName,
//                    ComponentType = item.ComponentType,
//                    MonthlyAmount = item.CalculatedValue,

//                    IsActive = true,
//                    IsDeleted = false,
//                    CreatedBy = obj.CreatedBy,
//                    CreatedOn = DateTime.Now,
//                    CompanyId = obj.CompanyId,
//                    OrgId = 0,
//                };
//                switch (item.ComponentType)
//                {
//                    case "Fixed":
//                        res.Status = RunPayRollComponentStatus.Increasing;
//                        break;

//                    case "Allowance":
//                        res.Status = RunPayRollComponentStatus.Increasing;
//                        break;

//                    case "Reimbursable Component":
//                        res.Status = RunPayRollComponentStatus.Increasing;
//                        break;

//                    case "Recurring Deduction":
//                        res.Status = RunPayRollComponentStatus.Decreasing;
//                        break;

//                    case "Reimbursment":
//                        res.Status = RunPayRollComponentStatus.Increasing;
//                        break;
//                }
//                _db.RunPayRollComponents.Add(res);
//                _db.SaveChanges();
//                list.Add(res);
//            }
//            return list;
//        }

//        /// <summary>
//        /// Created By SurajBundel on 20-06-2022
//        /// convert number to words
//        /// </summary>
//        /// <returns></returns>
//        public static RunPayRoll CalculateRunPayRollComponents(List<RunPayRollComponent> model, RunPayRoll obj)
//        {
//            obj.GrossSalery = model.Where(x => x.Status == RunPayRollComponentStatus.Increasing).Select(x => x.MonthlyAmount).ToList().Sum();
//            obj.GrossDeductions = model.Where(x => x.Status == RunPayRollComponentStatus.Decreasing).Select(x => x.MonthlyAmount).ToList().Sum();
//            obj.NetSalery = obj.GrossSalery - obj.GrossDeductions;
//            _db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
//            _db.SaveChanges();
//            return obj;
//        }



//        public class GetSetupComponent
//        {
//            public int ComponentId { get; set; }
//            public CalculationTypeConstants CalculationType { get; set; }
//            public string ComponentName { get; set; }
//            public string ComponentType { get; set; }
//            public string AnnulCalculation { get; set; }
//            public string CalculationFormula { get; set; }
//            public double CalculatedValue { get; set; }
//        }

//        public class GetSalarySlipModelClass
//        {
//            public List<GetSetupComponent> TotalAddition { get; set; }
//            public List<GetSetupComponent> ExtraBonus { get; set; }
//            public List<GetSetupComponent> ExtraDeduction { get; set; }
//        }
//        public class GetSalaryBreakDownParts
//        {
//            public string Type { get; set; }
//            public string ComponentName { get; set; }
//            public double MonthlyAmount { get; set; }
//            public double YearlyAmount { get; set; }
//        }

//        public class GetSaleryBreakupModelClass
//        {
//            public List<SaleryBreakupOuterModel> Outer { get; set; }
//            public double NetMonthly { get; set; }
//            public double NetAnnually { get; set; }
//        }

//        public class SaleryBreakupOuterModel
//        {
//            public string Title { get; set; }
//            public List<SaleryBreakupInnerModel> Inner { get; set; }
//            public double TotalMonthly { get; set; }
//            public double TotalAnnually { get; set; }
//        }

//        public class SaleryBreakupInnerModel
//        {
//            public string Name { get; set; }
//            public double Monthly { get; set; }
//            public double Anually { get; set; }
//        }

//    }
//}