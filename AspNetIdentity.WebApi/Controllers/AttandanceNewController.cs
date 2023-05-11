
using AspNetIdentity.WebApi.Helper;
using AspNetIdentity.WebApi.Infrastructure;
using AspNetIdentity.WebApi.Model;
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
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/attendancenew")]
    public class AttandanceNewController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        //#region Api To CheckIn and CheckOut

        ///// <summary>
        ///// API >> Put >> api/attendancenew/clockinclockout
        ///// Createde By Harshit Mitra on 01-03-2022
        ///// </summary>
        ///// <param name="isClockIn"></param>
        ///// <returns></returns>
        //[HttpPut]
        //[Route("clockinclockout")]
        //public async Task<ResponseBodyModel> ClockInClockOut()
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

        //        var todayDate = DateTime.Now.Date;
        //        var todatDateTime = DateTime.Now;

        //        var employee = await _db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == claims.employeeid);
        //        if (employee != null)
        //        {
        //            var attendance = await _db.UserAttendances.FirstOrDefaultAsync(x => x.CreatedBy == claims.employeeid && x.CreatedOn == todayDate && x.CompanyId == claims.companyid && x.OrgId == claims.orgid);
        //            if (attendance == null)
        //            {
        //                UserAttendanceLog obj = new UserAttendanceLog
        //                {
        //                    UserName = employee.FirstName + " " + employee.LastName,
        //                    IsCheckIn = true,
        //                    ClockInTime = todatDateTime,
        //                    TotalTime = null,
        //                    IsActive = true,
        //                    IsDeleted = false,
        //                    CreatedBy = claims.employeeid,
        //                    CreatedOn = todayDate,
        //                    CompanyId = claims.companyid,
        //                    OrgId = claims.orgid,
        //                };
        //                _db.UserAttendances.Add(obj);
        //                await _db.SaveChangesAsync();

        //                res.Message = "Clock In";
        //                res.Status = true;
        //                res.Data = obj;
        //            }
        //            else
        //            {
        //                if (!attendance.ClockOutTime.HasValue)
        //                {
        //                    TimeSpan timeDiffrences = todatDateTime - attendance.ClockInTime;
        //                    attendance.UpdatedOn = todayDate;
        //                    attendance.UpdatedBy = claims.employeeid;
        //                    attendance.ClockOutTime = todatDateTime;
        //                    attendance.IsCheckIn = false;
        //                    attendance.TotalTime = timeDiffrences.ToString(@"hh\:mm\:ss");

        //                    _db.Entry(attendance).State = System.Data.Entity.EntityState.Modified;
        //                    await _db.SaveChangesAsync();

        //                    res.Message = "Clock Out";
        //                    res.Status = true;
        //                    res.Data = attendance;
        //                }
        //                else
        //                {
        //                    res.Message = "Already Clock Out";
        //                    res.Status = false;
        //                    res.Data = attendance;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            res.Message = "Employee Not Found";
        //            res.Status = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}

        //#endregion Api To CheckIn and CheckOut

        #region Api To CheckIn and CheckOut

        /// <summary>
        /// API >> Put >> api/attendancenew/clockinclockout
        /// Createde By Harshit Mitra on 01-03-2022
        /// Modify By Suraj Bundel
        /// Modify By Ankit Jain Date - 26-12-2022
        /// </summary>
        /// <param name="isClockIn"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("clockinclockout")]
        public async Task<ResponseBodyModel> ClockInClockOut()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

                var todayDate = DateTime.Now.Date;
                var todayDateTime = DateTime.Now;
                var dayofweek = todayDate.DayOfWeek;
                var employee = _db.Employee.Where(x => x.EmployeeId == claims.employeeId).Select(x => x.ShiftGroupId).FirstOrDefault();
                var shiftgroupID = _db.ShiftTimings.Include("ShiftGroup").FirstOrDefault(x => x.ShiftGroup.ShiftGoupId == employee && x.WeekDay == dayofweek);

                var OnTimeArrival = ((shiftgroupID.StartTime) + TimeSpan.FromMinutes(30));
                var BeforeTimeArrival = ((shiftgroupID.StartTime) - TimeSpan.FromHours(3));
                var LateArrival = ((shiftgroupID.StartTime) + TimeSpan.FromHours(2));

                if (shiftgroupID != null)
                {
                    var attendance = await _db.UserAttendancesLog.FirstOrDefaultAsync(x => x.CreatedBy == claims.employeeId
                    && x.CreatedOn == todayDate && x.CompanyId == claims.companyId && x.OrgId == claims.orgId);

                    if (attendance == null)
                    {

                        UserAttendanceLog obj = new UserAttendanceLog
                        {
                            EmployeeId = claims.employeeId,
                            UserName = claims.displayName,
                            IsCheckIn = true,
                            IsClockin = true,
                            IsClockOut = false,
                            ClockInTime = todayDateTime,
                            TotalTime = null,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = claims.employeeId,
                            CreatedOn = todayDate,
                            CompanyId = claims.companyId,
                            OrgId = claims.orgId,
                            ArrivalStatus = shiftgroupID.StartTime <= OnTimeArrival ? AttendenceArrivalStatusConstants.OnTime : shiftgroupID.StartTime
                            >= BeforeTimeArrival ? AttendenceArrivalStatusConstants.Late : shiftgroupID.StartTime >= LateArrival ?
                            AttendenceArrivalStatusConstants.Late : AttendenceArrivalStatusConstants.Halfday,
                            ShiftTimingId = shiftgroupID.ShiftTimingId,
                        };
                        _db.UserAttendancesLog.Add(obj);
                        await _db.SaveChangesAsync();

                        res.Message = "Clock In";
                        res.Status = true;
                        res.Data = obj;
                    }
                    else
                    {
                        //var timespan = attendance.TotalTimeDate;
                        if (!attendance.ClockOutTime.HasValue)
                        {
                            TimeSpan timeDiffrences = todayDateTime - attendance.ClockInTime;
                            TimeSpan timeDiffrencesDate = todayDateTime - attendance.ClockInTime;
                            attendance.UpdatedOn = todayDate;
                            attendance.UpdatedBy = claims.employeeId;
                            attendance.ClockOutTime = todayDateTime;
                            attendance.IsCheckIn = false;
                            attendance.IsClockOut = true;
                            attendance.IsClockin = false;
                            attendance.TotalTime = timeDiffrences.ToString(@"hh\:mm\:ss");
                            attendance.TotalTimeDate = TimeSpan.ParseExact(timeDiffrencesDate.ToString(@"hh\:mm\:ss"), @"hh\:mm\:ss", null);


                            _db.Entry(attendance).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();

                            res.Message = "Clock Out";
                            res.Status = true;
                            res.Data = attendance;
                        }
                        else
                        {
                            var attendanceout = await _db.UserAttendancesLog.FirstOrDefaultAsync(x => x.CreatedBy == claims.employeeId && x.CreatedOn == todayDate
                            && x.CompanyId == claims.companyId && x.OrgId == claims.orgId);
                            var timespan = (TimeSpan)attendance.TotalTimeDate;
                            if (attendanceout.IsClockOut)
                            {


                                TimeSpan timeDiffrences = todayDateTime - attendance.ClockInTime;
                                TimeSpan timeDiffrencesDate = todayDateTime - attendance.ClockInTime;
                                attendance.UpdatedOn = todayDate;
                                attendance.UpdatedBy = claims.employeeId;
                                attendance.ClockOutTime = todayDateTime;
                                attendance.IsCheckIn = true;
                                attendance.IsClockOut = false;
                                attendance.IsClockin = true;
                                attendance.TotalTime = (timespan + timeDiffrences).ToString(@"hh\:mm\:ss");


                                attendance.TotalTimeDate = TimeSpan.ParseExact(attendance.TotalTime.ToString(), @"hh\:mm\:ss", null);
                                _db.Entry(attendance).State = System.Data.Entity.EntityState.Modified;
                                await _db.SaveChangesAsync();
                                res.Message = "Already Clock Out";
                                res.Status = true;
                                res.Data = attendance;
                            }
                            else
                            {

                                TimeSpan timeDiffrences = todayDateTime - attendance.ClockInTime;
                                TimeSpan timeDiffrencesDate = todayDateTime - attendance.ClockInTime;
                                attendance.UpdatedOn = todayDate;
                                attendance.UpdatedBy = claims.employeeId;
                                attendance.ClockOutTime = todayDateTime;
                                attendance.IsCheckIn = false;
                                attendance.IsClockOut = true;
                                attendance.IsClockin = false;
                                attendance.TotalTime = (timespan + timeDiffrences).ToString(@"hh\:mm\:ss");
                                attendance.TotalTimeDate = TimeSpan.ParseExact(attendance.TotalTime.ToString(), @"hh\:mm\:ss", null);

                                _db.Entry(attendance).State = System.Data.Entity.EntityState.Modified;
                                await _db.SaveChangesAsync();

                                res.Message = "Clock Out";
                                res.Status = true;
                                res.Data = attendance;
                            }
                        }
                    }
                }
                else
                {
                    res.Message = "Employee Not Found";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To CheckIn and CheckOut

        #region This Api Use To Get Shift Timeing
        /// <summary>
        /// Created By Ankit Jain On 27-12-2022
        /// API >> GET >> api/attendancenew/getshifttimeing
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [Route("getshifttimeing")]
        [HttpGet]
        public async Task<IHttpActionResult> GetShiftTimeing()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var today = TimeZoneConvert.ConvertTimeToSelectedZone(DateTime.UtcNow, tokenData.TimeZone);
                var employee = await _db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == tokenData.employeeId);
                if (employee != null)
                {
                    if (employee.ShiftGroupId == Guid.Empty)
                    {
                        res.Message = "Shift Group Not Assign";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        return Ok(res);
                    }
                    var shiftTimming = await _db.ShiftTimings
                        .Include("ShiftGroup")
                        .FirstOrDefaultAsync(x => x.WeekDay == today.DayOfWeek &&
                                x.ShiftGroup.ShiftGoupId == employee.ShiftGroupId);
                    if (shiftTimming == null)
                    {
                        res.Message = "Shift Group Not Assign";
                        res.Status = false;
                        res.StatusCode = HttpStatusCode.NotFound;
                        return Ok(res);
                    }
                    res.Message = "Shift Group";
                    res.Status = true;
                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = new ShiftTimeHelper
                    {
                        StartTime = shiftTimming.StartTime.ToString("T"),
                        EndTime = shiftTimming.EndTime.ToString("T"),
                    };
                    return Ok(res);
                }
                res.Message = "Shift Group Not Assign";
                res.Status = false;
                res.StatusCode = HttpStatusCode.NotFound;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("api/tasklog/getallassignproject", ex.Message);
                return BadRequest("Failed");
            }
        }

        public class ShiftTimeHelper
        {
            public string StartTime { get; set; }
            public string EndTime { get; set; }
        }
        #endregion

        #region Api To Get Total Time

        /// <summary>
        /// API >> Get >> api/attendancenew/getclocklogtime
        /// Createde By Harshit Mitra on 01-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getclocklogtime")]
        public async Task<ResponseBodyModel> GetClockLogTime()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            try
            {
                var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

                var todayDate = DateTime.Now.Date;
                var yestardayDate = DateTime.Now.Date.AddDays(-1);

                var attendanceList = await _db.UserAttendancesLog.Where(x => x.CreatedBy == claims.employeeId &&
                                        (x.CreatedOn == todayDate || x.CreatedOn == yestardayDate) && x.CompanyId == claims.companyId && x.OrgId == claims.orgId).ToListAsync();
                if (attendanceList.Count > 0)
                {
                    var attendance = attendanceList.FirstOrDefault(x => x.CreatedOn == todayDate);
                    if (attendance != null)
                    {
                        GetClockLogTimeModel obj = new GetClockLogTimeModel
                        {
                            IsClockIn = attendance.IsCheckIn,
                            ClockInTime = attendance.ClockInTime.ToString("HH:mm:ss"),
                            ClockOutTime = attendance.ClockOutTime.HasValue ? ((DateTimeOffset)attendance.ClockOutTime).ToString("HH:mm:ss") : "-----",
                            TotalTime = !String.IsNullOrEmpty(attendance.TotalTime) ? attendance.TotalTime : null,
                        };
                        res.Message = "Clock Time";
                        res.Status = true;
                        res.Data = obj;
                    }
                    else
                    {
                        GetClockLogTimeModel obj = new GetClockLogTimeModel
                        {
                            IsClockIn = false,
                            ClockInTime = "-----",
                            ClockOutTime = "-----",
                            TotalTime = null,
                        };
                        res.Message = "Not Clock In Today";
                        res.Status = true;
                        res.Data = obj;
                    }
                    var yestardayAttendance = attendanceList.FirstOrDefault(x => x.CreatedOn == yestardayDate);
                    if (yestardayAttendance != null)
                    {
                        if (!yestardayAttendance.ClockOutTime.HasValue)
                        {
                            yestardayAttendance.TotalTime = "00:00:00";
                            _db.Entry(yestardayAttendance).State = System.Data.Entity.EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }
                    }
                }
                else
                {
                    GetClockLogTimeModel obj = new GetClockLogTimeModel
                    {
                        IsClockIn = false,
                        ClockInTime = "-----",
                        ClockOutTime = "-----",
                        TotalTime = null,
                    };
                    res.Message = "Not Clock In Today";
                    res.Status = true;
                    res.Data = obj;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get Total Time

        #region Api To Get User Attendance List

        /// <summary>
        /// API >> Get >> api/attendancenew/getuserattendancelist
        /// Created By Harshit Mitra on 01-03-2022
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getuserattendancelist")]
        public async Task<ResponseBodyModel> GetUserAttendanceList()
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<GetAttendanceLog> list = new List<GetAttendanceLog>();
            try
            {
                var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);

                var todayDate = DateTime.Now.Date;
                var identity = User.Identity as ClaimsIdentity;

                var attendance = await _db.UserAttendancesLog.Where(x => x.CreatedBy == claims.employeeId &&
                                       x.CompanyId == claims.companyId && x.OrgId == claims.orgId).ToListAsync();
                if (attendance.Count > 0)
                {
                    foreach (var item in attendance)
                    {
                        GetAttendanceLog obj = new GetAttendanceLog()
                        {
                            Dates = item.CreatedOn,
                            DateString = item.CreatedOn.ToString("ddd, dd MMMM yyy"),
                            ClockInTime = item.ClockInTime.ToString("HH:mm:ss"),
                            //   ClockOutTime = item.ClockOutTime.HasValue ? ((DateTime)item.ClockOutTime).ToString("HH:mm:ss") : "",
                            ClockOutTime = item.ClockOutTime.HasValue ? ((DateTimeOffset)item.ClockOutTime).ToString("HH:mm:ss") : "",
                            TotalTime = !String.IsNullOrEmpty(item.TotalTime) ? item.TotalTime : "",
                            //LogDetails = item.TotalTime == "00:00:00" ? "Forgot to Check Out" :
                            //                (item.ClockInTime.ToString("HH:mm:ss") + " to " + (item.ClockOutTime.HasValue ?
                            //                ((DateTime)item.ClockOutTime).ToString("HH:mm:ss") : "still working")),
                            LogDetails = item.TotalTime == "00:00:00" ? "Forgot to Check Out" :
                                            (item.ClockInTime.ToString("HH:mm:ss") + " to " + (item.ClockOutTime.HasValue ?
                                            ((DateTimeOffset)item.ClockOutTime).ToString("HH:mm:ss") : "still working")),
                        };
                        list.Add(obj);
                    }
                    if (list.Count > 0)
                    {
                        var orderList = list.OrderByDescending(x => x.Dates);
                        res.Message = "Attendance List";
                        res.Status = true;
                        res.Data = orderList;
                    }
                    else
                    {
                        res.Message = "Attendance Log is Empty";
                        res.Status = false;
                        res.Data = list;
                    }
                }
                else
                {
                    res.Message = "Attendance Log is Empty";
                    res.Status = false;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get User Attendance List

        #region Api To Get All User Attendence List

        /// <summary>
        /// API >> Get >> api/attendancenew/allattendencelist
        /// Created By Harshit Mitra on 31-03-2022
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("allattendencelist")]
        public async Task<ResponseBodyModel> AllUserAttendenceList(int? userId)
        {
            ResponseBodyModel res = new ResponseBodyModel();
            List<GetAttendanceLog> list = new List<GetAttendanceLog>();
            var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var attendance = await (from x in _db.UserAttendancesLog
                                        join e in _db.Employee on x.CreatedBy equals e.EmployeeId
                                        where x.IsActive == true && x.IsDeleted == false && x.CompanyId == claims.companyId && x.OrgId == claims.orgId
                                        select new
                                        {
                                            x.CreatedOn,
                                            x.ClockInTime,
                                            x.ClockOutTime,
                                            x.TotalTime,
                                            e.EmployeeId,
                                            e.FirstName,
                                            e.LastName,
                                        }).ToListAsync();
                //var attendance = await _db.UserAttendances.Where(x => x.CompanyId == claims.companyid &&
                //                x.OrgId == claims.orgid).ToListAsync();

                if (userId != null && userId != 0)
                    attendance = attendance.Where(x => x.EmployeeId == userId).ToList();
                else
                    attendance = attendance.Where(x => x.CreatedOn.Date == DateTime.Now.Date).ToList();
                if (attendance.Count > 0)
                {
                    foreach (var item in attendance)
                    {
                        GetAttendanceLog obj = new GetAttendanceLog()
                        {
                            EmployeeId = item.EmployeeId,
                            EmployeeName = item.FirstName + " " + item.LastName,
                            Dates = item.CreatedOn,
                            DateString = item.CreatedOn.ToString("ddd, dd MMMM yyy"),
                            ClockInTime = item.ClockInTime.ToString("HH:mm:ss"),
                            //ClockOutTime = item.ClockOutTime.HasValue ? ((DateTime)item.ClockOutTime).ToString("HH:mm:ss") : "",
                            ClockOutTime = item.ClockOutTime.HasValue ? ((DateTimeOffset)item.ClockOutTime).ToString("HH:mm:ss") : "",
                            TotalTime = !String.IsNullOrEmpty(item.TotalTime) ? item.TotalTime : "",
                            //LogDetails = item.TotalTime == "00:00:00" ? "Forgot to Check Out" :
                            //                (item.ClockInTime.ToString("HH:mm:ss") + " to " + (item.ClockOutTime.HasValue ?
                            //                ((DateTime)item.ClockOutTime).ToString("HH:mm:ss") : "still working")),
                            LogDetails = item.TotalTime == "00:00:00" ? "Forgot to Check Out" :
                                            (item.ClockInTime.ToString("HH:mm:ss") + " to " + (item.ClockOutTime.HasValue ?
                                            ((DateTimeOffset)item.ClockOutTime).ToString("HH:mm:ss") : "still working")),
                        };
                        list.Add(obj);
                    }
                    if (list.Count > 0)
                    {
                        var orderList = list.OrderByDescending(x => x.Dates);
                        res.Message = "Attendance List";
                        res.Status = true;
                        res.Data = orderList;
                    }
                    else
                    {
                        res.Message = "Attendance Log is Empty";
                        res.Status = false;
                        res.Data = list;
                    }
                }
                else
                {
                    res.Message = "Attendance Log is Empty";
                    res.Status = false;
                    res.Data = list;
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.Status = false;
            }
            return res;
        }

        #endregion Api To Get All User Attendence List

        #region Helper Model Class

        /// <summary>
        /// Created By Harshit Mitra on 01-03-2022
        /// </summary>
        public class GetClockLogTimeModel
        {
            public bool IsClockIn { get; set; }
            public string ClockInTime { get; set; }
            public string ClockOutTime { get; set; }
            public string TotalTime { get; set; }
        }

        /// <summary>
        /// Created By Harshit Mitra on 01-03-2022
        /// </summary>
        public class GetAttendanceLog
        {
            public int EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public DateTime Dates { get; set; }
            public string DateString { get; set; }
            public string ClockInTime { get; set; }
            public string ClockOutTime { get; set; }
            public string TotalTime { get; set; }
            public string LogDetails { get; set; }
        }

        #endregion Helper Model Class

        #region Attendence Helper

        public static string GetAttendenceReport(AttendenceTypeConstants model)
        {
            string response = "";
            switch (model)
            {
                case AttendenceTypeConstants.Present:
                    response = "P";
                    break;
                case AttendenceTypeConstants.HalfDay:
                    response = "HD";
                    break;
                case AttendenceTypeConstants.Leave:
                    response = "L";
                    break;
                case AttendenceTypeConstants.Holiday:
                    response = "H";
                    break;
                case AttendenceTypeConstants.WeekOf:
                    response = "WF";
                    break;
                default:
                    response = "N/A";
                    break;
            }
            return response;
        }
        #endregion

        #region API TO GET ALL ATTENDANCE LOG OF A COMPANY BY MONTH AND YEAR
        /// <summary>
        /// Created By Harshit Mitra On 29-01-2023
        /// API >> GET >> api/attendancenew/allattendancelogbymonthyear
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("allattendancelogbymonthyear")]
        public async Task<IHttpActionResult> GetCompleteAttendanceLogInMonthYear(int month, int year)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employeeList = await _db.Employee
                    .Where(x => x.CompanyId == tokenData.companyId && x.IsActive && !x.IsDeleted &&
                            x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee)
                    .Select(x => new
                    {
                        x.EmployeeId,
                        x.DisplayName,
                    })
                    .OrderBy(x => x.DisplayName)
                    .ToListAsync();
                if (employeeList.Count == 0)
                {
                    res.Message = "No Employee In Company";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = new List<int>();
                    return Ok(res);
                }
                var employeeIdList = employeeList.Select(z => z.EmployeeId).ToList();
                List<AttendanceLogResponse> empLogList = new List<AttendanceLogResponse>();
                var attendanceLogList = await _db.UserAttendancesLog.Where(x => employeeIdList.Contains(x.EmployeeId)).ToListAsync();
                foreach (var employee in employeeList)
                {
                    Dictionary<string, string> listEmployeeLog = new Dictionary<string, string>();
                    var dayInMonth = DateTime.DaysInMonth(year, month) + 1;
                    long totalTicks = 0;
                    for (int i = 1; i != dayInMonth; i++)
                    {
                        var currentDayLog = attendanceLogList
                            .FirstOrDefault(x => x.ClockInTime.Month == month && x.ClockInTime.Year == year &&
                                    x.EmployeeId == employee.EmployeeId && x.ClockInTime.Day == i);
                        if (currentDayLog == null)
                        {
                            listEmployeeLog.Add("Day " + i, "No Log Enter");
                            totalTicks += 0;
                        }
                        else
                        {
                            if (!currentDayLog.ClockOutTime.HasValue)
                            {
                                listEmployeeLog.Add("Day " + i, currentDayLog.ClockInTime.ToString("HH:mm") + " - No Clock Out Found");
                                totalTicks += 0;
                            }
                            else
                            {
                                listEmployeeLog.Add("Day " + i, currentDayLog.ClockInTime.ToString("HH:mm") + " - " + currentDayLog.ClockInTime.ToString("HH:mm"));
                                totalTicks += currentDayLog.TotalTimeDate.Ticks;
                            }
                        }
                    }
                    var delta = new TimeSpan(totalTicks);
                    listEmployeeLog.Add("TotalTime", string.Format("{0:00}:{1:00}:{2:00}", delta.Hours, delta.Minutes, delta.Seconds));
                    var keyValueResponse = listEmployeeLog
                        .Select(x => new KeyValueClass
                        {
                            HeadData = x.Key,
                            Value = x.Value,
                        })
                        .ToList();
                    empLogList.Add(new AttendanceLogResponse
                    {
                        EmployeeName = employee.DisplayName,
                        LogData = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(listEmployeeLog, Formatting.Indented)),
                        //LogData = keyValueResponse,
                    });
                }
                res.Message = "Employee Attendance Report";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = empLogList;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/attendancenew/allattendancelogbymonthyear | " +
                    "Month : " + month + " | " +
                    "Year : " + year + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class AttendanceLogResponse
        {
            public string EmployeeName { get; set; }
            public dynamic LogData { get; set; }
        }

        public class KeyValueClass
        {
            public string HeadData { get; set; }
            public string Value { get; set; }
        }
        #endregion

        #region API TO GET ALL ATTENDANCE LOG OF A COMPANY BY MONTH AND YEAR
        /// <summary>
        /// Created By Harshit Mitra On 29-01-2023
        /// API >> GET >> api/attendancenew/allattendancelogbymonthyear
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("allattendancelogbymonthyear2")]
        public async Task<IHttpActionResult> GetCompleteAttendanceLogInMonthYear2(int month, int year)
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var employeeList = await _db.Employee
                    .Where(x => x.CompanyId == tokenData.companyId && x.IsActive && !x.IsDeleted && x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee)
                    .Select(x => new
                    {
                        x.EmployeeId,
                        x.DisplayName,
                    })
                    .OrderBy(x => x.DisplayName)
                    .ToListAsync();
                if (employeeList.Count == 0)
                {
                    res.Message = "No Employee In Company";
                    res.Status = false;
                    res.StatusCode = HttpStatusCode.NoContent;
                    res.Data = new List<int>();
                    return Ok(res);
                }
                var employeeIdList = employeeList.Select(z => z.EmployeeId).ToList();

                var dayInMonth = DateTime.DaysInMonth(year, month) + 1;
                var headList = new List<string>();
                headList.Add("Employee Name");
                for (int i = 1; i != dayInMonth; i++)
                {
                    headList.Add(i.ToString());
                }
                headList.Add("Total Time");

                var attendanceLogList = await _db.UserAttendancesLog.Where(x => employeeIdList.Contains(x.EmployeeId)).ToListAsync();
                List<List<string>> innerList = new List<List<string>>();
                foreach (var employee in employeeList)
                {
                    List<string> empLogList = new List<string>();
                    empLogList.Add(employee.DisplayName);

                    long totalTicks = 0;
                    for (int i = 1; i != dayInMonth; i++)
                    {
                        var currentDayLog = attendanceLogList
                            .FirstOrDefault(x => x.ClockInTime.Month == month && x.ClockInTime.Year == year &&
                                    x.EmployeeId == employee.EmployeeId && x.ClockInTime.Day == i);
                        if (currentDayLog == null)
                        {
                            empLogList.Add("No Log Enter");
                            totalTicks += 0;
                        }
                        else
                        {
                            if (!currentDayLog.ClockOutTime.HasValue)
                            {
                                empLogList.Add(currentDayLog.ClockInTime.ToString("HH:mm") + " - No Clock Out Found");
                                totalTicks += 0;
                            }
                            else
                            {
                                empLogList.Add(currentDayLog.ClockInTime.ToString("HH:mm") + " - " + ((DateTimeOffset)currentDayLog.ClockOutTime).ToString("HH:mm"));
                                totalTicks += currentDayLog.TotalTimeDate.Ticks;
                            }
                        }
                    }
                    var delta = new TimeSpan(totalTicks);
                    empLogList.Add(string.Format("{0:00}:{1:00}:{2:00}", delta.Hours, delta.Minutes, delta.Seconds));
                    innerList.Add(empLogList);
                }
                res.Message = "Employee Attendance Report";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = new AttendanceReportResponse
                {
                    HeadList = headList,
                    EmployeeLog = innerList,
                };
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/attendancenew/allattendancelogbymonthyear2 | " +
                    "Month : " + month + " | " +
                    "Year : " + year + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        public class AttendanceReportResponse
        {
            public List<string> HeadList { get; set; }
            public List<List<string>> EmployeeLog { get; set; }
        }
        #endregion

        #region API TO GET YEAR LIST IN ATTENDANCE LOG REPORT 
        /// <summary>
        /// Created By Harshit Mitra On 29-01-2023
        /// API >> GET >> api/attendancenew/getyearlistinattendancelog
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getyearlistinattendancelog")]
        public async Task<IHttpActionResult> GetYearListInAttendanceLog()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var yearList = await _db.UserAttendancesLog
                    .Where(x => x.CompanyId == tokenData.companyId)
                    .Select(x => x.ClockInTime.Year)
                    .Distinct()
                    .ToListAsync();

                res.Message = "Year List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = yearList
                    .Select(x => new
                    {
                        Id = x,
                        Value = x.ToString(),
                    })
                    .OrderBy(x => x.Id)
                    .ToList();
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/attendancenew/getyearlistinattendancelog | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion

        #region API TO GET MONTH LIST IN ATTENDANCE LOG REPORT 
        /// <summary>
        /// Created By Harshit Mitra On 29-01-2023
        /// API >> GET >> api/attendancenew/getmonthlistinattendancelog
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getmonthlistinattendancelog")]
        public IHttpActionResult GetMonthListInAttendanceLog()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            try
            {
                var monthList = new List<dynamic>();
                for (int i = 1; i != 13; i++)
                {
                    monthList.Add(new
                    {
                        Id = i,
                        Value = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                    });
                }
                res.Message = "Month List";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;
                res.Data = monthList;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/attendancenew/getmonthlistinattendancelog | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }
        #endregion


        //#region Attendence Date Helper

        //public static string GetAttendenceDateReport(Attendencereport repo, int date)
        //{
        //    string response = "";
        //    switch (date)
        //    {

        //        case 1:
        //            repo.One = AttendenceTypeEnum.NotAvailable;
        //            response = "01";
        //            break;
        //        case 2:
        //                 repo.Two =

        //                )
        //            response = "02";
        //            break;
        //        case repo.Three:
        //            response = "03";
        //            break;
        //        case repo.Four:
        //            response = "04";
        //            break;
        //        case repo.Five:
        //            response = "05";
        //            break;
        //        case repo.Six:
        //            response = "06";
        //            break;
        //        case repo.Seven:
        //            response = "07";
        //            break;
        //        case repo.Eight:
        //            response = "08";
        //            break;
        //        case repo.Nine:
        //            response = "09";
        //            break;
        //        case repo.Ten:
        //            response = "10";
        //            break;
        //        case repo.Eleven:
        //            response = "11";
        //            break;
        //        case repo.Twelve:
        //            response = "12";
        //            break;
        //        case repo.Thirteen:
        //            response = "13";
        //            break;
        //        case repo.Fourteen:
        //            response = "14";
        //            break;
        //        case repo.Fifteen:
        //            response = "15";
        //            break;
        //        case repo.Sixteen:
        //            response = "16";
        //            break;
        //        case repo.Seventeen:
        //            response = "17";
        //            break;
        //        case repo.Eighteen:
        //            response = "18";
        //            break;
        //        case repo.Nineteen:
        //            response = "19";
        //            break;
        //        case repo.Twenty:
        //            response = "20";
        //            break;
        //        case repo.Twentyone:
        //            response = "21";
        //            break;
        //        case repo.Twentytwo:
        //            response = "22";
        //            break;
        //        case repo.Twentythree:
        //            response = "23";
        //            break;
        //        case repo.Twentyfour:
        //            response = "24";
        //            break;
        //        case repo.Twentyfive:
        //            response = "25";
        //            break;
        //        case repo.Twentysix:
        //            response = "26";
        //            break;
        //        case repo.Twentyseven:
        //            response = "27";
        //            break;
        //        case repo.Twentyeight:
        //            response = "28";
        //            break;
        //        case repo.Twentynine:
        //            response = "29";
        //            break;
        //        case repo.Thirty:
        //            response = "30";
        //            break;
        //        case repo.Thirtyone:
        //            response = "31";
        //            break;
        //        case repo.Month:
        //            response = "1";
        //            break;
        //        case repo.Year:
        //            response = "2022";
        //            break;


        //        default:
        //            response = "N/A";
        //            break;
        //    }
        //    return response;
        //}
        //#endregion

        //#region check Attendence
        //public static object AttendenceCheck(string officeemail, ClaimsHelperModel claims)
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        ApplicationDbContext db = new ApplicationDbContext();

        //        var empid = db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.OfficeEmail == officeemail && x.CompanyId == claims.companyId).Select(x => x.EmployeeId).FirstOrDefault();
        //        var leave = db.LeaveRequests.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyId && x.RequestedBy == empid && (x.Status == LeaveStatusEnum.Approved || x.Status == LeaveStatusEnum.Partially_Approved) && x.FromDate >= DateTime.Today && x.ToDate <= DateTime.Today);
        //        if (leave == null)
        //        {
        //            res.Message = "Attendance not saved";
        //            res.Status = false;
        //        }
        //        else
        //        {
        //            var weekoff = ((DateTime.Today.DayOfWeek == DayOfWeek.Saturday) || (DateTime.Today.DayOfWeek == DayOfWeek.Saturday));
        //            if (weekoff)
        //            {
        //                res.Message = "This is a weekend";
        //                res.Status = false;
        //            }
        //            else
        //            {

        //            }
        //        }

        //        res.Message = "Message";
        //        res.Status = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //        throw;
        //    }
        //    return res;
        //}
        //#endregion

        //#region MyRegion
        //public async Task<ResponseBodyModel> AttendanceExcelImport(List<AttendanceImportFaultyLogs> models)
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    List<AttendanceImportFaultyLogs> falultyImports = new List<AttendanceImportFaultyLogs>();
        //    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        //    long successfullImported = 0;
        //    var claims = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
        //    try
        //    {
        //        if (models.Count <= 0)
        //        {
        //            res.Message = "Excel Not Have Any Data";
        //            res.Status = false;
        //            res.Data = falultyImports;
        //        }
        //        else
        //        {
        //            foreach (var item in models)
        //            {
        //                var empid = _db.Employee.Where(x => x.IsActive && !x.IsDeleted && x.OfficeEmail == item.OfficeEmail && x.CompanyId == claims.companyid).Select(x => x.EmployeeId).FirstOrDefault();
        //                var attendance = _db.Attendencereports.FirstOrDefault(x => x.IsActive && x.IsDeleted && x.CompanyId == claims.companyid);

        //                attendance.One = item.One;
        //                attendance.Two = item.Two;
        //                attendance.Three = item.Three;
        //                attendance.Four = item.Four;
        //                attendance.Five = item.Five;
        //                attendance.Six = item.Six;
        //                attendance.Seven = item.Seven;
        //                attendance.Eight = item.Eight;
        //                attendance.Nine = item.Nine;
        //                attendance.Ten = item.Ten;
        //                attendance.Eleven = item.Eleven;
        //                attendance.Twelve = item.Twelve;
        //                attendance.Thirteen = item.Thirteen;
        //                attendance.Fourteen = item.Fourteen;
        //                attendance.Fifteen = item.Fifteen;
        //                attendance.Sixteen = item.Sixteen;
        //                attendance.Seventeen = item.Seventeen;
        //                attendance.Eighteen = item.Eighteen;
        //                attendance.Nineteen = item.Nineteen;
        //                attendance.Twenty = item.Twenty;
        //                attendance.Twentyone = item.Twentyone;
        //                attendance.Twentytwo = item.Twentytwo;
        //                attendance.Twentythree = item.Twentythree;
        //                attendance.Twentyfour = item.Twentyfour;
        //                attendance.Twentyfive = item.Twentyfive;
        //                attendance.Twentysix = item.Twentysix;
        //                attendance.Twentyseven = item.Twentyseven;
        //                attendance.Twentyeight = item.Twentyeight;
        //                attendance.Twentynine = item.Twentynine;
        //                attendance.Thirty = item.Thirty;
        //                attendance.Thirtyone = item.Thirtyone;
        //                attendance.Month = item.Month;
        //                attendance.Year = item.Year;
        //                attendance.EmployeeId = item.EmployeeId;
        //                attendance.IsActive = true;
        //                attendance.IsDeleted = false;
        //                attendance.CompanyId = claims.companyid;
        //                attendance.OrgId = claims.orgid;
        //                attendance.CreatedBy = claims.employeeid;
        //                attendance.CreatedOn = DateTime.Now;

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //var leave = _db.LeaveRequests.Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == claims.companyid && x.RequestedBy == empid && (x.Status == LeaveStatusEnum.Approved || x.Status == LeaveStatusEnum.Partially_Approved) && x.FromDate >= DateTime.Today && x.ToDate <= DateTime.Today);
        //        //if (leave == null)
        //        //{
        //        //    res.Message = "Attendance not saved";
        //        //    res.Status = false;
        //        //}
        //        //else
        //        //{
        //        //    var weekoff = ((DateTime.Today.DayOfWeek == DayOfWeek.Saturday) || (DateTime.Today.DayOfWeek == DayOfWeek.Saturday));
        //        //    if (weekoff)
        //        //    {
        //        //        res.Message = "This is a weekend";
        //        //        res.Status = false;
        //        //    }
        //        //    else
        //        //    {

        //        res.Message = "Message";
        //        res.Status = true;
        //    }
        //}
        //#endregion



        #region API TO GET ATTANDANCE DASHBOARD
        /// <summary>
        /// Created By Harshit Mitra On 10-01-2023
        /// API >> GET >> api/attendancenew/attandancedashboard
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("attandancedashboard")]
        public async Task<IHttpActionResult> GetAttandanceDashboard()
        {
            ResponseStatusCode res = new ResponseStatusCode();
            var tokenData = ClaimsHelper.GetClaimsResult(User.Identity as ClaimsIdentity);
            try
            {
                var today = DateTime.Now;
                var attdanceLog = await _db.UserAttendancesLog
                    .Where(x => x.CompanyId == tokenData.companyId && DbFunctions.TruncateTime(x.CreatedOn) == today.Date)
                    .ToListAsync();
                var employee = await _db.Employee
                    .Where(x => x.CompanyId == tokenData.companyId && x.EmployeeTypeId != EmployeeTypeConstants.Ex_Employee)
                    .Select(x => new
                    {
                        x.EmployeeId,
                        x.DisplayName,
                    })
                    .ToListAsync();
                var onWorkFromHome = _db.WorkFromHomes.Count(x => x.WFHStatus == "Approved" && x.StartDate <= today && x.EndDate >= today && x.CompanyId == tokenData.companyId);
                var onLeaveToday = _db.LeaveRequests.Count(x => x.Status != LeaveStatusConstants.Rejected && x.Status != LeaveStatusConstants.Cancel && x.CompanyId == tokenData.companyId
                    && x.FromDate <= today && x.ToDate >= today);
                if (attdanceLog.Count > 0)
                {
                    res.Data = new
                    {
                        TotalEmployee = employee.Count,
                        EmployeeClockedIn = attdanceLog.Select(x => x.EmployeeId).Distinct().ToList().Count(),
                        EmployeeNotClockedIn = employee.Count - attdanceLog.Select(x => x.EmployeeId).Distinct().ToList().Count(),
                        WorkFromHome = onWorkFromHome,
                        OnleaveToday = onLeaveToday,
                        EmployeeInOffice = employee.Count - onWorkFromHome - onLeaveToday,
                        ClockInList = employee
                            .Select(x => new
                            {
                                x.EmployeeId,
                                x.DisplayName,
                                EmployeeLog = attdanceLog.FirstOrDefault(z => z.EmployeeId == x.EmployeeId),

                            })
                            .Select(x => new
                            {
                                x.EmployeeId,
                                x.DisplayName,
                                IsClockIn = x.EmployeeLog != null,
                                ClockInTime = x.EmployeeLog != null ? x.EmployeeLog.ClockInTime.ToString("HH:mm:ss") : TimeSpan.Zero.ToString(),
                                ClockOutTime = x.EmployeeLog != null ?
                                    (x.EmployeeLog.ClockOutTime.HasValue ?
                                        ((DateTimeOffset)x.EmployeeLog.ClockOutTime).ToString("HH:mm:ss") : TimeSpan.Zero.ToString())
                                    : TimeSpan.Zero.ToString()
                            })
                            .OrderByDescending(x => x.IsClockIn).ThenBy(x => x.DisplayName)
                            .ToList(),
                    };
                }
                else
                {
                    res.Data = new
                    {
                        TotalEmployee = employee.Count,
                        EmployeeClockedIn = 0,
                        EmployeeNotClockedIn = employee.Count,
                        WorkFromHome = onWorkFromHome,
                        OnleaveToday = onLeaveToday,
                        EmployeeInOffice = employee.Count - onWorkFromHome - onLeaveToday,
                        ClockInList = employee
                            .Select(x => new
                            {
                                x.EmployeeId,
                                x.DisplayName,
                                EmployeeLog = attdanceLog.FirstOrDefault(z => z.EmployeeId == x.EmployeeId),

                            })
                            .Select(x => new
                            {
                                x.EmployeeId,
                                x.DisplayName,
                                IsClockIn = x.EmployeeLog != null,
                                ClockInTime = x.EmployeeLog != null ? x.EmployeeLog.ClockInTime.ToString("HH:mm:ss") : TimeSpan.Zero.ToString(),
                                ClockOutTime = x.EmployeeLog != null ?
                                    (x.EmployeeLog.ClockOutTime.HasValue ?
                                        ((DateTimeOffset)x.EmployeeLog.ClockOutTime).ToString("HH:mm:ss") : TimeSpan.Zero.ToString())
                                    : TimeSpan.Zero.ToString()
                            })
                            .OrderByDescending(x => x.IsClockIn).ThenBy(x => x.DisplayName)
                            .ToList(),
                    };
                }


                res.Message = "Attandance Dashboard";
                res.Status = true;
                res.StatusCode = HttpStatusCode.OK;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error("API : api/attendancenew/attandancedashboard | " +
                    //"Model : " + JsonConvert.SerializeObject(model) + " | " +
                    "Exception : " + JsonConvert.SerializeObject(ex));
                return BadRequest("Failed");
            }
        }

        #endregion

        //#region Add Default ShiftGroup
        ///// <summary>
        ///// Created by Suraj Bundel on 11/10/2022
        ///// Post => api/attendancenew/adddefaultshiftgroup
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("adddefaultshiftgroup")]
        //public async Task<ResponseBodyModel> AddDefaultShiftGroupForAllCompanies()
        //{
        //    ResponseBodyModel res = new ResponseBodyModel();
        //    try
        //    {
        //        var companiesList = await _db.Company.Where(x => x.DefaultShiftId == Guid.Empty).ToListAsync();
        //        if (companiesList.Count > 0)
        //        {
        //            foreach (var company in companiesList)
        //            {
        //                ShiftGroup obj = new ShiftGroup();
        //                obj.ShiftGoupId = Guid.NewGuid();
        //                obj.ShiftName = "Default Shift";
        //                obj.ShiftCode = "DS";
        //                obj.Description = "Default Shift by Admin";
        //                obj.IsFlexible = false;
        //                obj.IsTimingDifferent = false;
        //                obj.IsDurationDifferent = false;
        //                obj.IsDefaultShiftGroup = true;
        //                obj.IsActive = true;
        //                obj.IsDeleted = false;
        //                obj.CompanyId = company.CompanyId;
        //                obj.OrgId = 0;
        //                obj.CreatedBy = 0;
        //                obj.CreatedOn = DateTime.Now;

        //                var addshifttiming = Enum.GetValues(typeof(DayOfWeek))
        //                        .Cast<DayOfWeek>()
        //                        .Select(x => new ShiftTiming
        //                        {
        //                            ShiftGroup = obj,
        //                            ShiftTimingId = Guid.NewGuid(),
        //                            WeekDay = x,
        //                            WeekName = x.ToString(),
        //                            StartTime = new TimeSpan(09, 0, 0),
        //                            EndTime = new TimeSpan(18, 0, 0),
        //                            BreakTime = 0,
        //                            IsActive = true,
        //                            IsDeleted = false,
        //                            CompanyId = company.CompanyId,
        //                            OrgId = 0,
        //                            CreatedBy = 0,
        //                            CreatedOn = DateTime.Now,
        //                        })
        //                        .OrderBy(x => x.WeekDay == DayOfWeek.Sunday).ThenBy(x => x.WeekDay)
        //                        .ToList();

        //                _db.ShiftGroups.Add(obj);
        //                _db.ShiftTimings.AddRange(addshifttiming);
        //                company.DefaultShiftId = obj.ShiftGoupId;
        //                _db.Entry(companiesList).State = EntityState.Modified;

        //                await _db.SaveChangesAsync();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Message = ex.Message;
        //        res.Status = false;
        //    }
        //    return res;
        //}
        //#endregion

    }
}