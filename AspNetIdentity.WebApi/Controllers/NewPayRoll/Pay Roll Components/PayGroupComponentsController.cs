using AspNetIdentity.WebApi.Controllers.NewPayRoll.Pay_Roll_Components;
using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.New_Pay_Roll;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.NewPayRoll.PayRollComponents
{
    [Authorize]
    [RoutePrefix("api/components")]
    public class PayGroupComponentsController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API TO GET COMPONENT TYPE LIST 
        /// <summary>
        /// Created By Harshit Mitra On 14/12/2022
        /// API >> GET >> api/components/componenttypelist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("componenttypelist")]
        public IHttpActionResult GetAllComponentTypeList()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var componentTypeList = Enum.GetValues(typeof(PayRollComponentTypeConstants))
                                .Cast<PayRollComponentTypeConstants>()
                                .Where(x => x != PayRollComponentTypeConstants.Fixed)
                                .Select(x => new
                                {
                                    Id = (int)x,
                                    Name = x.ToString().Replace("_", " "),

                                }).ToList();
                res.Message = "Component Type List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = componentTypeList;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/components/componenttypelist | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO CREATE RECURING COMPONENTS 
        /// <summary>
        /// Created By Harshit Mitra On 08/12/2022
        /// API >> POST >> api/components/createcomponent
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createcomponent")]
        public async Task<IHttpActionResult> CreatePayRollComponents(CreatePayRollComponetResponse model)
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
                    RecuringComponent obj = new RecuringComponent
                    {
                        ComponentName = model.ComponentName,
                        ComponentType = model.ComponentType,
                        IsAutoCalculated = model.IsAutoCalculated,
                        MaxiumLimitPerYear = model.MaxiumLimitPerYear,
                        Description = model.Description,
                        IsTaxExempted = model.IsTaxExempted,
                        IncomeTaxSection = model.IncomeTaxSection,
                        SectionMaxLimit = model.SectionMaxLimit,
                        IsDocumentRequired = model.IsDocumentRequired,
                        ShowOnPaySlip = model.ShowOnPaySlip,

                        CreatedBy = tokenData.employeeId,
                        CompanyId = tokenData.companyId,
                    };

                    _db.RecuringComponents.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "Component Added";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    res.Data = obj;

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/components/createcomponent | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO EDIT RECURING COMPONENTS 
        /// <summary>
        /// Created By Harshit Mitra On 08/12/2022
        /// API >> POST >> api/components/editcomponent
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("editcomponent")]
        public async Task<IHttpActionResult> EditPayRollComponents(EditPayRollComponentResponse model)
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
                    var component = await _db.RecuringComponents
                        .FirstOrDefaultAsync(x => x.RecuringComponentId == model.RecuringComponentId);
                    if (component == null)
                    {
                        res.Message = "Component Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;

                        return Ok(res);
                    }
                    string oldCode = component.ComponentName.ToUpper().Replace(" ", "");

                    component.ComponentName = model.ComponentName.Trim();
                    component.ComponentType = model.ComponentType;
                    component.IsAutoCalculated = model.IsAutoCalculated;
                    component.MaxiumLimitPerYear = model.MaxiumLimitPerYear;
                    component.Description = model.Description;
                    component.IsTaxExempted = model.IsTaxExempted;
                    component.IncomeTaxSection = model.IncomeTaxSection;
                    component.SectionMaxLimit = model.SectionMaxLimit;
                    component.IsDocumentRequired = model.IsDocumentRequired;
                    component.ShowOnPaySlip = model.ShowOnPaySlip;

                    component.UpdatedBy = tokenData.employeeId;
                    component.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);

                    _db.Entry(component).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Component Updaed";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = component;

                    //HostingEnvironment.QueueBackgroundWorkItem(ct => HostThreadAsync(component, oldCode));

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/components/editcomponent | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public async Task HostThreadAsync(RecuringComponent component, string oldCode)
        {
            Thread.Sleep(100);
            using (var ht = new HostingEnviormentController())
            {
                bool check = await ht.ChangeInStructureRecuringComponent(component, oldCode);
                logger.Info("ChangeInStructureRecuringComponent : " + check);
            };
        }
        #endregion

        #region API TO DELETE RECURING COMPONENTS
        /// <summary>
        /// Created By Harshit Mitra On 08/12/2022
        /// API >> POST >> api/components/deletecomponent
        /// </summary>
        /// <param name="recuringComponentId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deletecomponent")]
        public async Task<IHttpActionResult> DeletePayRollComponents(Guid recuringComponentId)
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
                    var component = await _db.RecuringComponents
                        .FirstOrDefaultAsync(x => x.RecuringComponentId == recuringComponentId);
                    if (component == null)
                    {
                        res.Message = "Component Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;

                        return Ok(res);
                    }
                    component.IsActive = false;
                    component.IsDeleted = true;

                    component.DeletedBy = tokenData.employeeId;
                    component.DeletedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);

                    _db.Entry(component).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Component Updaed";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = component;

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/components/deletecomponent | " +
                    "RecuringComponentId : " + recuringComponentId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET RECURING COMPONENTS BY ID
        /// <summary>
        /// Created By Harshit Mitra On 08/12/2022
        /// API >> POST >> api/components/getcomponentbyid
        /// </summary>
        /// <param name="recuringComponentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcomponentbyid")]
        public async Task<IHttpActionResult> GetPayRollComponentsById(Guid recuringComponentId)
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
                    var component = await _db.RecuringComponents
                        .FirstOrDefaultAsync(x => x.RecuringComponentId == recuringComponentId);
                    if (component == null)
                    {
                        res.Message = "Component Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;

                        return Ok(res);
                    }

                    res.Message = "Component Found";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = component;

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/components/getcomponentbyid | " +
                    "RecuringComponentId : " + recuringComponentId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET ALL COMPONENT LIST 
        /// <summary>
        /// Created By Harshit Mitra On 08/12/2022
        /// API >> GET >> api/components/getallcomponent
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallcomponent")]
        public async Task<IHttpActionResult> GetAllPayRollComponents()
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
                    var componentList = await _db.RecuringComponents
                        .Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId)
                        .Select(x => new
                        {
                            x.RecuringComponentId,
                            x.ComponentName,
                            ComponentType = x.ComponentType.ToString().Replace("_", " "),
                            x.IsTaxExempted,
                            x.IncomeTaxSection,
                            x.IsDocumentRequired,
                            x.IsEditable,
                            x.IsAutoCalculated,
                            x.MaxiumLimitPerYear,
                        })
                        .OrderBy(x => x.IsEditable)
                        .ToListAsync();
                    if (componentList.Count == 0)
                        componentList = AddComponentBasic(tokenData.companyId)
                            .Select(x => new
                            {
                                x.RecuringComponentId,
                                x.ComponentName,
                                ComponentType = x.ComponentType.ToString().Replace("_", " "),
                                x.IsTaxExempted,
                                x.IncomeTaxSection,
                                x.IsDocumentRequired,
                                x.IsEditable,
                                x.IsAutoCalculated,
                                x.MaxiumLimitPerYear,
                            })
                            .OrderBy(x => x.IsEditable)
                            .ToList();

                    res.Message = "Component List";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = componentList;

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/components/getallcomponent | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        #endregion


        #region RESPONSE AND REQUEST 
        public class CreatePayRollComponetResponse
        {
            public string ComponentName { get; set; } = String.Empty;
            public PayRollComponentTypeConstants ComponentType { get; set; }
            public bool IsAutoCalculated { get; set; } = false;
            public double MaxiumLimitPerYear { get; set; } = 0;
            public string Description { get; set; } = String.Empty;
            public bool IsTaxExempted { get; set; } = false;
            public string IncomeTaxSection { get; set; } = String.Empty;
            public double SectionMaxLimit { get; set; } = 0;
            public bool IsDocumentRequired { get; set; } = false;
            public bool ShowOnPaySlip { get; set; } = true;
        }
        public class EditPayRollComponentResponse : CreatePayRollComponetResponse
        {
            public Guid RecuringComponentId { get; set; }
        }

        public List<RecuringComponent> AddComponentBasic(int companyId)
        {
            string[] components = { "Basic", "HRA", "Special Allowance" };
            var componentAdd = components
                .Select(x => new RecuringComponent
                {
                    ComponentName = x,
                    ComponentType = PayRollComponentTypeConstants.Fixed,
                    IsTaxExempted = (x == "HRA"),
                    IncomeTaxSection = (x == "HRA") ? "Section_10(13)(a)" : String.Empty,
                    IsEditable = false,
                    IsAutoCalculated = true,

                    CompanyId = companyId,

                }).ToList();
            _db.RecuringComponents.AddRange(componentAdd);
            _db.SaveChanges();
            return componentAdd;
        }
        #endregion

    }
}
