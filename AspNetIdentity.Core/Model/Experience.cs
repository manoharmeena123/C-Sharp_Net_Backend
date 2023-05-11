using System;

namespace AspNetIdentity.WebApi.Model
{
    public class Experience : DefaultFields
    {
        public int ExperienceId { get; set; }
        public int EmployeeId { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string LastDrawnSalary { get; set; }
        public string CompanyName { get; set; }
        public string TotalYearOfExperience { get; set; }
        public string ReasonToLeave { get; set; }
    }
}