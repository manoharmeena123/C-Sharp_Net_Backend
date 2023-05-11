using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class ResourceCompany
    {
        [Key]
        public int CompanyId { get; set; }

        public string CompanyName { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public int OrgId { get; set; }
    }
}