using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Interface.ITsfService;
using AspNetIdentity.WebApi.Services.TsfService;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Req = AspNetIdentity.Core.ViewModel.TsfViewModel.RequestBlogsActions;

namespace AspNetIdentity.WebApi.Controllers.Tsf
{
    /// <summary>
    /// Created By Harshit Mitra On 12-04-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/blogactions")]
    public class BlogsActionController : ApiController
    {
        #region Properties
        private readonly IBlogsActionsService _blogsActions;
        #endregion

        #region Constructor 
        public BlogsActionController()
        {
            _blogsActions = new BlogsActionsService();
        }
        #endregion

        /// <summary>
        /// Created By Harshit Mitra On 12-04-2023
        /// API >> POST >> api/blogactions/likeunlike
        /// </summary>
        /// <param name="blogId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("likeunlike")]
        public async Task<IHttpActionResult> BlogLikeAndUnLile(Guid blogId)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _blogsActions.LikeUnlineBlogs(tokenData, blogId);
            return Ok(response);
        }

        /// <summary>
        /// Created By Harshit Mitra On 14-04-2023
        /// API >> POST >> api/blogactions/approveorrejectblog
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("approveorrejectblog")]
        public async Task<IHttpActionResult> ApproveOrRejectBlog(Req.BlogChangeStatusRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _blogsActions.ApprovedAndUnApprovedBlogs(tokenData, model);
            return Ok(response);
        }

        /// <summary>
        /// Created By Harshit Mitra ON 14-04-2023 
        /// API >> GET >> api/blogactions/livebloglist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("livebloglist")]
        public async Task<IHttpActionResult> LiveBlogList()
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _blogsActions.GetLiveBlogsLists(tokenData);
            return Ok(response);
        }

        /// <summary>
        /// Created By Harshit Mitra ON 14-04-2023 
        /// API >> GET >> api/blogactions/inreviewblogslist
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("inreviewblogslist")]
        public async Task<IHttpActionResult> InReviewBlogsList(Req.InReviewRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _blogsActions.GetInReviewBlogsLists(tokenData, model);
            return Ok(response);
        }

        /// <summary>
        /// Created By Harshit Mitra On 17-04-2023
        /// API >> POST >> api/blogactions/moveblogtotrash
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("moveblogtotrash")]
        public async Task<IHttpActionResult> MoveBlogsToTrash(Req.MoveBlogToTrashRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _blogsActions.MoveBlogsToTrash(tokenData, model.BlogsIds);
            return Ok(response);
        }

        /// <summary>
        /// Created By Harshit Mitra On 19-04-2023
        /// API >> POST >> api/blogactions/livebloglistfilter
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("livebloglistfilter")]
        public async Task<IHttpActionResult> LiveBlogListFilter(Req.BlogLiveViewRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _blogsActions.GetLiveBlogsListsPaging(tokenData, model);
            return Ok(response);
        }

        /// <summary>
        /// Created By Harshit Mitra On 26-04-2023
        /// API >> POST >> api/blogactions/approverejectmultiple
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("approverejectmultiple")]
        public async Task<IHttpActionResult> ApproveRejectMultiple(Req.ApproveOrRejectMultipleRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _blogsActions.ApproveAndRejectBlogMultiple(tokenData, model);
            return Ok(response);
        }
    }
}
