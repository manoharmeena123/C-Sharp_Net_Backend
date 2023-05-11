using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class ProjectTechnology
    {
        [Key]
        public int ProjectTechId { get; set; }

        public int ProjectID { get; set; }
        public List<int> TechnologyId { get; set; }
        public int TechnologyID { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}