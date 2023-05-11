using AspNetIdentity.Core.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Req = AspNetIdentity.Core.ViewModel.SubProjectViewModel.RequestProjectViewModel;
using Res = AspNetIdentity.Core.ViewModel.SubProjectViewModel.ResponseProjectViewModel;

namespace AspNetIdentity.WebApi.Interface.SubProject
{
    /// <summary>
    /// Created By Ravi Vyas On 03-04-2023
    /// </summary>
    public interface ISubProjectService
    {
        Task<ServiceResponse<List<Res.SubModelResponse>>> GetProjectAndSubProject(ClaimsHelperModel tokenData, int projectId);
        Task<ServiceResponse<List<Res.ResponseProjectSubProject>>> GetProjectSubProjectDashboard(ClaimsHelperModel tokenData);
        Task<ServiceResponse<List<Res.ResponseProjectListForApproval>>> GetAllProjectForApproval(ClaimsHelperModel tokenData);
        Task<ServiceResponse<Req.RequestForUpdateProjectInMainProject>> UpdateProjectInSubProject(ClaimsHelperModel tokenData, Req.RequestForUpdateProjectInMainProject model);
    }
}
