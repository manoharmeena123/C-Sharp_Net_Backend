using AspNetIdentity.Core.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Req = AspNetIdentity.Core.ViewModel.UserAccessViewModel.RequestModuleAccessClass;
using Res = AspNetIdentity.Core.ViewModel.UserAccessViewModel.ResponseModuleAccessClass;


namespace AspNetIdentity.WebApi.Interface.IUserAccessService
{
    internal interface IModuleAccessService
    {
        /// <summary>
        /// Get Module List API For Create Role Before Create Role.
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<Res.GetModuleSubModuleResponse>>> GetModuleList(ClaimsHelperModel tokenData);
        /// <summary>
        /// To Create Role And Adding Permission Provided By User
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> CreateRoleWithPermission(ClaimsHelperModel tokenData, Req.CreateRoleViewClassRequest model);
        /// <summary>
        /// Get Role And Permission By Role Id For Edit
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<ServiceResponse<Res.GetRoleAndPrmissionByIdResponse>> GetRoleAndPermissionById(ClaimsHelperModel tokenData, Guid roleId);
        /// <summary>
        /// To Update Role With Permission In Module Assess
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> UpdateRoleWithPermission(ClaimsHelperModel tokenData, Req.UpdateRoleViewClassRequest model);
    }
}
