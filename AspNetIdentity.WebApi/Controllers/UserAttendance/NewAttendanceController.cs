using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
using AspNetIdentity.WebApi.Model.NewUserAttendance;
using AspNetIdentity.WebApi.Model.ShiftModel;
using AspNetIdentity.WebApi.Models;
using Dommel;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static AspNetIdentity.WebApi.Controllers.Asset.AssetsController;
using static AspNetIdentity.WebApi.Helper.ClaimsHelper;

namespace AspNetIdentity.WebApi.Controllers.UserAttendance
{
    /// <summary>
    /// Created By Harshit Mitra On 23-01-2023
    /// </summary>
    [Authorize]
    [RoutePrefix("api/attendance")]
    public class NewAttendanceController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API TO CHECK IN AND CHECK OUT EMPLOYEE  
        /// <summary>
        /// Created By Harshit Mitra On 23-01-2023
        /// API >> POST >> api/attendance/checkincheckout
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("checkincheckout")]
        public async Task<IHttpActionResult> CheckInCheckOut()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                DateTimeOffset today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                var checkShift = await _db.ShiftTimings
                    .Include("ShiftGroup")
                    .FirstOrDefaultAsync(x => x.WeekDay == today.DayOfWeek &&
                        x.ShiftGroup.ShiftGoupId == tokenData.EmpShiftGroup);
                if (checkShift != null)
                {
                    var startTime = TimeSpan.FromMinutes(checkShift.StartTime.TotalMinutes - (4 * 60));
                    if (checkShift.StartTime > checkShift.EndTime && today.TimeOfDay < startTime)
                        today = today.AddDays(-1);
                }
                var userAttendance = await _db.UserAttendances
                    .FirstOrDefaultAsync(x => x.Year == today.Year && x.Month == today.Month && x.EmployeeId ==
                            tokenData.employeeId && x.CompanyId == tokenData.companyId);
                if (userAttendance == null)
                    userAttendance = await CreateNewAttendance(today, tokenData);
                else
                {
                    var monthLogList = JsonConvert.DeserializeObject<List<MonthlyLog>>(userAttendance.MonthLogs);
                    var monthLog = monthLogList.FirstOrDefault(x => x.Date.Date == today.Date);
                    if (monthLog == null)
                        userAttendance = await CreateDailyAttendance(today, userAttendance);
                    else
                    {
                        var dailyLogList = monthLog.DailyLogs;
                        var dailyLog = dailyLogList.OrderBy(x => x.Id).LastOrDefault();
                        if (!dailyLog.IsCheckIn)
                            userAttendance = await CreateCheckInDailyLog(today, userAttendance);
                        else
                            userAttendance = await CreateCheckOutDailyLog(today, userAttendance);
                    }
                }
                res.Message = "Log Updated";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;
                res.Data = userAttendance;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/attendance/checkincheckout | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public async Task<UserAttendanceModel> CreateNewAttendance(DateTimeOffset today, ClaimsHelperModel tokenData)
        {
            UserAttendanceModel obj = new UserAttendanceModel
            {
                Month = today.Month,
                Year = today.Year,
                EmployeeId = tokenData.employeeId,
                CompanyId = tokenData.companyId,
                CreatedBy = tokenData.employeeId,
                CreatedOn = today,
                MonthLogs = JsonConvert.SerializeObject(
                    new List<MonthlyLog>()
                    {
                        new MonthlyLog
                        {
                            Date = today.Date,
                            CaseId = AttendanceCase.Log,
                            DailyLogs = new List<DailyLog>()
                            {
                                new DailyLog
                                {
                                    CheckInTime = today,
                                    IsCheckIn = true,
                                    DailyCaseId = DailyCase.Present_BAR_Missing_Swipe,
                                    
                                }
                            },
                        },

                    }),
                //UserLocation = JsonConvert.SerializeObject(
                //    new List<MonthlyLogImg>()
                //    {
                //        new MonthlyLogImg
                //        {
                //            Date = today.Date,
                //            CaseId = AttendanceCase.Log,
                //            DailyLogs = new List<DailyLogImg>()
                //            {
                //                new DailyLogImg
                //                {
                //                    CheckInTime = today,
                //                    IsCheckIn = true,
                //                    DailyCaseId = DailyCase.Present_BAR_Missing_Swipe,
                //                    Lat = 0.0,
                //                    Lng = 0.0,
                //                    ImgURlCheckIn = string.Empty
                //                }


                //            },
                //        },

                //    }),
            };
            _db.UserAttendances.Add(obj);
            await _db.SaveChangesAsync();
            return obj;
        }
        public async Task<UserAttendanceModel> CreateDailyAttendance(DateTimeOffset today, UserAttendanceModel userAttendance)
        {
            var monthLogList = JsonConvert.DeserializeObject<List<MonthlyLog>>(userAttendance.MonthLogs);
            monthLogList.Add(
                new MonthlyLog
                {
                    Date = today.Date,
                    CaseId = AttendanceCase.Log,
                    DailyLogs = new List<DailyLog>()
                    {
                        new DailyLog
                        {
                            CheckInTime = today,
                            IsCheckIn = true,
                            DailyCaseId = DailyCase.Present_BAR_Missing_Swipe,
                        }
                    },
                });
            //userAttendance.UserLocation = JsonConvert.SerializeObject(monthLogList);
            userAttendance.MonthLogs = JsonConvert.SerializeObject(monthLogList);
            _db.Entry(userAttendance).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return userAttendance;
        }
        public async Task<UserAttendanceModel> CreateCheckInDailyLog(DateTimeOffset today, UserAttendanceModel userAttendance)
        {
            var monthLogList = JsonConvert.DeserializeObject<List<MonthlyLog>>(userAttendance.MonthLogs);
            //var imgLogList = JsonConvert.DeserializeObject<List<MonthlyLogImg>>(userAttendance.MonthLogs);
            var monthLog = monthLogList.FirstOrDefault(x => x.Date.Date == today.Date);
            var newDailyLog = new DailyLog
            {
                Id = monthLog.DailyLogs.Count(),
                CheckInTime = today,
                IsCheckIn = true,
                DailyCaseId = DailyCase.Present_BAR_Missing_Swipe,

            };
            
            monthLog.DailyLogs.Add(newDailyLog);
            userAttendance.MonthLogs = JsonConvert.SerializeObject(monthLogList);
            //userAttendance.UserLocation = JsonConvert.SerializeObject(imgLogList);
            _db.Entry(userAttendance).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return userAttendance;
        }
        public async Task<UserAttendanceModel> CreateCheckOutDailyLog(DateTimeOffset today, UserAttendanceModel userAttendance)
        {
            List<MonthlyLog> monthLogList = JsonConvert.DeserializeObject<List<MonthlyLog>>(userAttendance.MonthLogs);
            MonthlyLog monthLog = monthLogList.FirstOrDefault(x => x.Date.Date == today.Date);
            DailyLog currentLog = monthLog.DailyLogs.FirstOrDefault(x => x.IsCheckIn);

            var indexDailyLog = monthLog.DailyLogs.FindIndex(x => x.Id == currentLog.Id);
            if (indexDailyLog != -1)
            {
                monthLog.DailyLogs[indexDailyLog] = new DailyLog
                {
                    Id = currentLog.Id,
                    DailyCaseId = DailyCase.Present,
                    IsCheckIn = false,
                    CheckOutTime = today,
                    CheckInTime = currentLog.CheckInTime,
                };
            }
            var indexMonthlyLog = monthLogList.FindIndex(x => x.Date == today.Date);
            if (indexMonthlyLog != -1)
            {
                monthLogList[indexMonthlyLog] = new MonthlyLog
                {
                    Date = monthLog.Date,
                    CaseId = monthLog.CaseId,
                    TotalWorkingTime = (monthLog.TotalWorkingTime + (today - currentLog.CheckInTime)),
                    CaseStatus = false,
                    DailyLogs = monthLog.DailyLogs,
                };
            }
            userAttendance.MonthLogs = JsonConvert.SerializeObject(monthLogList);
            //userAttendance.UserLocation = JsonConvert.SerializeObject(monthLogList);

            _db.UserAttendances.Attach(userAttendance);
            _db.Entry(userAttendance).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return userAttendance;
        }
        #endregion

        #region API TO GET CHECK ATTENDANCE 
        /// <summary>
        /// Created By Harshit Mitra On 30-01-2023
        /// API >> POST >> api/attendance/getcheckincheckout
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getcheckincheckout")]
        public async Task<IHttpActionResult> GetCheckInCheckOut()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                DateTimeOffset today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                var checkShift = await _db.ShiftTimings
                    .Include("ShiftGroup")
                    .FirstOrDefaultAsync(x => x.WeekDay == today.DayOfWeek &&
                        x.ShiftGroup.ShiftGoupId == tokenData.EmpShiftGroup);
                if (checkShift != null)
                {
                    if (checkShift.StartTime > checkShift.EndTime)
                        today = today.AddDays(-1);
                }
                var userAttendance = await _db.UserAttendances
                    .FirstOrDefaultAsync(x => x.Year == today.Year && x.Month == today.Month && x.EmployeeId ==
                            tokenData.employeeId && x.CompanyId == tokenData.companyId);
                if (userAttendance == null)
                {
                    res.Message = "";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = false;
                    return Ok(res);
                }
                var monthLogList = JsonConvert.DeserializeObject<List<MonthlyLog>>(userAttendance.MonthLogs);
                var monthLog = monthLogList.FirstOrDefault(x => x.Date.Date == today.Date);
                if (monthLog == null)
                {
                    res.Message = "";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = false;
                }
                else
                {
                    var dailyLogList = monthLog.DailyLogs;
                    var dailyLog = dailyLogList.OrderBy(x => x.Id).LastOrDefault();
                    res.Message = "";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = dailyLog.IsCheckIn;
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/attendance/getcheckincheckout | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest();
            }
        }
        #endregion

        #region API TO GET MONTH LIST TILL 5 MONTH AFTER CURRENT
        /// <summary>
        /// Created By Harshit Mitra On 27-01-2023
        /// API >> GET >> api/attendance/getmonthlist
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getmonthlist")]
        public IHttpActionResult GetMonthListOnAttendance()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<MonthResponseClass> responseList = new List<MonthResponseClass>();
                responseList.Add(new MonthResponseClass
                {
                    MonthId = 0,
                    MonthName = "Current",
                });
                DateTimeOffset today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                for (int i = -1; i != -7; i--)
                {
                    MonthResponseClass obj = new MonthResponseClass
                    {
                        MonthId = i,
                        MonthName = today.AddMonths(i).ToString("MMM"),
                    };
                    responseList.Add(obj);
                }
                res.Message = "Month List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = responseList;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/attendance/getmonthlist | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class MonthResponseClass
        {
            public int MonthId { get; set; } = 0;
            public string MonthName { get; set; } = "Current";
        }
        #endregion

        #region API TO GET USER SELF ATTENDANCE LOG 
        /// <summary>
        /// Created By Harshit Mitra On 27-01-2023
        /// API >> GET >> api/attendance/getuserattendancelog
        /// </summary>
        /// <param name="monthCount"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getuserattendancelog")]
        public async Task<IHttpActionResult> GetUserAttendanceLog(int monthCount = 0)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                DateTimeOffset today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone).AddMonths(monthCount);
                var userAttendance = await (from x in _db.UserAttendances
                                            join e in _db.Employee on x.EmployeeId equals e.EmployeeId
                                            where x.Year == today.Year && x.Month == today.Month && x.EmployeeId ==
                                                 tokenData.employeeId && x.CompanyId == tokenData.companyId
                                            select new
                                            {
                                                x.Year,
                                                x.Month,
                                                x.EmployeeId,
                                                e.DisplayName,
                                                x.MonthLogs,
                                                //x.UserLocation
                                            })
                                            .FirstOrDefaultAsync();

                var dateList = new List<DateTime>();
                int length = monthCount == 0 ? today.Day : DateTime.DaysInMonth(today.Year, today.Month);
                for (int i = 1; i <= length; i++)
                    dateList.Add(new DateTime(today.Year, today.Month, i));
                dateList = dateList.OrderByDescending(x => x.Date).ToList();

                if (userAttendance == null)
                {
                    res.Message = "Log Found";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = new UserLogResponse
                    {
                        MonthYear = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(today.Month) + ", " + today.Year,
                        TableResponse = (from d in dateList.AsEnumerable()
                                         select new UserLogTableResponse
                                         {
                                             Date = d.ToString("MMM dd, ddd"),
                                             Case = string.Empty,
                                             TotalEntries = TimeSpan.Zero.ToString(),
                                             Arrivals = "Absent",
                                             Logs = new List<UserTableInnerLogResponse>(),
                                         })
                                         .ToList(),
                    };
                    return Ok(res);
                }

                var holiday = await _db.Holidays.Where(x => x.HolidayDate.Month == today.Month && x.HolidayDate.Year == today.Year).ToListAsync();
                var weekOff = await _db.WeekOffDaysCases.Include("Group").Where(x => x.Group.WeekOffId == tokenData.EmployeeWeekOff).ToListAsync();
                var leave = await _db.LeaveRequests.Where(x => x.FromDate.Month == today.Month && x.FromDate.Year == today.Year).ToListAsync();
                var monthlyLogs = JsonConvert.DeserializeObject<List<MonthlyLog>>(userAttendance.MonthLogs);
                //var selfiLogs = JsonConvert.DeserializeObject<List<MonthlyLogImg>>(userAttendance.UserLocation);
                var getLogs = new UserLogResponse
                {
                    MonthYear = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(userAttendance.Month) + ", " + userAttendance.Year,
                    TableResponse = (from d in dateList.AsEnumerable()
                                     join ed in monthlyLogs.AsEnumerable() on d.Date equals ed.Date.Date into empMonthlyLog
                                     from x in empMonthlyLog.DefaultIfEmpty()
                                         //join sl in selfiLogs.AsEnumerable() on d.Date equals sl.Date.Date into imgMonthlylog
                                         //from slx in imgMonthlylog.DefaultIfEmpty()
                                     select new UserLogTableResponse
                                     {
                                         Date = d.ToString("MMM dd, ddd"),
                                         Case = string.Empty,
                                         TotalEntries = x == null ? TimeSpan.Zero.ToString()
                                            : String.Format("{0:00}:{1:00}:{2:00}",
                                              x.TotalWorkingTime.Hours, x.TotalWorkingTime.Minutes, x.TotalWorkingTime.Seconds),
                                         Arrivals = x == null ? "Absent" :
                                            (x.DailyLogs.Count == 0 ? "Absent" : x.DailyLogs.OrderBy(z => z.Id).Select(z => z.CheckInTime.ToString("T")).FirstOrDefault()),
                                         Logs = x == null ? new List<UserTableInnerLogResponse>() :
                                            x.DailyLogs
                                            .Select(z => new UserTableInnerLogResponse
                                            {
                                                CheckInTime = z.CheckInTime.ToString("T"),
                                                CheckOutTime = x.IsAutoCheckOut || z.IsCheckIn ? "Missing.!" : z.CheckOutTime.ToString("T")
                                            }).ToList()
                                     })
                                     .ToList(),
                };

                res.Message = "Log Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = getLogs;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/attendance/getuserattendancelog | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class UserLogResponse
        {
            public string MonthYear { get; set; } = String.Empty;
            public List<UserLogTableResponse> TableResponse { get; set; }
        }
        public class UserLogTableResponse
        {
            public string Date { get; set; } = String.Empty;
            public string Case { get; set; } = String.Empty;
            public string TotalEntries { get; set; } = String.Empty;
            public string Arrivals { get; set; } = String.Empty;
            public List<UserTableInnerLogResponse> Logs { get; set; }
        }
        public class UserTableInnerLogResponse
        {
            public string CheckInTime { get; set; }
            public string CheckOutTime { get; set; }
        }
        public AttendanceCase CheckCase(DateTimeOffset date, List<Holiday> holidayModel, List<WeekOffDaysCases> weekOffs)
        {
            if (holidayModel.Any(x => x.HolidayDate.Date == date.Date))
                return AttendanceCase.Holiday;
            if (weekOffs.Count > 0)
            {

            }
            return AttendanceCase.Log;
        }
        #endregion

        #region API TO GET USER WEEK ATTENDANCE DETAILS BOX
        /// <summary>
        /// Created By Harshit Mitra On 27-01-2023
        /// API >> GET >> api/attendance/getuserattencandedetailsbox
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getuserattencandedetailsbox")]
        public async Task<IHttpActionResult> GetUserAttencandeDetailsBox()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                var lastmonth = today.AddMonths(-1);
                var weekData = Enum.GetValues(typeof(DayOfWeek))
                    .Cast<DayOfWeek>()
                    .Select(x => new
                    {
                        DayOfWeek = x,
                        Key = (int)x,
                        Value = x.ToString()
                    })
                    .OrderBy(x => ((int)x.DayOfWeek + 6) % 7)
                    .ToList();
                var weekOff = await _db.WeekOffDaysCases
                    .Include("Group")
                    .Where(x => x.Group.WeekOffId == tokenData.EmployeeWeekOff)
                    .ToListAsync();
                var shift = await _db.ShiftTimings
                    .Include("ShiftGroup")
                    .Where(x => x.ShiftGroup.ShiftGoupId == tokenData.EmpShiftGroup)
                    .ToListAsync();
                var responseData = weekData
                    .Select(x => new
                    {
                        x.Key,
                        x.Value,
                        IsCurrent = today.DayOfWeek == x.DayOfWeek,
                        IsWeekOff = CheckWeekOff(x.DayOfWeek, today, weekOff).IsWeekOff,
                    })
                    .ToList();
                var shifData = shift
                    .Where(x => x.WeekDay == today.DayOfWeek)
                    .Select(x => new
                    {
                        x.StartTime,
                        x.EndTime,
                        x.BreakTime,
                    })
                    .FirstOrDefault();

                LastSevenDaysResponse dayData = new LastSevenDaysResponse();
                var userAttendance = await _db.UserAttendances
                    .Where(x => x.EmployeeId == tokenData.employeeId &&
                        (x.Month == today.Month && x.Year == today.Year) ||
                        (x.Month == lastmonth.Month && x.Year == lastmonth.Year))
                    .ToListAsync();
                if (userAttendance.Count != 0)
                {
                    var monthLogs = userAttendance
                        .SelectMany(x => JsonConvert.DeserializeObject<List<MonthlyLog>>(x.MonthLogs))
                        .OrderByDescending(x => x.Date)
                        .Where(x => x.Date.Date < today.Date)
                        .Take(7)
                        .ToList();
                    var onArivalTime = 0;
                    TimeSpan totalTime = new TimeSpan();
                    for (int i = 0; i > -7; i--)
                    {
                        var timeLogs = monthLogs
                            .Where(x => x.Date.Date == today.AddDays(i).Date)
                            .FirstOrDefault();
                        if (timeLogs != null)
                        {
                            totalTime += timeLogs.TotalWorkingTime;
                            onArivalTime += timeLogs.DailyLogs.First().CheckInTime.TimeOfDay <= shifData.StartTime ? 1 : 0;
                        }
                    }
                    dayData.AverageTime = new TimeSpan((long)Math.Round(((double)totalTime.Ticks / 7)));
                    dayData.OnTimeArrival = Math.Round((double)onArivalTime / 7);
                }

                res.Message = "Log Found";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = new
                {
                    LastSevenDays = dayData,
                    ShiftData = shifData,
                    WeekList = responseData,
                };
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/attendance/getuserattencandedetailsbox | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public WeekOffCaseResponse CheckWeekOff(DayOfWeek day, DateTimeOffset today, List<WeekOffDaysCases> weekOff)
        {
            WeekOffCaseResponse obj = new WeekOffCaseResponse();
            var checkDayOff = weekOff.FirstOrDefault(x => x.DayId == day);
            if (checkDayOff != null)
            {
                if (checkDayOff.CaseId == WeekOffCase.All_Week)
                {
                    switch (checkDayOff.CaseResponseId)
                    {
                        case WeekOffDayConstants.Full_Day_Weekly_Off:
                            obj.IsWeekOff = true;
                            obj.Case = "FULL";
                            break;
                        case WeekOffDayConstants.First_Half_Weekly_Off:
                            obj.IsWeekOff = true;
                            obj.Case = "FIRST";
                            break;
                        case WeekOffDayConstants.Second_Half_Weekly_Off:
                            obj.IsWeekOff = true;
                            obj.Case = "SECOND";
                            break;
                    }
                }
            }
            return obj;
        }
        public class WeekOffCaseResponse
        {
            public bool IsWeekOff { get; set; } = false;
            public string Case { get; set; } = String.Empty;
        }
        public class LastSevenDaysResponse
        {
            public TimeSpan AverageTime { get; set; } = TimeSpan.Zero;
            public double OnTimeArrival { get; set; } = 0.0;
        }
        #endregion

        #region API TO UPLOAD IMAGE OF A USER FOR FACIAL CLOCK IN CLOCK OUT
        /// <summary>
        /// api/attendance/uploadtasktmultiple
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadtasktmultiple")]
        public async Task<HttpResponseMessageMultiple> UploadImageMullti()
        {
            HttpResponseMessageMultiple res = new HttpResponseMessageMultiple();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<PathLists> list = new List<PathLists>();
                List<string> extensionList = new List<string>();
                List<string> path = new List<string>();
                var data = Request.Content.IsMimeMultipartContent();
                if (Request.Content.IsMimeMultipartContent())
                {
                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);
                    var content = provider.Contents.Count;
                    for (int i = 0; i < content; i++)
                    {
                        var dates = DateTime.Now.ToString("yyyyMMddhhmmsstt");
                        var filefromreq = provider.Contents[i];
                        Stream _id = filefromreq.ReadAsStreamAsync().Result;
                        StreamReader reader = new StreamReader(_id);
                        string filename = filefromreq.Headers.ContentDisposition.FileName.Trim('\"');

                        ////////////// Add By Mohit 12-07-2021
                        string extension = System.IO.Path.GetExtension(filename);
                        string Fileresult = filename.Substring(0, filename.Length - extension.Length);

                        ////////////// Add By Mohit 12-07-2021
                        byte[] buffer = await filefromreq.ReadAsByteArrayAsync();
                        //f.byteArray = buffer;
                        string mime = filefromreq.Headers.ContentType.ToString();
                        Stream stream = new MemoryStream(buffer);
                        //var FileUrl = Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/TicketIages/" + claims.companyId), dates + filename);
                        //string DirectoryURL = (FileUrl.Split(new string[] { claims.companyId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + claims.companyId;

                        var FileUrl = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/uploadimage/LogInImages/" + tokenData.employeeId + "/"), ((tokenData.employeeId + "_" + i) + extension).Replace(" ", ""));
                        string DirectoryURL = (FileUrl.Split(new string[] { tokenData.employeeId + "\\" }, StringSplitOptions.None).FirstOrDefault()) + tokenData.employeeId;

                        //for create new Folder
                        DirectoryInfo objDirectory = new DirectoryInfo(DirectoryURL);
                        if (!objDirectory.Exists)
                        {
                            Directory.CreateDirectory(DirectoryURL);
                        }
                        ////////////// old Code 12-07-2021
                        //string path = "UploadImages\\" + compid + "\\" + filename;

                        //var temp = "uploadimage\\TicketIages\\" + claims.companyId + "\\" + dates + Fileresult + extension;

                        //var temp = "uploadimage\\TaskImages\\" + claims.companyId + "\\" + dates + Fileresult + extension;
                        extensionList.Add(extension);
                        string temp = "uploadimage\\LogInImages\\" + tokenData.employeeId + "\\" + (tokenData.employeeId + "_" + i) + extension;

                        ////////////// old Code 12-07-2021

                        File.WriteAllBytes(FileUrl, buffer.ToArray());
                        PathLists obj = new PathLists
                        {
                            Pathurl = temp.Replace(" ", ""),
                        };
                        list.Add(obj);
                        path.Add(temp.Replace(" ", ""));
                        var listdata = String.Join(",", list);
                    }

                    res.Message = "Successful";
                    res.Success = true;
                    res.Paths = list;
                    res.PathArray = path;
                    res.ExtensionList = extensionList;
                }
                else
                {
                    res.Message = "Error";
                    res.Success = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Success = false;
            }
            return res;
        }
        #endregion

        #region API TO ADD IMAGE FOR EMPLOYEE FOR FACIAL CLOCK IN
        /// <summary>
        /// Created By Harshit Mitra On 30-01-2023
        /// API >> Post >> api/attendance/addimageinemployee
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("addimageinemployee")]
        public async Task<IHttpActionResult> AddImageInEmployee(AddImageRequest model)
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
                var employeeImages = await _db.EmpImages
                    .FirstOrDefaultAsync(x => x.EmployeeId == tokenData.employeeId);
                if (employeeImages != null)
                {
                    res.Message = "Already Uploaded";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotAcceptable;
                    return Ok(res);
                }
                EmployeeImages obj = new EmployeeImages
                {
                    EmployeeId = tokenData.employeeId,
                    LastUpdateDate = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone),
                    ImagesUrl = JsonConvert.SerializeObject(model.ImageArray),
                };
                _db.EmpImages.Add(obj);
                await _db.SaveChangesAsync();

                res.Message = "Images Saved";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Created;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/attendance/getuserattendancelog | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class AddImageRequest
        {
            public List<string> ImageArray { get; set; } = new List<string>();
        }
        #endregion

        #region API TO GET ATTENDANCE DASHBOARD
        /// <summary>
        /// Created By Harshit Mitra On 08/12/2022
        /// API >> POST >> api/attendance/getattendancedashboard
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getattendancedashboard")]
        public async Task<IHttpActionResult> GetAttendanceDashboard()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                DateTimeOffset today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                var userAttendance = await (from x in _db.UserAttendances
                                            join e in _db.Employee on x.EmployeeId equals e.EmployeeId
                                            where x.Year == today.Year && x.Month == today.Month &&
                                                x.CompanyId == tokenData.companyId
                                            select new
                                            {
                                                x.Year,
                                                x.Month,
                                                x.EmployeeId,
                                                e.DisplayName,
                                                x.MonthLogs,
                                            })
                                            .FirstOrDefaultAsync();
                if (userAttendance == null)
                {
                    res.Message = "No Log Found";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = new List<int>();
                    return Ok(res);
                }


                return Ok();
            }
            catch (Exception ex)
            {
                logger.Error("API : api/salerybreakdown/addpaygroup | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class RangeListResponse
        {
            public int Key { get; set; } = 0;
            public string Value { get; set; } = "0%";
        }
        #endregion

        #region API FOR NEW LOGS
        /// <summary>
        /// api/attendance/makenewlogs
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("makenewlogs")]
        public async Task MakeNewLogs()
        {
            var newAttendance = await _db.UserAttendances.ToListAsync();
            var oldAttendance = await _db.UserAttendancesLog.Where(x => x.EmployeeId != 0).OrderBy(x => x.EmployeeId).ThenBy(x => x.CreatedOn).GroupBy(x => x.EmployeeId).ToListAsync();
            foreach (var ord in oldAttendance)
            {
                var orderYear = ord.OrderBy(x => x.CreatedOn.Year).GroupBy(x => x.CreatedOn.Year).ToList();
                foreach (var year in orderYear)
                {
                    var orderMonth = year.OrderBy(x => x.CreatedOn.Month).GroupBy(x => x.CreatedOn.Month).ToList();
                    foreach (var month in orderMonth)
                    {
                        if (!newAttendance.Any(x => x.Month == month.First().CreatedOn.Month && x.Year == month.First().CreatedOn.Month && x.EmployeeId == month.First().EmployeeId))
                        {
                            UserAttendanceModel obj = new UserAttendanceModel
                            {
                                Month = month.First().CreatedOn.Month,
                                Year = month.First().CreatedOn.Year,
                                EmployeeId = month.First().EmployeeId,
                                CompanyId = month.First().CompanyId,
                                CreatedBy = month.First().EmployeeId,
                                CreatedOn = month.First().CreatedOn,
                                MonthLogs = "",
                            };
                            var monthList = month
                                .Select(x => new MonthlyLog
                                {
                                    Date = x.CreatedOn,
                                    CaseId = AttendanceCase.Log,
                                    TotalWorkingTime = x.TotalTimeDate,
                                    DailyLogs = new List<DailyLog>()
                                    {
                                        new DailyLog()
                                        {
                                            CheckInTime = x.ClockInTime,
                                            CheckOutTime = x.ClockOutTime.HasValue ? x.ClockOutTime.Value : x.ClockInTime,
                                            IsCheckIn = !(x.ClockOutTime.HasValue),
                                            DailyCaseId = (x.ClockOutTime.HasValue) ? DailyCase.Present : DailyCase.Present_BAR_Missing_Swipe,
                                        },
                                    }
                                })
                                .ToList();
                            obj.MonthLogs = JsonConvert.SerializeObject(monthList);
                            _db.UserAttendances.Add(obj);
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            var userAttendance = newAttendance.FirstOrDefault(x => x.Month == month.First().CreatedOn.Month && x.Year == month.First().CreatedOn.Month && x.EmployeeId == month.First().EmployeeId);
                            var monthlyLog = JsonConvert.DeserializeObject<List<MonthlyLog>>(userAttendance.MonthLogs);
                            var orderDate = month.OrderBy(x => x.CreatedOn.Date).ToList();
                            foreach (var date in orderDate)
                            {
                                if (!monthlyLog.Any(x => x.Date.Date == date.CreatedOn.Date))
                                {
                                    monthlyLog.Add(new MonthlyLog
                                    {
                                        Date = date.CreatedOn,
                                        CaseId = AttendanceCase.Log,
                                        TotalWorkingTime = date.TotalTimeDate,
                                        DailyLogs = new List<DailyLog>()
                                        {
                                            new DailyLog()
                                            {
                                                CheckInTime = date.ClockInTime,
                                                CheckOutTime = date.ClockOutTime.HasValue ? date.ClockOutTime.Value : date.ClockInTime,
                                                IsCheckIn = !(date.ClockOutTime.HasValue),
                                                DailyCaseId = (date.ClockOutTime.HasValue) ? DailyCase.Present : DailyCase.Present_BAR_Missing_Swipe,
                                            },
                                        }
                                    });
                                }
                            }
                            monthlyLog = monthlyLog.OrderBy(x => x.Date).ToList();
                            userAttendance.MonthLogs = JsonConvert.SerializeObject(monthlyLog);
                            _db.Entry(userAttendance).State = EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }
                    }
                }
            }
        }
        #endregion

        #region API TO CHECK IN AND CHECK OUT EMPLOYEE THROUGH  SELFIE (Only App)
        /// <summary>
        /// Created By Ravi Vyas On 23-01-2023
        /// API >> POST >> api/attendance/checkincheckoutimg
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("checkincheckoutimg")]
        public async Task<IHttpActionResult> CheckInCheckOutImg(RequestForSelfieClockedInClockedOut model)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                DateTimeOffset today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                var checkShift = await _db.ShiftTimings
                    .Include("ShiftGroup")
                    .FirstOrDefaultAsync(x => x.WeekDay == today.DayOfWeek &&
                        x.ShiftGroup.ShiftGoupId == tokenData.EmpShiftGroup);
                if (checkShift != null)
                {
                    var startTime = TimeSpan.FromMinutes(checkShift.StartTime.TotalMinutes - (4 * 60));
                    if (checkShift.StartTime > checkShift.EndTime && today.TimeOfDay < startTime)
                        today = today.AddDays(-1);
                }
                var userAttendance = await _db.UserAttendances
                    .FirstOrDefaultAsync(x => x.Year == today.Year && x.Month == today.Month && x.EmployeeId ==
                            tokenData.employeeId && x.CompanyId == tokenData.companyId);
                if (userAttendance == null)
                    userAttendance = await CreateNewAttendanceImg(today, tokenData, model);
                else
                {
                    var monthLogList = JsonConvert.DeserializeObject<List<MonthlyLog>>(userAttendance.MonthLogs);
                    var monthLog = monthLogList.FirstOrDefault(x => x.Date.Date == today.Date);
                    if (monthLog == null)
                        userAttendance = await CreateDailyAttendanceImg(today, userAttendance, model);
                    else
                    {
                        var dailyLogList = monthLog.DailyLogs;
                        var dailyLog = dailyLogList.OrderBy(x => x.Id).LastOrDefault();
                        if (!dailyLog.IsCheckIn)
                            userAttendance = await CreateCheckInDailyLogImg(today, userAttendance, model);
                        else
                            userAttendance = await CreateCheckOutDailyLogImg(today, userAttendance, model);
                    }
                }
                res.Message = "Log Updated";
                res.Status = true;
                res.StatusCode = HttpStatusCode.Accepted;
                res.Data = userAttendance;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/attendance/checkincheckout | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        public class RequestForSelfieClockedInClockedOut
        {

            public double? Lat { get; set; }
            public double? Lng { get; set; }
            public string ImgURlCheckIn { get; set; } = string.Empty;
            public bool IsCheckIn { get; set; } = false;
            public double? CheckOutLat { get; set; }
            public double? CheckOutLng { get; set; }
            public string ImgURlCheckOut { get; set; } = string.Empty;
            public DateTimeOffset AttendanceDate { get; set; }

        }

        public async Task<UserAttendanceModel> CreateNewAttendanceImg(DateTimeOffset today, ClaimsHelperModel tokenData, RequestForSelfieClockedInClockedOut model)
        {
            UserAttendanceModel obj = new UserAttendanceModel
            {
                Month = today.Month,
                Year = today.Year,
                EmployeeId = tokenData.employeeId,
                CompanyId = tokenData.companyId,
                CreatedBy = tokenData.employeeId,
                CreatedOn = today,
                //UserLocation = JsonConvert.SerializeObject(
                //    new List<MonthlyLog>()
                //    {
                //        new MonthlyLog
                //        {
                //            Date = today.Date,
                //            CaseId = AttendanceCase.Log,
                //            DailyLogs = new List<DailyLog>()
                //            {
                //                new DailyLog
                //                {
                //                    CheckInTime = today,
                //                    IsCheckIn = true,
                //                    DailyCaseId = DailyCase.Present_BAR_Missing_Swipe,
                //                    Lat = model.Lat,
                //                    Lng = model.Lng,
                //                    ImgURlCheckIn = model.ImgURlCheckIn
                //                }
                //            },
                //        },

                //    }),
                MonthLogs = JsonConvert.SerializeObject(
                    new List<MonthlyLog>()
                    {
                        new MonthlyLog
                        {
                            Date = today.Date,
                            CaseId = AttendanceCase.Log,
                            DailyLogs = new List<DailyLog>()
                            {
                                new DailyLog
                                {
                                    CheckInTime = today,
                                    IsCheckIn = true,
                                    DailyCaseId = DailyCase.Present_BAR_Missing_Swipe,
                                    Lat = model.Lat,
                                    Lng = model.Lng,
                                    ImgURlCheckIn = model.ImgURlCheckIn
                                }
                            },
                        },

                    }),

            };
            _db.UserAttendances.Add(obj);
            await _db.SaveChangesAsync();
            return obj;
        }
        public async Task<UserAttendanceModel> CreateDailyAttendanceImg(DateTimeOffset today, UserAttendanceModel userAttendance, RequestForSelfieClockedInClockedOut model)
        {
            var monthLogList = JsonConvert.DeserializeObject<List<MonthlyLog>>(userAttendance.MonthLogs);
            monthLogList.Add(
                new MonthlyLog
                {
                    Date = today.Date,
                    CaseId = AttendanceCase.Log,
                    DailyLogs = new List<DailyLog>()
                    {
                        new DailyLog
                        {
                            CheckInTime = today,
                            IsCheckIn = true,
                            DailyCaseId = DailyCase.Present_BAR_Missing_Swipe,
                            Lat = model.Lat,
                            Lng = model.Lng,
                            ImgURlCheckIn = model.ImgURlCheckIn
                        }
                    },
                });
            //var imgLogList = JsonConvert.DeserializeObject<List<MonthlyLog>>(userAttendance.UserLocation);
            //imgLogList.Add(
            //    new MonthlyLog
            //    {
            //        Date = today.Date,
            //        CaseId = AttendanceCase.Log,
            //        DailyLogs = new List<DailyLog>()
            //        {
            //            new DailyLog
            //            {
            //                CheckInTime = today,
            //                IsCheckIn = true,
            //                DailyCaseId = DailyCase.Present_BAR_Missing_Swipe,
            //                Lat = model.Lat,
            //                Lng = model.Lng,
            //                ImgURlCheckIn = model.ImgURlCheckIn

            //            }
            //        },
            //    });
            userAttendance.MonthLogs = JsonConvert.SerializeObject(monthLogList);
            //userAttendance.MonthLogs = JsonConvert.SerializeObject(imgLogList);
            _db.Entry(userAttendance).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return userAttendance;
        }
        public async Task<UserAttendanceModel> CreateCheckInDailyLogImg(DateTimeOffset today, UserAttendanceModel userAttendance, RequestForSelfieClockedInClockedOut model)
        {
            var monthLogList = JsonConvert.DeserializeObject<List<MonthlyLog>>(userAttendance.MonthLogs);
            var monthLog = monthLogList.FirstOrDefault(x => x.Date.Date == today.Date);
            var newDailyLog = new DailyLog
            {
                Id = monthLog.DailyLogs.Count(),
                CheckInTime = today,
                IsCheckIn = true,
                DailyCaseId = DailyCase.Present_BAR_Missing_Swipe,
                Lat = model.Lat,
                Lng = model.Lng,
                ImgURlCheckIn = model.ImgURlCheckIn

            };
            //var imgLogList = JsonConvert.DeserializeObject<List<MonthlyLog>>(userAttendance.UserLocation);
            //var imgLog = imgLogList.FirstOrDefault(x => x.Date.Date == today.Date);
            //var newImgDailyLog = new DailyLog
            //{
            //    Id = monthLog.DailyLogs.Count(),
            //    CheckInTime = today,
            //    IsCheckIn = true,
            //    DailyCaseId = DailyCase.Present_BAR_Missing_Swipe,
            //    Lat = model.Lat,
            //    Lng = model.Lng,
            //    ImgURlCheckIn = model.ImgURlCheckIn
            //};


            //imgLog.DailyLogs.Add(newImgDailyLog);
            monthLog.DailyLogs.Add(newDailyLog);
            //userAttendance.UserLocation = JsonConvert.SerializeObject(monthLogList);
            userAttendance.MonthLogs = JsonConvert.SerializeObject(monthLogList);

            _db.Entry(userAttendance).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return userAttendance;
        }
        public async Task<UserAttendanceModel> CreateCheckOutDailyLogImg(DateTimeOffset today, UserAttendanceModel userAttendance, RequestForSelfieClockedInClockedOut model)
        {
            List<MonthlyLog> monthLogList = JsonConvert.DeserializeObject<List<MonthlyLog>>(userAttendance.MonthLogs);
            MonthlyLog monthLog = monthLogList.FirstOrDefault(x => x.Date.Date == today.Date);
            DailyLog currentLog = monthLog.DailyLogs.FirstOrDefault(x => x.IsCheckIn);

            var indexDailyLog = monthLog.DailyLogs.FindIndex(x => x.Id == currentLog.Id);
            if (indexDailyLog != -1)
            {
                monthLog.DailyLogs[indexDailyLog] = new DailyLog
                {
                    Id = currentLog.Id,
                    DailyCaseId = DailyCase.Present,
                    IsCheckIn = false,
                    CheckOutTime = today,
                    ImgURlCheckIn = currentLog.ImgURlCheckIn,
                    Lat = currentLog.Lat,
                    Lng = currentLog.Lng,
                    CheckInTime = currentLog.CheckInTime,
                    CheckOutLat = model.CheckOutLat,
                    CheckOutLng = model.CheckOutLng,
                    ImgURlCheckOut = model.ImgURlCheckOut

                };
            }
            var indexMonthlyLog = monthLogList.FindIndex(x => x.Date == today.Date);
            if (indexMonthlyLog != -1)
            {
                monthLogList[indexMonthlyLog] = new MonthlyLog
                {
                    Date = monthLog.Date,
                    CaseId = monthLog.CaseId,
                    TotalWorkingTime = (monthLog.TotalWorkingTime + (today - currentLog.CheckInTime)),
                    CaseStatus = false,
                    DailyLogs = monthLog.DailyLogs,
                };
            }
            //userAttendance.UserLocation = JsonConvert.SerializeObject(monthLogList);
            userAttendance.MonthLogs = JsonConvert.SerializeObject(monthLogList);

            _db.UserAttendances.Attach(userAttendance);
            _db.Entry(userAttendance).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return userAttendance;
        }
        #endregion

        #region Api for Get Company  ImgKitApiKey
        /// <summary>
        /// Created By Ravi Vyas On 17-04-2023 Only For Mobile App
        /// API >> GET >>  api/attendance/getcompanyimgkitapikey
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getcompanyimgkitapikey")]
        public async Task<IHttpActionResult> GetCompanyImgKitApiKey()
        {
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var getApiKey = _db.Company.Where(x => x.IsActive && !x.IsDeleted &&
                                           x.CompanyId == tokenData.companyId)
                                          .Select(x => x.ImgKitApiKey)
                                          .FirstOrDefault();
                if (getApiKey != null)
                {
                    res.Message = "Get Successfully !";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = getApiKey;
                    return Ok(res);
                }
                else
                {
                    res.Message = " Data Not Found !";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Data = getApiKey;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {

                logger.Error("API : api/attendance/getcompanyimgkitapikey | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        #endregion
    }
}
