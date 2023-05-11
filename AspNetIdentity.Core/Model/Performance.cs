using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class Performance
    {
        [Key]
        public int FeedbackId { get; set; }

        public int HR { get; set; }

        public int Employee { get; set; }

        public int ProjectManager { get; set; }

        public int ProjectName { get; set; }
        public string Password { get; set; }
        public string HashPassword { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}