using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.ShiftModel
{
    /// <summary>
    /// Created By Harshit Mitra On 07-10-2022
    /// </summary>
    public class WeekOffDaysCases : BaseModelClass
    {
        [Key]
        public Guid WeekOffCaseId { get; set; } = Guid.NewGuid();
        public virtual WeekOffDaysGroup Group { get; set; }
        public DayOfWeek DayId { get; set; }
        public WeekOffCase CaseId { get; set; }
        public WeekOffDayConstants CaseResponseId { get; set; }
    }
    /// <summary>
    /// Created By Harshit Mitra On 10-10-2022
    /// </summary>
    public enum WeekOffCase
    {
        All_Week = 0,
        First = 1,
        Second = 2,
        Third = 3,
        Fourth = 4,
        Fifth = 5,
        Last = 6,
    }
    /// <summary>
    /// Created By Ankit Jain On 01-10-2022
    /// </summary>
    public enum WeekOffDayConstants
    {
        Not_Set = 0,
        Full_Day_Weekly_Off = 1,
        First_Half_Weekly_Off = 2,
        Second_Half_Weekly_Off = 3,
    }
}