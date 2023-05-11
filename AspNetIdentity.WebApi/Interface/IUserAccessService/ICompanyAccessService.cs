using AspNetIdentity.Core.Common;
using System.Threading.Tasks;
using Req = AspNetIdentity.Core.ViewModel.UserAccessViewModel.RequestCompanyAccess;
using Res = AspNetIdentity.Core.ViewModel.UserAccessViewModel.ResponseCompanyAccess;

namespace AspNetIdentity.WebApi.Interface.IUserAccessService
{
    /// <summary>
    /// Created By Harshit Mitra On 03-04-2023
    /// </summary>
    public interface ICompanyAccessService
    {
        Task<ServiceResponse<Res.GetCompanyAccessResponse>> GetModuleList(int companyId);
        Task<ServiceResponse<bool>> UpdateCompanyAccess(Req.UpdateCompanyAccessRequest model);
        Task<ServiceResponse<bool>> SetPermissionToAllCompanies();
    }
}
