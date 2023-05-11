using AspNetIdentity.Core.Enum;
using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure.ITsfService;
using AspNetIdentity.WebApi.Services.TsfService;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Req = AspNetIdentity.Core.ViewModel.TsfViewModel.RequestNewsServiceClass;

namespace AspNetIdentity.WebApi.Controllers.Tsf
{
    [Authorize]
    [RoutePrefix("api/newsservice")]
    public class NewsServiceController : ApiController
    {
        #region Properties
        private readonly INewsService _newsService;
        #endregion

        #region Constructor
        public NewsServiceController()
        {
            _newsService = new NewsService();
        }
        #endregion

        /// <summary>
        /// Created By Harshit Mitra On 25-04-2023
        /// API >> POST >> api/newsservice/createnews
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createnews")]
        public async Task<IHttpActionResult> CreateNews(Req.CreateNewsRequestClass model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _newsService.CreateNews(tokenData, model);
            return Ok(response);
        }

        /// <summary>
        /// Created By Harshit Mitra On 25-04-2023
        /// API >> GET >> api/newsservice/newslistadmin
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("newslistadmin")]
        public async Task<IHttpActionResult> GetNewsListForAdmin(Req.SearchStringRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            Req.GetNewsAdminViewRequest obj = new Req.GetNewsAdminViewRequest
            {
                NewsType = NewsEnumType.Publish,
                SearchString = model.SearchString,
            };
            obj.SearchString = model.SearchString ?? string.Empty;
            var response = await _newsService.GetAllNewsAdminView(tokenData, obj);
            return Ok(response);
        }

        /// <summary>
        /// Created By Harshit Mitra On 25-04-2023
        /// API >> GET >> api/newsservice/getnewsbyid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getnewsbyid")]
        public async Task<IHttpActionResult> GetNewsById(Guid newsId)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _newsService.GetById(newsId);
            return Ok(response);
        }

        /// <summary>
        /// Created By Harshit Mitra On 25-04-2023
        /// API >> POST >> api/newsservice/updatenews
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("updatenews")]
        public async Task<IHttpActionResult> UpdateNews(Req.UpdateNewsClassRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _newsService.UpdateNews(tokenData, model);
            return Ok(response);
        }

        /// <summary>
        /// Created By Harshit Mitra On 25-04-2023
        /// API >> POST >> api/newsservice/movetotrash
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("movetotrash")]
        public async Task<IHttpActionResult> MoveToTrash(Req.MoveToTrashOrDeleteRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _newsService.MoveToTrashNews(tokenData, model.NewsIds);
            return Ok(response);
        }


        /// <summary>
        /// Created By Harshit Mitra On 25-04-2023
        /// API >> POST >> api/newsservice/deletenews
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("deletenews")]
        public async Task<IHttpActionResult> DeleteNews(Req.MoveToTrashOrDeleteRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _newsService.DeleteNews(tokenData, model.NewsIds);
            return Ok(response);
        }

        /// <summary>
        /// Created By Harshit Mitra On 25-04-2023
        /// API >> POST >> api/newsservice/newslisttrash
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("newslisttrash")]
        public async Task<IHttpActionResult> GetNewsListInTrash(Req.SearchStringRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            Req.GetNewsAdminViewRequest obj = new Req.GetNewsAdminViewRequest
            {
                NewsType = NewsEnumType.Trash,
                SearchString = model.SearchString,
            };
            obj.SearchString = model.SearchString ?? string.Empty;
            var response = await _newsService.GetAllNewsAdminView(tokenData, obj);
            return Ok(response);
        }

        /// <summary>
        /// Created By Harshit Mitra On 25-04-2023
        /// API >> POST >> api/newsservice/draftnewslist
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("draftnewslist")]
        public async Task<IHttpActionResult> GetDraftNewsList(Req.SearchStringRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            Req.GetNewsAdminViewRequest obj = new Req.GetNewsAdminViewRequest
            {
                NewsType = NewsEnumType.Drafted,
                SearchString = model.SearchString,
            };
            obj.SearchString = model.SearchString ?? string.Empty;
            var response = await _newsService.GetAllNewsAdminView(tokenData, obj);
            return Ok(response);
        }

    }
}
