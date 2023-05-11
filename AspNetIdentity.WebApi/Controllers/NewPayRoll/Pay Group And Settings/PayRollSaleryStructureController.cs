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
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.NewPayRoll.PayGroupAndSettings
{
    /// <summary>
    /// Created By Harshit Mitra On 19/12/2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/salerystructure")]
    public class PayRollSaleryStructureController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        /// ----------------- Salary Structure CRUD ------------------ ///

        #region API TO ADD SALARY STRUCTURE 
        /// <summary>
        /// Created By Harshit Mitra on 06-06-2022
        /// API >> Post >> api/salerystructure/addsalarystructure
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addsalarystructure")]
        public async Task<IHttpActionResult> AddSaleryStructure(CreateSalaryStructureRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                SalaryStructure obj = new SalaryStructure
                {
                    PayGroupId = model.PayGroupId,
                    StructureName = model.StructureName,
                    Description = model.Description,

                    CompanyId = tokenData.companyId,
                    CreatedBy = tokenData.employeeId,
                };

                _db.SalaryStructures.Add(obj);
                await _db.SaveChangesAsync();

                res.Message = "Structure Created";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                res.Data = obj;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerystructure/addsalarystructure | " +
                     "Model : " + JsonConvert.SerializeObject(model) + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO EDIT SALARY STRUCTURE 
        /// <summary>
        /// Created By Harshit Mitra on 06-06-2022
        /// API >> Post >> api/salerystructure/updatesalerystructure
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updatesalerystructure")]
        public async Task<IHttpActionResult> UpdateSalaryStructure(EditSalaryStructureRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var salaryStructure = await _db.SalaryStructures
                    .FirstOrDefaultAsync(x => x.StructureId == model.StructureId);
                if (salaryStructure == null)
                {
                    res.Message = "Salary Structure Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;

                    return Ok(res);
                }

                salaryStructure.StructureName = model.StructureName;
                salaryStructure.Description = model.Description;

                salaryStructure.UpdatedBy = tokenData.employeeId;
                salaryStructure.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);

                _db.Entry(salaryStructure).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "Structure Updated";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;
                res.Data = salaryStructure;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerystructure/updatesalerystructure | " +
                     "Model : " + JsonConvert.SerializeObject(model) + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO DELETE SALARY SRUCTURE BY ID
        /// <summary>
        /// Created By Harshit Mitra on 06-06-2022
        /// API >> Post >> api/salerystructure/deletesalarystructure
        /// </summary>
        /// <param name="structureId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deletesalarystructure")]
        public async Task<IHttpActionResult> DeleteSalaryStructure(Guid structureId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var salaryStructure = await _db.SalaryStructures
                    .FirstOrDefaultAsync(x => x.StructureId == structureId);
                if (salaryStructure == null)
                {
                    res.Message = "Structure Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;

                    return Ok(res);
                }
                if (_db.Employee.Count(x => x.StructureId == structureId) > 0)
                {
                    res.Message = "Structure Cannot Be Deleted Because Its Assign To Employees";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotAcceptable;

                    return Ok(res);
                }

                salaryStructure.IsActive = false;
                salaryStructure.IsDeleted = true;

                salaryStructure.DeletedBy = tokenData.employeeId;
                salaryStructure.DeletedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);

                _db.Entry(salaryStructure).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "Structure Deleted";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerystructure/deletesalarystructure | " +
                     "StructureId : " + structureId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET STRUCTURE BY ID
        /// <summary>
        /// Created By Harshit Mitra on 06-06-2022
        /// API >> Post >> api/salerystructure/getstructurebyid
        /// </summary>
        /// <param name="structureId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getstructurebyid")]
        public async Task<IHttpActionResult> GetSaleryStructureById(Guid structureId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var salaryStructure = await _db.SalaryStructures
                    .FirstOrDefaultAsync(x => x.StructureId == structureId);
                if (salaryStructure == null)
                {
                    res.Message = "Structure Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;

                    return Ok(res);
                }

                res.Message = "Structure Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = salaryStructure;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerystructure/getstructurebyid | " +
                     "Structure Id : " + structureId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET ALL SALARY STRUCTURE LIST
        /// <summary>
        /// Created By Harshit Mitra On 19/12/2022
        /// API >> GET >> api/salerystructure/getsalerystructurelist
        /// </summary>
        /// <param name="payGroupId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getsalerystructurelist")]
        public async Task<IHttpActionResult> GetSaleryStructureList(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var saleryStructureList = await _db.SalaryStructures
                    .Where(x => x.PayGroupId == payGroupId && !x.IsDeleted && x.IsActive)
                    .Select(x => new
                    {
                        x.StructureId,
                        x.PayGroupId,
                        x.StructureName,
                        x.Description,
                        NumberOfEmployees = _db.Employee.Count(z => z.StructureId == x.StructureId),
                        x.CreatedOn,
                        x.UpdatedOn,
                        x.IsCompleted,
                        CreatedBy = x.CreatedBy,
                        UpdateBy = x.UpdatedBy.HasValue ? (int)x.UpdatedBy : 0,

                    }).ToListAsync();
                if (saleryStructureList.Count == 0)
                {
                    res.Message = "No Structure Found";
                    res.Status = false;
                    res.StatusCode = System.Net.HttpStatusCode.NotFound;
                    res.Data = saleryStructureList;

                    return Ok(res);
                }
                res.Message = "Structures Found";
                res.Status = true;
                res.StatusCode = System.Net.HttpStatusCode.OK;
                res.Data = saleryStructureList
                    .Select(x => new
                    {
                        x.StructureId,
                        x.PayGroupId,
                        x.StructureName,
                        x.Description,
                        x.NumberOfEmployees,
                        x.CreatedOn,
                        x.UpdatedOn,
                        x.IsCompleted,
                        CreatedBy = _db.GetEmployeeNameById(x.CreatedBy),
                        UpdatedBy = x.UpdateBy == 0 ? String.Empty : _db.GetEmployeeNameById(x.UpdateBy),

                    }).ToList();

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerystructure/getsalerystructurelist | " +
                     "Pay Group Id : " + payGroupId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO CHECK STRUCTURE IS COMPLETE OR NOT 
        /// <summary>
        /// Created By Harshit Mitra on 21/12/2022
        /// API >> Post >> api/salerystructure/checkstructure
        /// </summary>
        /// <param name="structureId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("checkstructure")]
        public async Task<IHttpActionResult> CheckStructureIsCompletedOrNot(Guid structureId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var salaryStructure = await _db.SalaryStructures
                    .FirstOrDefaultAsync(x => x.StructureId == structureId);
                var componentInStructure = await (from ss in _db.SalaryStructures
                                                  join sc in _db.SalaryStructureConfigs on ss.StructureId equals sc.StructureId
                                                  where ss.StructureId == structureId && !ss.IsDeleted && ss.IsActive
                                                  select new
                                                  {
                                                      sc.ComponentType,
                                                      sc.IsCalculationDone,
                                                  })
                                                  .ToListAsync();
                //if (componentInStructure.Count(x=> x.ComponentType == ComponentTypeInPGConstants.RecurringComponent) == 0)
                //{
                //    res.Message = "You Have To Add Atleast One Recuring Component In Structure";
                //    res.Status = false;
                //    res.StatusCode = HttpStatusCode.NotAcceptable;

                //    return Ok(res);
                //}
                if (componentInStructure.Any(x => !x.IsCalculationDone))
                {
                    res.Message = "Structure Is Not Completed";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotAcceptable;

                    return Ok(res);
                }

                salaryStructure.IsCompleted = true;
                salaryStructure.UpdatedBy = tokenData.employeeId;
                salaryStructure.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);

                _db.Entry(salaryStructure).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "Structure Completed";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerystructure/checkstructure | " +
                     "Structure Id : " + structureId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion


        /// ------------------- Structure Config. -------------------- ///

        #region API TO GET COMPONENT LIST ON SALARY STRUCTURE BEFOR AND AFTER ADDING COMPONENTS
        /// <summary>
        /// Created By Harshit Mitra On 19/12/2022
        /// API >> GET >> api/salerystructure/getcomponents
        /// </summary>
        /// <param name="payGroupId"></param>
        /// <param name="structureId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcomponents")]
        public async Task<IHttpActionResult> GetRecuringComponetOnAddingComponentInStruture(Guid payGroupId, Guid structureId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var componentInStructure = await _db.SalaryStructureConfigs
                    .Where(x => x.StructureId == structureId &&
                        x.ComponentType == Model.EnumClass.ComponentTypeInPGConstants.RecurringComponent)
                    .Select(x => x.ComponentId)
                    .ToListAsync();
                var rescuringComponent = await (from cp in _db.ComponentInPays
                                                join rc in _db.RecuringComponents on cp.ComponentId equals rc.RecuringComponentId
                                                where cp.ComponentType == Model.EnumClass.ComponentTypeInPGConstants.RecurringComponent
                                                    && cp.CompanyId == tokenData.companyId && cp.PayGroupId == payGroupId
                                                select new
                                                {
                                                    cp.ComponentId,
                                                    rc.ComponentName,
                                                    rc.ComponentType,
                                                    rc.IsTaxExempted,
                                                    rc.IncomeTaxSection,
                                                    rc.IsAutoCalculated,
                                                    rc.MaxiumLimitPerYear,
                                                    Calculation = (!rc.IsAutoCalculated) ? rc.MaxiumLimitPerYear.ToString() : "Calculation Required"

                                                }).ToListAsync();
                if (componentInStructure.Count == 0)
                {
                    res.Message = "Component List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = rescuringComponent
                        .Select(x => new
                        {
                            ComponentId = x.ComponentId,
                            ComponentName = x.ComponentName,
                            ComponentType = x.ComponentType,
                            IsTaxExempted = x.IsTaxExempted,
                            IncomeTaxSection = x.IncomeTaxSection,
                            IsAutoCalculated = x.IsAutoCalculated,
                            MaxiumLimitPerYear = x.MaxiumLimitPerYear,
                            CalculatingValue = (!x.IsAutoCalculated) ? x.MaxiumLimitPerYear.ToString() : "Calculation Required",
                            IsChecked = false,

                        })
                        .OrderByDescending(x => x.IsChecked).ThenBy(x => x.ComponentName)
                        .ToList();
                }
                else
                {
                    res.Message = "Component List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = rescuringComponent
                        .Select(x => new
                        {
                            ComponentId = x.ComponentId,
                            ComponentName = x.ComponentName,
                            ComponentType = x.ComponentType,
                            IsTaxExempted = x.IsTaxExempted,
                            IncomeTaxSection = x.IncomeTaxSection,
                            IsAutoCalculated = x.IsAutoCalculated,
                            MaxiumLimitPerYear = x.MaxiumLimitPerYear,
                            CalculatingValue = (!x.IsAutoCalculated) ? x.MaxiumLimitPerYear.ToString() : "Calculation Required",
                            IsChecked = componentInStructure.Contains(x.ComponentId),

                        })
                        .OrderByDescending(x => x.IsChecked)
                        .ToList();
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerystructure/getcomponents | " +
                     "Pay Group Id : " + payGroupId + " | " +
                     "Structure Id : " + structureId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO ADD AND UPDATE COMPONENT IN SALARY STRUCTURE 
        /// <summary>
        /// Created By Harshit Mitra On 19/12/2022
        /// API >> GET >> api/salerystructure/addupdatecomponent
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("addupdatecomponent")]
        public async Task<IHttpActionResult> AddUpdateComponentList(AddComponentInStructureRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var componentInStructure = await _db.SalaryStructureConfigs
                    .Where(x => x.StructureId == model.StructureId &&
                        x.ComponentType == ComponentTypeInPGConstants.RecurringComponent)
                    .ToListAsync();

                List<AddComponentInStructureListRequest> addComponentList = model.ComponentList.Where(x => x.IsChecked).ToList();
                if (componentInStructure.Count != 0)
                {
                    var checkIds = componentInStructure.Select(x => x.ComponentId).ToList();
                    var newComponent = addComponentList.Where(x => x.IsChecked == true).Select(x => x.ComponentId).ToList();

                    var removeData = checkIds.Where(x => !newComponent.Contains(x)).ToList();
                    var addingData = newComponent.Where(x => !checkIds.Contains(x)).ToList();

                    addComponentList = addComponentList.Where(x => addingData.Contains(x.ComponentId) && x.IsChecked == true).ToList();
                    var removeComponentList = componentInStructure.Where(x => removeData.Contains(x.ComponentId)).ToList();

                    _db.SalaryStructureConfigs.RemoveRange(removeComponentList);
                }

                var addComponent = addComponentList
                    .Where(x => x.IsChecked)
                    .Select(x => new SalaryStructureConfig
                    {
                        StructureId = model.StructureId,
                        ComponentId = x.ComponentId,
                        ComponentType = ComponentTypeInPGConstants.RecurringComponent,
                        CalculationType = x.IsAutoCalculated,
                        CalculatingValue = x.CalculatingValue,
                        IsCalculationDone = !x.IsAutoCalculated,

                        CreatedBy = tokenData.employeeId,
                        CompanyId = tokenData.companyId,

                    }).ToList();

                _db.SalaryStructureConfigs.AddRange(addComponent);
                await _db.SaveChangesAsync();

                res.Message = "Component Added In Structure";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerystructure/getcomponents | " +
                     "Model : " + JsonConvert.SerializeObject(model) + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET ADDED SALARY COMPONENT 
        /// <summary>
        /// Created By Harshit Mitra On 19/12/2022
        /// API >> GET >> api/salerystructure/getselectedcomponent
        /// </summary>
        /// <param name="structureId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getselectedcomponent")]
        public async Task<IHttpActionResult> GetAddedSalaryComponent(Guid structureId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var componentInStructure = await (from sc in _db.SalaryStructureConfigs
                                                  join rc in _db.RecuringComponents on sc.ComponentId equals rc.RecuringComponentId
                                                  join em in _db.Employee on sc.CreatedBy equals em.EmployeeId
                                                  where sc.ComponentType == ComponentTypeInPGConstants.RecurringComponent
                                                      && sc.CompanyId == tokenData.companyId && sc.StructureId == structureId
                                                  select new
                                                  {
                                                      sc.ComponentId,
                                                      rc.ComponentName,
                                                      sc.CalculatingValue,
                                                      sc.IsCalculationDone,
                                                      CreatedByName = em.DisplayName,
                                                      sc.CreatedOn,

                                                  }).ToListAsync();
                if (componentInStructure.Count == 0)
                {
                    res.Message = "Component List Is Empty";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = componentInStructure;

                    return Ok(res);
                }
                res.Message = "Component List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = componentInStructure
                    .Select(x => new
                    {
                        x.ComponentId,
                        x.ComponentName,
                        x.CalculatingValue,
                        x.IsCalculationDone,
                        x.CreatedByName,
                        x.CreatedOn,
                    })
                    .ToList();
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerystructure/getcomponents | " +
                     "Structure Id : " + structureId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET COMPONENT BY ID FOR EDIT 
        /// <summary>
        /// Created By Harshit Mitra On 19/12/2022
        /// API >> GET >> api/salerystructure/getselectedcomponentbyid
        /// </summary>
        /// <param name="structureId"></param>
        /// <param name="componentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getselectedcomponentbyid")]
        public async Task<IHttpActionResult> GetAddedSalaryComponent(Guid structureId, Guid componentId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var componentInStructure = await _db.SalaryStructureConfigs
                     .FirstOrDefaultAsync(x => x.StructureId == structureId &&
                             x.ComponentId == componentId);
                if (componentInStructure == null)
                {
                    res.Message = "Component Not Found";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = componentInStructure;

                    return Ok(res);
                }
                res.Message = "Component Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = componentInStructure;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerystructure/getselectedcomponentbyid | " +
                     "Structure Id : " + structureId + " | " +
                     "Component Id : " + componentId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO ADD AND UPDATE COMPONENT IN SALARY STRUCTURE 
        /// <summary>
        /// Created By Harshit Mitra On 19/12/2022
        /// API >> POST >> api/salerystructure/updatecalculations
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updatecalculations")]
        public async Task<IHttpActionResult> UpdateCalculation(UpdateCalculationRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var componentInStructure = await _db.SalaryStructureConfigs
                    .FirstOrDefaultAsync(x => x.StructureId == model.StructureId &&
                            x.ComponentId == model.ComponentId);
                if (componentInStructure == null)
                {
                    res.Message = "Component Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;

                    return Ok(res);
                }
                if (!CheckBracketCount(componentInStructure.CalculatingValue))
                {
                    res.Message = "Component Formula Is Incorect";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotAcceptable;

                    return Ok(res);
                }
                componentInStructure.CalculatingValue = model.CalculatingValue.ToUpper().Replace(" ", "");
                componentInStructure.IsCalculationDone = true;

                componentInStructure.UpdatedBy = tokenData.employeeId;
                componentInStructure.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);

                _db.Entry(componentInStructure).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "Component Calculation Updated";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerystructure/updatecalculations | " +
                     "Model : " + JsonConvert.SerializeObject(model) + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public bool CheckBracketCount(string calculatingValue)
        {
            int rhsB = calculatingValue.Count(x => x == '[');
            int lhsB = calculatingValue.Count(x => x == ']');
            int rhsC = calculatingValue.Count(x => x == '{');
            int lhsC = calculatingValue.Count(x => x == '}');
            int rhsS = calculatingValue.Count(x => x == '(');
            int lhsS = calculatingValue.Count(x => x == ')');
            return (rhsB == lhsB && rhsC == lhsC && rhsS == lhsS);
        }
        #endregion

        #region API TO DELETE COMPONENT IN STRUCTURE 
        /// <summary>
        /// Created By Harshit Mitra On 19/12/2022
        /// API >> POST >> api/salerystructure/deletecomponent
        /// </summary>
        /// <param name="structureId"></param>
        /// <param name="componentId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deletecomponent")]
        public async Task<IHttpActionResult> DeleteComponent(Guid structureId, Guid componentId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var componentInStructure = await _db.SalaryStructureConfigs
                    .FirstOrDefaultAsync(x => x.StructureId == structureId &&
                            x.ComponentId == componentId);
                if (componentInStructure == null)
                {
                    res.Message = "Component Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;

                    return Ok(res);
                }
                _db.SalaryStructureConfigs.Remove(componentInStructure);
                await _db.SaveChangesAsync();

                res.Message = "Component Removed";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerystructure/deletecomponent | " +
                     "Structure Id : " + structureId + " | " +
                     "Component Id : " + componentId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO CHECK STRUCTURE IS COMPLETED OR NOT 
        /// <summary>
        /// Created By Harshit Mitra On 19/12/2022
        /// API >> POST >> api/salerystructure/checksetup
        /// </summary>
        /// <param name="payGroupId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("checksetup")]
        public async Task<IHttpActionResult> CheckFormulaIsAddedOrNot(Guid payGroupId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var structureList = await _db.SalaryStructures
                    .Where(x => x.PayGroupId == payGroupId && x.IsActive && !x.IsDeleted)
                    .ToListAsync();

                if (structureList.Any(z => !z.IsCompleted))
                {
                    res.Message = "Structure Is Not Completed";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotAcceptable;

                    return Ok(res);
                }
                var setup = await _db.PayGroupSetups
                   .Where(x => x.PayGroupId == payGroupId
                       && (x.StepsInSettings == PayRollSetupConstants.Salary_Structure
                       || x.StepsInSettings == PayRollSetupConstants.Finance_Setting))
                   .ToListAsync();
                setup.ForEach(x => x.IsSetupComplete = true);

                _db.Entry(setup[0]).State = EntityState.Modified;
                _db.Entry(setup[1]).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "Structure Is Completed";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerystructure/checksetup | " +
                     "PayGroup Id : " + payGroupId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion



        /// ------------------- Tax Component Config. -------------------- ///

        #region API TO GET TAX COMPONENT LIST BEFORE AND AFTER ADDING COMPONENTS
        /// <summary>
        /// Created By Harshit Mitra On 20/12/2022
        /// API >> GET >> api/salerystructure/gettaxcomponents
        /// </summary>
        /// <param name="payGroupId"></param>
        /// <param name="structureId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("gettaxcomponents")]
        public async Task<IHttpActionResult> GetTaxComponetOnAddingComponentInStruture(Guid payGroupId, Guid structureId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var componentInStructure = await _db.SalaryStructureConfigs
                    .Where(x => x.StructureId == structureId &&
                        x.ComponentType == ComponentTypeInPGConstants.TaxDeductionComponent)
                    .Select(x => x.ComponentId)
                    .ToListAsync();
                var countries = await _db.Country.ToListAsync();
                var states = await _db.State.ToListAsync();
                var taxComponent = await (from cp in _db.ComponentInPays
                                          join x in _db.TaxDeductions on cp.ComponentId equals x.TaxComponentId
                                          where cp.ComponentType == Model.EnumClass.ComponentTypeInPGConstants.TaxDeductionComponent
                                              && cp.CompanyId == tokenData.companyId && cp.PayGroupId == payGroupId
                                          select new
                                          {
                                              x.DeductionName,
                                              x.TaxComponentId,
                                              x.CountryId,
                                              x.StateId,
                                              x.Month,
                                              Deductionfor = x.Deductionfor.ToString(),
                                              x.IsPercentage,
                                              x.Value,
                                              x.Component,
                                              x.Limit,
                                              x.Max,
                                              x.Min,

                                          }).ToListAsync();
                if (componentInStructure.Count == 0)
                {
                    res.Message = "Component List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = taxComponent
                    .Select(x => new
                    {
                        ComponentId = x.TaxComponentId,
                        x.DeductionName,
                        x.CountryId,
                        x.StateId,
                        CountryName = countries.Where(z => z.CountryId == x.CountryId).Select(z => z.CountryName).FirstOrDefault(),
                        StateName = states.Where(z => z.StateId == x.StateId).Select(z => z.StateName).FirstOrDefault(),
                        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month),
                        x.Deductionfor,
                        x.IsPercentage,
                        x.Value,
                        x.Component,
                        x.Limit,
                        x.Max,
                        x.Min,
                        IsChecked = false,

                    })
                    .OrderByDescending(x => x.IsChecked).ThenBy(x => x.DeductionName)
                    .ToList();
                }
                else
                {
                    res.Message = "Component List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = taxComponent
                        .Select(x => new
                        {
                            ComponentId = x.TaxComponentId,
                            x.DeductionName,
                            x.CountryId,
                            x.StateId,
                            CountryName = countries.Where(z => z.CountryId == x.CountryId).Select(z => z.CountryName).FirstOrDefault(),
                            StateName = states.Where(z => z.StateId == x.StateId).Select(z => z.StateName).FirstOrDefault(),
                            MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month),
                            x.Deductionfor,
                            x.IsPercentage,
                            x.Value,
                            x.Component,
                            x.Limit,
                            x.Max,
                            x.Min,
                            IsChecked = componentInStructure.Contains(x.TaxComponentId),

                        })
                        .OrderByDescending(x => x.IsChecked)
                        .ToList();
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerystructure/getcomponents | " +
                     "Pay Group Id : " + payGroupId + " | " +
                     "Structure Id : " + structureId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO ADD AND UPDATE COMPONENT IN SALARY STRUCTURE 
        /// <summary>
        /// Created By Harshit Mitra On 20/12/2022
        /// API >> POST >> api/salerystructure/addupdatetaxcomponent
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("addupdatetaxcomponent")]
        public async Task<IHttpActionResult> AddUpdateTaxComponentList(AddComponentInStructureRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var componentInStructure = await _db.SalaryStructureConfigs
                    .Where(x => x.StructureId == model.StructureId &&
                        x.ComponentType == ComponentTypeInPGConstants.TaxDeductionComponent)
                    .ToListAsync();

                List<AddTaxComponentListRequest> addComponentList = model.TaxComponentList.Where(x => x.IsChecked).ToList();
                if (componentInStructure.Count != 0)
                {
                    var checkIds = componentInStructure.Select(x => x.ComponentId).ToList();
                    var newComponent = addComponentList.Where(x => x.IsChecked == true).Select(x => x.ComponentId).ToList();

                    var removeData = checkIds.Where(x => !newComponent.Contains(x)).ToList();
                    var addingData = newComponent.Where(x => !checkIds.Contains(x)).ToList();

                    addComponentList = addComponentList.Where(x => addingData.Contains(x.ComponentId) && x.IsChecked == true).ToList();
                    var removeComponentList = componentInStructure.Where(x => removeData.Contains(x.ComponentId)).ToList();

                    _db.SalaryStructureConfigs.RemoveRange(removeComponentList);
                }

                var addComponent = addComponentList
                    .Where(x => x.IsChecked)
                    .Select(x => new SalaryStructureConfig
                    {
                        StructureId = model.StructureId,
                        ComponentId = x.ComponentId,
                        ComponentType = ComponentTypeInPGConstants.TaxDeductionComponent,
                        CalculationType = x.IsPercentage,
                        CalculatingValue = (!x.IsPercentage) ? x.Value.ToString()
                            : "(" + x.Component + ")*" + x.Value + "/100",
                        TaxSettings = JsonConvert
                            .SerializeObject(new
                            {
                                x.CountryId,
                                x.CountryName,
                                x.StateId,
                                x.StateName,
                                x.MonthName,
                                x.Deductionfor,
                                x.Limit,
                                x.Max,
                                x.Min,
                            }),
                        IsCalculationDone = true,
                        CreatedBy = tokenData.employeeId,
                        CompanyId = tokenData.companyId,

                    }).ToList();

                _db.SalaryStructureConfigs.AddRange(addComponent);
                await _db.SaveChangesAsync();

                res.Message = "Component Added In Structure";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerystructure/addupdatetaxcomponent | " +
                     "Model : " + JsonConvert.SerializeObject(model) + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET ADDED TAX DEDUCTION COMPONENT 
        /// <summary>
        /// Created By Harshit Mitra On 20/12/2022
        /// API >> GET >> api/salerystructure/getselectedtaxcomponent
        /// </summary>
        /// <param name="structureId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getselectedtaxcomponent")]
        public async Task<IHttpActionResult> GetAddedTaxDeductionComponent(Guid structureId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var countries = await _db.Country.ToListAsync();
                var states = await _db.State.ToListAsync();
                var componentInStructure = await (from sc in _db.SalaryStructureConfigs
                                                  join x in _db.TaxDeductions on sc.ComponentId equals x.TaxComponentId
                                                  join em in _db.Employee on sc.CreatedBy equals em.EmployeeId
                                                  where sc.ComponentType == ComponentTypeInPGConstants.TaxDeductionComponent
                                                      && sc.CompanyId == tokenData.companyId && sc.StructureId == structureId
                                                  select new
                                                  {
                                                      x.DeductionName,
                                                      x.TaxComponentId,
                                                      x.CountryId,
                                                      x.StateId,
                                                      x.Month,
                                                      Deductionfor = x.Deductionfor.ToString(),
                                                      x.IsPercentage,
                                                      x.Value,
                                                      x.Component,
                                                      x.Limit,
                                                      x.Max,
                                                      x.Min,
                                                      sc.CreatedBy,
                                                      sc.CreatedOn,

                                                  }).ToListAsync();
                if (componentInStructure.Count == 0)
                {
                    res.Message = "Component List Is Empty";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = componentInStructure;

                    return Ok(res);
                }
                res.Message = "Component List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = componentInStructure
                    .Select(x => new
                    {
                        ComponentId = x.TaxComponentId,
                        x.DeductionName,
                        x.CountryId,
                        x.StateId,
                        CountryName = countries.Where(z => z.CountryId == x.CountryId).Select(z => z.CountryName).FirstOrDefault(),
                        StateName = states.Where(z => z.StateId == x.StateId).Select(z => z.StateName).FirstOrDefault(),
                        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month),
                        x.Deductionfor,
                        x.IsPercentage,
                        x.Value,
                        x.Component,
                        x.Limit,
                        x.Max,
                        x.Min,
                        CreatedByName = _db.GetEmployeeNameById(x.CreatedBy),
                        x.CreatedOn,
                    })
                    .OrderBy(x => x.DeductionName)
                    .ToList();
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerystructure/getselectedtaxcomponent | " +
                     "Structure Id : " + structureId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO DELETE TAX COMPONENT IN STRUCTURE 
        /// <summary>
        /// Created By Harshit Mitra On 20/12/2022
        /// API >> POST >> api/salerystructure/deletetaxcomponent
        /// </summary>
        /// <param name="structureId"></param>
        /// <param name="componentId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deletetaxcomponent")]
        public async Task<IHttpActionResult> DeleteTaxComponent(Guid structureId, Guid componentId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var componentInStructure = await _db.SalaryStructureConfigs
                    .FirstOrDefaultAsync(x => x.StructureId == structureId &&
                            x.ComponentId == componentId);
                if (componentInStructure == null)
                {
                    res.Message = "Component Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;

                    return Ok(res);
                }
                _db.SalaryStructureConfigs.Remove(componentInStructure);
                await _db.SaveChangesAsync();

                res.Message = "Component Removed";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerystructure/deletetaxcomponent | " +
                     "Structure Id : " + structureId + " | " +
                     "Component Id : " + componentId + " | " +
                     "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion





        #region REQUEST AND RESPONSE 
        public class CreateSalaryStructureRequest
        {
            public Guid PayGroupId { get; set; } = Guid.Empty;
            public string StructureName { get; set; } = String.Empty;
            public string Description { get; set; } = String.Empty;
        }
        public class EditSalaryStructureRequest : CreateSalaryStructureRequest
        {
            [Required]
            public Guid StructureId { get; set; } = Guid.Empty;
        }
        public class AddComponentInStructureRequest
        {
            public Guid StructureId { get; set; } = Guid.Empty;
            public List<AddComponentInStructureListRequest> ComponentList { get; set; }
            public List<AddTaxComponentListRequest> TaxComponentList { get; set; }
        }
        public class AddComponentInStructureListRequest
        {
            public Guid ComponentId { get; set; } = Guid.Empty;
            public bool IsAutoCalculated { get; set; }
            public string CalculatingValue { get; set; } = String.Empty;
            public bool IsChecked { get; set; }
        }
        public class UpdateCalculationRequest
        {
            public Guid StructureId { get; set; } = Guid.Empty;
            public Guid ComponentId { get; set; } = Guid.Empty;
            public string CalculatingValue { get; set; } = String.Empty;
        }
        public class AddTaxComponentListRequest
        {
            public Guid ComponentId { get; set; } = Guid.Empty;
            public int CountryId { get; set; }
            public string CountryName { get; set; } = String.Empty;
            public int StateId { get; set; }
            public string StateName { get; set; } = String.Empty;
            public string MonthName { get; set; } = String.Empty;
            public string Deductionfor { get; set; } = String.Empty;
            public bool IsPercentage { get; set; } = false;
            public double Value { get; set; } = 0.0;
            public string Component { get; set; } = String.Empty;
            public double Limit { get; set; } = 0.0;
            public double Min { get; set; } = 0.0;
            public double Max { get; set; } = 0.0;
            public bool IsChecked { get; set; }
        }
        #endregion

    }
}
