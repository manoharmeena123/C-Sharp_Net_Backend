using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.Reviews
{
    public class EmployeeReview : BaseModelClass
    {
        [Key]

        public Guid ReviewEmployeeId { get; set; } = Guid.NewGuid();

        public Guid ReviewGroupId { get; set; } = Guid.Empty;
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int EmployeeNumber { get; set; }
        public string ReportingManager { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string BusinessUnit { get; set; }
        public string PayGrade { get; set; }
        public DateTime? JoiningDate { get; set; }
    }
}