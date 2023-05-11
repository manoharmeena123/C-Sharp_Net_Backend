//using AspNetIdentity.WebApi.Helper;
//using AspNetIdentity.WebApi.Infrastructure;
//using AspNetIdentity.WebApi.Model.PayRollModel;
//using AspNetIdentity.WebApi.Models;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using System.Web.Http;
//using static AspNetIdentity.WebApi.Model.EnumClass;

//namespace AspNetIdentity.WebApi.Controllers.Payroll
//{
//    /// <summary>
//    /// Created By Harshit Mitra On 06-06-2022
//    /// </summary>
//    [Authorize]
//    [RoutePrefix("api/salerystructure")]
//    public class PayRollSaleryStructureController : ApiController
//    {
//        public readonly ApplicationDbContext _db = new ApplicationDbContext();

//        ///----------- Salary Component Main Page --------------///

//        #region Api To Get Salery Structure List

//        /// <summary>
//        /// Created By Harshit Mitra On 06-06-2022
//        /// API >> Get >> api/salerystructure/getsalerystructurelist
//        /// </summary>
//        /// <param name="payGroupId"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getsalerystructurelist")]
//        public async Task<ResponseBodyModel> GetSaleryStructureList(int payGroupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var saleryStructureList = await _db.SaleryStructurePayRolls.Where(x =>
//                    x.PayGroupId == payGroupId && x.IsDeleted == false && x.IsActive == true)
//                    .Select(x => new
//                    {
//                        x.StructureId,
//                        x.PayGroupId,
//                        x.StructureName,
//                        x.Description,
//                        NumberOfEmployees = _db.Employee.Where(z => z.StructureId == x.StructureId).ToList().Count,
//                        x.CreatedOn,
//                        CreatedBy = _db.Employee.Where(z => z.EmployeeId == x.CreatedBy)
//                                .Select(z => z.DisplayName).FirstOrDefault(),
//                        x.UpdatedOn,
//                        UpdatedBy = x.UpdatedOn.HasValue ? _db.Employee.Where(z => z.EmployeeId == (int)x.UpdatedBy)
//                                .Select(z => z.DisplayName).FirstOrDefault() : "",
//                    }).ToListAsync();
//                if (saleryStructureList.Count > 0)
//                {
//                    res.Message = "Structures Found";
//                    res.Status = true;
//                    res.Data = saleryStructureList;
//                }
//                else
//                {
//                    res.Message = "No Structure Found";
//                    res.Status = false;
//                    res.Data = saleryStructureList;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Salery Structure List

//        ///----------- Salary Structure APIs -------------///

//        #region Api To Add Salery Structure

//        /// <summary>
//        /// Created By Harshit Mitra on 06-06-2022
//        /// API >> Post >> api/salerystructure/addupdatesalerys
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("addupdatesalerys")]
//        public async Task<ResponseBodyModel> AddSaleryStructure(SaleryStructurePayRoll model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.CompanyId == claims.companyId &&
//                            x.OrgId == claims.orgId && x.IsActive == true && x.IsDeleted == false && x.PayGroupId == model.PayGroupId);
//                if (payGroup != null)
//                {
//                    var payRollStructureSetup = await _db.PayRollSetups.FirstOrDefaultAsync(x => x.IsActive == true &&
//                                    x.IsDeleted == false && x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.SaleryStructure);
//                    if (payRollStructureSetup != null)
//                    {
//                        SaleryStructurePayRoll obj = new SaleryStructurePayRoll
//                        {
//                            PayGroupId = model.PayGroupId,
//                            StructureName = model.StructureName,
//                            Description = model.Description,

//                            IsActive = true,
//                            IsDeleted = false,
//                            CreatedBy = claims.employeeId,
//                            CreatedOn = DateTime.Now,
//                            CompanyId = claims.companyId,
//                            OrgId = 0,
//                        };
//                        _db.SaleryStructurePayRolls.Add(obj);
//                        await _db.SaveChangesAsync();

