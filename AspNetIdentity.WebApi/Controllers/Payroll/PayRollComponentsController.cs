//using AspNetIdentity.WebApi.Helper;
//using AspNetIdentity.WebApi.Infrastructure;
//using AspNetIdentity.WebApi.Model.PayRollComponentsModel;
//using AspNetIdentity.WebApi.Model.PayRollModel;
//using AspNetIdentity.WebApi.Models;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Globalization;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using System.Web.Http;
//using static AspNetIdentity.WebApi.Model.EnumClass;

//namespace AspNetIdentity.WebApi.Controllers.Payroll
//{
//    /// <summary>
//    /// Created By Harshit Mitra on 19-04-2022
//    /// </summary>
//    [Authorize]
//    [RoutePrefix("api/components")]
//    public class PayRollComponentsController : ApiController
//    {
//        public readonly ApplicationDbContext _db = new ApplicationDbContext();

//        /// ----------------- Recuring Component ----------------- ///

//        #region Api To Add Recuring Components

//        /// <summary>
//        /// Created By Harshit Mitra on 19-04-2022
//        /// API >> Post >> api/components/addrecruingcomponents
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("addrecruingcomponents")]
//        public async Task<ResponseBodyModel> AddRecuringComponents(RecuringComponents model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                if (model == null)
//                {
//                    res.Message = "Model Is Invalid";
//                    res.Status = false;
//                }
//                else
//                {
//                    RecuringComponents obj = new RecuringComponents
//                    {
//                        ComponentName = model.ComponentName,
//                        ComponentType = model.ComponentType,
//                        IsAutoCalculated = model.IsAutoCalculated,
//                        MaxiumLimitPerYear = model.MaxiumLimitPerYear,
//                        Description = model.Description,
//                        IsTaxExempted = model.IsTaxExempted,
//                        IncomeTaxSection = model.IncomeTaxSection,
//                        SectionMaxLimit = model.SectionMaxLimit,
//                        IsDocumentRequired = model.IsDocumentRequired,
//                        IsEditable = true,

//                        IsActive = true,
//                        IsDeleted = false,
//                        CreatedBy = claims.employeeId,
//                        CreatedOn = DateTime.Now,
//                        CompanyId = claims.companyId,
//                        OrgId = 0,
//                    };
//                    _db.RecuringComponents.Add(obj);
//                    await _db.SaveChangesAsync();

//                    res.Message = "Component Added";
//                    res.Status = true;
//                    res.Data = obj;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Add Recuring Components

//        #region Api To Get All Active Recuring Components

//        /// <summary>
//        /// Created By Harshit Mitra on 19-04-2022
//        /// API >> api/components/allactiverecuringcomponents
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("allactiverecuringcomponents")]
//        public async Task<ResponseBodyModel> GetAllActiveRecuringComponents()
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                CheckComponents(claims.companyId);
//                var recuringComponent = await _db.RecuringComponents.Where(x => x.IsActive == true &&
//                        x.IsDeleted == false && x.CompanyId == claims.companyId)
//                        .ToListAsync();
//                if (recuringComponent.Count > 0)
//                {
//                    res.Message = "Recuring List";
//                    res.Status = true;
//                    res.Data = recuringComponent;
//                }
//                else
//                {
//                    res.Message = "Recuring Component Not Found";
//                    res.Status = false;
//                    res.Data = recuringComponent;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get All Active Recuring Components

//        #region Api To Get Recuring Component By Id

//        /// <summary>
//        /// Created By Harshit Mitra on 19-04-2022
//        /// API >> api/components/getrecuringcomponentbyId
//        /// </summary>
//        /// <param name="recuringComponentId"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getrecuringcomponentbyId")]
//        public async Task<ResponseBodyModel> GetRecuringComponentById(int recuringComponentId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var recuringComponent = await _db.RecuringComponents.FirstOrDefaultAsync(x => x.IsActive == true &&
//                        x.IsDeleted == false && x.CompanyId == claims.companyId &&
//                        x.RecuringComponentId == recuringComponentId);
//                if (recuringComponent != null)
//                {
//                    res.Message = "Recuring Component";
//                    res.Status = true;
//                    res.Data = recuringComponent;
//                }
//                else
//                {
//                    res.Message = "Recuring Component Not Found";
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

