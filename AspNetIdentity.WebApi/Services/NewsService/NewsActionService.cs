using AspNetIdentity.Core.Common;
using AspNetIdentity.Core.Enum;
using AspNetIdentity.Core.Model.TsfModule;
using AspNetIdentity.Core.Model.TsfModule.NewsEntities;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Interface;
using AspNetIdentity.WebApi.Interface.INewsService;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Req = AspNetIdentity.Core.ViewModel.NewsViewModel.NewsActionViewRequest;
using Res = AspNetIdentity.Core.ViewModel.NewsViewModel.NewsActionViewResponse;

namespace AspNetIdentity.WebApi.Services.NewsService
{
    public class NewsActionService : INewsActionService
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        private readonly IShortByService<NewsEntity> _shortByService;
        #endregion

        #region Constructor
        public NewsActionService()
        {
            _context = new ApplicationDbContext();
            _shortByService = new ShortByService<NewsEntity>();
        }
        #endregion

        /// <summary>
        /// Created By Harshit Mitra On 26-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<Res.GetNewsCategoriesCountResponse>>> GetNewsCategoryCount(ClaimsHelperModel tokenData)
        {
            List<Res.GetNewsCategoriesCountResponse> response =
                new List<Res.GetNewsCategoriesCountResponse>()
                {
                    new Res.GetNewsCategoriesCountResponse()
                    {
                        CategoryId = Guid.Empty,
                        CategoryName = "All",
                        Count = 0
                    },
                };

            var newsList = await (from c in _context.NewsCategories
                                  join n in _context.NewsEntities
                                    .Where(x => x.IsActive && x.NewsType == NewsEnumType.Publish
                                        && !x.IsDeleted && x.CompanyId == tokenData.companyId)
                                    .AsEnumerable()
                                    on c.CategoryId equals n.CategoryId
                                    into emptyNews
                                  from result in emptyNews.DefaultIfEmpty()
                                  where c.IsActive && !c.IsDeleted && c.CompanyId == tokenData.companyId
                                  select new
                                  {
                                      Category = c,
                                      News = result,
                                  })
                                  .GroupBy(x => x.Category.CategoryId)
                                  .Select(x => new Res.GetNewsCategoriesCountResponse
                                  {
                                      CategoryId = x.Key,
                                      CategoryName = x.FirstOrDefault().Category.CategoryName,
                                      Count = x.Where(z => z.News != null).Select(z => z.News).ToList().Count(),
                                  })
                                  .OrderBy(x => x.CategoryName)
                                  .ToListAsync();
            response[0].Count = newsList.Sum(x => x.Count);
            response.AddRange(newsList);
            return new ServiceResponse<List<Res.GetNewsCategoriesCountResponse>>(HttpStatusCode.OK, response);
        }

