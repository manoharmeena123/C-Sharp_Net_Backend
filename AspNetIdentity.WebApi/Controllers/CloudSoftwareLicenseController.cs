using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.CloudSofwareLicenes;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.CloudSoftwareLicenses
{
    /// <summary>
    /// Created By Ankit Jain On 14-11-2022
    /// </summary>
    [Authorize]
    [RoutePrefix("api/cloudsoftwarelicense")]
    public class CloudSoftwareLicenseController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API TO Add Module And SubModule
        /// <summary>
        /// Created By Ankit Jain On 15-11-2022
        /// API >> POST >> api/cloudsoftwarelicense/createcloudsoftwarelicense
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("createcloudsoftwarelicense")]
        public async Task<IHttpActionResult> CreateModuleAndSubmodule(CloudSoftwarelicenseHelperClass model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    CloudSoftwareLicense obj = new CloudSoftwareLicense
                    {
                        UserName = model.UserName,
                        Country = model.Country,
                        Department = model.Department,
                        USerCreatedDate = model.UserCreatedDate,
                        IsLicensed = model.IsLicensed,
                        LicensePlanWithEnabledService = model.LicensePlanWithEnabledService,
                        FriendlyNameOfLicensePlanAndEnabledService = model.FriendlyNameOfLicensePlanAndEnabledService,
                        CreateOn = DateTime.Now,
                        CreatedBY = 0
                    };

                    _db.CloudSoftwareLicenses.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "Created";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    res.Data = obj;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/cloudsoftwarelicense/createcloudsoftwarelicense | " +
                    "Error : " + JsonConvert.SerializeObject(ex) + " | " +
                    "Request : " + JsonConvert.SerializeObject(model));
                return BadRequest("Failed");
            }
        }



        public class CloudSoftwarelicenseHelperClass
        {
            public string UserName { get; set; }
            public string Country { get; set; }
            public string Department { get; set; }
            public DateTimeOffset? UserCreatedDate { get; set; }
            public bool IsLicensed { get; set; }
            public string LicensePlanWithEnabledService { get; set; }
            public string FriendlyNameOfLicensePlanAndEnabledService { get; set; }
        }
        #endregion

    }
}