//        #endregion Api To Get Recuring Component By Id

//        #region Api To Edit Recuring Component

//        /// <summary>
//        /// Created By Harshit Mitra on 19-04-2022
//        /// API >> api/components/editrecuringcomponent
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPut]
//        [Route("editrecuringcomponent")]
//        public async Task<ResponseBodyModel> EditRecuringComponent(RecuringComponents model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var recuringComponent = await _db.RecuringComponents.FirstOrDefaultAsync(x => x.IsActive == true &&
//                        x.IsDeleted == false && x.CompanyId == claims.companyId &&
//                        x.RecuringComponentId == model.RecuringComponentId);
//                if (recuringComponent != null)
//                {
//                    recuringComponent.ComponentName = model.ComponentName;
//                    recuringComponent.ComponentType = model.ComponentType;
//                    recuringComponent.MaxiumLimitPerYear = model.MaxiumLimitPerYear;
//                    recuringComponent.Description = model.Description;
//                    recuringComponent.IsTaxExempted = model.IsTaxExempted;
//                    recuringComponent.IncomeTaxSection = model.IncomeTaxSection;
//                    recuringComponent.IsDocumentRequired = model.IsDocumentRequired;
//                    recuringComponent.SectionMaxLimit = model.SectionMaxLimit;
//                    recuringComponent.UpdatedOn = DateTime.Now;
//                    recuringComponent.UpdatedBy = claims.employeeId;

//                    _db.Entry(recuringComponent).State = System.Data.Entity.EntityState.Modified;
//                    await _db.SaveChangesAsync();

//                    res.Message = "Recuring Component Edited";
//                    res.Status = true;
//                    res.Data = recuringComponent;
//                }
//                else
//                {
//                    res.Message = "Recuring Component Not Found";
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

//        #endregion Api To Edit Recuring Component

//        #region Api To Delete Recuring Component

//        /// <summary>
//        /// Created By Harshit Mitra on 19-04-2022
//        /// API >> api/components/deleterecuringcomponent
//        /// </summary>
//        /// <param name="recuringComponentId"></param>
//        /// <returns></returns>
//        [HttpPut]
//        [Route("deleterecuringcomponent")]
//        public async Task<ResponseBodyModel> DeleteRecuringComponent(int recuringComponentId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var recuringComponent = await _db.RecuringComponents.FirstOrDefaultAsync(x => x.IsActive == true &&
//                        x.IsDeleted == false && x.CompanyId == claims.companyId &&
//                        x.RecuringComponentId == recuringComponentId);
//                if (recuringComponent != null)
//                {
//                    recuringComponent.IsActive = false;
//                    recuringComponent.IsDeleted = true;
//                    recuringComponent.DeletedOn = DateTime.Now;
//                    recuringComponent.DeletedBy = claims.employeeId;

//                    _db.Entry(recuringComponent).State = System.Data.Entity.EntityState.Modified;
//                    await _db.SaveChangesAsync();

//                    res.Message = "Recuring Component Edited";
//                    res.Status = true;
//                    res.Data = recuringComponent;
//                }
//                else
//                {
//                    res.Message = "Recuring Component Not Found";
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

//        #endregion Api To Delete Recuring Component

//        #region Function To Check System Enter Salery Component When Salery Component List Is Empty

//        public void CheckComponents(int companyId)
//        {
//            var recuringComponent = _db.RecuringComponents.Where(x => x.IsActive == true &&
//                        x.IsDeleted == false && x.CompanyId == companyId)
//                        .ToList().Count;
//            if (recuringComponent == 0)
//            {
//                List<RecuringComponents> list = new List<RecuringComponents>();
//                RecuringComponents obj1 = new RecuringComponents
//                {
//                    ComponentName = "Basic",
//                    ComponentType = "Fixed",
//                    IsAutoCalculated = true,
//                    MaxiumLimitPerYear = 0,
//                    Description = "Basic Recurring Component",
//                    IsTaxExempted = false,
//                    IncomeTaxSection = "",
//                    SectionMaxLimit = 0,
//                    IsDocumentRequired = false,
//                    IsEditable = false,

