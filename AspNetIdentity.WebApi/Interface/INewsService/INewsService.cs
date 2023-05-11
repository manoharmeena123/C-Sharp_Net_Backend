using AspNetIdentity.Core.Common;
using AspNetIdentity.Core.Model.TsfModule;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Req = AspNetIdentity.Core.ViewModel.TsfViewModel.RequestNewsServiceClass;
using Res = AspNetIdentity.Core.ViewModel.TsfViewModel.ResponseNewsServiceClass;

namespace AspNetIdentity.WebApi.Infrastructure.ITsfService
{
    public interface INewsService
    {
        /// <summary>
        /// This Method Is Used To Create News 
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ServiceResponse<NewsEntity>> CreateNews(ClaimsHelperModel tokenData, Req.CreateNewsRequestClass model);
        /// <summary>
        /// This Method Is Used To Get All News For Admin ANd In Trash And Filter By News Type 
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<Res.GetNewsAdminResponse>>> GetAllNewsAdminView(ClaimsHelperModel tokenData, Req.GetNewsAdminViewRequest model);
        /// <summary>
        /// This Method Is To Used To Get News By Id
        /// </summary>
        /// <param name="newsId"></param>
        /// <returns></returns>
        Task<ServiceResponse<NewsEntity>> GetById(Guid newsId);
        /// <summary>
        /// This Method Is To Used To Update News
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> UpdateNews(ClaimsHelperModel tokenData, Req.UpdateNewsClassRequest model);
        /// <summary>
        /// This Method Is Used To Move News To Trash
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="newsIds"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> MoveToTrashNews(ClaimsHelperModel tokenData, Guid[] newsIds);
        /// <summary>
        /// This Method Is Used To Delete News From Trash
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="newsIds"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> DeleteNews(ClaimsHelperModel tokenData, Guid[] newsIds);
    }
}
