using AspNetIdentity.Core.Common;
using AspNetIdentity.Core.Enum;
using AspNetIdentity.Core.Model.TsfModule;
using AspNetIdentity.Core.Model.TsfModule.NewsEntities;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Infrastructure.ITsfService;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Req = AspNetIdentity.Core.ViewModel.TsfViewModel.RequestNewsServiceClass;
using Res = AspNetIdentity.Core.ViewModel.TsfViewModel.ResponseNewsServiceClass;

namespace AspNetIdentity.WebApi.Services.TsfService
{
    public class NewsService : INewsService
    {
        #region Properties 
        private readonly ApplicationDbContext _context;
        #endregion

        #region Constructor
        public NewsService()
        {
            _context = new ApplicationDbContext();
        }
        #endregion

        /// <summary>
        /// Created By Harshit Mitra On 25-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<NewsEntity>> CreateNews(ClaimsHelperModel tokenData, Req.CreateNewsRequestClass model)
        {
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
            NewsEntity obj = new NewsEntity
            {
                CategoryId = model.CategoryId,
                Title = model.Title.Trim(),
                Description = model.Description.Trim(),
                Image = model.Image,
                CompanyId = tokenData.companyId,
                CreatedBy = tokenData.employeeId,
                CreatedOn = today,
                NewsType = model.NewsType,
            };
            _context.NewsEntities.Add(obj);
            await _context.SaveChangesAsync();
            return new ServiceResponse<NewsEntity>(HttpStatusCode.Created, obj);
        }

        /// <summary>
        /// Created By Harshit Mitra On 25-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<Res.GetNewsAdminResponse>>> GetAllNewsAdminView(ClaimsHelperModel tokenData, Req.GetNewsAdminViewRequest model)
        {
            List<Res.GetNewsAdminResponse> response =
                await (from n in _context.NewsEntities
                       join c in _context.NewsCategories on n.CategoryId equals c.CategoryId
                       where n.CompanyId == tokenData.companyId && n.IsActive && !n.IsDeleted
                           && model.NewsType == n.NewsType
                       select new Res.GetNewsAdminResponse
                       {
                           Image = n.Image,
                           NewsId = n.NewsId,
                           Title = n.Title,
                           Description = n.Description,
                           CategoryName = c.CategoryName,
                           CreatedOn = n.CreatedOn,
                           UpdatedOn = n.UpdatedOn,
                           IsFeatured = n.IsFeatured,
                       })
                       .OrderByDescending(x => x.CreatedOn)
                       .ToListAsync();
            if (model.SearchString.Length > 2)
                response = response.Where(x =>
                        x.Title.ToLower().Contains(model.SearchString.ToLower()) ||
                        x.CategoryName.ToLower().Contains(model.SearchString.ToLower()))
                    .ToList();
            if (response.Count != 0)
                return new ServiceResponse<List<Res.GetNewsAdminResponse>>(HttpStatusCode.OK, response);
            return new ServiceResponse<List<Res.GetNewsAdminResponse>>(HttpStatusCode.NoContent, response, false);
        }

        /// <summary>
        /// Created By Harshit Mitra On 25-04-2023
        /// </summary>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<NewsEntity>> GetById(Guid newsId)
        {
            NewsEntity entity = await _context.NewsEntities
                .Where(x => x.NewsId == newsId)
                .FirstOrDefaultAsync();
            if (entity == null)
                return new ServiceResponse<NewsEntity>(HttpStatusCode.NotFound);
            return new ServiceResponse<NewsEntity>(HttpStatusCode.OK, entity);
        }

        /// <summary>
        /// Created By Harshit Mitra On 25-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> UpdateNews(ClaimsHelperModel tokenData, Req.UpdateNewsClassRequest model)
        {
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
            NewsEntity entity = await _context.NewsEntities
               .Where(x => x.NewsId == model.NewsId)
               .FirstOrDefaultAsync();
            if (entity == null)
                return new ServiceResponse<bool>(HttpStatusCode.NotFound, false);
            entity.Title = model.Title;
            entity.Description = model.Description;
            entity.Image = model.Image;
            entity.CategoryId = model.CategoryId;
            entity.UpdatedOn = today;
            entity.UpdatedBy = tokenData.employeeId;
            entity.NewsType = model.NewsType;
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(HttpStatusCode.Accepted, true);
        }

        /// <summary>
        /// Created By Harshit Mitra On 25-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="newsIds"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> MoveToTrashNews(ClaimsHelperModel tokenData, Guid[] newsIds)
        {
            if (newsIds.Length == 0)
                return new ServiceResponse<bool>(HttpStatusCode.BadRequest, "Required To Select Any One News");
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.Now, tokenData.TimeZone);
            List<NewsEntity> listsNews = await _context.NewsEntities
                .Where(x => newsIds.Contains(x.NewsId))
                .ToListAsync();
            listsNews.ForEach(x =>
            {
                x.UpdatedOn = today;
                x.UpdatedBy = tokenData.employeeId;
                x.NewsType = NewsEnumType.Trash;
                x.IsFeatured = false;
            });
            foreach (var item in listsNews)
            {
                _context.Entry(item).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(HttpStatusCode.Accepted, true, true, "Moved To Trash");
        }

        /// <summary>
        /// Created By Harshit Mitra On 25-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="newsIds"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> DeleteNews(ClaimsHelperModel tokenData, Guid[] newsIds)
        {
            if (newsIds.Length == 0)
                return new ServiceResponse<bool>(HttpStatusCode.BadRequest, "Required To Select Any One News");
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.Now, tokenData.TimeZone);
            List<NewsEntity> listsNews = await _context.NewsEntities
                .Where(x => newsIds.Contains(x.NewsId))
                .ToListAsync();
            listsNews.ForEach(x =>
            {
                x.DeletedOn = today;
                x.DeletedBy = tokenData.employeeId;
                x.IsActive = false;
                x.IsDeleted = true;
            });
            foreach (var item in listsNews)
            {
                _context.Entry(item).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(HttpStatusCode.MovedPermanently, true);
        }

        
     
    }
}