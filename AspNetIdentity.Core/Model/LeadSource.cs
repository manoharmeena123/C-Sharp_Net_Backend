namespace AspNetIdentity.WebApi.Model
{
    public class LeadSource
    {
        public int LeadSourceId { get; set; }
        public string SourceName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}