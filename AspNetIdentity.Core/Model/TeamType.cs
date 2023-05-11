using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model
{
    public class TeamType
    {
        [Key]
        public int TeamTypeId { get; set; }

        public int TeamLeadId { get; set; }
        public string TeamName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        [NotMapped]
        public List<EmployeeMiniModel> TeamMemberArray { get; set; }

        public string TeamLeadName { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
    public class EmployeeMiniModel
    {
        public int EmployeeMiniModelId { get; set; }
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public int RoleId { get; set; }
        public string RoleType { get; set; }
    }
}