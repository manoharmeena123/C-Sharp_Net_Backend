using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Interface.IUserAccessService;
using AspNetIdentity.WebApi.Services.UserAccessService;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Req = AspNetIdentity.Core.ViewModel.UserAccessViewModel.RequestModuleAccessClass;


namespace AspNetIdentity.WebApi.Controllers.UIAccess
{
    [Authorize]
    [RoutePrefix("api/moduleaccess")]
    public class ModuleAccessController : ApiController
    {
        #region Properties
        private readonly IModuleAccessService _moduleAccess;
        #endregion

        #region Constructor
        public ModuleAccessController()
        {
            _moduleAccess = new ModuleAccessService();
        }
        #endregion

        /// <summary>
        /// Created By Harshit Mitra On 10-04-2023
        /// API >> GET >> api/moduleaccess/getmodulelist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getmodulelist")]
        public async Task<IHttpActionResult> GetModuleList()
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            //var response = await _moduleAccess.GetModuleListsForUpdate();
            var response = await _moduleAccess.GetModuleList(tokenData);
            return Ok(response);
        }

        /// <summary>
        /// Created By Harshit Mitra On 10-04-2023
        /// API >> POST >> api/moduleaccess/createroleandpermission
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createroleandpermission")]
        public async Task<IHttpActionResult> CreateRoleAndPermission(Req.CreateRoleViewClassRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _moduleAccess.CreateRoleWithPermission(tokenData, model);
            return Ok(response);
        }

        /// <summary>
        /// Created By Harshit Mitra On 10-04-2023
        /// API >> GET >> api/moduleaccess/getroleandpermissionbyid
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getroleandpermissionbyid")]
        public async Task<IHttpActionResult> GetRoleAndPermissionById(Guid roleId)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _moduleAccess.GetRoleAndPermissionById(tokenData, roleId);
            return Ok(response);
        }

        /// <summary>
        /// Created By Harshit Mitra On 10-04-2023
        /// API >> POST >> api/moduleaccess/updaterolewithpermission
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updaterolewithpermission")]
        public async Task<IHttpActionResult> UpdateRoleWithPermission(Req.UpdateRoleViewClassRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _moduleAccess.UpdateRoleWithPermission(tokenData, model);
            return Ok(response);
        }
    }
}
