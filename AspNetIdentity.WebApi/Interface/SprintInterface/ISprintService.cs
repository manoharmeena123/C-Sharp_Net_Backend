using AspNetIdentity.Core.Common;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Req = AspNetIdentity.Core.ViewModel.SprintViewModel.RequestSprintViewModel;
namespace AspNetIdentity.WebApi.Interface.SprintInterface
{
    /// <summary>
    /// Created By Ravi Vyas on 06-04-2023
    /// </summary>
    public interface ISprintService
    {
        Task<ServiceResponse<Req.RequestCreateSprint>> CreateSprint(Req.RequestCreateSprint model, ClaimsHelperModel tokenData);
    }
}
