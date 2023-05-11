using AspNetIdentity.WebApi.Infrastructure;
using NLog;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.ISA
{
    [Authorize]
    [RoutePrefix("api/isadashboard")]
    public class UserDashboardController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        //#region API TO GET CLIENT HOME DASHBOARD
        ///// <summary>
        ///// Created By Harshit Mitra On 23-01-2023
        ///// API >> GET >> api/isadashboard/technologieslistdashboard
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("technologieslistdashboard")]
        //public async Task<IHttpActionResult> GetTechnologiesDashboard()
        //{
        //    ResponseStatusCode res = new ResponseStatusCode();
        //    var clientTokenData = ClaimsHelper.RetrieveClientToken(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        var technologiesList = Enum.GetValues(typeof(ClientTechnologyConstants))
        //                        .Cast<BloodGroupConstants>()
        //                        .Select(x => new BooodGroupList
        //                        {
        //                            BloodGroupId = (int)x,
        //                            BloodGroupType = Enum.GetName(typeof(BloodGroupConstants), x).Contains("_pos") ?
        //                                             Enum.GetName(typeof(BloodGroupConstants), x).Replace("_pos", "+") :
        //                                             Enum.GetName(typeof(BloodGroupConstants), x).Replace("_neg", "-"),
        //                        }).ToList();
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("API : api/isadashboard/technologieslistdashboard | " +
        //            "Exception : " + JsonConvert.SerializeObject(ex));
        //        return BadRequest("Failed");
        //    }
        //}
        //#endregion
    }
}
