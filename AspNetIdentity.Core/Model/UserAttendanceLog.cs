using System;
using System.ComponentModel.DataAnnotations;
using static AspNetIdentity.WebApi.Model.EnumClass;

namespace AspNetIdentity.WebApi.Model
{
    /// <summary>
    /// Created By Harshit Mitra on 01-03-2022
    /// </summary>
    public class UserAttendanceLog : DefaultFields
    {
        [Key]
        public int LogId { get; set; }
        public int EmployeeId { get; set; }
        public string UserName { get; set; }
        public bool IsCheckIn { get; set; }
        public DateTimeOffset ClockInTime { get; set; }
        public bool IsClockin { get; set; }
        public DateTimeOffset? ClockOutTime { get; set; }
        public bool IsClockOut { get; set; }
        public string TotalTime { get; set; }
        public TimeSpan TotalTimeDate { get; set; }
        public AttendenceArrivalStatusConstants ArrivalStatus { get; set; }
        public Guid ShiftTimingId { get; set; }

    }
}