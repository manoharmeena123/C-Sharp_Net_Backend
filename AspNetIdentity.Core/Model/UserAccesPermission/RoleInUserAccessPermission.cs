using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.WebApi.Model.UserAccesPermission
{
    public class RoleInUserAccessPermission : BaseModelClass
    {
        [Key]
        public Guid RoleId { get; set; } = Guid.NewGuid();
        public string RoleName { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public bool HeadRoleInCompany { get; set; } = false;
    }
}