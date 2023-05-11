using AspNetIdentity.Core.Common;
using AspNetIdentity.Core.Model.TsfModule;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Interface.ITsfService;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Req = AspNetIdentity.Core.ViewModel.BlogsViewModel.RequestBlogCategory;
using Res = AspNetIdentity.Core.ViewModel.BlogsViewModel.ResponseBlogCategory;

namespace AspNetIdentity.WebApi.Services.TsfService
{
    public class BlogCategoryService : IBlogCategoryService
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        #endregion

        #region Constructor
        public BlogCategoryService()
        {
            _context = new ApplicationDbContext();
        }
        #endregion

        /// <summary>
        /// Created By Harshit Mitra On 17-04-2023
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<BlogCategories>> CreateBlogCategories(ClaimsHelperModel tokenData, Req.CreateBlogCategoryRequest model)
        {
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.Now, tokenData.TimeZone);
            BlogCategories obj = new BlogCategories
            {
                CategoryName = model.CategoryName,
                Description = model.Description,
                CreatedBy = tokenData.employeeId,
                CreatedOn = today,
                CompanyId = tokenData.companyId,
            };
            _context.BlogCategories.Add(obj);
            await _context.SaveChangesAsync();
            return new ServiceResponse<BlogCategories>(HttpStatusCode.Created, obj);
        }
        /// <summary>
        /// Created By Harshit Mitra On 17-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> UpdateCategories(ClaimsHelperModel tokenData, Req.UpdateBlogCategoryRequest model)
        {
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.Now, tokenData.TimeZone);
            BlogCategories update = await _context.BlogCategories
                .FirstOrDefaultAsync(x => x.CategoryId == model.CategoryId);
            if (update == null)
                return new ServiceResponse<bool>(HttpStatusCode.NotFound, false, false);
            update.CategoryName = model.CategoryName;
            update.Description = model.Description;
            update.UpdatedOn = today;
            update.UpdatedBy = tokenData.employeeId;
            _context.Entry(update).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(HttpStatusCode.Accepted, true);
        }
        /// <summary>
        /// Created By Harshit Mitra On 17-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> DeleteCategories(ClaimsHelperModel tokenData, Guid categoryId)
        {
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.Now, tokenData.TimeZone);
            BlogCategories update = await _context.BlogCategories
                .FirstOrDefaultAsync(x => x.CategoryId == categoryId);
            if (update == null)
                return new ServiceResponse<bool>(HttpStatusCode.NotFound, false, false);
            update.DeletedOn = today;
            update.DeletedBy = tokenData.employeeId;
            update.IsDeleted = true;
            update.IsActive = false;
            _context.Entry(update).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(HttpStatusCode.Moved, true, true, "Removed");
        }
        /// <summary>
        /// Created By Harshit Mitra On 17-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<Res.GetBlogCategoriesResponse>>> GetBlogCategoriesList(ClaimsHelperModel tokenData)
        {
            List<Res.GetBlogCategoriesResponse> response =
                await _context.BlogCategories
                .Where(x => x.CompanyId == tokenData.companyId && x.IsActive && !x.IsDeleted)
                .Select(x => new Res.GetBlogCategoriesResponse
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                    Description = x.Description,
                })
                .ToListAsync();
            if (response.Count != 0)
                return new ServiceResponse<List<Res.GetBlogCategoriesResponse>>(HttpStatusCode.OK, response);
            return new ServiceResponse<List<Res.GetBlogCategoriesResponse>>(HttpStatusCode.NoContent, response, false);
        }
        /// <summary>
        /// Created By Harshit Mitra On 17-04-2023
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<Res.GetBlogCategoriesResponse>> GetBlogCategoryById(Guid categoryId)
        {
            Res.GetBlogCategoriesResponse blogCategory =
                await _context.BlogCategories
                .Where(x => x.CategoryId == categoryId)
                .Select(x => new Res.GetBlogCategoriesResponse
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                    Description = x.Description,
                })
                .FirstOrDefaultAsync();
            if (blogCategory != null)
                return new ServiceResponse<Res.GetBlogCategoriesResponse>(HttpStatusCode.OK, blogCategory);
            return new ServiceResponse<Res.GetBlogCategoriesResponse>(HttpStatusCode.NotFound, blogCategory, false);
        }
        /// <summary>
        /// Created By Harshit Mitra On 19-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<Res.GetCategoryWithBlogsCountResponse>>> GetCategoryWithBlogCount(ClaimsHelperModel tokenData)
        {
            List<Res.GetCategoryWithBlogsCountResponse> response =
                new List<Res.GetCategoryWithBlogsCountResponse>()
                {
                    new Res.GetCategoryWithBlogsCountResponse()
                    {
                        CategoryId = Guid.Empty,
                        CategoryName = "All",
                        Count = 0
                    },
                };

            var blogsList = await (from bc in _context.BlogCategories
                                   join bl in _context.Blogs
                                        .Where(x => x.IsActive && !x.IsDeleted && x.BlogsStatus == Core.Enum.BlogsTypeEnumClass.Live)
                                        .AsEnumerable()
                                   on bc.CategoryId equals bl.CategoryId
                                   into emptyBlog
                                   from result in emptyBlog.DefaultIfEmpty()
                                   where bc.CompanyId == tokenData.companyId && bc.IsActive && !bc.IsDeleted
                                   select new
                                   {
                                       bc,
                                       result,
                                   })
                                   .GroupBy(x => x.bc.CategoryId)
                                   .Select(x => new Res.GetCategoryWithBlogsCountResponse
                                   {
                                       CategoryId = x.Key,
                                       CategoryName = x.FirstOrDefault().bc.CategoryName,
                                       Count = x.Where(z=> z.result != null).Select(z=> z.result).ToList().Count(),
                                   })
                                   .OrderBy(x => x.CategoryName)
                                   .ToListAsync();
            response[0].Count = blogsList.Sum(x => x.Count);
            response.AddRange(blogsList);
            return new ServiceResponse<List<Res.GetCategoryWithBlogsCountResponse>>(HttpStatusCode.OK, response);
        }
    }
}