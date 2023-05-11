using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.NewUserAttendance
{
    public class UserAttendanceModel : BaseModelClass
    {
        [Key]
        public Guid AttendanceId { get; set; } = Guid.NewGuid();
        public int Year { get; set; } = 0;
        public int Month { get; set; } = 0;
        public int EmployeeId { get; set; } = 0;
        public string MonthLogs { get; set; } = String.Empty;
       // public string UserLocation { get; set; } = String.Empty;
    }
    public class MonthlyLog
    {
        public DateTimeOffset Date { get; set; } = DateTime.UtcNow;
        public AttendanceCase CaseId { get; set; } = AttendanceCase.No_Time_Entries_Logged;
        public TimeSpan TotalWorkingTime { get; set; } = TimeSpan.Zero;
        public bool CaseStatus { get; set; } = true;
        public bool IsAutoCheckOut { get; set; } = false;
        public List<DailyLog> DailyLogs { get; set; } = new List<DailyLog>();
    }
    public class DailyLog
    {
        public int Id { get; set; } = 0;
        public DailyCase DailyCaseId { get; set; } = DailyCase.Absent;
        public bool IsCheckIn { get; set; } = false;
        public DateTimeOffset CheckInTime { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset CheckOutTime { get; set; } = DateTimeOffset.UtcNow;
        public double? Lat { get; set; } = 0.0;
        public double? Lng { get; set; } = 0.0;
        public string ImgURlCheckIn { get; set; } = string.Empty;
        public double? CheckOutLat { get; set; } = 0.0;
        public double? CheckOutLng { get; set; } = 0.0;
        public string ImgURlCheckOut { get; set; } = string.Empty;
    }

    //public class MonthlyLogImg
    //{
    //    public DateTimeOffset Date { get; set; } = DateTime.UtcNow;
    //    public AttendanceCase CaseId { get; set; } = AttendanceCase.No_Time_Entries_Logged;
    //    public TimeSpan TotalWorkingTime { get; set; } = TimeSpan.Zero;
    //    public bool CaseStatus { get; set; } = true;
    //    public bool IsAutoCheckOut { get; set; } = false;
    //    public List<DailyLogImg> DailyLogs { get; set; } = new List<DailyLogImg>();
    //}
    //public class DailyLogImg
    //{
    //    public int Id { get; set; } = 0;
    //    public DailyCase DailyCaseId { get; set; } = DailyCase.Absent;
    //    public bool IsCheckIn { get; set; } = false;
    //    public DateTimeOffset CheckInTime { get; set; } = DateTimeOffset.UtcNow;
    //    public DateTimeOffset CheckOutTime { get; set; } = DateTimeOffset.UtcNow;
    //    public double? Lat { get; set; }
    //    public double? Lng { get; set; }
    //    public string ImgURlCheckIn { get; set; } = string.Empty;
    //    public double? CheckOutLat { get; set; }
    //    public double? CheckOutLng { get; set; }
    //    public string ImgURlCheckOut { get; set; } = string.Empty;

    //}
    public enum AttendanceCase
    {
        No_Time_Entries_Logged = 0,
        Week_Off = 1,
        Holiday = 2,
        On_Leave = 3,
        Log = 4,
    }
    public enum DailyCase
    {
        Absent = 0,
        Present_BAR_Missing_Swipe = 1,
        Present = 2,
    }

}