using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Interface.INewsService;
using AspNetIdentity.WebApi.Services.NewsService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Req = AspNetIdentity.Core.ViewModel.NewsViewModel.RequestNewsCategory;

namespace AspNetIdentity.WebApi.Controllers.TsfControllers.NewsControllers
{
    /// <summary>
    /// Created By Harshit Mitra On 24-04-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/newscategory")]
    public class NewsCategoryController : ApiController
    {
        #region Properties
        private readonly INewsCategoryService _newsCategoryService;
        #endregion

        #region Constructor
        public NewsCategoryController()
        {
            _newsCategoryService = new NewsCategoryService();
        }
        #endregion

        /// <summary>
        /// Created By Harshit Mitra On 24-04-2023
        /// API >> POST >> api/newscategory/createnewscategories
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createnewscategories")]
        public async Task<IHttpActionResult> CreateNewsCategories(Req.CreateNewCateoryRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _newsCategoryService.CreateNewsCategories(tokenData, model);   
            return Ok(response);
        }

        /// <summary>
        /// Created By Harshit Mitra On 24-04-2023
        /// API >> POST >> api/newscategory/updatenewscategories
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updatenewscategories")]
        public async Task<IHttpActionResult> UpdateNewsCategories(Req.UpdateNewCateoryRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _newsCategoryService.UpdateNewsCategories(tokenData, model);
            return Ok(response);
        }

        /// <summary>
        /// Created By Harshit Mitra On 24-04-2023
        /// API >> POST >> api/newscategory/deletenewscategories
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deletenewscategories")]
        public async Task<IHttpActionResult> DeleteNewsCategories(Req.MoveToTrashDeleteReQuest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _newsCategoryService.DeleteNewsCategories(tokenData, model.CategoriesIds);
            return Ok(response);
        }

        /// <summary>
        /// Created By Harshit Mitra On 24-04-2023
        /// API >> GET >> api/newscategory/getnewscategorylist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getnewscategorylist")]
        public async Task<IHttpActionResult> GetNewsCategoryList()
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _newsCategoryService.GetNewsCategoryList(tokenData);
            return Ok(response);
        }


        /// <summary>
        /// Created By Harshit Mitra On 24-04-2023
        /// API >> GET >> api/newscategory/getnewscategorybyid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getnewscategorybyid")]
        public async Task<IHttpActionResult> GetNewsCategoryById(Guid categoryId)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var response = await _newsCategoryService.GetNewsCategoryById(categoryId);
            return Ok(response);
        }
    }
}
