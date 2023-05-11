using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public string AppliedBy { get; set; }
        public DateTime? Date { get; set; }
        public string ClockIn { get; set; }
        public string ClockOut { get; set; }
        public int TotalHours { get; set; }
        public int TotalMinute { get; set; }
        public string Comment { get; set; }
        public bool IsClock { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}