//                    IsActive = true,
//                    IsDeleted = false,
//                    CreatedBy = 0,
//                    CreatedOn = DateTime.Now,
//                    CompanyId = companyId,
//                    OrgId = 0,
//                };
//                list.Add(obj1);
//                RecuringComponents obj2 = new RecuringComponents
//                {
//                    ComponentName = "HRA",
//                    ComponentType = "Fixed",
//                    IsAutoCalculated = true,
//                    MaxiumLimitPerYear = 0,
//                    Description = "HRA Recurring Component",
//                    IsTaxExempted = true,
//                    IncomeTaxSection = "Section_10(13)(a)",
//                    SectionMaxLimit = 0,
//                    IsDocumentRequired = false,
//                    IsEditable = false,

//                    IsActive = true,
//                    IsDeleted = false,
//                    CreatedBy = 0,
//                    CreatedOn = DateTime.Now,
//                    CompanyId = companyId,
//                    OrgId = 0,
//                };
//                list.Add(obj2);
//                RecuringComponents obj3 = new RecuringComponents
//                {
//                    ComponentName = "Special Allowance",
//                    ComponentType = "Allowance",
//                    IsAutoCalculated = true,
//                    MaxiumLimitPerYear = 0,
//                    Description = "Special Allowance Recurring Component",
//                    IsTaxExempted = false,
//                    IncomeTaxSection = "",
//                    SectionMaxLimit = 0,
//                    IsDocumentRequired = false,
//                    IsEditable = false,

//                    IsActive = true,
//                    IsDeleted = false,
//                    CreatedBy = 0,
//                    CreatedOn = DateTime.Now,
//                    CompanyId = companyId,
//                    OrgId = 0,
//                };
//                list.Add(obj3);
//                _db.RecuringComponents.AddRange(list);
//                _db.SaveChanges();
//            }
//        }

//        #endregion Function To Check System Enter Salery Component When Salery Component List Is Empty

//        /// ---------------- Ad HOC Components ----------------- ///

//        #region Api To Add Ad HOC Components

//        /// <summary>
//        /// Created By Harshit Mitra on 19-04-2022
//        /// API >> Post >> api/components/createadhoc
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("createadhoc")]
//        public async Task<ResponseBodyModel> CreateAdHOC(AdHocComponent model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                if (model == null)
//                {
//                    res.Message = "Model Is Invalid";
//                    res.Status = false;
//                }
//                else
//                {
//                    AdHocComponent obj = new AdHocComponent
//                    {
//                        Name = model.Name,
//                        Description = model.Description,
//                        HasTaxBenefits = model.HasTaxBenefits,

//                        IsActive = true,
//                        IsDeleted = false,
//                        CreatedBy = claims.employeeId,
//                        CreatedOn = DateTime.Now,
//                        CompanyId = claims.companyId,
//                        OrgId = 0,
//                    };

//                    _db.AdHocComponents.Add(obj);
//                    await _db.SaveChangesAsync();

//                    res.Message = "Ad Hoc Added";
//                    res.Status = true;
//                    res.Data = obj;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Add Ad HOC Components

//        #region Api To Get All Active Ad HOC

//        /// <summary>
//        /// Created By Harshit Mitra on 19-04-2022
//        /// API >> Post >> api/components/allactiveachoc
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("allactiveachoc")]
//        public async Task<ResponseBodyModel> GetAllActiveAddHoc(int page, int list)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var adHocList = await _db.AdHocComponents.Where(x => x.IsDeleted == false &&
//                        x.IsActive == true && x.CompanyId == claims.companyId)
//                        .OrderByDescending(x => x.Name).Skip((page - 1) * list).Take(list).ToListAsync();

//                var total_count = _db.AdHocComponents.Where(x => x.IsDeleted == false && x.IsActive == true).Count();
//                res.Data = adHocList;

