using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetIdentity.WebApi.Model
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleType { get; set; }
        public int CategoryTypeId { get; set; }

        [NotMapped]
        public DateTime CreatedDate { get; set; }

        // public int CategoryId { get; set; }
        public string LoginRoleType { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
        public int OrgId { get; set; }
    }
}