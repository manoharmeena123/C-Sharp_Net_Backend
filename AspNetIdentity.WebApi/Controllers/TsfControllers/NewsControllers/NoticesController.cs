using AspNetIdentity.Core.Model.TsfModule.CircularsNotices;
using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Interface.INewsService;
using AspNetIdentity.WebApi.Services.CircularNoticesService;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Req = AspNetIdentity.Core.ViewModel.CircularNoticeViewModel.RequestCircularNoticeViewModel;

namespace AspNetIdentity.WebApi.Controllers.TsfControllers.NewsControllers
{
    /// <summary>
    /// Created By Harshit Mitra On 27-04-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/notice")]
    public class NoticesController : ApiController
    {
        #region Properties
        private readonly ICircularsNoticesService _notice;
        #endregion

        #region Constructor
        public NoticesController()
        {
            _notice = new CircularsNoticesService();
        }
        #endregion


        /// <summary>
        /// Created By Harshit Mitra On 27-04-2023
        /// API >> POST >> api/notice/createnotices
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createnotices")]
        public async Task<IHttpActionResult> CreateNotices(Req.CreateRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _notice.Create(tokenData, model, CircularsNoticesType.Notices);
            return Ok(response);
        }


        // <summary>
        /// Created By Harshit Mitra On 28-04-2023
        /// API >> POST >> api/notice/getnoticesonadmin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getnoticesonadmin")]
        public async Task<IHttpActionResult> GetNoticesOnAdmin(Req.AdminRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _notice.GetOnAdminView(tokenData, model, CircularsNoticesType.Notices);
            return Ok(response);
        }


        /// <summary>
        /// Created By Harshit Mitra On 28-04-2023
        /// API >> POST >> api/notice/multiselectnoticesdelete
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("multiselectnoticesdelete")]
        public async Task<IHttpActionResult> MultiSelectNoticesDelete(Req.MultiSelectSelectRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _notice.MultiSelectDelete(tokenData, model, CircularsNoticesType.Notices);
            return Ok(response);
        }


        /// <summary>
        /// Created By Harshit Mitra On 28-04-2023
        /// API >> POST >> api/notice/getnoticesbyid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getnoticesbyid")]
        public async Task<IHttpActionResult> GetNoticesById(Req.GetByIdRequest model)
        {
            var response = await _notice.GetDataById(model, CircularsNoticesType.Notices);
            return Ok(response);
        }


        /// <summary>
        /// Created By Harshit Mitra On 28-04-2023
        /// API >> POST >> api/notice/updatenotices
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updatenotices")]
        public async Task<IHttpActionResult> UpdateNotices(Req.UpdateRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _notice.Update(tokenData, model, CircularsNoticesType.Notices);
            return Ok(response);
        }
    }
}
