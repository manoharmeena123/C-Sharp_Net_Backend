using AspNetIdentity.Core.Common;
using AspNetIdentity.Core.Model.TsfModule.CircularsNotices;
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
using Req = AspNetIdentity.Core.ViewModel.CircularNoticeViewModel.RequestCircularNoticeViewModel;
using Res = AspNetIdentity.Core.ViewModel.CircularNoticeViewModel.ResponseCircularNoticeViewModel;

namespace AspNetIdentity.WebApi.Services.CircularNoticesService
{
    public class CircularsNoticesService : ICircularsNoticesService
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        private readonly IShortByService<CircularsNoticesEntities> _shortByService;
        #endregion

        #region Constructor
        public CircularsNoticesService()
        {
            _context = new ApplicationDbContext();
            _shortByService = new ShortByService<CircularsNoticesEntities>();
        }
        #endregion


        /// <summary>
        /// Created By Harshit Mitra On 27-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<CircularsNoticesEntities>> Create(ClaimsHelperModel tokenData, Req.CreateRequest model, CircularsNoticesType type)
        {
            DateTimeOffset today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
            CircularsNoticesEntities obj = new CircularsNoticesEntities()
            {
                Title = model.Title,
                Icon = model.Icon,
                DocumentUrl = model.DocumentUrl,
                Type = type,

                CreatedOn = today,
                CreatedBy = tokenData.employeeId,
                CompanyId = tokenData.companyId,
            };
            _context.CircularsNoticesEntities.Add(obj);
            await _context.SaveChangesAsync();
            return new ServiceResponse<CircularsNoticesEntities>(HttpStatusCode.Created, obj);
        }


