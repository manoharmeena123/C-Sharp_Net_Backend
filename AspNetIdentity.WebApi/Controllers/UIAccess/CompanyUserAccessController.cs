using AspNetIdentity.WebApi.Helper;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Req = AspNetIdentity.Core.ViewModel.UserAccessViewModel.RequestCompanyAccess;

namespace AspNetIdentity.WebApi.Controllers.UIAccess
{
    /// <summary>
    /// Created By Harshit Mitra On 03-04-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/companyaccess")]
    public class CompanyUserAccessController : ApiController
    {
        #region Properties
        private readonly AspNetIdentity.WebApi.Interface.IUserAccessService.ICompanyAccessService _companyAccess;
        #endregion

        #region Constructor
        public CompanyUserAccessController()
        {
            _companyAccess = new AspNetIdentity.WebApi.Services.UserAccessService.CompanyAccessService();
        }
        #endregion

        #region API TO GET COMPANY MODULE LIST
        /// <summary>
        /// Created By Harshit Mitra On 03-04-2023
        /// API >> GET >> api/companyaccess/getcompanymodulelist
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcompanymodulelist")]
        public async Task<IHttpActionResult> GetCompanyModuleList(int companyId)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _companyAccess.GetModuleList(companyId);
            return Ok(response);
        }
        #endregion

        #region API TO UPDATE MODULE ACCESS TO COMPANY
        /// <summary>
        /// Created By Harshit Mitra On 03-04-2023
        /// API >> POST >> api/companyaccess/updatecomapnymoduleaccess
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updatecomapnymoduleaccess")]
        public async Task<IHttpActionResult> UpdateComapnyModuleAccess(Req.UpdateCompanyAccessRequest model)
        {
            var response = await _companyAccess.UpdateCompanyAccess(model);
            return Ok(response);
        }
        #endregion

        #region API TO SET PERMISSION TO ALL COMPANIES
        /// <summary>
        /// API >> GET >> api/companyaccess/setaccesstoallcompanies
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("setaccesstoallcompanies")]
        public async Task<IHttpActionResult> SetAccessToAllCompanies()
        {
            var response = await _companyAccess.SetPermissionToAllCompanies();
            return Ok(response);
        }
        #endregion
    }
}