//                if (adHocList.Count > 0)
//                {
//                    res.Message = "Ad Hoc List";
//                    res.Status = true;
//                    res.Data = adHocList;
//                }
//                else
//                {
//                    res.Message = "Ad Hoc Not Found";
//                    res.Status = false;
//                    res.Data = adHocList;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get All Active Ad HOC

//        #region Api To Get Ad Hoc By Id

//        /// <summary>
//        /// Created By Harshit Mitra on 19-04-2022
//        /// API >> Post >> api/components/getadhocbyid
//        /// </summary>
//        /// <param name="adHocId"></param>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("getadhocbyid")]
//        public async Task<ResponseBodyModel> GetAllActiveAddHoc(int adHocId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var adHoc = await _db.AdHocComponents.FirstOrDefaultAsync(x => x.IsDeleted == false &&
//                        x.IsActive == true && x.CompanyId == claims.companyId && x.AdHocId == adHocId);
//                if (adHoc != null)
//                {
//                    res.Message = "Ad Hoc List";
//                    res.Status = true;
//                    res.Data = adHoc;
//                }
//                else
//                {
//                    res.Message = "Ad Hoc Not Found";
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

//        #endregion Api To Get Ad Hoc By Id

//        #region Api To Delete Ad HOC Components

//        /// <summary>
//        /// Created By Harshit Mitra on 19-04-2022
//        /// API >> Post >> api/components/deleteadhoc
//        /// </summary>
//        /// <param name="adHocId"></param>
//        /// <returns></returns>
//        [HttpPut]
//        [Route("deleteadhoc")]
//        public async Task<ResponseBodyModel> DeleteAdHOC(int adHocId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var adHoc = await _db.AdHocComponents.FirstOrDefaultAsync(x => x.CompanyId == claims.companyId &&
//                        x.IsDeleted == false && x.IsActive == true && x.AdHocId == adHocId);
//                if (adHoc != null)
//                {
//                    adHoc.IsActive = false;
//                    adHoc.IsDeleted = true;
//                    adHoc.DeletedBy = claims.employeeId;
//                    adHoc.DeletedOn = DateTime.Now;

//                    _db.Entry(adHoc).State = System.Data.Entity.EntityState.Modified;
//                    await _db.SaveChangesAsync();

//                    res.Message = "Ad Hoc Updated";
//                    res.Status = true;
//                    res.Data = adHoc;
//                }
//                else
//                {
//                    res.Message = "Ad Hoc Not Found";
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

//        #endregion Api To Delete Ad HOC Components

//        #region Api To Edit Ad Hoc Components

//        /// <summary>
//        /// Created By Harshit Mitra on 19-04-2022
//        /// API >> Post >> api/components/editadhoc
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPut]
//        [Route("editadhoc")]
//        public async Task<ResponseBodyModel> UpdateAdHOC(AdHocComponent model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var adHoc = await _db.AdHocComponents.FirstOrDefaultAsync(x => x.CompanyId == claims.companyId &&
//                        x.IsDeleted == false && x.IsActive == true && x.AdHocId == model.AdHocId);
//                if (adHoc != null)
//                {
//                    adHoc.Name = model.Name;
//                    adHoc.Description = model.Description;
//                    adHoc.HasTaxBenefits = model.HasTaxBenefits;

//                    adHoc.UpdatedBy = claims.employeeId;
//                    adHoc.UpdatedOn = DateTime.Now;

//                    _db.Entry(adHoc).State = System.Data.Entity.EntityState.Modified;
//                    await _db.SaveChangesAsync();

//                    res.Message = "Ad Hoc Updated";
//                    res.Status = true;
//                    res.Data = adHoc;
//                }
//                else
//                {
//                    res.Message = "Ad Hoc Not Found";
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

//        #endregion Api To Edit Ad Hoc Components

//        /// ------------------ Bonus Components --------------------- ///

//        #region Api To Add Bonus Components

