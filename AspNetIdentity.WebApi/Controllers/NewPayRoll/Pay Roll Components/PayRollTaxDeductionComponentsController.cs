using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.New_Pay_Roll;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers.NewPayRoll.PayRollComponents
{
    /// <summary>
    /// Created By Harshit Mitra On 15/12/2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/taxdeductioncomponent")]
    public class PayRollTaxDeductionComponentsController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API TO GET MONTH LIST 
        /// <summary>
        /// Created By Harshit Mitra On 15/12/2022
        /// API >> GET >> api/taxdeductioncomponent/getmonthlist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getmonthlist")]
        public IHttpActionResult GetAllMonthList()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var response = Enumerable
                    .Range(1, 12)
                    .Select(i => new
                    {
                        MonthId = i,
                        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                    })
                    .ToList();
                res.Message = "Month List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = response;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/taxdeductioncomponent/getallcomponent | " +
                                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET DEDUCTION COMPONET FOR ENUM LIST
        /// <summary>
        /// Created By Harshit Mitra On 15/12/2022
        /// API >> GET >> api/taxdeductioncomponent/getdeductionfor
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getdeductionfor")]
        public IHttpActionResult GetAllDeductionForConstants()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var response = Enum.GetValues(typeof(DeductionForConstants))
                                .Cast<DeductionForConstants>()
                                .Where(x => x != DeductionForConstants.Undefined)
                                .Select(x => new
                                {
                                    Id = (int)x,
                                    Name = x.ToString().Replace("_", " "),
                                })
                                .ToList();

                res.Message = "Month List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = response;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/taxdeductioncomponent/getallcomponent | " +
                                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO ADD TAX DEDUCTION COMPONENT
        /// <summary>
        /// Created By Harshit Mitra On 15/12/2022
        /// API >> POST >> api/taxdeductioncomponent/createcomponent
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createcomponent")]
        public async Task<IHttpActionResult> CreateTDComponents(CreateTaxDeductionComponentRequest model)
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
                    TaxDeductionComponent obj = new TaxDeductionComponent
                    {
                        DeductionName = model.DeductionName,
                        Description = model.Description,
                        CountryId = model.CountryId,
                        StateId = model.StateId,
                        Month = model.Month,
                        Deductionfor = model.DeductionFor,
                        IsPercentage = model.IsPercentage,
                        Value = model.Value,
                        Component = String.IsNullOrEmpty(model.Component) ? String.Empty : model.Component.ToUpper(),
                        Limit = model.Limit,
                        Min = model.Min,
                        Max = model.Max,

                        CreatedBy = tokenData.employeeId,
                        CompanyId = tokenData.companyId,
                    };

                    _db.TaxDeductions.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "Deduction Component Added";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    res.Data = obj;

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/taxdeductioncomponent/createcomponent | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO EDIT TAX DEDUCTION COMPONENT
        /// <summary>
        /// Created By Harshit Mitra On 15/12/2022
        /// API >> POST >> api/taxdeductioncomponent/editcomponent
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("editcomponent")]
        public async Task<IHttpActionResult> EditTDComponents(EditTaxDeductionComponentRequest model)
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
                    var taxComponent = await _db.TaxDeductions
                        .FirstOrDefaultAsync(x => x.TaxComponentId == model.TaxComponentId);
                    if (taxComponent == null)
                    {
                        res.Message = "Deduction Component Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;

                        return Ok(res);
                    }

                    taxComponent.DeductionName = model.DeductionName;
                    taxComponent.Description = model.Description;
                    taxComponent.CountryId = model.CountryId;
                    taxComponent.StateId = model.StateId;
                    taxComponent.Month = model.Month;
                    taxComponent.Deductionfor = model.DeductionFor;
                    taxComponent.IsPercentage = model.IsPercentage;
                    taxComponent.Value = model.Value;
                    taxComponent.Component = String.IsNullOrEmpty(model.Component) ? String.Empty : model.Component.ToUpper();
                    taxComponent.Limit = model.Limit;
                    taxComponent.Min = model.Min;
                    taxComponent.Max = model.Max;

                    taxComponent.UpdatedBy = tokenData.employeeId;
                    taxComponent.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);

                    _db.Entry(taxComponent).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Deduction Component Edited";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = taxComponent;

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/taxdeductioncomponent/editcomponent | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO DELETE TAX DEDUCTION COMPONENT
        /// <summary>
        /// Created By Harshit Mitra On 15/12/2022
        /// API >> POST >> api/taxdeductioncomponent/deletecomponent
        /// </summary>
        /// <param name="taxComponentId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deletecomponent")]
        public async Task<IHttpActionResult> DeleteTDComponents(Guid taxComponentId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var taxComponent = await _db.TaxDeductions
                    .FirstOrDefaultAsync(x => x.TaxComponentId == taxComponentId);
                if (taxComponent == null)
                {
                    res.Message = "Deduction Component Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;

                    return Ok(res);
                }

                taxComponent.IsActive = false;
                taxComponent.IsDeleted = true;

                taxComponent.DeletedBy = tokenData.employeeId;
                taxComponent.DeletedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow);

                _db.Entry(taxComponent).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "Deduction Component Deleted";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/taxdeductioncomponent/deletecomponent | " +
                    "TaxComponentId : " + taxComponentId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET TAX DEDUCTION COMPONENT BY ID
        /// <summary>
        /// Created By Harshit Mitra On 15/12/2022
        /// API >> POST >> api/taxdeductioncomponent/getcomponentbyid
        /// </summary>
        /// <param name="taxComponentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcomponentbyid")]
        public async Task<IHttpActionResult> GetTDComponentsById(Guid taxComponentId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var taxComponent = await _db.TaxDeductions
                    .FirstOrDefaultAsync(x => x.TaxComponentId == taxComponentId);
                if (taxComponent == null)
                {
                    res.Message = "Deduction Component Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;

                    return Ok(res);
                }

                res.Message = "Deduction Component Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;
                res.Data = taxComponent;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/taxdeductioncomponent/getcomponentbyid | " +
                    "TaxComponentId : " + taxComponentId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET ALL TAX DEDUCTION COMPONENT
        /// <summary>
        /// Created By Harshit Mitra On 15/12/2022
        /// API >> POST >> api/taxdeductioncomponent/getallcomponent
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallcomponent")]
        public async Task<IHttpActionResult> GetAllTDComponents()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var taxComponent = await _db.TaxDeductions
                    .Where(x => x.IsActive && !x.IsDeleted)
                    .ToListAsync();

                if (taxComponent.Count == 0)
                {
                    res.Message = "Deduction Component Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = taxComponent;

                    return Ok(res);
                }
                var countries = await _db.Country.ToListAsync();
                var states = await _db.State.ToListAsync();

                res.Message = "Deduction Component Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = taxComponent
                    .Select(x => new
                    {
                        x.TaxComponentId,
                        x.DeductionName,
                        CountryName = countries.Where(z => z.CountryId == x.CountryId).Select(z => z.CountryName).FirstOrDefault(),
                        StateName = states.Where(z => z.StateId == x.StateId).Select(z => z.StateName).FirstOrDefault(),
                        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month),
                        Deductionfor = x.Deductionfor.ToString(),
                        x.IsPercentage,
                        x.Value,
                        x.Component,
                        x.Limit,
                        x.Max,
                        x.Min,
                    })
                    .ToList();

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/taxdeductioncomponent/getallcomponent | " +
                    //"TaxComponentId : " + taxComponentId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion


        #region REQUEST AND RESPONSE 
        public class CreateTaxDeductionComponentRequest
        {
            public string DeductionName { get; set; } = String.Empty;
            public string Description { get; set; } = String.Empty;
            public int CountryId { get; set; } = 0;
            public int StateId { get; set; } = 0;
            public int Month { get; set; } = 13;
            public DeductionForConstants DeductionFor { get; set; } = DeductionForConstants.Undefined;
            public bool IsPercentage { get; set; } = false;
            public double Value { get; set; } = 0.0;
            public string Component { get; set; } = String.Empty;
            public double Limit { get; set; } = 0.0;
            public double Min { get; set; } = 0.0;
            public double Max { get; set; } = 0.0;
        }
        public class EditTaxDeductionComponentRequest : CreateTaxDeductionComponentRequest
        {
            public Guid TaxComponentId { get; set; }
        }
        #endregion

    }
}
