using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.New_Pay_Roll
{
    /// <summary>
    /// Created By Harshit Mitra on 16/12/2022
    /// </summary>
    public class GeneralPayrollSetting : BaseModelClass
    {
        [Key]
        public Guid GernalSettingId { get; set; } = Guid.NewGuid();

        public Guid PayGroupId { get; set; } = Guid.Empty;
        public string PayFrequency { get; set; } = String.Empty;
        public string PayCycleForHRMS { get; set; } = String.Empty;
        public string StartPeriod { get; set; } = String.Empty;
        public string EndPeriod { get; set; } = String.Empty;
        public int TotalPayDays { get; set; } = 0;
        public bool ExcludeWeeklyOffs { get; set; } = false;
        public bool ExcludeHolidays { get; set; } = false;
        public string CurrencyId { get; set; } = String.Empty;
        public string CurrencyName { get; set; } = String.Empty;
        public bool RemunerationMonthly { get; set; } = false;
        public bool RemunerationDaily { get; set; } = false;
    }
}