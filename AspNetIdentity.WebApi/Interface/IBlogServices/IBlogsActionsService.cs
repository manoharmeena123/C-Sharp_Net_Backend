using AspNetIdentity.Core.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Req = AspNetIdentity.Core.ViewModel.TsfViewModel.RequestBlogsActions;
using Res = AspNetIdentity.Core.ViewModel.TsfViewModel.ResponseBlogsActions;

namespace AspNetIdentity.WebApi.Interface.ITsfService
{
    public interface IBlogsActionsService
    {
        /// <summary>
        /// This Method Use To Like And Unlike Blogs With Refrences Of Blog Id
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="blogId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> LikeUnlineBlogs(ClaimsHelperModel tokenData, Guid blogId);
        /// <summary>
        /// Thi Method Is Used To Approved Or Reject Blogs 
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> ApprovedAndUnApprovedBlogs(ClaimsHelperModel tokenData, Req.BlogChangeStatusRequest model);
        /// <summary>
        /// This Method Is Used To Get Live Blogs List
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<Res.GetLiveBlogListResponse>>> GetLiveBlogsLists(ClaimsHelperModel tokenData);
        /// <summary>
        /// This Method Is Used TO GEt List Of Blogs Which are In Review
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<Res.GetBlogListInReview>>> GetInReviewBlogsLists(ClaimsHelperModel tokenData, Req.InReviewRequest model);
        /// <summary>
        /// This Method Is Used To Move Blogs To Trash By Prarams Blogs Ids
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="blogsIds"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> MoveBlogsToTrash(ClaimsHelperModel tokenData, params Guid[] blogsIds);
        /// <summary>
        /// This Method Is Used To Get Live Blog Data In Pagination, Short By And Category Filter
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ServiceResponse<PagingResponse<Res.GetLiveBlogListResponse>>> GetLiveBlogsListsPaging(ClaimsHelperModel tokenData, Req.BlogLiveViewRequest model);
        /// <summary>
        /// This Method Is Used To Approve And Reject Blogs In Multiple Check Request
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> ApproveAndRejectBlogMultiple(ClaimsHelperModel tokenData, Req.ApproveOrRejectMultipleRequest model);
    }
}
