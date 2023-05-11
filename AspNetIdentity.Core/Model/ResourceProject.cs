namespace AspNetIdentity.WebApi.Model
{
    public class ResourceProject
    {
        public int ResourceProjectId { get; set; }
        public int ResourceID { get; set; }
        public int Project { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}