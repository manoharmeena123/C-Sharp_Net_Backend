namespace AspNetIdentity.WebApi.Model
{
    public class ProjectType
    {
        public int ProjectTypeId { get; set; }
        public string Project_Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}