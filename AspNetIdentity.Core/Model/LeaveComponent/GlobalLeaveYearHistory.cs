using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.LeaveComponent
{
    public class GlobalLeaveYearHistory : DefaultFields
    {
        [Key]
        public int Id { get; set; }

        public DateTime StartMonthYear { get; set; }
        public DateTime EndMonthYear { get; set; }
        public string Duration { get; set; }
        public bool CurrentActive { get; set; }
    }
}