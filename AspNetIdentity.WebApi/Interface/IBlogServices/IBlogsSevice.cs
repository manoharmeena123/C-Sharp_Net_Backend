using AspNetIdentity.Core.Common;
using AspNetIdentity.Core.Enum;
using AspNetIdentity.Core.Model.TsfModule;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Req = AspNetIdentity.Core.ViewModel.TsfViewModel.BlogRequestClass;
using Res = AspNetIdentity.Core.ViewModel.TsfViewModel.BlogResponseClass;

namespace AspNetIdentity.WebApi.Interface.ITsfService
{
    public interface IBlogsService
    {
        Task<ServiceResponse<Blogs>> AddBlogsData(ClaimsHelperModel tokenData, Req.CreateBlogRequestclass model);
        Task<ServiceResponse<bool>> UpdateBlogsData(ClaimsHelperModel tokenData, Req.UpdateBlogRequestclass model);
        Task<ServiceResponse<Res.GetBlogResponseByIdClass>> GetBlogsById(ClaimsHelperModel tokenData, Guid blogsId);
        Task<ServiceResponse<List<Res.GetBlogListPagingResponse>>> GetBlogsData(ClaimsHelperModel tokenData, Req.GetRequestClass model);
        Task<ServiceResponse<bool>> DeleteBlog(ClaimsHelperModel tokenData, params Guid[] blogsIds);
        Task<ServiceResponse<List<Res.GetBlogResponseByIdClass>>> GetBlogByFilter(ClaimsHelperModel tokenData, ShortByEnumClass? shortRequest);
        ServiceResponse<List<Res.BlogsResponseEnumData>> GetBlogsEnumType(ClaimsHelperModel tokenData);
        Task<ServiceResponse<List<Res.GetBlogHeaderResponse>>> GetBlogHeaderWithCount(ClaimsHelperModel tokenData);
        /// <summary>
        /// This Method Is Used Get Blog Details Preview Page For Approve Or Reject Blog
        /// </summary>
        /// <param name="blogId"></param>
        /// <returns></returns>
        Task<ServiceResponse<Res.PreviewBlogResponse>> GetPriviewBlogForReview(Guid blogId);
        /// <summary>
        /// This Method Is Used To Get Latest 3 Blogs Created By Loged In User
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<Res.GetMyBlogListResponse>>> MyLatestTopBlogs(ClaimsHelperModel tokenData);
    }
}