//        /// <summary>
//        /// Created By Harshit Mitra on 20-04-2022
//        /// API >> Post >> api/components/addbonus
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("addbonus")]
//        public async Task<ResponseBodyModel> CreateBonus(BonusComponent model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                if (model == null)
//                {
//                    res.Message = "Model Is Invalid";
//                    res.Status = false;
//                }
//                else
//                {
//                    BonusComponent obj = new BonusComponent();
//                    obj.BonusName = model.BonusName;
//                    obj.Description = model.Description;
//                    obj.IsDeleted = false;
//                    obj.IsActive = true;
//                    obj.CreatedBy = claims.employeeId;
//                    obj.CreatedOn = DateTime.Now;
//                    obj.CompanyId = claims.companyId;
//                    obj.OrgId = 0;

//                    _db.BonusComponents.Add(obj);
//                    await _db.SaveChangesAsync();

//                    res.Message = "Bonus Added";
//                    res.Status = true;
//                    res.Data = obj;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Add Bonus Components

//        #region Api To Get All Active Bonus Components

//        /// <summary>
//        /// Created By Harshit Mitra On 20-04-2022
//        /// API >> Get >> api/components/allactivebonus
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("allactivebonus")]
//        public async Task<ResponseBodyModel> GetAllActiveBonus()
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var bonusList = await _db.BonusComponents.Where(x => x.IsActive == true &&
//                        x.IsDeleted == false && x.CompanyId == claims.companyId)
//                        .ToListAsync();
//                if (bonusList.Count > 0)
//                {
//                    res.Message = "Bonus List";
//                    res.Status = true;
//                    res.Data = bonusList;
//                }
//                else
//                {
//                    res.Message = "Bonus Not Found";
//                    res.Status = false;
//                    res.Data = bonusList;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get All Active Bonus Components

//        #region Api To Get Bonus Component By Id

//        /// <summary>
//        /// Created By Harshit Mitra On 20-04-2022
//        /// API >> Get >> api/components/bonusbyid
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("bonusbyid")]
//        public async Task<ResponseBodyModel> GetBonusById(int bonusId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var bonus = await _db.BonusComponents.FirstOrDefaultAsync(x => x.IsActive == true &&
//                        x.BonusId == bonusId && x.IsDeleted == false && x.CompanyId == claims.companyId);
//                if (bonus != null)
//                {
//                    res.Message = "Bonus Found";
//                    res.Status = true;
//                    res.Data = bonus;
//                }
//                else
//                {
//                    res.Message = "Bonus Not Found";
//                    res.Status = false;
//                    res.Data = bonus;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Bonus Component By Id

//        #region Api To Edit Bonus Components

//        /// <summary>
//        /// Created By Harshit Mitra On 20-04-2022
//        /// API >> Put >> api/components/editbonus
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPut]
//        [Route("editbonus")]
//        public async Task<ResponseBodyModel> EditBonus(BonusComponent model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var bonus = await _db.BonusComponents.FirstOrDefaultAsync(x => x.IsActive == true &&
//                        x.BonusId == model.BonusId && x.IsDeleted == false && x.CompanyId == claims.companyId);
//                if (bonus != null)
//                {
//                    bonus.BonusName = model.BonusName;
//                    bonus.Description = model.Description;
//                    bonus.UpdatedOn = DateTime.Now;
//                    bonus.UpdatedBy = claims.employeeId;

//                    _db.Entry(bonus).State = System.Data.Entity.EntityState.Modified;
//                    await _db.SaveChangesAsync();

//                    res.Message = "Bonus Updated";
//                    res.Status = true;
//                    res.Data = bonus;
//                }
//                else
//                {
//                    res.Message = "Bonus Not Found";
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

//        #endregion Api To Edit Bonus Components

//        #region Api To Delete Bonus Component

