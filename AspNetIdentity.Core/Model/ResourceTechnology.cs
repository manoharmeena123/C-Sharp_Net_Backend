namespace AspNetIdentity.WebApi.Model
{
    public class ResourceTechnology
    {
        public int ResourceTechnologyId { get; set; }
        public int ResourceID { get; set; }
        public int Technology { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}