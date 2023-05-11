using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Interface.SubProject;
using AspNetIdentity.WebApi.Services.SubProject;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Req = AspNetIdentity.Core.ViewModel.SubProjectViewModel.RequestProjectViewModel;
namespace AspNetIdentity.WebApi.Controllers.NewTimeSheet
{
    [Authorize]
    [RoutePrefix("api/newsubproject")]
    public class NewSubProjectController : ApiController
    {
        #region Properties
        private readonly ISubProjectService _subProject;
        #endregion

        #region Constructor
        public NewSubProjectController()
        {
            _subProject = new SubProjectService();
        }
        #endregion

        #region Methods

        #region Api for Get Project And Subroject
        /// <summary>
        /// Create By Ravi Vyas On 03/04/2023
        /// Api >> Get >> api/newsubproject/getprojectsubprojectdata
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getprojectsubprojectdata")]
        public async Task<IHttpActionResult> GetProjectAndSubProject(int projectId)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _subProject.GetProjectAndSubProject(tokenData, projectId);
            return Ok(response);
        }
        #endregion

        #region Api for Get All Assign Project Or SubProject of Login Employee
        /// <summary>
        /// Create By Ravi Vyas On 05/04/2023
        /// Api >> Get >> api/newsubproject/getassignprojectsubproject
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getassignprojectsubproject")]
        public async Task<IHttpActionResult> GetProjectSubProjectDashboard()
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _subProject.GetProjectSubProjectDashboard(tokenData);
            return Ok(response);
        }

        #endregion

        #region Api for Get All Project For Approvel
        /// <summary>
        /// Created By Ravi Vyas on 07-04-2023
        /// API >> GET >> api/newsubproject/getallassignprojectforapprovel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallassignprojectforapprovel")]
        public async Task<IHttpActionResult> GetAllProjectForApprovel()
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _subProject.GetAllProjectForApproval(tokenData);
            return Ok(response);
        }

        #endregion

        #region Api for Add SubProject in MainProject
        /// <summary>
        /// Created By Ravi Vyas on 10-04-2023
        /// API >> GET >> api/newsubproject/updateprojectinsubproject
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("updateprojectinsubproject")]
        public async Task<IHttpActionResult> UpdateProjectInSubProject(Req.RequestForUpdateProjectInMainProject model)
        {

            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _subProject.UpdateProjectInSubProject(tokenData, model);
            return Ok(response);
        }
        #endregion

        #endregion Methods

    }
}