//        /// <summary>
//        /// Created By Harshit Mitra On 20-04-2022
//        /// API >> Put >> api/components/deletebonus
//        /// </summary>
//        /// <param name="bonusId"></param>
//        /// <returns></returns>
//        [HttpPut]
//        [Route("deletebonus")]
//        public async Task<ResponseBodyModel> DeleteBonus(int bonusId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var bonus = await _db.BonusComponents.FirstOrDefaultAsync(x => x.IsActive == true &&
//                        x.BonusId == bonusId && x.IsDeleted == false && x.CompanyId == claims.companyId);
//                if (bonus != null)
//                {
//                    bonus.IsDeleted = true;
//                    bonus.IsActive = false;
//                    bonus.DeletedOn = DateTime.Now;
//                    bonus.DeletedBy = claims.employeeId;

//                    _db.Entry(bonus).State = System.Data.Entity.EntityState.Modified;
//                    await _db.SaveChangesAsync();

//                    res.Message = "Bonus Updated";
//                    res.Status = true;
//                    res.Data = bonus;
//                }
//                else
//                {
//                    res.Message = "Bonus Not Found";
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

//        #endregion Api To Delete Bonus Component

//        /// -------------- Deduction Component ----------------------- ///

//        #region Api To Add Deduction Components

//        /// <summary>
//        /// Created By Harshit Mitra on 20-04-2022
//        /// API >> Post >> api/components/adddeduction
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route("adddeduction")]
//        public async Task<ResponseBodyModel> CreateDeduction(DeductionComponent model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                if (model == null)
//                {
//                    res.Message = "Model Is Invalid";
//                    res.Status = false;
//                }
//                else
//                {
//                    DeductionComponent obj = new DeductionComponent();
//                    obj.DeductionName = model.DeductionName;
//                    obj.Description = model.Description;
//                    obj.HasAffectOnGross = model.HasAffectOnGross;
//                    obj.Country = model.Country;
//                    obj.State = model.State;

//                    obj.Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month);
//                    obj.Deductionfor = model.Deductionfor;
//                    obj.isPercentage = model.isPercentage;
//                    obj.Value = model.Value;
//                    obj.Component = model.Component;
//                    obj.Limit = model.Limit;
//                    obj.Min = model.Min;
//                    obj.Max = model.Max;

//                    obj.IsDeleted = false;
//                    obj.IsActive = true;
//                    obj.CreatedBy = claims.employeeId;
//                    obj.CreatedOn = DateTime.Now;
//                    obj.CompanyId = claims.companyId;
//                    obj.OrgId = 0;

//                    _db.DeductionComponents.Add(obj);
//                    await _db.SaveChangesAsync();

//                    res.Message = "Bonus Added";
//                    res.Status = true;
//                    res.Data = obj;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Add Deduction Components

//        #region Api To Get All Active Deduction Components

//        /// <summary>
//        /// Created By Harshit Mitra On 20-04-2022
//        /// API >> Get >> api/components/allactivededuction
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("allactivededuction")]
//        public async Task<ResponseBodyModel> GetAllActiveDeduction()
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var deductionList = await _db.DeductionComponents.Where(x => x.IsActive &&
//                        !x.IsDeleted && x.CompanyId == claims.companyId)
//                        .ToListAsync();
//                if (deductionList.Count > 0)
//                {
//                    res.Message = "Deduction List";
//                    res.Status = true;
//                    res.Data = deductionList;
//                }
//                else
//                {
//                    res.Message = "Deduction Not Found";
//                    res.Status = false;
//                    res.Data = deductionList;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get All Active Deduction Components

//        #region Api To Get Deduction Component By Id

//        /// <summary>
//        /// Created By Harshit Mitra On 20-04-2022
//        /// API >> Get >> api/components/deductionbyid
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Route("deductionbyid")]
//        public async Task<ResponseBodyModel> GetDeductionById(int deductionId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var deduction = await _db.DeductionComponents.FirstOrDefaultAsync(x => x.IsActive &&
//                        x.DeductionId == deductionId && !x.IsDeleted && x.CompanyId == claims.companyId);
//                if (deduction != null)
//                {
//                    res.Message = "Deduction Found";
//                    res.Status = true;
//                    res.Data = deduction;
//                }
//                else
//                {
//                    res.Message = "Deduction Not Found";
//                    res.Status = false;
//                    res.Data = deduction;
//                }
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        #endregion Api To Get Deduction Component By Id

