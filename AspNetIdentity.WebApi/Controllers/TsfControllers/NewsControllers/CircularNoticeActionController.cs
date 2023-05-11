using AspNetIdentity.Core.Common;
using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Interface;
using AspNetIdentity.WebApi.Interface.INewsService;
using AspNetIdentity.WebApi.Services;
using AspNetIdentity.WebApi.Services.CircularNoticesService;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Req = AspNetIdentity.Core.ViewModel.CircularNoticeViewModel.RequestCircularNoticeViewModel;

namespace AspNetIdentity.WebApi.Controllers.TsfControllers.NewsControllers
{
    /// <summary>
    /// Created By Harshit Mitra On 28-04-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/circularnoticeaction")]
    public class CircularNoticeActionController : ApiController
    {
        #region Properties
        private readonly ICircularsNoticesService _service;
        private readonly IFileUploadService _uploadService;

        #endregion

        #region Constructor
        public CircularNoticeActionController()
        {
            _service = new CircularsNoticesService();
            _uploadService = new FileUploadService();
        }
        #endregion


        /// <summary>
        /// Created By Harshit Mitra On 28-04-2023
        /// API >> POST >> api/circularnoticeaction/uploadcirculars
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadcirculars")]
        public async Task<IHttpActionResult> UploadCirculars()
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _uploadService.UploadFile(tokenData, Request, "CircularImgDoc");
            return Ok(response);
        }


        /// <summary>
        /// Created By Harshit Mitra On 28-04-2023
        /// API >> POST >> api/circularnoticeaction/uploadnotices
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadnotices")]
        public async Task<IHttpActionResult> UploadNotices()
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _uploadService.UploadFile(tokenData, Request, "NoticeImgDoc");
            return Ok(response);
        }


        /// <summary>
        /// Created By Harshit Mitra On 28-04-2023
        /// API >> POST >> api/circularnoticeaction/getcountonuser
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getcountonuser")]
        public async Task<IHttpActionResult> GetCountOnUser(RequestShortBy model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _service.GetCountOnUser(tokenData, model);
            return Ok(response);
        }


        /// <summary>
        /// Created By Harshit Mitra On 28-04-2023
        /// API >> POST >> api/circularnoticeaction/getuserview
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getuserview")]
        public async Task<IHttpActionResult> GetUserView(Req.UserRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _service.GetUserView(tokenData, model);
            return Ok(response);
        }
    }
}
