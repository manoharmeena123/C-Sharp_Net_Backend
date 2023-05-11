using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.ShiftModel
{
    /// <summary>
    /// Created By Harshit Mitra On 29-09-2022
    /// </summary>
    public class ShiftTiming : BaseModelClass
    {
        [Key]
        public Guid ShiftTimingId { get; set; } = Guid.NewGuid();
        public virtual ShiftGroup ShiftGroup { get; set; }
        public DayOfWeek WeekDay { get; set; }
        public string WeekName { get; set; } = String.Empty;
        public TimeSpan StartTime { get; set; } = new TimeSpan(09, 00, 00);
        public TimeSpan EndTime { get; set; } = new TimeSpan(19, 00, 00);
        public int BreakTime { get; set; } = 60;
    }
}