namespace AspNetIdentity.WebApi.Model
{
    public class ResourcePM
    {
        public int ResourcePMId { get; set; }
        public int ResourceID { get; set; }
        public int ProjectManager { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}