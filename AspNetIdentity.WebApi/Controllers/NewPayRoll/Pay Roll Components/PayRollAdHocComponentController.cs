using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.New_Pay_Roll;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.NewPayRoll.PayRollComponents
{
    /// <summary>
    /// Created By Harshit Mitra On 14/12/2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/adhoccomponents")]
    public class PayRollAdHocComponentController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API TO ADD AD-HOC COMPONENTS 
        /// <summary>
        /// Created By Harshit Mitra On 14/12/2022
        /// API >> POST >> api/adhoccomponents/createadhoccomponent
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createadhoccomponent")]
        public async Task<IHttpActionResult> CreateAdHocComponent(CreateAdHocComponentRequest model)
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
                    AdHocComponent obj = new AdHocComponent
                    {
                        ComponentType = model.ComponentType,
                        Title = model.Title,
                        Description = model.Description,
                        HasTaxBenefits = model.HasTaxBenefits,

                        CompanyId = tokenData.companyId,
                        CreatedBy = tokenData.employeeId,
                    };

                    _db.AdHocComponents.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "Component Created";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    res.Data = obj;

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/adhoccomponents/createadhoccomponentp | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO EDIT AD-HOC COMPONENTS 
        /// <summary>
        /// Created By Harshit Mitra On 14/12/2022
        /// API >> POST >> api/adhoccomponents/editadhoccomponent
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("editadhoccomponent")]
        public async Task<IHttpActionResult> EditAdHocComponent(EditAdHocComponentRequest model)
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
                    var adHocComponent = await _db.AdHocComponents
                        .FirstOrDefaultAsync(x => x.ComponentId == model.ComponentId);
                    if (adHocComponent == null)
                    {
                        res.Message = "Component Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;

                        return Ok(res);
                    }

                    adHocComponent.ComponentType = model.ComponentType;
                    adHocComponent.Title = model.Title;
                    adHocComponent.Description = model.Description;
                    adHocComponent.HasTaxBenefits = model.HasTaxBenefits;

                    _db.Entry(adHocComponent).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Component Edited";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = adHocComponent;

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/adhoccomponents/editeadhoccomponent | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO DELETE AD-HOC COMPONENTS
        /// <summary>
        /// Created By Harshit Mitra On 14/12/2022
        /// API >> POST >> api/adhoccomponents/deleteadhoccomponent
        /// </summary>
        /// <param name="componentId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deleteadhoccomponent")]
        public async Task<IHttpActionResult> DeleteAdHocComponent(Guid componentId)
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
                    var adHocComponent = await _db.AdHocComponents
                        .FirstOrDefaultAsync(x => x.ComponentId == componentId);
                    if (adHocComponent == null)
                    {
                        res.Message = "Component Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;

                        return Ok(res);
                    }

                    adHocComponent.IsActive = false;
                    adHocComponent.IsDeleted = true;

                    _db.Entry(adHocComponent).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Component Edited";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = adHocComponent;

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/adhoccomponents/deleteadhoccomponent | " +
                    "ComponentId : " + componentId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET AD-HOC COMPONENT BY ID
        /// <summary>
        /// Created By Harshit Mitra On 15/12/2022
        /// API >> POST >> api/adhoccomponents/getadhoccomponentbyid
        /// </summary>
        /// <param name="componentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getadhoccomponentbyid")]
        public async Task<IHttpActionResult> GetAdHocComponentById(Guid componentId)
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
                    var adHocComponent = await _db.AdHocComponents
                        .FirstOrDefaultAsync(x => x.ComponentId == componentId);
                    if (adHocComponent == null)
                    {
                        res.Message = "Component Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;

                        return Ok(res);
                    }
                    res.Message = "Component Found";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = adHocComponent;

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/adhoccomponents/getadhoccomponentbyid | " +
                    "ComponentId : " + componentId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET ALL AD-HOC COMPONENT 
        /// <summary>
        /// Created By Harshit Mitra On 15/12/2022
        /// API >> POST >> api/adhoccomponents/getalladhoccomponent
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getalladhoccomponent")]
        public async Task<IHttpActionResult> GetAllAdHocComponent()
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
                    var adHocComponent = await _db.AdHocComponents
                        .Where(x => x.IsActive && !x.IsDeleted)
                        .ToListAsync();
                    if (adHocComponent.Count == 0)
                    {
                        res.Message = "Component Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;

                        return Ok(res);
                    }
                    var response = Enum.GetValues(typeof(AdHocComponentTypeConstants))
                                .Cast<AdHocComponentTypeConstants>()
                                .Select(x => new
                                {
                                    Id = (int)x,
                                    Name = x.ToString().Replace("_", " "),
                                    ComponentsList = adHocComponent
                                            .Where(z => z.ComponentType == x)
                                            .Select(z => new
                                            {
                                                z.ComponentId,
                                                z.Title,
                                                z.Description,
                                                z.HasTaxBenefits,
                                            })
                                            .ToList(),

                                })
                                .ToList();
                    res.Message = "Component Found";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = response;

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/adhoccomponents/getalladhoccomponent | " +
                    //"ComponentId : " + componentId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET AD-HOC COMPONENT LIST BY TYPE
        /// <summary>
        /// Created By Harshit Mitra On 15/12/2022
        /// API >> POST >> api/adhoccomponents/getalladhoccomponentbytype
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getalladhoccomponentbytype")]
        public async Task<IHttpActionResult> GetAllAdHocComponentByType(AdHocComponentTypeConstants componentType)
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
                    var adHocComponent = await _db.AdHocComponents
                        .Where(x => x.IsActive && !x.IsDeleted && x.ComponentType == componentType)
                        .Select(x => new
                        {
                            x.ComponentId,
                            x.ComponentType,
                            x.Title,
                            x.Description,
                            x.HasTaxBenefits,
                        })
                        .ToListAsync();
                    if (adHocComponent == null)
                    {
                        res.Message = "Component Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;

                        return Ok(res);
                    }
                    res.Message = "Component Found";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = adHocComponent;

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/adhoccomponents/getalladhoccomponent | " +
                    //"ComponentId : " + componentId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion




        #region RESPONSE AND REQUEST
        public class CreateAdHocComponentRequest
        {
            public AdHocComponentTypeConstants ComponentType { get; set; }
            [Required]
            public string Title { get; set; } = String.Empty;
            public string Description { get; set; } = String.Empty;
            public bool HasTaxBenefits { get; set; } = false;
        }
        public class EditAdHocComponentRequest : CreateAdHocComponentRequest
        {
            [Required]
            public Guid ComponentId { get; set; }
        }
        #endregion

    }
}
