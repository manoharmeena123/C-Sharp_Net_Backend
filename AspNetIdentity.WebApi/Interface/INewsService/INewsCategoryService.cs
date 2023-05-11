using AspNetIdentity.Core.Common;
using AspNetIdentity.Core.Model.TsfModule.NewsEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Req = AspNetIdentity.Core.ViewModel.NewsViewModel.RequestNewsCategory;
using Res = AspNetIdentity.Core.ViewModel.NewsViewModel.ResponseNewsCategory;

namespace AspNetIdentity.WebApi.Interface.INewsService
{
    public interface INewsCategoryService
    {
        /// <summary>
        /// This Method Is Used To Create News Categories
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ServiceResponse<NewsCategoryEntity>> CreateNewsCategories(ClaimsHelperModel tokenData, Req.CreateNewCateoryRequest model);
        /// <summary>
        /// This Method Is Used To Update News Categories 
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> UpdateNewsCategories(ClaimsHelperModel tokenData, Req.UpdateNewCateoryRequest model);
        /// <summary>
        /// This Method Is Used To Move News Categories To The Trash 
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> MoveToTrashNewsCategories(ClaimsHelperModel tokenData, params Guid[] categoryId);
        /// <summary>
        /// This Method Is Used To Get News Category In List View
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<Res.GetNewsCategoryResponse>>> GetNewsCategoryList(ClaimsHelperModel tokenData);
        /// <summary>
        /// This Method Is Used To Get News Category By If For Edit
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<ServiceResponse<Res.GetNewsCategoryByIdResponse>> GetNewsCategoryById(Guid categoryId);
        /// <summary>
        /// This Method Is Used To Delete News Categories Which Are In Trash
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> DeleteNewsCategories(ClaimsHelperModel tokenData, params Guid[] categoryId);
    }
}
