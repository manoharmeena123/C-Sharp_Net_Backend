using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.ShiftModel;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using static AspNetIdentity.WebApi.Controllers.DashboardController;

namespace AspNetIdentity.WebApi.Controllers.UserAttendance
{
    [Authorize]
    [RoutePrefix("api/holidays")]
    public class NewHolidayController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API TO CREATE HOLIDAY
        /// <summary>
        /// Created By Harshit Mitra On 06-02-20230
        /// API >> POST >> api/holidays/createholiday
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createholiday")]
        public async Task<IHttpActionResult> CreateHoliday(CreateHolidayRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }
                else
                {
                    HolidayModel obj = new HolidayModel
                    {
                        HolidayName = model.HolidayName,
                        Description = model.Description,
                        IsFloaterOptional = model.IsFloaterOptional,
                        ImageUrl = model.ImageUrl,
                        TextColor = model.TextColor,
                        HolidayDate = TimeZoneConvert.ConvertTimeToSelectedZone(model.HolidayDate.Date, tokenData.TimeZone),

                        CompanyId = tokenData.companyId,
                        CreatedBy = tokenData.employeeId,
                        CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                    };
                    _db.HolidayModels.Add(obj);
                    await _db.SaveChangesAsync();

                    res.Message = "Holiday Created";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Created;
                    res.Data = obj;

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/holidays/createholidaygroup | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class CreateHolidayRequest
        {
            public string HolidayName { get; set; } = String.Empty;
            public string Description { get; set; } = String.Empty;
            public bool IsFloaterOptional { get; set; } = false;
            public string ImageUrl { get; set; } = String.Empty;
            public string TextColor { get; set; } = String.Empty;
            public DateTime HolidayDate { get; set; } = DateTime.UtcNow;
        }
        #endregion

        #region API TO UPDATE HOLIDAY GROUP
        /// <summary>
        /// Created By Harshit Mitra On 06-02-2023
        /// API >> POST >> api/holidays/updateholiday
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updateholiday")]
        public async Task<IHttpActionResult> UpdateHoliday(UpdateHolidayRequest model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(message);
                }
                else
                {
                    var holiday = await _db.HolidayModels
                        .FirstOrDefaultAsync(x => x.HolidayId == model.HolidayId);
                    if (holiday == null)
                    {
                        res.Message = "Holiday Not Found";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        return Ok(res);
                    }
                    holiday.HolidayName = model.HolidayName;
                    holiday.Description = model.Description;
                    holiday.IsFloaterOptional = model.IsFloaterOptional;
                    holiday.ImageUrl = model.ImageUrl;
                    holiday.TextColor = model.TextColor;
                    holiday.HolidayDate = TimeZoneConvert.ConvertTimeToSelectedZone(model.HolidayDate.Date, tokenData.TimeZone);

                    holiday.UpdatedBy = tokenData.employeeId;
                    holiday.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);

                    _db.Entry(holiday).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    res.Message = "Holiday Group Updated";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.Accepted;
                    res.Data = holiday;

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error("API : api/holidays/updateholiday | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class UpdateHolidayRequest : CreateHolidayRequest
        {
            public Guid HolidayId { get; set; } = Guid.Empty;
        }
        #endregion

        #region API TO DELETE HOLIDAY
        /// <summary>
        /// Created By Harshit Mitra On 06-02-2023
        /// API >> POST >> api/holidays/deleteholiday
        /// </summary>
        /// <param name="holidayId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deleteholiday")]
        public async Task<IHttpActionResult> DeleteHolidayGroupById(Guid holidayId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var holiday = await _db.HolidayModels
                    .FirstOrDefaultAsync(x => x.HolidayId == holidayId);
                if (holiday == null)
                {
                    res.Message = "Holiday Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    return Ok(res);
                }
                holiday.IsActive = false;
                holiday.IsDeleted = true;

                holiday.DeletedBy = tokenData.employeeId;
                holiday.DeletedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);

                _db.Entry(holiday).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "Holiday Deleted";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/holidays/deleteholiday | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "HolidayId Id : " + holidayId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET HOLIDAY BY ID
        /// <summary>
        /// Created By Harshit Mitra On 06-02-2023
        /// API >> GET >> api/holidays/getholidaybyid
        /// </summary>
        /// <param name="holidayId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getholidaybyid")]
        public async Task<IHttpActionResult> GetHolidayGroupById(Guid holidayId)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var holiday = await _db.HolidayModels
                    .FirstOrDefaultAsync(x => x.HolidayId == holidayId && x.IsActive && !x.IsDeleted);
                if (holiday == null)
                {
                    res.Message = "Holiday Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    return Ok(res);
                }

                res.Message = "Holiday Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = holiday;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/holidays/getholidaybyid | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "HolidayId Id : " + holidayId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET ALL HOLIDAYS FILTER
        /// <summary>
        /// Created By Harshit Mitra On 06-02-2023
        /// API >> POST >> api/holidays/getallholidayfilter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getallholidayfilter")]
        public async Task<IHttpActionResult> GetHolidayGroupById(HolidayFilterRequest filter)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var holiday = await _db.HolidayModels
                    .Where(x => x.IsActive && !x.IsDeleted &&
                        x.CompanyId == tokenData.companyId)
                    .ToListAsync();
                if (holiday.Count == 0)
                {
                    res.Message = "Holiday Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    return Ok(res);
                }
                var holidayData = FilterHolidatData(holiday, filter);
                var response = (from h in holidayData
                                join ce in _db.Employee on h.CreatedBy equals ce.EmployeeId
                                join ue in _db.Employee on h.UpdatedBy equals ue.EmployeeId into upEmpty
                                from up in upEmpty.DefaultIfEmpty()
                                select new
                                {
                                    h.HolidayId,
                                    h.HolidayName,
                                    h.Description,
                                    h.IsFloaterOptional,
                                    HolidayDate = h.HolidayDate,
                                    h.ImageUrl,
                                    CreateBy = ce.DisplayName,
                                    CreatedOn = h.CreatedOn,
                                    UpdateBy = up != null ? up.DisplayName : String.Empty,
                                    UpdatedOn = up != null ? h.UpdatedOn : null,
                                })
                                .ToList();

                res.Message = "Holiday Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = response;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/holidays/getallholidayfilter | " +
                    "Model : " + JsonConvert.SerializeObject(filter) + " | " +
                    //"Group Id : " + holidayId + " | " +
                    //"Filter Model : " + filtermodel + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class HolidayFilterRequest
        {
            public FilterEvent Filter { get; set; } = FilterEvent.CreatedOn;
            public bool DescOrder { get; set; } = false;
        }
        public enum FilterEvent
        {
            CreatedOn = 0,
            HolidayName = 1,
            HolidayDate = 2,
        }
        public List<HolidayModel> FilterHolidatData(List<HolidayModel> list, HolidayFilterRequest filter)
        {
            switch (filter.DescOrder)
            {
                case true:
                    switch (filter.Filter)
                    {
                        case FilterEvent.CreatedOn:
                            list = list.OrderByDescending(x => x.CreatedOn).ToList();
                            break;
                        case FilterEvent.HolidayName:
                            list = list.OrderByDescending(x => x.HolidayName).ToList();
                            break;
                        case FilterEvent.HolidayDate:
                            list = list.OrderByDescending(x => x.HolidayDate.Date).ToList();
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    switch (filter.Filter)
                    {
                        case FilterEvent.CreatedOn:
                            list = list.OrderBy(x => x.CreatedOn).ToList();
                            break;
                        case FilterEvent.HolidayName:
                            list = list.OrderBy(x => x.HolidayName).ToList();
                            break;
                        case FilterEvent.HolidayDate:
                            list = list.OrderBy(x => x.HolidayDate.Date).ToList();
                            break;
                        default:
                            break;
                    }
                    break;
            }
            return list;
        }
        #endregion

        #region API TO GET HOLIDAY LIST ON HOME DASHBOARD
        /// <summary>
        /// Created By Harshit Mitra On 08-02-2023
        /// API >> GET >> api/holidays/getholidayinhomedahboard
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getholidayinhomedahboard")]
        public async Task<IHttpActionResult> GetHolidayInHomeDahboard()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                var holidayDataList = await (from emp in _db.Employee
                                             join g in _db.HolidayGroups on emp.HolidayGroupId equals g.GroupId
                                             join hg in _db.HolidayInGroups on g.GroupId equals hg.GroupId
                                             join h in _db.HolidayModels on hg.HolidayId equals h.HolidayId
                                             where g.GroupId == emp.HolidayGroupId && emp.EmployeeId == tokenData.employeeId
                                             select h)
                                             .OrderBy(x => x.HolidayDate)
                                             .ToListAsync();
                var checkList = holidayDataList
                    .OrderBy(x => x.HolidayDate)
                    .Select(x => new GetDashboardHolidayListResponse
                    {
                        HolidayName = x.HolidayName,
                        ImageUrl = x.ImageUrl,
                        TextColor = x.TextColor,
                        IsFloaterOptional = x.IsFloaterOptional,
                        MonthName = x.HolidayDate.ToString("MMMM"),
                        StartDate = x.HolidayDate.ToString("dd"),
                        DayName = x.HolidayDate.ToString("dddd"),
                        IsPrevious = (x.HolidayDate.Date < today.Date)
                    })
                    .ToList();
                res.Message = "Holiday List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = checkList;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/holidays/getholidayinhomedahboard | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class GetDashboardHolidayListResponse : HolidayOnHomeResponse
        {
            public bool IsPrevious { get; set; } = false;
        }
        #endregion
    }
}
