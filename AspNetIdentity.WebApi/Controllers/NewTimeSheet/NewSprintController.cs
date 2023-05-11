using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Interface.SprintInterface;
using AspNetIdentity.WebApi.Services.SprintService;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Req = AspNetIdentity.Core.ViewModel.SprintViewModel.RequestSprintViewModel;
namespace AspNetIdentity.WebApi.Controllers.NewTimeSheet
{
    /// <summary>
    /// Creted By Ravi Vyas On 06-04-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/newsprint")]
    public class NewSprintController : ApiController
    {
        #region Properties
        private readonly ISprintService _sprintService;
        #endregion

        #region Constructor
        public NewSprintController()
        {
            _sprintService = new SprintService();
        }
        #endregion

        #region Methods

        #region Api for Create Sprint For Project
        /// <summary>
        /// Created By Ravi Vyas on  06-04-2023
        /// API >> POST >> api/newsprint/newsprintcreate
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("newsprintcreate")]
        public async Task<IHttpActionResult> CreateSprint(Req.RequestCreateSprint model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                  .SelectMany(v => v.Errors)
                  .Select(e => e.ErrorMessage));
                return BadRequest(message);
            }
            var response = await _sprintService.CreateSprint(model, tokenData);
            return Ok(response);
        }

        #endregion

        #region Api for Get All Sprint 

        //public async Task<IHttpActionResult> GetAllNewSprint()
        //{

        //}

        #endregion

        #endregion Methods

    }
}
