using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model
{
    public class MenuItem
    {
        [Key]
        public int MenuId { get; set; }

        public string Tittle { get; set; }
        public string MenuIcon { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}