//                        res.Message = "Structure Added";
//                        res.Status = true;
//                        res.Data = obj;
//                    }
//                    else
//                    {
//                        res.Message = "Pay Roll Setup not Created";
//                        res.Status = false;
//                    }
//                }
//                else
//                {
//                    res.Message = "Pay Group Not Found";
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

//        #endregion Api To Add Salery Structure

//        #region Api To Get Salery Structure By Id

//        /// <summary>
//        /// Created By Harshit Mitra on 07-06-2022
//        /// API >> Get >> api/salerystructure/getstructurebyid
//        /// </summary>
//        /// <param name="structureId"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getstructurebyid")]
//        public async Task<ResponseBodyModel> GetSaleryStructureById(int structureId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var structure = await _db.SaleryStructurePayRolls.FirstOrDefaultAsync(x =>
//                            x.CompanyId == claims.companyId && x.StructureId == structureId);
//                if (structure != null)
//                {
//                    res.Message = "Structure Found";
//                    res.Status = true;
//                    res.Data = structure;
//                }
//                else
//                {
//                    res.Message = "Structure Not Found";
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

//        #endregion Api To Get Salery Structure By Id

//        #region Api To Edit Salery Structure

//        /// <summary>
//        /// Created By Harshit Mitra on 07-06-2022
//        /// API >> Put >> api/salerystructure/updatesalerystructure
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPut]
//        [Route("updatesalerystructure")]
//        public async Task<ResponseBodyModel> GetSaleryStructureById(SaleryStructurePayRoll model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var structure = await _db.SaleryStructurePayRolls.FirstOrDefaultAsync(x =>
//                            x.CompanyId == claims.companyId && x.StructureId == model.StructureId);
//                if (structure != null)
//                {
//                    structure.StructureName = model.StructureName;
//                    structure.Description = model.Description;

//                    structure.UpdatedOn = DateTime.Now;
//                    structure.UpdatedBy = claims.employeeId;

//                    _db.Entry(structure).State = System.Data.Entity.EntityState.Modified;
//                    await _db.SaveChangesAsync();

//                    res.Message = "Updated";
//                    res.Status = true;
//                    res.Data = structure;
//                }
//                else
//                {
//                    res.Message = "Structure Not Found";
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

//        #endregion Api To Edit Salery Structure

//        #region Api To Delete Structure

//        /// <summary>
//        /// Created By Harshit Mitra on 07-06-2022
//        /// API >> Put >> api/salerystructure/deletesalarystructure
//        /// </summary>
//        /// <param name="structureId"></param>
//        /// <returns></returns>
//        [HttpPut]
//        [Route("deletesalarystructure")]
//        public async Task<ResponseBodyModel> DeleteSalaryStructure(int structureId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var structure = await _db.SaleryStructurePayRolls.FirstOrDefaultAsync(x =>
//                            x.CompanyId == claims.companyId && x.StructureId == structureId);
//                if (structure != null)
//                {
//                    structure.IsDeleted = true;
//                    structure.IsActive = false;

//                    structure.DeletedOn = DateTime.Now;
//                    structure.DeletedBy = claims.employeeId;

//                    _db.Entry(structure).State = System.Data.Entity.EntityState.Modified;
//                    await _db.SaveChangesAsync();

//                    res.Message = "Updated";
//                    res.Status = true;
//                    res.Data = structure;
//                }
//                else
//                {
//                    res.Message = "Structure Not Found";
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

//        #endregion Api To Delete Structure

//        ///----------- Salary Component In Config. APIs --------------///

//        #region Api To Get Pay Roll Component Added In PayGroup for Salary Config.

