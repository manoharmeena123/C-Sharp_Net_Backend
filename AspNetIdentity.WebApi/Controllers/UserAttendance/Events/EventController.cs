using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.EventModel;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.UserAttendance.Events
{
    [Authorize]
    [RoutePrefix("api/events")]
    public class EventController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API TO CREATE EVENTS 
        /// <summary>
        /// Created By Harshit Mitra On 03-03-2023
        /// API >> POST >> api/events/createevents
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createevents")]
        public async Task<IHttpActionResult> CreateEvent(CreateEventRequest model)
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
                EventModel obj = new EventModel
                {
                    EventTitle = model.EventTitle,
                    EventDescription = model.EventDescription,
                    EventVenue = model.EventVenue,
                    EventTimeEnd = model.EventTimeEnd,
                    EventTimeStart = model.EventTimeStart,
                    EventDate = TimeZoneConvert.ConvertTimeToSelectedZone(model.EventDate, tokenData.TimeZone),

                    CompanyId = tokenData.companyId,
                    CreatedBy = tokenData.employeeId,
                    CreatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                };
                _db.EventModels.Add(obj);
                await _db.SaveChangesAsync();

                res.Message = "Event Add";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                res.Data = obj;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/events/createevents | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class CreateEventRequest
        {
            public string EventTitle { get; set; } = String.Empty;
            public string EventDescription { get; set; } = String.Empty;
            public string EventVenue { get; set; } = String.Empty;
            public TimeSpan EventTimeStart { get; set; } = TimeSpan.Zero;
            public TimeSpan EventTimeEnd { get; set; } = TimeSpan.Zero;
            public DateTime EventDate { get; set; } = DateTime.UtcNow;
        }
        #endregion

        #region API TO EDIT EVENTS 
        /// <summary>
        /// Created By Harshit Mitra On 03-03-2023
        /// API >> POST >> api/events/editevents
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("editevents")]
        public async Task<IHttpActionResult> EditEvents(EditEventRequest model)
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
                var events = await _db.EventModels
                    .FirstOrDefaultAsync(x => x.EventId == model.EventId);
                if (events == null)
                {
                    res.Message = "Event Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    return Ok(res);
                }
                events.EventTitle = model.EventTitle;
                events.EventDescription = model.EventDescription;
                events.EventVenue = model.EventVenue;
                events.EventTimeEnd = model.EventTimeEnd;
                events.EventTimeStart = model.EventTimeStart;
                events.EventDate = TimeZoneConvert.ConvertTimeToSelectedZone(model.EventDate, tokenData.TimeZone);

                events.UpdatedBy = tokenData.employeeId;
                events.UpdatedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);

                _db.Entry(events).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "Event Updated";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;
                res.Data = events;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/events/editevents | " +
                    "Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class EditEventRequest : CreateEventRequest
        {
            public Guid EventId { get; set; } = Guid.Empty;
        }
        #endregion

        #region API TO DELETE EVENT
        /// <summary>
        /// Created By Harshit Mitra On 03-03-2023
        /// API >> POST >> api/events/deleteevent
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deleteevent")]
        public async Task<IHttpActionResult> DeleteEvent(Guid eventId)
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
                var events = await _db.EventModels
                    .FirstOrDefaultAsync(x => x.EventId == eventId);
                if (events == null)
                {
                    res.Message = "Event Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    return Ok(res);
                }
                events.IsActive = false;
                events.IsDeleted = true;

                events.DeletedBy = tokenData.employeeId;
                events.DeletedOn = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);

                _db.Entry(events).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                res.Message = "Event Deleted";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/events/deleteevent | " +
                    "EventId : " + eventId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET EVENT BY ID
        /// <summary>
        /// Created By Harshit Mitra On 03-03-2023
        /// API >> POST >> api/events/geteventbyid
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("geteventbyid")]
        public async Task<IHttpActionResult> GetEventById(Guid eventId)
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
                var events = await _db.EventModels
                    .FirstOrDefaultAsync(x => x.EventId == eventId);
                if (events == null)
                {
                    res.Message = "Event Not Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    return Ok(res);
                }

                res.Message = "Event Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = events;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/events/geteventbyid | " +
                    "EventId : " + eventId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET ALL EVENT
        /// <summary>
        /// Created By Harshit Mitra On 03-03-2023
        /// API >> POST >> api/events/getallevents
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getallevents")]
        public async Task<IHttpActionResult> GetAllEvents(EventFilterModelRequet filter)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var events = await _db.EventModels
                    .Where(x => x.IsActive && !x.IsDeleted &&
                        x.CompanyId == tokenData.companyId)
                    .ToListAsync();
                if (events.Count == 0)
                {
                    res.Message = "No Event Are Created";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = events;
                    return Ok(res);
                }
                var eventData = FilterEventData(events, filter);
                var response = (from e in eventData
                                join ce in _db.Employee on e.CreatedBy equals ce.EmployeeId
                                join ue in _db.Employee on e.UpdatedBy equals ue.EmployeeId into upEmpty
                                from up in upEmpty.DefaultIfEmpty()
                                select new
                                {
                                    e.EventId,
                                    e.EventTitle,
                                    e.EventDescription,
                                    e.EventVenue,
                                    e.EventDate,
                                    e.EventTimeStart,
                                    e.EventTimeEnd,
                                    CreateBy = ce.DisplayName,
                                    CreatedOn = e.CreatedOn,
                                    UpdateBy = up != null ? up.DisplayName : String.Empty,
                                    UpdatedOn = up != null ? e.UpdatedOn : null,
                                })
                                .ToList();

                res.Message = "Event Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = response;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/events/getallevents | " +
                    //"EventId : " + eventId + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class EventFilterModelRequet
        {
            public FilterEvent Filter { get; set; } = FilterEvent.CreatedOn;
            public bool DescOrder { get; set; } = false;
        }
        public enum FilterEvent
        {
            CreatedOn = 0,
            EventName = 1,
            EventDate = 2,
        }
        public List<EventModel> FilterEventData(List<EventModel> list, EventFilterModelRequet filter)
        {
            switch (filter.DescOrder)
            {
                case true:
                    switch (filter.Filter)
                    {
                        case FilterEvent.CreatedOn:
                            list = list.OrderByDescending(x => x.CreatedOn).ToList();
                            break;
                        case FilterEvent.EventName:
                            list = list.OrderByDescending(x => x.EventTitle).ToList();
                            break;
                        case FilterEvent.EventDate:
                            list = list.OrderByDescending(x => x.EventDate.Date).ToList();
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
                        case FilterEvent.EventName:
                            list = list.OrderBy(x => x.EventTitle).ToList();
                            break;
                        case FilterEvent.EventDate:
                            list = list.OrderBy(x => x.EventDate.Date).ToList();
                            break;
                        default:
                            break;
                    }
                    break;
            }
            return list;
        }
        #endregion

        #region API TO GET CALANDER EVENTS WITH HOLIDAY MERGE (UNION TABLE DATA)
        /// <summary>
        /// Created By Harshit Mitra On 03-03-2023
        /// API >> GET >> api/events/getcalandermerge
        /// </summary>
        /// <param name="takeTop"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcalandermerge")]
        public async Task<IHttpActionResult> GetCalanderMerge(bool takeTop = false)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                Guid holidayGroup = _db.Employee.Where(x => x.EmployeeId == tokenData.employeeId).Select(x => x.HolidayGroupId).FirstOrDefault();
                var response = await GetCalanderMergeSP(
                    tokenData.companyId,
                    holidayGroup,
                    today.Date,
                    null
                    );

                res.Message = "Get Data";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                res.Data = new
                {
                    ListData = takeTop ?
                        response.Where(x => !x.CheckOldDate).ToList().Take(3) : response,
                    DateList = response.Select(x => new
                    {
                        Date = x.Select_Date.ToString("yyyy-MM-dd"),
                        Type = x.Type == MergeResponseType.Holiday,
                    }).ToList(),
                };


                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/events/createevents | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class EventHolidayMergeDataResponse
        {
            public MergeResponseType Type { get; set; }
            public string Title { get; set; }
            public string Details { get; set; }
            public DateTime Select_Date { get; set; }
            public string Venue { get; set; }
            public TimeSpan Time_Start { get; set; }
            public TimeSpan Time_End { get; set; }
            public string ImageUrl { get; set; }
            public bool CheckOldDate { get; set; }
            public bool IsToday { get; set; }
            public string Date { get; set; }
            public string Month { get; set; }
        }
        public enum MergeResponseType
        {
            Holiday = 1,
            Event = 2,
        }
        public async Task<List<EventHolidayMergeDataResponse>> GetCalanderMergeSP(int @companyId, Guid @groupId, DateTime @checkDate, DateTime? @selectDate)
        {
            List<EventHolidayMergeDataResponse> listData = new List<EventHolidayMergeDataResponse>();
            try
            {
                var _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                using (var con = new SqlConnection(_connectionString.ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SP_GetHolidayEvents", con);

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@companyId", System.Data.SqlDbType.Int).Value = @companyId;
                    cmd.Parameters.Add("@groupId", System.Data.SqlDbType.UniqueIdentifier).Value = @groupId;
                    cmd.Parameters.Add("@checkDate", System.Data.SqlDbType.Date).Value = @checkDate;
                    cmd.Parameters.Add("@selectDate", System.Data.SqlDbType.Date).Value = @selectDate;

                    con.Open();
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    while (rdr.Read())
                    {
                        listData.Add(new EventHolidayMergeDataResponse
                        {
                            Type = (MergeResponseType)Convert.ToInt32(rdr["Type"]),
                            Title = rdr["Title"].ToString(),
                            Details = rdr["Details"].ToString(),
                            Select_Date = Convert.ToDateTime(rdr["Select_Date"]),
                            Venue = rdr["Venue"].ToString(),
                            Time_Start = (TimeSpan)rdr["Time_Start"],
                            Time_End = (TimeSpan)rdr["Time_End"],
                            ImageUrl = rdr["ImageUrl"].ToString(),
                            CheckOldDate = Convert.ToBoolean(rdr["CheckOldDate"]),
                            IsToday = Convert.ToBoolean(rdr["IsToday"]),
                            Date = Convert.ToDateTime(rdr["Select_Date"]).ToString("dd"),
                            Month = Convert.ToDateTime(rdr["Select_Date"]).ToString("MMM"),
                        });
                    }
                    con.Close();
                }
                return listData;
            }
            catch (Exception)
            {
                return listData;
            }
        }
        #endregion

        #region API TO GET CALANDER EVENT ON BEHALF OF DATE (UNION TABLE DATA)
        /// <summary>
        /// Created By Harshit Mitra On 03-03-2023
        /// API >> GET >> api/events/getcalandermergebydate
        /// </summary>
        /// <param name="selectDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getcalandermergebydate")]
        public async Task<IHttpActionResult> GetCalanderMergeByDate(DateTime selectDate)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                Guid holidayGroup = _db.Employee.Where(x => x.EmployeeId == tokenData.employeeId).Select(x => x.HolidayGroupId).FirstOrDefault();
                var response = await GetCalanderMergeSP(
                    tokenData.companyId,
                    holidayGroup,
                    today.Date,
                    selectDate.Date
                    );

                res.Message = "Get Data";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                res.Data = response;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/events/createevents | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion
    }
}
