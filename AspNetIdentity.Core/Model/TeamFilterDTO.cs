using System;

namespace AspNetIdentity.WebApi.Model
{
    public class TeamFilterDTO
    {
        public int EmployeeId { get; set; }

        public DateTime CreatedDate { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}