//        /// <summary>
//        /// Created By Harshit Mitra On 06-06-2022
//        /// API >> Get >> api/salerystructure/getcomponents
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getcomponents")]
//        public async Task<ResponseBodyModel> GetSaleryStructure(int payGroupId, int structureId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.CompanyId == claims.companyId &&
//                             x.OrgId == claims.orgId && x.IsActive == true && x.IsDeleted == false && x.PayGroupId == payGroupId);
//                if (payGroup != null)
//                {
//                    var payRollStructureSetup = await _db.PayRollSetups.FirstOrDefaultAsync(x => x.IsActive == true &&
//                                    x.IsDeleted == false && x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.SaleryStructure);
//                    if (payRollStructureSetup != null)
//                    {
//                        var checkComponent = await _db.PayGroupComponents.Where(x => x.PayGroupId == payGroup.PayGroupId &&
//                                    x.ComponentType == PayGroupComponentTypeConstants.RecurringComponent).ToListAsync();
//                        if (checkComponent.Count > 0)
//                        {
//                            var checkAddedComponents = await _db.SaleryStructureComponents.Where(x => x.StructureId == structureId &&
//                                    x.CompanyId == claims.companyId && x.IsActive == true && x.IsDeleted == false).Select(x => x.ComponentId).ToListAsync();
//                            var componentIds = checkComponent.Select(x => x.SelectedComponentId).ToList();
//                            var getComponentList = _db.RecuringComponents.Where(x => componentIds.Contains(x.RecuringComponentId))
//                                    .Select(x => new GetAddComponentListHelperModelClass
//                                    {
//                                        IsChecked = checkAddedComponents.Contains(x.RecuringComponentId) || (x.ComponentName == "Basic" || x.ComponentName == "HRA"),
//                                        IsDisabbled = (x.ComponentName == "Basic" || x.ComponentName == "HRA"),
//                                        ComponentId = x.RecuringComponentId,
//                                        ComponentName = x.ComponentName,
//                                        ComponentType = x.ComponentType,
//                                        IsTaxExempted = x.IsTaxExempted,
//                                        IncomeTaxSection = x.IncomeTaxSection,
//                                        IsAutoCalculated = x.IsAutoCalculated,
//                                        MaxiumLimitPerYear = x.MaxiumLimitPerYear,
//                                        Calculation = (!x.IsAutoCalculated) ? x.MaxiumLimitPerYear.ToString() : "Calculation Required",
//                                    }).ToList();

//                            if (getComponentList.Count > 0)
//                            {
//                                res.Message = "Component List";
//                                res.Status = true;
//                                res.Data = getComponentList;
//                            }
//                            else
//                            {
//                                res.Message = "Component Not Found";
//                                res.Status = false;
//                                res.Data = getComponentList;
//                            }
//                        }
//                        else
//                        {
//                            res.Message = "Component Not Added";
//                            res.Status = false;
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Roll Setup not Created";
//                        res.Status = false;
//                    }
//                }
//                else
//                {
//                    res.Message = "Pay Group Not Found";
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

//        #endregion Api To Get Pay Roll Component Added In PayGroup for Salary Config.

//        #region Api To Add and Update Component In Salery Structure

