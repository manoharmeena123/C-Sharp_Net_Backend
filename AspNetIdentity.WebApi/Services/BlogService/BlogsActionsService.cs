using AspNetIdentity.Core.Common;
using AspNetIdentity.Core.Enum;
using AspNetIdentity.Core.Model.TsfModule;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Interface;
using AspNetIdentity.WebApi.Interface.ITsfService;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Req = AspNetIdentity.Core.ViewModel.TsfViewModel.RequestBlogsActions;
using Res = AspNetIdentity.Core.ViewModel.TsfViewModel.ResponseBlogsActions;

namespace AspNetIdentity.WebApi.Services.TsfService
{
    public class BlogsActionsService : IBlogsActionsService
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        private readonly IShortByService<Blogs> _blogShortBy;
        #endregion

        #region Constructor
        public BlogsActionsService()
        {
            _context = new ApplicationDbContext();
            _blogShortBy = new ShortByService<Blogs>();
        }
        #endregion

        /// <summary>
        /// Created By Harshit Mitra On 12-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="blogId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> LikeUnlineBlogs(ClaimsHelperModel tokenData, Guid blogId)
        {
            BlogsLikesEntity checkblogLike =
                await _context.BlogsLikesEntities
                .FirstOrDefaultAsync(x => blogId == x.BlogId && x.EmployeeId == tokenData.employeeId);
            if (checkblogLike == null)
            {
                BlogsLikesEntity obj = new BlogsLikesEntity
                {
                    BlogId = blogId,
                    EmployeeId = tokenData.employeeId,
                };
                _context.BlogsLikesEntities.Add(obj);
                await _context.SaveChangesAsync();
                return new ServiceResponse<bool>(HttpStatusCode.Accepted, true);
            }
            else
            {
                _context.BlogsLikesEntities.Remove(checkblogLike);
                await _context.SaveChangesAsync();
                return new ServiceResponse<bool>(HttpStatusCode.Accepted, true);
            }
        }
        /// <summary>
        /// Created By Harshit Mitra On 14-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> ApprovedAndUnApprovedBlogs(ClaimsHelperModel tokenData, Req.BlogChangeStatusRequest model)
        {
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
            Blogs blog = await _context.Blogs
                .FirstOrDefaultAsync(x => x.BlogId == model.BlogId);
            if (blog == null)
                return new ServiceResponse<bool>(HttpStatusCode.NotFound, false, false);
            string responseMessage = string.Empty;
            if (model.StatusType == Core.Enum.BlogStatusEnumClass.Approved)
            {
                blog.StatusType = BlogStatusEnumClass.Approved;
                blog.BlogsStatus = BlogsTypeEnumClass.Live;
                responseMessage = BlogStatusEnumClass.Approved.ToString();
            }
            else
            {
                blog.ReasonOfRejection = model.ReasonOfRejection;
                blog.StatusType = BlogStatusEnumClass.Not_DASH_Approved;
                blog.BlogsStatus = BlogsTypeEnumClass.Not_DASH_Approved;
                responseMessage = BlogStatusEnumClass.Not_DASH_Approved.ToString();
            }
            blog.ActionDate = today;
            blog.ActionBy = tokenData.employeeId;

            blog.UpdatedOn = today;
            blog.UpdatedBy = tokenData.employeeId;

            _context.Entry(blog).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(HttpStatusCode.OK, true, true, responseMessage);
        }
        /// <summary>
        /// Created By Harshit Mitra On 14-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<Res.GetLiveBlogListResponse>>> GetLiveBlogsLists(ClaimsHelperModel tokenData)
        {
            List<Res.GetLiveBlogListResponse> liveBlogsList =
                await _context.Blogs
                .Where(x => x.CompanyId == tokenData.companyId && !x.IsDeleted
                    && x.IsActive && x.BlogsStatus == BlogsTypeEnumClass.Live)
                .Select(x => new Res.GetLiveBlogListResponse
                {
                    BlogId = x.BlogId,
                    Title = x.Title,
                    Description = x.Description,
                    ActionDate = x.ActionDate,
                    Image = x.Image,
                })
                .ToListAsync();
            if (liveBlogsList.Count != 0)
                return new ServiceResponse<List<Res.GetLiveBlogListResponse>>(HttpStatusCode.OK, liveBlogsList);
            return new ServiceResponse<List<Res.GetLiveBlogListResponse>>(HttpStatusCode.NoContent, liveBlogsList, false);
        }
        /// <summary>
        /// Created By Harshit Mitra On 14-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<Res.GetBlogListInReview>>> GetInReviewBlogsLists(ClaimsHelperModel tokenData, Req.InReviewRequest model)
        {
            List<Res.GetBlogListInReview> liveBlogsList =
                await (from b in _context.Blogs
                       join c in _context.BlogCategories on b.CategoryId equals c.CategoryId
                       where b.CompanyId == tokenData.companyId && !b.IsDeleted &&
                            b.IsActive && b.BlogsStatus == BlogsTypeEnumClass.In_DASH_Review
                       select new Res.GetBlogListInReview
                       {
                           BlogId = b.BlogId,
                           Title = b.Title,
                           Description = b.Description,
                           Image = b.Image,
                           InReviewDate = b.CreatedOn,
                           CategoryName = c.CategoryName,

                       })
                       .ToListAsync();
            if (model.SearchString.Length > 2)
                liveBlogsList = liveBlogsList
                    .Where(x => x.Title.ToLower().Contains(model.SearchString.ToLower()) ||
                       x.CategoryName.ToLower().Contains(model.SearchString.ToLower()))
                    .ToList();
            if (liveBlogsList.Count != 0)
                return new ServiceResponse<List<Res.GetBlogListInReview>>(HttpStatusCode.OK, liveBlogsList);
            return new ServiceResponse<List<Res.GetBlogListInReview>>(HttpStatusCode.NoContent, liveBlogsList, false);
        }
        /// <summary>
        /// Created By Harshit Mitra On 18-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="blogsIds"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> MoveBlogsToTrash(ClaimsHelperModel tokenData, params Guid[] blogsIds)
        {
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.Now, tokenData.TimeZone);
            if (blogsIds.Length != 0)
            {
                List<Blogs> blogList =
                    await _context.Blogs
                    .Where(x => blogsIds.Contains(x.BlogId))
                    .ToListAsync();
                foreach (var blog in blogList)
                {
                    blog.UpdatedBy = tokenData.employeeId;
                    blog.UpdatedOn = today;
                    blog.BlogsStatus = BlogsTypeEnumClass.Trash;
                    _context.Entry(blog).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                return new ServiceResponse<bool>(HttpStatusCode.Accepted, true, true, "Moved To Trash");
            }
            return new ServiceResponse<bool>(HttpStatusCode.BadRequest, "Required To Select any One Blog");
        }
        /// <summary>
        /// Created By Harshit Mitra On 19-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<PagingResponse<Res.GetLiveBlogListResponse>>> GetLiveBlogsListsPaging(ClaimsHelperModel tokenData, Req.BlogLiveViewRequest model)
        {
            List<Res.GetLiveBlogListResponse> liveBlogsList = new List<Res.GetLiveBlogListResponse>();
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.Now, tokenData.TimeZone);
            var blogList = await _context.Blogs
                .Where(x => x.CompanyId == tokenData.companyId && !x.IsDeleted
                    && x.IsActive && x.BlogsStatus == BlogsTypeEnumClass.Live)
                .OrderByDescending(x => x.CreatedOn)
                .ToListAsync();
            var blogsIdList = blogList.Select(z => z.BlogId).ToList();
            var likesList = await _context.BlogsLikesEntities
                .Where(x => blogsIdList.Contains(x.BlogId))
                .ToListAsync();
            if (model.ShortBy.HasValue)
                _blogShortBy.ShortValue(model.ShortBy.Value, today.Date, ref blogList);
            if (model.CategoryId != Guid.Empty)
                liveBlogsList = (from b in blogList.AsEnumerable()
                                 join c in _context.BlogCategories.AsEnumerable() on b.CategoryId equals c.CategoryId
                                 join e in _context.Employee.AsEnumerable() on b.CreatedBy equals e.EmployeeId
                                 where b.CategoryId == model.CategoryId
                                 select new Res.GetLiveBlogListResponse
                                 {
                                     BlogId = b.BlogId,
                                     Title = b.Title,
                                     Description = b.Description,
                                     ActionDate = b.ActionDate,
                                     Image = b.Image,
                                     CategoryName = c.CategoryName,
                                     CreatedByName = e.DisplayName,
                                     LikeCount = likesList.AsEnumerable().LongCount(x => x.BlogId == b.BlogId),
                                     IsLikedByUser = likesList.AsEnumerable().Any(x => x.BlogId == b.BlogId && x.EmployeeId == tokenData.employeeId),
                                 })
                                 .ToList();
            else
                liveBlogsList = (from b in blogList.AsEnumerable()
                                 join c in _context.BlogCategories.AsEnumerable() on b.CategoryId equals c.CategoryId
                                 join e in _context.Employee.AsEnumerable() on b.CreatedBy equals e.EmployeeId
                                 select new Res.GetLiveBlogListResponse
                                 {
                                     BlogId = b.BlogId,
                                     Title = b.Title,
                                     Description = b.Description,
                                     ActionDate = b.ActionDate,
                                     Image = b.Image,
                                     CategoryName = c.CategoryName,
                                     CreatedByName = e.DisplayName,
                                     LikeCount = likesList.LongCount(x => x.BlogId == b.BlogId),
                                     IsLikedByUser = likesList.Any(x => x.BlogId == b.BlogId && x.EmployeeId == tokenData.employeeId),
                                 })
                                 .ToList();
            if (model.SearchString.Length > 2)
                liveBlogsList = liveBlogsList
                    .Where(x => x.Title.ToLower().Contains(model.SearchString.ToLower()) ||
                        x.CategoryName.ToLower().Contains(model.SearchString.ToLower()))
                    .ToList();
            if (liveBlogsList.Count != 0)
                return new ServiceResponse<PagingResponse<Res.GetLiveBlogListResponse>>(
                    HttpStatusCode.OK, new PagingResponse<Res.GetLiveBlogListResponse>(liveBlogsList, model.Page, model.Count));
            return new ServiceResponse<PagingResponse<Res.GetLiveBlogListResponse>>(
                HttpStatusCode.NoContent, new PagingResponse<Res.GetLiveBlogListResponse>(liveBlogsList, model.Page, model.Count), false);
        }
        /// <summary>
        /// Created By Harshit Mitra On 26-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> ApproveAndRejectBlogMultiple(ClaimsHelperModel tokenData, Req.ApproveOrRejectMultipleRequest model)
        {
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.Now, tokenData.TimeZone);
            if (model.BlogsIds.Length != 0)
            {
                List<Blogs> blogList =
                    await _context.Blogs
                    .Where(x => model.BlogsIds.Contains(x.BlogId))
                    .ToListAsync();
                foreach (var blog in blogList)
                {
                    switch (model.StatusType)
                    {
                        case BlogStatusEnumClass.Still_Pending:
                            blog.StatusType = BlogStatusEnumClass.Still_Pending;
                            blog.BlogsStatus = BlogsTypeEnumClass.In_DASH_Review;
                            break;
                        case BlogStatusEnumClass.Approved:
                            blog.StatusType = BlogStatusEnumClass.Approved;
                            blog.BlogsStatus = BlogsTypeEnumClass.Live;
                            break;
                        case BlogStatusEnumClass.Not_DASH_Approved:
                            blog.StatusType = BlogStatusEnumClass.Not_DASH_Approved;
                            blog.BlogsStatus = BlogsTypeEnumClass.Not_DASH_Approved;
                            break;
                    }
                    blog.UpdatedBy = tokenData.employeeId;
                    blog.UpdatedOn = today;
                    _context.Entry(blog).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                return new ServiceResponse<bool>(HttpStatusCode.Accepted, true, true, model.StatusType.ToString()
                    .Replace("_DASH_", "-").Replace("_", " "));
            }
            return new ServiceResponse<bool>(HttpStatusCode.BadRequest, "Required To Select any One Blog");
        }
    }
}