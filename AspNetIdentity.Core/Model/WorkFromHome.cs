using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model
{
    public class WorkFromHome : DefaultFields
    {
        [Key]
        public int WFHId { get; set; }

        public int EmployeeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string NumberOfDays { get; set; }
        public string Comment { get; set; }
        public string ReasonToReject { get; set; }
        public string AppliedBy { get; set; }
        public bool IsApproved { get; set; }
        public string WFHStatus { get; set; }

        [NotMapped]
        public List<WfhNofifyByEmployee> NotifyEmployees { get; set; }
    }
}