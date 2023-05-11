using System;

namespace AspNetIdentity.WebApi.Model
{
    public class Resource
    {
        public int ResourceId { get; set; }
        public string Name { get; set; }
        public int Designation { get; set; }
        public int ProjectManager { get; set; }
        public string TechLead { get; set; }
        public int Technology { get; set; }
        public int Project { get; set; }
        public string BillType { get; set; }
        public int CompanyName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}