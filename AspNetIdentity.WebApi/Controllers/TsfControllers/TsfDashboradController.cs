using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure.ITsfService;
using AspNetIdentity.WebApi.Services.TsfService;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.Tsf
{
    [Authorize]
    [RoutePrefix("api/tsfdashborad")]
    public class TsfDashboradController : ApiController
    {
        #region Properties
        private readonly ITsfDashboradService _tsfDashborad;
        #endregion

        #region Constructor
        public TsfDashboradController()
        {
            _tsfDashborad = new TsfDashboradService();
        }
        #endregion

        #region This Api Use To Get Tsf Data
        /// <summary>
        /// Created By Ankit Jain 04-04-2023
        /// Api >> Get >> api/tsfdashborad/gettsfdeshboarddata
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("gettsfdeshboarddata")]
        public async Task<IHttpActionResult> GetTsfDashboard()
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _tsfDashborad.GetDasboradData(tokenData);
            return Ok(response);
        }
        #endregion
    }
}
