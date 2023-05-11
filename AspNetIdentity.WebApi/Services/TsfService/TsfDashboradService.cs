using AspNetIdentity.Core.Common;
using AspNetIdentity.Core.Enum;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Infrastructure.ITsfService;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.EventModel;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;
using Res = AspNetIdentity.Core.ViewModel.TsfViewModel.ReponseTsfDashboradClass;

namespace AspNetIdentity.WebApi.Services.TsfService
{
    public class TsfDashboradService : ITsfDashboradService
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        private readonly Logger _logger;
        #endregion

        #region Constructor
        public TsfDashboradService()
        {
            _context = new ApplicationDbContext();
            _logger = LogManager.GetCurrentClassLogger();
        }
        #endregion

        #region This service Class Use to Get all tsf Dashboard Data
        public async Task<ServiceResponse<Res.ResponseTsfDashboradClass>> GetDasboradData(ClaimsHelperModel tokenData)
        {
            try
            {
                DateTime currentDate = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow).Date;
                List<DateTime> dateList = new List<DateTime>();

                for (int i = 1; i <= 7; i++)
                    dateList.Add(currentDate.AddDays(i));

                List<Employee> employeeData = await _context.Employee
                    .Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId)
                    .ToListAsync();

                List<Res.ResponseBdayEmployeeClass> bdayList =
                    employeeData
                    .Where(x => x.DateOfBirth.Day == currentDate.Day &&
                        x.DateOfBirth.Month == currentDate.Month && !x.HideDOB)
                    .Select(x => new Res.ResponseBdayEmployeeClass
                    {
                        EmployeeId = x.EmployeeId,
                        EmployeeName = x.DisplayName,
                        DOB = x.DateOfBirth,
                        HideDOB = x.HideDOB,
                        UserImage = x.ProfileImageUrl,
                        DepartmentName = _context.Department.Where(y => y.DepartmentId == x.DepartmentId)
                                        .Select(y => y.DepartmentName).FirstOrDefault(),
                    })
                   .ToList();

                List<EventModel> eventData =
                    await _context.EventModels
                    .Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == tokenData.companyId)
                    .ToListAsync();
                List<Res.ResponseEventClass> eventList = eventData
                    .Select(x => new Res.ResponseEventClass
                    {
                        EventTitle = x.EventTitle,
                        EventVenue = x.EventVenue,
                        EventTimeStart = x.EventTimeStart.ToString(@"hh\:mm"),
                        EventTimeEnd = x.EventTimeEnd.ToString(@"hh\:mm"),
                        EventMonth = x.EventDate.ToString(@"MMM"),
                        EventDate = x.EventDate.ToString(@"dd"),
                    })
                    .ToList();

                //List<Res.ResponseCompanyNewsClass> companyNews =
                //    await _context.NewsEntities
                //    .Where(x => !x.IsDeleted && x.CompanyId == tokenData.companyId && x.IsActive)
                //    .OrderByDescending(x => x.CreatedOn)
                //    .Take(3)
                //    .Select(x => new Res.ResponseCompanyNewsClass
                //    {
                //        NewsId = x.NewsId,
                //        News = x.Title,
                //        Image = x.Image,
                //        NewsHeading = x.Description,
                //    })
                //    .ToListAsync();

                List<string> profileData =
                    employeeData
                    .Where(x => !string.IsNullOrEmpty(x.ProfileImageUrl))
                    .Select(x => x.ProfileImageUrl)
                    .ToList();

                Res.ResponseTsfDashboradClass returnObj = new Res.ResponseTsfDashboradClass()
                {
                    CurrentBirthdatList = bdayList,
                    EventList = eventList,
                    //CompanyNewsList = companyNews,
                    TopBlogList = await GetLatestBlogListDashboard(tokenData.companyId, tokenData.employeeId),
                    TopNewsList = await GetFeatureListOnDashboard(tokenData.companyId, tokenData.employeeId),
                    ProfileImageList = profileData
                };
                return new ServiceResponse<Res.ResponseTsfDashboradClass>(HttpStatusCode.OK, returnObj);
            }
            catch (Exception ex)
            {
                _logger.Error("API : api/tsfdashborad/test | " +
                "Exception : " + JsonConvert.SerializeObject(ex));
                throw ex;
            }
        }
        #endregion
        public async Task<List<Res.GetLatestBlogDashboardResponse>> GetLatestBlogListDashboard(int companyId, int employeeId)
        {
            var blogListTop = await (from b in _context.Blogs
                                     join c in _context.BlogCategories on b.CategoryId equals c.CategoryId
                                     join l in _context.BlogsLikesEntities on b.BlogId equals l.BlogId into empty
                                     from like in empty.DefaultIfEmpty()
                                     where b.IsActive && !b.IsDeleted && b.CompanyId == companyId &&
                                        b.BlogsStatus == Core.Enum.BlogsTypeEnumClass.Live
                                     select new Res.GetLatestBlogDashboardResponse
                                     {
                                         BlogId = b.BlogId,
                                         CategoryName = c.CategoryName,
                                         ActionDate = b.ActionDate,
                                         Title = b.Title,
                                         Image = b.Image,
                                         Description = b.Description,
                                     })
                                     .OrderByDescending(x => x.ActionDate)
                                     .Take(2)
                                     .ToListAsync();
            var blogsIdList = blogListTop.Select(x => x.BlogId).Distinct().ToList();
            var likesList = await _context.BlogsLikesEntities
               .Where(x => blogsIdList.Contains(x.BlogId))
               .ToListAsync();
            blogListTop.ForEach(x =>
            {
                x.LikeCount = likesList.AsEnumerable().LongCount(z => x.BlogId == z.BlogId);
                x.IsLikedByUser = likesList.AsEnumerable().Any(z => x.BlogId == z.BlogId && z.EmployeeId == employeeId);
            });
            return blogListTop;
        }
        public async Task<List<Res.GetLatestNewsOnDashboardResponse>> GetFeatureListOnDashboard(int companyId, int employeeId)
        {
            List<Res.GetLatestNewsOnDashboardResponse> response =
                await (from n in _context.NewsEntities
                       join c in _context.NewsCategories on n.CategoryId equals c.CategoryId
                       where n.CompanyId == companyId && n.IsActive && !n.IsDeleted
                           && n.NewsType == NewsEnumType.Publish && n.IsFeatured
                       select new Res.GetLatestNewsOnDashboardResponse
                       {
                           Image = n.Image,
                           NewsId = n.NewsId,
                           Title = n.Title,
                           Description = n.Description,
                           CategoryName = c.CategoryName,
                           CreatedOn = n.CreatedOn,
                           LikeCount = _context.NewsLikeEntities.LongCount(x => x.NewsId == n.NewsId),
                           IsLikedByUser = _context.NewsLikeEntities.Any(x => x.NewsId == n.NewsId && x.EmployeeId == employeeId),
                       })
                       .OrderByDescending(x => x.CreatedOn)
                       .ToListAsync();
            return response;
        }
    }
}