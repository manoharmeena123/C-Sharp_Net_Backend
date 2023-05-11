using AspNetIdentity.Core.Common;
using AspNetIdentity.Core.Model.TsfModule.NewsEntities;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Interface.INewsService;
using AspNetIdentity.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Req = AspNetIdentity.Core.ViewModel.NewsViewModel.RequestNewsCategory;
using Res = AspNetIdentity.Core.ViewModel.NewsViewModel.ResponseNewsCategory;

namespace AspNetIdentity.WebApi.Services.NewsService
{
    public class NewsCategoryService : INewsCategoryService
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        #endregion

        #region Constructor
        public NewsCategoryService()
        {
            _context = new ApplicationDbContext();
        }
        #endregion

        /// <summary>
        /// Created By Harshit Mitra On 21-04-2023
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<NewsCategoryEntity>> CreateNewsCategories(ClaimsHelperModel tokenData, Req.CreateNewCateoryRequest model)
        {
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.Now, tokenData.TimeZone);
            if (await _context.NewsCategories.AnyAsync(x => x.CategoryName.Trim().ToUpper() == model.CategoryName.ToUpper()))
                return new ServiceResponse<NewsCategoryEntity>(HttpStatusCode.NotAcceptable, null, false, "Category Allready Exist");
            NewsCategoryEntity obj = new NewsCategoryEntity()
            {
                CategoryName = model.CategoryName.Trim(),
                Description = model.Description ?? string.Empty,
                CreatedOn = today,
                CreatedBy = tokenData.employeeId,
                CompanyId = tokenData.companyId,
            };
            _context.NewsCategories.Add(obj);
            await _context.SaveChangesAsync();
            return new ServiceResponse<NewsCategoryEntity>(HttpStatusCode.Created, obj);
        }
        /// <summary>
        /// Created By Harshit Mitra On 24-04-2023
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> UpdateNewsCategories(ClaimsHelperModel tokenData, Req.UpdateNewCateoryRequest model)
        {
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.Now, tokenData.TimeZone);
            NewsCategoryEntity update = await _context.NewsCategories.
                FirstOrDefaultAsync(x => x.CategoryId == model.CategoryId);
            if (update == null)
                return new ServiceResponse<bool>(HttpStatusCode.NotFound, false, false);
            if (await _context.NewsCategories
                .AnyAsync(x => x.CategoryName.Trim().ToUpper() == model.CategoryName.ToUpper() &&
                x.CategoryId != model.CategoryId))
                return new ServiceResponse<bool>(HttpStatusCode.NotAcceptable, false, false, "Category Allready Exist");
            update.UpdatedOn = today;
            update.UpdatedBy = tokenData.employeeId;
            update.CategoryName = model.CategoryName;
            update.Description = model.Description;
            _context.Entry(update).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(HttpStatusCode.Accepted, true);
        }
        /// <summary>
        /// Created By Harshit Mitra On 24-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> MoveToTrashNewsCategories(ClaimsHelperModel tokenData, params Guid[] categoryId)
        {
            if (categoryId.Length == 0)
                return new ServiceResponse<bool>(HttpStatusCode.BadRequest, "Required To Select any One News Category");
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.Now, tokenData.TimeZone);
            List<NewsCategoryEntity> listCategories = await _context.NewsCategories
                .Where(x => categoryId.Contains(x.CategoryId))
                .ToListAsync();
            listCategories.ForEach(x =>
            {
                x.UpdatedOn = today;
                x.UpdatedBy = tokenData.employeeId;
                x.InTrash = true;
            });
            foreach (var item in listCategories)
            {
                _context.Entry(item).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(HttpStatusCode.Accepted, true, true, "Moved To Trash");
        }
        /// <summary>
        /// Created By Harshit Mitra On 24-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<Res.GetNewsCategoryResponse>>> GetNewsCategoryList(ClaimsHelperModel tokenData)
        {
            List<Res.GetNewsCategoryResponse> response =
                await (from c in _context.NewsCategories
                       join e in _context.Employee on c.CreatedBy equals e.EmployeeId
                       where c.IsActive && !c.IsDeleted && !c.InTrash &&
                            c.CompanyId == tokenData.companyId
                       select new Res.GetNewsCategoryResponse
                       {
                           CategoryId = c.CategoryId,
                           CategoryName = c.CategoryName,
                           Description = c.Description,
                           CreatedOn = c.CreatedOn,
                           CreatedByName = e.DisplayName,
                       })
                       .OrderByDescending(x => x.CreatedOn)
                       .ToListAsync();
            if (response.Count != 0)
                return new ServiceResponse<List<Res.GetNewsCategoryResponse>>(HttpStatusCode.OK, response);
            return new ServiceResponse<List<Res.GetNewsCategoryResponse>>(HttpStatusCode.NoContent, response, false);
        }
        /// <summary>
        /// Created By Harshit Mitra On 24-04-2023
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<Res.GetNewsCategoryByIdResponse>> GetNewsCategoryById(Guid categoryId)
        {
            Res.GetNewsCategoryByIdResponse response =
                await (from c in _context.NewsCategories
                       where c.IsActive && !c.IsDeleted && !c.InTrash &&
                            c.CategoryId == categoryId
                       select new Res.GetNewsCategoryByIdResponse
                       {
                           CategoryId = c.CategoryId,
                           CategoryName = c.CategoryName,
                           Description = c.Description,
                           CreatedOn = c.CreatedOn,
                       })
                       .FirstOrDefaultAsync();
            if (response != null)
                return new ServiceResponse<Res.GetNewsCategoryByIdResponse>(HttpStatusCode.OK, response);
            return new ServiceResponse<Res.GetNewsCategoryByIdResponse>(HttpStatusCode.NotFound);
        }
        /// <summary>
        /// Created By Harshit Mitra On 24-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> DeleteNewsCategories(ClaimsHelperModel tokenData, params Guid[] categoryId)
        {
            if (await _context.NewsEntities.AnyAsync(x => x.IsActive && !x.IsDeleted && categoryId.Contains(x.CategoryId)))
                return new ServiceResponse<bool>(HttpStatusCode.BadRequest, "Selected Category Is Exist In News");
            if (categoryId.Length == 0)
                return new ServiceResponse<bool>(HttpStatusCode.BadRequest, "Required To Select any One News Category");
            var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.Now, tokenData.TimeZone);
            List<NewsCategoryEntity> listCategories = await _context.NewsCategories
                .Where(x => categoryId.Contains(x.CategoryId))
                .ToListAsync();
            listCategories.ForEach(x =>
            {
                x.DeletedOn = today;
                x.DeletedBy = tokenData.employeeId;
                x.IsActive = false;
                x.IsDeleted = true;
            });
            foreach (var item in listCategories)
            {
                _context.Entry(item).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(HttpStatusCode.MovedPermanently, true);
        }
    }
}