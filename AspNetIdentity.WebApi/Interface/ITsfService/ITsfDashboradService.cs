using AspNetIdentity.Core.Common;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Res = AspNetIdentity.Core.ViewModel.TsfViewModel.ReponseTsfDashboradClass;

namespace AspNetIdentity.WebApi.Infrastructure.ITsfService
{
    public interface ITsfDashboradService
    {
        Task<ServiceResponse<Res.ResponseTsfDashboradClass>> GetDasboradData(ClaimsHelperModel tokenData);
    }
}
