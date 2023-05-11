using AspNetIdentity.WebApi.Infrastructure;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using static AspNetIdentity.WebApi.Controllers.NewPayRoll.Salary_Breakup.RunPayRollController;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.NewPayRoll.Salary_Breakup
{
    /// <summary>
    /// Created By Harshit Mitra On 21/12/2022
    /// </summary>
    public class CalculationController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        #region FUNCTION TO CALCULATE SALARY BREAKDOWN 
        public List<GetSetupProperties> Calculate(Guid structureId, double grossSalary)
        {
            var structure = _db.SalaryStructures.FirstOrDefault(x => x.StructureId == structureId);
            List<GetSetupProperties> listComponents = new List<GetSetupProperties>();
            listComponents = (from sc in _db.SalaryStructureConfigs
                              join rc in _db.RecuringComponents on sc.ComponentId equals rc.RecuringComponentId
                              //join cp in _db.ComponentInPays on sc.ComponentId equals cp.ComponentId
                              where sc.StructureId == structureId &&
                                    sc.ComponentType == ComponentTypeInPGConstants.RecurringComponent
                              select new GetSetupProperties
                              {
                                  PayGroupId = structure.PayGroupId,
                                  ComponentId = sc.ComponentId,
                                  ComponentName = rc.ComponentName,
                                  ComponentCode = "[" + rc.ComponentName.Replace(" ", "").ToUpper() + "]",
                                  TaxSettings = "",
                                  CalculationType = sc.CalculationType ? "FORMULA" : "FIXED",
                                  ComponentType = sc.ComponentType,
                                  RecuringComponentType = rc.ComponentType,
                                  CalculatingValue = sc.CalculatingValue.Replace(" ", "_").ToUpper(),
                                  Formula = "",
                                  AfterCalculation = 0,
                                  IncludeInGross = rc.ShowOnPaySlip,
                              })
                              .Distinct()
                              .ToList();
            //if (check != 1)
            //{
            //    var taxComponents = (from sc in _db.SalaryStructureConfigs
            //                         join tc in _db.TaxDeductions on sc.ComponentId equals tc.TaxComponentId
            //                         join cp in _db.ComponentInPays on sc.ComponentId equals cp.ComponentId
            //                         where sc.StructureId == structureId &&
            //                               sc.ComponentType == ComponentTypeInPGConstants.TaxDeductionComponent
            //                         select new GetSetupProperties
            //                         {
            //                             ComponentId = sc.ComponentId,
            //                             ComponentName = tc.DeductionName,
            //                             ComponentCode = "[" + tc.DeductionName.Replace(" ", "").ToUpper() + "]",
            //                             TaxSettings = sc.TaxSettings,
            //                             CalculationType = sc.CalculationType ? "FORMULA" : "FIXED",
            //                             ComponentType = sc.ComponentType,
            //                             RecuringComponentType = PayRollCompnentTypeConstants.Other,
            //                             CalculatingValue = sc.CalculatingValue.Replace(" ", "_").ToUpper(),
            //                             Formula = "",
            //                             AfterCalculation = 0,
            //                         })
            //                         .Distinct()
            //                         .ToList();
            //    listComponents.AddRange(taxComponents);
            //}
            foreach (var x in listComponents)
            {
                if (x.ComponentType == ComponentTypeInPGConstants.RecurringComponent)
                {
                    if (x.CalculationType == "FIXED")
                    {
                        x.Formula = x.CalculatingValue;
                        x.AfterCalculation = ConversionStringToValue(x.Formula);
                    }
                    else
                    {
                        x.Formula = ChangeComponentToFomula(x.CalculatingValue, grossSalary, listComponents);
                        x.AfterCalculation = ConversionStringToValue(x.Formula);
                    }
                }
            }
            //double checkMonthly = listComponents
            //    .Where(x => x.ComponentType == ComponentTypeInPGConstants.RecurringComponent
            //            && x.RecuringComponentType != PayRollCompnentTypeConstants.Recurring_Deduction)
            //    .Sum(x => x.MontlyCalculation) -
            //    listComponents
            //    .Where(x => x.ComponentType == ComponentTypeInPGConstants.RecurringComponent
            //            && x.RecuringComponentType == PayRollCompnentTypeConstants.Recurring_Deduction)
            //    .Sum(x => x.MontlyCalculation);
            //foreach (var x in listComponents)
            //{
            //    if (x.ComponentType == ComponentTypeInPGConstants.TaxDeductionComponent)
            //    {
            //        dynamic taxSetting = JsonConvert.DeserializeObject(x.TaxSettings);
            //        if (x.CalculationType == "FIXED")
            //        {
            //            if (taxSetting.Min < taxSetting.Max)
            //            {
            //                if (checkMonthly >= taxSetting.Min && checkMonthly <= taxSetting.Max)
            //                    x.MontlyCalculation = Double.Parse(x.CalculatingValue);
            //                else
            //                    listComponents.Remove(x);
            //            }
            //            if (taxSetting.Min != 0 & taxSetting.Max == 0)
            //            {
            //                if (checkMonthly <= taxSetting.Min)
            //                    x.MontlyCalculation = Double.Parse(x.CalculatingValue);
            //                else
            //                    listComponents.Remove(x);
            //            }
            //            if (taxSetting.Min == 0 & taxSetting.Max == 0)
            //            {
            //                x.MontlyCalculation = Double.Parse(x.CalculatingValue);
            //            }
            //        }
            //        else
            //        {

            //        }
            //    }
            //}
            return listComponents;
        }
        public static double ConversionStringToValue(string formula)
        {
            DataTable dt = new DataTable();
            var value = double.Parse(dt.Compute(formula, "").ToString());
            return Math.Round(value, 2);
        }
        public static string ChangeComponentToFomula(string calculatingValue, double grossSalary, List<GetSetupProperties> componentList)
        {
            try
            {
                foreach (var component in componentList)
                    calculatingValue = calculatingValue.Replace(component.ComponentCode, "(" + component.CalculatingValue + ")");
                calculatingValue = calculatingValue.Replace("[GROSS]", grossSalary.ToString());
                if (calculatingValue.Contains("[") || calculatingValue.Contains("]"))
                    calculatingValue = ChangeComponentToFomula(calculatingValue, grossSalary, componentList);
                return calculatingValue;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public class GetSetupProperties
        {
            public Guid PayGroupId { get; set; }
            public Guid ComponentId { get; set; }
            public string ComponentName { get; set; }
            public string ComponentCode { get; set; }
            public string TaxSettings { get; set; }
            public string CalculationType { get; set; }
            public ComponentTypeInPGConstants ComponentType { get; set; }
            public PayRollComponentTypeConstants RecuringComponentType { get; set; }
            public string CalculatingValue { get; set; }
            public string Formula { get; set; }
            public double AfterCalculation { get; set; }
            public double MontlyCalculation { get; set; } = 0.0;
            public bool IncludeInGross { get; set; } = true;
        }
        #endregion

        #region FUNCTION FOR CALCULATION SALARY COMPONENTS
        public ReturnCalculatedComponents GetSalaryStructureTypes(List<GetSetupProperties> listModel)
        {
            listModel.ForEach(x => x.MontlyCalculation = Math.Round(x.AfterCalculation / 12, 2));
            return new ReturnCalculatedComponents
            {
                Earnnings = listModel
                    .Where(x => x.RecuringComponentType != PayRollComponentTypeConstants.Recurring_Deduction &&
                            x.RecuringComponentType != PayRollComponentTypeConstants.Other)
                    .ToList(),
                Deductions = listModel
                    .Where(x => x.RecuringComponentType == PayRollComponentTypeConstants.Recurring_Deduction)
                    .ToList(),
                Others = listModel
                    .Where(x => x.RecuringComponentType != PayRollComponentTypeConstants.Recurring_Deduction &&
                            x.RecuringComponentType == PayRollComponentTypeConstants.Other)
                    .ToList(),
            };
        }
        public class ReturnCalculatedComponents
        {
            public List<GetSetupProperties> Earnnings { get; set; }
            public List<GetSetupProperties> Deductions { get; set; }
            public List<GetSetupProperties> Others { get; set; }
        }
        #endregion

        #region FUNCTION TO GET SALARY BREAKDOWN IN PARTS
        public List<GetSalaryBreakDownParts> SalaryBreakdownBreakInParts(List<GetSetupProperties> model)
        {
            return model
                .Select(x => new GetSalaryBreakDownParts
                {
                    Type = x.RecuringComponentType == PayRollComponentTypeConstants.Recurring_Deduction ? "Deductions" :
                    (x.RecuringComponentType == PayRollComponentTypeConstants.Other && !x.IncludeInGross) ? "Other Benifits" : "Earnings",
                    ComponentName = x.IncludeInGross ? x.ComponentName : x.ComponentName + " (This will be excluded from gross calclation)",
                    MonthlyAmount = Math.Round(x.AfterCalculation / 12, 2),
                    YearlyAmount = x.AfterCalculation,
                    IncludeInGross = x.IncludeInGross,
                })
                .ToList();
        }
        public class GetSalaryBreakDownParts
        {
            public string Type { get; set; }
            public string ComponentName { get; set; }
            public double MonthlyAmount { get; set; }
            public double YearlyAmount { get; set; }
            public bool IncludeInGross { get; set; }
        }
        #endregion

        #region AMOUNT IN WORDS CONERTER
        public string AmountToIndianCurrencyString(long number)
        {
            string words = "";
            var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven",
                "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen",
                "sixteen", "seventeen", "eighteen", "nineteen" };
            var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty",
                "sixty", "seventy", "eighty", "ninety" };

            if (number == 0)
            {
                words = "zero";
                return words.ToUpper();
            }
            if (number < 0)
            {
                return ("minus " + AmountToIndianCurrencyString(Math.Abs(number))).ToUpper();
            }
            if ((number / 1000000000) > 0)
            {
                words += AmountToIndianCurrencyString(number / 1000000000) + " arab ";
                number %= 1000000000;
            }
            if ((number / 10000000) > 0)
            {
                words += AmountToIndianCurrencyString(number / 10000000) + " crores ";
                number %= 10000000;
            }
            if ((number / 100000) > 0)
            {
                words += AmountToIndianCurrencyString(number / 100000) + " lakhs ";
                number %= 100000;
            }
            if ((number / 1000) > 0)
            {
                words += AmountToIndianCurrencyString(number / 1000) + " thousand ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += AmountToIndianCurrencyString(number / 100) + " hundred ";
                number %= 100;
            }
            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                if (number < 20)
                {
                    words += unitsMap[number];
                }
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }
            return words.Trim().ToUpper();
        }
        #endregion

        #region FUNCTION TO CALCULATED TAX IN SALARY BREAKDOWN
        public List<GetSetupProperties> CalculateTaxInStructure(string month, dynamic property, dynamic payGroupSetting, List<GetSetupProperties> taxComponents, List<CountryAndStates> countryStateList)
        {
            try
            {
                double grossMonthly = property.GrossMonthly;
                List<GetSetupProperties> recurringComponents = property.earnings;
                if (property.deductions.Count > 0)
                    recurringComponents.AddRange(property.deductions);
                List<GetSetupProperties> CheckComponentCaseA = new List<GetSetupProperties>();
                if (taxComponents.Count > 0)
                {
                    foreach (var t in taxComponents)
                    {
                        dynamic taxSetting = JsonConvert.DeserializeObject(t.TaxSettings);
                        if ((string)taxSetting.Deductionfor == "Employee")
                        {
                            if ((int)taxSetting.CountryId == payGroupSetting.CompanyInfos.CountryId)
                            {
                                if ((int)taxSetting.StateId != 0)
                                {
                                    if (payGroupSetting.PtSetting.StateId == (int)taxSetting.StateId)
                                    {
                                        if (!String.IsNullOrEmpty((string)taxSetting.MonthName))
                                        {
                                            if (taxSetting.MonthName == month)
                                            {
                                                CheckComponentCaseA.Add(t);
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            CheckComponentCaseA.Add(t);
                                            continue;
                                        }
                                    }
                                    else
                                        continue;
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty((string)taxSetting.MonthName))
                                    {
                                        if (taxSetting.MonthName == month)
                                        {
                                            CheckComponentCaseA.Add(t);
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        CheckComponentCaseA.Add(t);
                                        continue;
                                    }
                                }
                            }
                            else
                                continue;
                        }
                        else
                            continue;
                    }
                }
                taxComponents = CheckComponentCaseA;
                List<GetSetupProperties> CheckComponentCaseB = new List<GetSetupProperties>();
                if (taxComponents.Count > 0)
                {
                    foreach (var t in taxComponents)
                    {
                        dynamic taxSetting = JsonConvert.DeserializeObject(t.TaxSettings);
                        if (t.CalculationType == "FIXED")
                        {
                            if ((double)taxSetting.Min < (double)taxSetting.Max)
                            {
                                if (grossMonthly >= (double)taxSetting.Min && grossMonthly <= (double)taxSetting.Max)
                                {
                                    t.MontlyCalculation = Double.Parse(t.CalculatingValue);
                                    CheckComponentCaseB.Add(t);
                                    continue;
                                }
                                else
                                    continue;
                            }
                            if ((double)taxSetting.Min != 0 & (double)taxSetting.Max == 0)
                            {
                                if (grossMonthly >= (double)taxSetting.Min)
                                {
                                    t.MontlyCalculation = Double.Parse(t.CalculatingValue);
                                    CheckComponentCaseB.Add(t);
                                    continue;
                                }
                                else
                                    continue;
                            }
                            if ((double)taxSetting.Min == 0 & (double)taxSetting.Max == 0)
                            {
                                t.MontlyCalculation = Double.Parse(t.CalculatingValue);
                                CheckComponentCaseB.Add(t);
                                continue;
                            }
                            if ((double)taxSetting.Min > (double)taxSetting.Max)
                                continue;
                        }
                        else
                        {
                            if ((double)taxSetting.Min < (double)taxSetting.Max)
                            {
                                if (grossMonthly >= (double)taxSetting.Min && grossMonthly <= (double)taxSetting.Max)
                                {
                                    t.Formula = ChangeComponentToFomula(t.CalculatingValue, grossMonthly, recurringComponents);
                                    t.MontlyCalculation = ConversionStringToValue(t.Formula);
                                    CheckComponentCaseB.Add(t);
                                    continue;
                                }
                                else
                                    continue;
                            }
                            if ((double)taxSetting.Min != 0 & (double)taxSetting.Max == 0)
                            {
                                if (grossMonthly >= (double)taxSetting.Min)
                                {
                                    t.Formula = ChangeComponentToFomula(t.CalculatingValue, grossMonthly, recurringComponents);
                                    t.MontlyCalculation = ConversionStringToValue(t.Formula);
                                    CheckComponentCaseB.Add(t);
                                    continue;
                                }
                                else
                                    continue;
                            }
                            if ((double)taxSetting.Min == 0 & (double)taxSetting.Max == 0)
                            {
                                t.Formula = ChangeComponentToFomula(t.CalculatingValue, grossMonthly, recurringComponents);
                                t.MontlyCalculation = ConversionStringToValue(t.Formula);
                                CheckComponentCaseB.Add(t);
                                continue;
                            }
                            if ((double)taxSetting.Min > (double)taxSetting.Max)
                                continue;
                        }
                    }
                }
                return CheckComponentCaseB;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}
