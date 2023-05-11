namespace AspNetIdentity.WebApi.Model
{
    public class EmployeeType
    {
        public int EmployeeTypeId { get; set; }
        public string EmployeeTypes { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}