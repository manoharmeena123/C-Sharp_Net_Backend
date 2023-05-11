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
    [RoutePrefix("api/circular")]
    public class CircularsController : ApiController
    {
        #region Properties
        private readonly ICircularsNoticesService _circular;
        #endregion

        #region Constructor
        public CircularsController()
        {
            _circular = new CircularsNoticesService();
        }
        #endregion

        /// <summary>
        /// Created By Harshit Mitra On 27-04-2023
        /// API >> POST >> api/circular/createcirculars
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createcirculars")]
        public async Task<IHttpActionResult> CreateCirculars(Req.CreateRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _circular.Create(tokenData, model, CircularsNoticesType.Circulars);
            return Ok(response);
        }


        /// <summary>
        /// Created By Harshit Mitra On 28-04-2023
        /// API >> POST >> api/circular/getcircularonadmin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getcircularonadmin")]
        public async Task<IHttpActionResult> GetCircularOnAdmin(Req.AdminRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _circular.GetOnAdminView(tokenData, model, CircularsNoticesType.Circulars);
            return Ok(response);
        }


        /// <summary>
        /// Created By Harshit Mitra On 28-04-2023
        /// API >> POST >> api/circular/multiselectcirculardelete
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("multiselectcirculardelete")]
        public async Task<IHttpActionResult> MultiSelectCircularDelete(Req.MultiSelectSelectRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _circular.MultiSelectDelete(tokenData, model, CircularsNoticesType.Circulars);
            return Ok(response);
        }


        /// <summary>
        /// Created By Harshit Mitra On 28-04-2023
        /// API >> POST >> api/circular/getcircularbyid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getcircularbyid")]
        public async Task<IHttpActionResult> GetCircularById(Req.GetByIdRequest model)
        {
            var response = await _circular.GetDataById(model, CircularsNoticesType.Circulars);
            return Ok(response);
        }


        /// <summary>
        /// Created By Harshit Mitra On 28-04-2023
        /// API >> POST >> api/circular/updatecircular
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updatecircular")]
        public async Task<IHttpActionResult> UpdateCircular(Req.UpdateRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _circular.Update(tokenData, model, CircularsNoticesType.Circulars);
            return Ok(response);
        }
    }
}
