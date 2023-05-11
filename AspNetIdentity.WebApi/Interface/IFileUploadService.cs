using System.Net.Http;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using static AspNetIdentity.WebApi.Services.FileUploadService;

namespace AspNetIdentity.WebApi.Interface
{
    public interface IFileUploadService
    {
        /// <summary>
        /// This Method Is Used To Upload File
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="request"></param>
        /// <param name="urlPath"></param>
        /// <returns></returns>
        Task<UploadResponse> UploadFile(ClaimsHelperModel tokenData, HttpRequestMessage request, string urlPath);
    }
}
