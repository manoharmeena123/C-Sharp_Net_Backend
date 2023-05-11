using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Interface.INewsService;
using AspNetIdentity.WebApi.Services.NewsService;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Req = AspNetIdentity.Core.ViewModel.NewsViewModel.NewsActionViewRequest;

namespace AspNetIdentity.WebApi.Controllers.TsfControllers.NewsControllers
{
    /// <summary>
    /// Created By Harshit Mitra On 26-04-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/newsaction")]
    public class NewsActionServiceController : ApiController
    {
        #region Properties
        private readonly INewsActionService _newsActionService;
        #endregion

        #region Constructor
        public NewsActionServiceController()
        {
            _newsActionService = new NewsActionService();
        }
        #endregion


        /// <summary>
        /// Created By Harshit Mitra On 26-04-2023
        /// API >> GET >> api/newsaction/getnewscategorycount
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getnewscategorycount")]
        public async Task<IHttpActionResult> GetNewsCategoryCount()
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _newsActionService.GetNewsCategoryCount(tokenData);
            return Ok(response);
        }


        /// <summary>
        /// Created By Harshit Mitra On 26-04-2023
        /// API >> Post >> api/newsaction/getpublishnewsuserview
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getpublishnewsuserview")]
        public async Task<IHttpActionResult> GetPublishNewsUserView(Req.PublishNewsFilterRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _newsActionService.GetPublishNewsUserView(tokenData, model);
            return Ok(response);
        }


        /// <summary>
        /// Created By Harshit Mitra On 26-04-2023
        /// API >> Post >> api/newsaction/gettopnewsonuserview
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("gettopnewsonuserview")]
        public async Task<IHttpActionResult> GetTopNewsOnUserView(Req.GetTopNewsRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _newsActionService.GetTopNewsOnUserView(tokenData, model);
            return Ok(response);
        }


        /// <summary>
        /// Created By Harshit Mitra On 27-04-2023
        /// API >> GET >> api/newsaction/getnewsdetailsbyid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getnewsdetailsbyid")]
        public async Task<IHttpActionResult> GetNewsDetailsById(Guid newsId)
        {
            var response = await _newsActionService.GetNewsDetailsById(newsId);
            return Ok(response);
        }


        /// <summary>
        /// Created By Harshit Mitra On 27-04-2023
        /// API >> Post >> api/newsaction/likeunlikenews
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("likeunlikenews")]
        public async Task<IHttpActionResult> LikeUnlikeNews(Guid newsId)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _newsActionService.LikeUnlikeNews(tokenData, newsId);
            return Ok(response);
        }


        /// <summary>
        /// Created By Harshit Mitra On 27-04-2023
        /// API >> Post >> api/newsaction/setnewsasfeature
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("setnewsasfeature")]
        public async Task<IHttpActionResult> SetNewsAsFeature(Req.ByIdClassRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _newsActionService.SetNewsAsFeature(tokenData, model.NewsId);
            return Ok(response);
        }
    }
}
