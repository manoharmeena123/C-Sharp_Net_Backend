using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model.NewUserAttendance;
using AspNetIdentity.WebApi.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Controllers.UserAttendance.Holidays
{
    [Authorize]
    [RoutePrefix("api/attendancereport")]
    public class AttendanceReportController : ApiController
    {
        public readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region API TO GET YEAR LIST OF ATTENCANCE 
        /// <summary>
        /// Created By Harshit Mitra On 09-02-2023
        /// API >> GET >> api/attendancereport/getyearlistattendancereport
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getyearlistattendancereport")]
        public async Task<IHttpActionResult> GetYearListAttendanceReport()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                var minYear = await _db.UserAttendances.MinAsync(x => x.Year);
                if (minYear == 0)
                    minYear = today.Year;
                List<KeyValueAttReportResponse> list = new List<KeyValueAttReportResponse>();
                for (int i = today.Year; i != (minYear - 1); i--)
                {
                    list.Add(new KeyValueAttReportResponse
                    {
                        Id = i,
                        Value = i.ToString(),
                    });
                }
                res.Message = "Year List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = list;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/attendancereport/getyearlistattendancereport | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest();
            }
        }
        public class KeyValueAttReportResponse
        {
            public int Id { get; set; } = 0;
            public string Value { get; set; } = String.Empty;
        }
        #endregion

        #region API TO GET MONTH LIST OF ATTENCANCE BY YEAR
        /// <summary>
        /// Created By Harshit Mitra On 09-02-2023
        /// API >> GET >> api/attendancereport/getmonthlistattendancereport
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getmonthlistattendancereport")]
        public async Task<IHttpActionResult> GetMonthListAttendanceReport(int year)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                var maxMonth = await _db.UserAttendances.Where(x => x.Year == year).MaxAsync(x => x.Month);
                if (maxMonth == 0)
                    maxMonth = today.Month;
                List<KeyValueAttReportResponse> list = new List<KeyValueAttReportResponse>();
                for (int i = 1; i != (maxMonth + 1); i++)
                {
                    list.Add(new KeyValueAttReportResponse
                    {
                        Id = i,
                        Value = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                    });
                }
                res.Message = "Month List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = list.OrderByDescending(x => x.Id).ToList();

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/attendancereport/getmonthlistattendancereport | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest();
            }
        }
        #endregion

        #region API TO GET ATTENDANCE REPORTS 
        /// <summary>
        /// Created By Harshit Mitra On 09-02-2023
        /// API >> POST >> api/attendancereport/getattendancereports
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getattendancereports")]
        public async Task<IHttpActionResult> GetAttendanceReports(int year, int month)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<AttendanceLogResponse> employeeLog = new List<AttendanceLogResponse>();
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                int daysInMonth = DateTime.DaysInMonth(year, month);
                List<DateTime> dateList = new List<DateTime>();
                for (int i = 1; i <= daysInMonth; i++)
                    dateList.Add(new DateTime(year, month, i));

                var employeeDetails = await (from e in _db.Employee
                                                 //join s in _db.ShiftTimings.Include("ShiftGroup") on e.ShiftGroupId equals s.ShiftGroup.ShiftGoupId
                                                 //join w in _db.WeekOffDaysCases.Include("Group") on e.WeekOffId equals w.Group.WeekOffId
                                             where e.CompanyId == tokenData.companyId
                                             orderby e.DisplayName
                                             select new
                                             {
                                                 e.EmployeeId,
                                                 e.DisplayName,
                                                 //s = s,
                                                 //w = w,
                                             })
                                             .ToListAsync();
                var empIds = employeeDetails.Select(x => x.EmployeeId).Distinct().ToList();
                var userAttendance = await _db.UserAttendances
                    .Where(x => empIds.Contains(x.EmployeeId) && x.Year == year && x.Month == month)
                    .ToListAsync();
                List<AttendanceLogResponse> logList = new List<AttendanceLogResponse>();
                foreach (var emp in employeeDetails)
                {
                    AttendanceLogResponse log = new AttendanceLogResponse
                    {
                        EmployeeName = emp.DisplayName,
                    };
                    var attendance = userAttendance.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                    if (attendance == null)
                    {
                        log.TotalTime = "00h 00m";
                        var data = dateList
                            .Select(x => new
                            {
                                Key = x.ToString("dd"),
                                Value = x,
                            })
                            .ToDictionary(x => x.Key, x => new InnerLogResponse
                            {
                                Case = x.Value > today.Date ? "Up Coming" : "Absent",
                                Time = "00h 00m",
                            })
                            .Select(x => new
                            {
                                x.Value.Case,
                                x.Value.Time,
                            }).ToList();

                        log.LogData = data;

                        log.TotalTime = "00h 00m";
                    }
                    else
                    {
                        Dictionary<string, InnerLogResponse> logData = new Dictionary<string, InnerLogResponse>();
                        var monthyLog = JsonConvert.DeserializeObject<List<MonthlyLog>>(attendance.MonthLogs);
                        long timeTicks = 0;
                        foreach (var item in dateList)
                        {
                            var checkDailyLog = monthyLog.FirstOrDefault(x => x.Date.Date == item.Date);
                            if (checkDailyLog == null)
                            {
                                logData.Add(item.ToString("dd"),
                                        new InnerLogResponse
                                        {
                                            Case = item.Date > today.Date ? "Up Coming" : "Absent",
                                            Time = "00h 00m",

                                        }
                                    );
                            }
                            else
                            {
                                logData.Add(item.ToString("dd"),
                                    checkDailyLog.DailyLogs.All(x => !x.IsCheckIn) ?
                                        new InnerLogResponse
                                        {
                                            Case = "Present",
                                            Time = (String.Format("{0:hh\\:mm}", checkDailyLog.TotalWorkingTime)).Replace(":", "h ") + "m",
                                        }
                                    :
                                        new InnerLogResponse
                                        {
                                            Case = "Clock Out Missing",
                                            Time = (String.Format("{0:hh\\:mm}", checkDailyLog.TotalWorkingTime)).Replace(":", "h ") + "m",
                                        }
                                    );
                                timeTicks += checkDailyLog.TotalWorkingTime.Ticks;
                            }
                        }
                        var data = logData
                            .Select(x => new
                            {
                                x.Value.Case,
                                x.Value.Time,
                            })
                            .ToList();
                        log.LogData = data;
                        log.TotalTime = (String.Format("{0:hh\\:mm}", new TimeSpan(timeTicks))).Replace(":", "h ") + "m";
                    }
                    logList.Add(log);
                }
                var strigDateList = dateList.Select(x => x.ToString("dd")).ToList();

                res.Message = "Attendance Report";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = new AttendanceReportResponse
                {
                    DateList = strigDateList,
                    LogResponse = logList,
                };

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/attendancereport/getattendancereports | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest();
            }
        }
        public class AttendanceReportResponse
        {
            public List<string> DateList { get; set; } = new List<string>();
            public List<AttendanceLogResponse> LogResponse { get; set; } = new List<AttendanceLogResponse>();
        }
        public class AttendanceLogResponse
        {
            public string EmployeeName { get; set; }
            public string TotalTime { get; set; }
            public object LogData { get; set; }
        }
        public class InnerLogResponse
        {
            public string Case { get; set; }
            public string Time { get; set; }
        }
        #endregion

        #region API TO GENERATE ATTENDANCE REPORT
        /// <summary>
        /// Created By Harshit Mitra On 09-02-2023
        /// API >> POST >> api/attendancereport/generateattendancereports
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("generateattendancereports")]
        public async Task<IHttpActionResult> GenerateAttendanceReports(int year, int month)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                List<AttendanceLogResponse> employeeLog = new List<AttendanceLogResponse>();
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                int daysInMonth = DateTime.DaysInMonth(year, month);
                List<DateTime> dateList = new List<DateTime>();
                for (int i = 1; i <= daysInMonth; i++)
                    dateList.Add(new DateTime(year, month, i));

                var employeeDetails = await (from e in _db.Employee
                                                 //join s in _db.ShiftTimings.Include("ShiftGroup") on e.ShiftGroupId equals s.ShiftGroup.ShiftGoupId
                                                 //join w in _db.WeekOffDaysCases.Include("Group") on e.WeekOffId equals w.Group.WeekOffId
                                             where e.CompanyId == tokenData.companyId
                                             orderby e.DisplayName
                                             select new
                                             {
                                                 e.EmployeeId,
                                                 e.DisplayName,
                                                 //s = s,
                                                 //w = w,
                                             })
                                             .ToListAsync();
                var empIds = employeeDetails.Select(x => x.EmployeeId).Distinct().ToList();
                var userAttendance = await _db.UserAttendances
                    .Where(x => empIds.Contains(x.EmployeeId) && x.Year == year && x.Month == month)
                    .ToListAsync();
                List<AttendanceLogResponse> logList = new List<AttendanceLogResponse>();
                foreach (var emp in employeeDetails)
                {
                    AttendanceLogResponse log = new AttendanceLogResponse
                    {
                        EmployeeName = emp.DisplayName,
                    };
                    var attendance = userAttendance.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                    if (attendance == null)
                    {
                        log.TotalTime = "00h 00m";
                        var data = dateList
                            .Select(x => new
                            {
                                Key = "Day " + x.ToString("dd"),
                                Value = x,
                            })
                            .ToDictionary(x => x.Key, x => "00h 00m");

                        log.LogData = data;

                        log.TotalTime = "00h 00m";
                    }
                    else
                    {
                        Dictionary<string, string> logData = new Dictionary<string, string>();
                        var monthyLog = JsonConvert.DeserializeObject<List<MonthlyLog>>(attendance.MonthLogs);
                        long timeTicks = 0;
                        foreach (var item in dateList)
                        {
                            var checkDailyLog = monthyLog.FirstOrDefault(x => x.Date.Date == item.Date);
                            if (checkDailyLog == null)
                            {
                                logData.Add("Day " + item.ToString("dd"), "00h 00m");
                            }
                            else
                            {
                                logData.Add("Day " + item.ToString("dd"),
                                    checkDailyLog.DailyLogs.All(x => !x.IsCheckIn) ?
                                       ((String.Format("{0:hh\\:mm}", checkDailyLog.TotalWorkingTime)).Replace(":", "h ") + "m")
                                    :
                                        ((String.Format("{0:hh\\:mm}", checkDailyLog.TotalWorkingTime)).Replace(":", "h ") + "m"));

                                timeTicks += checkDailyLog.TotalWorkingTime.Ticks;
                            }
                        }
                        var data = logData;
                        //.Select(x => new
                        //{
                        //    x.Value.Case,
                        //    x.Value.Time,
                        //})
                        //.ToList();
                        log.LogData = data;
                        log.TotalTime = (String.Format("{0:hh\\:mm}", new TimeSpan(timeTicks))).Replace(":", "h ") + "m";
                    }
                    logList.Add(log);
                }
                var strigDateList = dateList.Select(x => x.ToString("dd")).ToList();

                res.Message = "Attendance Report";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = logList;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/attendancereport/generateattendancereports | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest();
            }
        }
        public class MyClass
        {

        }
        #endregion

    }
}
