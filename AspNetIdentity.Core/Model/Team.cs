using System;
using System.Collections.Generic;

namespace AspNetIdentity.WebApi.Model
{
    public class Team
    {
        public int TeamId { get; set; }
        public int ProjectLeadId { get; set; }
        public int ProjectId { get; set; }
        public List<int> EmployeeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string UitilizationPercentage { get; set; }

        public int TeamMemberId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}