//        /// <summary>
//        /// Created By Harshit Mitra on 06-06-2022
//        /// API >> Post >> api/salerystructure/addupdatecomponent
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("addupdatecomponent")]
//        public async Task<ResponseBodyModel> AddUpdateComponentInSaleryStructure(AddUpdateComponentHelperModelClass model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            List<SaleryStructureComponent> listobj = new List<SaleryStructureComponent>();
//            try
//            {
//                var payGroup = await _db.PayGroups.FirstOrDefaultAsync(x => x.CompanyId == claims.companyId &&
//                             x.OrgId == claims.orgId && x.IsActive == true && x.IsDeleted == false && x.PayGroupId == model.PayGroupId);
//                if (payGroup != null)
//                {
//                    var payRollStructureSetup = await _db.PayRollSetups.FirstOrDefaultAsync(x => x.IsActive == true &&
//                                    x.IsDeleted == false && x.PayGroupId == payGroup.PayGroupId && x.Step == (int)PayrollSetupConstants.SaleryStructure);
//                    if (payRollStructureSetup != null)
//                    {
//                        var checkComponents = await _db.SaleryStructureComponents.Where(x => x.StructureId == model.StructureId &&
//                                    x.CompanyId == claims.companyId && x.IsActive == true && x.IsDeleted == false).ToListAsync();
//                        if (checkComponents.Count == 0)
//                        {
//                            if (model.ComponentList.Count > 0)
//                            {
//                                foreach (var item in model.ComponentList)
//                                {
//                                    if (item.IsChecked)
//                                    {
//                                        SaleryStructureComponent obj = new SaleryStructureComponent
//                                        {
//                                            StructureId = model.StructureId,
//                                            ComponentId = item.ComponentId,
//                                            ComponentsName = item.ComponentName,
//                                            AnnulCalculation = item.Calculation,
//                                            CalculationType = item.IsAutoCalculated == true ?
//                                                    CalculationTypeConstants.Calculation :
//                                                    CalculationTypeConstants.Fixed,
//                                            CalculationDone = !item.IsAutoCalculated,

//                                            CompanyId = claims.companyId,
//                                            IsActive = true,
//                                            IsDeleted = false,
//                                            CreatedBy = claims.employeeId,
//                                            CreatedOn = DateTime.Now,
//                                        };

//                                        _db.SaleryStructureComponents.Add(obj);
//                                        await _db.SaveChangesAsync();
//                                        listobj.Add(obj);
//                                    }
//                                }
//                                res.Message = "Added";
//                                res.Status = true;
//                                res.Data = listobj;
//                            }
//                        }
//                        else
//                        {
//                            var checkIds = checkComponents.Select(x => x.ComponentId).ToList();
//                            var newComponent = model.ComponentList.Where(x => x.IsChecked == true).Select(x => x.ComponentId).ToList();
//                            var removeData = new List<int>();
//                            var addingData = new List<int>();
//                            removeData = checkIds.Where(x => !newComponent.Contains(x)).ToList();
//                            addingData = newComponent.Where(x => !checkIds.Contains(x)).ToList();
//                            var addComponentList = model.ComponentList.Where(x => addingData.Contains(x.ComponentId) && x.IsChecked == true).ToList();
//                            var removeComponentList = checkComponents.Where(x => removeData.Contains(x.ComponentId)).ToList();
//                            foreach (var component in removeComponentList)
//                            {
//                                _db.Entry(component).State = EntityState.Deleted;
//                                await _db.SaveChangesAsync();
//                            }
//                            foreach (var item in addComponentList)
//                            {
//                                if (item.IsChecked)
//                                {
//                                    SaleryStructureComponent obj = new SaleryStructureComponent
//                                    {
//                                        StructureId = model.StructureId,
//                                        ComponentId = item.ComponentId,
//                                        ComponentsName = item.ComponentName,
//                                        AnnulCalculation = item.Calculation,
//                                        CalculationType = item.IsAutoCalculated == true ?
//                                                CalculationTypeConstants.Calculation :
//                                                CalculationTypeConstants.Fixed,
//                                        CalculationDone = !item.IsAutoCalculated,

//                                        CompanyId = claims.companyId,
//                                        IsActive = true,
//                                        IsDeleted = false,
//                                        CreatedBy = claims.employeeId,
//                                        CreatedOn = DateTime.Now,
//                                    };

//                                    _db.SaleryStructureComponents.Add(obj);
//                                    await _db.SaveChangesAsync();
//                                    listobj.Add(obj);
//                                }
//                            }
//                            var data = await _db.SaleryStructureComponents.Where(x => x.StructureId == model.StructureId &&
//                                    x.CompanyId == claims.companyId && x.IsActive == true && x.IsDeleted == false).ToListAsync();
//                            res.Message = "Updated";
//                            res.Status = true;
//                            res.Data = data;
//                        }
//                    }
//                    else
//                    {
//                        res.Message = "Pay Roll Setup not Created";
//                        res.Status = false;
//                    }
//                }
//                else
//                {
//                    res.Message = "Pay Group Not Found";
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

