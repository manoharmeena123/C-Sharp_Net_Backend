using System.Collections.Generic;

namespace AspNetIdentity.WebApi.Model
{
    public class FeedbackFilter
    {
        public int RoleId { get; set; }
        public int CategoryId { get; set; }
        public List<Employee> Employees { get; set; }
        public int CategoryTypeId { get; set; } ////
        public int RatedByEmpId { get; set; } ////
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}