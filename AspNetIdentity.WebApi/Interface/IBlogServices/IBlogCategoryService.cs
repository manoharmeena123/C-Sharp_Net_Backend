using AspNetIdentity.Core.Common;
using AspNetIdentity.Core.Model.TsfModule;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Req = AspNetIdentity.Core.ViewModel.BlogsViewModel.RequestBlogCategory;
using Res = AspNetIdentity.Core.ViewModel.BlogsViewModel.ResponseBlogCategory;

namespace AspNetIdentity.WebApi.Interface.ITsfService
{
    public interface IBlogCategoryService
    {
        /// <summary>
        /// This Method Is Used To Create Blog Category
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ServiceResponse<BlogCategories>> CreateBlogCategories(ClaimsHelperModel tokenData, Req.CreateBlogCategoryRequest model);
        /// <summary>
        /// This Method Is Used To Update Blog Category
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> UpdateCategories(ClaimsHelperModel tokenData, Req.UpdateBlogCategoryRequest model);
        /// <summary>
        /// This Method Is Used To Delete Category (Soft Delete)
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> DeleteCategories(ClaimsHelperModel tokenData, Guid categoryId);
        /// <summary>
        /// This Method Is Used To Get Blog Categories In List
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<Res.GetBlogCategoriesResponse>>> GetBlogCategoriesList(ClaimsHelperModel tokenData);
        /// <summary>
        /// This Method Is Used To Get Blog Category By Category Id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<ServiceResponse<Res.GetBlogCategoriesResponse>> GetBlogCategoryById(Guid categoryId);
        /// <summary>
        /// This Method Is Used To Get Category List With Count Of Total Blogs In Category (ONLY LIVE BLOGS)
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<Res.GetCategoryWithBlogsCountResponse>>> GetCategoryWithBlogCount(ClaimsHelperModel tokenData);
    }
}