//        #endregion Api To Add and Update Component In Salery Structure

//        //------------ Get Compponent And Calculation CRUD ------------///

//        #region Api To Get Selected Component In Salary Config.

//        /// <summary>
//        /// Created By Harshit Mitra on 07-06-2022
//        /// API >> Get >> api/salerystructure/getselectedcomponent
//        /// </summary>
//        /// <param name="structureId"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getselectedcomponent")]
//        public async Task<ResponseBodyModel> GetSelectedComponent(int structureId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var checkComponents = await _db.SaleryStructureComponents.Where(x => x.StructureId == structureId &&
//                                    x.CompanyId == claims.companyId && x.IsActive == true && x.IsDeleted == false).ToListAsync();
//                if (checkComponents.Count > 0)
//                {
//                    checkComponents.ForEach(x => x.CreatedByName = _db.GetEmployeeNameById(x.CreatedBy));
//                    res.Message = "Component";
//                    res.Status = true;
//                    res.Data = checkComponents;
//                }
//                else
//                {
//                    res.Message = "No Component Added";
//                    res.Status = false;
//                    res.Data = checkComponents;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Selected Component In Salary Config.

//        #region Api To Update Calculation In Component

//        /// <summary>
//        /// Created By Harshit Mitra on 07-06-2022
//        /// API >> Put >> api/salerystructure/updatecalculation
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPut]
//        [Route("updatecalculation")]
//        public async Task<ResponseBodyModel> AddCalculation(SaleryStructureComponent model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var component = await _db.SaleryStructureComponents.FirstOrDefaultAsync(x => x.Id == model.Id &&
//                            x.CompanyId == claims.companyId && x.IsActive == true && x.IsDeleted == false);
//                if (component != null)
//                {
//                    component.AnnulCalculation = model.AnnulCalculation;
//                    component.CalculationDone = true;
//                    component.UpdatedOn = DateTime.Now;
//                    component.UpdatedBy = claims.employeeId;

//                    _db.Entry(component).State = System.Data.Entity.EntityState.Modified;
//                    await _db.SaveChangesAsync();

//                    res.Message = "Calulation Updated";
//                    res.Status = true;
//                    res.Data = component;
//                }
//                else
//                {
//                    res.Message = "Component Not Found";
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

//        #endregion Api To Update Calculation In Component

//        #region Api To Delete Component in Salary Config.

//        /// <summary>
//        /// Created By Harshit Mitra on 07-06-2022
//        /// API >> Put >> api/salerystructure/deletecomponent
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [HttpPut]
//        [Route("deletecomponent")]
//        public async Task<ResponseBodyModel> DeleteComponent(int id)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var component = await _db.SaleryStructureComponents.FirstOrDefaultAsync(x => x.Id == id &&
//                            x.CompanyId == claims.companyId && x.IsActive == true && x.IsDeleted == false);
//                if (component != null)
//                {
//                    _db.Entry(component).State = EntityState.Deleted;
//                    await _db.SaveChangesAsync();

//                    res.Message = "Component Deleted";
//                    res.Status = true;
//                }
//                else
//                {
//                    res.Message = "Component Not Found";
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

//        #endregion Api To Delete Component in Salary Config.

//        #region Api To Check Added Formula For Setup Complection

//        /// <summary>
//        /// Created By Harshit Mitra on 07-06-2022
//        /// API >> Post >> api/salerystructure/checksetup
//        /// </summary>
//        /// <param name="payGroupId"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("checksetup")]
//        public ResponseBodyModel CheckFormulaIsAddedOrNot(int payGroupId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var check = PayRollCentralHelper.CheckStructure(payGroupId, claims.employeeId);
//                if (check)
//                {
//                    res.Message = "Added Succesfull";
//                    res.Status = true;
//                }
//                else
//                {
//                    res.Message = "You Skip Calculation";
//                    res.Status = false;
//                }

