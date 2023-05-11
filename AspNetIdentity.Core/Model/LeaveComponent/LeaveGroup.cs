using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model.Leave
{
    public class LeaveGroup : DefaultFields
    {
        [Key]
        public int LeaveGroupId { get; set; }

        public string GroupName { get; set; }
        public string Description { get; set; }

        [NotMapped]
        public int EmployeeCount { get; set; }

        public bool IsDateSet { get; set; }
        public DateTime LeavePolicyStartDate { get; set; }
        public string StartMonth { get; set; }
        public DateTime LeavePolicyEndingDate { get; set; }
        public string EndMonth { get; set; }
    }
}