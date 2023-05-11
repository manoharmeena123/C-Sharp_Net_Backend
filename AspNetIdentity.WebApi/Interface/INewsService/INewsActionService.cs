using AspNetIdentity.Core.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Req = AspNetIdentity.Core.ViewModel.NewsViewModel.NewsActionViewRequest;
using Res = AspNetIdentity.Core.ViewModel.NewsViewModel.NewsActionViewResponse;

namespace AspNetIdentity.WebApi.Interface.INewsService
{
    public interface INewsActionService
    {
        /// <summary>
        /// This Method Is Used To Get News Category Name With Count Of News Which Are Live/Published
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<Res.GetNewsCategoriesCountResponse>>> GetNewsCategoryCount(ClaimsHelperModel tokenData);
        /// <summary>
        /// This Method Is Used To Get News List On User View With Category, Short By , Pagination And Search Filter 
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ServiceResponse<PagingResponse<Res.GetPublishNewsResponse>>> GetPublishNewsUserView(ClaimsHelperModel tokenData, Req.PublishNewsFilterRequest model);
        /// <summary>
        /// This Method Is Used To Get Top News List In On User Which Are Live/Published
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        Task<ServiceResponse<Res.GetPublishNewsResponse>> GetTopNewsOnUserView(ClaimsHelperModel tokenData, Req.GetTopNewsRequest model);
        /// <summary>
        /// This Method Is Used To Get News On Behalf Of News Id
        /// </summary>
        /// <param name="newsId"></param>
        /// <returns></returns>
        Task<ServiceResponse<Res.GetPublishNewsResponse>> GetNewsDetailsById(Guid newsId);
        /// <summary>
        /// This Method Is Used To Like And Un Like News By News Id
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> LikeUnlikeNews(ClaimsHelperModel tokenData, Guid newsId);
        /// <summary>
        /// This Method Is Used To Set News As Featured News
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> SetNewsAsFeature(ClaimsHelperModel tokenData, Guid newsId);
    }
}
