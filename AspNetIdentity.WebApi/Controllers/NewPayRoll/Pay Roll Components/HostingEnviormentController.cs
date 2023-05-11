using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.New_Pay_Roll;
using Newtonsoft.Json;
using NLog;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Controllers.NewPayRoll.Salary_Breakup.CalculationController;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.NewPayRoll.Pay_Roll_Components
{
    /// <summary>
    /// Created By Harshit Mitra On 03/01/2022
    /// </summary>
    public class HostingEnviormentController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region FUNCTION TO CHANGE STRUCTURE IF COMPONENT UPDATE
        public async Task<bool> ChangeInStructureRecuringComponent(RecuringComponent component, string oldCode)
        {
            try
            {
                oldCode = oldCode.Trim();
                var componentInStructure = await _db.SalaryStructureConfigs
                        .Where(x => x.CalculatingValue.Contains(oldCode))
                        .ToListAsync();
                componentInStructure.ForEach(x => x.CalculatingValue = x.CalculatingValue.Replace(oldCode, component.ComponentName.ToUpper().Replace(" ", "")));
                foreach (var item in componentInStructure)
                    _db.Entry(componentInStructure).State = EntityState.Modified;

                var structureList = componentInStructure.Select(x => x.StructureId).Distinct().ToList();

                var componentInBreakDown = await (from sb in _db.SalaryBreakDowns
                                                  join em in _db.Employee on sb.EmployeeId equals em.EmployeeId
                                                  where sb.IsCurrentlyUse && structureList.Contains(em.StructureId)
                                                  select sb).ToListAsync();
                componentInBreakDown
                    .SelectMany(x => JsonConvert.DeserializeObject<List<GetSetupProperties>>(x.Earnings))
                    .ToList()
                    .Where(x => x.ComponentId == component.RecuringComponentId)
                    .ToList()
                    .ForEach(x =>
                    {
                        x.ComponentName = component.ComponentName;
                        x.ComponentCode = "[" + component.ComponentName.ToUpper().Replace(" ", "") + "]";
                    });
                componentInBreakDown
                   .SelectMany(x => JsonConvert.DeserializeObject<List<GetSetupProperties>>(x.Earnings))
                   .ToList()
                   .Where(x => x.CalculatingValue.Contains(oldCode))
                   .ToList()
                   .ForEach(x =>
                   {
                       x.CalculatingValue = x.CalculatingValue.Replace(oldCode, component.ComponentName.ToUpper().Replace(" ", ""));
                   });
                foreach (var item in componentInBreakDown)
                {
                    var check = JsonConvert.DeserializeObject<List<GetSetupProperties>>(item.Earnings);
                    foreach (var x in check)
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
                                x.Formula = ChangeComponentToFomula(x.CalculatingValue, item.GrossYearly, check);
                                x.AfterCalculation = ConversionStringToValue(x.Formula);
                            }
                        }
                    }
                    _db.Entry(item).State = EntityState.Modified;
                }
                //await _db.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        #endregion

    }
}
