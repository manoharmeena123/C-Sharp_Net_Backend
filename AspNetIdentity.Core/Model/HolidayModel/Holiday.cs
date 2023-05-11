using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model
{
    public class Holiday : BaseModelClass
    {
        [Key]
        public long HolidayId { get; set; }

        public string HolidayName { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public bool IsFloaterOptional { get; set; } = false;
        public string ImageUrl { get; set; } = String.Empty;
        public string TextColor { get; set; } = String.Empty;
        public DateTimeOffset HolidayDate { get; set; } = DateTimeOffset.UtcNow;

        [NotMapped]
        public string MonthName { get; set; }
        [NotMapped]
        public string StartDate { get; set; }
        [NotMapped]
        public string DayName { get; set; }

        public bool IsCompleted { get; set; } = false;
    }
}