        /// <summary>
        /// Created By Harshit Mitra On 26-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<PagingResponse<Res.GetPublishNewsResponse>>> GetPublishNewsUserView(ClaimsHelperModel tokenData, Req.PublishNewsFilterRequest model)
        {
            List<Res.GetPublishNewsResponse> liveNewsList = new List<Res.GetPublishNewsResponse>();
            DateTimeOffset today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.Now, tokenData.TimeZone);
            List<NewsEntity> newsList = await _context.NewsEntities
                .Where(x => x.CompanyId == tokenData.companyId && x.IsActive
                    && !x.IsDeleted && x.NewsType == NewsEnumType.Publish)
                .ToListAsync();
            var newsListIds = newsList.Select(x => x.NewsId).Distinct().ToList();
            var likesList = await _context.NewsLikeEntities
                .Where(x => newsListIds.Contains(x.NewsId))
                .ToListAsync();
            if (model.ShortBy.HasValue)
                _shortByService.ShortValue(model.ShortBy.Value, today.Date, ref newsList);
            if (model.CategoryId == Guid.Empty)
                liveNewsList = (from n in newsList.AsEnumerable()
                                join c in _context.NewsCategories on n.CategoryId equals c.CategoryId
                                where c.IsActive && !c.IsDeleted
                                orderby n.CreatedOn descending
                                select new Res.GetPublishNewsResponse
                                {
                                    CategoryName = c.CategoryName,
                                    NewsId = n.NewsId,
                                    Title = n.Title,
                                    Description = n.Description,
                                    Image = n.Image,
                                    CreatedOn = n.CreatedOn,
                                    LikeCount = likesList.AsEnumerable().LongCount(x => x.NewsId == n.NewsId),
                                    IsLikedByUser = likesList.AsEnumerable().Any(x => x.NewsId == n.NewsId && x.EmployeeId == tokenData.employeeId),
                                })
                                .ToList();
            else
                liveNewsList = (from n in newsList.AsEnumerable()
                                join c in _context.NewsCategories on n.CategoryId equals c.CategoryId
                                where n.CategoryId == model.CategoryId && c.IsActive && !c.IsDeleted
                                orderby n.CreatedOn descending
                                select new Res.GetPublishNewsResponse
                                {
                                    CategoryName = c.CategoryName,
                                    NewsId = n.NewsId,
                                    Title = n.Title,
                                    Description = n.Description,
                                    Image = n.Image,
                                    CreatedOn = n.CreatedOn,
                                    LikeCount = likesList.AsEnumerable().LongCount(x => x.NewsId == n.NewsId),
                                    IsLikedByUser = likesList.AsEnumerable().Any(x => x.NewsId == n.NewsId && x.EmployeeId == tokenData.employeeId),
                                })
                                .ToList();
            if (model.SearchString.Length > 2)
                liveNewsList = liveNewsList
                    .Where(x => x.Title.ToLower().Contains(model.SearchString.ToLower()) ||
                        x.CategoryName.ToLower().Contains(model.SearchString.ToLower()))
                    .ToList();
            if (liveNewsList.Count != 0)
                return new ServiceResponse<PagingResponse<Res.GetPublishNewsResponse>>(
                    HttpStatusCode.OK, new PagingResponse<Res.GetPublishNewsResponse>(liveNewsList, model.Page, model.Count));
            return new ServiceResponse<PagingResponse<Res.GetPublishNewsResponse>>(
                HttpStatusCode.NoContent, new PagingResponse<Res.GetPublishNewsResponse>(liveNewsList, model.Page, model.Count), false);
        }

        /// <summary>
        /// Created By Harshit Mitra On 26-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<Res.GetPublishNewsResponse>> GetTopNewsOnUserView(ClaimsHelperModel tokenData, Req.GetTopNewsRequest model)
        {
            Res.GetPublishNewsResponse response = null;
            if (model.CategoryId == Guid.Empty)
                response = await (from n in _context.NewsEntities
                                  join c in _context.NewsCategories on n.CategoryId equals c.CategoryId
                                  where c.IsActive && !c.IsDeleted && !n.IsDeleted && n.IsActive
                                   && n.NewsType == NewsEnumType.Publish
                                  orderby n.CreatedOn descending
                                  select new Res.GetPublishNewsResponse
                                  {
                                      CategoryName = c.CategoryName,
                                      NewsId = n.NewsId,
                                      Title = n.Title,
                                      Description = n.Description,
                                      Image = n.Image,
                                      CreatedOn = n.CreatedOn,
                                      LikeCount = _context.NewsLikeEntities.LongCount(x => x.NewsId == n.NewsId),
                                      IsLikedByUser = _context.NewsLikeEntities.Any(x => x.NewsId == n.NewsId && x.EmployeeId == tokenData.employeeId),
                                  })
                                  .FirstOrDefaultAsync();
            else
                response = await (from n in _context.NewsEntities
                                  join c in _context.NewsCategories on n.CategoryId equals c.CategoryId
                                  where c.IsActive && !c.IsDeleted && !n.IsDeleted && n.IsActive
                                    && n.NewsType == NewsEnumType.Publish && c.CategoryId == model.CategoryId
                                  orderby n.CreatedOn descending
                                  select new Res.GetPublishNewsResponse
                                  {
                                      CategoryName = c.CategoryName,
                                      NewsId = n.NewsId,
                                      Title = n.Title,
                                      Description = n.Description,
                                      Image = n.Image,
                                      CreatedOn = n.CreatedOn,
                                      LikeCount = _context.NewsLikeEntities.LongCount(x => x.NewsId == n.NewsId),
                                      IsLikedByUser = _context.NewsLikeEntities.Any(x => x.NewsId == n.NewsId && x.EmployeeId == tokenData.employeeId),
                                  })
                                  .FirstOrDefaultAsync();
            if (response != null)
                return new ServiceResponse<Res.GetPublishNewsResponse>(HttpStatusCode.OK, response);
            return new ServiceResponse<Res.GetPublishNewsResponse>(HttpStatusCode.NoContent, response, false);
        }

        /// <summary>
        /// Created By Harshit Mitra On 27-04-2023
        /// </summary>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<Res.GetPublishNewsResponse>> GetNewsDetailsById(Guid newsId)
        {
            var response = await (from n in _context.NewsEntities
                                  join c in _context.NewsCategories on n.CategoryId equals c.CategoryId
                                  where c.IsActive && !c.IsDeleted && !n.IsDeleted && n.IsActive
                                   && n.NewsType == NewsEnumType.Publish
                                  orderby n.CreatedOn descending
                                  select new Res.GetPublishNewsResponse
                                  {
                                      CategoryName = c.CategoryName,
                                      NewsId = n.NewsId,
                                      Title = n.Title,
                                      Description = n.Description,
                                      Image = n.Image,
                                      CreatedOn = n.CreatedOn,
                                  })
                                  .FirstOrDefaultAsync();
            if (response != null)
                return new ServiceResponse<Res.GetPublishNewsResponse>(HttpStatusCode.OK, response);
            return new ServiceResponse<Res.GetPublishNewsResponse>(HttpStatusCode.NoContent, response, false);
        }

        /// <summary>
        /// Created By Harshit Mitra On 27-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> LikeUnlikeNews(ClaimsHelperModel tokenData, Guid newsId)
        {
            NewsLikeEntity checkNewsLike =
                await _context.NewsLikeEntities
                .FirstOrDefaultAsync(x => newsId == x.NewsId && x.EmployeeId == tokenData.employeeId);
            if (checkNewsLike == null)
            {
                NewsLikeEntity obj = new NewsLikeEntity
                {
                    NewsId = newsId,
                    EmployeeId = tokenData.employeeId,
                };
                _context.NewsLikeEntities.Add(obj);
                await _context.SaveChangesAsync();
                return new ServiceResponse<bool>(HttpStatusCode.Accepted, true);
            }
            else
            {
                _context.NewsLikeEntities.Remove(checkNewsLike);
                await _context.SaveChangesAsync();
                return new ServiceResponse<bool>(HttpStatusCode.Accepted, true);
            }
        }

        /// <summary>
        /// Created By Harshit Mitra On 27-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> SetNewsAsFeature(ClaimsHelperModel tokenData, Guid newsId)
        {
            DateTimeOffset today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
            var newsList = await _context.NewsEntities
                .Where(x => x.IsFeatured || x.NewsId == newsId)
                .ToListAsync();
            var checkNews = newsList.FirstOrDefault(x => x.NewsId == newsId);
            if (checkNews == null)
                return new ServiceResponse<bool>(HttpStatusCode.NotFound, false, false);
            if (checkNews.IsFeatured)
            {
                checkNews.IsFeatured = false;
                checkNews.UpdatedOn = today;
                checkNews.UpdatedBy = tokenData.employeeId;

                _context.Entry(checkNews).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return new ServiceResponse<bool>(HttpStatusCode.Accepted, true);
            }
            else
            {
                if(newsList.Count != 4)
                {
                    checkNews.IsFeatured = true;
                    checkNews.FeaturedOn = today;
                    checkNews.UpdatedOn = today;
                    checkNews.UpdatedBy = tokenData.employeeId;

                    _context.Entry(checkNews).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return new ServiceResponse<bool>(HttpStatusCode.Accepted, true);
                }
                else
                {
                    var removeLastFeatured = newsList
                        .Where(x=> x.NewsId != newsId)
                        .OrderBy(x=> x.FeaturedOn)
                        .FirstOrDefault();

                    removeLastFeatured.IsFeatured = false;
                    _context.Entry(removeLastFeatured).State = EntityState.Modified;

                    checkNews.IsFeatured = true;
                    checkNews.FeaturedOn = today;
                    checkNews.UpdatedOn = today;
                    checkNews.UpdatedBy = tokenData.employeeId;

                    _context.Entry(checkNews).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return new ServiceResponse<bool>(HttpStatusCode.Accepted, true);
                }
            }
        }
    }
}