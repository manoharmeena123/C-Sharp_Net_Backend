using AspNetIdentity.Core.Common;
using AspNetIdentity.Core.Enum;
using AspNetIdentity.Core.Model.TsfModule;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Interface;
using AspNetIdentity.WebApi.Interface.ITsfService;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Req = AspNetIdentity.Core.ViewModel.TsfViewModel.BlogRequestClass;
using Res = AspNetIdentity.Core.ViewModel.TsfViewModel.BlogResponseClass;

namespace AspNetIdentity.WebApi.Services.TsfService
{
    public class BlogsService : IBlogsService
    {
        #region Properties 
        private readonly ApplicationDbContext _context;
        private readonly IShortByService<Blogs> _shortByService;
        private readonly Logger _logger;
        #endregion

        #region Constructor
        public BlogsService()
        {
            _context = new ApplicationDbContext();
            _logger = LogManager.GetCurrentClassLogger();
            _shortByService = new ShortByService<Blogs>();
        }
        #endregion

        /// <summary>
        /// Create Blogs
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<Blogs>> AddBlogsData(ClaimsHelperModel tokenData, Req.CreateBlogRequestclass model)
        {
            var createDate = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
            try
            {
                Blogs obj = new Blogs
                {
                    CategoryId = model.CategoryId,
                    Title = model.Title,
                    Description = model.Description,
                    Image = model.Image,
                    BlogsStatus = model.BlogsStatus,
                    CreatedBy = tokenData.employeeId,
                    CompanyId = tokenData.companyId,
                    CreatedOn = createDate,
                };
                _context.Blogs.Add(obj);
                await _context.SaveChangesAsync();
                return new ServiceResponse<Blogs>(HttpStatusCode.Created, obj);
            }
            catch (Exception ex)
            {
                _logger.Error("API : api/blogs/addblogs | " +
                "Exception : " + JsonConvert.SerializeObject(ex));
                throw ex;
            }
        }
        /// <summary>
        /// Update Blogs
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> UpdateBlogsData(ClaimsHelperModel tokenData, Req.UpdateBlogRequestclass model)
        {
            var updateDate = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
            try
            {
                var updateData = await _context.Blogs.FirstOrDefaultAsync
                                (x => x.BlogId == model.BlogId && x.CompanyId == tokenData.companyId
                                  && x.IsActive && !x.IsDeleted);
                if (updateData != null)
                {
                    updateData.BlogsStatus = model.BlogsStatus;
                    updateData.Title = model.Title;
                    updateData.Description = model.Description;
                    updateData.Image = model.Image;
                    updateData.CategoryId = model.CategoryId;
                    updateData.UpdatedBy = tokenData.employeeId;
                    updateData.UpdatedOn = updateDate;
                    updateData.CompanyId = tokenData.companyId;
                    _context.Entry(updateData).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return new ServiceResponse<bool>(HttpStatusCode.Accepted, true);
                }
                return new ServiceResponse<bool>(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                _logger.Error("API : api/blogs/updateblogs | " +
                "Exception : " + JsonConvert.SerializeObject(ex));
                throw ex;
            }
        }
        /// <summary>
        /// Get Blog By Id
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="blogId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<Res.GetBlogResponseByIdClass>> GetBlogsById(ClaimsHelperModel tokenData, Guid blogId)
        {
            var createDate = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
            try
            {
                var getBlogs =
                await _context.Blogs
                .Where(x => x.CompanyId == tokenData.companyId && x.IsActive && !x.IsDeleted && x.BlogId == blogId)
                .Select(x => new Res.GetBlogResponseByIdClass
                {
                    BlogId = x.BlogId,
                    CategoryId = x.CategoryId,
                    Title = x.Title,
                    Description = x.Description,
                    Image = x.Image,
                    BlogsStatus = x.BlogsStatus,
                })
                .FirstOrDefaultAsync();

                if (getBlogs != null)
                    return new ServiceResponse<Res.GetBlogResponseByIdClass>(HttpStatusCode.OK, getBlogs);
                return new ServiceResponse<Res.GetBlogResponseByIdClass>(HttpStatusCode.NoContent, getBlogs, false);
            }
            catch (Exception ex)
            {
                _logger.Error("API : api/blogs/getblogsbyid | " +
                "Exception : " + JsonConvert.SerializeObject(ex));
                throw ex;
            }
        }
        /// <summary>
        /// Get Blog List By Status
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<Res.GetBlogListPagingResponse>>> GetBlogsData(ClaimsHelperModel tokenData, Req.GetRequestClass model)
        {
            var createDate = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow).Date;
            try
            {
                List<Blogs> blogList = await _context.Blogs
                    .Where(x => x.CompanyId == tokenData.companyId && x.IsActive && !x.IsDeleted)
                    .OrderByDescending(x => x.CreatedOn)
                    .ToListAsync();
                if (model.ShortType.HasValue)
                    _shortByService.ShortValue(model.ShortType.Value, today, ref blogList);
                List<Res.GetBlogListPagingResponse> response = new List<Res.GetBlogListPagingResponse>();
                if (model.BlogType == BlogsTypeEnumClass.All_Blogs)
                    response = blogList
                        .Where(x => x.BlogsStatus != BlogsTypeEnumClass.Trash)
                        .Select(x => new Res.GetBlogListPagingResponse
                        {
                            BlogId = x.BlogId,
                            Title = x.Title,
                            CategoryId = x.CategoryId,
                            Description = x.Description,
                            BlogsStatus = x.BlogsStatus,
                            BlogStatusName = x.BlogsStatus.ToString().ToUpper()
                                .Replace("_DASH_", "-").Replace("_", " "),
                            CreatedDate = x.CreatedOn,
                            UpdatedDate = x.UpdatedOn,
                            Image = x.Image,
                        })
                        .ToList();
                else
                    response = blogList
                        .Where(x => x.BlogsStatus == model.BlogType)
                        .Select(x => new Res.GetBlogListPagingResponse
                        {
                            BlogId = x.BlogId,
                            Title = x.Title,
                            CategoryId = x.CategoryId,
                            Description = x.Description,
                            BlogsStatus = x.BlogsStatus,
                            BlogStatusName = x.BlogsStatus.ToString().ToUpper()
                                .Replace("_DASH_", "-").Replace("_", " "),
                            CreatedDate = x.CreatedOn,
                            UpdatedDate = x.UpdatedOn,
                            Image = x.Image,
                        })
                        .ToList();
                response = response.Join(_context.BlogCategories.Where(z => z.CompanyId == tokenData.companyId).AsEnumerable(),
                     x => x.CategoryId,
                    category => category.CategoryId,
                    (x, category) => new Res.GetBlogListPagingResponse
                    {
                        BlogId = x.BlogId,
                        Title = x.Title,
                        CategoryId = x.CategoryId,
                        Description = x.Description,
                        BlogsStatus = x.BlogsStatus,
                        BlogStatusName = x.BlogStatusName,
                        CreatedDate = x.CreatedDate,
                        UpdatedDate = x.UpdatedDate,
                        CategoryName = category.CategoryName,
                        Image = x.Image,

                    }).ToList();
                if (model.SearchString.Length > 2)
                    response = response
                        .Where(x => x.CategoryName.ToLower().Contains(model.SearchString.ToLower()) ||
                            x.Title.ToLower().Contains(model.SearchString.ToLower()) ||
                            x.BlogStatusName.ToLower().Contains(model.SearchString.ToLower()))
                        .ToList();
                if (response.Count == 0)
                    return new ServiceResponse<List<Res.GetBlogListPagingResponse>>(HttpStatusCode.NoContent, response, false);
                return new ServiceResponse<List<Res.GetBlogListPagingResponse>>(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                _logger.Error("API : api/blogs/getblogs | " +
                "Exception : " + JsonConvert.SerializeObject(ex));
                throw ex;
            }
        }
        /// <summary>
        /// Created By Harshit Mitra On 18-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="blogsIds"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> DeleteBlog(ClaimsHelperModel tokenData, params Guid[] blogsIds)
        {
            try
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
                        blog.DeletedBy = tokenData.employeeId;
                        blog.DeletedOn = today;
                        blog.BlogsStatus = BlogsTypeEnumClass.Trash;
                        blog.IsActive = false;
                        blog.IsDeleted = true;
                        _context.Entry(blog).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                    return new ServiceResponse<bool>(HttpStatusCode.MovedPermanently, true);
                }
                return new ServiceResponse<bool>(HttpStatusCode.BadRequest, "Required To Select any One Blog");
            }
            catch (Exception ex)
            {
                _logger.Error("API : api/newsintsf/removenewsbyid | " +
                "Exception : " + JsonConvert.SerializeObject(ex));
                throw ex;
            }
        }
        /// <summary>
        /// Get Blog List By Filter 
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="shortRequest"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<Res.GetBlogResponseByIdClass>>> GetBlogByFilter(ClaimsHelperModel tokenData, ShortByEnumClass? shortRequest)
        {
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow).Date;
            List<Blogs> blogList = await _context.Blogs.Where(x => x.CompanyId == tokenData.companyId)
                   .OrderByDescending(x => x.CreatedOn).ToListAsync();
            if (shortRequest.HasValue)
                _shortByService.ShortValue(shortRequest.Value, today, ref blogList);
            var responseBlogList = blogList
                .Select(x => new Res.GetBlogResponseByIdClass
                {
                    BlogId = x.BlogId,
                    Title = x.Title,
                    Description = x.Description,
                    Image = x.Image,
                    BlogsStatus = x.BlogsStatus,
                })
                .ToList();
            if (blogList.Count == 0)
                return new ServiceResponse<List<Res.GetBlogResponseByIdClass>>(HttpStatusCode.NoContent, responseBlogList, false);
            return new ServiceResponse<List<Res.GetBlogResponseByIdClass>>(HttpStatusCode.OK, responseBlogList);
        }
        /// <summary>
        /// Get Blogs Enum List 
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        public ServiceResponse<List<Res.BlogsResponseEnumData>> GetBlogsEnumType(ClaimsHelperModel tokenData)
        {
            try
            {
                List<Res.BlogsResponseEnumData> getBlogs =
                    Enum.GetValues(typeof(BlogsTypeEnumClass))
                    .Cast<BlogsTypeEnumClass>()
                    .Select(x => new Res.BlogsResponseEnumData
                    {
                        Id = (int)x,
                        Name = x.ToString()
                            .Replace("_DASH_", "-")
                            .Replace("_", " "),
                    })
                    .ToList();
                return new ServiceResponse<List<Res.BlogsResponseEnumData>>(HttpStatusCode.OK, getBlogs);
            }
            catch (Exception ex)
            {
                _logger.Error("API : api/blogs/getallblogs | " +
                "Exception : " + JsonConvert.SerializeObject(ex));
                throw ex;
            }
        }
        /// <summary>
        /// Created By Harshit Mitra On 11-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<Res.GetBlogHeaderResponse>>> GetBlogHeaderWithCount(ClaimsHelperModel tokenData)
        {
            List<Blogs> blogList =
                await _context.Blogs
                .Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId)
                .ToListAsync();
            List<Res.GetBlogHeaderResponse> getBlogEnum =
                Enum.GetValues(typeof(BlogsTypeEnumClass))
                .Cast<BlogsTypeEnumClass>()
                .Select(x => new Res.GetBlogHeaderResponse
                {
                    Id = (int)x,
                    Name = x.ToString()
                        .Replace("_DASH_", "-")
                        .Replace("_", " "),
                    Count = x == BlogsTypeEnumClass.All_Blogs ?
                        blogList.LongCount(z => z.BlogsStatus != BlogsTypeEnumClass.Trash)
                        :
                        blogList.LongCount(z => z.BlogsStatus == x),
                })
                .ToList();
            return new ServiceResponse<List<Res.GetBlogHeaderResponse>>(HttpStatusCode.OK, getBlogEnum);
        }
        /// <summary>
        /// Created By Harshit Mitra On 24-04-2023
        /// </summary>
        /// <param name="blogId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<Res.PreviewBlogResponse>> GetPriviewBlogForReview(Guid blogId)
        {
            Res.PreviewBlogResponse respone =
                await (from b in _context.Blogs
                       join c in _context.BlogCategories on b.CategoryId equals c.CategoryId
                       join e in _context.Employee on b.CreatedBy equals e.EmployeeId
                       where b.BlogId == blogId
                       select new Res.PreviewBlogResponse
                       {
                           BlogId = b.BlogId,
                           Title = b.Title,
                           Description = b.Description,
                           Image = b.Image,
                           CategoryName = c.CategoryName,
                           CreateOn = b.CreatedOn,
                           CreatedByName = e.DisplayName,
                       })
                       .FirstOrDefaultAsync();
            if (respone == null)
                return new ServiceResponse<Res.PreviewBlogResponse>(HttpStatusCode.NotFound);
            return new ServiceResponse<Res.PreviewBlogResponse>(HttpStatusCode.OK, respone);
        }
        /// <summary>
        /// Created By Harshit Mitra On 26-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<Res.GetMyBlogListResponse>>> MyLatestTopBlogs(ClaimsHelperModel tokenData)
        {
            var response = await (from b in _context.Blogs
                                  join c in _context.BlogCategories on b.CategoryId equals c.CategoryId
                                  where b.CreatedBy == tokenData.employeeId && !b.IsDeleted &&
                                        b.BlogsStatus == BlogsTypeEnumClass.Live && b.IsActive 
                                  orderby b.CreatedOn descending
                                  select new Res.GetMyBlogListResponse
                                  {
                                      BlogId = b.BlogId,
                                      CategoryName = c.CategoryName,
                                      Title = b.Title,
                                      Image = b.Image,
                                      Description = b.Description,
                                      CreateOn = b.CreatedOn
                                  })
                                  .Take(3)
                                  .ToListAsync();
            if (response.Count != 0)
                return new ServiceResponse<List<Res.GetMyBlogListResponse>>(HttpStatusCode.OK, response);
            return new ServiceResponse<List<Res.GetMyBlogListResponse>>(HttpStatusCode.NoContent, response, false);
        }
    }
}