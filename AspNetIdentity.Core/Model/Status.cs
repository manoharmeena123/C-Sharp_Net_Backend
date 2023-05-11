namespace AspNetIdentity.WebApi.Model
{
    public class Status
    {
        public int statusId { get; set; }
        public string StatusType { get; set; }
        public string StatusVal { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}