//        #region Api To Edit Deduction Components

//        /// <summary>
//        /// Created By Harshit Mitra On 20-04-2022
//        /// API >> Put >> api/components/editdeduction
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPut]
//        [Route("editdeduction")]
//        public async Task<ResponseBodyModel> EditDeduction(DeductionComponent model)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var deduction = await _db.DeductionComponents.FirstOrDefaultAsync(x => x.IsActive &&
//                        x.DeductionId == model.DeductionId && !x.IsDeleted && x.CompanyId == claims.companyId);
//                if (deduction != null)
//                {
//                    deduction.DeductionName = model.DeductionName;
//                    deduction.Description = model.Description;
//                    deduction.HasAffectOnGross = model.HasAffectOnGross;

//                    deduction.Country = model.Country;
//                    deduction.State = model.State;
//                    deduction.Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month);
//                    deduction.Deductionfor = model.Deductionfor;
//                    deduction.isPercentage = model.isPercentage;
//                    deduction.Value = model.Value;
//                    deduction.Component = model.Component;
//                    deduction.Limit = model.Limit;
//                    deduction.Min = model.Min;
//                    deduction.Max = model.Max;

//                    deduction.UpdatedOn = DateTime.Now;
//                    deduction.UpdatedBy = claims.employeeId;

//                    _db.Entry(deduction).State = System.Data.Entity.EntityState.Modified;
//                    await _db.SaveChangesAsync();

//                    res.Message = "Bonus Updated";
//                    res.Status = true;
//                    res.Data = deduction;
//                }
//                else
//                {
//                    res.Message = "Bonus Not Found";
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

//        #endregion Api To Edit Deduction Components

//        #region Api To Delete Deduction Component

//        /// <summary>
//        /// Created By Harshit Mitra On 20-04-2022
//        /// API >> Put >> api/components/deletededuction
//        /// </summary>
//        /// <param name="deductionId"></param>
//        /// <returns></returns>
//        [HttpPut]
//        [Route("deletededuction")]
//        public async Task<ResponseBodyModel> DeleteDeduction(int deductionId)
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
//            try
//            {
//                var deduction = await _db.DeductionComponents.FirstOrDefaultAsync(x => x.IsActive &&
//                         x.DeductionId == deductionId && !x.IsDeleted && x.CompanyId == claims.companyId);
//                if (deduction != null)
//                {
//                    deduction.IsDeleted = true;
//                    deduction.IsActive = false;
//                    deduction.DeletedOn = DateTime.Now;
//                    deduction.DeletedBy = claims.employeeId;

//                    _db.Entry(deduction).State = System.Data.Entity.EntityState.Modified;
//                    await _db.SaveChangesAsync();

//                    res.Message = "Bonus Updated";
//                    res.Status = true;
//                    res.Data = deduction;
//                }
//                else
//                {
//                    res.Message = "Bonus Not Found";
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

//        #endregion Api To Delete Deduction Component

//        #region Get Deduction type  // dropdown
//        /// <summary>
//        /// Created By Suraj Bundel on 14-11-2022
//        /// API >> Get >>api/components/getdeductionenum
//        /// Dropdown using Enum for Deduction
//        /// </summary>
//        [Route("getdeductionenum")]
//        [HttpGet]
//        public ResponseBodyModel DeductionEnumData()
//        {
//            ResponseBodyModel res = new ResponseBodyModel();
//            try
//            {
//                var DeductionType = Enum.GetValues(typeof(DeductionType))
//                    .Cast<DeductionType>()
//                    .Select(x => new DeductionHelperModel
//                    {
//                        TypeId = (int)x,
//                        TypeName = Enum.GetName(typeof(DeductionType), x).Replace("_", " ")
//                    }).ToList();

//                res.Message = "List Deduction";
//                res.Status = true;
//                res.Data = DeductionType;
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//                res.Status = false;
//            }
//            return res;
//        }

//        public class DeductionHelperModel
//        {
//            public int TypeId { get; set; }
//            public string TypeName { get; set; }
//        }
//        #endregion


//    }
//}