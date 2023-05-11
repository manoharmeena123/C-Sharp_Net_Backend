using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Interface.ITsfService;
using AspNetIdentity.WebApi.Services.TsfService;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Req = AspNetIdentity.Core.ViewModel.BlogsViewModel.RequestBlogCategory;

namespace AspNetIdentity.WebApi.Controllers.Tsf
{
    /// <summary>
    /// Created By Harshit Mitra On 17-04-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/blogcategory")]
    public class BlogCategoryController : ApiController
    {
        #region Properties
        private readonly IBlogCategoryService _blogCategory;
        #endregion

        #region Constructor
        public BlogCategoryController()
        {
            _blogCategory = new BlogCategoryService();
        }
        #endregion

        /// <summary>
        /// Created By Harshit Mitra On 17-04-2023
        /// API >> POST >> api/blogcategory/createcategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createcategory")]
        public async Task<IHttpActionResult> CreateCategory(Req.CreateBlogCategoryRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var respone = await _blogCategory.CreateBlogCategories(tokenData, model);
            return Ok(respone);
        }

        /// <summary>
        /// Created By Harshit Mitra On 17-04-2023
        /// API >> POST >> api/blogcategory/updatecategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updatecategory")]
        public async Task<IHttpActionResult> UpdateCategory(Req.UpdateBlogCategoryRequest model)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var respone = await _blogCategory.UpdateCategories(tokenData, model);
            return Ok(respone);
        }

        /// <summary>
        /// Created By Harshit Mitra On 17-04-2023
        /// API >> POST >> api/blogcategory/deletecategory
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deletecategory")]
        public async Task<IHttpActionResult> DeleteCategory(Guid categoryId)
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var respone = await _blogCategory.DeleteCategories(tokenData, categoryId);
            return Ok(respone);
        }

        /// <summary>
        /// Created By Harshit Mitra On 17-04-2023
        /// API >> GET >> api/blogcategory/getallcategory
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallcategory")]
        public async Task<IHttpActionResult> GetAllCategory()
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var respone = await _blogCategory.GetBlogCategoriesList(tokenData);
            return Ok(respone);
        }

        /// <summary>
        /// Created By Harshit Mitra On 17-04-2023
        /// API >> GET >> api/blogcategory/getcategorybyid
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcategorybyid")]
        public async Task<IHttpActionResult> GetCategoryById(Guid categoryId)
        {
            var respone = await _blogCategory.GetBlogCategoryById(categoryId);
            return Ok(respone);
        }

        /// <summary>
        /// Created By Harshit Mitra On 19-04-2023
        /// API >> GET >> api/blogcategory/getcategorywithblogcount
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getcategorywithblogcount")]
        public async Task<IHttpActionResult> GetCategoryWithBlogCount()
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            var respone = await _blogCategory.GetCategoryWithBlogCount(tokenData);
            return Ok(respone);
        }
    }
}