        /// <summary>
        /// Created By Harshit Mitra On 28-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<Res.AdminResponse>>> GetOnAdminView(ClaimsHelperModel tokenData, Req.AdminRequest model, CircularsNoticesType type)
        {
            List<Res.AdminResponse> response = await _context.CircularsNoticesEntities
                .Where(x => x.IsActive && !x.IsDeleted && x.Type == type)
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new Res.AdminResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    Icon = x.Icon,
                    DocumentUrl = x.DocumentUrl,
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn,
                })
                .ToListAsync();
            if (model.SearchString.Length > 2)
                response = response
                    .Where(x => x.Title.ToLower().Contains(model.SearchString.ToLower()))
                    .ToList();
            if (response.Count != 0)
                return new ServiceResponse<List<Res.AdminResponse>>(HttpStatusCode.OK, response);
            return new ServiceResponse<List<Res.AdminResponse>>(HttpStatusCode.NoContent, response, false);
        }


        /// <summary>
        /// Created By Harshit Mitra On 28-04-2023
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> MultiSelectDelete(ClaimsHelperModel tokenData, Req.MultiSelectSelectRequest model, CircularsNoticesType type)
        {
            DateTimeOffset today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
            if (model.Ids.Length == 0)
                return new ServiceResponse<bool>(HttpStatusCode.BadRequest, false, false, "Required To Select Any One Circular");
            var responseList = await _context.CircularsNoticesEntities
                .Where(x => model.Ids.Contains(x.Id) && x.Type == type)
                .ToListAsync();
            foreach (var item in responseList)
            {
                item.DeletedOn = today;
                item.DeletedBy = tokenData.employeeId;
                item.IsActive = false;
                item.IsDeleted = true;

                _context.Entry(item).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            return new ServiceResponse<bool>(HttpStatusCode.MovedPermanently, true);
        }


        /// <summary>
        /// Created By Harshit Mitra On 28-04-2023
        /// </summary>
        /// <param name="model"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<CircularsNoticesEntities>> GetDataById(Req.GetByIdRequest model, CircularsNoticesType type)
        {
            CircularsNoticesEntities response = await _context.CircularsNoticesEntities
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.Type == type);
            if (response != null)
                return new ServiceResponse<CircularsNoticesEntities>(HttpStatusCode.OK, response);
            return new ServiceResponse<CircularsNoticesEntities>(HttpStatusCode.NotFound, response, false);
        }


        /// <summary>
        /// Created By Harshit Mitra On 28-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> Update(ClaimsHelperModel tokenData, Req.UpdateRequest model, CircularsNoticesType type)
        {
            DateTimeOffset today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
            CircularsNoticesEntities dataForUpdate = await _context.CircularsNoticesEntities
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.Type == type);
            if (dataForUpdate == null)
                return new ServiceResponse<bool>(HttpStatusCode.NotFound, false, false);
            dataForUpdate.UpdatedOn = today;
            dataForUpdate.UpdatedBy = tokenData.employeeId;

            dataForUpdate.Title = model.Title;
            dataForUpdate.Icon = model.Icon;
            dataForUpdate.DocumentUrl = model.DocumentUrl;

            _context.Entry(dataForUpdate).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool>(HttpStatusCode.Accepted, true);
        }

        /// <summary>
        /// Created By Harshit Mitra On 28-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<List<Res.GetCountOnUser>>> GetCountOnUser(ClaimsHelperModel tokenData, RequestShortBy model)
        {
            DateTimeOffset today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
            List<Res.GetCountOnUser> response = new List<Res.GetCountOnUser>()
            {
                new Res.GetCountOnUser()
                {
                    TypeId = 0,
                    Name = "All",
                    Count = 0,
                },
            };

            List<CircularsNoticesEntities> listEntities = await _context.CircularsNoticesEntities
                .Where(x => x.CompanyId == tokenData.companyId && x.IsActive && !x.IsDeleted)
                .ToListAsync();
            if (model.ShortBy.HasValue)
                _shortByService.ShortValue(model.ShortBy.Value, today.Date, ref listEntities);

            var types = Enum.GetValues(typeof(CircularsNoticesType))
                                .Cast<CircularsNoticesType>()
                                .Select(x => new Res.GetCountOnUser
                                {
                                    TypeId = (int)x,
                                    Name = x.ToString().Replace("_", " "),
                                    Count = listEntities.LongCount(z => z.Type == x),

                                })
                                .ToList();
            response[0].Count = listEntities.LongCount();
            response.AddRange(types);
            return new ServiceResponse<List<Res.GetCountOnUser>>(HttpStatusCode.OK, response);
        }


        /// <summary>
        /// Created By Harshit Mitra On 28-04-2023
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<PagingResponse<Res.UserViewResponse>>> GetUserView(ClaimsHelperModel tokenData, Req.UserRequest model)
        {
            List<Res.UserViewResponse> response = new List<Res.UserViewResponse>();
            DateTimeOffset today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
            List<CircularsNoticesEntities> listEntities = await _context.CircularsNoticesEntities
               .Where(x => x.CompanyId == tokenData.companyId && x.IsActive && !x.IsDeleted)
               .ToListAsync();
            if (model.ShortBy.HasValue)
                _shortByService.ShortValue(model.ShortBy.Value, today.Date, ref listEntities);
            if (model.TypeId == 0)
                response = listEntities
                    .Select(x => new Res.UserViewResponse
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Icon = x.Icon,
                        DocumentUrl = x.DocumentUrl,
                    })
                    .ToList();
            else
                response = listEntities
                    .Where(x => x.Type == (CircularsNoticesType)model.TypeId)
                    .Select(x => new Res.UserViewResponse
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Icon = x.Icon,
                        DocumentUrl = x.DocumentUrl,
                    })
                    .ToList();
            var pagingResponse = new PagingResponse<Res.UserViewResponse>(response, model.Page, model.Count);
            if (response.Count != 0)
                return new ServiceResponse<PagingResponse<Res.UserViewResponse>>(HttpStatusCode.OK, pagingResponse);
            return new ServiceResponse<PagingResponse<Res.UserViewResponse>>(HttpStatusCode.NoContent, pagingResponse, false);
        }
    }
}