//                //var checkStructure = await _db.SaleryStructurePayRolls.Where(x => x.PayGroupId == payGroupId &&
//                //        x.IsActive == true && x.IsDeleted == false).Select(x => x.StructureId).ToListAsync();
//                //var checkComponents = await _db.SaleryStructureComponents.Where(x => checkStructure.Contains(x.StructureId) &&
//                //                    x.CompanyId == claims.companyid && x.IsActive == true && x.IsDeleted == false).ToListAsync();
//                //if (checkComponents.Count > 0)
//                //{
//                //    var checking = checkComponents.Select(x => x.CalculationDone).ToList();
//                //    if (!checking.Contains(false))
//                //    {
//                //        res.Message = "Added Succesfull";
//                //        res.Status = true;

//                //        var payRollComponentSetup = await _db.PayRollSetups.Where(x => x.IsActive == true &&
//                //                    x.IsDeleted == false && x.PayGroupId == payGroupId && x.Step == (int)PayrollSetupEnum.SaleryStructure ||
//                //                    x.Step == (int)PayrollSetupEnum.FinanceSetting).ToListAsync();
//                //        foreach (var item in payRollComponentSetup)
//                //        {
//                //            item.UpdatedOn = DateTime.Now;
//                //            item.UpdatedBy = claims.userid;
//                //            item.Status = Enum.GetName(typeof(PayrollSetupStatus), (int)PayrollSetupStatus.COMPLETED);

//                //            _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
//                //            await _db.SaveChangesAsync();
//                //        }
//                //    }
//                //    else
//                //    {
//                //        res.Message = "You Skip Calculation";
//                //        res.Status = false;

//                //        var payRollComponentSetup = await _db.PayRollSetups.Where(x => x.IsActive == true &&
//                //                    x.IsDeleted == false && x.PayGroupId == payGroupId && x.Step == (int)PayrollSetupEnum.SaleryStructure ||
//                //                    x.Step == (int)PayrollSetupEnum.FinanceSetting).ToListAsync();
//                //        foreach (var item in payRollComponentSetup)
//                //        {
//                //            item.UpdatedOn = DateTime.Now;
//                //            item.UpdatedBy = claims.userid;
//                //            item.Status = Enum.GetName(typeof(PayrollSetupStatus), (int)PayrollSetupStatus.PENDING);

//                //            _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
//                //            await _db.SaveChangesAsync();
//                //        }
//                //    }
//                //}
//                //else
//                //{
//                //    res.Message = "No Component Added";
//                //    res.Status = false;
//                //    res.Data = checkComponents;
//                //}
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Check Added Formula For Setup Complection

//        #region Helper Model Class

//        /// <summary>
//        /// Created By Harshit Mitra on 06-06-2022
//        /// </summary>
//        public class AddUpdateComponentHelperModelClass
//        {
//            public int PayGroupId { get; set; }
//            public int StructureId { get; set; }
//            public List<GetAddComponentListHelperModelClass> ComponentList { get; set; }
//        }

//        /// <summary>
//        /// Created By Harshit Mitra On 06-06-2022
//        /// </summary>
//        public class GetAddComponentListHelperModelClass
//        {
//            public bool IsChecked { get; set; }
//            public bool IsDisabbled { get; set; }
//            public int ComponentId { get; set; }
//            public string ComponentName { get; set; }
//            public string ComponentType { get; set; }
//            public bool IsTaxExempted { get; set; }
//            public string IncomeTaxSection { get; set; }
//            public bool IsAutoCalculated { get; set; }
//            public int MaxiumLimitPerYear { get; set; }
//            public string Calculation { get; set; }
//        }

//        #endregion Helper Model Class
//    }
//}